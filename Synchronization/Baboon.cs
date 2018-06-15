using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Synchronization;
using System.Threading;

namespace Synchronization
{
    /*
        Question 1) Baboon Crossing Synchronization Problem
        There is a deep canyon somewhere in Kruger National Park, South Africa, and a single rope
        that spans the canyon.Baboons can cross the canyon by swinging hand-over-hand on the rope,
        but if two baboons going in opposite directions meet in the middle, they will fight and drop to
        their deaths.Furthermore, the rope is only strong enough to hold 5 baboons.If there are more
        baboons on the rope at the same time, it will break. Assuming that we can teach the baboons to
        use semaphores, we would like to design a synchronization scheme with the following
        properties:
        (a) Once a baboon has begun to cross, it is guaranteed to get to the other side without
        running into a baboon going the other way.
        (b) There are never more than 5 baboons on the rope.
        (c) A continuing stream of baboons crossing in one direction should not bar baboons going
        the other way indefinitely(no starvation).
    */

    /*
    how do you indicate that a baboon is going in a certain direction
    how do you know if there is already a baboon on the rope going in a direction

    semaphore for crossing - allow 0 resources
    if an opposite baboon sees that there is a baboon crossing, then block on this semaphore

     */

    public class Baboon
    {

        Semaphore blockLeft;
        Semaphore blockRight;

        Mutex mutex;

        static int goingRight = 0;
        static int goingLeft = 0;
        static int waitingToGoRight = 0;
        static int waitingToGoLeft = 0;
        int myDirection = 0;
        static int numberThatHaveCrossed = 0;


        public Baboon(int direction, Semaphore blockLeft,Semaphore blockRight, Mutex mutex)
        {
            this.myDirection = direction;
            this.blockLeft = blockLeft;
            this.blockRight = blockRight;
            this.mutex = mutex;
        }

        /// <summary>

        /// </summary>
        public void Cross()
        {

            while (true)
            {
                mutex.WaitOne();
              
                // Console.WriteLine("Baboon wants to Cross: " + (mydirection == 0 ? "Left" : "Right"));
                //why the hell are the left threads getting blocked when they don't call crossing.WaitOne()
                //because you were blocking the Right threads and then that stopped them from releasing the Mutex
                if (myDirection == 0)
                {
                    //going left is never getting up to 5 because they are crossing to fast
                    //still need a solution for breaking the flow and allowing threads on the right to get through
                    //what is this solution?
                    //how do you make a determination that it is time for right to go
                    //should there just be a counter for how many lefts have gone???
                    if (numberThatHaveCrossed >= 5 || goingRight > 0)
                    {
                        numberThatHaveCrossed = 0;
                        //how does this indicate that the other direction can now go
                        //this logic probably has to go after crossing has been done
                        waitingToGoLeft++;
                        //this needs to be released before you block the semaphore or deadlock occurs
                        mutex.ReleaseMutex();
                        blockLeft.WaitOne();
                       
                       
                    }
                    else
                    {
                        goingLeft++;
                        numberThatHaveCrossed++;
                        mutex.ReleaseMutex();
                        
                    }
                }
                else
                {
                    
                    if (numberThatHaveCrossed >= 5 || goingLeft > 0)
                    {
                        numberThatHaveCrossed = 0;
                        waitingToGoRight++;
                        //this needs to be released before you block the semaphore or deadlock occurs
                        mutex.ReleaseMutex();
                        blockRight.WaitOne();
                       
                    }
                    else
                    {
                        goingRight++;
                        numberThatHaveCrossed++;
                        mutex.ReleaseMutex();
                       
                    }
                }

                Console.WriteLine("Crossing " + myDirection);


                mutex.WaitOne();
                if (myDirection == 0)
                {
                    goingLeft--;
                    if (goingLeft == 0 && waitingToGoRight >  0)
                    {
                        waitingToGoRight--;
                        blockRight.Release();
                    }
                }
                else
                {
                    goingRight--;
                    if (goingRight == 0 && waitingToGoLeft > 0)
                    {
                        waitingToGoLeft--;
                        blockLeft.Release();
                    }

                }
                mutex.ReleaseMutex();


            }

        }
        /// <summary>
        /// 
        /// this should show an output of 
        /// Crossing: Right or Left in a series of 5 in a row
        /// it should not keep saying Right and never left or visa vera indicating starvation
        /// </summary>
        public static void Run()
        {


            var blockLeft = new Semaphore(0, 5);
            var blockRight = new Semaphore(0, 5);
            var mutex = new Mutex();


            for (int i = 0; i < 3; i++)
            {
                Thread t1 = new Thread(new Baboon(0,blockLeft, blockRight, mutex).Cross);
                t1.Start();
            }

            for (int i = 0; i < 3; i++)
            {
                Thread t1 = new Thread(new Baboon(1, blockLeft, blockRight, mutex).Cross);
                t1.Start();
            }




        }
    }
}
