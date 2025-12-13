using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//  the container
builder.Services.AddControllersWithViews();

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = "Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true";
}

builder.Services.AddDbContext<FitnessClubDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity Configuration
builder.Services.AddIdentity<Gebruiker, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<FitnessClubDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI(); 

// Configure Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
});

//  MVC with Runtime Compilation
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

//  Razor Pages 
builder.Services.AddRazorPages();

//  Session 
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
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
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<FitnessClubDbContext>();
        var userManager = services.GetRequiredService<UserManager<Gebruiker>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();


        // Apply migrations
        await context.Database.MigrateAsync();

        // Seed roles and users
        await SeedDatabase(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

await app.RunAsync();

// Seed Database Method
async Task SeedDatabase(FitnessClubDbContext context, UserManager<Gebruiker> userManager, RoleManager<IdentityRole> roleManager)
{
    // Create roles if they don't exist
    string[] roles = { "Admin", "Trainer", "Lid" };

    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Create admin user if it doesn't exist
    if (await userManager.FindByEmailAsync("admin@fitness.com") == null)
    {
        var adminUser = new Gebruiker
        {
            UserName = "admin@fitness.com",
            Email = "admin@fitness.com",
            EmailConfirmed = true,
            Voornaam = "Admin",
            Achternaam = "User",
            PhoneNumber = "0123456789",
            Geboortedatum = new DateTime(1980, 1, 1),
            Rol = "Admin",
            AangemaaktOp = DateTime.UtcNow
        };

        await userManager.CreateAsync(adminUser, "Admin123!");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    // Add default abonnementen if none exist
    if (!context.Abonnementen.Any())
    {
        context.Abonnementen.AddRange(
            new Abonnement
            {
                Naam = "Basic",
                Prijs = 29.99m,
                Omschrijving = "Basis abonnement",
                LooptijdMaanden = 1,
                IsActief = true,
                AangemaaktOp = DateTime.UtcNow
            },
            new Abonnement
            {
                Naam = "Premium",
                Prijs = 49.99m,
                Omschrijving = "Premium abonnement",
                LooptijdMaanden = 1,
                IsActief = true,
                AangemaaktOp = DateTime.UtcNow
            },
            new Abonnement
            {
                Naam = "VIP",
                Prijs = 79.99m,
                Omschrijving = "VIP abonnement",
                LooptijdMaanden = 1,
                IsActief = true,
                AangemaaktOp = DateTime.UtcNow
            }
        );
        await context.SaveChangesAsync();
    }
}