using Microsoft.EntityFrameworkCore;
using PastebinApi.Data;

var builder = WebApplication.CreateBuilder(args);

// ✅ EF Core + Neon = perfect
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ CORS POLICY DEFINITION (ADD THIS)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "https://pruthvirajpallykonda.github.io",           // GitHub Pages
                "https://pastebin-lite-ui-*.up.railway.app"         // Railway frontend
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ✅ Controllers + Swagger = good
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ CORS ENABLED
app.UseCors("AllowFrontend");  // Now works!
app.UseAuthorization();

app.MapControllers();

// ✅ Railway port binding
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    app.Urls.Add($"http://0.0.0.0:{port}");
}

app.Run();
