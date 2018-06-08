using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Synchronization
{
    public class Philosopher
    {
        static int THINKING = 1;
        static int HUNGRY = 2;
        static int EATING = 3;
        static int N = 5;

        int LEFT(int i)
        {
            return (i + N - 1) % N;
        }
        int RIGHT(int i)
        {
            return (i + 1) % N;
        }

        int index;
        Semaphore mutex;
        Semaphore[] s;
        int[] state;

        public Philosopher(int index, Semaphore mutex, Semaphore[] s, int[] state)
        {
            this.index = index;
            this.mutex = mutex;
            this.s = s;
            this.state = state;
        }

        public void Think() {
            //thinking is when you are not doing anything
            Console.WriteLine("philosopher " + index + " is thinking");
        }
        public void Eat()
        {
            //eating means doing something
            Console.WriteLine("philosopher " + index + " is eating");
        }

        public void Run() {
            while (true)
            {
                Think();
                TakeForks();
                Eat();
                PutForks();

            }
        }
        void TakeForks()
        {
            mutex.WaitOne();
            state[index] = HUNGRY;
            Test(index);
            mutex.Release();
            s[index].WaitOne();

        }
        void PutForks()
        {
            mutex.WaitOne();
            state[index] = THINKING;
            Test(LEFT(index));
            Test(RIGHT(index));
            mutex.Release();
        }
        void Test(int i)
        {
            if(state[i] == HUNGRY && state[LEFT(i)] != EATING && state[RIGHT(i)] != EATING)
            {
                state[i] = EATING;
                s[i].Release();
            }
        }

        public static void GO()
        {
            Semaphore mutex = new Semaphore(1,1);
            Semaphore[] s = new Semaphore[N];

            for (int i = 0; i < N; i++) {
                s[i] = new Semaphore(0, 1);
            }

            int[] state = new int[N];
            for (int i = 0; i < N; i++)
            {
                Philosopher p = new Philosopher(i, mutex, s, state);
                Thread t1 = new Thread(p.Run);
                t1.Start();
            }
           


        }
    }
}
