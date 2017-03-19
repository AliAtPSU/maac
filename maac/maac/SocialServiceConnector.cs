using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace maac
{
    public abstract class SocialServiceConnector
    {
        public abstract Task<TweetsCollection> search(string searchTerm, long? id, bool? searchOlder);
        protected abstract Task<TweetsCollection> fetchResults(TwitAuthenticateResponse authentication, string searchTerm, long? id, bool? searchOlder);
        protected abstract StringBuilder buildWebAddressString(string searchTerm, long? id, bool? searchOlder);
        protected abstract Task<TwitAuthenticateResponse> generateAuthentication();
        protected abstract Task<HttpResponseMessage> sendAuthenticationRequestAndReceiveResponse();
        protected abstract Type deserializeObject<Type>(string json);
    };
}
