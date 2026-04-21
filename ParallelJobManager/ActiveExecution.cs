using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelJobManager
{
    public class ActiveExecution
    {
        public GridNode GridNode { get; set; }
        public Job Job { get; set; }
        public Task Task { get; set; }
        private static readonly Random _random = new();

        public ActiveExecution(GridNode node, Job job)
        {
            this.GridNode = node;
            this.Job = job;
            this.Task = null;
        }

        public bool ExecuteAsyncJob()
        {
            var currJob = Job;

            if (currJob == null)
            {
                throw new InvalidOperationException($"No job exists on {GridNode.NodeName} to run.");
            }

            int runTime = _random.Next(300, 1000);
            currJob.Status = Helpers.JobStatus.Running;

            Console.WriteLine($"[{DateTime.Now}]: Job number {currJob.Id} Status={currJob.Status} on master node {GridNode.NodeName}");
            System.Threading.Thread.Sleep(runTime);

            int result = _random.Next(0, 99);
            currJob.ExitCode = result;

            bool success = result < 90;
            currJob.Status = success ? Helpers.JobStatus.Success : Helpers.JobStatus.Failure;

            Console.WriteLine($"[{DateTime.Now}]: Job number {currJob.Id} on master node {GridNode.NodeName} ran for {runTime} ms and finished with result={currJob.ExitCode}. Status={currJob.Status}");

            return success;
        }
    }
}
