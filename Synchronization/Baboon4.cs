using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Synchronization
{
    /// <summary>
    /// https://users.cs.duke.edu/~chase/cps110-archive/prob1_s00_sol.pdf
    /// 
    /// this doesn't handle starvation
    /// Whatever direction gets there first is going to have a lock on the rope and never release it
    /// 
    /// </summary>
    /// 




    public class Baboon4
    {
        // static int[] monkey_count = new int[2] { 0, 0 };
        enum direction { WEST = 0, EAST = 1 };
        Semaphore mutex; // initially 1, protect critical sections
        Semaphore[] blocked = new Semaphore[2];
        // one for each direction. Each initially 0, so always sleep on first P
        static int[] blockedCnt = new int[2]; // number of baboons waiting on each side
        static int[] travellers = new int[2]; // number of baboons on the rope heading each direction
                                              //(at least one is zero at any time)

        public Baboon4(Semaphore mutex, Semaphore[] blocked)
        {
            this.mutex = mutex;
            this.blocked = blocked;

        }
        void Baboon(direction dir)
        {
            int revdir = (int)(dir == direction.EAST ? direction.WEST : direction.EAST); // the reverse direction
            mutex.WaitOne();

            while (travellers[revdir] != 0)
            {
                blockedCnt[(int)dir]++; // announce our intention to block
                mutex.Release(); // trade mutex for block
                blocked[(int)dir].WaitOne();
                mutex.WaitOne();
            }
            travellers[(int)dir]++; // we’re free to cross
            mutex.Release();
            Console.WriteLine("Crossing bridge from " + dir.ToString());

            mutex.WaitOne();
            travellers[(int)dir]--;
            if (!(travellers[(int)dir] == 0) )
            {
                // if we’re the last one heading this way,
                // wakeup baboons waiting for us to finish.
                while (blockedCnt[(int)revdir]-- != 0)
                    blocked[(int)revdir].Release();
            }
            mutex.Release();
        }

        void CrossEast()
        {
            while (true)
            {
                Baboon(direction.EAST);
            }
        }
        void CrossWest()
        {
            while (true)
            {
                Baboon(direction.WEST);
            }
        }
        public static void Run()
        {

            //block the baboons trying to cross in this direction
            var mutex = new Semaphore(1, 1);
            Semaphore[] blocked = new Semaphore[2] {
                new Semaphore(0,1),
                new Semaphore(0,1)
            };


            Thread t1 = new Thread(new Baboon4(mutex,blocked).CrossEast);
            t1.Start();
            t1 = new Thread(new Baboon4(mutex, blocked).CrossWest);
            t1.Start();
            t1 = new Thread(new Baboon4(mutex, blocked).CrossWest);
            t1.Start();




        }
    }
}
