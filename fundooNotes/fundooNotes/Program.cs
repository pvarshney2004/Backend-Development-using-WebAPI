using fundooNotes.Context;
using fundooNotes.Helpers;
using fundooNotes.Repositories.Implementations;
using fundooNotes.Repositories.Interfaces;
using fundooNotes.Services.Implementations;
using fundooNotes.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); // Replace default logging with Serilog

// Add services to the builder container.

builder.Services.AddControllers(); // add controller services to the application
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); // collects info about API endpoints 
// generate documentation for the APIs collected by .
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
     // this tell swagger all apis require bearer authentication scheme
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
// registering the ef core (DbContext)
builder.Services.AddDbContext<FundooContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("fundooNotes")));

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

// registering repositories and services for dependency injection

// registering user repository and service
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// registering notes repository and service
builder.Services.AddScoped<INotesService, NotesService>();
builder.Services.AddScoped<INotesRepository, NotesRepository>();

builder.Services.AddScoped<ICollaboratorService, CollaboratorService>();
builder.Services.AddScoped<ICollaboratorRepository, CollaboratorRepository>();

builder.Services.AddScoped<ILabelService, LabelService>();
builder.Services.AddScoped<ILabelRepository, LabelRepository>();

builder.Services.AddScoped<IEmailService, EmailService>();

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

// allowing frontend using core
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

// build the app pipeline after registering all services
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// middlewares
app.UseHttpsRedirection(); // http -> https
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers(); // maps controller routes to HTTP endpoints

// method to start web server
app.Run();
