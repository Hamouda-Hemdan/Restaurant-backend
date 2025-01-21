using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using resturant1.Services;
using resturant1.Repositories;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Security.Claims;
using resturant1.Data;
 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
//.AddJsonOptions(options =>
// {
//     options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
//     options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
// });
// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .LogTo(Console.WriteLine, LogLevel.Information)  // Log SQL queries for debugging
           .EnableSensitiveDataLogging());

// Configure ASP.NET Identity with default IdentityRole
builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// Configure JWT Authentication
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
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

// Configure Authorization
builder.Services.AddAuthorization();

// Register application services
builder.Services.AddScoped<DishService>();
builder.Services.AddScoped<IDishRepository, DishRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddScoped<BasketService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();




// Add Swagger with AuthorizationOperationFilter
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    // Add custom operation filter
    c.OperationFilter<AuthorizationOperationFilter>();

    // Add JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = "bearer",
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();

// Define the AuthorizationOperationFilter
public class AuthorizationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Get Authorize attribute
        var attributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                               .Union(context.MethodInfo.GetCustomAttributes(true))
                               .OfType<AuthorizeAttribute>();

        if (attributes != null && attributes.Any())
        {
            var attr = attributes.First();

            // Add what should be shown inside the security section
            IList<string> securityInfos = new List<string>
            {
                $"{nameof(AuthorizeAttribute.Policy)}:{attr.Policy}",
                $"{nameof(AuthorizeAttribute.Roles)}:{attr.Roles}",
                $"{nameof(AuthorizeAttribute.AuthenticationSchemes)}:{attr.AuthenticationSchemes}"
            };

            switch (attr.AuthenticationSchemes)
            {
                case var p when p == JwtBearerDefaults.AuthenticationScheme:
                default:
                    operation.Security = new List<OpenApiSecurityRequirement>
                    {
                        new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Id = "bearer", 
                                        Type = ReferenceType.SecurityScheme
                                    }
                                },
                                securityInfos
                            }
                        }
                    };
                    break;

                case "basic":
                    operation.Security = new List<OpenApiSecurityRequirement>
                    {
                        new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Id = "basic",
                                        Type = ReferenceType.SecurityScheme
                                    }
                                },
                                securityInfos
                            }
                        }
                    };
                    break;
            }
        }
        else
        {
            operation.Security?.Clear();
        }
    }
}
