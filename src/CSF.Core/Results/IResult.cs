using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     The interface implemented by all result types.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        ///     True if succesful, False if not.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        ///     The error message. <c>null</c> if not applicable.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        ///     The result's inner exception. <c>null</c> if not applicable.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        ///     Calls the pipeline to handle the exposed result.
        /// </summary>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        internal async Task RequestToHandleAsync<TContext>(TContext context, ICommandConveyor service, CancellationToken cancellationToken)
            where TContext : IContext
        {
            await service.OnResultAsync(context, this, cancellationToken);
        }
    }
}
