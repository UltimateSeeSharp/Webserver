using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Webserver.Extentions;

namespace Webserver.Model;

public class ClientHandler
{
    private readonly StreamService _streamService = Bootstrapper.Resolve<StreamService>();
    private readonly HeaderService _headerService = Bootstrapper.Resolve<HeaderService>();
    private readonly BodyParserService _jsonBodyParser = Bootstrapper.Resolve<BodyParserService>();
    private readonly DbService _dbService = Bootstrapper.Resolve<DbService>();
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
        RequestStruct request = _headerService.ParseRequest(buffer);

        //  Process data from parsed request struct
        await ManageRequestMethod(request, networkStream);
    }

    async Task ProcessGET(RequestStruct request, NetworkStream networkStream)
    {
        //  Send generic http response header
        await _streamService.SendHeader(networkStream, _serverName, HttpStatusCode.OK);

        //  Get requested file from http server
        string bodyContent = String.Empty;

        if (request.RequestPath == "/Products")
            bodyContent = JsonConvert.SerializeObject(_dbService.Products.ToList());
        else
            bodyContent = File.ReadAllText("Webspace\\" + request.RequestPath);

        //  Convert requested file to bytes and send over stream
        byte[] bodyBytes = Encoding.ASCII.GetBytes(bodyContent);
        await networkStream.WriteAsync(bodyBytes, 0, bodyBytes.Length);
    }

    async Task ProcessHEAD(NetworkStream networkStream)
    {
        //  Send header => HEAD method => without body
        await _streamService.SendHeader(networkStream, _serverName, HttpStatusCode.OK);
    }

    async Task ProcessPOST(RequestStruct request, NetworkStream networkStream)
    {
        //  Parse body
        List<Product>? products = _jsonBodyParser.ParseProducts(request.JsonBody);

        if (products is null)
        {
            await _streamService.SendHeader(networkStream, _serverName, HttpStatusCode.NoContent);
            return;
        }

        _dbService.AddRange(products);
        await _dbService.SaveChangesAsync();

        await _streamService.SendHeader(networkStream, _serverName, HttpStatusCode.OK);
    }

    async Task ProcessPUT(RequestStruct request, NetworkStream networkStream)
    {
        //  Parse body
        List<Product>? products = _jsonBodyParser.ParseProducts(request.JsonBody);

        if (products is null)
        {
            await _streamService.SendHeader(networkStream, _serverName, HttpStatusCode.NoContent);
            return;
        }

        foreach (Product product in products)
        {
            if (product is null)
                continue;

            Product? exisitingProduct = _dbService.Products.FirstOrDefault(x => x.Id == product.Id);

            if (exisitingProduct is null)
                continue;

            exisitingProduct = product;
        }

        await _dbService.SaveChangesAsync();
        await _streamService.SendHeader(networkStream, _serverName, HttpStatusCode.OK);
    }

    async Task ManageRequestMethod(RequestStruct request, NetworkStream networkStream)
    {
        switch (request.HttpMethod)
        {
            case "GET":
                await ProcessGET(request, networkStream);
                break;

            case "HEAD":
                await ProcessHEAD(networkStream);
                break;

            case "POST":
                await ProcessPOST(request, networkStream);
                break;

            case "PUT":
                await ProcessPUT(request, networkStream);
                break;

            default: throw new NotImplementedException("HTTP Method not implemented.");
        }

        networkStream.Close();
    }
}