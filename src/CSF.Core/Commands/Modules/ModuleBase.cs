using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents a generic command base to implement commands with.
    /// </summary>
    /// <typeparam name="T">The <see cref="IContext"/> expected to use for this command.</typeparam>
    public abstract class ModuleBase<T> : IModuleBase where T : IContext
    {
        /// <summary>
        ///     Gets the command's context.
        /// </summary>
        public T Context { get; private set; }
        internal void SetContext(IContext context)
            => Context = (T)context;

        /// <summary>
        ///     Displays all information about the command thats currently in scope.
        /// </summary>
        public CommandInfo CommandInfo { get; private set; }
        internal void SetInformation(CommandInfo info)
            => CommandInfo = info;

        /// <summary>
        ///     The logger of the framework being used.
        /// </summary>
        public ILogger Logger { get; private set; }
        internal void SetLogger(ILogger logger)
            => Logger = logger;

        /// <inheritdoc/>
        public virtual ExecuteResult Error(string message, params object[] values)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Format(message, values));
            Console.ResetColor();

            return ExecuteResult.FromSuccess();
        }

        /// <inheritdoc/>
        public virtual Task<ExecuteResult> ErrorAsync(string message, params object[] values)
            => Task.FromResult(Error(message, values));

        /// <inheritdoc/>
        public virtual ExecuteResult Success(string message, params object[] values)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Format(message, values));
            Console.ResetColor();

            return ExecuteResult.FromSuccess();
        }

        /// <inheritdoc/>
        public virtual Task<ExecuteResult> SuccessAsync(string message, params object[] values)
            => Task.FromResult(Success(message, values));

        /// <inheritdoc/>
        public virtual ExecuteResult Info(string message, params object[] values)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(string.Format(message, values));
            Console.ResetColor();

            return ExecuteResult.FromSuccess();
        }

        /// <inheritdoc/>
        public virtual Task<ExecuteResult> InfoAsync(string message, params object[] values)
            => Task.FromResult(Info(message, values));

        /// <inheritdoc/>
        public virtual ExecuteResult Respond(string message, params object[] values)
        {
            Console.WriteLine(string.Format(message, values));

            return ExecuteResult.FromSuccess();
        }

        /// <inheritdoc/>
        public virtual Task<ExecuteResult> RespondAsync(string message, params object[] values)
            => Task.FromResult(Respond(message, values));

        /// <summary>
        ///     Invoked right before a command is executed.
        /// </summary>
        /// <remarks>
        ///     This method executes after all pipeline steps have resolved, it will always enter the command directly after.
        /// </remarks>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public virtual Task BeforeExecuteAsync(CommandInfo info, T context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Invoked right after a command is executed.
        /// </summary>
        /// <remarks>
        ///     This method does not execute if the command fails to finish execution or returns an error.
        /// </remarks>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public virtual Task AfterExecuteAsync(CommandInfo info, T context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
