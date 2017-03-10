using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VeeamSoftwareFirstCase.ProducerConsumerPattern
{
    class Producer
    {
        Queue<byte[]> _queue;
        SyncEvents _sync;
        public Producer(Queue<byte[]> Queue, SyncEvents sync)
        {
            _queue = Queue;
            _sync = sync;
        }
        internal void ThreadRun()
        {
            try
            {
                using (FileStream s = new FileStream(Program.Path, FileMode.Open, FileAccess.Read))
                {
                    int _nRead;
                    _sync.numberOfBlocks = ((long)Math.Round(s.Length / (double)Program.BlockSize)) + 1;
                    while (s.Position < s.Length)
                    {
                        byte[] block = new byte[Program.BlockSize];  
                        if (_sync.BreakAll)
                            break;  
                        lock (_sync.QueueSync)
                        {
                            if (_queue.Count >= Environment.ProcessorCount)
                            {
                                lock (_sync.QueueSync) { Monitor.Wait(_sync.QueueSync); }
                            }
                            _nRead = s.Read(block, 0, Program.BlockSize);
                            if (_queue.Count < Environment.ProcessorCount)
                            {
                                _queue.Enqueue(block);
                                Monitor.PulseAll(_sync.QueueSync);
                            }
                            if (_nRead < Program.BlockSize)
                            {
                                block = new byte[_nRead];
                                lock (_sync.QueueSync)
                                {
                                    _queue.Enqueue(block);
                                    Monitor.PulseAll(_sync.QueueSync);
                                }
                                break;
                            }
                        }
                    }
                    _sync.endOfFile = true;
                    Thread.CurrentThread.Join();
                }
            }
            catch (OutOfMemoryException e)
            {
                Console.WriteLine("OutOfMemoryException - попробуйте уменьшить размер блока данных");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                foreach (var item in _sync.tPool)
                {
                    item.Abort();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                foreach (var item in _sync.tPool)
                {
                    item.Abort();
                }
            }
        }
    }
}