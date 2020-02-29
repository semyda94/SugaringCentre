using System.Web.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SugarCenter.Classes
{
    public class UserData
    {
        [JsonProperty("username")]
        public string Username;
    }
}