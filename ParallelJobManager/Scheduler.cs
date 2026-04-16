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
        public List<GridNode> AllNodes { get; private set; }
        public List<ActiveExecution> ActiveExecutions { get; set; }

        public Scheduler(Queue<Job> jobQueue, List<GridNode> allNodes)
        {
            this.JobQueue = jobQueue;
            this.AllNodes = allNodes;
            this.ActiveExecutions = new List<ActiveExecution>();
        }

        public async Task BeginOrchestration()
        {
            while (this.JobQueue.Count > 0 || this.ActiveExecutions.Count > 0)
            {
                foreach (var node in AllNodes)
                {
                    if (node.IsAvailable && this.JobQueue.Count > 0)
                    {
                        var nextJob = JobQueue.Dequeue();
                        node.CurrentJob = nextJob;

                        var currExecution = new ActiveExecution(node, nextJob);
                        currExecution.Task = Task.Run(currExecution.Run);

                        ActiveExecutions.Add(currExecution);
                    }
                }

                if (ActiveExecutions.Any())
                {
                    var finishedTask = await Task.WhenAny(ActiveExecutions.Select(x => x.Task));
                    var finishedExecution = ActiveExecutions.Where(x => x.Task == finishedTask).FirstOrDefault();

                    finishedExecution.GridNode.CurrentJob = null;
                    ActiveExecutions.Remove(finishedExecution);
                }
            }
        }
    }
}
