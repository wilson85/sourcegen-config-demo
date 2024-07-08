namespace Abstractions;

public class FromConfig
{
    private string _key;

    public FromConfig(string key)
    {
        _key = key;
    }

    public ValueTask<string> Get(string tenant)
    {
        return TenantUtil.Instance.GetValue(tenant, _key);
    }

    public static implicit operator FromConfig(string key)
    {
        return new FromConfig(key);
    }
}

public class FromCurrentTenantConfig
{
    private string _key;

    public FromCurrentTenantConfig(string key)
    {
        _key = key;
    }

    public ValueTask<string> Get()
    {
        return TenantUtil.Instance.GetCurentTenantValue(_key);
    }

    public static implicit operator FromCurrentTenantConfig(string key)
    {
        return new FromCurrentTenantConfig(key);
    }
}
