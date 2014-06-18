//MyClassLibrary https://github.com/mhertzfeld/MyClassLibrary
//Copyright (c) 2014, Matthew Hertzfeld https://github.com/mhertzfeld
//All rights reserved.



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