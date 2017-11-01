using System;
using System.Collections.Generic;


namespace M3ApiClientInterface
{
    public interface DataObjectReaderInterface<T_DataObject>
        : ReaderProcessInterface
    {
        T_DataObject DataObject { get; }
    }
}