using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using Webserver.Model;

namespace Webserver.Extentions;

internal class HeaderService
{
    internal RequestStruct ParseRequest(byte[] headerBuffer)
    {
        String message = Encoding.ASCII.GetString(headerBuffer, 0, headerBuffer.Length);
        var requestLines = message.Split("\r\n");

        var firstLine = requestLines[0];
        var firstLineSplit = firstLine.Split(" ");

        RequestStruct requestStruct = new();
        requestStruct.Headers = new();

        requestStruct.HttpMethod = firstLineSplit[0];
        requestStruct.RequestPath = firstLineSplit[1];
        
        //  Default path index.html
        if (requestStruct.RequestPath == "/")
            requestStruct.RequestPath = "index.html";

        requestStruct.HttpVersion = firstLineSplit[2];
        requestStruct.Host = requestLines[1].Split(": ")[1];

        //  Save each header statement
        int headerBodySeperationIndex = 0;        
        foreach (var headerItem in requestLines.Skip(2).ToList())
        {
            //  Body beginning => stop adding to header hashtable
            if (headerItem == String.Empty)
            {
                headerBodySeperationIndex = requestLines.ToList().IndexOf(headerItem);
                break;
            }

            var split = headerItem.Split(':');

            var key = split[0];
            var value = String.Empty;

            foreach (var valueItem in split.Skip(1).ToList())
                value += valueItem;

            if (value == String.Empty)
                continue;

            requestStruct.Headers.Add(key, value);
        }

        //  Save json in body
        requestStruct.JsonBody = requestLines.Skip(headerBodySeperationIndex + 1).First();

        return requestStruct;
    }

    internal string CreateResponseHeader(string serverName, HttpStatusCode statusCode)
    {
        ResponseStruct response = new();
        response.StatusCode = (int)statusCode;
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