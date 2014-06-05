using MyClassLibrary;
using System;


namespace ConsoleApplication1
{
    internal class CustomerDataListReaderProcess
        : M3ApiClientInterface.DataObjectCollectionReaderProcessBase<CustomerData, System.Collections.Generic.List<CustomerData>, LogWriter>
    {
        protected override CustomerData CreateDataObject()
        {
            CustomerData customerData = new CustomerData();
            
            customerData.CustomerName = GetValueFromField("CUNM");
            customerData.CustomerNumber = GetValueFromField("CUNO");

            return customerData;
        }
    }
}