using isportDMS.Shared.Logging;
using Microsoft.Extensions.Logging;

namespace Cb.LogAnalytics.Logger
{
    public static class AzureLogAnalyticsLoggerExtensions
    {
        public static void AddSmartConnectLogger(this ILoggerFactory loggerFactory, AzureLogAnalyticsLoggerConfiguration configuration)
        {
            loggerFactory.AddProvider(new AzureLogAnalyticsLoggerProvider(configuration));
        }

        public static ILoggingBuilder AddSmartConnectLogger(this ILoggingBuilder loggingBuilder, AzureLogAnalyticsLoggerConfiguration configuration)
        {
            return loggingBuilder.AddProvider(new AzureLogAnalyticsLoggerProvider(configuration));
        }
    }
}
