using System.Net;
using System.Net.Sockets;

namespace Webserver.Requester;

public static class Program
{
    private static int _requestCount = 0;

    public static async Task Main()
    {
        while (true)
        {
            Task.Run(SendGET);
        }
    }

    private static async Task SendGET()
    {
        string html = string.Empty;
        string url = "http://localhost:8000/";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.AutomaticDecompression = DecompressionMethods.GZip;

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            html = reader.ReadToEnd();
        }

        Console.WriteLine(_requestCount);
        _requestCount++;
    }
}