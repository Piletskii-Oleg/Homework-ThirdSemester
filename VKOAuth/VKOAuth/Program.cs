using Newtonsoft.Json;
using System.Diagnostics;

var client = new HttpClient();

//var id = "razber3";
//var version = "5.131";
//var access_token = "2cebd2592cebd2592cebd259c22ffae7ab22ceb2cebd2594fb33c33b851b838082b5034";

//var response = await client.GetAsync($"https://api.vk.com/method/users.get?user_id={id}&v={version}&fields=bdate&access_token={access_token}");
//if (response.IsSuccessStatusCode)
//{
//    var content = await response.Content.ReadAsStringAsync();
//    var account = JsonConvert.DeserializeObject<VKOAuth.UsersGetResponse>(content);
//    Console.WriteLine(content);
//    Console.WriteLine(account.Response[0].FirstName + " " + account.Response[0].LastName + " " + account.Response[0].Id);
//    Console.WriteLine(account.Response[0].BirthdayDate);
//}

var clientId = 51459570;
var redirectUri = "https://oauth.vk.com/blank.html";
var authString = $"https://oauth.vk.com/authorize?client_id={clientId}&display=page&redirect_uri={redirectUri}&scope=friends&response_type=token&v=5.131&state=123456";
authString = authString.Replace("&", "^&");
Process.Start(new ProcessStartInfo("cmd", $"/c start {authString}") { CreateNoWindow = true });

var userId = 344702527;
var access_token = Console.ReadLine();
var response = await client.GetAsync($"https://api.vk.com/method/friends.getOnline?user_id={userId}&v=5.131&access_token={access_token}");
var content = await response.Content.ReadAsStringAsync();

var onlineFriendsIds = JsonConvert.DeserializeObject<VKOAuth.FriendsGetOnline>(content);
foreach (var id in onlineFriendsIds.Response)
{
    var userResponse = await client.GetAsync($"https://api.vk.com/method/users.get?user_id={id}&v=5.131&fields=bdate&access_token={access_token}");
    var userContent = await userResponse.Content.ReadAsStringAsync();
    var userAccount = JsonConvert.DeserializeObject<VKOAuth.UsersGetResponse>(userContent);
    Console.WriteLine(userAccount.Response[0].FirstName + " " + userAccount.Response[0].LastName);
    Thread.Sleep(500);
}