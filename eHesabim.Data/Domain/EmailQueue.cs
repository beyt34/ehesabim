using System;

namespace eHesabim.Data.Domain {
    public class EmailQueue : BaseEntity<Guid> {
        public string FromEmail { get; set; }

        public string FromName { get; set; }

        public string ToEmail { get; set; }

        public string ToName { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public int Priority { get; set; }

        public int SentTries { get; set; }

        public DateTime? SentDateTime { get; set; }

        public int? EmailAccountId { get; set; }

        public virtual EmailAccount EmailAccount { get; set; }
    }
}
