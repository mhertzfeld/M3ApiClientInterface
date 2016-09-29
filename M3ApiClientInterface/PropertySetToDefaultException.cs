using System;


namespace M3ApiClientInterface
{
    public class PropertySetToDefaultException
        : System.Exception
    {
        public PropertySetToDefaultException(String propertyName)
            : base(String.Format("'{0}' can not be set to its default.", propertyName))
        {
            
        }
    }
}