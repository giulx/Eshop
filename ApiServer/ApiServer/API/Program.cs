using ApiServer.API.Mapping;        // Profilo AutoMapper
using ApiServer.Application.Interfaces;
using ApiServer.Application.Services;
using ApiServer.Infrastructure.Persistence;
using ApiServer.Infrastructure.Repositories;
using ApiServer.Facades;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// =======================================================
// 1️⃣ Creazione del builder
// =======================================================
var builder = WebApplication.CreateBuilder(args);

// =======================================================
// 2️⃣ Registrazione dei servizi e configurazioni di base
// =======================================================

// 🔹 2a. CORS (Cross-Origin)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 🔹 2b. Controllers
builder.Services.AddControllers();

// 🔹 2c. Database con EF Core (MySQL)
builder.Services.AddDbContext<ApiServerContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// 🔹 2d. AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 🔹 2e. Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 2f. Application Services & Repositories
builder.Services.AddScoped<IUtenteService, UtenteService>();
builder.Services.AddScoped<IProdottoService, ProdottoService>();
builder.Services.AddScoped<IOrdineService, OrdineService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddScoped<IRepositoryUtente, RepositoryUtente>();
builder.Services.AddScoped<IRepositoryProdotto, RepositoryProdotto>();
builder.Services.AddScoped<IRepositoryOrdine, RepositoryOrdine>();

// 🔹 2g. Facade
builder.Services.AddScoped<PaymentFacade>();

// 🔹 2h. JWT Authentication (facoltativo)
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "secret_key_minimale"; // meglio leggere da config
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

// =======================================================
// 3️⃣ Costruzione dell'app
// =======================================================
var app = builder.Build();

// =======================================================
// 4️⃣ Middleware pipeline
// =======================================================
app.UseCors("AllowAngular");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// 🔹 Swagger solo in sviluppo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// =======================================================
// 5️⃣ Database + Seed
// =======================================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApiServerContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db);
}

// =======================================================
// 6️⃣ Mappa i controller
// =======================================================
app.MapControllers();

// =======================================================
// 7️⃣ Avvio dell'applicazione
// =======================================================
app.Run();

