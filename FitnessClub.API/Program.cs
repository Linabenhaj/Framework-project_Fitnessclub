using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ========== DATABASE CONFIGURATIE ==========
builder.Services.AddDbContext<FitnessClubDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IFitnessClubDbContext>(sp =>
    sp.GetRequiredService<FitnessClubDbContext>());

// ========== IDENTITY CONFIGURATIE ==========
builder.Services.AddIdentity<Gebruiker, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;   // schoolproject: geen mailbevestiging nodig
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<FitnessClubDbContext>()
.AddDefaultTokenProviders();

// ========== JWT CONFIGURATIE ==========
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "YourVeryLongSecretKeyForJWT32CharactersMinimum");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "FitnessClubAPI",
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"] ?? "FitnessClubMobileApp",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// ========== SERVICES ==========
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Voorkomt crashes bij circulaire referenties in EF navigation properties
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        // camelCase voor alle API responses (activityId, lesId, ...)
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();

// Swagger met JWT ondersteuning
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FitnessClub API",
        Version = "v1",
        Description = "API voor FitnessClub Web en MAUI app"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS voor MAUI en Web
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ========== MIDDLEWARE PIPELINE ==========
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FitnessClub API v1");
        c.RoutePrefix = "swagger";
    });
}

// Geen HTTPS-redirect in dev — anders bereikt Android-emulator de API niet
// app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        status = "API is running",
        timestamp = DateTime.Now,
        version = "1.0",
        environment = app.Environment.EnvironmentName
    });
});

// ========== DATABASE MIGRATIE + SEEDING ==========
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FitnessClubDbContext>();
        var userManager = services.GetRequiredService<UserManager<Gebruiker>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // 1. Database aanmaken / migraties uitvoeren
        await context.Database.MigrateAsync();

        // 2. Rollen aanmaken — alleen Admin + Member (trainers staan als data op een Les)
        string[] roles = { "Admin", "Lid" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // 3. Admin-account aanmaken (als hij nog niet bestaat)
        var adminEmail = "admin@fitnessclub.be";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new Gebruiker
            {
                UserName = adminEmail,
                Email = adminEmail,
                Voornaam = "Admin",
                Achternaam = "FitnessClub",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // 4. Demo-gebruiker aanmaken
        var memberEmail = "user@fitnessclub.be";
        var memberUser = await userManager.FindByEmailAsync(memberEmail);
        if (memberUser == null)
        {
            memberUser = new Gebruiker
            {
                UserName = memberEmail,
                Email = memberEmail,
                Voornaam = "Lid",
                Achternaam = "Demo",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(memberUser, "User123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(memberUser, "Lid");
        }

        // 5. Abonnementen seeden — Basic / Medium / Pro
        if (!context.Abonnementen.Any())
        {
            context.Abonnementen.AddRange(
                new Abonnement { Naam = "Basic",  Type = "Basic",  Prijs = 19.99m, DuurInMaanden = 1, Beschrijving = "Toegang tot fitnesszaal",         IsActief = true },
                new Abonnement { Naam = "Medium", Type = "Medium", Prijs = 34.99m, DuurInMaanden = 1, Beschrijving = "Fitnesszaal + groepslessen",       IsActief = true },
                new Abonnement { Naam = "Pro",    Type = "Pro",    Prijs = 54.99m, DuurInMaanden = 1, Beschrijving = "Alles + persoonlijke begeleiding", IsActief = true }
            );
            await context.SaveChangesAsync();
        }

        // 6. Mock-lessen opruimen (van eerdere seeding) — admin maakt lessen handmatig aan
        var mockTrainers = new[] { "John Smith", "Marie Dubois", "Lukas Janssens", "Sophie Peeters", "Anna Verhoeven", "Tom De Wit" };
        var mockLessen = await context.Lessen.Where(l => mockTrainers.Contains(l.Trainer)).ToListAsync();
        if (mockLessen.Any())
        {
            context.Lessen.RemoveRange(mockLessen);
            await context.SaveChangesAsync();
            Console.WriteLine($"🧹 {mockLessen.Count} mock-lessen verwijderd");
        }

        Console.WriteLine("✅ Database geseed (Admin + Member + abonnementen klaar)");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Fout tijdens database seeding van API.");
    }
}

Console.WriteLine("🚀 FitnessClub API gestart");
Console.WriteLine($"📡 Mode: {app.Environment.EnvironmentName}");
Console.WriteLine("🔐 JWT Authentication: ACTIEF");
Console.WriteLine("👤 Admin login: admin@fitnessclub.be / Admin123!");
Console.WriteLine("👤 Member login: user@fitnessclub.be / User123!");

app.Run();
