using System;
using System.Collections.Generic;


namespace M3ApiClientInterface
{
    public interface ReaderProcessInterface
    {
        ApiData ApiData { get; set; }

        ConnectionData ConnectionData { get; set; }

        Boolean ErrorOnReturnCode8 { get; set; }

        Boolean EnableZippedTransactions { get; set; }

        UInt16 ExecutionsAttempted { get; }

        UInt16 ExecutionsToAttempt { get; set; }

        UInt64 MaximumRecordsToReturn { get; set; }

        UInt32 MaximumTimeToWaitBetweenRetries { get; set; }

        UInt32 MaximumWaitTime { get; set; }

        UInt32 MinimumTimeToWaitBetweenRetries { get; set; }

        List<RequestFieldData> RequestFieldDataList { get; set; }

        UInt32? ReturnCode { get; }


        Boolean ExecuteProcess();
    }
}