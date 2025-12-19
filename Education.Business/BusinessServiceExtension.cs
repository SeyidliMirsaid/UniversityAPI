using Education.Business.Services.Impl;
using Education.Business.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Business
{
    public static class BusinessServiceExtension
    {
        public static IServiceCollection InjectBusinessLayer(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddScoped<IAuthService, AuthService>();

            
            return services;
        }
    }
}
