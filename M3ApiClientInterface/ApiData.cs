using MyClassLibrary;
using System;


namespace M3ApiClientInterface
{
    public class ApiData
    {
        //FIELDS
        protected String api;

        protected String method;


        //PROPERTIES
        public virtual String Api
        {
            get { return api; }

            set
            {
                if (value == default(String))
                { throw new PropertySetToDefaultException("Api"); }

                api = value;
            }
        }

        public virtual String Method
        {
            get { return method; }

            set
            {
                if (value == default(String))
                { throw new PropertySetToDefaultException("Method"); }

                method = value;
            }
        }


        //INITIALIZE
        public ApiData()
        {
            api = null;

            method = null;
        }

        public ApiData(String Api, String Method)
        {
            this.Api = Api;

            this.Method = Method;
        }
    }
}