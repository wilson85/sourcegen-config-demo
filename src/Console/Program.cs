using System.Text.Json;
using Abstractions;

namespace ConfigDemo;


internal class Program
{
    private static FromConfig websiteId = "website.id";

    private static FromConfig currency = "website.curreny";

    private static FromConfig language = "website.Language";

    private static FromCurrentTenantConfig test = "website.test";

    static async Task Main(string[] args)
    {
        // force the static constructor to run
        new PreloadKeys();

        Console.WriteLine("preloaded keys:" + JsonSerializer.Serialize(TenantUtil.Instance.Preloads));
        await TenantUtil.Instance.Initialize(new MockSettingProvider(), new StaticTenantLocator("tenant1.uk.dev"));

        Console.WriteLine("websiteId: " + websiteId.Get("tenant1.uk.dev"));

        Console.WriteLine("current tenant websiteId: " + test.Get());

        Console.ReadKey();

    }
}


public partial class PreloadKeys
{

}


