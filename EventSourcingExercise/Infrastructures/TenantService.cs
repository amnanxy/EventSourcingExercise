using EventSourcingExercise.Cores;

namespace EventSourcingExercise.Infrastructures;

public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<string> GetTenantCode()
    {
        var tenantCode = _httpContextAccessor.HttpContext!.Request.Headers["Tenant-Code"].ToString();
        return Task.FromResult(tenantCode);
    }
}