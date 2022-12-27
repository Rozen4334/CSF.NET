using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSF
{
    public readonly struct CheckResult : IResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }

        /// <inheritdoc/>
        public string ErrorMessage { get; }

        /// <summary>
        ///     The commands that passed through the preconditions, the first of which will be selected for execution.
        /// </summary>
        internal CommandInfo[] Result { get; }

        /// <inheritdoc/>
        public Exception Exception { get; }

        private CheckResult(bool success, CommandInfo[] matches, string msg = null, Exception exception = null)
        {
            Result = matches;
            IsSuccess = success;
            ErrorMessage = msg;
            Exception = exception;
        }

        public static implicit operator ValueTask<CheckResult>(CheckResult result)
            => result.AsValueTask();

        /// <summary>
        ///     Creates a failed result with provided parameters.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static CheckResult FromError(string errorMessage, Exception exception = null)
            => new CheckResult(false, null, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        public static CheckResult FromSuccess(CommandInfo[] matches)
            => new CheckResult(true, matches);
    }
}
