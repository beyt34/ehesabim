using Newtonsoft.Json;

namespace eHesabim.Framework.Facebook {
    public class Person {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string FullName { get; set; }
    }
}
