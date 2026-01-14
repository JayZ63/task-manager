using Microsoft.EntityFrameworkCore;
using TaskManager.Data;

var builder = WebApplication.CreateBuilder(args);
// Configure the application to listen on the port defined by the PORT environment variable
//Redploy
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

// Add services
builder.Services.AddDbContext<TaskContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS policy for local development
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


var app = builder.Build();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowVercel");


app.UseAuthorization();
app.MapControllers();
app.Run();
