using MyClassLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;


namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString());

            String apiServer = MyDataConverter.ToString(ConfigurationManager.AppSettings.Get("ApiServer"));

            Int32 apiPort = MyDataConverter.ToInt32(ConfigurationManager.AppSettings.Get("ApiPort"));

            String userName = MyDataConverter.ToString(ConfigurationManager.AppSettings.Get("UserName"));

            String password = MyDataConverter.ToString(ConfigurationManager.AppSettings.Get("Password"));

            M3ApiClientInterface.ConnectionData connectionData = new M3ApiClientInterface.ConnectionData(apiServer, apiPort, userName, password);

            M3ApiClientInterface.ApiData customerDataListReaderApiData = new M3ApiClientInterface.ApiData("CRS610MI", "LstByNumber");

            CustomerDataListReaderProcess customerDataListReaderProcess = new CustomerDataListReaderProcess();
            if (customerDataListReaderProcess.ProcessExecution(connectionData, customerDataListReaderApiData, new M3ApiClientInterface.RequestFieldDataList()))
            { Console.WriteLine("CustomerDataListReaderProcess PASS"); }
            else
            { Console.WriteLine("CustomerDataListReaderProcess FAIL"); }

            M3ApiClientInterface.ApiData customerDataReaderApiData = new M3ApiClientInterface.ApiData("CRS610MI", "GetBasicData");

            M3ApiClientInterface.RequestFieldDataList requestFieldDataList = new M3ApiClientInterface.RequestFieldDataList();
            requestFieldDataList.Add(new M3ApiClientInterface.RequestFieldData("CONO", "1"));
            requestFieldDataList.Add(new M3ApiClientInterface.RequestFieldData("CUNO", "8888"));

            CustomerDataReaderProcess customerDataReaderProcess = new CustomerDataReaderProcess();
            if (customerDataReaderProcess.ProcessExecution(connectionData, customerDataReaderApiData, requestFieldDataList))
            { Console.WriteLine("CustomerDataReaderProcess PASS"); }
            else
            { Console.WriteLine("CustomerDataReaderProcess FAIL"); }

            WarehouseTableDataListReaderProcess warehouseTableDataListReaderProcess = new WarehouseTableDataListReaderProcess();

            if (warehouseTableDataListReaderProcess.ProcessExecution())
            { Console.WriteLine("WarehouseTableDataListReaderProcess PASS"); }
            else
            { Console.WriteLine("WarehouseTableDataListReaderProcess FAIL"); }

            Console.WriteLine(DateTime.Now.ToString());
            Console.ReadLine();
        }
    }
}