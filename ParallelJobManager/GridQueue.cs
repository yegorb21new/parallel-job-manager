using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelJobManager
{
    public class GridQueue
    {
        public Job CurrentJob { get; set; }
        public string QueueName { get; private set; }
        public bool IsAvailable => CurrentJob == null;

        private static readonly Random _random = new();

        public GridQueue(string queue)
        {
            this.CurrentJob = null;
            this.QueueName = queue;
        }

        public bool Run()
        {
            var job = CurrentJob;

            if (job == null)
            {
                throw new InvalidOperationException($"No job exists on {QueueName} to run.");
            }

            int runTime = _random.Next(3000, 10000);
            job.Status = Helpers.JobStatus.Running;

            Console.WriteLine($"[{DateTime.Now}]: Job number {job.Id} Status={job.Status} on queue {QueueName}");
            System.Threading.Thread.Sleep(runTime);

            int result = _random.Next(0, 99);
            job.ExitCode = result;

            bool success = result < 90;
            job.Status = success ? Helpers.JobStatus.Success : Helpers.JobStatus.Failure;

            Console.WriteLine($"[{DateTime.Now}]: Job number {job.Id} on queue {QueueName} ran for {runTime / 1000} seconds and finished with result={result}. Status={job.Status}");


            CurrentJob = null;
            return success;
        }
    }
}
