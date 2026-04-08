using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;
using System.Text;
using TaskManagerAPIs_Review.Context;
using TaskManagerAPIs_Review.Filters;
using TaskManagerAPIs_Review.Helpers;
using TaskManagerAPIs_Review.Repositories.Implementation;
using TaskManagerAPIs_Review.Repositories.Interface;
using TaskManagerAPIs_Review.Services.Implementation;
using TaskManagerAPIs_Review.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// confihuring serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.

//builder.Services.AddControllers();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelAttribute>();
});

//Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http, // uses http authentication scheme
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header, // token sent in headers
        Description = "Enter JWT Token"
    });
    // this tell swagger all apis require bearer authtentication scheme
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
            new string[] {}
        }
    });
});

// registering TaskContext in program.cs
builder.Services.AddDbContext<TaskContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("TaskManager")));

// Register Redis distributed cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["RedisCacheOptions:Configuration"];
    options.InstanceName = builder.Configuration["RedisCacheOptions:InstanceName"];
});
// Register the Redis connection multiplexer as a singleton service
// This allows the application to interact directly with Redis for advanced scenarios
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration["RedisCacheOptions:Configuration"]));

// 
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IAuthRepository,AuthRepository>();

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskRepository,TaskRepository>();

builder.Services.AddScoped<JwtTokenGenerator>();

// validating JWT tokens with every request
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);
builder.Services.AddAuthentication(options =>
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
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication(); // first authenticated the user
app.UseAuthorization();

app.MapControllers();

app.Run();
