﻿namespace MyFTP;

using System.Net;
using System.Net.Sockets;

public class Server
{
    private readonly TcpListener listener;
    private readonly string serverPath;

    public Server(string path, int port)
    {
        this.serverPath = path;

        listener = new TcpListener(IPAddress.Any, port);
    }

    public async Task Start()
    {
        listener.Start();

        while (true)
        {
            var socket = listener.AcceptSocket();
            await Query(socket);
        }
    }

    private async Task Query(Socket socket)
    {
        var stream = new NetworkStream(socket);
        var reader = new StreamReader(stream);

        var input = await reader.ReadLineAsync();
        if (input == null)
        {
            throw new InvalidOperationException();
        }

        var inputSplit = input.Split();

        if (inputSplit[0] == "1")
        {
            await List(inputSplit[1], stream);
        }
        else if (inputSplit[0] == "2")
        {
            await Get(inputSplit[1], stream);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private async Task List(string path, NetworkStream stream)
    {
        var writer = new StreamWriter(stream);

        path = Path.Combine(this.serverPath, path);
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);

        var fileCount = files.Length + directories.Length;
        await writer.WriteAsync($"{fileCount}");

        foreach (var file in files)
        {
            await writer.WriteAsync($" {file} {false}");
        }

        foreach (var directory in directories)
        {
            await writer.WriteAsync($" {directory} {true}");
        }

        await writer.WriteLineAsync();
        await writer.FlushAsync();

        stream.Socket.Close();
        await stream.DisposeAsync();
    }


    private async Task Get(string path, NetworkStream stream)
    {
        await using var writer = new StreamWriter(stream);
        await using var binaryWriter = new BinaryWriter(stream);
        path = Path.Combine(this.serverPath, path);

        var info = new FileInfo(path);
        if (!info.Exists)
        {
            throw new FileNotFoundException();
        }

        await stream.WriteAsync(BitConverter.GetBytes(info.Length));

        using var reader = new BinaryReader(File.OpenRead(path));
        for (int i = 0; i < info.Length; i++)
        {
            var readByte = reader.ReadByte();
            stream.WriteByte(readByte);
            await stream.FlushAsync();
        }

        await stream.FlushAsync();

        stream.Socket.Close();
        await stream.DisposeAsync();
    }
}