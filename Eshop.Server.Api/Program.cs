using System.Text;
using Eshop.Server.Application.Interfacce;
using Eshop.Server.Application.ServiziApplicativi;
using Eshop.Server.Infrastructure.Auth;
using Eshop.Server.Infrastructure.Persistenza;
using Eshop.Server.Infrastructure.Persistenza.Repositories;
using Eshop.Server.Infrastructure.Pagamenti;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

internal class Program
{
    // Metodo Main asincrono
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. DbContext / EF Core (MySQL)
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            var connString = builder.Configuration.GetConnectionString("DefaultConnection")
                             // fallback nel caso tu non l'abbia messa nell'appsettings
                             ?? "Server=localhost;Port=3306;Database=eshopdb;User=root;Password=5tr3g49ll4;";
            options.UseMySql(connString, ServerVersion.AutoDetect(connString));
        });

        // 2. Repository (Infrastructure)
        builder.Services.AddScoped<IUtenteRepository, UtenteRepository>();
        builder.Services.AddScoped<IProdottoRepository, ProdottoRepository>();
        builder.Services.AddScoped<IOrdineRepository, OrdineRepository>();
        builder.Services.AddScoped<ICarrelloRepository, CarrelloRepository>();

        // 3. Servizi applicativi
        builder.Services.AddScoped<UtenteService>();
        builder.Services.AddScoped<ProdottoService>();
        builder.Services.AddScoped<OrdineService>();
        builder.Services.AddScoped<CarrelloService>();
        builder.Services.AddScoped<AuthService>();

        // 3.1 Servizio pagamenti (per soddisfare OrdineService)
        builder.Services.AddScoped<IPagamentoService, FakePagamentoService>();

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
                                            .AddPolicy("OnlyCliente", policy =>
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

        // 8. Registrare il Seeder nel DI
        builder.Services.AddScoped<DatabaseSeeder>();

        var app = builder.Build();

        // 9. Pipeline HTTP
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Eshop API v1");
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // **Esegui il Seeder durante l'avvio dell'applicazione (asincrono)**
        using (var scope = app.Services.CreateScope())
        {
            // Ottieni l'istanza del seeder dal DI
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            // Esegui il seeding asincrono
            await seeder.SeedAsync(scope.ServiceProvider);
        }

        app.Run();
    }
}
