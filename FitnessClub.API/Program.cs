using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

// Minimale services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS voor MAUI - ALLES TOESTAAN
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()       // Vanuit MAUI
              .AllowAnyMethod()       // GET, POST, etc.
              .AllowAnyHeader();      // Alle headers
    });
});

var app = builder.Build();

// Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS gebruiken
app.UseCors("AllowAll");

// MAUI heeft deze nodig
app.UseAuthorization();

// De endpoints die je MAUI app nodig heeft
app.MapGet("/api/health", () =>
{
    Console.WriteLine($"Health check op: {DateTime.Now}");
    return Results.Ok(new
    {
        status = "API is running",
        timestamp = DateTime.Now,
        message = "Ready for MAUI app"
    });
});

// Login endpoint - SIMPELE VERSIE
app.MapPost("/api/gebruikers/login", (LoginRequest request) =>
{
    Console.WriteLine($"Login poging: {request.Email}");

    // Simpele hardcoded login - GEEN DATABASE
    if (request.Email == "admin@fitness.com" && request.Password == "admin123")
    {
        return Results.Ok(new
        {
            success = true,
            token = "fake-jwt-token-admin-12345",
            email = "admin@fitness.com",
            voornaam = "Admin",
            achternaam = "User",
            id = "1",
            roles = new[] { "Admin" }
        });
    }

    if (request.Email == "user@fitness.com" && request.Password == "user123")
    {
        return Results.Ok(new
        {
            success = true,
            token = "fake-jwt-token-user-67890",
            email = "user@fitness.com",
            voornaam = "Regular",
            achternaam = "User",
            id = "2",
            roles = new[] { "Gebruiker" }
        });
    }

    // Als login mislukt
    return Results.Unauthorized();
});

// Lessen endpoint - SIMPELE DATA
app.MapGet("/api/lessen", () =>
{
    var lessen = new[]
    {
        new {
            id = 1,
            naam = "Morning Yoga",
            beschrijving = "Ontspannende yoga sessie",
            startTijd = DateTime.Today.AddDays(1).AddHours(9),
            eindTijd = DateTime.Today.AddDays(1).AddHours(10),
            locatie = "Zaal 1",
            trainer = "Anna",
            maxDeelnemers = 20,
            isActief = true,
            lastSynced = DateTime.Now
        },
        new {
            id = 2,
            naam = "HIIT Workout",
            beschrijving = "High Intensity Interval Training",
            startTijd = DateTime.Today.AddDays(2).AddHours(18),
            eindTijd = DateTime.Today.AddDays(2).AddHours(19),
            locatie = "Zaal 2",
            trainer = "Mike",
            maxDeelnemers = 15,
            isActief = true,
            lastSynced = DateTime.Now
        }
    };

    return Results.Ok(new
    {
        success = true,
        data = lessen,
        message = $"{lessen.Length} lessen geladen"
    });
});

// Token validatie endpoint
app.MapGet("/api/gebruikers/validate", () =>
{
    // Altijd true teruggeven voor test
    return Results.Ok(new { valid = true });
});

// 🔴 BELANGRIJK: LUISTER OP ALLE IP's
Console.WriteLine("🚀 Starting FitnessClub API...");
Console.WriteLine("📡 Listening on ALL interfaces (0.0.0.0:5000)");
Console.WriteLine("🌐 Swagger UI: http://localhost:5000/swagger");
Console.WriteLine("📱 MAUI endpoint: http://172.20.96.1:5000/api/");

// 🔴 DEZE REGEL IS KRITIEK: 0.0.0.0 betekent "luister op alle IP's"
app.Run("http://0.0.0.0:5000");

// DTO voor login
public record LoginRequest(string Email, string Password);