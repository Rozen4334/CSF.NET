using System;
using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     Base configuration for the <see cref="CommandFramework"/>.
    /// </summary>
    public sealed class CommandConfiguration
    {
        /// <summary>
        ///     If enabled, commands will execute asynchronously, ensuring that sync handlers will not wait out the execution before returning to the source method.
        /// </summary>
        /// <remarks>
        ///     When opting in to asynchronous execution, <see cref="CommandFramework{T}.ExecuteCommandsAsync{TContext}(TContext, IServiceProvider, System.Threading.CancellationToken)"/> will always return success immediately after being invoked.
        ///     <br/>
        ///     Read more about the reason behind this in it's XML documentation.
        /// </remarks>
        public bool DoAsynchronousExecution { get; set; } = false;

        /// <summary>
        ///     If enabled, all commands that match the provided input will execute, no matter the priority.
        /// </summary>
        /// <remarks>
        ///     The commands executed will be handled in order. 
        ///     Every command will have its module remade transiently because the framework cannot guarantee all commands are from the same module.
        /// </remarks>
        public bool ExecuteAllValidMatches { get; set; } = false;

        /// <summary>
        ///     Represents the log level at which the <see cref="ILogger"/> is created during the creation of the target <see cref="CommandFramework"/>.
        /// </summary>
        /// <remarks>
        ///     This value can freely be changed by changing the default <see cref="ILogger.LogLevel"/> in the <see cref="CommandFramework.Logger"/> property or other references across execution.
        /// </remarks>
        public LogLevel DefaultLogLevel { get; set; } = LogLevel.Information;

        /// <summary>
        ///     The assemblies that should be used for registering commands, typereaders and event resolvers.
        /// </summary>
        public Assembly[] RegistrationAssemblies { get; set; } = new[] { Assembly.GetEntryAssembly() };

        /// <summary>
        ///     The prefixes that should be used to validate incoming command values.
        /// </summary>
        public PrefixProvider Prefixes { get; set; }

        /// <summary>
        ///     The typereaders that should be used to parse command input.
        /// </summary>
        public TypeReaderProvider TypeReaders { get; set; }
    }
}
