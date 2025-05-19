using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProdFlow.Data;
using ProdFlow.Services;
using ProdFlow.Services.Interfaces; 
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register your single DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Microsoft.Data.SqlClient for stored procedure access
builder.Services.AddTransient(provider =>
    new Microsoft.Data.SqlClient.SqlConnection(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register your services
builder.Services.AddScoped<IEmployeeValidationService, EmployeeValidationService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IJustificationService, JustificationService>();
builder.Services.AddScoped<ISynoptiqueService, SynoptiqueService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IGalliaService, GalliaService>();
builder.Services.AddScoped<IClientReferenceService, ClientReferenceService>();
builder.Services.AddScoped<IFlanDecoupeService, FlanDecoupeService>();
builder.Services.AddScoped<IAssemblageService, AssemblageService>();

//builder.Services.AddScoped<IProfileService, ProfileService>();
// Add other service registrations here as needed
// builder.Services.AddScoped<IMyOtherService, MyOtherService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
        });
});

// Add logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");
app.UseAuthorization();
app.MapControllers();

// Verify database connection on startup
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Test connection for EF Core operations
    dbContext.Database.OpenConnection();
    dbContext.Database.CloseConnection();
    Console.WriteLine("Database connection successful");

    // Test raw SQL connection
    var sqlConnection = scope.ServiceProvider.GetRequiredService<Microsoft.Data.SqlClient.SqlConnection>();
    await sqlConnection.OpenAsync();
    await sqlConnection.CloseAsync();
    Console.WriteLine("Raw SQL connection successful");
}
catch (Exception ex)
{
    Console.WriteLine($"Database connection error: {ex.Message}");
}

app.Run();