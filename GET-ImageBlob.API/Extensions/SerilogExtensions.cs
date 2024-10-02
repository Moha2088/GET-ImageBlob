using Serilog;

namespace GET_ImageBlob.API.Extensions
{
    public static class SerilogExtensions
    {
        public static void AddSerilogService(this IHostBuilder builder)
        {
            builder.UseSerilog((ctx, cfg) =>
            cfg.MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("..\\SeriLogs\\GeneratedLog.txt",
                              rollingInterval: RollingInterval.Day,
                              rollOnFileSizeLimit: true,
                              outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                              ));
        }
    }
}
