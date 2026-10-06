
using System.Security.Claims;
using ConcertAggregator.Infrastructure;
using ConcertAggregator.Infrastructure.BLL;
using EmailService;
using EmailService.BLL;
using EmailService.DTO;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using Refit;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer("http://ticket-booking-system.local/identity-provider");
        options.UseSystemNetHttp();
        options.UseAspNetCore();
    });
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserOnly", policy => policy.RequireClaim("role", "User"));
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("role", "Admin"));
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Servers = new List<Microsoft.OpenApi.OpenApiServer>
        {
            new Microsoft.OpenApi.OpenApiServer { Url = "/email-service" }
        };
        return Task.CompletedTask;
    });

    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en-US", "ky-KG", "ru-RU" };
    options.SetDefaultCulture(supportedCultures[2])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

builder.Services.AddScoped<MailingService>();
builder.Services.AddSingleton<IEmailSender, TestEmailSender>();
builder.Services.AddHostedService<MailingBackgroundService>();
builder.Services.AddSingleton<ITokenAccessor, AsyncLocalTokenAccessor>();

builder.Services.AddTransient<BearerTokenForwardingHandler>();

builder.Services
    .AddRefitClient<ITicketApi>()
    .ConfigureHttpClient(c =>
        c.BaseAddress = new Uri(builder.Configuration["TicketApi:BaseUrl"]
                                ?? "http://localhost:5001"))
    .AddHttpMessageHandler<BearerTokenForwardingHandler>();
var app = builder.Build();

app.UsePathBase("/email-service");
 
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/email-service/openapi/v1.json", "v1");
    });
}

try
{
    using var scope = ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

app.MapGet("/manage/health", () => StatusCodes.Status200OK);

var apiV1 = app.MapGroup("/api/v1");

apiV1.MapGet("/admin", () => "Только для админов")
    .RequireAuthorization("AdminOnly");

var group = app.MapGroup("/api/v1/mailings").WithTags("Mailings");

group.MapGet("/", async (MailingService svc, CancellationToken ct) =>
    Results.Ok(await svc.GetAllAsync(ct)));

group.MapGet("/{id:int}", async (int id, MailingService svc, CancellationToken ct) =>
{
    var m = await svc.GetAsync(id, ct);
    return m is null ? Results.NotFound() : Results.Ok(m);
}).RequireAuthorization();;

group.MapPost("/", async (
    CreateMailingRequest req,
    MailingService svc,
    CancellationToken ct) =>
{
    var m = await svc.CreateAsync(req, ct);
    return Results.Created($"/api/mailings/{m.Id}", m);
}).RequireAuthorization();;

group.MapPut("/{id:int}", async (
    int id,
    UpdateMailingRequest req,
    MailingService svc,
    CancellationToken ct) =>
{
    var m = await svc.UpdateAsync(id, req, ct);
    return m is null ? Results.NotFound() : Results.Ok(m);
}).RequireAuthorization();;

group.MapDelete("/{id:int}", async (
    int id,
    MailingService svc,
    CancellationToken ct) =>
{
    var ok = await svc.DeleteAsync(id, ct);
    return ok ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();;

group.MapPost("/process", async (
    HttpContext http,
    ITokenAccessor accessor,
    MailingService svc,
    CancellationToken ct) =>
{
    accessor.Token = http.Request.Headers.Authorization
        .FirstOrDefault()?.Replace("Bearer ", "");
    await svc.ProcessAllAsync(ct);
    return Results.Ok(new { status = "processed" });
}).RequireAuthorization();;

app.Run();
