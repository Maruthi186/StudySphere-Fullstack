using Microsoft.EntityFrameworkCore;
using StudySphereApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// This tells the API to use SQLite and names the database file "studysphere.db"
// --- START: Replace your old AddDbContext block with this ---

builder.Services.AddDbContext<ApiDbContext>(options =>
{
    options.UseSqlite("Data Source=studysphere.db");
});

// This "builds" the services so we can use them right here
var serviceProvider = builder.Services.BuildServiceProvider();
try
{
    // This is our "database bridge"
    var context = serviceProvider.GetRequiredService<ApiDbContext>();

    // This is a check to see if we ALREADY have a user with Id = 1
    var demoUser = context.Users.Find(1);

    // If the demoUser is null (doesn't exist), we create it
    if (demoUser == null)
    {
        context.Users.Add(new StudySphereApi.Models.User
        {
            Id = 1,
            Name = "Demo User",
            Email = "demo@example.com"
        });
        context.SaveChanges(); // Save the new user to the database
    }
}
catch (Exception ex)
{
    // A simple log if something goes wrong
    Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
}
// --- END: New block ---
builder.Services.AddControllers();

// This adds the CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        // This is the URL of your React app
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
// This tells your app to USE the policy you just created
app.UseCors("AllowReactApp");

app.MapControllers();

app.Run();
