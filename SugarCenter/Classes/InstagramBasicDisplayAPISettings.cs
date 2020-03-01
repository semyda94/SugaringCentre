using Microsoft.Extensions.Configuration;
using SugarCenter.Interfaces;

namespace SugarCenter.Classes
{
    public class InstagramBasicDisplayAPISettings : IInstagramBasicDisplayAPISettings
    {
        public long AppId { get; set; }
        public string AppSecret { get; set; }
        public string InstagramRedirectUrl { get; set; }
        public string ApiBaseUrl { get; set; }
        public string Scope { get; set; }
    }
}