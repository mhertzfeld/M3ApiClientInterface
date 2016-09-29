using System;


namespace M3ApiClientInterface
{
    public class PropertySetToOutOfRangeValueException
        : System.Exception
    {
        public PropertySetToOutOfRangeValueException(String propertyName)
            : base(String.Format("'{0}' can not be set to an out of range vale.", propertyName))
        {
            
        }
    }
}