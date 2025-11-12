using AuthService.API.Configurations;
using AuthService.API.Extensions;
using AuthService.API.GraphQL;
using AuthService.API.Middleware;
using AuthService.Application.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGraphQLConfiguration();

// Add Database Configuration
builder.Services.AddDatabaseConfiguration(builder.Configuration);

// Add DependencyInjection Services
builder.Services.AddProjectServices();

// Add CORS Configuration
builder.Services.AddCorsPolicy();

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<JwtConfiguration>(
    builder.Configuration.GetSection("Jwt")
);

builder.Services.AddValidationServices();

// Add JWT authentication via Middleware
builder.Services.AddJwtAuthentication();

var app = builder.Build();
app.UseRequestLogging();
app.MapGraphQL("/graphql");

// Configure the HTTP request pipeline.

app.UseCorsPolicy();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }