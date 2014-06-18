using System;
using System.Diagnostics;


namespace ConsoleApplication1
{
    public class TraceListener
        : System.Diagnostics.TraceListener
    {
        public override void Write(string message)
        {
            LogWriter.WriteToLog(TraceEventType.Error, "Trace Message", message, "Trace");
        }

        public override void WriteLine(string message)
        {
            LogWriter.WriteToLog(TraceEventType.Error, "Trace Message", message, "Trace");
        }
    }
}