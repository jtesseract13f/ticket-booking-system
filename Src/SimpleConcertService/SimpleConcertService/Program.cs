using Microsoft.EntityFrameworkCore;
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


builder.Services.AddDbContext<DbContext>(x => x.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
try //Migrator
{
    using var scope = ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
    scope.ServiceProvider.GetRequiredService<DbContext>().Database.Migrate();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}
app.UsePathBase("/simple-concert-service");

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

apiV1.MapGet("/concerts", () => new List<SimpleConcert>()
{
    new SimpleConcert(
        Guid.Parse("47e7674a-e730-445e-a37e-9e48d093f470"),
        2500,
        new DateTime(2026, 9, 25, 19, 0, 0, DateTimeKind.Utc),
        "TARDIGRADE INFERNO",
        "Тяжёлый вечер в самом сердце Бишкека: пять часов отборного пост-метала и стоунера. " +
        "Хедлайнер — локальная группа TARDIGRADE INFERNO с новым альбомом «Пепел над Ала-Тоо». " +
        "На разогреве — коллективы из Оша и Каракола. 18+.",
        "http://ticket-booking-system.local/simple-concert-service/swagger/index.html",
        "г. Бишкек, ЦУМ Айчурок, концертный зал на 3 этаже"
    ),
    new SimpleConcert(
        Guid.Parse("a1b2c3d4-1111-2222-3333-444455556666"),
        1800,
        new DateTime(2026, 10, 3, 20, 0, 0, DateTimeKind.Utc),
        "Bishkek Jazz Nights: Trio Ala-Too",
        "Камерный джазовый вечер в стиле bebop и modal jazz. Трио Ala-Too представит программу " +
        "«Кыргызские стандарты» — джазовые аранжировки народных мелодий Комуза и Курмангазы. " +
        "Специальный гость — саксофонист из Алматы. Свободная рассадка, welcome-drink включён.",
        "http://ticket-booking-system.local/simple-concert-service/swagger/index.html",
        "г. Бишкек, ул. Киевская 95, Джаз-клуб «Ой-Булак»"
    ),
    new SimpleConcert(
        Guid.Parse("b2c3d4e5-2222-3333-4444-555566667777"),
        3500,
        new DateTime(2026, 10, 17, 18, 30, 0, DateTimeKind.Utc),
        "Симфония Чингиза Айтматова",
        "Национальный симфонический оркестр Кыргызстана исполняет «Реквием» современного композитора " +
        "Азамата Жумагулова по мотивам «И дольше века длится день». Премьера в Бишкеке. " +
        "В программе также — Второй фортепианный концерт Рахманинова с солистом из Москвы. " +
        "Дресс-код: smart casual.",
        "http://ticket-booking-system.local/simple-concert-service/swagger/index.html",
        "г. Бишкек, Кыргызская национальная филармония им. Т. Сатылганова"
    ),
    new SimpleConcert(
        Guid.Parse("c3d4e5f6-3333-4444-5555-666677778888"),
        1500,
        new DateTime(2026, 11, 7, 21, 0, 0, DateTimeKind.Utc),
        "Electronic Bishkek: Techno Marathon",
        "Ночной рейв на 8 часов: local DJ-резиденты и хедлайнер из Берлина. Techno, minimal, " +
        "deep house. Три танцпола, лазерное шоу, визуалы в стиле горных пейзажей Тянь-Шаня. " +
        "Face control, вход строго 18+ с документом.",
        "http://ticket-booking-system.local/simple-concert-service/swagger/index.html",
        "г. Бишкек, ул. Ибраимова 115, клуб «Промзона»"
    ),
    new SimpleConcert(
        Guid.Parse("d4e5f6a7-4444-5555-6666-777788889999"),
        1200,
        new DateTime(2026, 11, 21, 19, 0, 0, DateTimeKind.Utc),
        "Фольклорный фестиваль «Комуз-Fest»",
        "Большой вечер традиционной музыки: комузисты, акыны и манасчи со всех регионов страны. " +
        "Выступят ансамбли «Камбаркан», «Кыял» и молодые исполнительницы горлового пения. " +
        "Ведущий — народный артист КР. После концерта — мастер-класс по игре на комузе для всех желающих. " +
        "Семейный формат, дети до 7 лет — бесплатно.",
        "http://ticket-booking-system.local/simple-concert-service/swagger/index.html",
        "г. Бишкек, пр. Чуй 114, Кыргызский национальный театр оперы и балета"
    )
});
app.Run();