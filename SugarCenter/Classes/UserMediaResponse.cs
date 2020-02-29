using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SugarCenter.Classes
{
    public class UserMediaResponse
    {
        [JsonProperty("data")]
        public List<UserMedia> UserMedia { get; set; }
    }
}