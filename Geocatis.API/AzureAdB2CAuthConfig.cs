// Configuration de l’authentification Azure AD B2C pour Geocatis.API
// À placer dans Program.cs ou Startup.cs selon la version .NET

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// ...configuration existante...

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

// ...configuration existante...

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// ...configuration existante...

app.Run();
