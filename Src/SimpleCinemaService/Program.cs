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

app.UsePathBase("/simple-cinema-service");

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

apiV1.MapGet("/cinemas", () => new List<SimpleCinema>()
{
    new SimpleCinema(
        Guid.Parse("47e7674a-e730-445e-a37e-9e48d093f476"),
        350,
        new DateTime(2026, 9, 25, 18, 0, 0, DateTimeKind.Utc),
        "Игла",
        "Легендарный фильм Рашида Нугманова с Виктором Цоем в главной роли. " +
        "Реставрированная копия в 4K, специальный показ к 35-летию выхода. " +
        "Перед сеансом — короткая лекция о казахской новой волне и влиянии фильма на постсоветский кинематограф.",
        "http://ticket-booking-system.local/simple-cinema-service/swagger/index.html",
        "г. Бишкек, ЦУМ Айчурок, кинотеатр «Ала-Тоо Cinema», зал 1"
    ),
    new SimpleCinema(
        Guid.Parse("a1b2c3d4-1111-2222-3333-444455556667"),
        450,
        new DateTime(2026, 10, 2, 20, 30, 0, DateTimeKind.Utc),
        "Дюна: Часть третья",
        "Премьера в Кыргызстане. Завершение трилогии Дени Вильнёва по романам Фрэнка Герберта. " +
        "IMAX-показ, оригинальная звуковая дорожка с кыргызскими субтитрами. " +
        "Хронометраж 2 ч 45 мин, без антракта.",
        "http://ticket-booking-system.local/simple-cinema-service/swagger/index.html",
        "г. Бишкек, пр. Чуй 155, кинотеатр «Манас IMAX», зал IMAX"
    ),
    new SimpleCinema(
        Guid.Parse("b2c3d4e5-2222-3333-4444-555566667778"),
        300,
        new DateTime(2026, 10, 15, 19, 0, 0, DateTimeKind.Utc),
        "Курманжан Датка",
        "Историческая драма о легендарной алайской правительнице Курманжан Датке, " +
        "сыгравшей ключевую роль в присоединении Кыргызстана к России. " +
        "Фильм Сатыбалады Жумадилова, восстановленная версия. Показ приурочен к 215-летию со дня рождения.",
        "http://ticket-booking-system.local/simple-cinema-service/swagger/index.html",
        "г. Бишкек, ул. Логвиненко 17, Дом кино им. Ч. Айтматова"
    ),
    new SimpleCinema(
        Guid.Parse("c3d4e5f6-3333-4444-5555-666677778889"),
        400,
        new DateTime(2026, 11, 8, 21, 0, 0, DateTimeKind.Utc),
        "Оппенгеймер",
        "Ночной показ байопика Кристофера Нолана о создателе атомной бомбы. " +
        "Оригинальная версия с русскими субтитрами, формат 70mm. " +
        "Вход для зрителей 18+, перед началом — короткое вступление кинокритика.",
        "http://ticket-booking-system.local/simple-cinema-service/swagger/index.html",
        "г. Бишкек, ул. Ибраимова 115, кинотеатр «Cinematica», зал 2"
    ),
    new SimpleCinema(
        Guid.Parse("d4e5f6a7-4444-5555-6666-777788889999"),
        250,
        new DateTime(2026, 11, 22, 12, 0, 0, DateTimeKind.Utc),
        "Тайна Коко",
        "Семейный показ анимационного фильма Pixar на кыргызском языке. " +
        "Дубляж студии «Кыргызфильм» с участием известных кыргызских актёров озвучки. " +
        "После сеанса — мастер-класс по изготовлению бумажных алтарей в фойе кинотеатра. Дети до 5 лет — бесплатно.",
        "http://ticket-booking-system.local/simple-cinema-service/swagger/index.html",
        "г. Бишкек, ул. Киевская 95, кинотеатр «Октябрь», большой зал"
    )
});

app.Run();
