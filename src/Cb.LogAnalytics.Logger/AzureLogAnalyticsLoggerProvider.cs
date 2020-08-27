using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Cb.LogAnalytics.Logger
{
    public class AzureLogAnalyticsLoggerProvider : ILoggerProvider
    {
        private readonly AzureLogAnalyticsLoggerConfiguration configuration;
        private readonly ICollection<ILogger> loggers;

        public AzureLogAnalyticsLoggerProvider(AzureLogAnalyticsLoggerConfiguration configuration)
        {
            this.configuration = configuration;
            this.loggers = new List<ILogger>(0);
        }

        public ILogger CreateLogger(string categoryName)
        {
            loggers.Add(new AzureLogAnalyticsLogger(configuration));
            return loggers.Last();
        }

        public void Dispose()
        {
            loggers.Cast<AzureLogAnalyticsLogger>().First().DisposeAsync().GetAwaiter().GetResult();
        }
    }
}