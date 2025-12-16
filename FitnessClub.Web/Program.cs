using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FitnessClubDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity
builder.Services.AddIdentity<Gebruiker, IdentityRole>()
    .AddEntityFrameworkStores<FitnessClubDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// Add localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Configure MVC met localization
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.AddRazorPages()
    .AddViewLocalization();

var supportedCultures = new[]
{
    new CultureInfo("nl"), // Nederlands
    new CultureInfo("en"), // Engels
    new CultureInfo("fr")  // Frans
};

var app = builder.Build();

// Configure  middleware
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("nl"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// SEED DATABASE
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<FitnessClubDbContext>();
    var userManager = services.GetRequiredService<UserManager<Gebruiker>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await SeedData.InitializeAsync(context, userManager, roleManager);
}

app.Run();