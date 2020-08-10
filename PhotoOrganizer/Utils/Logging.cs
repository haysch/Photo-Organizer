using Microsoft.Extensions.Logging;

namespace PhotoOrganizer.Utils
{
    internal static class Logging
    {
        private static ILoggerFactory _loggerFactory;

        internal static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_loggerFactory is null)
                {
                    _loggerFactory = new LoggerFactory();
                }
                return _loggerFactory;
            }

            set => _loggerFactory = value;
        }

        internal static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();

        internal static ILogger CreateLogger(string name) => LoggerFactory.CreateLogger(name);
    }
}
