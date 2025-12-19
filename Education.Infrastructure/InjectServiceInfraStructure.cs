using Education.Infrastructure.Persistence.Data;
using Education.Infrastructure.Persistence.Repositories.Impl;
using Education.Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static System.Formats.Asn1.AsnWriter;

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

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
