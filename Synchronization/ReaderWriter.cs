using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Synchronization;
using System.Threading;

namespace Synchronization
{
    /// <summary>
    /// We need to allow multiple readers to do their thing whenever they want
    /// readers need to block the writer
    /// one writer at a time can do work
    /// writer must block all readers
    /// 
    /// </summary>
    public class ReaderWriter
    {

        Semaphore db;
        Mutex mutex;
        static int readcount = 0;
        volatile static int provereadersdonotblockeachother = 0;
        static Dictionary<int,int> numbersalreadycreated = new Dictionary<int, int>();

        public ReaderWriter(Semaphore db, Mutex mutex)
        {
            this.db = db;
            this.mutex = mutex;
        }
        /// <summary>
      
        /// </summary>
        public void Reader()
        {

            while (true)
            {
                //this looks like a consistent pattern
                //a counter is most often a static variable
                //a count needs to be locked
                //protect counter from all Reader Threads
                mutex.WaitOne();
                readcount++;
                //one means that the first reader has arrived so block a writer
                //all subsequent reader threads pass over this and aren't blocked
                if (readcount == 1)
                {
                    //this allows one thing through so thread 1 reader will pass this
                    db.WaitOne();
                }
                mutex.ReleaseMutex();
                //do the work of reading which doesn't need to be locked
                Console.WriteLine("Reading " + readcount);
                //i can't get this to duplicate which proves there is a race condition which doesn't matter
                provereadersdonotblockeachother++;

                mutex.WaitOne();
                numbersalreadycreated.Add(provereadersdonotblockeachother, provereadersdonotblockeachother);
                mutex.ReleaseMutex();

                Console.WriteLine("Prove " + provereadersdonotblockeachother);
                //done reading so lock the counter and decrement
                mutex.WaitOne();
                readcount--;
                if (readcount == 0) {
                    db.Release();
                }
                mutex.ReleaseMutex();
            }

        }

        /// <summary>

        /// </summary>
        public void Writer()
        {
            while (true)
            {
                //db allows 2 threads to enter
                //if no reader has entered then waitone will allow one write thread through
                //if a reader already used up the resource then this semaphore is blocked
                db.WaitOne();
                Console.WriteLine("Writing");
                db.Release();
            }
        }
        /// <summary>

        /// </summary>
        public static void Run()
        {


            var db = new Semaphore(1, 1);
            var mutex = new Mutex();


            Thread t1 = new Thread(new ReaderWriter(db, mutex).Writer);
            t1.Start();
      


            Thread t2 = new Thread(new ReaderWriter(db, mutex).Reader);
            t2.Start();
            t2 = new Thread(new ReaderWriter(db, mutex).Reader);
            t2.Start();
            t2 = new Thread(new ReaderWriter(db, mutex).Reader);
            t2.Start();
            t2 = new Thread(new ReaderWriter(db, mutex).Reader);
            t2.Start();
            t2 = new Thread(new ReaderWriter(db, mutex).Reader);
            t2.Start();
            t2 = new Thread(new ReaderWriter(db, mutex).Reader);
            t2.Start();
            t2 = new Thread(new ReaderWriter(db, mutex).Reader);
            t2.Start();
            t2 = new Thread(new ReaderWriter(db, mutex).Reader);
            t2.Start();



            t1.Join();


        }
    }
}
