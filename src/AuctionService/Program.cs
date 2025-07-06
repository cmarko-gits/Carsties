using AuctionService.Data;
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

// AutoMapper konfiguracija
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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

app.UseHttpsRedirection();

// Omogući routing i mapiranje kontrolera
app.MapControllers();

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
