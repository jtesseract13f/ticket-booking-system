using SimpleConcertApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//TODO: Add testing data, about 5 concerts
app.MapGet("/api/v1/cinemas", () => new List<SimpleConcert>()
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
