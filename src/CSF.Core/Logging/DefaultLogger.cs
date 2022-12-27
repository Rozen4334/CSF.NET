using System;

namespace CSF
{
    /// <summary>
    ///     The default logger. This logger does not support overrides. Implement <see cref="ILogger"/> for message format customization.
    /// </summary>
    public sealed class DefaultLogger : ILogger
    {
        private readonly object _lock = new object();

        /// <inheritdoc/>
        public LogLevel LogLevel { get; set; }

        /// <inheritdoc/>
        public Action<Log> SendAction { get; set; }

        public DefaultLogger(LogLevel level, Action<Log> sendAction)
        {
            LogLevel = level;
            SendAction = sendAction;
        }

        internal static DefaultLogger Create(LogLevel level)
            => new DefaultLogger(level, (x) =>
            {
                var message = $"{string.Format("{0:HH:mm:ss}", DateTime.UtcNow)} [{x.LogLevel}]: {x.Value}";

                if (x.Exception != null)
                    message += $"\n -> {x.Exception.ToString().Replace("\n\r", "\n\r -> ")}";

                Console.WriteLine(message);
            });

        /// <inheritdoc/>
        public void Trace(string message, Exception exception = null)
            => Write(new Log(LogLevel.Trace, message, exception));

        /// <inheritdoc/>
        public void Debug(string message, Exception exception = null)
            => Write(new Log(LogLevel.Debug, message, exception));

        /// <inheritdoc/>
        public void Info(string message, Exception exception = null)
            => Write(new Log(LogLevel.Information, message, exception));

        /// <inheritdoc/>
        public void Warning(string message, Exception exception = null)
            => Write(new Log(LogLevel.Warning, message, exception));

        /// <inheritdoc/>
        public void Error(string message, Exception exception)
            => Write(new Log(LogLevel.Error, message, exception));

        /// <inheritdoc/>
        public void Critical(string message, Exception exception)
            => Write(new Log(LogLevel.Critical, message, exception));

        /// <inheritdoc/>
        public void Write(Log log)
        {
            if (LogLevel is LogLevel.None)
                return;

            if (log.LogLevel >= LogLevel)
                Send(log);

            if (log.LogLevel is LogLevel.Critical)
                ResolveCriticalException(log);
        }

        /// <inheritdoc/>
        public void Send(Log log)
        {
            lock (_lock)
                SendAction(log);
        }

        private void ResolveCriticalException(Log log)
            => throw new InvalidOperationException("A critical exception has occurred.", log.Exception);
    }
}
