using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Education.API
{
    public static class ProgramExtensionApi
    {
        public static IServiceCollection InjectApiLayer(this IServiceCollection services,IConfiguration configuration)
        {
            // ==================== 5. JWT AUTHENTICATION CONFIGURATION ====================
            // Niyə? - Token-based authentication aktiv etmək
            // Hansı protocol? - JWT Bearer Token Authentication
            services.AddAuthentication(options =>
            {
                // Niyə iki option? - Default scheme-ləri təyin edirik
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // ======= Token Validation Parameters =======
                // Niyə? - Gələn JWT token-lərinin validasiya qaydalarını təyin edirik

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // ====== 1. VALIDATE ISSUER ======
                    // Niyə? - Token-in kim tərəfindən yaradıldığını yoxlamaq
                    // Hansı təhlükə? - Saxta token (fake issuer)
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    // Niyə appsettings-dən? - Configuration mərkəzləşdirilməsi üçün

                    // ====== 2. VALIDATE AUDIENCE ======
                    // Niyə? - Token-in hansı app üçün olduğunu yoxlamaq
                    ValidateAudience = true,
                    ValidAudience = configuration["JwtSettings:Audience"],

                    // ====== 3. VALIDATE LIFETIME ======
                    // Niyə? - Token-in vaxtının keçib-keçmədiyini yoxlamaq
                    ValidateLifetime = true,

                    // ====== 4. VALIDATE SIGNING KEY ======
                    // Niyə? - Token-in signature-nın düzgünlüyünü yoxlamaq
                    // ƏN ƏHƏMİYYƏTLİ HİSSƏ - Security
                    ValidateIssuerSigningKey = true,

                    // ====== 5. SIGNING KEY ======
                    // Niyə? - Token-in şifrələnməsi üçün gizli açar
                    // Niyə SymmetricSecurityKey? - Eyni açar ilə sign və verify
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!)),

                    // ====== 6. CLOCK SKEW ======
                    // Niyə? - Server və client saat fərqləri üçün tolerantlıq
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                // ======= EVENTS CONFIGURATION =======
                // Niyə? - Authentication prosesində baş verən event-ləri handle etmək
                options.Events = new JwtBearerEvents
                {
                    // ====== 1. OnAuthenticationFailed ======
                    // Niyə? - Authentication uğursuz olduqda loglamaq və ya xüsusi cavab vermək
                    OnAuthenticationFailed = context =>
                    {
                        // Niyə Console? - Development zamanı debugging üçün
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },

                    // ====== 2. OnTokenValidated ======
                    // Niyə? - Token validasiya edildikdən sonra əlavə yoxlamalar
                    OnTokenValidated = context =>
                    {
                        // Əlavə logic: User-in aktiv olub-olmadığını yoxlamaq
                        // Database-dən user statusunu yoxlamaq
                        return Task.CompletedTask;
                    }
                };
            });

            // ==================== 6. AUTHORIZATION POLICIES ====================
            // Niyə? - Role-based authorization üçün policy-lər təyin etmək
            // Hansı məqsəd? - Fərqli rollar üçün fərqli icazələr
            services.AddAuthorization(options =>
            {
                // ====== 1. ADMIN POLICY ======
                // Niyə? - Yalnız Admin rollu user-lər üçün
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));

                // ====== 2. TEACHER POLICY ======
                // Niyə? - Teacher və Admin üçün
                options.AddPolicy("TeacherOrAdmin", policy =>
                    policy.RequireRole("Teacher", "Admin"));

                // ====== 3. STUDENT POLICY ======
                // Niyə? - Student, Teacher və Admin üçün
                options.AddPolicy("StudentOrAbove", policy =>
                    policy.RequireRole("Student", "Teacher", "Admin"));

                // ====== 4. EMAIL CONFIRMED POLICY ======
                // Niyə? - Yalnız emaili təsdiqlənmiş user-lər üçün
                options.AddPolicy("EmailConfirmed", policy =>
                    policy.RequireClaim("email_confirmed", "true"));
            });
            return services;
        }
    }
}
