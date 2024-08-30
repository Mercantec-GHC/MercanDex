using System.Text;
using API.Context;
using API.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    name: MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    }
                );
            });

            builder.Services.AddHttpClient();
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            IConfiguration Configuration = builder.Configuration;

            // Retrieve the connection string from configuration or environment variables
            string connectionString = Configuration.GetConnectionString("DefaultConnection")
                                      ?? Environment.GetEnvironmentVariable("CONNECTION_STRINGS_DEFAULT_CONNECTION");

            builder.Services.AddDbContext<AppDBContext>(options =>
                options.UseNpgsql(connectionString));

            // Retrieve JWT settings from configuration or environment variables
            var jwtKey = Configuration["JwtSettings:Key"] ?? Environment.GetEnvironmentVariable("JWT_SETTINGS_KEY");
            var jwtIssuer = Configuration["JwtSettings:Issuer"] ?? Environment.GetEnvironmentVariable("JWT_SETTINGS_ISSUER");
            var jwtAudience = Configuration["JwtSettings:Audience"] ?? Environment.GetEnvironmentVariable("JWT_SETTINGS_AUDIENCE");


            // Check if critical JWT settings are missing
            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
            {
                throw new InvalidOperationException("JWT settings are not configured properly.");
            }

            // Configure JWT Authentication
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

            // Retrieve custom settings from configuration or environment variables
            var accessKey = Configuration["AccessKey"] ?? Environment.GetEnvironmentVariable("ACCESS_KEY");
            var secretKey = Configuration["SecretKey"] ?? Environment.GetEnvironmentVariable("SECRET_KEY");

            builder.Services.AddSingleton(new AppConfiguration
            {
                AccessKey = accessKey,
                SecretKey = secretKey
            });

            var app = builder.Build();

            // Ensure the CORS middleware runs before the endpoint routing middleware
            app.UseCors(MyAllowSpecificOrigins);

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
