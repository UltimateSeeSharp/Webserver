using System.Net;

namespace HttpServer;

static class Program
{
    static HttpListener _httpListener;
    static readonly string url = "http://localhost:8000";
    static readonly int port = 8000;
    static readonly string responseData =
        "!DOCTYPE html" + 
        "<html>" +
        "<header>" +
        "<title>HttpServer Example</title>" +
        "</header>" +
        "<body>" +
        "<h1>HttpServer Example</h1>" + 
        "</body>" +
        "</html>";

    static readonly string responseImg =

    static async Task Main()
    {
        _httpListener = new();
        _httpListener.Prefixes.Add("http://localhost:8000");
        _httpListener.Start();

        Console.WriteLine($"Listening to {url}");

        while (true)
        {
            await HandleConnection();
        }


    }

    static async Task HandleConnection()
    {

    }
}