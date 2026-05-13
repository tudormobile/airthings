using Tudormobile.AirthingsService;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy for local testing and specific domains
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(
                "https://www.tudormobile.com",
                "https://www.tudorzone.com",
                "https://tudormobile.com",
                "https://tudorzone.com",
                "https://localhost:5162",
                "http://localhost:5162",
                "https://localhost:7043",
                "http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add output caching for the Airthings Service API.
builder.Services.AddOutputCache();

// Add authorization for running under TudormoibleAPI.
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowSpecificOrigins");
//app.UseHttpsRedirection();
app.UseAuthorization();

// map for use in the Tudormobile API host (testing purposes)
app.UseAirthingsService();

app.Run();
