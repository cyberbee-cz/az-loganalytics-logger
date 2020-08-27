using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Cb.LogAnalytics.Logger
{
    public class AzureLogAnalyticsLogger : ILogger, IAsyncDisposable
    {
        private readonly AzureLogAnalyticsLoggerConfiguration configuration;
        public AzureLogAnalyticsLogger(AzureLogAnalyticsLoggerConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public async ValueTask DisposeAsync()
        {
            await Task.Factory.StartNew(() => {});
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // Create a hash for the API signature
			var datestring = DateTime.UtcNow.ToString("r");
            var jsonMessage = JsonSerializer.Serialize(new LogMessage
            {
                LogLevel = logLevel.ToString(),
                Message = formatter(state, exception),
                Exception = exception != null ? $"Type: {exception.GetType().Name}. Message: {exception.Message}. Stacktrace: {exception.StackTrace}" : string.Empty
            });
			var jsonBytes = Encoding.UTF8.GetBytes(jsonMessage);
			string stringToHash = "POST\n" + jsonBytes.Length + "\napplication/json\n" + "x-ms-date:" + datestring + "\n/api/logs";
			string hashedString = BuildSignature(stringToHash, configuration.PrimaryKey);
			string signature = "SharedKey " + configuration.WorkspaceId + ":" + hashedString;

			var resp = PostData(signature, datestring, jsonMessage).GetAwaiter().GetResult();

            var responseContent = resp.Content;
            string result = responseContent.ReadAsStringAsync().Result;
        }

        // Build the API signature
		public static string BuildSignature(string message, string secret)
		{
			var encoding = new System.Text.ASCIIEncoding();
			byte[] keyByte = Convert.FromBase64String(secret);
			byte[] messageBytes = encoding.GetBytes(message);
			using (var hmacsha256 = new HMACSHA256(keyByte))
			{
				byte[] hash = hmacsha256.ComputeHash(messageBytes);
				return Convert.ToBase64String(hash);
			}
		}

		// Send a request to the POST API endpoint
		public async Task<HttpResponseMessage> PostData(string signature, string date, string json)
		{
            string url = "https://" + configuration.WorkspaceId + ".ods.opinsights.azure.com/api/logs?api-version=2016-04-01";

            var client = new System.Net.Http.HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Log-Type", string.IsNullOrWhiteSpace(configuration.LogTypeName) ? Assembly.GetEntryAssembly().GetName().Name : configuration.LogTypeName);
            client.DefaultRequestHeaders.Add("Authorization", signature);
            client.DefaultRequestHeaders.Add("x-ms-date", date);
            if(!string.IsNullOrWhiteSpace(configuration.TimeStampFieldName))
                client.DefaultRequestHeaders.Add("time-generated-field", configuration.TimeStampFieldName);

            var httpContent = new StringContent(json, Encoding.UTF8);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return await client.PostAsync(new Uri(url), httpContent);
		}
    }
}