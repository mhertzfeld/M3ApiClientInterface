using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString());

            M3ApiClientInterface.ConnectionData connectionData = new M3ApiClientInterface.ConnectionData("M3BENONP", 16305, "username", @"password");

            M3ApiClientInterface.ApiData customerDataListReaderApiData = new M3ApiClientInterface.ApiData("CRS610MI", "LstByNumber");

            using (CustomerDataListReaderProcess customerDataListReaderProcess = new CustomerDataListReaderProcess())
            {
                if (customerDataListReaderProcess.ProcessExecution(connectionData, customerDataListReaderApiData, new M3ApiClientInterface.RequestFieldDataList()))
                { Console.WriteLine("PASS"); }
                else
                { Console.WriteLine("FAIL"); }
            }

            M3ApiClientInterface.ApiData customerDataReaderApiData = new M3ApiClientInterface.ApiData("CRS610MI", "GetBasicData");

            M3ApiClientInterface.RequestFieldDataList requestFieldDataList = new M3ApiClientInterface.RequestFieldDataList();
            requestFieldDataList.Add(new M3ApiClientInterface.RequestFieldData("CONO", "1"));
            requestFieldDataList.Add(new M3ApiClientInterface.RequestFieldData("CUNO", "8888"));

            using (CustomerDataReaderProcess customerDataReaderProcess = new CustomerDataReaderProcess())
            {
                if (customerDataReaderProcess.ProcessExecution(connectionData, customerDataReaderApiData, requestFieldDataList))
                { Console.WriteLine("PASS"); }
                else
                { Console.WriteLine("FAIL"); }
            }

            Console.WriteLine(DateTime.Now.ToString());
            Console.ReadLine();
        }
    }
}