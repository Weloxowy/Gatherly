using System.Reflection;
using FluentMigrator.Runner;
using gatherly.server.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Gatherly", Description = "Publiczny interfejs dla endpointów warstwy serwerowej.",Version = "0.0.1" });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins")?.Get<string[]>() ??
                             Array.Empty<string>();

        policy
            .WithOrigins( /*allowedOrigins*/"http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithExposedHeaders("Content-Disposition")
            .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});

//Add DB Context
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(
        "Server=localhost\\SQLEXPRESS;Database=Gatherly;Integrated Security=SSPI;Application Name=Gatherly; TrustServerCertificate=true;"
        , sqlOptions => sqlOptions.MigrationsAssembly(typeof(DataContext).Assembly.GetName().Name))
);

//FluentMigrator
builder.Services.AddFluentMigratorCore() // Move FluentMigrator registration here
    .ConfigureRunner(c =>
    {
        c.AddSqlServer2016()
            .WithGlobalConnectionString(
                "Server=localhost\\SQLEXPRESS;Database=Gatherly;Integrated Security=SSPI;Application Name=Gatherly; TrustServerCertificate=true;")
            .ScanIn(Assembly.GetExecutingAssembly()).For.All();
    })
    .AddLogging(config => config.AddFluentMigratorConsole());

//Authentication and Authorization
builder.Services.AddAuthentication(options =>
    {
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options => { builder.Configuration.GetRequiredSection("BearerAuthOptions").Bind(options); });

builder.Services.AddAuthorization();

var app = builder.Build();
using var scope = app.Services.CreateScope();
var migrator = scope.ServiceProvider.GetService<IMigrationRunner>();

if (migrator != null)
{
    migrator.ListMigrations();
    migrator.MigrateUp();
}
else
{
    throw new Exception("Migration fault");
}

app.UseCors("AllowAllOrigins");
app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();


public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Users> Users { get; set; }
}