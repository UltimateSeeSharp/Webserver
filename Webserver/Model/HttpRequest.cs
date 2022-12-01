using System.Collections;

namespace Webserver.Model;

internal struct RequestStruct
{
    public string HttpMethod;
    public string RequestPath;
    public string Host;
    public string HttpVersion;
    public Hashtable Headers;
}

internal struct ResponseStruct
{
    public int StatusCode;
    public string HttpVersion;
    public Hashtable HeaderEntries;
}