using Newtonsoft.Json;
using System.Text.Json.Nodes;
using Webserver.Model;

namespace Webserver.Extentions;

internal class BodyParserService
{
    internal List<Product>? ParseProducts(string json)
    {
        List<Product>? productList = null;
        Product? product = null;

        try
        {
            product = JsonConvert.DeserializeObject<Product>(json);
        }
        catch { };

        try
        {
            productList = JsonConvert.DeserializeObject<List<Product>>(json);
        }
        catch { };

        if (productList is not null)
            return productList;

        if (product is not null)
        {
            productList = new();
            productList.Add(product);
            return productList;
        }
        else
            return null;
    }
}
