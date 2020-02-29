using System;
using Newtonsoft.Json;

namespace SugarCenter.Classes
{
    public class UserMedia
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("media_type")]
        public string MediaType { get; set; }
        [JsonProperty("media_url")] 
        public string MediaUrl { get; set; }
        [JsonProperty("caption")]
        public string Caption { get; set; }
        [JsonProperty("timestamp")] 
        public DateTime DateTime { get; set; }
    }
}