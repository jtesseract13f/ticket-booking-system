using System.Security.Claims;
using ConcertAggregator.BLL;
using ConcertAggregator.DAL;
using ConcertAggregator.DAL.Repositories;
using ConcertAggregator.DTO;
using ConcertAggregator.Infrastructure;
using ConcertAggregator.Models;
using Microsoft.AspNetCore.Mvc;
using Refit;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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
            new Microsoft.OpenApi.OpenApiServer { Url = "/concert-aggregator" }
        };
        return Task.CompletedTask;
    });

    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddDbContext<ConcertDbContext>(x =>
    x.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en-US", "ky-KG", "ru-RU" };
    options.SetDefaultCulture(supportedCultures[2])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

builder.Services
    .AddRefitClient<ILibreTranslateApi>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["libretranslate"]
                                ?? "http://localhost:5000");
        c.Timeout = TimeSpan.FromSeconds(30);
    });

builder.Services.AddScoped<TranslationService>();
builder.Services.AddScoped<NodeRepository>();
builder.Services.AddScoped<NodeService>();
builder.Services.AddScoped<EventRepository>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<NodeStrategy>();

var app = builder.Build();

try //Migrator
{
    using var scope = ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
    scope.ServiceProvider.GetRequiredService<ConcertDbContext>().Database.Migrate();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

app.UsePathBase("/concert-aggregator");

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/concert-aggregator/openapi/v1.json", "v1");
    });
}


app.MapGet("/manage/health", () => StatusCodes.Status200OK);

var apiV1 = app.MapGroup("/api/v1");
apiV1.MapGet("/admin", () => "Только для админов")
    .RequireAuthorization("AdminOnly");

var nodes = apiV1.MapGroup("/nodes")
    .RequireAuthorization("AdminOnly");

nodes.MapPost("/", async (AddNodeDto dto, [FromServices] NodeService service) =>
{
    var id = await service.AddNode(dto);
    return Results.Created($"/api/v1/nodes/{id}", id);
});

nodes.MapGet("/", async ([FromServices] NodeService service) =>
{
    var result = await service.GetAllNodes();
    return Results.Ok(result);
});

nodes.MapPut("/", async (UpdateNodeDto dto, [FromServices] NodeService service) =>
{
    var id = await service.UpdateNode(dto);
    return Results.Ok(id);
});

nodes.MapDelete("/{id:guid}", async (Guid id, NodeService service) =>
{
    var deletedId = await service.DeleteNode(new DeleteNodeDto (id));
    return Results.Ok(deletedId);
});

var group = apiV1.MapGroup("/translate-test")
            .RequireAuthorization("AdminOnly");

// GET /api/v1/translate-test/ping — быстрый пинг LibreTranslate
group.MapGet("/ping", async (
    [FromServices] ILibreTranslateApi api,
    CancellationToken ct) =>
{
    try
    {
        var languages = await api.GetLanguagesAsync(ct);
        return Results.Ok(new
        {
            ok = true,
            availableLanguages = languages.Select(l => l.Code).ToArray(),
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "LibreTranslate unavailable",
            detail: ex.Message,
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
});

// GET /api/v1/translate-test?text=Hello&from=En&to=Ru
group.MapGet("/", async (
    string text,
    LanguageCode from,
    LanguageCode to,
    [FromServices] TranslationService service,
    CancellationToken ct) =>
{
    var result = await service.TranslateAsync(text, from, to, ct);
    return Results.Ok(new
    {
        source = text,
        from = from.ToString(),
        to = to.ToString(),
        translation = result,
        changed = !string.Equals(text, result, StringComparison.Ordinal),
    });
});

group.MapPost("/event/{eventId:guid}", async (
    Guid eventId,
    LanguageCode sourceLanguage,
    [FromServices] EventRepository eventRepo,
    [FromServices] TranslationService service,
    [FromServices] ConcertDbContext db,
    CancellationToken ct) =>
{
    var evt = await eventRepo.GetEventById(eventId);
    if (evt is null) return Results.NotFound();

    var targets = await db.Languages.ToListAsync(ct);

    try
    {
        var translations = await service.TranslateEventAsync(
            evt, sourceLanguage, targets, ct);

        return Results.Ok(translations.Select(t => new
        {
            language = t.Language.Name,
            eventName = t.EventName,
            description = t.Description,
        }));
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

var events = apiV1.MapGroup("/events");

events.MapGet("/", async (
    [AsParameters] Filter filter,
    EventService service,
    CancellationToken ct) =>
{
    var result = await service.GetAllEventsFiltered(filter, ct);
    return Results.Ok(result);
});


app.Run();