using MyFTP;

var client = new Client("localhost", 8888);
await client.Get("EULA.txt", @"C:\Games\Higurashi\Higurashi When They Cry Hou - Ch.3 Tatarigoroshi\EULA2.txt");