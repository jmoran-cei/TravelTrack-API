using System.Reflection;
using Microsoft.OpenApi.Models;
using TravelTrack_API.Authorization;
using TravelTrack_API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:4200");
            policy.WithMethods("GET", "POST", "OPTIONS", "PUT", "DELETE");
            policy.WithHeaders("Content-Type");
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

builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IUserService, UserService>(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelTrack v1"));
}

app.UseMiddleware<ApiKeyMiddleware>();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
