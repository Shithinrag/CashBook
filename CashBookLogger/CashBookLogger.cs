using Serilog;
using Serilog.Events;

namespace Logger
{
    
        public class CashBookLogger
        {
            private static ILogger _logger;
            private static CashBookLogger _instance;

            private CashBookLogger(CashBookLoggerSettings cashBookLoggerSettings)
            {
                if (_logger == null)
                    // _logger = new LoggerConfiguration().WriteTo.File(loggerSettings.WriteTo,
                    //   rollingInterval: GeRollingType(loggerSettings.RollingType)).CreateLogger();

                    _logger = new LoggerConfiguration().MinimumLevel.Verbose()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .Enrich.FromLogContext()
                            .WriteTo.File(cashBookLoggerSettings.WriteTo,
                                outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff zzz} [{Level}] ({SourceContext}.{Method}) {Message}{NewLine}{Exception}")
                            .CreateLogger();
            }
            public static CashBookLogger Create(CashBookLoggerSettings loggerOptions)
            {
                // get existing instance
                if (_instance == null)
                    _instance = new CashBookLogger(loggerOptions);
                return _instance;
            }
            public static void LogInformation(string message)
            {
                _logger.Information(message);
            }

            public static void LogError(Exception ex, string? message = null)
            {
                if (message == null)
                {
                    message = ex.Message;
                }

                _logger.Error(ex, message);
            }
        }
    }
