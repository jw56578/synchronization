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
    /// Suppose there are N passenger threads and one car thread. 
    /// The passengers repeatedly wait to take rides in the car, which can hold C passengers, where C < N. 
    /// The car can go around the tracks only when it is full.Here are some additional details: 
    //Passengers should invoke board and unboard.
    //The car should invoke load, run and unload.
   // Passengers cannot board until the car has invoked load
   // The car cannot depart until C passengers have boarded.
//Passengers cannot unboard until the car has invoked unload.
   // Puzzle: Write code for the passengers and car that enforces these constraints.

    /// 
    /// 
    /// </summary>
    public class RollerCoaster
    {
        static int N = 8;
        static int C = 4;
        //this semaphore needs to start at 0
        Semaphore unloadSem;
        Semaphore loadSem;
        Semaphore waitForUnload;
        //mutexes are only used to lock critical areas
        //they are not used to syncronize execution between threads doing different things
        Mutex mutex;
        //this semaphore
        Semaphore carSema;
        static int count = 0;
        static int provereadersdonotblockeachother = 0;
        static Dictionary<int, int> numbersalreadycreated = new Dictionary<int, int>();
        int id;

        public RollerCoaster(Semaphore unloadSem, Semaphore carSema,Semaphore load, Semaphore waitForUnload, Mutex mutex,int id)
        {
            this.unloadSem = unloadSem;
            this.carSema = carSema;
            this.mutex = mutex;
            this.loadSem  = load;
            this.waitForUnload = waitForUnload;
            this.id = id;
        }
        /// <summary>

        /// </summary>
        public void Passenger()
        {

            while (true)
            {
                //i am ready to board but i cannot board until the car is ready for loading
                //3 threads could reach this point before the car loads, 
                //how does car block those threads without blocking itself
                //i think that passenger has to block itself
                //1) all passenger threads are blocked by this semaphore that is ZERO
                loadSem.WaitOne();
                Console.Write("Board - ");
                //3) passengers have been allowed to board
                //4) passengers must board one at a time so they can be counted
                mutex.WaitOne();
                count++;
                if (count == C)
                {
                    //the car is full, we need to release the car for departures
                    //release the Car for departing
                    carSema.Release();

                }
                mutex.ReleaseMutex();

                //we cannot unboard until the car calls unload 
                //this is like a roller coaster has come to a stop and safe to get out
                //how do i block here, do I need another 
                //i can't reuse the same semaphore because its already released to let all passengers through
                //there must be a seperate semaphore for each instance of having to wait for somoething
                unloadSem.WaitOne();

                //if we get to this line we can start unloading
                //once all are unloaded, let the car continue to do its thing
                mutex.WaitOne();
                Console.Write("Unboard - ");
                count--;
                if (count == 0)
                {
                    waitForUnload.Release();

                }
                mutex.ReleaseMutex();

              
            }

        }

        /// <summary>

        /// </summary>
        public void Car()
        {
            while (true)
            {
                //I am ready to take passengers in
                //i need to block on a semaphore 
                Console.WriteLine("");
                Console.Write("Load - ");
                //2) release C passengers to LOAD up
                for (int i = 0; i < C; i++) {
                    loadSem.Release();
                }
                //i need to block untill all the passengers are loaded
                carSema.WaitOne();
               

                //I am full time to go
                Console.Write("Departing - ");

                //Ride is done, get out
                Console.Write("Unload - ");
                //5) depart and unload happen in series, you depart whateve happens then you unload

 
                //okay now all passengers can unload
                for (int i = 0; i < C; i++)
                {
                    unloadSem.Release();

                }

                //need to block and wait till all passengers have unloaded
                waitForUnload.WaitOne();
                
                Console.WriteLine("Done");

                
                
            }
        }
        /// <summary>
        /// whilst running we should see a constant stream of output
        /// Load - Board - Board - Board - Board - Departing - Unload - Unboard - Unboard - Unboard - Unboard
        /// Load - Board - Board - Board - Board - Departing - Unload - Unboard - Unboard - Unboard - Unboard
        /// </summary>
        public static void Run()
        {

            //we do not want the passenger to be allowed to do anything until the car loads
            //so you start he semaphore at 0 so nothing can be done
            //but then the Car will release this lock after it loads
            //the car needs to release the lock for as many passengers as it can handle C
            var unloadSem = new Semaphore(0, C);
            var loadSem = new Semaphore(0, C);
            var car = new Semaphore(0, 1);
            var mutex = new Mutex();
            var waitForUnload = new Semaphore(0,1);


            Thread t1 = new Thread(new RollerCoaster(unloadSem, car,loadSem, waitForUnload, mutex,1).Car);
            t1.Start();


            for (int i = 0; i < N; i++)
            {

                Thread t2 = new Thread(new RollerCoaster(unloadSem, car, loadSem, waitForUnload, mutex, i).Passenger);
                t2.Start();
            }
     



            t1.Join();


        }
    }
}
