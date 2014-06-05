using Lawson.M3.MvxSock;
using MyClassLibrary.Logging;
using System;
using System.Collections.Generic;


namespace M3ApiClientInterface
{
    public abstract class DataObjectDictionaryReaderProcessBase<T_Key, T_DataObject, T_DataObjectDictionary, T_LogWriter>
        : ReaderProcessBase<T_LogWriter>
        where T_DataObjectDictionary : System.Collections.Generic.IDictionary<T_Key, T_DataObject>, new()
        where T_LogWriter : MyClassLibrary.Logging.ILogWriter, new ()
    {
        //FIELDS
        protected T_DataObjectDictionary dataObjectDictionary;


        //PROPERTIES
        public virtual T_DataObjectDictionary DataObjectDictionary
        {
            get { return dataObjectDictionary; }

            protected set { dataObjectDictionary = value; }
        }


        //INITIALIZE
        public DataObjectDictionaryReaderProcessBase()
        {
            dataObjectDictionary = default(T_DataObjectDictionary);
        }


        //FUNCTIONS
        protected abstract void AddDataObjectToDataObjectDictionary(T_DataObject dataObject);

        protected abstract T_DataObject CreateDataObject();

        protected virtual bool ProcessApiResult()
        {
            T_DataObject dataObject = CreateDataObject();

            AddDataObjectToDataObjectDictionary(dataObject);

            return true;
        }

        protected override bool ProcessApiResults()
        {
            DataObjectDictionary = new T_DataObjectDictionary();

            try
            {
                while (MvxSock.More(ref serverId))
                {
                    if (!ProcessApiResult())
                    { return false; }

                    ReturnCode = MvxSock.Access(ref serverId, null);

                    if (ReturnCode != 0)
                    { return false; }
                }
            }
            catch (Exception exception)
            {
                LoggingUtilities.WriteLogEntry<T_LogWriter>(exception);

                return false;
            }

            return true;
        }

        protected override void ResetProcess()
        {
            base.ResetProcess();

            DataObjectDictionary = default(T_DataObjectDictionary);
        }
    }
}