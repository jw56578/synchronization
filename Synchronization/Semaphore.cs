using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Synchronization
{
    //you cannot create your own semphore implemenation because you cannot turn off interupts in order to protect count

    public class _Semaphore
    {
        int count;
        Queue threads;
        System.Threading.Semaphore realSemaphore;
        string name = null;
        public _Semaphore(int value, string name)
        {
            this.count = value;
            this.name = name;
            threads = new Queue();
            realSemaphore = new System.Threading.Semaphore(value,value);
        }
        public void P() {
            //turn off interrupts
            count--;
            if (count < 0)
            {
                //block
                realSemaphore.WaitOne();
                //Thread.CurrentThread.Suspend();
                //threads.Enqueue(Thread.CurrentThread);
            }
            //turn on interrupts
        }
        public void V() {
            //turn off interrupts
            count++;
            if (count <= 0) {
                //unblock
               // var thread = threads.Dequeue() as Thread;
               // thread.Resume();
                realSemaphore.Release();
            }
            //turn on interrupts
        }
    }
}
