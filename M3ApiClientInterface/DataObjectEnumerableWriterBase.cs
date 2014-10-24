using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace M3ApiClientInterface
{
    public abstract class DataObjectEnumerableWriterBase<T_DataObject>
        : WriterProcessBase
    {
        //FIELDS
        protected IEnumerable<T_DataObject> dataObjectEnumerable;


        //PROPERTIES
        public virtual IEnumerable<T_DataObject> DataObjectEnumerable
        {
            get { return dataObjectEnumerable; }

            set
            {
                if (value == default(IEnumerable<T_DataObject>))
                { throw new PropertySetToDefaultException("DataObjectEnumerable"); }

                dataObjectEnumerable = value;
            }
        }


        //INITIALIZE
        public DataObjectEnumerableWriterBase()
        {
            dataObjectEnumerable = null;
        }


        //METHODS
        public override bool ExecuteProcess()
        {
            if (dataObjectEnumerable == null)
            { throw new InvalidOperationException("DataObject can not be null."); }

            return base.ExecuteProcess();
        }

        public virtual Boolean ExecuteProcess(ConnectionData ConnectionData, ApiData ApiData, IEnumerable<T_DataObject> DataObjectEnumerable, Boolean ErrorOnReturnCode8 = true)
        {
            this.ApiData = ApiData;

            this.ConnectionData = ConnectionData;

            this.DataObjectEnumerable = DataObjectEnumerable;

            this.ErrorOnReturnCode8 = ErrorOnReturnCode8;

            return ExecuteProcess();
        }


        //FUNCTIONS
        protected abstract RequestFieldDataList CreateRequestFieldDataList(T_DataObject dataObject);

        protected virtual void GetApiResultValues(T_DataObject dataObject)
        {

        }

        protected virtual Boolean ProcessApiResults(T_DataObject dataObject)
        {
            try
            {
                GetApiResultValues(dataObject);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);

                return false;
            }

            return true;
        }

        protected override bool WriteToServer()
        {
            foreach (T_DataObject dataObject in DataObjectEnumerable)
            {
                RequestFieldDataList = CreateRequestFieldDataList(dataObject);

                if (!base.WriteToServer())
                { return false; }

                if (!ProcessApiResults(dataObject))
                { return false; }
            }

            return true;
        }
    }
}