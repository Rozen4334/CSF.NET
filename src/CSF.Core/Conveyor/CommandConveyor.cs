using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents the default command factory. 
    ///     <br/>
    ///     This type <b>can</b> be used when providing the <see cref="CommandFramework{T}"/> with a provider, or overwritten for custom implementations.
    /// </summary>
    public class CommandConveyor : ICommandConveyor
    {
        /// <inheritdoc/>
        public virtual async ValueTask<string> GetInputAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return Console.ReadLine();
        }

        /// <inheritdoc/>
        public virtual async ValueTask<IContext> BuildContextAsync(string rawInput, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return new CommandContext(rawInput);
        }

        /// <inheritdoc/>
        public virtual async Task OnResultAsync<TContext>(TContext context, IResult result, CancellationToken cancellationToken)
            where TContext : IContext
        {
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task OnRegisteredAsync(IConditionalComponent component, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task OnRegisteredAsync(ITypeReader typeReader, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual SearchResult OnCommandNotFound<TContext>(TContext context)
            where TContext : IContext
            => SearchResult.FromError($"Failed to find command with name: {context.Name}.");

        /// <inheritdoc/>
        public virtual SearchResult OnBestOverloadUnavailable<TContext>(TContext context)
            where TContext : IContext
            => SearchResult.FromError($"Failed to find overload that best matches input: {context.Name}.");

        /// <inheritdoc/>
        public virtual ConstructionResult OnServiceNotFound<TContext>(TContext context, DependencyInfo dependency)
            where TContext : IContext
            => ConstructionResult.FromError($"The service of type {dependency.Type.FullName} does not exist in the current {nameof(IServiceProvider)}.");

        /// <inheritdoc/>
        public virtual ConstructionResult OnInvalidModule<TContext>(TContext context, ModuleInfo module)
            where TContext : IContext
            => ConstructionResult.FromError($"Failed to interpret module of type {module.Type.FullName} with type of {nameof(ModuleBase<TContext>)}");

        /// <inheritdoc/>
        public virtual TypeReaderResult OnMissingValue<TContext>(TContext context, ParameterInfo param)
            where TContext : IContext
            => TypeReaderResult.FromSuccess(Type.Missing);

        /// <inheritdoc/>
        public virtual ParseResult OnMissingReturnedInvalid<TContext>(TContext context, Type expectedType, Type returnedType)
            where TContext : IContext
            => ParseResult.FromError($"Returned type does not match expected result. Expected: '{expectedType.Name}'. Got: '{returnedType.Name}'");

        /// <inheritdoc/>
        public virtual ParseResult OnOptionalNotPopulated<TContext>(TContext context)
            where TContext : IContext
            => ParseResult.FromError($"Optional parameter did not get {nameof(Type.Missing)} or self-populated value.");

        /// <inheritdoc/>
        public virtual ExecuteResult OnUnhandledReturnType<TContext>(TContext context, object returnValue)
            where TContext : IContext
            => ExecuteResult.FromError($"Received an unhandled type from method execution: {returnValue.GetType().Name}. \n\rConsider overloading {nameof(OnUnhandledReturnType)} if this is intended.");

        /// <inheritdoc/>
        public virtual ExecuteResult OnUnhandledException<TContext>(TContext context, CommandInfo command, Exception ex)
            where TContext : IContext
            => ExecuteResult.FromError(ex.Message, ex);
    }
}
