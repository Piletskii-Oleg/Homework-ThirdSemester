using MyFTP;

var client = new Client("localhost", 8888);
await client.List("");