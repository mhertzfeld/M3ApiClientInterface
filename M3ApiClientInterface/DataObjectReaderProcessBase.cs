using Lawson.M3.MvxSock;
using MyClassLibrary.Logging;
using System;


namespace M3ApiClientInterface
{
    public abstract class DataObjectReaderProcessBase<T_DataObject, T_LogWriter>
        : ReaderProcessBase<T_LogWriter>
        where T_LogWriter : MyClassLibrary.Logging.ILogWriter, new()
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
            }
            catch (Exception exception)
            {
                LoggingUtilities.WriteLogEntry<T_LogWriter>(exception);

                return false;
            }

            return true;
        }
    }
}