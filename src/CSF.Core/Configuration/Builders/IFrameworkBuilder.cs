using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents a builder for the <see cref="CommandFramework"/>.
    /// </summary>
    public interface IFrameworkBuilder
    {
        /// <summary>
        ///     Gets or sets the pipeline service that will populate the framework.
        /// </summary>
        ICommandConveyor Conveyor { get; set; }

        /// <summary>
        ///     Gets or sets the services used to configure the framework.
        /// </summary>
        IServiceProvider Services { get; set; }

        /// <summary>
        ///     Gets or sets the configuration used to configure the framework.
        /// </summary>
        CommandConfiguration Configuration { get; set; }

        /// <summary>
        ///     Modifies the <see cref="Configuration"/> of this builder.
        /// </summary>
        /// <param name="action"></param>
        /// <returns>The current <see cref="IFrameworkBuilder"/> for chaining calls.</returns>
        IFrameworkBuilder ConfigureCommands(Action<CommandConfiguration> action);

        /// <summary>
        ///     Sets the <see cref="Services"/> of this builder.
        /// </summary>
        /// <param name="services"></param>
        /// <returns>The current <see cref="IFrameworkBuilder"/> for chaining calls.</returns>
        IFrameworkBuilder ConfigureServices(IServiceProvider services);

        /// <summary>
        ///     Sets the <see cref="Conveyor"/> of this builder.
        /// </summary>
        /// <param name="conveyor"></param>
        /// <returns>The current <see cref="IFrameworkBuilder"/> for chaining calls.</returns>
        IFrameworkBuilder ConfigureConveyor(ICommandConveyor conveyor);

        /// <summary>
        ///     Builds the current builder into a new <see cref="CommandFramework{T}"/>.
        /// </summary>
        /// <returns>A new <see cref="IFrameworkBuilder"/> with provided builder configuration.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        ICommandFramework Build();

        /// <summary>
        ///     Builds the current builder into a new <see cref="CommandFramework{T}"/> and immediately calls <see cref="CommandFramework{T}.RunAsync(bool, CancellationToken)"/>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        Task BuildAndRunAsync(CancellationToken cancellationToken = default);
    }
}
