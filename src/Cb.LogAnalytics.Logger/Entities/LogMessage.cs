namespace isportDMS.Shared.Logging
{
    public class LogMessage
    {
        public LogMessage()
        {}

        public LogMessage(string logLevel, string message, string exc = null, params object[] parameters)
        {
            LogLevel = logLevel;
            Message = message;
            Exception = exc;
            Parameters = parameters;
        }

        public string LogLevel { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public object[] Parameters { get; set; }
    }
}