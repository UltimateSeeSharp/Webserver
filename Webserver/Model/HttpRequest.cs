using System.Collections;

namespace Webserver.Model;

internal struct RequestStruct
{
    //  Header
    public string HttpMethod;
    public string RequestPath;
    public string Host;
    public string HttpVersion;
}

internal struct ResponseStruct
{
    public int statusCode;
    public string version;
    public Hashtable HeaderEntris;
    public int BodySize;
    public byte[] BodyData;
    public FileStream fileStream;
}