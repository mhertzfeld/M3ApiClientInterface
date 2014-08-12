﻿using Lawson.M3.MvxSock;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace M3ApiClientInterface
{
    public abstract class DataObjectDictionaryReaderProcessBase<T_Key, T_DataObject, T_DataObjectDictionary>
        : ReaderProcessBase
        where T_DataObjectDictionary : System.Collections.Generic.IDictionary<T_Key, T_DataObject>, new()
    {
        //FIELDS
        protected T_DataObjectDictionary dataObjectDictionary;


        //PROPERTIES
        public virtual T_DataObjectDictionary DataObjectDictionary
        {
            get { return dataObjectDictionary; }

            protected set { dataObjectDictionary = value; }
        }


        //INITIALIZE
        public DataObjectDictionaryReaderProcessBase()
        {
            dataObjectDictionary = default(T_DataObjectDictionary);
        }


        //METHODS
        public override bool ExecuteProcess()
        {
            DataObjectDictionary = default(T_DataObjectDictionary);

            return base.ExecuteProcess();
        }


        //FUNCTIONS
        protected abstract void AddDataObjectToDataObjectDictionary(T_DataObject dataObject);

        protected abstract T_DataObject CreateDataObject();

        protected virtual bool ProcessApiResult()
        {
            T_DataObject dataObject = CreateDataObject();

            AddDataObjectToDataObjectDictionary(dataObject);

            return true;
        }

        protected override bool ProcessApiResults()
        {
            DataObjectDictionary = new T_DataObjectDictionary();

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
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);

                return false;
            }

            return true;
        }
    }
}