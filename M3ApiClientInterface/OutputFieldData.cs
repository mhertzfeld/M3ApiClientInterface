using System;


namespace M3ApiClientInterface
{
    public class OutputFieldData
    {
        //FIELDS
        protected String _FieldName;

        protected String _Value;


        //PROPERTIES
        public virtual String FieldName
        {
            get { return _FieldName; }

            set
            {
                if (value == default(String))
                { throw new PropertySetToDefaultException("FieldName"); }

                _FieldName = value;
            }
        }

        public virtual String Value
        {
            get { return _Value; }

            set { _Value = value; }
        }


        //INITIALIZE
        public OutputFieldData()
        {
            _FieldName = null;

            _Value = null;
        }


        //INITIALIZE
        public OutputFieldData(String FieldName)
        {
            this.FieldName = FieldName;

            _Value = null;
        }
    }
}