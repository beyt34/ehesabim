namespace eHesabim.Data.Domain {
    public class City : BaseEntity<int> {
        public int Code { get; set; }

        public string Name { get; set; }

        public int? Order { get; set; }
    }
}
