using Google.BLL.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Google.BLL
{
    public static class DependenciesBootstrapper
    {
        public static IServiceCollection AddGoogleBll(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddTransient<IGoogleService, GoogleService>();
            services.AddTransient<IFileService, FileService>();

            return services;
        }
    }
}
