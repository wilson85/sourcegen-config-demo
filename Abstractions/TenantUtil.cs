namespace Abstractions;

public class TenantUtil
{
    private record NoOpTenantsettings : ITenantSettingProvider
    {
        public ValueTask<string> Get(string key, string tenant)
        {
            throw new NotImplementedException();
        }

        public Task Preload(IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }
    }


    private static TenantUtil? _instance;
    private static readonly object _lock = new object();

    private TenantUtil()
    {
    }

    ITenantSettingProvider _provider = new NoOpTenantsettings();

    ICurrentTenantLocator? _tenantLocator = null;

    public ValueTask<string> GetValue(string tenant, string key)
    {
        return _provider.Get(tenant, key);
    }

    public ValueTask<string> GetCurentTenantValue(string key)
    {
        var tenant = _tenantLocator?.GetCurrentTenant() 
            ?? throw new ApplicationException("unable to locate tenant");

        return _provider.Get(tenant, key);
    }

    public async Task Initialize(ITenantSettingProvider provider, ICurrentTenantLocator? tenantLocator)
    {
        _provider = provider;
        if (Preloads != null)
        {
            await _provider.Preload(Preloads);
        }
        _tenantLocator = tenantLocator;
    }


    public void SetPreloadKeys(IEnumerable<string> keys)
    {
        Preloads = keys;
    }

    public static TenantUtil Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new TenantUtil();
                    }
                }
            }
            return _instance;
        }
    }

    public IEnumerable<string>? Preloads { get; set; }
}
