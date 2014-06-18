using System;
using System.Diagnostics;
using System.Text;


namespace ConsoleApplication1
{
    internal static class LogWriter
    {
        public static void WriteToLog(Exception exception)
        {
            Debug.WriteLine(exception.ToString());
        }

        public static void WriteToLog(TraceEventType traceEventType, String title, String message, String category = null, Int32 priority = 0, Int32 eventId = 0)
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