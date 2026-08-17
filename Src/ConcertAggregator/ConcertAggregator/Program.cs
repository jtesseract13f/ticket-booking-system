using ConcertAggregator.DAL;
using ConcertAggregator.Models;
using Refit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddOpenApi();
builder.Services.AddDbContext<ConcertDbContext>(x => x.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en-US", "fr" };
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});
builder.Services.AddRefitClient<ILlmApi>()
    .ConfigureHttpClient(c =>
        c.BaseAddress = new Uri("http://192.168.0.107:1234")); // TODO: перенести в конфиги
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
        app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

//app.UseHttpsRedirection();

app.MapPost("/llm", async (string text, ILlmApi api) =>
    {
        var result = await api.GetTranslation(new ModelBody(text));
        return result;
    }
);

app.Run();
