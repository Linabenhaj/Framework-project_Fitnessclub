using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using FitnessClub.Web.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<FitnessClubDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IFitnessClubDbContext>(sp =>
    sp.GetRequiredService<FitnessClubDbContext>());

// Identity
builder.Services.AddIdentity<Gebruiker, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<FitnessClubDbContext>()
.AddDefaultUI()
.AddDefaultTokenProviders();

//  Meertaligheid (NL / EN / FR) 
// FitnessClub.Web.Resources, localizer zoekt automatisch in de juiste map
builder.Services.AddLocalization();

builder.Services.AddControllersWithViews(options =>
    {
        
        // automatisch als [Required] behandelt — anders krijg je validatie-fouten
        
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();
builder.Services.AddRazorPages();
builder.Services.AddMvc().AddRazorRuntimeCompilation();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("nl"),
        new CultureInfo("en"),
        new CultureInfo("fr")
    };
    options.DefaultRequestCulture = new RequestCulture("nl");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Lokalisatie pipeline 
var localizationOptions = app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

// Eigen middleware logt elk request
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FitnessClubDbContext>();
        var userManager = services.GetRequiredService<UserManager<Gebruiker>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        // Min. 3 rollen voor de criteria Admin, Lid, Trainer
        
        string[] roles = { "Admin", "Lid", "Trainer" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

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

        // Demo Trainer-account aanmaken
        var trainerEmail = "trainer@fitnessclub.be";
        var trainerUser = await userManager.FindByEmailAsync(trainerEmail);
        if (trainerUser == null)
        {
            trainerUser = new Gebruiker
            {
                UserName = trainerEmail,
                Email = trainerEmail,
                Voornaam = "Trainer",
                Achternaam = "Demo",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(trainerUser, "Trainer123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(trainerUser, "Trainer");
        }

        // Abonnementen seeden — Basic / Medium / Pro
        if (!context.Abonnementen.Any())
        {
            context.Abonnementen.AddRange(
                new Abonnement { Naam = "Basic",  Type = "Basic",  Prijs = 19.99m, DuurInMaanden = 1, Beschrijving = "Toegang tot fitnesszaal",          IsActief = true },
                new Abonnement { Naam = "Medium", Type = "Medium", Prijs = 34.99m, DuurInMaanden = 1, Beschrijving = "Fitnesszaal + groepslessen",        IsActief = true },
               
                new Abonnement { Naam = "Pro",    Type = "Pro",    Prijs = 54.99m, DuurInMaanden = 1, Beschrijving = "Alles + persoonlijke begeleiding",  IsActief = true }
            );
            await context.SaveChangesAsync();
        }
        else
        {
            var renamings = new Dictionary<string, (string Naam, decimal Prijs, string Beschrijving)>
            {
                { "Basis",   ("Basic",  19.99m, "Toegang tot fitnesszaal") },
                { "Student", ("Medium", 34.99m, "Fitnesszaal + groepslessen") },
                { "Premium", ("Pro",    54.99m, "Alles + persoonlijke begeleiding") }
            };
            foreach (var abo in context.Abonnementen.ToList())
            {
                if (renamings.TryGetValue(abo.Naam, out var nieuw))
                {
                    abo.Naam = nieuw.Naam;
                    abo.Type = nieuw.Naam;
                    abo.Prijs = nieuw.Prijs;
                    abo.Beschrijving = nieuw.Beschrijving;
                    abo.GewijzigdOp = DateTime.UtcNow;
                }
            }
            await context.SaveChangesAsync();
        }

        // Mock-lessen opruimen (oude seeded data)
        var mockTrainers = new[] { "John Smith", "Marie Dubois", "Lukas Janssens", "Sophie Peeters", "Anna Verhoeven", "Tom De Wit" };
        var mockLessen = context.Lessen.Where(l => mockTrainers.Contains(l.Trainer)).ToList();
        if (mockLessen.Any())
        {
            context.Lessen.RemoveRange(mockLessen);
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Fout tijdens database seeding.");
    }
}

app.Run();
