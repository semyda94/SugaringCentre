using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SugarCenter.Classes;

namespace SugarCenter.Helpers
{
    public static class InstagramBasicDisplayAPI
    {
        private static HttpClient _client = new HttpClient();
        
        private static string appId = "2561182167502522";
        private static string appSecret = "8e398dbb198eaa66c4d4a2539159d40e";
        private static string instagrammRedirectUri = "https://localhost:5001/Blog/OAuth";
        private static string apiBaseURL = "https://api.instagram.com/oauth/authorize";
        private static string scope = "user_profile,user_media";

        public static readonly string AutorisationUrl = $"{apiBaseURL}?client_id={appId}&redirect_uri={instagrammRedirectUri}&scope={scope}&response_type=code";
        
        public static async Task<oAuthResponseWithToken> GetToken(string code)
        {
            const string tokenUrl = "https://api.instagram.com/oauth/access_token";
            
            var requestContent = new FormUrlEncodedContent(new [] {
                new KeyValuePair<string, string>("client_id", appId),
                new KeyValuePair<string, string>("client_secret", appSecret),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("redirect_uri", instagrammRedirectUri),
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