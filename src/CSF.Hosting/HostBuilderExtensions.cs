using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Hosting
{
    public static class HostBuilderExtensions
    {
        /// <summary>
        ///     Configures the necessary settings to set up the command framework for the use of <see cref="IHost"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="CommandConveyor"/> instance to use.</typeparam>
        /// <typeparam name="THost">The <see cref="HostedCommandService{T}"/> implementation to use.</typeparam>
        /// <param name="hostBuilder"></param>
        /// <param name="action">An action that configures the necessary hosting necessities.</param>
        /// <returns>The same <see cref="IHostBuilder"/> for chaining calls.</returns>
        public static IHostBuilder ConfigureCommands<T, THost>(this IHostBuilder hostBuilder, Action<HostBuilderContext, CommandConfiguration> action)
            where T : CommandConveyor where THost : HostedCommandService<T>
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                var config = new CommandConfiguration();

                action(context, config);

                services.ConfigureCommands<T, THost>(config);
            });

            return hostBuilder;
        }

        /// <summary>
        ///     Configures the necessary settings to set up the command framework for the use of <see cref="Microsoft.Extensions.Hosting"/>. 
        /// </summary>
        /// <typeparam name="T">The <see cref="CommandConveyor"/> instance to use.</typeparam>
        /// <typeparam name="THost">The <see cref="HostedCommandService{T}"/> implementation to use.</typeparam>
        /// <param name="hostBuilder"></param>
        /// <param name="action">An action that configures the necessary hosting necessities.</param>
        /// <returns>The same <see cref="IHostBuilder"/> for chaining calls.</returns>
        public static IHostBuilder ConfigureCommands<T, THost>(this IHostBuilder hostBuilder, Action<CommandConfiguration> action)
            where T : CommandConveyor where THost : HostedCommandService<T>
        {
            hostBuilder.ConfigureServices(services =>
            {
                var config = new CommandConfiguration();

                action(config);

                services.ConfigureCommands<T, THost>(config);
            });

            return hostBuilder;
        }

        /// <summary>
        ///     Configures the necessary settings to set up the command framework for the use of <see cref="Microsoft.Extensions.Hosting"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="CommandConveyor"/> instance to use.</typeparam>
        /// <typeparam name="THost">The <see cref="HostedCommandService{T}"/> implementation to use.</typeparam>
        /// <param name="hostBuilder"></param>
        /// <returns>The same <see cref="IHostBuilder"/> for chaining calls.</returns>
        public static IHostBuilder ConfigureCommands<T, THost>(this IHostBuilder hostBuilder)
            where T : CommandConveyor where THost : HostedCommandService<T>
        {
            hostBuilder.ConfigureServices(services =>
            {
                services.ConfigureCommands<T, THost>(new CommandConfiguration());
            });
            return hostBuilder;
        }

        /// <summary>
        ///     Configures the necessary settings to set up the command framework for the use of <see cref="Microsoft.Extensions.Hosting"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="CommandConveyor"/> instance to use.</typeparam>
        /// <typeparam name="THost">The <see cref="HostedCommandService{T}"/> implementation to use.</typeparam>
        /// <param name="services"></param>
        /// <returns>The same <see cref="IServiceCollection"/> for chaining calls.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IServiceCollection ConfigureCommands<T, THost>(this IServiceCollection services, CommandConfiguration configuration)
            where T : CommandConveyor where THost : HostedCommandService<T>
        {
            services.AddSingleton(configuration);

            services.AddSingleton<T>();
            services.AddHostedService<THost>();

            return services;
        }
    }
}
