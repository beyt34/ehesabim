using System;
using System.ServiceProcess;

namespace eHesabim.Console {
    public class Program {
        public const string ServiceName = "eHesabim.Service";
        
        public static void Main(string[] args) {
            if (Environment.UserInteractive) {
                // running as console app
                Start(args);

                System.Console.WriteLine(@"Press any key to stop...");
                System.Console.ReadKey(true);

                Stop();
            }
            else {
                // running as service
                using (var service = new Service1()) {
                    ServiceBase.Run(service);
                }
            }
        }

        public static void Start(string[] args) {
            ////File.AppendAllText(@"c:\temp\MyService.txt", string.Format("{0:yyyy-MM-dd HH:mm:ss} started{1}", DateTime.Now, Environment.NewLine));

            System.Console.WriteLine(@"{0:yyyy-MM-dd HH:mm:ss} started", DateTime.Now);

            // 60 seconds  
            var timer = new System.Timers.Timer { Interval = 10000 };
            timer.Elapsed += OnTimer;
            timer.Start();
        }

        public static void Stop() {
            System.Console.WriteLine(@"{0:yyyy-MM-dd HH:mm:ss} stopped", DateTime.Now);
            ////File.AppendAllText(@"c:\temp\MyService.txt", string.Format("{0:yyyy-MM-dd HH:mm:ss} stopped{1}", DateTime.Now, Environment.NewLine));
        }

        public static void OnTimer(object sender, System.Timers.ElapsedEventArgs args) {
            System.Console.WriteLine(@"{0:yyyy-MM-dd HH:mm:ss} timer", DateTime.Now);

            EmailQueueSender.Send();

            ////var emailQueues = messageService.GetEmailQueueList();
            ////foreach (var emailQueue in emailQueues) {
            ////    try {
            ////        ////messageService.SendMail(emailQueue.Id);

            ////        logging.Info(string.Format("{0} adresine '{1}' konulu maili başarıyla gönderildi.", emailQueue.ToEmail, emailQueue.Subject));
            ////        emailQueue.SentDateTime = DateTime.Now;
            ////    }
            ////    catch (Exception exc) {
            ////        logging.Fatal(exc);
            ////    }
            ////    finally {
            ////        ////emailQueue.SentTries = emailQueue.SentTries + 1;
            ////        ////messageService.UpdateEmailQueue(emailQueue.Id, emailQueue.SentTries, emailQueue.SentDateTime);
            ////    }
            ////}

            ////File.AppendAllText(@"c:\temp\MyService.txt", string.Format("{0:yyyy-MM-dd HH:mm:ss} timer{1}", DateTime.Now, Environment.NewLine));
        }
    }
}
