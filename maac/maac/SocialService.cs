using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.IO;

using System.Runtime.Serialization.Json;

namespace maac
{
    public class SocialService
    {
        HttpClient client = new HttpClient();
        const string OAUTH_CONSUMER_KEY = "pVzGTy1Pgu9yMiurxyoL3eHOH";
        const string OAUTH_CONSUMER_SECRET = "pqkz9fImaM3U4Tv7BWL4z5JhWpnhKooa95jOzvgvzWzmo4GVcA";
        //Token URL
        const string oauth_url = "https://api.twitter.com/oauth2/token";
        HttpRequestMessage authorizationRequest = new HttpRequestMessage(HttpMethod.Post, oauth_url);
        public SocialService()
        {
            string headerFormat = "Basic {0}";
            var authorizationHeader = string.Format(headerFormat,
                Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.EscapeDataString(OAUTH_CONSUMER_KEY) + ":" +
                Uri.EscapeDataString((OAUTH_CONSUMER_SECRET)))
            ));
            authorizationRequest.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
            //  request.Content.Headers.ContentType.MediaType = "application/x-www-form-urlencoded;charset=UTF-8";
            authorizationRequest.Headers.Add("Authorization", authorizationHeader);
            authorizationRequest.Headers.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
        }

        public async Task<TweetsCollection> search(string searchTerm)
        {
            TwitAuthenticateResponse authentication = await generateAuthentication();
            var searchResultsToDisplay = await fetchResults(authentication, searchTerm);
            return searchResultsToDisplay;
        }

        private async Task<TweetsCollection> fetchResults(TwitAuthenticateResponse authentication, string searchTerm)
        {
            var httpClient = new HttpClient();
            const string baseAddress = "https://api.twitter.com/1.1/search/tweets.json?q=";
            string encodedSearchTerm = WebUtility.HtmlEncode(searchTerm);
            HttpRequestMessage requestResults = new HttpRequestMessage(HttpMethod.Get, baseAddress + encodedSearchTerm);
            requestResults.Headers.Add("Authorization", authentication.token_type + " " + authentication.access_token);
            var response = await httpClient.SendAsync(requestResults);
            string resultsAsJson = await response.Content.ReadAsStringAsync();
            var deserializedResults = deserializeObject<TweetsCollection>(resultsAsJson);
            return deserializedResults;
        }

        private async Task<TwitAuthenticateResponse> generateAuthentication()
        {
            HttpResponseMessage response = await sendAuthenticationRequestAndReceiveResponse();
            var deserializedAuthentication = deserializeObject<TwitAuthenticateResponse>(await response.Content.ReadAsStringAsync());
            return deserializedAuthentication;
        }

        private async Task<HttpResponseMessage> sendAuthenticationRequestAndReceiveResponse()
        {
            HttpResponseMessage authenticationResponse = new HttpResponseMessage();
            authenticationResponse = await client.SendAsync(authorizationRequest);
            string content = await authenticationResponse.Content.ReadAsStringAsync();
            return authenticationResponse;
        }

        private static Type deserializeObject<Type>(string json)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(Type));
            stream.Position = 0;
            Type returnObject = (Type)deserializer.ReadObject(stream);
            return returnObject;
        }

    }
}

