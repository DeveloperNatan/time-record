using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TimeRecord;
using TimeRecord.Data;
using TimeRecord.DTO.Auth;
using TimeRecord.Middleware;
using TimeRecord.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== Load JWT secret from config (appsettings / env var) =====
JwtConfiguration.PrivateKey =
    builder.Configuration["Jwt:PrivateKey"]
    ?? throw new Exception("Missing config: Jwt:PrivateKey"); 

// ===== Controllers + custom model validation response =====
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(k => k.Key, v => v.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

        return new BadRequestObjectResult(new { message = "Invalid data", errors });
    };
});

// ===== Swagger (must be BEFORE builder.Build) =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Enables the "Authorize" button + sends Authorization: Bearer <token> [web:163]
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cole só o token (eyJ...)."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicyCors", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

// ===== Database =====
var connectionString =
    Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_AppDbConnectionString")
    ?? builder.Configuration.GetConnectionString("POSTGRESQLCONNSTR_AppDbConnectionString");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// ===== JWT Auth =====
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var result = new AuthResponseTokenDTO()
                {
                    StatusCode = 401,
                    Message = "Missing or invalid access token.",
                    Authentication = false,
                };
                return context.Response.WriteAsJsonAsync(result);
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(JwtConfiguration.PrivateKey)
            ),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    });

builder.Services.AddAuthorization(); // required for [Authorize] [web:11]

// ===== DI =====
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<TimeRecordsService>();
builder.Services.AddScoped<CompanyService>();

var app = builder.Build();

// ===== Swagger middleware (commonly only in Development) =====

app.UseSwagger();
app.UseSwaggerUI();


// ===== Middleware pipeline =====
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors("MyPolicyCors");

// auth must be before MapControllers [web:204]
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();