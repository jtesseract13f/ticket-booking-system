using SimpleCinemaService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Servers = new List<Microsoft.OpenApi.OpenApiServer>
        {
            new Microsoft.OpenApi.OpenApiServer
            {
                Url = "/simple-cinema-service"
            }
        };
        return Task.CompletedTask;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/simple-cinema-service/openapi/v1.json", "v1");
    });
}

app.MapGet("/manage/health", () => StatusCodes.Status200OK);
var apiV1 = app.MapGroup("/api/v1");

app.MapGet("/cinemas", () => new List<SimpleCinema>()
{
    new SimpleCinema(
        Guid.Empty, 
        1000, 
        DateTime.MinValue.AddYears(2026), 
        "Игла",
        "Desc",
        "Uri",
        "г.Бишкек, ЦУМ Айчурок"
    )
});

app.Run();
