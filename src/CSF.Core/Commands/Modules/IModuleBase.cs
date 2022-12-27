using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents the default interface for <see cref="ModuleBase{T}"/>.
    /// </summary>
    public interface IModuleBase
    {
        /// <summary>
        ///     Formats and sends an error response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        ExecuteResult Error(string message, params object[] values);

        /// <summary>
        ///     Formats and sends an error response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        Task<ExecuteResult> ErrorAsync(string message, params object[] values);

        /// <summary>
        ///     Formats and sends a successful response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        ExecuteResult Success(string message, params object[] values);

        /// <summary>
        ///     Formats and sends a successful response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        Task<ExecuteResult> SuccessAsync(string message, params object[] values);

        /// <summary>
        ///     Formats and sends an informational response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        ExecuteResult Info(string message, params object[] values);

        /// <summary>
        ///     Formats and sends an informational response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        Task<ExecuteResult> InfoAsync(string message, params object[] values);

        /// <summary>
        ///     Formats and sends a plain response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        ExecuteResult Respond(string message, params object[] values);

        /// <summary>
        ///     Formats and sends a plain response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        Task<ExecuteResult> RespondAsync(string message, params object[] values);
    }
}
