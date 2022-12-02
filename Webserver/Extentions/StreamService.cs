using System.Net.Sockets;
using System.Text;

namespace Webserver.Extentions;

internal class StreamService
{
    private readonly HeaderService _headerService = Bootstrapper.Resolve<HeaderService>();

    internal async Task SendHeader(NetworkStream networkStream, string serverName)
    {
        string headerString = _headerService.CreateResponseHeader(serverName);
        byte[] headerBytes = Encoding.ASCII.GetBytes(headerString);
        await networkStream.WriteAsync(headerBytes, 0, headerBytes.Length);
    }

}