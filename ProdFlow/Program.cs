using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProdFlow.Data;
using ProdFlow.Services;
using ProdFlow.Services.Interfaces;
using Hangfire;
using Hangfire.SqlServer; // Changed to specific storage
using Microsoft.AspNetCore.Authentication.JwtBearer; // Added for JWT
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Microsoft.Data.SqlClient for stored procedure access
builder.Services.AddTransient(provider =>
    new Microsoft.Data.SqlClient.SqlConnection(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Hangfire
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseDefaultTypeSerializer()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

// Register JWT Authentication
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Register services
builder.Services.AddScoped<IEmployeeValidationService, EmployeeValidationService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IJustificationService, JustificationService>();
builder.Services.AddScoped<ISynoptiqueService, SynoptiqueService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IGalliaService, GalliaService>();
builder.Services.AddScoped<IClientReferenceService, ClientReferenceService>();
builder.Services.AddScoped<IFlanDecoupeService, FlanDecoupeService>();
builder.Services.AddScoped<IAssemblageService, AssemblageService>();

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
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");
app.UseAuthentication(); // Added before UseAuthorization
app.UseAuthorization();
app.MapControllers();
app.UseHangfireDashboard();

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