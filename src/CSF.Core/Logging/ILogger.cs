using System;

namespace CSF
{
    /// <summary>
    ///     Represents a logger to write logs to.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        ///     The log level inherited from the <see cref="CommandConfiguration"/>. This value can be overwritten at runtime to change the default severity for all log messages.
        /// </summary>
        LogLevel LogLevel { get; set; }

        /// <summary>
        ///     The action used to log the provided response.
        /// </summary>
        Action<Log> SendAction { get; set; }

        /// <summary>
        ///     Writes a trace log.
        /// </summary>
        void Trace(string message, Exception exception = null);

        /// <summary>
        ///     Writes a debug log.
        /// </summary>
        void Debug(string message, Exception exception = null);

        /// <summary>
        ///     Writes an information log.
        /// </summary>
        void Info(string message, Exception exception = null);

        /// <summary>
        ///     Writes a warning log.
        /// </summary>
        void Warning(string message, Exception exception = null);

        /// <summary>
        ///     Writes an error log.
        /// </summary>
        void Error(string message, Exception exception = null);

        /// <summary>
        ///     Writes a critical log.
        /// </summary>
        void Critical(string message, Exception exception = null);

        /// <summary>
        ///     Writes a log.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="log"></param>
        void Write(Log log);

        /// <summary>
        ///     Uses the <see cref="SendAction"/> to send the provided message to the target handler.
        /// </summary>
        /// <param name="message"></param>
        void Send(Log message);
    }
}
