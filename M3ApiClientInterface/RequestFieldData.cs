using MyClassLibrary;
using System;


namespace M3ApiClientInterface
{
    public class RequestFieldData
    {
        //FIELDS
        protected String fieldName;

        protected String fieldValue;


        //PROPERTIES
        public virtual String FieldName
        {
            get { return fieldName; }

            set
            {
                if (value == default(String))
                { throw new PropertySetToDefaultException("FieldName"); }

                fieldName = value;
            }
        }

        public virtual String FieldValue
        {
            get { return fieldValue; }

            set { fieldValue = value; }
        }


        //INITIALIZE
        public RequestFieldData()
        {
            fieldName = null;

            fieldValue = null;
        }

        public RequestFieldData(String FieldName, String FieldValue)
        {
            this.FieldName = FieldName;

            this.FieldValue = FieldValue;
        }
    }
}