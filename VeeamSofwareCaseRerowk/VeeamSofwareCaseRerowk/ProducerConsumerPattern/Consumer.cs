using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VeeamSoftwareFirstCase.Cryptography;

namespace VeeamSoftwareFirstCase.ProducerConsumerPattern
{
    class Consumer
    {
        Queue<byte[]> _queue;
        SyncEvents _sync;
        Hasher _hash;
        public Consumer(Queue<byte[]> Queue, SyncEvents sync)
        {
            _queue = Queue;
            _sync = sync;
        }
        internal void ThreadRun()
        {
            try
            {
                _hash = new Hasher();
                byte[] block = new byte[Program.BlockSize];
                byte[] hashedBlock = new byte[32];
                bool newBlock = false;
                int blockNumber;
                while (!_sync.BreakAll)
                {
                    newBlock = false;
                    lock (_sync.QueueSync)
                    {
                        if (_queue.Count > 0)
                        {
                            block = _queue.Dequeue();
                            newBlock = true;
                            Monitor.Pulse(_sync.QueueSync);
                        }
                        else if (!_sync.endOfFile)
                            Monitor.Wait(_sync.QueueSync);
                        else
                            Thread.CurrentThread.Join();
                    }
                    if (newBlock)
                    {
                        blockNumber = Interlocked.Increment(ref _sync.blockNumber);
                        hashedBlock = _hash.CreateHash(block);
                        Console.WriteLine("{0}: {1}", blockNumber, Hasher.ToString(hashedBlock));
                        lock (_sync.lockConsole)
                        {
                            double fullPercent = Math.Round(_sync.blockNumber / (double)_sync.numberOfBlocks * 100);
                            Console.Title = string.Format("Loading: {0}%", fullPercent);
                        }
                    }
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
                Console.WriteLine("Current threadId:{0}", Thread.CurrentThread.ManagedThreadId);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                foreach (var item in _sync.tPool)
                {
                    item.Abort();
                }
            }
            finally
            {
                _hash.Dispose();
            }
        }
    }
}