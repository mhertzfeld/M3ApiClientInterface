using Lawson.M3.MvxSock;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;


namespace M3ApiClientInterface
{
    public abstract class ReaderProcessBase
        : ReaderProcessInterface
    {
        //FIELDS
        protected ApiData apiData;

        protected ConnectionData connectionData;

        protected Boolean enableZippedTransactions;

        protected Boolean errorOnReturnCode8;

        protected UInt16 executionsAttempted;

        protected UInt16 executionsToAttempt;

        protected UInt64 maximumRecordsToReturn;

        protected UInt32 maximumTimeToWaitBetweenRetries;

        protected UInt32 maximumWaitTime;

        protected UInt32 minimumTimeToWaitBetweenRetries; 

        protected Random random;

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

        public virtual UInt16 ExecutionsAttempted
        {
            get { return executionsAttempted; }

            protected set { executionsAttempted = value; }
        }

        public virtual UInt16 ExecutionsToAttempt
        {
            get { return executionsToAttempt; }

            set { executionsToAttempt = value; }
        }

        public virtual UInt64 MaximumRecordsToReturn
        {
            get { return maximumRecordsToReturn; }

            set { maximumRecordsToReturn = value; }
        }

        public virtual UInt32 MaximumTimeToWaitBetweenRetries
        {
            get { return maximumTimeToWaitBetweenRetries; }

            set
            {
                if (value <= 500)
                { throw new PropertySetToOutOfRangeValueException("MaximumTimeToWaitBetweenRetries cannot be set to less than 500."); }

                maximumTimeToWaitBetweenRetries = value;
            }
        }

        public virtual UInt32 MaximumWaitTime
        {
            get { return maximumWaitTime; }

            set { maximumWaitTime = value; }
        }

        public virtual UInt32 MinimumTimeToWaitBetweenRetries
        {
            get { return minimumTimeToWaitBetweenRetries; }

            set
            {
                if (value < 250)
                { throw new PropertySetToOutOfRangeValueException("MinimumTimeToWaitBetweenRetries cannot be set to less than 250."); }

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

        public virtual UInt32? ReturnCode
        {
            get { return returnCode; }

            protected set { returnCode = value; }
        }
        

        //INITIALIZE
        public ReaderProcessBase()
        {
            apiData = new ApiData();

            connectionData = new ConnectionData();

            enableZippedTransactions = false;

            errorOnReturnCode8 = false;

            executionsAttempted = 0;

            maximumRecordsToReturn = 0;

            maximumTimeToWaitBetweenRetries = 30000;

            maximumWaitTime = 30000;

            minimumTimeToWaitBetweenRetries = 5000;

            random = new Random();

            requestFieldDataList = new List<RequestFieldData>();

            executionsToAttempt = 3;

            returnCode = null;

            serverId = default(SERVER_ID);
        }


        //METHODS
        public virtual Boolean ExecuteProcess()
        {
            try
            {
                Boolean returnState = false;

                while ((ExecutionsAttempted <= ExecutionsToAttempt) && (!returnState))
                {
                    if (ExecutionsAttempted > 1)
                    {
                        System.Threading.Thread.Sleep(random.Next((Int32)MinimumTimeToWaitBetweenRetries, (Int32)MaximumTimeToWaitBetweenRetries));

                        Trace.WriteLine(String.Format("Attempting to retry the API call.  Execution Attempt:{0}", ExecutionsAttempted));
                    }
                    
                    if (ApiData == null)
                    { throw new InvalidOperationException("ApiData can not be null."); }
                    
                    if (ConnectionData == null)
                    { throw new InvalidOperationException("ConnectionData can not be null."); }

                    if (RequestFieldDataList == null)
                    { throw new InvalidOperationException("RequestFieldDataList can not be null."); }

                    if (!ValidateInputs())
                    { return false; }
                    
                    returnCode = null;

                    serverId = new SERVER_ID();

                    ExecutionsAttempted++;

                    if (!ConnectToServer())
                    {
                        CloseServerConnection();

                        continue;
                    }

                    if (EnableZippedTransactions)
                    {
                        if (!SetEnableZippedTransactions())
                        {
                            CloseServerConnection();

                            continue;
                        }
                    }

                    if (!SetMaximumRecordsForApiToReturn())
                    {
                        CloseServerConnection();

                        continue;
                    }

                    if (!SetMaximumWaitTime())
                    {
                        CloseServerConnection();

                        continue;
                    }

                    if (!SetRequestFields())
                    {
                        CloseServerConnection();

                        continue;
                    }

                    if (!ExecuteApi())
                    {
                        CloseServerConnection();

                        continue;
                    }

                    if (ReturnCode.Value == 0)
                    {
                        if (!ProcessApiResults())
                        {
                            CloseServerConnection();

                            continue;
                        }
                    }

                    CloseServerConnection();

                    returnState = true;
                }

                return returnState;
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }

            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());

            return false;
        }
        

        //FUNCTIONS
        protected virtual Boolean CloseServerConnection()
        {
            try
            {
                ReturnCode = MvxSock.Close(ref serverId);

                if (ReturnCode.Value == 0)
                { return true; }
                else
                { Trace.WriteLine(String.Format("The 'MvxSock.Close' method retured non zero code:{0}", ReturnCode)); }
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }

            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());

            ReturnCode = null;

            return false;
        }

        protected virtual Boolean ConnectToServer()
        {
            try
            {
                ReturnCode = MvxSock.Connect(ref serverId, ConnectionData.Server, ConnectionData.Port, ConnectionData.UserName, ConnectionData.Password, ApiData.Api, null);

                if (ReturnCode.Value == 0)
                { return true; }
                else
                {
                    Trace.WriteLine(String.Format("The 'MvxSock.Connect' method retured non zero code:{0}", ReturnCode));

                    String errorText = GetErrorText();

                    Trace.WriteLineIf((errorText != null), errorText);
                }
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }
            
            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());

            return false;
        }
        
