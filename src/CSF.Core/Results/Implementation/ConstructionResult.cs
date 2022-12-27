using System;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents constructor results.
    /// </summary>
    public readonly struct ConstructionResult : IResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }

        /// <inheritdoc/>
        public string ErrorMessage { get; }

        /// <summary>
        ///     The result object of this reader.
        /// </summary>
        internal IModuleBase Result { get; }

        /// <inheritdoc/>
        public Exception Exception { get; }

        private ConstructionResult(bool success, IModuleBase result = null, string msg = null, Exception exception = null)
        {
            IsSuccess = success;
            ErrorMessage = msg;
            Exception = exception;
            Result = result;
        }

        public static implicit operator ValueTask<ConstructionResult>(ConstructionResult result)
            => result.AsValueTask();

        /// <summary>
        ///     Creates a failed result with provided parameters.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static ConstructionResult FromError(string errorMessage, Exception exception = null)
            => new ConstructionResult(false, null, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        internal static ConstructionResult FromSuccess(IModuleBase value)
            => new ConstructionResult(true, value);
    }
}
