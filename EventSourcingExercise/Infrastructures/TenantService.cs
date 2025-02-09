using EventSourcingExercise.Cores;

namespace EventSourcingExercise.Infrastructures;

public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<string> GetTenantId()
    {
        var tenantId = _httpContextAccessor.HttpContext!.Request.Headers["Tenant-Id"].ToString();
        return Task.FromResult(tenantId);
    }
}