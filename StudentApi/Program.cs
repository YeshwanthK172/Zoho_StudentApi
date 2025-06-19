// StudentApi/Program.cs
using MySql.Data.MySqlClient;
using StudentApi.Models; // <-- Corrected: StudentApi.Models (lowercase 'a')
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "StudentApi", Version = "v1" }); // Also update Swagger title
});

// Configure CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:5XXX", "https://localhost:7YYY") // <-- IMPORTANT: Replace with your StudentMVC app's ACTUAL URLs (e.g., https://localhost:7076, http://localhost:5000)
                               .AllowAnyHeader()
                               .AllowAnyMethod());
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "StudentApi v1"); // Also update Swagger endpoint title
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowSpecificOrigin"); // Apply the CORS policy

app.UseAuthorization();

app.MapControllers();

app.Run();