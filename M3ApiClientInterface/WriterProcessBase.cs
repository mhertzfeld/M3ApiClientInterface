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
        
        protected Int32 executionAttempts;

        protected Int32 maximumTimeToWaitBetweenRetries;

        protected UInt32 maximumWaitTime;

        protected Int32 minimumTimeToWaitBetweenRetries;

        protected Random random;

        protected List<RequestFieldData> requestFieldDataList;

        protected Int32 retries;

        protected Boolean retryOnErrorCode8;

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
        
        public virtual Boolean EnableZippedTransactions
        {
            get { return enableZippedTransactions; }

            set { enableZippedTransactions = value; }
        }
        
        public virtual Int32 ExecutionAttempts
        {
            get { return executionAttempts; }

            protected set
            {
                if (value < 1)
                { throw new PropertySetToOutOfRangeValueException("ExecutionAttempts"); }

                executionAttempts = value;
            }
        }

        public virtual Int32 MaximumTimeToWaitBetweenRetries
        {
            get { return maximumTimeToWaitBetweenRetries; }

            set
            {
                if (value <= 1000)
                { throw new PropertySetToOutOfRangeValueException("MaximumTimeToWaitBetweenRetries"); }

                maximumTimeToWaitBetweenRetries = value;
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

        public virtual Int32 MinimumTimeToWaitBetweenRetries
        {
            get { return minimumTimeToWaitBetweenRetries; }

            set
            {
                if (value < 500)
                { throw new PropertySetToOutOfRangeValueException("MinimumTimeToWaitBetweenRetries"); }

                minimumTimeToWaitBetweenRetries = value;
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

        public virtual Int32 Retries
        {
            get { return retries; }

            set
            {
                if (value < 0)
                { throw new PropertySetToOutOfRangeValueException("Retries"); }

                retries = value;
            }
        }

        public virtual Boolean RetryOnErrorCode8
        {
            get { return retryOnErrorCode8; }

            set { retryOnErrorCode8 = value; }
        }

        public virtual UInt32? ReturnCode
        {
            get { return returnCode; }

            protected set { returnCode = value; }
        }

        
        //INITIALIZE
        public WriterProcessBase()
        {
            apiData = new ApiData();

            connectionData = new ConnectionData();

            enableZippedTransactions = false;
            
            executionAttempts = 0;
            
            maximumTimeToWaitBetweenRetries = 30000;

            maximumWaitTime = 30000;

            minimumTimeToWaitBetweenRetries = 5000;

            random = null;

            requestFieldDataList = new List<RequestFieldData>();

            retries = 5;

            retryOnErrorCode8 = false;

            returnCode = null;
            
            serverId = default(SERVER_ID);
        }


        //METHODS
        public virtual Boolean ExecuteProcess()
        {
            try
            {
                SetApiData();

                SetConnectionData();

                SetRequestFieldData();

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

                random = new Random();

                serverId = new SERVER_ID();

                ExecutionAttempts++;

                if (!ConnectToServer())
                {
                    CloseServerConnection();

                    return Retry();
                }

                if (EnableZippedTransactions)
                {
                    if (!SetEnableZippedTransactions())
                    {
                        CloseServerConnection();

                        return Retry();
                    }
                }

                if (!SetMaximumWaitTime())
                {
                    CloseServerConnection();

                    return Retry();
                }

                if (WriteToServer())
                {
                    CloseServerConnection();

                    return true;
                }
                else
                {
                    if ((ReturnCode.Value == 8) && (!RetryOnErrorCode8))
                    {
                        CloseServerConnection();

                        return false;
                    }

                    CloseServerConnection();

                    return Retry();
                }            
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }

            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());

            return false;
        }
        

        //FUNCTIONS
        protected virtual Boolean CheckReturnCode()
        {
            if (ReturnCode.Value == 0)
            {
                return true;
            }
            else
            {
                Trace.WriteLine("The 'MvxSock.Access' method retured the following non zero code.  " + ReturnCode);

                String errorText = GetErrorText();

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

        protected virtual Boolean Retry()
        {
            try
            {
                if ((ExecutionAttempts -1) <= Retries)
                {
                    System.Threading.Thread.Sleep(random.Next(MinimumTimeToWaitBetweenRetries, MaximumTimeToWaitBetweenRetries));

                    Trace.WriteLine("Attempting to retry the API call.  " + ExecutionAttempts);

                    return ExecuteProcess();
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            { Trace.WriteLine(exception); }

            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());

            return false;
        }

        protected abstract void SetApiData();

        protected abstract void SetConnectionData();

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

        protected abstract void SetRequestFieldData();

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
            { return false; }

            if (!ExecuteApi())
            { return false; }

            if (!CheckReturnCode())
            { return false; }

            return true;
        }
    }
}