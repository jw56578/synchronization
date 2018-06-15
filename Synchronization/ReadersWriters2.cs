using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Synchronization
{
    /// <summary>
    /// I think the point of this is to allow more than 1 writer
    /// I think this can be used as the crossing bridge or crossing canyon problem
    /// I copied this code from the slides, not sure why there are so many mutexas
    /// </summary>
    public class ReadersWriters2
    {
        Mutex mutex, wc_mutex, rc_mutex;
        Semaphore r, w;
        static int readcount = 0;
        static int writecount = 0;

        public ReadersWriters2(Semaphore r, Semaphore w, Mutex mutex, Mutex wc_mutex, Mutex rc_mutex)
        {
            this.r = r;
            this.w = w;
            this.mutex = mutex;
            this.wc_mutex = wc_mutex;
            this.rc_mutex = rc_mutex;

        }
        void Reader()
        {
            while (true)
            {
                //we are about to modify a counter so we need to lock on mutex
                //what is the point of this mutex when we have rc_mutex
                mutex.WaitOne();
                //we are waiting here because the writer will potentially tell us to hold off because its already writing
                r.WaitOne();
                //why are we waiting again? the writer doesn't have this
                rc_mutex.WaitOne();
                readcount++;
                
                if (readcount == 1) w.WaitOne();
                rc_mutex.ReleaseMutex();
                r.Release();
                mutex.ReleaseMutex();


                //read();
                Console.WriteLine("reading");
                rc_mutex.WaitOne();
                readcount--;
                if (readcount == 0) w.Release();
                rc_mutex.ReleaseMutex();
            }
        }
        void Writer()
        {
            while (true)
            {
                wc_mutex.WaitOne();
                writecount++;
                //tell the readers to stop
                //the writer won't stop here because the semaphore allows 1
                if (writecount == 1) r.WaitOne();
                wc_mutex.ReleaseMutex();


                //this is only allowing one write at a time, what if multiple are allowed
                w.WaitOne();
                Console.WriteLine("writing");
                w.Release();


                wc_mutex.WaitOne();
                writecount--;
                if (writecount == 0) r.Release();
                wc_mutex.ReleaseMutex();
            }
        }
        public static void Run()
        {


            var read = new Semaphore(1, 1);
            var write = new Semaphore(1, 1);
            var mutex = new Mutex();
            var wc = new Mutex();
            var rc = new Mutex();


            Thread t1 = new Thread(new ReadersWriters2(read,write,mutex,wc,rc).Writer);
            t1.Start();
            t1 = new Thread(new ReadersWriters2(read, write, mutex, wc, rc).Writer);
            t1.Start();
            t1 = new Thread(new ReadersWriters2(read, write, mutex, wc, rc).Writer);
            t1.Start();

            t1 = new Thread(new ReadersWriters2(read, write, mutex, wc, rc).Reader);
            t1.Start();
            t1 = new Thread(new ReadersWriters2(read, write, mutex, wc, rc).Reader);
            t1.Start();
            t1 = new Thread(new ReadersWriters2(read, write, mutex, wc, rc).Reader);
            t1.Start();
        }

   }
}
