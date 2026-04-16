using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelJobManager
{
    public class Scheduler
    {
        public Queue<Job> JobQueue { get; private set; }
        public List<GridQueue> UniverseQueues { get; private set; }
        public List<ActiveExecution> ActiveExecutions { get; set; }

        public Scheduler(Queue<Job> jobQueue, List<GridQueue> univQueues)
        {
            this.JobQueue = jobQueue;
            this.UniverseQueues = univQueues;
            this.ActiveExecutions = new List<ActiveExecution>();
        }

        public async Task<bool> Run()
        {

            while (this.JobQueue.Count > 0 || this.ActiveExecutions.Count > 0)
            {
                foreach (var gridQ in UniverseQueues)
                {
                    if (gridQ.IsAvailable && this.JobQueue.Count > 0)
                    {
                        var nextJob = JobQueue.Dequeue();
                        gridQ.CurrentJob = nextJob;
                        var currJobTask = Task.Run(gridQ.Run);

                        var currExecution = new ActiveExecution(gridQ, nextJob, currJobTask);

                        ActiveExecutions.Add(currExecution);
                    }
                }

                if (ActiveExecutions.Any())
                {
                    var finishedTask = await Task.WhenAny(ActiveExecutions.Select(x => x.Task));
                    var finishedExecution = ActiveExecutions.Where(x => x.Task == finishedTask).FirstOrDefault();

                    finishedExecution.Queue = null;
                    finishedExecution.Job.Status = Helpers.JobStatus.Success;
                    ActiveExecutions.Remove(finishedExecution);
                }
            }

            return false;
        }
    }
}
