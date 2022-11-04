namespace VKOAuth;

using Newtonsoft.Json;

public class UsersGetResponse
{
    public List<UserInfo> Response { get; set; }

    public class UserInfo
    {
        public long Id { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

    }
}