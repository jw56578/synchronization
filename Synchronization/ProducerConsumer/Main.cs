using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Synchronization;
using System.Threading;

namespace Synchronization.ProducerConsumer
{
    public class Main
    {
        Semaphore fillCount;
        Semaphore emptyCount;
        public static List<int> queue;

        public Main()
        {
            int bufferSize = 100;
            queue = new List<int>();
            fillCount = new Semaphore(0, bufferSize);
            emptyCount = new Semaphore(bufferSize,bufferSize);

        }
        public void Producer()
        {
            int index = 0;
            while (true) {
                //this is the code that is supposed to be the work being done to produce data to put in the queue
                //this work is not protected so if there are 2 producers then it could be a problem
                int item = index++; 
                //this is decrementing the counter, obviously if you are adding to the queue, then number of empty slots
                //reduces by one
                emptyCount.WaitOne();
                queue.Add(item);
                //this is incrementing count. if you added to queue, the number of slots filled increases by one
                //this will now possibly wake up a consumer thread
                fillCount.Release();
                Console.WriteLine("Produced:" + item);
            }
                
        }
        public void Consumer()
        {
            while (true)
            {
                //decrement the counter. we are removing an item so the slots filled is one less
                fillCount.WaitOne();
                Console.WriteLine("Size:" + queue.Count);
                int item = queue[0];
                queue.RemoveAt(0);
                //increment counter. the number of empty slots has now increased by one
                emptyCount.Release();
                Console.WriteLine("removed " + item);
            }
        }

        public void Run()
        {
            Thread t1 = new Thread(Producer);
            t1.Start();
            Thread p2 = new Thread(Producer);
            p2.Start();


            Thread t2 = new Thread(Consumer);
            t2.Start();
            Thread t3 = new Thread(Consumer);
            t3.Start();



            t1.Join();


        }
    }
}
