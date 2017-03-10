using System;
using System.Collections.Generic;
using System.Threading;
using VeeamSoftwareFirstCase.ProducerConsumerPattern;

namespace VeeamSoftwareFirstCase
{
    class Program
    {
        public static string Path;
        public static int BlockSize;

        static void Main(string[] args)
        {
            ArgsHandler argsHandler = new ArgsHandler(args);
            if (argsHandler.ReadArgs())
                ProducerConsumerThreadStart();
            Console.ReadLine();
        }

        private static void ProducerConsumerThreadStart()
        {
            Queue<byte[]> queue = new Queue<byte[]>();
            SyncEvents sync = new SyncEvents();

            Producer producer = new Producer(queue, sync);
            Thread producerThread = new Thread(producer.ThreadRun);
            producerThread.Start();

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                Consumer consumer = new Consumer(queue, sync);
                Thread consumerThread = new Thread(consumer.ThreadRun);
                sync.tPool.Add(consumerThread);
                consumerThread.Start();
            }
        }
    }
}