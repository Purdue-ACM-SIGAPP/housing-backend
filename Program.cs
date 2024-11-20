using System.Security.Claims;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SimpleWebAppReact;
using SimpleWebAppReact.Entities;
using SimpleWebAppReact.Services;

var builder = WebApplication.CreateBuilder(args);

// Add logging configuration
builder.Logging.AddConsole(); // Enable console logging

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Swagger config
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });

    option.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Description = "Auth0 OAuth2 Authorization",
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://dev-mkdb0weeluguzopu.us.auth0.com/authorize"),
                TokenUrl = new Uri("https://dev-mkdb0weeluguzopu.us.auth0.com/oauth/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "read:api", "Access the API" } // Define your scopes
                }
            }
        }
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "OAuth2"
                }
            },
            new[] { "read:api" }
        }
    });
});

builder.Services.AddSingleton<MongoDbService>();
// Here, configure User
var connectionString = builder.Configuration.GetConnectionString("DbConnection");
var databaseName = builder.Configuration.GetConnectionString("DatabaseName");
builder.Services.AddIdentity<User, MongoIdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
}).AddMongoDbStores<User, MongoIdentityRole<string>, string>(connectionString, databaseName);

builder.Services.AddHttpClient<BuildingOutlineService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.Authority = "https://dev-mkdb0weeluguzopu.us.auth0.com/";
    options.Audience = "http://localhost:5128";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Configure logging
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started.");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowAll"); // Enable CORS
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapFallbackToFile("/index.html"); // This will serve index.html for any routes not handled by the controller
});


app.UseSwagger();
app.UseSwaggerUI();
app.Run();
