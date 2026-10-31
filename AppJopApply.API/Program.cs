using AppJopApply.Application.Interfaces;
using AppJopApply.Domain.Entities;
using AppJopApply.Infrastruction.Data;
using AppJopApply.Infrastruction.Repositries;
using AppJopApply.Infrastruction.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers ───────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Database ──────────────────────────────────────────────────────────────────
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// ── ASP.NET Core Identity ─────────────────────────────────────────────────────
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key not configured.");

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

builder.Services.AddAuthorization();

// ── Application Services ──────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailSender, ConsoleEmailSender>();

// ── MediatR ───────────────────────────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(AppJopApply.Application.AssemblyReference).Assembly));

// ── OpenAPI / Swagger ─────────────────────────────────────────────────────────
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AppJopApply API",
        Version = "v1",
        Description = "Job application management API with Role-based Authorization"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token only (e.g. your_token_here)"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

// ── Database Migration & Seeding ──────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
        await DbInitializer.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// ── HTTP pipeline ─────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AppJopApply API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
