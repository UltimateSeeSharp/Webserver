#pragma warning disable 0618 // Supress Listener outdated
#pragma warning disable 4014 // Start Task without await

using System.Net.Sockets;
using System.Text;
using Webserver.Extentions;
using Webserver.Model;

namespace Webserver;

static class Program
{
    static readonly string _serverName = "Server v1.0";
    static TcpListener? _listener;

    static async Task Main()
    {
        Bootstrapper.Start();

        _listener = new(8000);
        _listener.Start();

        while (true)
        {
            await HandleRequest();
        }
    }

    static async Task HandleRequest()
    {
        TcpClient client = await _listener!.AcceptTcpClientAsync();

        ClientHandler clientHandler = new ClientHandler(_serverName);
        Task.Factory.StartNew(() => clientHandler.ProcessRequest(client));
    }
}