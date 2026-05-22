using System.Diagnostics.CodeAnalysis;
using Tudormobile.Airthings.Service;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy for specific domains (localhost origins only in Development)
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowSpecificOrigins",
        policy =>
        {
            var origins = new List<string>
            {
                "https://www.tudormobile.com",
                "https://www.tudorzone.com",
                "https://tudormobile.com",
                "https://tudorzone.com",
            };

            if (builder.Environment.IsDevelopment())
            {
                origins.AddRange([
                    "https://localhost:5162",
                    "http://localhost:5162",
                    "https://localhost:7043",
                    "http://localhost:5173",
                ]);
            }

            policy.WithOrigins([.. origins])
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add all services required by the Airthings Service.
builder.Services.AddAirthingsService(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowSpecificOrigins");

// map for use in the Tudormobile API host (testing purposes)
app.UseAirthingsService();

app.Run();

/// <summary>
/// Host for the Airthings Service to run locally during testing.
/// </summary>
#pragma warning disable ASP0027 // Using this to exclude the Program class from code coverage as it contains only boilerplate code.
[ExcludeFromCodeCoverage]
public partial class Program
{
}
#pragma warning restore ASP0027 // Unnecessary public Program class declaration
