using System;
using System.Collections.Generic;


namespace M3ApiClientInterface
{
    public abstract class DataObjectEnumerableWriterBase<T_DataObject>
        : WriterProcessBase
    {
        //FIELDS
        protected T_DataObject dataObject;

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
            dataObject = default(T_DataObject);

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
        protected abstract RequestFieldDataList CreateRequestFieldDataList();

        protected override bool WriteToServer()
        {
            foreach (T_DataObject iterativeDataObject in DataObjectEnumerable)
            {
                dataObject = iterativeDataObject;

                RequestFieldDataList = CreateRequestFieldDataList();

                if (!base.WriteToServer())
                { return false; }
            }

            return true;
        }
    }
}