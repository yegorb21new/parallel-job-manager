using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

                        var currExecution = new ActiveExecution(node, nextJob);
                        Console.WriteLine($"[{DateTime.Now}]: Dispatching Job number {nextJob.Id} to master node {node.NodeName}");
                        currExecution.Task = Task.Run(currExecution.ExecuteAsyncJob);

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
