//MyClassLibrary https://github.com/mhertzfeld/MyClassLibrary
//Copyright (c) 2014, Matthew Hertzfeld https://github.com/mhertzfeld
//All rights reserved.



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