using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TimeRecord;
using TimeRecord.Data;
using TimeRecord.DTO.Users;
using TimeRecord.Middleware;
using TimeRecord.Services;

var builder = WebApplication.CreateBuilder(args);

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

// ===== Swagger =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cole só o token (eyJ...)."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    options.EnableAnnotations();

    // TimeRecord.Models.ProblemDetails and Microsoft.AspNetCore.Mvc.ProblemDetails
    // would both map to the schemaId "ProblemDetails", so the app one gets its own id.
    options.CustomSchemaIds(type =>
    {
        if (type == typeof(TimeRecord.Models.ProblemDetails))
            return "AppProblemDetails";

        return type.IsGenericType
            ? type.Name.Split('`')[0] + string.Concat(type.GetGenericArguments().Select(a => a.Name))
            : type.Name;
    });
});

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicyCors", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ===== Connection =====
var connectionString =
    Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_AppDbConnectionString")
    ?? builder.Configuration.GetConnectionString("POSTGRESQLCONNSTR_AppDbConnectionString");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// ===== JWT =====
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
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["access-token"];

                if (!string.IsNullOrWhiteSpace(token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var result = new UsersResponseTokenDto()
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
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<TimeRecordsService>();
builder.Services.AddScoped<CompaniesService>();

var app = builder.Build();

// ===== Swagger middleware =====
app.UseSwagger();
app.UseSwaggerUI();

// ===== Middleware exception =====
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors("MyPolicyCors");

// auth must be before MapControllers [web:204]
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();