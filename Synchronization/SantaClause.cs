using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Synchronization;
using System.Threading;

namespace Synchronization
{
    //https://github.com/ristes/synchronization-examples/blob/master/src/mk/ukim/finki/os/synchronization/problems/santaclaus/SantaClaus.java
    /*
     * Question 2) Santa Claus Synchronization Problem
        Santa Claus sleeps in his shop at the North Pole and can only be awakened by either (1) all
        nine reindeer being back from their vacation, or (2) the elves are having difficulty making toys.
        There are 3 types of threads: santa claus (1), reindeer (9), elves (N).
        Constraints:
        1. The last reindeer to arrive must get Santa while the others wait in a warming hut before
        being harnessed to the sleigh.
        2. After the ninth reindeer arrives, Santa must invoke prepareSleigh, and then all nine
        reindeer must invoke getHitched.
        3. To allow Santa to get some sleep, the elves can only wake him when three of them have
        problems.
        4. When three elves are having their problems solved, any other elves wishing to visit
        Santa must wait for those elves to return.
        5. After the third elf arrives, Santa must invoke helpElves. Concurrently, all three elves
        should invoke getHelp.
        6. All three elves must invoke getHelp before any additional elves enter (increment the elf
        counter).
        7. Santa should run in a loop so he can help many sets of elves.
        */
    public class SantaClause
    {
        //Santa needs to wait and other threads need to wake him so that means a semaphore is needed
        //since sleeping is default it should start at 0
        Semaphore santa;
        //we need to block reindeer while santa is doing prepareSleigh
        //a thread needs to block and be unblocked by another thread, therefore semaphore
        Semaphore reindeer;
        //
        Semaphore elves;
        
        //need to know when 9 reindeer have arrived. this calls for a counter
        static int reindeerCount = 0;
        //need to know when 3 elves arrive, this calls for a counter

        static int elvesCounter = 0;

        //as reindeer are incrementing their counter, it needs to be locked so only one thread is incrementing at a time
        Semaphore reindeerMutex;
        //as elves are incrementing their counter, it needs to be locked
        Semaphore elvesMutex;
        //santa needs a lock when it checks the counts
        //you cannot use the elf or reindeer mutex
        Semaphore countMutex;


        public SantaClause(Semaphore santa, Semaphore reindeer, Semaphore elves, Semaphore reindeerMutex, Semaphore elvesMutex, Semaphore countMutex)
        {
            this.santa = santa;
            this.reindeer = reindeer;
            this.elves = elves;
            this.reindeerMutex = reindeerMutex;
            this.elvesMutex = elvesMutex;
            this.countMutex = countMutex;

        }
        /// <summary>

        /// </summary>
        public void Santa()
        {

            while (true)
            {
                santa.WaitOne();
                Console.WriteLine("Santa going to sleep");
                //santa sleeps immediately until needed
                
                //check if its time to help reindeer which have priority

                countMutex.WaitOne();

                if (reindeerCount == 9)
                {
                    reindeerCount = 0;
                    Console.WriteLine("Preparing Sleigh");
                    reindeer.Release(9);
                }
                else 
                {
                    Console.WriteLine("Giving Help");
                    elves.Release(3);
                }
                countMutex.Release();


            }

        }
        public void Reindeer()
        {

            while (true)
            {
              
                countMutex.WaitOne();
                Console.WriteLine("Reindeer " + reindeerCount + " back from vacation");
                reindeerCount++;

                if (reindeerCount == 9)
                {
                    //wake up santa
                    santa.Release();
                }
                countMutex.Release();
                reindeer.WaitOne();
                //santa is done preparing, time to get hitched
                //this all needs to happen before going back on vacation
                Console.WriteLine("Getting Hitched");
                countMutex.WaitOne();
                reindeerCount++;
                if (reindeerCount == 9)
                {
                    reindeerCount = 0;
                    reindeer.Release(8);
                    countMutex.Release();
                }
                else
                {
                    countMutex.Release();
                    reindeer.WaitOne();
                }

            }

        }

        /// <summary>

        /// </summary>
        public void Elves()
        {
            while (true)
            {
                //this will block elves after 3 because its only released if count < 3
                elvesMutex.WaitOne();
                Console.WriteLine("Elf needs help " + elvesCounter);
                countMutex.WaitOne();
                elvesCounter++;
                if (elvesCounter == 3)
                {
                    santa.Release();
                }
                else
                {
                    elvesMutex.Release();
                }
                countMutex.Release();
                elves.WaitOne();

                Console.WriteLine("Elf get help");


                //all elves need to leave before more can ask for help
                countMutex.WaitOne();
                elvesCounter--;
                if (elvesCounter == 0) {
                    elvesMutex.Release();
                }
                countMutex.Release();






            }
        }
        /// <summary>
        /// whilst running we should see a constant stream of output
        /// Prepare sleigh should not be seen till 9 reindeer come back from vacation
        /// </summary>
        public static void Run()
        {

            //Santa needs to wait and other threads need to wake him so that means a semaphore is needed
            //since sleeping is default it should start at 0
            //other threads needing to control others = semaphore
            var santa = new Semaphore(0, 1);

            var reindeer = new Semaphore(0, 9);
            var elves = new Semaphore(0,3);

            //C# mutex's don't work properly 
            var reindeerMutex = new Semaphore(1, 1);
            var elvesMutex = new Semaphore(1, 1);
            var countMutex = new Semaphore(1, 1);


            Thread t1 = new Thread(new SantaClause(santa, reindeer, elves, reindeerMutex, elvesMutex, countMutex).Santa);
            t1.Start();


            for (int i = 0; i < 9; i++)
            {

                Thread t2 = new Thread(new SantaClause(santa, reindeer, elves, reindeerMutex, elvesMutex, countMutex).Reindeer);
                t2.Start();
            }


            for (int i = 0; i < 3; i++)
            {

                Thread t2 = new Thread(new SantaClause(santa, reindeer, elves, reindeerMutex, elvesMutex, countMutex).Elves);
                t2.Start();
            }

            t1.Join();


        }
    }
}
