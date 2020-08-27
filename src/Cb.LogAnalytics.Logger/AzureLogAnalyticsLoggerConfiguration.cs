
namespace Cb.LogAnalytics.Logger
{
    public class AzureLogAnalyticsLoggerConfiguration
    {
        public string WorkspaceId { get; set; }
        
        public string PrimaryKey { get; set; } 
        
        /// <summary>
        /// Name of the log type within log analytics. Leave empty for entry assembly name
        /// </summary>
        public string LogTypeName { get; set; }
        
        /// <summary>
        /// You can use an optional field to specify the timestamp from the data.  
        /// If the time field is not specified, Azure Monitor assumes the time is the message ingestion time
        /// </summary>
        public string TimeStampFieldName { get; set; }
    }
}