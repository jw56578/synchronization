using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Synchronization
{
    /// <summary>
    /// http://www.cs.cornell.edu/courses/cs414/2007sp/homework/hw2_soln.pdf
    /// </summary>
    public class Baboon3
    {

 
        //Shared data
        static int[] monkey_count = new int[2] { 0,0}; /* baboon counter for each direction */



        Semaphore[] mutex = new Semaphore[2] {
            new Semaphore(1,1),
            new Semaphore(1,1)
        };  /* mutual exclusion for each direction*///semaphore mutex[2] = { 1, 1 };

        //
        Semaphore max_on_rope = new Semaphore(5,5);

        //rope is used to indicate that someone has taken the rope in a certain direction and the other direction cannot go
        //that is why its value is 1 as opposed to max rope which is 5
        //thread 1 Left enters WaitUntilSafeToCross and incremens to 1 so it takes the rope
        //thread 2 Right will do the same thing and be at count 1 so it will try to take the rope
        ///      but it will be blocked since rope is taken by thread 1
        ///      
        Semaphore rope = new Semaphore(1,1); 

        Semaphore order = new Semaphore(1, 1);
     
        public Baboon3(Semaphore[] mutex, Semaphore max_on_rope, Semaphore rope, Semaphore order)
        {
            this.mutex = mutex;
            this.max_on_rope = max_on_rope;
            this.rope = rope;
            this.order = order;

        }
        void WaitUntilSafeToCross(int dest)
        {
            order.WaitOne();
            //block all other threads but this one in order to modify the count
            mutex[dest].WaitOne();
            //increment how many baboons are wanting to get on the rope
            monkey_count[dest]++;
            //so this means that only the first baboon to arrive gets to go on the rope
            if (monkey_count[dest] == 1)
            {
                Console.WriteLine(dest + " Is taking the rope");
                //is the first monkey in line, waiting to acquire the rope
                (rope).WaitOne();
            }
            //done modifying the counter
            mutex[dest].Release();

            order.Release();
            Console.WriteLine(dest + " Is taking max rope");

            //not sure how this is working
            //anyone is able to take the max rope even if the didn't take the rop
            max_on_rope.WaitOne();
        }
        void DoneWithCrossing(int dest)
        {
            //modifying the count so lock
            mutex[dest].WaitOne();
            max_on_rope.Release();
            monkey_count[dest]--;
            if (monkey_count[dest] == 0)
                //is the last monkey, release the rope
                rope.Release();
            mutex[dest].Release();
        }

        void CrossFromLeft()
        {
            while (true)
            {
                Console.WriteLine("About to cross From Left ");
                WaitUntilSafeToCross(0);
                Console.WriteLine("Crossing From Left ");
                DoneWithCrossing(0);
                Console.WriteLine("Done From Left ");

            }
        }

        void CrossFromRight()
        {
            while (true)
            {
                Console.WriteLine("About to cross From Right ");
                WaitUntilSafeToCross(1);
                Console.WriteLine("Crossing From Right ");
                DoneWithCrossing(1);
                Console.WriteLine("Done From Right ");

            }
        }
        /// <summary>
        /// Im not sure what exactly the out put should show
        /// 1) it should never say Left or Right more than 5 times in a row
       
        /// </summary>
        public static void Run()
        {

            //block the baboons trying to cross in this direction
            Semaphore[] mutex = new Semaphore[2] {
            new Semaphore(1,1),
            new Semaphore(1,1)
        };  
            Semaphore max_on_rope = new Semaphore(5, 5); 
            Semaphore rope = new Semaphore(1, 1);
            Semaphore order = new Semaphore(1, 1);

            Thread t1 = new Thread(new Baboon3(mutex, max_on_rope, rope,order).CrossFromLeft);
            t1.Start();
            t1 = new Thread(new Baboon3(mutex, max_on_rope, rope, order).CrossFromLeft);
            t1.Start();
            t1 = new Thread(new Baboon3(mutex, max_on_rope, rope, order).CrossFromLeft);
            t1.Start();

            t1 = new Thread(new Baboon3(mutex, max_on_rope, rope, order).CrossFromRight);
            t1.Start();
            t1 = new Thread(new Baboon3(mutex, max_on_rope, rope, order).CrossFromRight);
            t1.Start();
            t1 = new Thread(new Baboon3(mutex, max_on_rope, rope, order).CrossFromRight);
            t1.Start();
        }

    }


}
