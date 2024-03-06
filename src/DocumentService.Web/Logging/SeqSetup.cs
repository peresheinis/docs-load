using Serilog;
using System.Reflection;

namespace DocumentService.Web.Logging
{
    public static class SeqSetup
    {
        public static void SetupLogging(IHostEnvironment hostEnvironment, ConfigurationManager configuration)
        {
            var logConf = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.WithProperty("Environment", hostEnvironment.EnvironmentName);

            // The identifier is an assembly name plus an environment name in a snake-case
            var applicationIdentifier =
                $"{Assembly.GetEntryAssembly()?.GetName().Name?.ToLower()}-{hostEnvironment.EnvironmentName.ToLower()}"
                    .Replace(".", "-");

            var seqConfiguration = configuration
                                       .GetSection(SeqConfiguration.Seq)
                                       .Get<SeqConfiguration>()
                                   ?? throw new ArgumentNullException(SeqConfiguration.Seq);

            logConf = logConf
                .WriteTo.Seq(seqConfiguration.Host, apiKey: seqConfiguration.ApiKey)
                .Enrich.WithProperty("AppId", applicationIdentifier);

            Log.Logger = logConf.CreateLogger();
        }
    }

    public class SeqConfiguration
    {
        public const string Seq = "Seq";

        public string Host { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }
}
