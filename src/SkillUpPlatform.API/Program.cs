using SkillUpPlatform.Application;
using SkillUpPlatform.Infrastructure;
using SkillUpPlatform.API.Middleware;
using SkillUpPlatform.API.Extensions;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger with JWT support and role-based categorization
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "SkillUp Platform API", 
        Version = "v1",
        Description = "Smart Career Training Platform for Students and Graduates - Role-Based Endpoints"
    });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Configure role-based tags for better organization (simplified)
    c.TagActionsBy(api =>
    {
        try
        {
            return new[] { GetSwaggerTag(api) };
        }
        catch
        {
            return new[] { "General" };
        }
    });
    
    c.DocInclusionPredicate((name, api) => true);
    
    // Add XML comments for better documentation (if available)
    try
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    }
    catch (Exception ex)
    {
        // Log warning but continue without XML documentation
        Console.WriteLine($"Warning: Could not load XML documentation: {ex.Message}");
    }
});

// Helper method to determine Swagger tag based on endpoint
string GetSwaggerTag(ApiDescription api)
{
    try
    {
        var controllerName = api.ActionDescriptor.RouteValues["controller"];
        if (controllerName == null) return "🔧 General";
        
        return controllerName.ToLower() switch
        {
            "users" or "auth" => "👨‍🎓 Student - Authentication & Profile",
            "learningpaths" => "👨‍🎓 Student - Learning Paths",
            "content" => "👨‍🎓 Student - Content Consumption",
            "assessments" => "👨‍🎓 Student - Assessments",
            "aiassistant" => "👨‍🎓 Student - AI Assistant",
            "resources" => "👨‍🎓 Student - Resources & Tools",
            "dashboard" => "👨‍🎓 Student - Dashboard",
            "notifications" => "👨‍🎓 Student - Notifications",
            "files" => "👨‍🎓 Student - File Management",
            "creator" => "👨‍🏫 Content Creator - Management",
            "admin" => "👨‍💼 Admin - System Management",
            _ => "� General"
        };
    }
    catch
    {
        return "🔧 General";
    }
}

// Add application services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add API specific services
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
// Enable Swagger in all environments (Development, Production, etc.)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillUp Platform API V1");
    c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
    c.DocumentTitle = "SkillUp Platform API Documentation";
    c.DisplayRequestDuration();
});

app.UseHttpsRedirection();

// Add CORS
app.UseCors("AllowAll");

// Add custom middleware
try
{
    app.UseMiddleware<ExceptionMiddleware>();
}
catch (Exception ex)
{
    Console.WriteLine($"Warning: Error setting up ExceptionMiddleware: {ex.Message}");
}

// Add authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Add user context middleware after authentication
try
{
    app.UseUserContext();
}
catch (Exception ex)
{
    Console.WriteLine($"Warning: Error setting up UserContext: {ex.Message}");
}

app.MapControllers();

app.Run();
