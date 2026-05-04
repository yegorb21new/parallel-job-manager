using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static ParallelJobManager.Helpers;

namespace ParallelJobManager
{
    public class Scheduler
    {
        public Dictionary<int, Queue<Job>> PendingJobsByPriority { get; private set; }
        public List<GridNode> AllNodes { get; private set; }
        public List<ActiveExecution> ActiveExecutions { get; set; }

        public Scheduler(Dictionary<int, Queue<Job>> jobsByPriority, List<GridNode> allNodes)
        {
            this.PendingJobsByPriority = jobsByPriority;
            this.AllNodes = allNodes;
            this.ActiveExecutions = new List<ActiveExecution>();
        }

        public async Task BeginOrchestration()
        {
            while (PendingJobsByPriority.Values.Any(v => v.Count > 0) || this.ActiveExecutions.Count > 0)
            {
                foreach (var node in AllNodes)
                {
                    if (node.IsAvailable && PendingJobsByPriority.Values.Any(v => v.Count > 0))
                    {
                        var nextJob = GetNextJobByPriority(PendingJobsByPriority);
                        node.CurrentJob = nextJob;
                        nextJob.AttemptCount += 1;

                        var currExecution = new ActiveExecution(node, nextJob);
                        Console.WriteLine($"[{DateTime.Now}]: Dispatching Job number {nextJob.Id} to master node {node.NodeName} for attempt {nextJob.AttemptCount} of {nextJob.Input.MaxAttempts} max attempts.");

                        ActiveExecutions.Add(currExecution);
                        currExecution.Task = Task.Run(currExecution.ExecuteAsyncJob);
                    }
                }

                if (ActiveExecutions.Any())
                {
                    var finishedTask = await Task.WhenAny(ActiveExecutions.Select(x => x.Task));
                    var finishedExecution = ActiveExecutions.Where(x => x.Task == finishedTask).FirstOrDefault();
                    var finishedJob = finishedExecution.Job;

                    var finishedJobRetryPolicy = finishedJob.Input.RetryPolicy;
                    int maxTries = finishedJob.Input.MaxAttempts;

                    if (finishedJob.Status == JobStatus.Failure && finishedJob.AttemptCount < maxTries)
                    {
                        finishedJob.AttemptCount += 1;

                        finishedExecution.Task = Task.Run(finishedExecution.ExecuteAsyncJob);

                        Console.WriteLine($"[{DateTime.Now}]: Immediately retrying Job number {finishedJob.Id} on master node {finishedExecution.GridNode.NodeName} for attempt {finishedJob.AttemptCount} of {maxTries} max attempts.");
                    }
                    else if (finishedJob.Status == JobStatus.Success || finishedJob.AttemptCount >= maxTries)
                    {
                        finishedExecution.GridNode.CurrentJob = null;
                        ActiveExecutions.Remove(finishedExecution);
                    }
                }
            }
        }
    }
}
