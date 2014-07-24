using MyClassLibrary;
using System;
using System.Configuration;


namespace ConsoleApplication1
{
    public class WarehouseTableDataListReaderProcess
        : M3ApiClientInterface.DataObjectCollectionReaderProcessBase<WarehouseTableData, WarehouseTableDataList>
    {
        //INITIALIZE
        public WarehouseTableDataListReaderProcess()
        {
            ApiData = new M3ApiClientInterface.ApiData();
            ApiData.Api = WarehouseTableDataListReaderProcessResource.API_Name;
            ApiData.Method = WarehouseTableDataListReaderProcessResource.API_Method;

            ConnectionData = new M3ApiClientInterface.ConnectionData();
            ConnectionData.Password = MyDataConverter.ToString(ConfigurationManager.AppSettings.Get("Password"));
            ConnectionData.Port = MyDataConverter.ToInt32(ConfigurationManager.AppSettings.Get("ApiPort"));
            ConnectionData.Server = MyDataConverter.ToString(ConfigurationManager.AppSettings.Get("ApiServer"));
            ConnectionData.UserName = MyDataConverter.ToString(ConfigurationManager.AppSettings.Get("UserName"));            
        }


        //METHODS
        public override bool ExecuteProcess()
        {
            RequestFieldDataList = new M3ApiClientInterface.RequestFieldDataList();

            return base.ExecuteProcess();
        }


        //FUNCTIONS
        protected override void AddDataObjectToDataObjectCollection(WarehouseTableData dataObject)
        {
            if (!dataObject.Exclude.Value)
            { base.AddDataObjectToDataObjectCollection(dataObject); }
        }

        protected override WarehouseTableData CreateDataObject()
        {
            WarehouseTableData warehouseTableData = new WarehouseTableData();
            warehouseTableData.Exclude = MyDataConverter.ToBooleanNullable(GetValueFromField(WarehouseTableDataListReaderProcessResource.Field_Exclude));
            warehouseTableData.FilterField = GetValueFromField(WarehouseTableDataListReaderProcessResource.Field_FilterField);
            warehouseTableData.FromWarehouseId = MyDataConverter.ToInt32(GetValueFromField(WarehouseTableDataListReaderProcessResource.Field_FromWarehouseId));
            warehouseTableData.ToWarehouseId = MyDataConverter.ToInt32(GetValueFromField(WarehouseTableDataListReaderProcessResource.Field_ToWarehouseId));
            warehouseTableData.WarehouseTableId = GetValueFromField(WarehouseTableDataListReaderProcessResource.Field_WarehouseTableId);

            return warehouseTableData;
        }


        //STATIC METHODS
        public static Boolean GetWarehouseTableDataList(out WarehouseTableDataList warehouseTableDataList)
        {
            WarehouseTableDataListReaderProcess warehouseTableDataListReaderProcess = new WarehouseTableDataListReaderProcess();

            if (warehouseTableDataListReaderProcess.ExecuteProcess())
            {
                warehouseTableDataList = warehouseTableDataListReaderProcess.DataObjectCollection;

                return true;
            }
            else
            {
                //LOG HERE

                warehouseTableDataList = null;

                return false;
            }
        }
    }
}