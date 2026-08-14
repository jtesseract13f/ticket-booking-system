using SimpleCinemaApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
//TODO: Add testing data, about 5 movies
app.MapGet("/api/v1/cinemas", () => new List<SimpleCinema>()
{
    new SimpleCinema(
        Guid.Empty, 
        1000, 
        DateTime.MinValue.AddYears(2026), 
        "Name",
        "Desc",
        "Uri",
        "г.Бишкек, ЦУМ Айчурок"
        )
});

app.Run();
