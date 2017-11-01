using System;


namespace M3ApiClientInterface
{
    public class DataObjectReaderProcess<T_DataObjectReader>
        : DataObjectReaderProcessBase<T_DataObjectReader>
        where T_DataObjectReader : DataObjectInterface, new()
    {
        protected override T_DataObjectReader CreateDataObject()
        {
            T_DataObjectReader _DataObjectReader = new T_DataObjectReader();
            _DataObjectReader.SetFields(ref serverId);
            return _DataObjectReader;
        }
    }
}