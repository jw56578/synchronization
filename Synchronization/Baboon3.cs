using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Synchronization
{
    /// <summary>
    /// 
    /// </summary>
    public class Baboon2
    {
        Semaphore mutex, crossRightMutex, crossLeftMutex;
        Semaphore crossRight, crossLeft;
        static int waitingLeftCount = 0;
        static int waitingRightCount = 0;
        static int onRopeFromLeftCount = 0;
        static int onRopeFromRightCount = 0;

        public Baboon2(Semaphore crossRight, Semaphore crossLeft, Semaphore mutex, Semaphore crossRightMutex, Semaphore crossLeftMutex)
        {
            this.crossRight = crossRight;
            this.crossLeft = crossLeft;
            this.mutex = mutex;
            this.crossRightMutex = crossRightMutex;
            this.crossLeftMutex = crossLeftMutex;

        }
        void CrossFromLeft()
        {
            while (true)
            {
                //we block everyone so that if 2 baboons reach the rope at the same time
                //only one can get on
                mutex.WaitOne();
                waitingLeftCount++;
                if (onRopeFromRightCount > 0)
                {
                    mutex.Release();
                    crossLeft.WaitOne();
                    mutex.WaitOne();
                }
               
                //if we have reached this point then we are the first on the rope so block the other direction
                onRopeFromLeftCount++;
                if (onRopeFromLeftCount == 1)
                {
                    crossRight.WaitOne();
                }
                waitingLeftCount--;
               

           
                mutex.Release();

                Console.WriteLine("Crossing Left " + onRopeFromLeftCount);


                mutex.WaitOne();
                if (waitingRightCount >= 3)
                {
                    crossRightMutex.Release();
                    crossRight.Release();
                    mutex.Release();
                    crossLeftMutex.WaitOne();
                    mutex.WaitOne();

                }
                mutex.Release();


            }
        }
        void WaitTillSaveToCross(Semaphore mutex, int onropefromoppositeside)
        {

        }
        void CrossFromRight()
        {
            while (true)
            {
                //we block everyone so that if 2 baboons reach the rope at the same time
                //only one can get on
                mutex.WaitOne();
                waitingRightCount++;
                if (onRopeFromLeftCount > 0)
                {
                    mutex.Release();
                    crossRight.WaitOne();
                    mutex.WaitOne();
                }
            
         
                //if we have reached this point then we are the first on the rope so block the other direction
                onRopeFromRightCount++;
                if (onRopeFromLeftCount == 1)
                {
                    crossLeft.WaitOne();
                }

                waitingRightCount--;

       
                mutex.Release();


                Console.WriteLine("Crossing Right " + onRopeFromRightCount);

                mutex.WaitOne();
                if (waitingLeftCount >= 3)
                {
                    crossLeftMutex.Release();
                    crossLeft.Release();
                    mutex.Release();
                    crossRightMutex.WaitOne();
                    mutex.WaitOne();

                }
                mutex.Release();
            }
        }
        public static void Run()
        {

            //block the baboons trying to cross in this direction
            var crossRight = new Semaphore(1, 1);
            var crossLeft = new Semaphore(1, 1);
            var mutex = new Semaphore(1, 1);
            var wc = new Semaphore(1, 1);
            var rc = new Semaphore(1, 1);


            Thread t1 = new Thread(new Baboon2(crossRight, crossLeft, mutex, wc, rc).CrossFromLeft);
            t1.Start();
            t1 = new Thread(new Baboon2(crossRight, crossLeft, mutex, wc, rc).CrossFromLeft);
            t1.Start();
            t1 = new Thread(new Baboon2(crossRight, crossLeft, mutex, wc, rc).CrossFromLeft);
            t1.Start();

            t1 = new Thread(new Baboon2(crossRight, crossLeft, mutex, wc, rc).CrossFromRight);
            t1.Start();
            t1 = new Thread(new Baboon2(crossRight, crossLeft, mutex, wc, rc).CrossFromRight);
            t1.Start();
            t1 = new Thread(new Baboon2(crossRight, crossLeft, mutex, wc, rc).CrossFromRight);
            t1.Start();
        }

    }
}
