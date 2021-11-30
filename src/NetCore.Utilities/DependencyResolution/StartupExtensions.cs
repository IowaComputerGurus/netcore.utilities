using ICG.NetCore.Utilities;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class StartupExtensions
    {
        /// <summary>
        /// Registers the items included in the ICG NetCore Utilities project for Dependency Injection
        /// </summary>
        /// <param name="services">Your existing services collection</param>
        /// <param name="configuration">The configuration instance to load settings</param>
        public static void UseIcgNetCoreUtilities(this IServiceCollection services, IConfiguration configuration )
        {
            //Bind additional services
            services.AddTransient<IUrlSlugGenerator, UrlSlugGenerator>();
            services.AddTransient<ITimeProvider, TimeProvider>();
            services.AddTransient<ITimeSpanProvider, TimeSpanProvider>();
            services.AddTransient<IPathProvider, PathProvider>();
            services.AddTransient<IGuidProvider, GuidProvider>();
            services.AddTransient<IDatabaseEnvironmentModelFactory, DatabaseEnvironmentModelFactory>();
            services.AddTransient<IFileProvider, FileProvider>();
            services.AddTransient<IDirectoryProvider, DirectoryProvider>();
            services.AddTransient<IAesEncryptionService, AesEncryptionService>();

            services.Configure<AesEncryptionServiceOptions>(configuration.GetSection(nameof(AesEncryptionServiceOptions)));

        }
    }
}