using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Data;
using TaskManager.Models;

var builder = WebApplication.CreateBuilder(args);

// Use Azure port
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");


// -------------------
// Database (Azure SQL)
// -------------------
builder.Services.AddDbContext<TaskContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure(
            maxRetryCount: 10,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        )
    ));


// -------------------
// Identity
// -------------------
builder.Services
    .AddIdentityCore<ApplicationUser>(options => { })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<TaskContext>()
    .AddDefaultTokenProviders();


// -------------------
// JWT Authentication
// -------------------
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "taskmanager",
            ValidAudience = "taskmanager",
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

// -------------------
// Controllers & OpenAPI
// -------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(); // .NET 10 native OpenAPI


// -------------------
// CORS (Vercel + Local)
// -------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVercel", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://task-manager-kappa-gray.vercel.app"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});


// =====================
// App Pipeline
// =====================
var app = builder.Build();

app.MapOpenApi(); // /openapi/v1.json

app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowVercel");

// 🔴 THIS ORDER IS CRITICAL
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
