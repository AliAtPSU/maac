using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;

namespace maac
{
    public sealed class TwitterConnector:SocialServiceConnector
    {

        const string OAUTH_CONSUMER_KEY = "pVzGTy1Pgu9yMiurxyoL3eHOH";
        const string OAUTH_CONSUMER_SECRET = "pqkz9fImaM3U4Tv7BWL4z5JhWpnhKooa95jOzvgvzWzmo4GVcA";
        //Token URL
        const string oauth_url = "https://api.twitter.com/oauth2/token";

        public TwitterConnector()
        {


        }

        public override async Task<TweetsCollection> search(string searchTerm,long? id,bool? searchOlder)
        {
            TwitAuthenticateResponse authentication = await generateAuthentication();
            var searchResultsToDisplay = await fetchResults(authentication, searchTerm,id,searchOlder);
            return searchResultsToDisplay;
        }

        protected override async Task<TweetsCollection> fetchResults(TwitAuthenticateResponse authentication, string searchTerm,long? id, bool? searchOlder)
        {
            var httpClient = new HttpClient();
            StringBuilder baseAddress = buildWebAddressString(searchTerm, id, searchOlder);
            HttpRequestMessage requestResults = new HttpRequestMessage(HttpMethod.Get, baseAddress.ToString());
            requestResults.Headers.Add("Authorization", authentication.token_type + " " + authentication.access_token);
            var response = await httpClient.SendAsync(requestResults);
            string resultsAsJson = await response.Content.ReadAsStringAsync();
            var deserializedResults = deserializeObject<TweetsCollection>(resultsAsJson);
            deserializedResults.biggestTweetId = deserializedResults.searchResults.Max(t => t.id);
            deserializedResults.smallestTweetId = deserializedResults.searchResults.Min(t => t.id);
            return deserializedResults;
        }

        protected override StringBuilder buildWebAddressString(string searchTerm, long? id, bool? searchOlder)
        {
            string encodedSearchTerm = WebUtility.HtmlEncode(searchTerm);
            StringBuilder baseAddress = new StringBuilder(String.Format("https://api.twitter.com/1.1/search/tweets.json?q={0}&count=50", encodedSearchTerm));
            if (searchOlder != null)
            {
                if (searchOlder == true)
                {
                    baseAddress.Append("&max_id=" + (id - 1));
                }
                else
                {
                    baseAddress.Append("&since_id=" + (id - 1));
                }
            }

            return baseAddress;
        }

        protected override async Task<TwitAuthenticateResponse> generateAuthentication()
        {
            HttpResponseMessage response = await sendAuthenticationRequestAndReceiveResponse();
            var deserializedAuthentication = deserializeObject<TwitAuthenticateResponse>(await response.Content.ReadAsStringAsync());
            return deserializedAuthentication;
        }

        protected override async Task<HttpResponseMessage> sendAuthenticationRequestAndReceiveResponse()
        {
            string headerFormat = "Basic {0}";
            var authorizationHeader = string.Format(headerFormat,
                Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.EscapeDataString(OAUTH_CONSUMER_KEY) + ":" +
                Uri.EscapeDataString((OAUTH_CONSUMER_SECRET)))
            ));
            HttpClient client = new HttpClient();
            HttpRequestMessage authorizationRequest = new HttpRequestMessage(HttpMethod.Post, oauth_url);
            authorizationRequest.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
            //  request.Content.Headers.ContentType.MediaType = "application/x-www-form-urlencoded;charset=UTF-8";
            authorizationRequest.Headers.Add("Authorization", authorizationHeader);
            authorizationRequest.Headers.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
            HttpResponseMessage authenticationResponse = new HttpResponseMessage();
            authenticationResponse = await client.SendAsync(authorizationRequest);
            return authenticationResponse;
        }

        protected override Type deserializeObject<Type>(string json)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(Type));
            stream.Position = 0;
            Type returnObject = (Type)deserializer.ReadObject(stream);
            return returnObject;
        }

    }
}

