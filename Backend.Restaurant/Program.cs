using Backend.Restaurant.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Backend.Restaurant
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Configurar la cadena de conexión a PostgreSQL
            var connectionString = builder.Configuration.GetConnectionString("CadenaSQLPostgre");

            builder.Services.AddDbContext<AppData>(options =>
                options.UseNpgsql(connectionString));

            // 2. CONFIGURACIÓN DE CORS (conexión desde React)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "https://localhost:7166") // URL de mi Frontend React
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // 3. CONFIGURACIÓN JWT (Seguridad)
            var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key no está configurada"));

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero // Elimina los 5 minutos de tolerancia por defecto
                };
            });

            builder.Services.AddControllers();

            // Configuración de OpenAPI (Documentación)
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                // Nota: En .NET 9 con AddOpenApi, para ver la UI gráfica usualmente se visita /openapi/v1.json
            }

            app.UseHttpsRedirection();

            // 4. ACTIVAR LOS MIDDLEWARES (El orden importa)
            app.UseCors("AllowReactApp"); // Primero CORS
            app.UseAuthentication();      // Segundo Autenticación (¿Quién eres?)
            app.UseAuthorization();       // Tercero Autorización (¿Qué puedes hacer?)

            app.MapControllers();

            app.Run();
        }
    }
}