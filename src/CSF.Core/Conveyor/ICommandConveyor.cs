using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents the default interface for a command conveyor. This type cannot be used to invoke commands.
    /// </summary>
    public interface ICommandConveyor
    {
        /// <summary>
        ///     Waits for an input and returns it to the caller.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>The provided input.</returns>
        ValueTask<string> GetInputAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Builds a new <see cref="IContext"/> from the provided raw string.
        /// </summary>
        /// <typeparam name="T">The context type to return.</typeparam>
        /// <param name="rawInput">The raw input.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>The built command context.</returns>
        ValueTask<IContext> BuildContextAsync(string rawInput, CancellationToken cancellationToken);

        /// <summary>
        ///     Called when a command succesfully (or unsuccesfully) executed.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="result">The result returned by the caller.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        Task OnResultAsync<TContext>(TContext context, IResult result, CancellationToken cancellationToken)
            where TContext : IContext;

        /// <summary>
        ///     Called when a command group is succesfully registered to the command framework.
        /// </summary>
        /// <param name="component">The component (command group) that has been registered.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        Task OnRegisteredAsync(IConditionalComponent component, CancellationToken cancellationToken);

        /// <summary>
        ///     Called when a typereader is succesfully registered to the command framework.
        /// </summary>
        /// <param name="typeReader">The type reader to register.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        Task OnRegisteredAsync(ITypeReader typeReader, CancellationToken cancellationToken);

        /// <summary>
        ///     Returns the error message when the context input was not found.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="SearchResult"/> holding the returned error.</returns>
        SearchResult OnCommandNotFound<TContext>(TContext context)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error message when no best match was found for the command.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="SearchResult"/> holding the returned error.</returns>
        SearchResult OnBestOverloadUnavailable<TContext>(TContext context)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error message when a service to inject into the module has not been found in the provided <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="dependency">Information about the service to inject.</param>
        /// <returns>A <see cref="ConstructionResult"/> holding the returned error.</returns>
        ConstructionResult OnServiceNotFound<TContext>(TContext context, DependencyInfo dependency)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error message when the module in question cannot be cast to an <see cref="IModuleBase"/>.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="module">The module that failed to cast to an <see cref="IModuleBase"/>.</param>
        /// <returns>A <see cref="ConstructionResult"/> holding the returned error.</returns>
        ConstructionResult OnInvalidModule<TContext>(TContext context, ModuleInfo module)
            where TContext : IContext;

        /// <summary>
        ///     Called when an optional parameter has a lacking value.
        /// </summary>
        /// <remarks>
        ///     The result will fail to resolve and exit execution if the type does not match the provided <see cref="ParameterInfo.Type"/> or <see cref="Type.Missing"/>.
        /// </remarks>
        /// <param name="context"></param>
        /// <param name="param"></param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="TypeReaderResult"/> for the target parameter.</returns>
        TypeReaderResult OnMissingValue<TContext>(TContext context, ParameterInfo param)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error when <see cref="OnMissingValue{T}(T, ParameterInfo)"/> returned a type that did not match the expected type.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="expectedType">The type that was expected to return.</param>
        /// <param name="returnedType">The returned type.</param>
        /// <returns>A <see cref="ParseResult"/> holding the returned error.</returns>
        ParseResult OnMissingReturnedInvalid<TContext>(TContext context, Type expectedType, Type returnedType)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error when <see cref="OnMissingValue{T}(T, ParameterInfo)"/> failed to return a valid result. 
        ///     This method has to return <see cref="Type.Missing"/> if no self-implemented value has been returned.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="ParseResult"/> holding the returned error.</returns>
        ParseResult OnOptionalNotPopulated<TContext>(TContext context)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error message when an unhandled return type has been returned from the command method.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="returnValue">The returned value of the method.</param>
        /// <returns>An <see cref="ExecuteResult"/> holding the returned error.</returns>
        ExecuteResult OnUnhandledReturnType<TContext>(TContext context, object returnValue)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error message when command execution fails on the user's end.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="ex">The exception that occurred while executing the command.</param>
        /// <returns>An <see cref="ExecuteResult"/> holding the returned error.</returns>
        ExecuteResult OnUnhandledException<TContext>(TContext context, CommandInfo command, Exception ex)
            where TContext : IContext;
    }
}