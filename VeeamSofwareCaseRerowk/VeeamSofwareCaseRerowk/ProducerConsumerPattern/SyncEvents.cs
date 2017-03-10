using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VeeamSoftwareFirstCase.ProducerConsumerPattern
{
    class SyncEvents
    {
        public List<Thread> tPool;
        public object QueueSync;
        public object lockConsole;
        public object QueueSize;
        public bool BreakAll;
        public bool endOfFile;
        public int blockNumber;
        public long numberOfBlocks;

        public SyncEvents()
        {
            tPool = new List<Thread>();
            QueueSync = new object();
            lockConsole = new object();
            QueueSize = new object();
            BreakAll = false;
            endOfFile = false;
        }
    }
}