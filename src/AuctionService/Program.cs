using AuctionService.Consumers;
using AuctionService.Data;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Dodaj servise za kontrolere
builder.Services.AddControllers();

// Dodaj Swagger servise
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext konfiguracija
builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// ✅ Registracija MassTransita u AuctionService
builder.Services.AddMassTransit(x=>
{

    x.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);
        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction",false));

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", h =>
        {
            h.Username(builder.Configuration.GetValue("RabbitMq:Username" ,"guest"));
            h.Password(builder.Configuration.GetValue("RabbitMq:Password" ,"guest"));
        });
        cfg.ConfigureEndpoints(context);
    });
});

// AutoMapper konfiguracija
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme
).AddJwtBearer(opt=>
{
    opt.Authority = builder.Configuration["IdentityServiceUrl"];
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters.ValidateAudience = false;
    opt.TokenValidationParameters.NameClaimType = "username";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auction API v1");
        c.RoutePrefix = "swagger"; // Swagger će biti na /swagger
    });
}


// Omogući routing i mapiranje kontrolera

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();

// Inicijalizacija podataka u bazi
try
{
    DbInitializer.InitiData(app);
}
catch (Exception ex)
{
    Console.WriteLine($"Error during DB initialization: {ex.Message}");
}

app.Run();
