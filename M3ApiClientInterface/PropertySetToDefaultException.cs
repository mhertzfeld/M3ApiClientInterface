using System;


namespace M3ApiClientInterface
{
    public class PropertySetToDefaultException
        : System.Exception
    {
        public PropertySetToDefaultException(String propertyName)
            : base("'" + propertyName + "' can not be set to its default.")
        {
            
        }
    }
}