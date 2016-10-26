using Lawson.M3.MvxSock;
using System;
using System.Diagnostics;


namespace M3ApiClientInterface
{
    public abstract class DataObjectCollectionReaderProcessBase<T_DataObject, T_DataObjectCollection>
        : ReaderProcessBase
        where T_DataObjectCollection : System.Collections.Generic.ICollection<T_DataObject>, new()
    {
        //FIELDS
        protected T_DataObjectCollection dataObjectCollection;


        //PROPERTIES
        public virtual T_DataObjectCollection DataObjectCollection
        {
            get { return dataObjectCollection; }

            protected set { dataObjectCollection = value; }
        }


        //INITIALIZE
        public DataObjectCollectionReaderProcessBase()
        {
            dataObjectCollection = default(T_DataObjectCollection);
        }


        //METHODS
        protected override bool ExecuteApi()
        {
            dataObjectCollection = default(T_DataObjectCollection);

            return base.ExecuteApi();
        }


        //FUNCTIONS
        protected virtual void AddDataObjectToDataObjectCollection(T_DataObject dataObject)
        {
            DataObjectCollection.Add(dataObject);
        }

        protected abstract T_DataObject CreateDataObject();

        protected virtual bool ProcessApiResult()
        {
            try
            {
                T_DataObject dataObject = CreateDataObject();

                AddDataObjectToDataObjectCollection(dataObject);

                return true;
            }
            catch (Exception exception)
            { Trace.WriteLine(exception); }

            return false;
        }

        protected override bool ProcessApiResults()
        {
            DataObjectCollection = new T_DataObjectCollection();

            try
            {
                while (MvxSock.More(ref serverId))
                {
                    if (!ProcessApiResult())
                    { return false; }

                    ReturnCode = MvxSock.Access(ref serverId, null);

                    if (ReturnCode != 0)
                    {
                        Trace.WriteLine("The 'MvxSock.Access' method retured the following non zero code.  " + ReturnCode);

                        String errorText = GetErrorText();

                        Trace.WriteLineIf((errorText != null), errorText);

                        return false;
                    }
                }

                return true;
            }
            catch (Exception exception)
            { Trace.WriteLine(exception); }

            return false;
        }
    }
}