using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents a builder for a new <see cref="CommandFramework{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="ICommandConveyor"/> used for building the new <see cref="CommandFramework{T}"/>.</typeparam>
    public sealed class FrameworkBuilder<T> : IFrameworkBuilder
        where T : CommandConveyor
    {
        /// <inheritdoc/>
        public CommandConfiguration Configuration { get; set; }

        /// <inheritdoc/>
        public IServiceProvider Services { get; set; }

        /// <summary>
        ///     Gets or sets the pipeline service that will populate the framework.
        /// </summary>
        public T Conveyor { get; set; }

        /// <inheritdoc/>
        ICommandConveyor IFrameworkBuilder.Conveyor { get; set; }

        /// <summary>
        ///     Creates a new <see cref="FrameworkBuilder{T}"/>.
        /// </summary>
        internal FrameworkBuilder(T pipelineService)
        {
            Conveyor = pipelineService;
            Services = EmptyServiceProvider.Instance;
            Configuration = new CommandConfiguration();
        }

        /// <inheritdoc/>
        public IFrameworkBuilder ConfigureCommands(Action<CommandConfiguration> action)
        {
            action(Configuration);
            return this;
        }

        /// <inheritdoc/>
        public IFrameworkBuilder ConfigureServices(IServiceProvider services)
        {
            Services = services;
            return this;
        }

        /// <inheritdoc/>
        public IFrameworkBuilder ConfigureConveyor(ICommandConveyor conveyor)
        {
            Conveyor = conveyor.Cast<T>();
            return this;
        }

        /// <inheritdoc/>
        public IFrameworkBuilder ConfigureConveyor(T conveyor)
        {
            Conveyor = conveyor;
            return this;
        }

        /// <inheritdoc/>
        public ICommandFramework Build()
        {
            if (Conveyor is null)
                throw new ArgumentNullException(nameof(Conveyor));

            return new CommandFramework<T>(Services, Configuration, Conveyor);
        }

        /// <inheritdoc/>
        public async Task BuildAndRunAsync(CancellationToken cancellationToken = default)
            => await Build().RunAsync();
    }
}
