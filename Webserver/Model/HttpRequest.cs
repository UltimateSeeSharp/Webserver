using System.Collections;

namespace Webserver.Model;

internal struct RequestStruct
{
    public string HttpMethod;
    public string RequestPath;
    public string Host;
    public string HttpVersion;

    //  Saves all header items like compression-type, age, ...
    public Hashtable Headers;

    public string[] Jsons;
}