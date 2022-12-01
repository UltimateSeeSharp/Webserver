using System.Collections;
using System.Text;
using Webserver.Model;

namespace Webserver.Extentions;

internal static class HeaderHelper
{
    internal static RequestStruct ParseHeader(byte[] headerBuffer)
    {
        String message = Encoding.ASCII.GetString(headerBuffer, 0, headerBuffer.Length);
        var headerLines = message.Split("\r\n");

        var firstLine = headerLines[0];
        var firstLineSplit = firstLine.Split(" ");

        RequestStruct requestStruct = new();
        requestStruct.Headers = new();

        requestStruct.HttpMethod = firstLineSplit[0];
        requestStruct.RequestPath = firstLineSplit[1];
        
        //  Default path index.html
        if (requestStruct.RequestPath == "/")
            requestStruct.RequestPath = "index.html";

        requestStruct.HttpVersion = firstLineSplit[2];
        requestStruct.Host = headerLines[1].Split(": ")[1];

        //  Save each header statement
        foreach (var item in headerLines.Skip(2).ToList())
        {
            var split = item.Split(':');

            var key = split[0];
            var value = String.Empty;

            foreach (var valueItem in split.Skip(1).ToList())
                value += valueItem;

            if (value == String.Empty)
                continue;

            requestStruct.Headers.Add(key, value);
        }

        return requestStruct;
    }

    internal static string CreateResponseHeader(string serverName)
    {
        ResponseStruct response = new();
        response.StatusCode = 200;
        response.HttpVersion = "HTTP/1.1";
        response.HeaderEntries = new();
        response.HeaderEntries.Add("Server", serverName);
        response.HeaderEntries.Add("Date", DateTime.Now.ToString("r"));

        //  Create header string
        String responseHeader = String.Empty;
        responseHeader += response.HttpVersion + " " + response.StatusCode + " " + "OK" + "\n";

        //  Write header items to header string
        foreach (DictionaryEntry headerItem in response.HeaderEntries)
        {
            responseHeader += headerItem.Key + ": " + headerItem.Value + "\n";
        }

        responseHeader += "\n";

        return responseHeader;
    }
}