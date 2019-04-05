using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using eHesabim.Core.Data;
using eHesabim.Core.Token;
using eHesabim.Data.Domain;
using eHesabim.Services.Models;

namespace eHesabim.Services {
    public class MessageService : IMessageService {
        private readonly IRepository<EmailAccount, int> emailAccountRepository;
        private readonly IRepository<EmailQueue, Guid> emailQueueRepository;
        private readonly IRepository<EmailTemplate, int> emailTemplateRepository;
        private readonly ITokenizer tokenizer;

        public MessageService(IRepository<EmailAccount, int> emailAccountRepository, IRepository<EmailQueue, Guid> emailQueueRepository, IRepository<EmailTemplate, int> emailTemplateRepository, ITokenizer tokenizer) {
            this.emailAccountRepository = emailAccountRepository;
            this.emailQueueRepository = emailQueueRepository;
            this.emailTemplateRepository = emailTemplateRepository;
            this.tokenizer = tokenizer;
        }

        public Guid AddUpdateEmailQueue(Guid id, int priority, string toemail, string toname, string subject, string body, string cc, string bcc, int sentTries, int emailAccountId) {
            var entity = id == Guid.Empty ? new EmailQueue() : emailQueueRepository.Detail(id);
            if (entity != null) {
                entity.Priority = priority;
                entity.ToEmail = !string.IsNullOrEmpty(toemail) ? toemail.Trim() : string.Empty;
                entity.ToName = !string.IsNullOrEmpty(toname) ? toname.Trim() : string.Empty;
                entity.Subject = !string.IsNullOrEmpty(subject) ? subject.Trim() : string.Empty;
                entity.Body = body;
                entity.Cc = !string.IsNullOrEmpty(cc) ? cc.Trim() : string.Empty;
                entity.Bcc = !string.IsNullOrEmpty(bcc) ? bcc.Trim() : string.Empty;
                entity.SentTries = sentTries;
                entity.EmailAccountId = emailAccountId;

                var emailAccount = emailAccountRepository.Detail(emailAccountId);
                if (emailAccount != null) {
                    entity.FromEmail = emailAccount.Email;
                    entity.FromName = emailAccount.DisplayName;
                }

                return emailQueueRepository.AddUpdate(entity);
            }

            return Guid.Empty;
        }

        public void UpdateEmailQueue(Guid id, int sentTries, DateTime? sendDateTime) {
            var data = emailQueueRepository.Detail(id);
            if (data != null) {
                data.SentTries = sentTries;
                data.SentDateTime = sendDateTime;
                emailQueueRepository.AddUpdate(data);
            }
        }

        public List<EmailQueueDataModel> GetEmailQueueList() {
            var list = emailQueueRepository
                        .Query(a => !a.SentDateTime.HasValue && a.SentTries <= 3)
                        .ToList();

            return AutoMapperConfiguration.Mapper.Map<List<EmailQueue>, List<EmailQueueDataModel>>(list);
        }

        public bool SendUserPasswordRecoveryMail(string firstName, string email, Guid passwordGuid) {
            var emailTemplate = GetEmailTemplate("User.PasswordRecovery");
            if (emailTemplate == null) {
                return false;
            }

            var tokens = new List<Token> {
                    new Token("PasswordRecovery.FirstName", firstName),
                    new Token("PasswordRecovery.RecoveryURL", string.Format("{0}/sifre-belirle/{1}", ConfigurationManager.AppSettings["PortalPath"], passwordGuid)),
                    new Token("PortalPath", ConfigurationManager.AppSettings["PortalPath"])
                };

            return SendEmailNotification(emailTemplate, tokens, email, firstName);
        }

        public bool SendUserRegisterMail(string firstName, string email) {
            var emailTemplate = GetEmailTemplate("User.Register");
            if (emailTemplate == null) {
                return false;
            }

            var tokens = new List<Token> {
                    new Token("Register.FirstName", firstName),
                    new Token("PortalPath", ConfigurationManager.AppSettings["PortalPath"])
                };

            return SendEmailNotification(emailTemplate, tokens, email, firstName);
        }

        public bool SendMail(Guid id) {
            var data = emailQueueRepository.Query(a => a.Id == id).Include(i => i.EmailAccount).FirstOrDefault();
            if (data != null) {
                var bcc = string.IsNullOrWhiteSpace(data.Bcc) ? null : data.Bcc.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var cc = string.IsNullOrWhiteSpace(data.Cc) ? null : data.Cc.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                SendEmail(
                    data.EmailAccount,
                    data.Subject,
                    data.Body,
                    new MailAddress(data.FromEmail, data.FromName),
                    new MailAddress(data.ToEmail, data.ToName),
                    bcc,
                    cc,
                    null,
                    string.Empty);
            }

            return false;
        }

        private void SendEmail(EmailAccount emailAccount, string subject, string body, MailAddress @from, MailAddress to, IEnumerable<string> bcc, IEnumerable<string> cc, byte[] attachment, string attachmentName) {
            var message = new MailMessage { From = @from };
            message.To.Add(to);
            if (bcc != null) {
                foreach (var address in bcc.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue))) {
                    message.Bcc.Add(address.Trim());
                }
            }
            else {
                message.Bcc.Add("beyt34@gmail.com");
            }

            if (cc != null) {
                foreach (var address in cc.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue))) {
                    message.CC.Add(address.Trim());
                }
            }

            if (attachment != null && attachment.Length > 0) {
                var memoryStream = new MemoryStream(attachment);
                message.Attachments.Add(new Attachment(memoryStream, attachmentName));
            }

            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtpClient = new SmtpClient()) {
                smtpClient.UseDefaultCredentials = emailAccount.UseDefaultCredentials;
                smtpClient.Host = emailAccount.Host;
                smtpClient.Port = emailAccount.Port;
                smtpClient.EnableSsl = emailAccount.EnableSsl;
                smtpClient.Credentials = emailAccount.UseDefaultCredentials
                    ? CredentialCache.DefaultNetworkCredentials
                    : new NetworkCredential(emailAccount.Username, emailAccount.Password);

                smtpClient.Send(message);
            }
        }

        private EmailTemplate GetEmailTemplate(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("messageTemplateName");
            }

            return emailTemplateRepository
                    .Query(a => a.Name == name)
                    .Include(i => i.EmailAccount)
                    .FirstOrDefault();
        }

        private bool SendEmailNotification(EmailTemplate emailTemplate, IEnumerable<Token> tokens, string toemailAddress, string toname) {
            var subject = emailTemplate.Subject;
            var masterTemplate = GetEmailTemplate("Mail.Template.Master");
            var body = masterTemplate.Body.Replace("%Common.MailContent%", emailTemplate.Body);
            body = body.Replace("%PortalPath%", ConfigurationManager.AppSettings["PortalPath"]);

            // Replace subject and body tokens 
            var token = tokens as Token[] ?? tokens.ToArray();
            var subjectReplaced = tokenizer.Replace(subject, token, false);
            var bodyReplaced = tokenizer.Replace(body, token, false);

            var queueGuid = AddUpdateEmailQueue(
                Guid.Empty,
                emailTemplate.Priority,
                toemailAddress,
                toname,
                subjectReplaced,
                bodyReplaced,
                string.Empty,
                string.Empty,
                0,
                emailTemplate.EmailAccountId);
            return queueGuid != Guid.Empty;
        }
    }
}
