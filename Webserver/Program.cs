#pragma warning disable 0618 // Supress Listener outdated
#pragma warning disable 4014 // Start Task without await

using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using Webserver.Extentions;
using Webserver.Model;

namespace Webserver;

static class Program
{
    static DbService _dbService = DbService.GetContext();
    static TcpListener? _listener;

    static readonly string _serverName = "Server v1.0";

    static async Task Main()
    {
        Bootstrapper.Start();

        //  Open Port for connection
        _listener = new(8000);
        _listener.Start();

        _dbService.Products.Add(new() { Id = Guid.NewGuid(), Name = "TestProductOne" });
        _dbService.SaveChanges();

        var product = _dbService.Products.First();
        var json = JsonConvert.SerializeObject(product);

        while (true)
        {
            //  Wait for incomming requests
            await HandleRequest();
        }
    }

    static async Task HandleRequest()
    {
        //  Save client which made request
        TcpClient client = await _listener!.AcceptTcpClientAsync();

        //  Create handler for processing incomming request
        ClientHandler clientHandler = new ClientHandler(_serverName);
        Task.Factory.StartNew(() => clientHandler.ProcessRequest(client));
    }
}