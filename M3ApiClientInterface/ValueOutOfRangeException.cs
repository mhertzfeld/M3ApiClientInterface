using System;


namespace M3ApiClientInterface
{
    public class ValueOutOfRangeException
        : System.Exception
    {
        public ValueOutOfRangeException(String variableName)
            : base("The value for '" + variableName + "' is out of range.")
        {

        }
    }
}