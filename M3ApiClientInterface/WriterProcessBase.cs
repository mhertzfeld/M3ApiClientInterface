using Lawson.M3.MvxSock;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;


namespace M3ApiClientInterface
{
    public abstract class WriterProcessBase
    {
        //FIELDS
        protected ApiData apiData;

        protected ConnectionData connectionData;

        protected Boolean enableZippedTransactions;

        protected Boolean errorOnReturnCode8;

        protected UInt32 maximumWaitTime;

        protected List<RequestFieldData> requestFieldDataList;

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

        public virtual List<RequestFieldData> RequestFieldDataList
        {
            get { return requestFieldDataList; }

            set
            {
                if (value == default(List<RequestFieldData>))
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
        public WriterProcessBase()
        {
            apiData = null;

            connectionData = null;

            enableZippedTransactions = false;

            errorOnReturnCode8 = true;
            
            maximumWaitTime = 30000;

            requestFieldDataList = new List<RequestFieldData>();

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

            if (MaximumWaitTime <= 0)
            { throw new InvalidOperationException("MaximumWaitTime is not in range."); }

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

            if (!SetMaximumWaitTime())
            {
                CloseServerConnection();

                return false;
            }

            if (!WriteToServer())
            {
                CloseServerConnection();

                return false;
            }

            CloseServerConnection();

            return true;
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

                if (ReturnCode.GetValueOrDefault() == 0)
                { return true; }
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }

            Trace.WriteLine("The 'MvxSock.Close' method retured the following non zero code.  " + ReturnCode);

            String errorText = GetErrorText();

            Trace.WriteLineIf((errorText != null), errorText);

            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());

            return false;
        }

        protected virtual Boolean ConnectToServer()
        {
            try
            {
                ReturnCode = MvxSock.Connect(ref serverId, ConnectionData.Server, ConnectionData.Port, ConnectionData.UserName, ConnectionData.Password, ApiData.Api, null);

                if (ReturnCode.GetValueOrDefault() == 0)
                { return true; }
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }

            Trace.WriteLine("The 'MvxSock.Connect' method retured the following non zero code.  " + ReturnCode);

            String errorText = GetErrorText();

            Trace.WriteLineIf((errorText != null), errorText);

            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());
            
            return false;
        }

        protected virtual Boolean ExecuteApi()
        {
            try
            {
                ReturnCode = MvxSock.Access(ref serverId, ApiData.Method);

                return true;
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }

            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());
            
            return false;
        }

        protected virtual String GetErrorText()
        {
            String errorText;

            try
            {
                errorText = Lawson.M3.MvxSock.MvxSock.GetLastError(ref serverId).Trim();

                return (errorText.Length == 0 ? null : errorText);
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }

            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());

            return null;
        }

        protected virtual String GetValueFromField(String fieldName)
        {
            return MvxSock.GetField(ref serverId, fieldName);
        }

        protected virtual Boolean SetEnableZippedTransactions()
        {
            try
            {
                ReturnCode = MvxSock.SetZippedTransactions(ref serverId, 1);

                if (ReturnCode.GetValueOrDefault() == 0)
                { return true; }
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }

            Trace.WriteLine("The 'MvxSock.SetZippedTransactions' method retured the following non zero code.  " + ReturnCode);

            String errorText = GetErrorText();

            Trace.WriteLineIf((errorText != null), errorText);

            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());

            return false;
        }

        protected virtual Boolean SetMaximumWaitTime()
        {
            try
            {
                ReturnCode = MvxSock.SetMaxWait(ref serverId, MaximumWaitTime);

                if (ReturnCode.GetValueOrDefault() == 0)
                { return true; }
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }

            Trace.WriteLine("The 'MvxSock.SetMaxWait' method retured the following non zero code.  " + ReturnCode);

            String errorText = GetErrorText();

            Trace.WriteLineIf((errorText != null), errorText);

            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());

            return false;
        }

        protected virtual Boolean SetRequestField(RequestFieldData requestFieldData)
        {
            try
            {
                ReturnCode = null;

                MvxSock.SetField(ref serverId, requestFieldData.FieldName, requestFieldData.FieldValue);

                return true;
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }

            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());

            return false;
        }

        protected virtual Boolean SetRequestFields()
        {
            try
            {
                foreach (RequestFieldData requestField in RequestFieldDataList)
                {
                    if (!SetRequestField(requestField))
                    { return false; }
                }

                return true;
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }

            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());

            return false;
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

        protected virtual Boolean WriteToServer()
        {
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

            return true;
        }
    }
}