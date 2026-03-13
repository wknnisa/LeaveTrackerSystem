using Serilog;

namespace LeaveTrackerSystem.WebApp.Logging
{
    public static class LoggingExtensions
    {
        public static void ConfigureSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
