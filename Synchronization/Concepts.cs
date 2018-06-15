using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Synchronization
{
    public class Concepts
    {
        public void Mutex()
        {
            var mutex = new Mutex(false,null);
            //one thread is allowed to go through a mutex at a time untill it calls release
            mutex.WaitOne();
           //if you call wait/P/Down it doesn't matter because the thread owns this lock and can't be locked
           //is this just a c# thing? or is this how all mutexes work
           //this is a c# thing, in C if you lock twice it will become deadlocked
            mutex.WaitOne();
        }

        public void Semaphore()
        {
            var s = new Semaphore(1,1);
            //if you set the semaphore to one it is just like a mutex
            s.WaitOne();
            //this will block, DAmmit is this only how it works in c# or 
            s.WaitOne();
        }



        public static void Run()
        {

            Thread t1 = new Thread(new Concepts().Mutex);

            t1.Start();

        }
        }
    }
