using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelJobManager
{
    public static class Helpers
    {
        public enum JobStatus
        {
            Unknown = -1,
            Pending = 0,
            Running,
            Success,
            Failure
        }


        public static Job GetNextJobByPriority(Dictionary<int, Queue<Job>> fullPrioJobsDict)
        {
            var listOfPriosInOrder = fullPrioJobsDict.Keys.ToList();
            listOfPriosInOrder.Sort();

            foreach (int priority in listOfPriosInOrder)
            {
                var currPrioJobQueue = fullPrioJobsDict[priority];
                if (currPrioJobQueue.Count > 0)
                {
                    var nextJob = currPrioJobQueue.Dequeue();
                    return nextJob;
                }
            }

            throw new InvalidOperationException("Scheduler should not call GetNextJobByPriority when there are no Jobs left.");
        }
    }
}
