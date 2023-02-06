using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TravelTrack_API.Authorization;
using TravelTrack_API.DbContexts;
using TravelTrack_API.Services;
using TravelTrack_API.Services.BlobManagement;

var builder = WebApplication.CreateBuilder(args);
bool isProduction = builder.Environment.IsProduction();
var dbConnectionString = "Server=localhost; Database=TravelTrackDB; Trusted_Connection=True;"; // dev
string ApiKeyValue = "dev"; // dev

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("https://localhost:7194", "http://localhost:4200", "https://bootcamp-traveltrack.azurewebsites.net");
            policy.WithMethods("GET", "POST", "OPTIONS", "PUT", "DELETE");
            policy.WithHeaders("Content-Type", "X-Api-Key", "X-Api-Version");
        });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TravelTrack API",
        Description = "An ASP.NET Core Web API for TravelTrack",
        Contact = new OpenApiContact
        {
            Name = "Jon Moran",
            Email = "jmoran@ceiamerica.com",
            Url = new Uri("https://www.ceiamerica.com/")
        },
        Version = "v1"
    });

    // API Key in Swagger
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "The Api Key must be present in the header",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-Api-Key",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });
    var key = new OpenApiSecurityScheme()
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey"
        },
        In = ParameterLocation.Header
    };
    // It'll prompt the developer for the key value when needing authorization
    var requirement = new OpenApiSecurityRequirement { { key, new List<string>() } };
    c.AddSecurityRequirement(requirement);

    c.EnableAnnotations();
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

if (isProduction)
{
    dbConnectionString = builder.Configuration.GetConnectionString("DBConnectionString");
    ApiKeyValue = builder.Configuration.GetValue<string>("ApiAccessKey");
}

builder.Services
    .AddDbContext<TravelTrackContext>(dbContextOptions => dbContextOptions.UseSqlServer(dbConnectionString));
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBlobService, BlobService>();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // "api-supported-versions: 1.0"
    options.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
});

builder.Services.AddVersionedApiExplorer(
    options =>
    {
        options.GroupNameFormat = "'v'VVV";
    });

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelTrack v1"));
}

app.UseCors();

app.UseHttpsRedirection();

app.UseMiddleware<ApiKeyMiddleware>(ApiKeyValue);

app.UseAuthorization();

app.MapControllers();

app.Run();
