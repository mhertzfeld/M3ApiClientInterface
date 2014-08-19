using Lawson.M3.MvxSock;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


namespace M3ApiClientInterface
{
    public abstract class ReaderProcessBase
    {
        //FIELDS
        protected ApiData apiData;

        protected ConnectionData connectionData;

        protected Boolean enableZippedTransactions;

        protected Boolean errorOnReturnCode8;

        protected Int64 maximumRecordsToReturn;

        protected UInt32 maximumWaitTime;

        protected RequestFieldDataList requestFieldDataList;

        protected UInt32? returnCode;

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

        public virtual Boolean ErrorOnReturnCode8
        {
            get { return errorOnReturnCode8; }

            set { errorOnReturnCode8 = value; }
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

        public virtual UInt32 MaximumWaitTime
        {
            get { return maximumWaitTime; }

            set
            {
                if (value < 0)
                { throw new PropertySetToOutOfRangeValueException("MaximumWaitTime"); }

                maximumWaitTime = value;
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

        public virtual UInt32? ReturnCode
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

            errorOnReturnCode8 = true;

            maximumRecordsToReturn = 0;

            maximumWaitTime = 90;

            requestFieldDataList = new M3ApiClientInterface.RequestFieldDataList();

            returnCode = null;

            serverId = default(SERVER_ID);
        }


        //METHODS
        public virtual Boolean ExecuteProcess()
        {
            if (ApiData == null)
            { throw new InvalidOperationException("ApiData can not be null."); }
            
            if (ConnectionData == null)
            { throw new InvalidOperationException("ConnectionData can not be null."); }

            if (MaximumRecordsToReturn < 0)
            { throw new InvalidOperationException("MaximumRecordsToReturn is out of range."); }
            
            if (RequestFieldDataList == null)
            { throw new InvalidOperationException("RequestFieldDataList can not be null."); }

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

            if (!SetMaximumWaitTime())
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

            if (!CheckReturnCode())
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

        public virtual Boolean ExecuteProcess(ConnectionData ConnectionData, ApiData ApiData, RequestFieldDataList RequestFieldDataList, Int64 MaximumRecordsToReturn = 0, Boolean ErrorOnReturnCode8 = true)
        {
            this.ApiData = ApiData;

            this.ConnectionData = ConnectionData;

            this.MaximumRecordsToReturn = MaximumRecordsToReturn;

            this.RequestFieldDataList = RequestFieldDataList;

            return ExecuteProcess();
        }
        

        //FUNCTIONS
        protected virtual Boolean CheckReturnCode()
        {
            String errorText;

            switch (ReturnCode)
            {
                case 0:

                    return true;

                case 8:

                    Trace.WriteLine("The 'MvxSock.Access' method retured the following non zero code.  " + ReturnCode);

                    errorText = GetErrorText();

                    Trace.WriteLineIf((errorText != null), errorText);

                    return (!ErrorOnReturnCode8);

                default:

                    Trace.WriteLine("The 'MvxSock.Access' method retured the following non zero code.  " + ReturnCode);

                    errorText = GetErrorText();

                    Trace.WriteLineIf((errorText != null), errorText);

                    return false;
            }
        }

        protected virtual Boolean CloseServerConnection()
        {
            try
            {
                ReturnCode = MvxSock.Close(ref serverId);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);

                ReturnCode = null;

                return false;
            }

            if (ReturnCode != 0)
            {
                Trace.WriteLine("The 'MvxSock.Close' method retured the following non zero code.  " + ReturnCode);

                return false;
            }

            return true;
        }

        protected virtual Boolean ConnectToServer()
        {
            try
            {
                ReturnCode = MvxSock.Connect(ref serverId, ConnectionData.Server, ConnectionData.Port, ConnectionData.UserName, ConnectionData.Password, ApiData.Api, null);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);

                ReturnCode = null;

                return false;
            }

            if (ReturnCode != 0)
            {
                Trace.WriteLine("The 'MvxSock.Connect' method retured the following non zero code.  " + ReturnCode);

                String errorText = GetErrorText();

                Trace.WriteLineIf((errorText != null), errorText);

                return false;
            }

            return true;
        }

        protected virtual Boolean ExecuteApi()
        {
            try
            {
                ReturnCode = MvxSock.Access(ref serverId, ApiData.Method);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);

                ReturnCode = null;

                return false;
            }

            return true;
        }

        protected virtual String GetErrorText()
        {
            String errorText;

            try
            {
                errorText = Lawson.M3.MvxSock.MvxSock.GetLastError(ref serverId).Trim();
            }
            catch(Exception exception)
            {
                Trace.Write(exception);

                return null;
            }

            return (errorText.Length == 0 ? null : errorText);
        }

        protected virtual String GetValueFromField(String fieldName)
        {
            return MvxSock.GetField(ref serverId, fieldName);
        }

        protected abstract Boolean ProcessApiResults();

        protected virtual Boolean SetEnableZippedTransactions()
        {
            try
            {
                ReturnCode = MvxSock.SetZippedTransactions(ref serverId, 1);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);

                ReturnCode = null;

                return false;
            }

            if (ReturnCode != 0)
            {
                Trace.WriteLine("The 'MvxSock.SetZippedTransactions' method retured the following non zero code.  " + ReturnCode);

                String errorText = GetErrorText();

                Trace.WriteLineIf((errorText != null), errorText);

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
                Trace.WriteLine(exception);

                ReturnCode = null;

                return false;
            }

            if (ReturnCode != 0)
            {
                Trace.WriteLine("The 'MvxSock.Trans' method retured the following non zero code.  " + ReturnCode);

                String errorText = GetErrorText();

                Trace.WriteLineIf((errorText != null), errorText);

                return false;
            }

            return true;
        }

        protected virtual Boolean SetMaximumWaitTime()
        {
            try
            {
                ReturnCode = MvxSock.SetMaxWait(ref serverId, MaximumWaitTime);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);

                ReturnCode = null;

                return false;
            }

            if (ReturnCode != 0)
            {
                Trace.WriteLine("The 'MvxSock.SetMaxWait' method retured the following non zero code.  " + ReturnCode);

                String errorText = GetErrorText();

                Trace.WriteLineIf((errorText != null), errorText);

                return false;
            }

            return true;
        }

        protected virtual Boolean SetRequestField(RequestFieldData requestFieldData)
        {
            ReturnCode = null;

            try
            {
                MvxSock.SetField(ref serverId, requestFieldData.FieldName, requestFieldData.FieldValue);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);

                return false;
            }

            return true;
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
            if ((ApiData.Api == null) || (ApiData.Api.Length < 8))
            {
                Trace.WriteLine("'ApiData.Api' has failed validation.");

                return false; 
            }

            if ((ApiData.Method == null) || (ApiData.Method.Length == 0))
            {
                Trace.WriteLine("'ApiData.Method' has failed validation.");

                return false; 
            }

            if (ConnectionData.Password == null)
            {
                Trace.WriteLine("'ConnectionData.Password' has failed validation.");

                return false;
            }

            if (ConnectionData.Port < 1)
            {
                Trace.WriteLine("'ConnectionData.Port' has failed validation.");

                return false;
            }

            if ((ConnectionData.Server == null) || (ConnectionData.Server.Length == 0))
            {
                Trace.WriteLine("'ConnectionData.Server' has failed validation.");

                return false;
            }

            if ((ConnectionData.UserName == null) || (ConnectionData.UserName.Length == 0))
            {
                Trace.WriteLine("'ConnectionData.UserName' has failed validation.");

                return false;
            }

            return true;
        }
    }
}