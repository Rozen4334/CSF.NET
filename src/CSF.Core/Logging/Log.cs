using System;

namespace CSF
{
    /// <summary>
    ///     Represents a log message.
    /// </summary>
    public readonly struct Log
    {
        /// <summary>
        ///     The message of this log.
        /// </summary>
        public string Value { get; }

        /// <summary>
        ///     The severity of this log.
        /// </summary>
        public LogLevel LogLevel { get; }

        /// <summary>
        ///     The exception of this log. <see langword="null"/> if none.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        ///     Creates a new log with the provided severity.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="value"></param>
        /// <param name="exception"></param>
        public Log(LogLevel level, string value, Exception exception = null)
        {
            Value = value;
            LogLevel = level;
            Exception = exception;
        }
    }
}
