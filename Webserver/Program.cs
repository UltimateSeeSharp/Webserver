#pragma warning disable 0618

using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using Webserver.Extentions;
using Webserver.Model;

namespace Webserver;

static class Program
{
    static TcpClient? _client;
    static TcpListener? _listener;
    static NetworkStream _clientStream;

    static readonly string _serverName = "Server v1.0";

    static async Task Main()
    {
        _listener = new(8000);
        _listener.Start();

        while (true)
        {
            RequestStruct request = await HandleRequest();
            await ProcessRequest(request);
        }
    }

    static async Task<RequestStruct> HandleRequest()
    {
        if (_listener is null)
            throw new NullReferenceException("Listener not available");

        //  Accept connection and get transmitted stream
        _client = await _listener.AcceptTcpClientAsync();
        _clientStream = _client.GetStream();

        //  Read transmitted stream
        byte[] buffer = new byte[_client.ReceiveBufferSize];
        _clientStream.Read(buffer, 0, buffer.Length);

        //  Parse transmitted stream
        RequestStruct request = HeaderHelper.ParseHeader(buffer);

        return request;
    }

    static async Task ProcessRequest(RequestStruct request)
    {
        switch (request.HttpMethod)
        {
            case "GET": await ProcessGET(request);
                break;

            default: throw new NotImplementedException("HTTP Method not implemented.");
        }
    }

    static async Task ProcessGET(RequestStruct request)
    {
        string responseHeader = HeaderHelper.CreateResponseHeader(_serverName);

        //  Send header
        byte[] responseHeaderBytes = Encoding.ASCII.GetBytes(responseHeader);
        await _clientStream.WriteAsync(responseHeaderBytes, 0, responseHeaderBytes.Length);

        //  Send body
        string bodyContent = File.ReadAllText("Webspace\\" + request.RequestPath);
        byte[] responseBodyBytes = Encoding.ASCII.GetBytes(bodyContent);
        await _clientStream.WriteAsync(responseBodyBytes, 0, responseBodyBytes.Length);

        _clientStream.Close();
    }
}