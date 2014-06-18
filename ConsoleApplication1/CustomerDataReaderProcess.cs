using System;


namespace ConsoleApplication1
{
    internal class CustomerDataReaderProcess
        : M3ApiClientInterface.DataObjectReaderProcessBase<CustomerData>
    {
        //FUNCTIONS
        protected override CustomerData CreateDataObject()
        {
            CustomerData customerData = new CustomerData();

            customerData.CustomerName = GetValueFromField("CUNM");
            customerData.CustomerNumber = GetValueFromField("CUNO");

            return customerData;
        }
    }
}