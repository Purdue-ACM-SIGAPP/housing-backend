/*using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SimpleWebAppReact;
/// <summary>
/// runs startup commands, builds front end, CORS
/// </summary>
public class Startup
{
    /// <summary>
    /// start up
    /// </summary>
    /// <param name="configuration"></param>
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public IConfiguration Configuration { get; }  

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddHttpClient();
        services.AddSingleton<BuildingOutlineService>();

        // Configure CORS to allow requests from React Native frontend
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                 builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });
        services.AddMvc();

        // 1. Add Authentication Services
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "https://dev-2gowyyl3kin685ua.us.auth0.com/";
                options.Audience = "http://localhost:5128";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    RoleClaimType = "https://my-app.example.com/roles" // Match the namespace in your token
                };
            });

        services.AddAuthorization();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("/index.html");
        });
    }
}*/