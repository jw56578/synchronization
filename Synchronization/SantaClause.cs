using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synchronization
{
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
    class SantaClause
    {
    }
}
