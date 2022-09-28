using System.Reflection;
using Microsoft.OpenApi.Models;

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
        c.EnableAnnotations();
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, 
            $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelTrack v1"));
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