        protected virtual Boolean ExecuteApi()
        {
            try
            {
                ReturnCode = MvxSock.Access(ref serverId, ApiData.Method);

                return EvaluateMvxSockAccessReturnCode();
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }

            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());

            ReturnCode = null;

            return false;
        }

        protected virtual Boolean EvaluateMvxSockAccessReturnCode()
        {
            switch (ReturnCode.Value)
            {
                case 0:

                    return true;

                case 8:

                    if (ErrorOnReturnCode8)
                    {
                        Trace.WriteLine(String.Format("The 'MvxSock.Access' method retured non zero code:{0}.", ReturnCode));

                        String returnCode8ErrorText = GetErrorText();

                        Trace.WriteLineIf((returnCode8ErrorText != null), returnCode8ErrorText);

                        return false;
                    }
                    else
                    {
                        return true;
                    }

                default:

                    Trace.WriteLine(String.Format("The 'MvxSock.Access' method retured non zero code:{0}.", ReturnCode));

                    String errorText = GetErrorText();

                    Trace.WriteLineIf((errorText != null), errorText);

                    return false;
            }
        }

        protected virtual String GetErrorText()
        {
            String errorText;

            try
            {
                errorText = MvxSock.GetLastError(ref serverId).Trim();

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

        protected abstract Boolean ProcessApiResults();
        
        protected virtual Boolean SetEnableZippedTransactions()
        {
            try
            {
                ReturnCode = MvxSock.SetZippedTransactions(ref serverId, 1);

                if (ReturnCode.Value == 0)
                { return true; }
                else
                {
                    Trace.WriteLine(String.Format("The 'MvxSock.SetZippedTransactions' method retured non zero code:{0}.", ReturnCode));

                    String errorText = GetErrorText();

                    Trace.WriteLineIf((errorText != null), errorText);
                }
            }
            catch (Exception exception)
            { Trace.WriteLine(exception); }

            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());

            return false;
        }

        protected virtual Boolean SetMaximumRecordsForApiToReturn()
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();

                UInt32 size = 128;
                
                ReturnCode = MvxSock.Trans(ref serverId, String.Format("SetLstMaxRec   {0}", MaximumRecordsToReturn.ToString()), stringBuilder, ref size);

                if (ReturnCode.Value == 0)
                { return true; }
                else
                {
                    Trace.WriteLine(String.Format("The 'MvxSock.Trans' method retured non zero code:{0}.", ReturnCode));

                    String errorText = GetErrorText();

                    Trace.WriteLineIf((errorText != null), errorText);
                }
            }
            catch (Exception exception)
            { Trace.WriteLine(exception); }

            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());

            return false;
        }

        protected virtual Boolean SetMaximumWaitTime()
        {
            try
            {
                ReturnCode = MvxSock.SetMaxWait(ref serverId, MaximumWaitTime);

                if (ReturnCode.Value == 0)
                { return true; }
                else
                {
                    Trace.WriteLine(String.Format("The 'MvxSock.SetMaxWait' method retured non zero code:{0}.", ReturnCode));

                    String errorText = GetErrorText();

                    Trace.WriteLineIf((errorText != null), errorText);
                }
            }
            catch (Exception exception)
            { Trace.WriteLine(exception); }

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
            { Trace.WriteLine(exception); }

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
            if ((ApiData.Api == null) || (ApiData.Method == null))
            {
                Trace.WriteLine("'ApiData' Property has failed validation.");

                return false; 
            }

            if ((ConnectionData.Password == null) || (ConnectionData.Port == 0) || (ConnectionData.Server == null) || (ConnectionData.UserName == null))
            {
                Trace.WriteLine("'ConnectionData' Property has failed validation.");

                return false;
            }
            
            foreach (RequestFieldData _RequestFieldData in RequestFieldDataList)
            {
                if ((_RequestFieldData.FieldName == null) || (_RequestFieldData.FieldValue == null))
                {
                    Trace.WriteLine("'RequestFieldDataList' Property has failed validation.");

                    return false;
                }
            }

            return true;
        }
    }
}