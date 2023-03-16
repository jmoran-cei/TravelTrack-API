using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using TravelTrack_API.Authorization;
using TravelTrack_API.Authorization.Policies;
using TravelTrack_API.DbContexts;
using TravelTrack_API.SharedServices.BlobManagement;
using TravelTrack_API.SharedServices.MicrosoftGraph;
using v1_Services = TravelTrack_API.Versions.v1.Services;
using v2_Services = TravelTrack_API.Versions.v2.Services;
using v3_Services = TravelTrack_API.Versions.v3.Services;

var builder = WebApplication.CreateBuilder(args);
bool isProduction = builder.Environment.IsProduction();
string dbConnectionString = "Server=localhost; Database=TravelTrackDB; Trusted_Connection=True;"; // dev
string ApiKeyValue = "dev"; // dev


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("https://localhost:7194", "http://localhost:4200", "https://bootcamp-traveltrack.azurewebsites.net");
            policy.WithMethods("GET", "POST", "OPTIONS", "PUT", "DELETE");
            policy.WithHeaders("Content-Type", "X-Api-Key", "Authorization");
            policy.AllowCredentials();
        });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(options =>
        {
            builder.Configuration.Bind("AzureAdB2C", options);

            options.TokenValidationParameters.NameClaimType = "name";
        },
        options =>
        {
            builder.Configuration.Bind("AzureAdB2C", options);
        });

builder.Services.AddAuthorization(options =>
{
    // ---- Trip Scopes ---
    // Create policy to check for the scope 'Read'
    options.AddPolicy("TripReadScope",
        policy => policy.Requirements.Add(
            new ScopesRequirement(
                "https://TravelTrackApp.onmicrosoft.com/TravelTrack/api/Trips.Read"))
        );

    // Create policy to check for the scope 'Write'
    options.AddPolicy("TripWriteScope",
        policy => policy.Requirements.Add(
            new ScopesRequirement(
                "https://TravelTrackApp.onmicrosoft.com/TravelTrack/api/Trips.Write"))
        );



    // ---- User scopes ---
    // Create policy to check for the scope 'Read'
    options.AddPolicy("UserReadScope",
        policy => policy.Requirements.Add(
            new ScopesRequirement(
                "https://TravelTrackApp.onmicrosoft.com/TravelTrack/api/User.Read"))
        );

    // Create policy to check for the scope 'Write'
    options.AddPolicy("UserWriteScope",
        policy => policy.Requirements.Add(
            new ScopesRequirement(
                "https://TravelTrackApp.onmicrosoft.com/TravelTrack/api/User.Write"))
        );
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TravelTrack API",
        Description = "An ASP.NET Core Web API for TravelTrack v3",
        Contact = new OpenApiContact
        {
            Name = "Jon Moran",
            Email = "jmoran@ceiamerica.com",
            Url = new Uri("https://www.ceiamerica.com/")
        },
        Version = "v1"
    });
    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "TravelTrack API",
        Description = "An ASP.NET Core Web API for TravelTrack v2",
        Contact = new OpenApiContact
        {
            Name = "Jon Moran",
            Email = "jmoran@ceiamerica.com",
            Url = new Uri("https://www.ceiamerica.com/")
        },
        Version = "v2"
    });
    c.SwaggerDoc("v3", new OpenApiInfo
    {
        Title = "TravelTrack API",
        Description = "An ASP.NET Core Web API for TravelTrack v3",
        Contact = new OpenApiContact
        {
            Name = "Jon Moran",
            Email = "jmoran@ceiamerica.com",
            Url = new Uri("https://www.ceiamerica.com/")
        },
        Version = "v3"
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

builder.Services.AddScoped<v1_Services.ITripService, v1_Services.TripService>();
builder.Services.AddScoped<v1_Services.IUserService, v1_Services.UserService>(); 
builder.Services.AddScoped<v2_Services.ITripService, v2_Services.TripService>();
builder.Services.AddScoped<v2_Services.IUserService, v2_Services.UserService>(); 
builder.Services.AddScoped<v3_Services.ITripService, v3_Services.TripService>();
builder.Services.AddScoped<v3_Services.IUserService, v3_Services.UserService>();
builder.Services.AddScoped<IBlobService, BlobService>();
builder.Services.AddScoped<IMicrosoftGraphService, MicrosoftGraphService>();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(3, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // "api-supported-versions: 1.0, 2.0, 3.0"
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
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
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelTrack v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "TravelTrack v2");
        c.SwaggerEndpoint("/swagger/v3/swagger.json", "TravelTrack v3");
    });
}

app.UseCors();

app.UseHttpsRedirection();

app.UseMiddleware<ApiKeyMiddleware>(ApiKeyValue);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
