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

                    var finishedJobStatus = finishedJob.Status;
                    var finishedJobRetryPolicy = finishedJob.Input.RetryPolicy;
                    int maxTries = finishedJob.Input.MaxAttempts;
                    int attemps = finishedJob.AttemptCount;

                    if (finishedJobStatus == JobStatus.Failure && attemps < maxTries)
                    {
                        attemps += 1;
                        Console.WriteLine($"[{DateTime.Now}]: Immediately retrying Job number {finishedJob.Id} on master node {finishedExecution.GridNode.NodeName} for attempt {attemps} of {maxTries} max attempts.");
                        finishedExecution.ExecuteAsyncJob();
                    }

                    if (finishedJobStatus == JobStatus.Success || (attemps >= maxTries))
                    {
                        finishedExecution.GridNode.CurrentJob = null;
                        ActiveExecutions.Remove(finishedExecution);
                    }
                }
            }
        }
    }
}
