namespace eHesabim.Data.Domain {
    public class EmailTemplate : BaseEntity<int> {
        public string Name { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public int Priority { get; set; }

        public int EmailAccountId { get; set; }

        public virtual EmailAccount EmailAccount { get; set; }
    }
}
