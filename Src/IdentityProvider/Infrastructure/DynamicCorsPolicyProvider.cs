using IdP.Web.Infrastructure.Data;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenIddict.EntityFrameworkCore.Models;

namespace IdP.Web.Infrastructure
{
    public class DynamicCorsPolicyProvider : ICorsPolicyProvider
    {
        private readonly CorsOptions _corsOptions;
        private readonly IServiceProvider _serviceProvider;
        public DynamicCorsPolicyProvider(IOptions<CorsOptions> corsOptions, IServiceProvider serviceProvider)
        {
            _corsOptions = corsOptions.Value;
            _serviceProvider = serviceProvider;
        }

        public async Task<CorsPolicy?> GetPolicyAsync(HttpContext context, string? policyName)
        {
            if (policyName != null)
            {
                return _corsOptions.GetPolicy(policyName);
            }
            var origin = context.Request.Headers["Origin"].ToString();
            if (string.IsNullOrEmpty(origin))
            {
                return null;
            }
            
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var isAllowed = await dbContext.Set<OpenIddictEntityFrameworkCoreApplication>()
                .AnyAsync(app => !string.IsNullOrEmpty(app.RedirectUris) && app.RedirectUris.Contains(origin));

            if (isAllowed)
            {
                var policyBuilder = new CorsPolicyBuilder();
                policyBuilder.WithOrigins(origin)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();

                return policyBuilder.Build();
            }

            return null;
        }
    }
}
