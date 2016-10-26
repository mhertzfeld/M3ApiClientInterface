using Lawson.M3.MvxSock;
using System;
using System.Diagnostics;


namespace M3ApiClientInterface
{
    public abstract class DataObjectReaderProcessBase<T_DataObject>
        : ReaderProcessBase
    {
        //FIELDS
        protected T_DataObject dataObject;


        //PROPERTIES
        public virtual T_DataObject DataObject
        {
            get { return dataObject; }

            protected set { dataObject = value; }
        }


        //INITIALIZE
        public DataObjectReaderProcessBase()
        {
            dataObject = default(T_DataObject);
        }


        //FUNCTIONS
        protected abstract T_DataObject CreateDataObject();

        protected override bool ProcessApiResults()
        {
            try
            {
                DataObject = CreateDataObject();

                return true;
            }
            catch (Exception exception)
            { Trace.WriteLine(exception); }

            return false;
        }
    }
}