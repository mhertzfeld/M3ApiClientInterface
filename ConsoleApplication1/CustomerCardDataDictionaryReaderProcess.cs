using MyClassLibrary;
using System;


namespace ConsoleApplication1
{
    internal class CustomerCardDataDictionaryReaderProcess
        : M3ApiClientInterface.DataObjectDictionaryReaderProcessBase<String, CustomerCardData, System.Collections.Generic.Dictionary<String, CustomerCardData>>
    {
        protected override void AddDataObjectToDataObjectDictionary(CustomerCardData dataObject)
        {
            DataObjectDictionary.Add(dataObject.Key, dataObject);
        }

        protected override CustomerCardData CreateDataObject()
        {
            CustomerCardData customerCardData = new CustomerCardData();
            customerCardData.CardNumber = GetValueFromField("CCNR");
            customerCardData.CustomerNumber = GetValueFromField("CUNO");
            customerCardData.Key = GetValueFromField("CUNO");

            return customerCardData;
        }
    }
}