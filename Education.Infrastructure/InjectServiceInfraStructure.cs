using Education.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Infrastructure
{
    public static class InjectServiceInfraStructure
    {
        public static IServiceCollection InjectInfraStructure(this IServiceCollection services,
            IConfiguration configuration)
        {

            services.AddDbContext<UniversityDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DbConnect"));
            });
            return services;
        }
    }
}
