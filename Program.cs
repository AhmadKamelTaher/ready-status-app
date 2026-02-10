using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReadyStatusApp.Data;
using ReadyStatusApp.Hubs;
using ReadyStatusApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Database Context - Support both SQL Server (local) and PostgreSQL (cloud)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (databaseProvider == "PostgreSQL" || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DATABASE_URL")))
    {
        // Use PostgreSQL for cloud deployment
        var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL") ?? connectionString;
        options.UseNpgsql(dbUrl);
    }
    else
    {
        // Use SQL Server for local development
        options.UseSqlServer(connectionString);
    }
});

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings (relaxed for development)
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Add SignalR for real-time communication
builder.Services.AddSignalR();

var app = builder.Build();

// Seed admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        
        // Apply migrations automatically
        context.Database.Migrate();

        // Create admin user if not exists
        var adminEmail = "admin@readystatus.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Admin",
                IsAdmin = true,
                EmailConfirmed = true
            };
            
            await userManager.CreateAsync(adminUser, "Admin123");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Map SignalR hub
app.MapHub<StatusHub>("/statusHub");

app.Run();
