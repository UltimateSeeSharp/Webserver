using System.Net.Sockets;
using System.Text;
using Webserver.Extentions;
using Webserver.Model;

namespace Webserver;

public class ClientHandler
{
    private readonly StreamService _streamService = Bootstrapper.Resolve<StreamService>();
    private readonly HeaderService _headerService = Bootstrapper.Resolve<HeaderService>();
    private readonly string _serverName;

    public ClientHandler(string serverName)
    {
        _serverName = serverName;
    }

    internal async Task ProcessRequest(TcpClient tcpClient)
    {
        if (tcpClient is null)
            throw new NullReferenceException("No Client found to process request.");

        NetworkStream networkStream = tcpClient.GetStream();

        byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
        networkStream.Read(buffer, 0, buffer.Length);

        RequestStruct request = _headerService.ParseHeader(buffer);
        await ProcessRequest(request, networkStream);
    }

    async Task ProcessGET(RequestStruct request, NetworkStream networkStream)
    {
        await _streamService.SendHeader(networkStream, _serverName);

        //  Send body
        string bodyContent = File.ReadAllText("Webspace\\" + request.RequestPath);
        byte[] bodyBytes = Encoding.ASCII.GetBytes(bodyContent);
        await networkStream.WriteAsync(bodyBytes, 0, bodyBytes.Length);

        networkStream.Close();
    }

    async Task ProcessHEAD(NetworkStream networkStream)
    {
        await _streamService.SendHeader(networkStream, _serverName);
    }


    async Task ProcessRequest(RequestStruct request, NetworkStream networkStream)
    {
        switch (request.HttpMethod)
        {
            case "GET":
                await ProcessGET(request, networkStream);
                break;

            case "HEAD":
                await ProcessHEAD(networkStream);    
                break;

            default: throw new NotImplementedException("HTTP Method not implemented.");
        }
    }
}