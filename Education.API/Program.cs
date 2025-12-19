using Education.API;
using Education.Business;
using Education.Infrastructure;
using Education.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ====================== Start Services =====================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // ====== 1. SWAGGER DOC INFO ======
    // Niyə? - API haqqında məlumat
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Education API",
        Version = "v1",
        Description = "Education Management System API",
        Contact = new OpenApiContact
        {
            Name = "Education Team",
            Email = "support@education.com"
        }
    });

    // ====== 2. JWT AUTH IN SWAGGER ======
    // Niyə? - Swagger UI-də authentication test etmək
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    // ====== 3. XML COMMENTS ======
    // Niyə? - Controller method-larının summary-lərini göstərmək
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddCors(options => options.AddPolicy("AllowAll", policy =>
{
    // Niyə AllowAll? - Development üçün, production-da spesifik etmək lazımdır
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
}));

// ====================== Api Services =====================

builder.Services.InjectApiLayer(builder.Configuration);

// ====================== Business Services =====================
builder.Services.InjectBusinessLayer(builder.Configuration);

// ====================== Domain Services =====================


// ====================== Infrastructure Services =====================

builder.Services.InjectInfraStructure(builder.Configuration);


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();
}
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
// ==================== DATABASE MIGRATION ====================
// Niyə? - App başlayanda database-in güncəl olmasını təmin etmək
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UniversityDbContext>();

    // Niyə? - Pending migration-ları apply etmək
    dbContext.Database.Migrate();

    // Niyə? - İlkin məlumatları seed etmək (sonra əlavə edəcəyik)
    // await SeedData.Initialize(scope.ServiceProvider);
}
app.Run();
