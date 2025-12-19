using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Education.API
{
    public static class ProgramExtensionApi
    {
        public static IServiceCollection InjectApiLayer(this IServiceCollection services,IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");

            // SecretKey yoxlanışı
            var secretKey = jwtSettings["SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentNullException("Jwt:SecretKey", "JWT Secret Key təyin edilməyib");
            }

            // Minimum 32 simvol yoxlanışı
            if (secretKey.Length < 32)
            {
                throw new ArgumentException("JWT Secret Key ən azı 32 simvol olmalıdır");
            }

            // Authentication service'ini əlavə et
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Token validation parametrləri
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,                    // Issuer yoxlanılır
                    ValidateAudience = true,                  // Audience yoxlanılır
                    ValidateLifetime = true,                  // Müddət yoxlanılır
                    ValidateIssuerSigningKey = true,          // İmza key'i yoxlanılır

                    ValidIssuer = jwtSettings["Issuer"],      // Etibarlı issuer
                    ValidAudience = jwtSettings["Audience"],  // Etibarlı audience
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey)),   // İmza key'i

                    ClockSkew = TimeSpan.Zero                 // Dəqiq müddət yoxlaması
                };

                // Token'i header'dan alma
                options.Events = new JwtBearerEvents
                {
                    // Token'i Authorization header'dan oxu
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    },

                    // Authentication xətası
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Append("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            return services;
        }
    }
}
