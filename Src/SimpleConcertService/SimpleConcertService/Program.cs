using SimpleConcertService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Servers = new List<Microsoft.OpenApi.OpenApiServer>
        {
            new Microsoft.OpenApi.OpenApiServer
            {
                Url = "/simple-concert-service"
            }
        };
        return Task.CompletedTask;
    });
});
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/simple-concert-service/openapi/v1.json", "v1");
    });
}

//app.UseHttpsRedirection();

app.MapGet("/manage/health", () => StatusCodes.Status200OK);
var apiV1 = app.MapGroup("/api/v1");

//TODO: Add testing data, about 5 concerts
apiV1.MapGet("/cinemas", () => new List<SimpleConcert>()
{
    new SimpleConcert(
        Guid.Empty, 
        1000, 
        DateTime.MinValue.AddYears(2026), 
        "TARDIGRADE INFERNO",
        "Desc",
        "Uri",
        "г.Бишкек, ЦУМ Айчурок"
    )
});
app.Run();