using IdP.Web.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
//using Microsoft.Data.SqlClient;

namespace IdentityProvider.Infrastructure.Worker
{
    public class ClientSeederWorker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public ClientSeederWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var retries = 10;
            while (retries > 0)
            {
                try
                {
                    await context.Database.MigrateAsync(cancellationToken);
                    break;
                }
                catch(Exception)
                {
                    // Any other error (e.g. Connection Refused because SQL is still starting)
                    retries--;
                    if (retries == 0) throw;

                    // Wait 2 seconds before trying again
                    await Task.Delay(2000, cancellationToken);
                }
            }
            var appManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
            var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if(await scopeManager.FindByNameAsync("ims_resource_server", cancellationToken) is null)
            {
                await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    Name = "ims_resource_server",
                    DisplayName = "IMS API",
                    Description = "Access the Inventory management system.",
                    Resources =
                    {
                        "ims_backend_api" // Optional: Validates the 'aud' claim for the resource server
                    }
                }, cancellationToken);
            }

            // B. SEED CLIENTS

            // 1. Angular/React Client
            if (await appManager.FindByClientIdAsync("ims-react-client", cancellationToken) is null)
            {
                await appManager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "ims-react-client",
                    ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
                    DisplayName = "React Client",
                    RedirectUris = { new Uri("http://localhost:5173/callback") },
                    PostLogoutRedirectUris = { new Uri("http://localhost:5173") , 
                        new Uri("http://localhost:5173/") },
                    
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.EndSession,
                        OpenIddictConstants.Permissions.Endpoints.Token,

                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                        OpenIddictConstants.Permissions.ResponseTypes.Code,

                        OpenIddictConstants.Permissions.Scopes.Email,
                        OpenIddictConstants.Permissions.Scopes.Profile,
                        OpenIddictConstants.Permissions.Scopes.Roles,
                        OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.OfflineAccess,
                        
                        OpenIddictConstants.Permissions.Prefixes.Scope + "ims_resource_server"
                    },
                    Requirements =
                    {
                        OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
                    }
                }, cancellationToken);
            }

            if (await appManager.FindByClientIdAsync("postman", cancellationToken) is null)
            {
                await appManager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "postman",
                    ClientSecret = "postman-secret",
                    DisplayName = "Postman",
                    RedirectUris = { new Uri("https://oauth.pstmn.io/v1/callback") },
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                        OpenIddictConstants.Permissions.Endpoints.EndSession,
                        OpenIddictConstants.Permissions.ResponseTypes.Code,
                        OpenIddictConstants.Permissions.Scopes.Email,
                        OpenIddictConstants.Permissions.Scopes.Profile,
                        OpenIddictConstants.Permissions.Scopes.Roles,
                    
                        // Allow Postman to test IMS scope too
                        OpenIddictConstants.Permissions.Prefixes.Scope + "ims_resource_server"
                    },
                    Requirements =
                    {
                        OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
                    }
                }, cancellationToken);
            }
            
            if (await roleManager.FindByNameAsync("User") is null)
            {
                var role = new IdentityRole("User");
                await roleManager.CreateAsync(role);
            }
            
            if (await roleManager.FindByNameAsync("Admin") is null)
            {
                var role = new IdentityRole("Admin");
                await roleManager.CreateAsync(role);
            }

            if (await userManager.FindByNameAsync("sosal") is null)
            {
                var user = new ApplicationUser
                {
                    UserName = "sosal",
                    Email = "sosal@test.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "Pass123$");
                await userManager.AddToRoleAsync(user, "User");
            }
            
            if (await userManager.FindByNameAsync("admin") is null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@test.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "Pass123$");
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
