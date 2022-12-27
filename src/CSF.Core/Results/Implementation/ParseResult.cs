using System;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents typereader parsing results.
    /// </summary>
    public readonly struct ParseResult : IResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }

        /// <inheritdoc/>
        public string ErrorMessage { get; }

        /// <summary>
        ///     The result objects of this parse result.
        /// </summary>
        internal object[] Result { get; }

        /// <inheritdoc/>
        public Exception Exception { get; }

        private ParseResult(bool success, object[] result = null, string msg = null, Exception exception = null)
        {
            IsSuccess = success;
            ErrorMessage = msg;
            Exception = exception;
            Result = result;
        }

        public static implicit operator ValueTask<ParseResult>(ParseResult result)
            => result.AsValueTask();

        /// <summary>
        ///     Creates a failed result with provided parameters.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static ParseResult FromError(string errorMessage, Exception exception = null)
            => new ParseResult(false, null, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        internal static ParseResult FromSuccess(object[] value)
            => new ParseResult(true, value);
    }
}
