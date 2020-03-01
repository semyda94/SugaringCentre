using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SugarCenter.Classes;

namespace SugarCenter.Helpers
{
    public static class InstagramBasicDisplayAPIHelper
    {
        private static HttpClient _client = new HttpClient();
        
        private static string appId = "";
        private static string appSecret = "";
        private static string instagramRedirectUrl = "";
        private static string apiBaseURL = "";
        private static string scope = "";

        public static string AppId { set => appId = value; }
        public static string AppSecret { set => appSecret = value; }
        public static string InstagramRedirectUrl { set => instagramRedirectUrl = value; }
        public static string ApiBaseUrl { set => apiBaseURL = value; }
        public static string Scope { set => scope = value; }

        public static string GetAuthorisationUrl()
        {
            return $"{apiBaseURL}?client_id={appId}&redirect_uri={instagramRedirectUrl}&scope={scope}&response_type=code";
        }

        public static async Task<oAuthResponseWithToken> GetToken(string code)
        {
            const string tokenUrl = "https://api.instagram.com/oauth/access_token";
            
            var requestContent = new FormUrlEncodedContent(new [] {
                new KeyValuePair<string, string>("client_id", appId),
                new KeyValuePair<string, string>("client_secret", appSecret),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("redirect_uri", instagramRedirectUrl),
                new KeyValuePair<string, string>("code", code),
            });
            
            HttpResponseMessage response = await _client.PostAsync(
                tokenUrl, requestContent);
            
            HttpContent responseContent = response.Content;

            oAuthResponseWithToken deserializedResponse;
            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                deserializedResponse = JsonConvert.DeserializeObject<oAuthResponseWithToken>(await reader.ReadToEndAsync());
            }

            return deserializedResponse;
        }

        public static async Task<UserData> GetUserDate(oAuthResponseWithToken oAuthResponse)
        {
            string userDataUrl = $"https://graph.instagram.com/me?fields=username&access_token={oAuthResponse.access_token}";

            HttpResponseMessage response = await _client.GetAsync(
                userDataUrl);
            
            HttpContent responseContent = response.Content;
            
            UserData deserializedUserData;
            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                deserializedUserData = JsonConvert.DeserializeObject<UserData>(await reader.ReadToEndAsync());
            }

            return deserializedUserData;
        }

        public static async Task<UserMediaResponse> GetUserMedia(oAuthResponseWithToken oAuthResponse)
        {
            string userDataUrl = $"https://graph.instagram.com/me/media?fields=id,media_type,media_url,caption,timestamp&access_token={oAuthResponse.access_token}";

            HttpResponseMessage response = await _client.GetAsync(
                userDataUrl);
            
            HttpContent responseContent = response.Content;
            
            var deserializedUserMedia = new UserMediaResponse();
            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                var jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;

                var tmp = await reader.ReadToEndAsync();
                Console.WriteLine(tmp);
                deserializedUserMedia = JsonConvert.DeserializeObject<UserMediaResponse>(tmp, jsonSerializerSettings);
            }

            return  deserializedUserMedia;
        }
    }
}