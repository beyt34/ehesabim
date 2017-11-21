using System;
using eHesabim.Core.Engine;
using eHesabim.Services;

namespace eHesabim.Console {
    public static class EmailQueueSender {
        public static void Send() {
            System.Console.WriteLine("Hello World!");

            try {
                var messageService = EngineContext.Current.Resolve<IMessageService>();
                var emailQueues = messageService.GetEmailQueueList();
                System.Console.WriteLine("emails count:{0}", emailQueues.Count);
            }
            catch (Exception e) {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
