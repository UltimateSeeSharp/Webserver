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

        //  Get data stream from request transmission
        NetworkStream networkStream = tcpClient.GetStream();

        //  Read data from stream
        byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
        networkStream.Read(buffer, 0, buffer.Length);

        //  Parse stram data to request struct
        RequestStruct request = _headerService.ParseHeader(buffer);

        //  Process data from parsed request struct
        await ProcessRequest(request, networkStream);
    }

    async Task ProcessGET(RequestStruct request, NetworkStream networkStream)
    {
        //  Send generic http response header
        await _streamService.SendHeader(networkStream, _serverName);

        //  Get requested file from http server
        string bodyContent = File.ReadAllText("Webspace\\" + request.RequestPath);

        //  Convert requested file to bytes and send over stream
        byte[] bodyBytes = Encoding.ASCII.GetBytes(bodyContent);
        await networkStream.WriteAsync(bodyBytes, 0, bodyBytes.Length);

        //  Close stream and singal transmission end
        networkStream.Close();
    }

    async Task ProcessHEAD(NetworkStream networkStream)
    {
        //  Send header => HEAD method => without body
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