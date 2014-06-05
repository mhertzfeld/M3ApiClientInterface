using System;
using System.Diagnostics;
using System.Text;


namespace ConsoleApplication1
{
    internal class LogWriter
        : MyClassLibrary.Logging.ILogWriter
    {
        public void WriteToLog(Exception exception)
        {
            Debug.WriteLine(exception.ToString());
        }

        public void WriteToLog(TraceEventType traceEventType, String title, String message, String category = null, Int32 priority = 0, Int32 eventId = 0)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(title);
            stringBuilder.AppendLine(traceEventType.ToString());
            stringBuilder.AppendLine(category);
            stringBuilder.AppendLine(message);
            
            Debug.WriteLine(stringBuilder.ToString());
        }
    }
}