using Lawson.M3.MvxSock;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;


namespace M3ApiClientInterface
{
    public class WriterProcess
    {
        //FIELDS
        protected ApiData apiData;

        protected ConnectionData connectionData;
        
        protected UInt32 maximumWaitTime;
        
        protected List<RequestFieldData> _InputFieldDataList;

        protected List<OutputFieldData> _OutputFieldData;
        
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
        
        public virtual List<RequestFieldData> InputFieldDataList
        {
            get { return _InputFieldDataList; }

            set
            {
                if (value == default(List<RequestFieldData>))
                { throw new PropertySetToDefaultException("InputFieldDataList"); }

                _InputFieldDataList = value;
            }
        }

        public virtual List<OutputFieldData> OutputFieldData
        {
            get { return _OutputFieldData; }

            set
            {
                if (value == default(List<OutputFieldData>))
                { throw new PropertySetToDefaultException("OutputFieldData"); }

                _OutputFieldData = value;
            }
        }

        public virtual UInt32? ReturnCode
        {
            get { return returnCode; }

            protected set { returnCode = value; }
        }

        
        //INITIALIZE
        public WriterProcess()
        {
            apiData = new ApiData();

            connectionData = new ConnectionData();
            
            maximumWaitTime = 30000;
            
            _InputFieldDataList = new List<RequestFieldData>();

            _OutputFieldData = new List<OutputFieldData>();
            
            returnCode = null;
            
            serverId = default(SERVER_ID);
        }


        //METHODS
        public virtual Boolean ExecuteProcess()
        {
            try
            {
                if (ApiData == null)
                { throw new InvalidOperationException("'ApiData' cannot be null."); }

                if (ApiData.Api == null)
                { throw new InvalidOperationException("'ApiData.Api' cannot be null"); }

                if (ConnectionData == null)
                { throw new InvalidOperationException("'ConnectionData' cannot be null."); }

                if (ConnectionData.Password == null)
                { throw new InvalidOperationException("'ConnectionData.Password' cannot be null."); }

                if (ConnectionData.Port == 0)
                { throw new InvalidOperationException("'ConnectionData.Port' cannot be 0."); }

                if (ConnectionData.Server == null)
                { throw new InvalidOperationException("'ConnectionData.Server' cannot be null."); }

                if (ConnectionData.UserName == null)
                { throw new InvalidOperationException("'ConnectionData.UserName' cannot be null."); }

                if (InputFieldDataList == null)
                { throw new InvalidOperationException("RequestFieldDataList cannot be null."); }

                if (InputFieldDataList.Count == 0)
                { throw new InvalidOperationException("RequestFieldDataList.Count cannot be 0."); }
                
                serverId = new SERVER_ID();
                
                if (!ConnectToServer())
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
                
                if (!GetOutputFieldData())
                {
                    CloseServerConnection();

                    return false;
                }

                CloseServerConnection();

                return true;
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
                { Trace.WriteLine(String.Format("The 'MvxSock.Close' method retured the following non zero code.  {0}", ReturnCode)); }
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }

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

                if (ReturnCode.Value == 0)
                { return true; }
                else
                { Trace.WriteLine(String.Format("The 'MvxSock.Connect' method retured the following non zero code.  {0}", ReturnCode)); }
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }
            
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

                if (ReturnCode.Value == 0)
                { return true; }
                else
                { Trace.WriteLine(String.Format("The 'MvxSock.Access' method retured the following non zero code.  {0}", ReturnCode)); }
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
                errorText = MvxSock.GetLastError(ref serverId).Trim();

                return (errorText.Length == 0 ? null : errorText);
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }

            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());

            return null;
        }

        protected virtual Boolean GetOutputFieldData()
        {
            try
            {
                foreach (OutputFieldData _OutputFieldData in OutputFieldData)
                {
                    _OutputFieldData.Value = GetValueFromField(_OutputFieldData.FieldName);
                }

                return true;
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }

            String errorText = GetErrorText();

            Trace.WriteLineIf((errorText != null), errorText);

            TraceUtilities.WriteMethodError(MethodBase.GetCurrentMethod());

            return false;
        }

        protected virtual String GetValueFromField(String fieldName)
        {
            return MvxSock.GetField(ref serverId, fieldName);
        }
        
        protected virtual Boolean SetMaximumWaitTime()
        {
            try
            {
                ReturnCode = MvxSock.SetMaxWait(ref serverId, MaximumWaitTime);

                if (ReturnCode.Value == 0)
                { return true; }
                else
                { Trace.WriteLine(String.Format("The 'MvxSock.SetMaxWait' method retured the following non zero code.  {0}", ReturnCode)); }
            }
            catch (Exception exception)
            { Trace.WriteLine(exception.ToString()); }
            
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
                foreach (RequestFieldData requestField in InputFieldDataList)
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
    }
}