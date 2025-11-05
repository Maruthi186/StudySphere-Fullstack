using Npgsql;
using Microsoft.EntityFrameworkCore;
using StudySphereApi.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["DATABASE_URL"];

builder.Services.AddDbContext<ApiDbContext>(options =>
{

    options.UseNpgsql(connectionString);
});


var serviceProvider = builder.Services.BuildServiceProvider();
try
{
    var context = serviceProvider.GetRequiredService<ApiDbContext>();

    var demoUser = context.Users.Find(1);

    if (demoUser == null)
    {
        context.Users.Add(new StudySphereApi.Models.User
        {
            Id = 1,
            Name = "Demo User",
            Email = "demo@example.com"
        });
        context.SaveChanges(); 
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
}
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowReactApp");

app.MapControllers();

app.Run();
