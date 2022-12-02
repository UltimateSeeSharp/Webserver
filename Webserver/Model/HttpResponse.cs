using System.Collections;

namespace Webserver.Model;

internal struct ResponseStruct
{
    public int StatusCode;
    public string HttpVersion;
    public Hashtable HeaderEntries;
}
