using System;
using System.Collections.Generic;

using eHesabim.Services.Models;

namespace eHesabim.Services {
    public interface IMessageService {
        Guid AddUpdateEmailQueue(Guid id, int priority, string toemail, string toname, string subject, string body, string cc, string bcc, int sentTries, int emailAccountId);

        void UpdateEmailQueue(Guid id, int sentTries, DateTime? sendDateTime);

        List<EmailQueueDataModel> GetEmailQueueList();

        bool SendUserPasswordRecoveryMail(string firstName, string email, Guid passwordGuid);

        bool SendUserRegisterMail(string firstName, string email);

        bool SendMail(Guid id);
    }
}