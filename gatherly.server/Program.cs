using System.Reflection;
using System.Text;
using DotNetEnv;
using FluentAssertions.Common;
using FluentMigrator.Runner;
using gatherly.server;
using gatherly.server.Models.Authentication.RecoverySession;
using gatherly.server.Models.Authentication.SsoSession;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Chat.Chat;
using gatherly.server.Models.Mailing.MailEntity;
using gatherly.server.Models.Meetings.Invitations;
using gatherly.server.Models.Meetings.Meeting;
using gatherly.server.Models.Meetings.UserMeeting;
using gatherly.server.Models.Tokens.BlacklistToken;
using gatherly.server.Models.Tokens.RefreshToken;
using gatherly.server.Models.Tokens.TokenEntity;
using gatherly.server.Persistence.Authentication.RecoverySession;
using gatherly.server.Persistence.Authentication.SsoSession;
using gatherly.server.Persistence.Authentication.UserEntity;
using gatherly.server.Persistence.Chat;
using gatherly.server.Persistence.Mailing;
using gatherly.server.Persistence.Mailing.EmailTemplates;
using gatherly.server.Persistence.Meetings.Invitations;
using gatherly.server.Persistence.Meetings.Meeting;
using gatherly.server.Persistence.Meetings.UserMeeting;
using gatherly.server.Persistence.Tokens;
using gatherly.server.Persistence.Tokens.BlacklistToken;
using gatherly.server.Persistence.Tokens.RefreshToken;
using gatherly.server.Persistence.Tokens.TokenEntity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using NHibernate;
using ISession = NHibernate.ISession;

Env.Load(".env");

var builder = WebApplication.CreateBuilder(args);

// Add NHibernate
builder.Services.AddSingleton<ISessionFactory>(provider =>
{
    return NHibernateHelper.SessionFactory;
});

builder.Services.AddScoped(provider =>
{
    var sessionFactory = provider.GetService<ISessionFactory>();
    return sessionFactory.OpenSession();
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<TokenHelper>();
builder.Services.AddScoped<ISsoSessionRepository, SsoSessionRepository>();
builder.Services.AddScoped<IRecoverySessionRepository, RecoverySessionRepository>();
builder.Services.AddScoped<IUserEntityRepository, UserEntityRepository>();

builder.Services.AddScoped<ISsoSessionService, SsoSessionService>();
builder.Services.AddScoped<IRecoverySessionService, RecoverySessionService>();
builder.Services.AddScoped<IUserEntityService, UserEntityService>();

builder.Services.AddScoped<ITokenEntityService, TokenEntityService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IBlacklistTokenService, BlacklistTokenService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IMailEntityService, MailEntityService>();
builder.Services.AddScoped<IMailEntityRepository, MailEntityRepository>();
builder.Services.AddScoped<IMeetingService, MeetingService>();
builder.Services.AddScoped<IUserMeetingService, UserMeetingService>();
builder.Services.AddScoped<IInvitationsService, InvitationsService>();
builder.Services.AddScoped<IChatService, ChatService>();


// Add controllers and endpoints
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR(options =>
{
    options.KeepAliveInterval = TimeSpan.FromMinutes(4);
});

// Konfiguracja Swaggera
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gatherly API", Version = "v1.1" });
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
     $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));

    // Konfiguracja JWT
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
              Enter 'Bearer' [space] and then your token in the text input below.
              \r\n\r\nExample: 'Bearer 12345abcdef'",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

//Authentication and Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Env.GetString("ISSUER"),
        ValidAudience = Env.GetString("AUDIENCE"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.GetString("SECRET")))
        //ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                context.Response.Headers.Add("Token-Expired", "true");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine("Authentication challenge: " + context.Error + " " + context.ErrorDescription);
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = JsonConvert.SerializeObject(new { error = "You are not authorized" });
            return context.Response.WriteAsync(result);
        },
        OnMessageReceived = context =>
        {
            Console.WriteLine("Token received: " + context.Token);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated for user: " + context.Principal.Identity.Name);
            return Task.CompletedTask;
        }
    };
});

//email sending

builder.Services
    .AddFluentEmail(Env.GetString("MAIL_LOGIN"))
    .AddRazorRenderer()
    .AddSmtpSender(Env.GetString("SMTP_ADDRESS"), Env.GetInt("PORT_NUMBER"),
        Env.GetString("MAIL_LOGIN"), Env.GetString("MAIL_PASSWORD"));

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins(Env.GetString("AUDIENCE"))
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            //.WithExposedHeaders("Content-Disposition")
            .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        , sqlOptions => sqlOptions.MigrationsAssembly(typeof(DataContext).Assembly.GetName().Name))
);

builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(c =>
    {
        c.AddSqlServer2016()
            .WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
            .ScanIn(Assembly.GetExecutingAssembly()).For.All();
    })
    .AddLogging(config => config.AddFluentMigratorConsole());


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

app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["Authorization"];
    if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer ")) context.Request.Headers["Authorization"] = token;
    await next();
});

app.UseCors("AllowSpecificOrigin");
app.UseDefaultFiles();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();
app.UseDeveloperExceptionPage();
/*
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
*/
app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();


public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    /*
    public DbSet<UserEntity> User { get; set; }
    public DbSet<SsoSession> SsoSessions { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<BlacklistToken> BlacklistTokens { get; set; }
    */
}