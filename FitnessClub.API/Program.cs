using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<FitnessClubDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<Gebruiker, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<FitnessClubDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "FitnessClub",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "FitnessClubUsers",
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "JWT_SECRET_KEY_MIN_32_CHARACTERS_LONG!"))
    };
});

// CORS voor MAUI
builder.Services.AddCors(options =>
{
    options.AddPolicy("MauiApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5062",    // Windows
                "http://localhost:5070",    // Alternatieve poort
                "http://10.0.2.2:5062",     // Android emulator
                "http://10.0.2.2:5070"      // Android alternatief
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS moet voor Authentication komen
app.UseCors("MauiApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Maak database aan als die niet bestaat
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FitnessClubDbContext>();
    dbContext.Database.EnsureCreated();

    // Seed test data als database leeg is
    if (!dbContext.Users.Any())
    {
        await SeedTestData(dbContext, scope.ServiceProvider);
    }
}

app.Run();

async Task SeedTestData(FitnessClubDbContext context, IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<Gebruiker>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Maak rollen
    var roles = new[] { "Admin", "Lid", "Trainer" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Maak test abonnementen
    var abonnementen = new[]
    {
        new Abonnement { Naam = "Basis", Prijs = 29.99m, LooptijdMaanden = 1, Omschrijving = "Toegang tot alle lessen" },
        new Abonnement { Naam = "Premium", Prijs = 49.99m, LooptijdMaanden = 1, Omschrijving = "Toegang + persoonlijke trainer" },
        new Abonnement { Naam = "Jaarabonnement", Prijs = 299.99m, LooptijdMaanden = 12, Omschrijving = "Jaarabonnement met 2 maanden gratis" }
    };

    await context.Abonnementen.AddRangeAsync(abonnementen);
    await context.SaveChangesAsync();

    // Maak test gebruikers
    var adminUser = new Gebruiker
    {
        UserName = "admin@fitness.com",
        Email = "admin@fitness.com",
        Voornaam = "Admin",
        Achternaam = "User",
        Geboortedatum = new DateTime(1980, 1, 1),
        EmailConfirmed = true,
        AbonnementId = abonnementen[1].Id
    };

    var result = await userManager.CreateAsync(adminUser, "Admin123!");
    if (result.Succeeded)
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    var lidUser = new Gebruiker
    {
        UserName = "lid@fitness.com",
        Email = "lid@fitness.com",
        Voornaam = "Lid",
        Achternaam = "User",
        Geboortedatum = new DateTime(1990, 1, 1),
        EmailConfirmed = true,
        AbonnementId = abonnementen[0].Id
    };

    result = await userManager.CreateAsync(lidUser, "Lid123!");
    if (result.Succeeded)
    {
        await userManager.AddToRoleAsync(lidUser, "Lid");
    }

    var trainerUser = new Gebruiker
    {
        UserName = "trainer@fitness.com",
        Email = "trainer@fitness.com",
        Voornaam = "Trainer",
        Achternaam = "User",
        Geboortedatum = new DateTime(1985, 1, 1),
        EmailConfirmed = true,
        AbonnementId = abonnementen[2].Id
    };

    result = await userManager.CreateAsync(trainerUser, "Trainer123!");
    if (result.Succeeded)
    {
        await userManager.AddToRoleAsync(trainerUser, "Trainer");
    }

    // Maak test lessen
    var lessen = new[]
    {
        new Les
        {
            Naam = "Yoga Beginners",
            Beschrijving = "Ontspannende yoga voor beginners",
            StartTijd = DateTime.Now.AddDays(1).AddHours(10),
            EindTijd = DateTime.Now.AddDays(1).AddHours(11),
            Locatie = "Zaal 1",
            Trainer = "Lisa",
            MaxDeelnemers = 15,
            IsActief = true
        },
        new Les
        {
            Naam = "HIIT Training",
            Beschrijving = "High Intensity Interval Training",
            StartTijd = DateTime.Now.AddDays(2).AddHours(18),
            EindTijd = DateTime.Now.AddDays(2).AddHours(19),
            Locatie = "Zaal 2",
            Trainer = "Mike",
            MaxDeelnemers = 20,
            IsActief = true
        },
        new Les
        {
            Naam = "Spinning",
            Beschrijving = "Cardio op muziek",
            StartTijd = DateTime.Now.AddDays(3).AddHours(9),
            EindTijd = DateTime.Now.AddDays(3).AddHours(10),
            Locatie = "Spinningzaal",
            Trainer = "Sarah",
            MaxDeelnemers = 25,
            IsActief = true
        }
    };

    await context.Lessen.AddRangeAsync(lessen);
    await context.SaveChangesAsync();

    Console.WriteLine("Test data seeded successfully!");
}