namespace Abstractions;

public interface ITenantSettingProvider
{
    ValueTask<string> Get(string tenant, string key);

    Task Preload(IEnumerable<string> keys);
}

public interface ICurrentTenantLocator
{
    string GetCurrentTenant();
}

public class StaticTenantLocator : ICurrentTenantLocator
{
    public StaticTenantLocator(string tenant)
    {
        Tenant = tenant;
    }

    public string Tenant { get; }

    public string GetCurrentTenant() => Tenant;
}

public class MockSettingProvider : ITenantSettingProvider
{
    public async ValueTask<string> Get(string tenant, string key)
        => key + ".value";


    public Task Preload(IEnumerable<string> keys)
        => Task.CompletedTask;
}