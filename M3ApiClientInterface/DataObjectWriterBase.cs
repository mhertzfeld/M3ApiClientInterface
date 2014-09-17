using System;


namespace M3ApiClientInterface
{
    public abstract class DataObjectWriterBase<T_DataObject>
        : WriterProcessBase
    {
        //FIELDS
        protected T_DataObject dataObject;


        //PROPERTIES
        public virtual T_DataObject DataObject
        {
            get { return dataObject; }

            set
            {
                if (value == null)
                { throw new PropertySetToDefaultException("DataObject"); }

                dataObject = value;
            }
        }


        //INITIALIZE
        public DataObjectWriterBase()
        {
            dataObject = default(T_DataObject);
        }


        //METHODS
        public override bool ExecuteProcess()
        {
            if (dataObject == null)
            { throw new InvalidOperationException("DataObject can not be null."); }

            return base.ExecuteProcess();
        }

        public virtual Boolean ExecuteProcess(ConnectionData ConnectionData, ApiData ApiData, T_DataObject DataObject, Boolean ErrorOnReturnCode8 = true)
        {
            this.ApiData = ApiData;

            this.ConnectionData = ConnectionData;

            this.DataObject = DataObject;

            this.ErrorOnReturnCode8 = ErrorOnReturnCode8;

            return ExecuteProcess();
        }


        //FUNCTIONS
        protected abstract RequestFieldDataList CreateRequestFieldDataList();

        protected override bool WriteToServer()
        {
            RequestFieldDataList = CreateRequestFieldDataList();

            return base.WriteToServer();
        }
    }
}