#pragma warning disable 0618

using System.Collections;
using System.Net.Sockets;
using System.Text;
using Webserver.Model;

namespace Webserver;

static class Program
{
    static TcpClient? _client;
    static TcpListener? _listener;
    static NetworkStream _clientStream;

    static readonly string _serverName = "KathiServer v1.0";

    static async Task Main()
    {
        _listener = new(8000);
        _listener.Start();

        while (true)
        {
            RequestStruct request = await AwaitRequest();
            await ProcessRequest(request);
        }
    }

    static async Task<RequestStruct> AwaitRequest()
    {
        if (_listener is null)
            throw new NullReferenceException("Listener not available");

        _client = await _listener.AcceptTcpClientAsync();

        String message = String.Empty;
        byte[] buffer = new byte[_client.ReceiveBufferSize];

        _clientStream = _client.GetStream();
        _clientStream.Read(buffer, 0, buffer.Length);

        message = String.Concat(message, Encoding.ASCII.GetString(buffer, 0, buffer.Length));
        var split = message.Split("\r\n");

        StringBuilder stringBuilder = new();
        buffer.ToList().ForEach(b => stringBuilder.Append((char)b));

        var headerItems = stringBuilder.ToString().Split();

        RequestStruct requestStruct = new();
        requestStruct.HttpMethod = headerItems[0];
        requestStruct.RequestPath = headerItems[1];
        requestStruct.HttpVersion = headerItems[2];
        requestStruct.Host = headerItems[5];

        return requestStruct;
    }

    static async Task ProcessRequest(RequestStruct request)
    {
        switch (request.HttpMethod)
        {
            case "GET":
                await ProcessRequest(request);
                break;

            case "HEAD":
                break;

            default:
                throw new NotImplementedException("HttpMethod not supported " + request.HttpMethod);
        }
    }

    static async Task ProcessGET(RequestStruct request)
    {
        ResponseStruct response = new();
        response.statusCode = 200;
        response.version = "HTTP/1.1";
        response.HeaderEntris = new();
        response.HeaderEntris.Add("Server", _serverName);
        response.HeaderEntris.Add("Date", DateTime.Now.ToString("r"));

        String responseHeader = String.Empty;
        responseHeader += response.version + " " + response.statusCode + " " + "OK";

        foreach (DictionaryEntry headerItem in response.HeaderEntris)
        {
            responseHeader += headerItem.Key + ": " + headerItem.Value + "\n";
        }

        responseHeader += "\n";

        //  Send header
        byte[] responseHeaderBytes = Encoding.ASCII.GetBytes(responseHeader);
        _clientStream.Write(responseHeaderBytes, 0, responseHeaderBytes.Length);

        //  Send body
        string bodyContent = File.ReadAllText("Webspace\\index.html");
        byte[] responseBodyBytes = Encoding.ASCII.GetBytes(bodyContent);
        _clientStream.Write(responseBodyBytes, 0, responseBodyBytes.Length);
    }
}