using System;

using eHesabim.Core.Engine;
using eHesabim.Core.Logging;
using eHesabim.Core.Tasks;
using eHesabim.Services;

namespace eHesabim.Tasks {
    public class EmailQueueSendTask : ITask {
        private readonly IMessageService messageService = EngineContext.Current.Resolve<IMessageService>();
        private readonly ILogging logging = EngineContext.Current.Resolve<ILogging>();

        public void Execute(DateTime? lastSuccess) {
            Console.WriteLine(@"{0:yyyy-MM-dd HH:mm:ss} EmailQueueSendTask started.", DateTime.Now);
            var emailQueues = messageService.GetEmailQueueList();

            foreach (var emailQueue in emailQueues) {
                try {
                    messageService.SendMail(emailQueue.Id);

                    logging.Info(string.Format("{0} adresine '{1}' konulu maili başarıyla gönderildi.", emailQueue.ToEmail, emailQueue.Subject));
                    emailQueue.SentDateTime = DateTime.Now;
                }
                catch (Exception exc) {
                    logging.Fatal(exc);
                }
                finally {
                    emailQueue.SentTries = emailQueue.SentTries + 1;
                    messageService.UpdateEmailQueue(emailQueue.Id, emailQueue.SentTries, emailQueue.SentDateTime);
                }
            }

            Console.WriteLine(@"{0:yyyy-MM-dd HH:mm:ss} EmailQueueSendTask ended.", DateTime.Now);
        }
    }
}
