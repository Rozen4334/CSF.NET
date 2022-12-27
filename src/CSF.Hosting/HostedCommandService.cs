using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

[assembly: CLSCompliant(true)]
namespace CSF.Hosting
{
    /// <summary>
    ///     Represents a hosted <see cref="CommandFramework{T}"/>. Provides the necessary extensions to set up a hosted environment for CSF.
    /// </summary>
    /// <remarks>
    ///     Use <see cref="HostBuilderExtensions.ConfigureCommands{T, THost}(IHostBuilder)"/> to configure the command pipeline for hosting.
    /// </remarks>
    /// <typeparam name="T">The </typeparam>
    public abstract class HostedCommandService<T> : CommandFramework<T>, IHostedService
        where T : CommandConveyor
    {
        /// <summary>
        ///     Represents a <see cref="Microsoft.Extensions.Logging"/> counterpart to the internal <see cref="CommandFramework.Logger"/> and resolves through the intended stream at logging configuration.
        /// </summary>
        public new ILogger<HostedCommandService<T>> Logger { get; }

        public HostedCommandService(T conveyor, CommandConfiguration configuration, IServiceProvider provider, ILogger<HostedCommandService<T>> logger)
            : base(provider, configuration, conveyor)
        {
            base.Logger.SendAction = async (x) => await LogAsync(x);
            Logger = logger;
        }

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(true, cancellationToken);
        }

        /// <inheritdoc/>
        public new async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }

        /// <summary>
        ///     Mutates inbound logs from the <see cref="CommandFramework.Logger"/> into its <see cref="ILogger{TCategoryName}"/> counterpart.
        /// </summary>
        /// <param name="log">The log message to transform.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        protected virtual Task LogAsync(Log log)
        {
            Logger.Log((Microsoft.Extensions.Logging.LogLevel)log.LogLevel, log.Exception, "{}", log.Value);

            return Task.CompletedTask;
        }
    }
}
