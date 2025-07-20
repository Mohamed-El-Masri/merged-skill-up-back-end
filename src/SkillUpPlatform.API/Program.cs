using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SkillUpPlatform.API.Extensions;
using SkillUpPlatform.API.Middleware;
using SkillUpPlatform.Application;
using SkillUpPlatform.Infrastructure;
using SkillUpPlatform.Infrastructure.Data;
using SkillUpPlatform.Infrastructure.Persistence;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 🔐 Add Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SkillUp Platform API",
        Version = "v1",
        Description = "Smart Career Training Platform for Students and Graduates - Role-Based Endpoints"
    });

    // JWT Bearer setup for Swagger
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

    // Tags for swagger
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

    // XML Comments if available
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
        Console.WriteLine($"Warning: Could not load XML documentation: {ex.Message}");
    }
});

string GetSwaggerTag(ApiDescription api)
{
    try
    {
        var controllerName = api.ActionDescriptor.RouteValues["controller"];
        return controllerName?.ToLower() switch
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
            _ => "🔧 General"
        };
    }
    catch
    {
        return "🔧 General";
    }
}

// 📦 Add core services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);


builder.Services.AddAuthorization();

var app = builder.Build();

// 🚀 Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillUp Platform API V1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "SkillUp Platform API Documentation";
    c.DisplayRequestDuration();
});

// 🔒 HTTPS & CORS
app.UseHttpsRedirection();
app.UseCors("AllowAll");

// 🛡️ Global Exception Middleware
app.UseMiddleware<ExceptionMiddleware>();

// 🔐 Auth
app.UseAuthentication();
app.UseAuthorization();

// 👤 User Context Middleware
app.UseMiddleware<UserContextMiddleware>();

// 🌐 Map Controllers
app.MapControllers();

// 🌱 Seed DB
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbSeeder.SeedAsync(context);
}



app.Run();
