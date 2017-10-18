namespace eHesabim.Data.Domain {
    public class County : BaseEntity<int> {
        public int CityId { get; set; }

        public virtual City City { get; set; }
        
        public string Name { get; set; }
    }
}
