using System.Text;
using Eshop.Server.Application.Interfaces;
using Eshop.Server.Application.ApplicationServices;
using Eshop.Server.Infrastructure.Auth;
using Eshop.Server.Infrastructure.Persistence;
using Eshop.Server.Infrastructure.Persistence.Repositories;
using Eshop.Server.Infrastructure.Payments;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ===========================
        // CORS: consente al frontend Angular (localhost:4200) di chiamare le API
        // ===========================
        var corsPolicyName = "AllowFrontend";

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(corsPolicyName, policy =>
            {
                policy
                    .WithOrigins("http://localhost:4200") // dev server Angular
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        // 1. DbContext / EF Core (MySQL)
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' non trovata. Impostala in appsettings o via variabili d'ambiente (Docker Compose).");

            // Fix pulita: niente AutoDetect (evita connessione immediata mentre MySQL sta ancora partendo)
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 0)); // MySQL 8.x

            options.UseMySql(connString, serverVersion, mySqlOptions =>
            {
                mySqlOptions.EnableRetryOnFailure();
            });

        });


        // 2. Repository (Infrastructure)
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<ICartRepository, CartRepository>();

        // 3. Servizi applicativi
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<OrderService>();
        builder.Services.AddScoped<CartService>();
        builder.Services.AddScoped<AuthService>();

        // 3.1 Servizio pagamenti (per OrderService)
        builder.Services.AddScoped<IPaymentService, FakePaymentService>();

        // 4. Password hasher
        builder.Services.AddScoped<IPasswordHasher<string>, PasswordHasher<string>>();

        // 5. JWT service
        builder.Services.AddScoped<IJwtService, JwtService>();

        // 6. Configurazione autenticazione JWT
        var jwtSection = builder.Configuration.GetSection("Jwt");
        var key = jwtSection.GetValue<string>("Key") ?? "DEV-KEY-TROPPO-CORTA-CAMBIALA";
        var issuer = jwtSection.GetValue<string>("Issuer") ?? "eshop";
        var audience = jwtSection.GetValue<string>("Audience") ?? "eshop-client";

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        // 6.1 Autorizzazione con policy
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("OnlyCustomer", policy =>
                policy.RequireClaim("is_admin", "false"))
            .AddPolicy("OnlyAdmin", policy =>
                policy.RequireClaim("is_admin", "true"));

        // 7. Controller + Swagger
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(o =>
        {
            o.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Eshop API",
                Version = "v1",
                Description = "API E-commerce per progetto tesi"
            });

            o.EnableAnnotations();

            var xmlName = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlName);
            if (File.Exists(xmlPath))
                o.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

            o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Inserisci il token JWT così: Bearer {token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // 8. Seeder nel DI
        builder.Services.AddScoped<DatabaseSeeder>();

        var app = builder.Build();

        // 9. Pipeline HTTP
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Eshop API v1");
        });

        // CORS deve stare prima di Authentication/Authorization e MapControllers
        app.UseCors(corsPolicyName);

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // 10. Esegui il seeder ma NON bloccare l'app se fallisce
        using (var scope = app.Services.CreateScope())
        {
            try
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
                await seeder.SeedAsync(scope.ServiceProvider);
            }
            catch (Exception ex)
            {
                // così Swagger resta accessibile
                Console.WriteLine("ERRORE SEEDER: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        app.Run();
    }
}
