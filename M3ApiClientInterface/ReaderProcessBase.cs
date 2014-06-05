using Lawson.M3.MvxSock;
using MyClassLibrary;
using MyClassLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Text;


namespace M3ApiClientInterface
{
    public abstract class ReaderProcessBase<T_LogWriter>
        : MyClassLibrary.Process.ProcessWorkerBase
        where T_LogWriter : MyClassLibrary.Logging.ILogWriter, new()
    {
        //FIELDS
        protected ApiData apiData;

        protected ConnectionData connectionData;

        protected Boolean enableZippedTransactions;

        protected Int64 maximumRecordsToReturn;

        protected RequestFieldDataList requestFieldDataList;

        protected UInt32 returnCode;

        protected SERVER_ID serverId;


        //PROPERTIES
        public virtual ApiData ApiData
        {
            get { return apiData; }

            set
            {
                if (value == default(ApiData))
                { throw new PropertySetToDefaultException("ApiData"); }

                apiData = value;
            }
        }

        public virtual ConnectionData ConnectionData
        {
            get { return connectionData; }

            set
            {
                if (value == default(ConnectionData))
                { throw new PropertySetToDefaultException("ConnectionData"); }

                connectionData = value;
            }
        }

        public virtual Boolean EnableZippedTransactions
        {
            get { return enableZippedTransactions; }

            set { enableZippedTransactions = value; }
        }

        public virtual Int64 MaximumRecordsToReturn
        {
            get { return maximumRecordsToReturn; }

            set
            {
                if (value < 0)
                { throw new PropertySetToOutOfRangeValueException("MaximumRecordsToReturn"); }

                maximumRecordsToReturn = value;
            }
        }

        public virtual RequestFieldDataList RequestFieldDataList
        {
            get { return requestFieldDataList; }

            set
            {
                if (value == default(RequestFieldDataList))
                { throw new PropertySetToDefaultException("RequestFieldDataList"); }

                requestFieldDataList = value;
            }
        }

        public virtual UInt32 ReturnCode
        {
            get { return returnCode; }

            protected set { returnCode = value; }
        }


        //INITIALIZE
        public ReaderProcessBase()
        {
            apiData = null;

            connectionData = null;

            enableZippedTransactions = true;

            maximumRecordsToReturn = 0;

            requestFieldDataList = null;

            returnCode = 0;

            serverId = default(SERVER_ID);
        }


        //METHODS
        public override bool ProcessExecution()
        {
            if (ApiData == null)
            { throw new NullReferenceException("ApiData"); }

            if (ConnectionData == null)
            { throw new NullReferenceException("ConnectionData"); }

            if (MaximumRecordsToReturn < 0)
            { throw new ValueOutOfRangeException("MaximumRecordsToReturn"); }
            
            if (RequestFieldDataList == null)
            { throw new NullReferenceException("RequestFieldDataList"); }

            if (!ValidateInputs())
            { return false; }

            serverId = new SERVER_ID();

            if (!ConnectToServer())
            { return false; }

            if (EnableZippedTransactions)
            {
                if (!SetEnableZippedTransactions())
                {
                    CloseServerConnection();

                    return false;
                }
            }

            if (!SetMaximumRecordsForApiToReturn())
            {
                CloseServerConnection();

                return false;
            }

            if (!SetRequestFields())
            {
                CloseServerConnection();

                return false;
            }

            if (!ExecuteApi())
            {
                CloseServerConnection();

                return false;
            }

            if (!ProcessApiResults())
            {
                CloseServerConnection();

                return false;
            }

            CloseServerConnection();

            return true;
        }

        public virtual Boolean ProcessExecution(ConnectionData ConnectionData, ApiData ApiData, RequestFieldDataList RequestFieldDataList, Int64 MaximumRecordsToReturn = 0)
        {
            this.ApiData = ApiData;

            this.ConnectionData = ConnectionData;

            this.MaximumRecordsToReturn = MaximumRecordsToReturn;

            this.RequestFieldDataList = RequestFieldDataList;

            return ProcessExecution();
        }

        public virtual void RunWorker(ConnectionData ConnectionData, ApiData ApiData, RequestFieldDataList RequestFieldDataList, Int64 MaximumRecordsToReturn = 0)
        {
            this.ApiData = ApiData;

            this.ConnectionData = ConnectionData;

            this.MaximumRecordsToReturn = MaximumRecordsToReturn;

            this.RequestFieldDataList = RequestFieldDataList;

            RunWorker();
        }
        

        //FUNCTIONS
        protected virtual Boolean CloseServerConnection()
        {
            try
            {
                MvxSock.Close(ref serverId);
            }
            catch (Exception exception)
            {
                LoggingUtilities.WriteLogEntry<T_LogWriter>(exception);

                ReturnCode = 0;

                return false;
            }

            return (ReturnCode == 0);
        }

        protected virtual Boolean ConnectToServer()
        {
            try
            {
                ReturnCode = MvxSock.Connect(ref serverId, ConnectionData.Server, ConnectionData.Port, ConnectionData.UserName, ConnectionData.Password, ApiData.Api, null);
            }
            catch (Exception exception)
            {
                LoggingUtilities.WriteLogEntry<T_LogWriter>(exception);

                ReturnCode = 0;

                return false;
            }

            return (ReturnCode == 0);
        }

        protected virtual Boolean ExecuteApi()
        {
            try
            {
                ReturnCode = MvxSock.Access(ref serverId, ApiData.Method);
            }
            catch (Exception exception)
            {
                LoggingUtilities.WriteLogEntry<T_LogWriter>(exception);

                ReturnCode = 0;

                return false;
            }

            return (ReturnCode == 0);
        }

        protected virtual String GetValueFromField(String fieldName)
        {
            return MvxSock.GetField(ref serverId, fieldName);
        }

        protected abstract Boolean ProcessApiResults();

        protected override void ResetProcess()
        {
            base.ResetProcess();

            serverId = default(SERVER_ID);
        }

        protected virtual Boolean SetEnableZippedTransactions()
        {
            try
            {
                ReturnCode = MvxSock.SetZippedTransactions(ref serverId, 1);
            }
            catch (Exception exception)
            {
                LoggingUtilities.WriteLogEntry<T_LogWriter>(exception);

                ReturnCode = 0;

                return false;
            }

            return true;
        }

        protected virtual Boolean SetMaximumRecordsForApiToReturn()
        {
            StringBuilder stringBuilder = new StringBuilder();

            UInt32 test = 128;

            try
            {
                ReturnCode = MvxSock.Trans(ref serverId, "SetLstMaxRec   0", stringBuilder, ref test);
            }
            catch (Exception exception)
            {
                LoggingUtilities.WriteLogEntry<T_LogWriter>(exception);

                ReturnCode = 0;

                return false;
            }

            return (ReturnCode == 0);
        }

        protected virtual Boolean SetRequestField(RequestFieldData requestFieldData)
        {
            try
            {
                MvxSock.SetField(ref serverId, requestFieldData.FieldName, requestFieldData.FieldValue);
            }
            catch (Exception exception)
            {
                LoggingUtilities.WriteLogEntry<T_LogWriter>(exception);

                ReturnCode = 0;

                return false;
            }

            return (ReturnCode == 0);
        }

        protected virtual Boolean SetRequestFields()
        {
            foreach (RequestFieldData requestField in RequestFieldDataList)
            {
                if (!SetRequestField(requestField))
                { return false; }
            }

            return true;
        }

        protected virtual Boolean ValidateInputs()
        {
            if ((ApiData.Api == null) || (ApiData.Api.Length != 8))
            { return false; }

            if ((ApiData.Method == null) || (ApiData.Method.Length == 0))
            { return false; }

            if (ConnectionData.Password == null)
            { return false; }

            if (ConnectionData.Port < 1)
            { return false; }

            if ((ConnectionData.Server == null) || (ConnectionData.Server.Length == 0))
            { return false; }

            if ((ConnectionData.UserName == null) || (ConnectionData.UserName.Length == 0))
            { return false; }

            return true;
        }
    }
}