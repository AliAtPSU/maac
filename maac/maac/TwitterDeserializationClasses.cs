using System.Collections.Generic;
using System.Runtime.Serialization;

namespace maac
{
    [DataContract]
    public class TwitAuthenticateResponse
    {
        [DataMember(Name = "token_type")]
        public string token_type { get; set; }
        [DataMember(Name = "access_token")]
        public string access_token { get; set; }
    }

    [DataContract]
    public class TweetsCollection
    {
        public long smallestTweetId { get; set; }
        public long biggestTweetId { get; set; }
        [DataMember(Name = "statuses")]
        public IEnumerable<Tweet> searchResults;
    }

    [DataContract]
    public class Tweet
    {
        [DataMember(Name ="user")]
        public User user;

        [DataMember(Name = "text")]
        public string text { get; set; }

        [DataMember(Name ="Id")]
        public long id { get; set; }

    }
    [DataContract]
    public class User
    {
        [DataMember(Name = "name")]
        public string name;

        [DataMember(Name = "profile_image_url")]
        public string imageUrl;
    }
}