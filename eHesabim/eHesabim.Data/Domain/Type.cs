namespace eHesabim.Data.Domain {
    public class Type : BaseEntity<int> {
        public int? ParentId { get; set; }

        public string Name { get; set; }

        public int? Order { get; set; }
    }
}
