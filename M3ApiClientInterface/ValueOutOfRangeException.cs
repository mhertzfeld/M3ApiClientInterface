using System;


namespace M3ApiClientInterface
{
    public class ValueOutOfRangeException
        : System.Exception
    {
        public ValueOutOfRangeException(String variableName)
            : base(String.Format("The value for '{0}' is out of range.", variableName))
        {

        }
    }
}