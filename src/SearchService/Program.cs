using System.Net;
using MassTransit;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using MongoDB.Entities;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Model;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Dodaj HttpClient sa timeout od 5 minuta i retry politikom
builder.Services.AddHttpClient<AuctionSvcHttpClient>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(5);
})
.AddPolicyHandler(GetPolicy());

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.AddConsumer<AuctionUpdatedConsumer>(); // <--- OVO MORA BITI TU

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("search-auction-created", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
            e.ConfigureConsumer<AuctionUpdatedConsumer>(context);
        });

           cfg.Host(builder.Configuration["RabbitMq:Host"], "/", h =>
        {
            h.Username(builder.Configuration.GetValue("RabbitMq:Username" ,"guest"));
            h.Password(builder.Configuration.GetValue("RabbitMq:Password" ,"guest"));
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

var app = builder.Build();

// Swagger UI u Development modu
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Lifetime.ApplicationStarted.Register( async () =>

{
try
{
    await DbInitializer.InitDb(app);
}
catch (Exception ex)
{
    Console.WriteLine("Greška prilikom inicijalizacije baze:");
    Console.WriteLine(ex.ToString());
}

});
app.UseHttpsRedirection();

app.MapControllers();

app.Run();

// Polly retry politika - beskonačno retry sa pauzom od 3 sekunde
static IAsyncPolicy<HttpResponseMessage> GetPolicy()
    => HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
