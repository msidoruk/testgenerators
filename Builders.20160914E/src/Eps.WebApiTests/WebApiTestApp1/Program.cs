using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApiTestApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:4860/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            //
            // CallWebAPI(httpClient);

            Tests(httpClient);
        }

        private static async void CallWebAPI(HttpClient httpClient)
        {
            string stringContent = "grant_type=password&username=Test1&password=111";
            HttpContent httpContent = new StringContent(stringContent);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            Task<HttpResponseMessage> res = httpClient.PostAsync("/token", httpContent);
            HttpResponseMessage httpResponseMessage = res.Result;
            httpResponseMessage.EnsureSuccessStatusCode();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                dynamic responseJson = JsonConvert.DeserializeObject(responseContent);
                //
                int bp = 0;
            }
        }

        private static async void Tests(HttpClient httpClient)
        {
            dynamic getTokenResult = await Post(httpClient, "/token", RequestsUrlsHelper.Token("password", "UserManager", "111"));
            if (getTokenResult != null)
            {
                string accessToken = getTokenResult.access_token;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                string userName = "NewUser";
                string password = "112";

                dynamic userJObject = new JObject();
                userJObject.UserName = userName;
                userJObject.Password = password;
                dynamic createUserResult = await PostJson(httpClient, "testApi/users", userJObject);

                dynamic users = await Get(httpClient, "api/Account/Users", string.Empty);

                dynamic roles = await Get(httpClient, "api/Account/Roles", string.Empty);

                dynamic createdUser = await PostJson(httpClient, "api/Account/User", userJObject);
                dynamic createdUserAsJObject = new JObject(createdUser);

                dynamic retrievedUser1010 = await Get(httpClient, $"api/Account/User", "?id=1010");

                string createdObjectId = createdUserAsJObject.id;
                dynamic retrievedUser = await Get(httpClient, $"api/Account/User", $"?id={createdObjectId}");

                userJObject.Id = createdObjectId;
                userJObject.UserName = userJObject.UserName + "_Updated";
                userJObject.Roles = new JArray();
                userJObject.Roles.Add(roles[0]);
                dynamic updatedUser = await PostJson(httpClient, "api/Account/User", userJObject);

                dynamic retrievedUserAfterUpdate = await Get(httpClient, $"api/Account/User", $"?id={createdObjectId}");

                dynamic userSecuritySettings = await Get(httpClient, $"api/Account/UserSecuritySettings", string.Empty);

                int bp = 0;
            }
        }

        private static async Task<dynamic> Post(HttpClient httpClient, string url, string requestContent, string accessToken = null)
        {
            HttpContent httpContent = new StringContent(requestContent, Encoding.UTF8, "application/json");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            Task<HttpResponseMessage> res = httpClient.PostAsync(url, httpContent);
            HttpResponseMessage httpResponseMessage = res.Result;
            httpResponseMessage.EnsureSuccessStatusCode();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                dynamic responseJson = JsonConvert.DeserializeObject(responseContent);
                return responseJson;
            }
            return null;
        }

        private static async Task<dynamic> PostJson(HttpClient httpClient, string url, dynamic jsonToPost, string accessToken = null)
        {
            string requestContent = JsonConvert.SerializeObject(jsonToPost);
            HttpContent httpContent = new StringContent(requestContent, Encoding.UTF8, "application/json");
            Task<HttpResponseMessage> res = httpClient.PostAsync(url, httpContent);
            HttpResponseMessage httpResponseMessage = res.Result;
            httpResponseMessage.EnsureSuccessStatusCode();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                dynamic responseJson = JsonConvert.DeserializeObject(responseContent);
                return responseJson;
            }
            return null;
        }

        private static async Task<dynamic> Get(HttpClient httpClient, string url, string requestContent)
        {
            HttpContent httpContent = new StringContent(requestContent);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            Task<HttpResponseMessage> res = httpClient.GetAsync(url + requestContent);
            HttpResponseMessage httpResponseMessage = res.Result;
            httpResponseMessage.EnsureSuccessStatusCode();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                dynamic responseJson = JsonConvert.DeserializeObject(responseContent);
                return responseJson;
            }
            return null;
        }
    }

    internal class RequestsUrlsHelper
    {
        public static string Token(string grantType, string userName, string password)
        {
            return $"grant_type={grantType}&username={Uri.EscapeUriString(userName)}&password={Uri.EscapeUriString(password)}";
        }
    }
}
