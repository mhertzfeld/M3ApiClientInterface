using System;
using System.Collections.Generic;


namespace M3ApiClientInterface
{
    public interface DataObjectCollectionReaderInterface<T_DataObject, T_DataObjectCollection>
        : ReaderProcessInterface
        where T_DataObjectCollection : System.Collections.Generic.ICollection<T_DataObject>, new()
    {
        T_DataObjectCollection DataObjectCollection { get; }
    }
}