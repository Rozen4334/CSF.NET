using System;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents precondition results.
    /// </summary>
    public readonly struct PreconditionResult : IResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }

        /// <inheritdoc/>
        public string ErrorMessage { get; }

        /// <inheritdoc/>
        public Exception Exception { get; }

        private PreconditionResult(bool success, string msg = null, Exception exception = null)
        {
            IsSuccess = success;
            ErrorMessage = msg;
            Exception = exception;
        }

        public static implicit operator ValueTask<PreconditionResult>(PreconditionResult result)
            => result.AsValueTask();

        /// <summary>
        ///     Creates a failed result with provided parameters.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static PreconditionResult FromError(string errorMessage, Exception exception = null)
            => new PreconditionResult(false, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        public static PreconditionResult FromSuccess()
            => new PreconditionResult(true);
    }
}
