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
    /// The point of this is to constanly make H20 molecues concurrently
    /// The rule is that you first make 2 Hydrogens, then a Oxygen, which then does the work of bonding
    /// H1 + H2 + 0 = H20
    /// 
    /// 
    /// </summary>
    public class H20
    {

        Semaphore hSem, oSem, wait;
        Mutex mutex;
        static int count = 0;
        
        public H20(Semaphore hSem, Semaphore oSem, Semaphore wait, Mutex mutex)
        {
            this.hSem = hSem;
            this.oSem = oSem;
            this.wait = wait;
            this.mutex = mutex;
        }
        /// <summary>
        /// 1) Thread1 enters
        //  2) Thread1 locks and modifies count
        //  3) Thread1 is waiting on hSem.WaitOne();
        /// 4) Thread2 enters, locks and modifies count
        /// 5) thread2 releases thread1 and Oxyten
        /// 6) Thread1 and Thread2 wait on wait.WaitOne();
        /// 7) Oxygen Goes
        /// 
        /// </summary>
        public void Hyrdrogen()
        {
            //whenever we do these things, the processing has to run constantly so its always a while loop
            while (true)
            {

                //only allow one H thread here at a time
                //the critical section is the count variable and any use of it read or write
                //mutex is protecting critical section count
                mutex.WaitOne();  //1) only one thread at a time can access count
                if (count % 2 == 0)
                {
                    count++; //2) increment count to indicate a thread has handled it 
                    Console.Write("H1");
                    mutex.ReleaseMutex();
                    hSem.WaitOne(); // this is blocking H1 until H2 is created so no more H1s are created until H2 is done
                }
                else {
                    count++;
                    Console.Write(" + H2");
                    mutex.ReleaseMutex();
                    hSem.Release(); //this is releasing H1 to allow it to make another H1
                    oSem.Release(); //this is telling oxygen that its okay to bond

                }
                //this blocks all Hydrogens until the Oxygen releases it

                wait.WaitOne();
            }

        }

        void Bond()
        {
            Console.WriteLine(" = H2O");
        }
        /// <summary>
        /// 1) Thread 3 enters and waits on oSem.WaitOne()  means Thread3 will not run till thread1 and 2 are done
        /// 2) Thread 3 continues as soon as Thread 2 calls oSem.Release()
        /// 3) thread 3 does work in Bond()
        /// 4) Thread 3 calls wait.Release() twice which fress both Thread1 and Thread2
        /// </summary>
        public void Oxygen()
        {
            while (true)
            {
                //the oSem semaphore is set to 0 meaning that it is paused immediately
                //if nobody did oSem.release, this would get stuck forever
                //why does the second H do the releasing
                oSem.WaitOne();
                Console.Write(" + O");
                Bond();
                //this is called 2 times because hydrogen is setup such 
                //that it calls wait.waitone twice before oxygen goes
                wait.Release(); 
                wait.Release();
            }
        }
        /// <summary>
        /// can all processes be listed out in one cycle like this  to show exactly what would happen?
        /// 1) Thread1 enters
        //  2) Thread1 locks and modifies count
        //  3) Thread1 is waiting on hSem.WaitOne();
        /// 4) Thread2 enters, locks and modifies count
        /// 5) thread2 releases thread1 and Oxyten
        /// 6) Thread1 and Thread2 wait on wait.WaitOne();
        /// 7) Oxygen Goes
        /// 8) Thread 3 enters and waits on oSem.WaitOne()  
        /// 9) Thread 3 continues as soon as Thread 2 calls oSem.Release()
        /// 10) thread 3 does work in Bond()
        /// 11) Thread 3 calls wait.Release() twice which fress both Thread1 and Thread2
        /// </summary>
        public static void Run()
        {

            //no matter what happens, how many threads 
            //the console should always print in order HH0, HH0, HHO, 
            //no O should be next to each other
            //no HHH should happen

            var hSem = new Semaphore(0, 10);
            var oSem = new Semaphore(0, 10);
            var wait = new Semaphore(0, 10);
            var mutex = new Mutex();


            Thread t1 = new Thread( new H20(hSem,oSem,wait,mutex).Hyrdrogen);
            t1.Start();
            Thread p2 = new Thread(new H20(hSem, oSem, wait, mutex).Hyrdrogen);
            p2.Start();
         


            Thread t2 = new Thread(new H20(hSem, oSem, wait, mutex).Oxygen);
            t2.Start();
            t2 = new Thread(new H20(hSem, oSem, wait, mutex).Oxygen);
            t2.Start();
            t2 = new Thread(new H20(hSem, oSem, wait, mutex).Oxygen);
            t2.Start();



            t1.Join();


        }
    }
}
