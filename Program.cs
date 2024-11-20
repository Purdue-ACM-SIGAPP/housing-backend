using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
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
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddIdentity<User, IdentityRole>();
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
