using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents an interface used by <see cref="TypeReader{T}"/> without needing to internally provide the generic parameter.
    /// </summary>
    /// <remarks>
    ///     Do not use this interface to build type readers on. Use <see cref="TypeReader{T}"/> instead.
    /// </remarks>
    public interface ITypeReader
    {
        /// <summary>
        ///     The type this typereader wraps.
        /// </summary>
        Type Type { get; }

        /// <summary>
        ///     Reads the provided parameter value and tries to parse it into the target type.
        /// </summary>
        /// <param name="context">The <see cref="IContext"/> passed into this pipeline.</param>
        /// <param name="parameter">The parameter to implement.</param>
        /// <param name="value">The string value that will populate this parameter.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="TypeReaderResult"/> with provided error or successful parse.</returns>
        ValueTask<TypeReaderResult> ReadAsync(IContext context, ParameterInfo parameter, object value, CancellationToken cancellationToken);

        /// <summary>
        ///     Calls the pipeline to handle the exposed result.
        /// </summary>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        internal async Task RequestToHandleAsync(ICommandConveyor service, CancellationToken cancellationToken)
        {
            await service.OnRegisteredAsync(this, cancellationToken);
        }
    }
}
