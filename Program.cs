using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using LinguaQuest.Services;
using LinguaQuest.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.AddScoped<UserSessionStore>();


var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>() ?? new MongoDbSettings();
mongoSettings.ConnectionString = ResolveMongoConnectionString(builder.Configuration, mongoSettings.ConnectionString);
if (string.IsNullOrWhiteSpace(mongoSettings.DatabaseName))
{
    mongoSettings.DatabaseName = "LinguaQuestDb";
}

// Впровадження залежностей (Dependency Injection) контексту MongoDB
builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton(new MongoDbContext(mongoSettings));
// Run DB initialization (indexes + seeding) in background to support remote clusters and avoid startup crashes
builder.Services.AddHostedService<MongoInitializer>();

// Реєструємо сервіси застосунку
builder.Services.AddScoped<IWordService, WordService>();
builder.Services.AddScoped<IUserService, UserService>();
// Register concrete types as well for components that inject the concrete service
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<WordService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

static string ResolveMongoConnectionString(IConfiguration configuration, string? configuredConnectionString)
{
    var mongoUri = configuration["MONGODB_URI"];
    if (!string.IsNullOrWhiteSpace(mongoUri))
    {
        return mongoUri;
    }

    if (!string.IsNullOrWhiteSpace(configuredConnectionString) &&
        !configuredConnectionString.Contains("<db_password>", StringComparison.OrdinalIgnoreCase) &&
        !configuredConnectionString.Contains("<", StringComparison.Ordinal))
    {
        return configuredConnectionString;
    }

    var fallback = configuration.GetConnectionString("MongoDb");
    if (!string.IsNullOrWhiteSpace(fallback))
    {
        return fallback;
    }

    return "mongodb://localhost:27017";
}