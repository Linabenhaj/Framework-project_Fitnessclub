using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

// Basis services registreren voor API functionaliteit
builder.Services.AddControllers();  // Voegt controller ondersteuning toe
builder.Services.AddEndpointsApiExplorer();  // API explorer voor endpoints
builder.Services.AddSwaggerGen();  // Genereert Swagger documentatie

// CORS configuratie - staat alle cross-origin requests toe voor MAUI
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()       // Accepteert requests vanaf elke bron
              .AllowAnyMethod()       // Staat alle HTTP methodes toe (GET, POST, etc.)
              .AllowAnyHeader();      // Accepteert alle headers
    });
});

var app = builder.Build();

// Swagger UI alleen in development omgeving
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // Voegt Swagger middleware toe
    app.UseSwaggerUI();  // Voegt Swagger UI interface toe
}

// CORS policy toepassen
app.UseCors("AllowAll");

// Authorization middleware toevoegen
app.UseAuthorization();

// Health check endpoint - controleert of API draait
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

// Login endpoint - verwerkt gebruikersauthenticatie
app.MapPost("/api/gebruikers/login", (LoginRequest request) =>
{
    Console.WriteLine($"Login poging: {request.Email}");

    // Hardcoded login voor testdoeleinden
    if (request.Email == "admin@fitness.com" && request.Password == "admin123")
    {
        return Results.Ok(new
        {
            success = true,
            token = "fake-jwt-token-admin-12345",  // Test token voor admin
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
            token = "fake-jwt-token-user-67890",  // Test token voor gebruiker
            email = "user@fitness.com",
            voornaam = "Regular",
            achternaam = "User",
            id = "2",
            roles = new[] { "Gebruiker" }
        });
    }

    // Retourneert unauthorized bij foute credentials
    return Results.Unauthorized();
});

// Lessen endpoint - retourneert lesdata
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

// Token validatie endpoint - controleert token geldigheid
app.MapGet("/api/gebruikers/validate", () =>
{
    // Retourneert altijd true voor testdoeleinden
    return Results.Ok(new { valid = true });
});

// Console output voor startup informatie
Console.WriteLine("🚀 Starting FitnessClub API...");
Console.WriteLine("📡 Listening on ALL interfaces (0.0.0.0:5000)");
Console.WriteLine("🌐 Swagger UI: http://localhost:5000/swagger");
Console.WriteLine("📱 MAUI endpoint: http://172.20.96.1:5000/api/");

// Start API en luister op alle netwerkinterfaces
app.Run("http://0.0.0.0:5000");

// DTO (Data Transfer Object) voor login requests
public record LoginRequest(string Email, string Password);