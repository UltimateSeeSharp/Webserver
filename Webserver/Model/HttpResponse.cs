using System.Collections;

namespace Webserver.Model;

internal struct ResponseStruct
{
    public int StatusCode;
    public string HttpVersion;

    //  Saves all header items like server-name, date, ...
    public Hashtable HeaderEntries;
}
