using System;


namespace M3ApiClientInterface
{
    public class DataObjectCollectionReaderProcess<T_DataObject, T_DataObjectCollection>
        : DataObjectCollectionReaderProcessBase<T_DataObject, T_DataObjectCollection>
        where T_DataObject : DataObjectInterface, new()
        where T_DataObjectCollection : System.Collections.Generic.ICollection<T_DataObject>, new()
    {
        protected override T_DataObject CreateDataObject()
        {
            T_DataObject _DataObject = new T_DataObject();
            _DataObject.SetFields(ref serverId);
            return _DataObject;
        }
    }
}