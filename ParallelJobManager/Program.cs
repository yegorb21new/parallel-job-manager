using ParallelJobManager;
using System.CommandLine;

namespace ParallelJobManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var inputsList = new List<InputDef>()
            {
                new InputDef(1, "AnnBase_", "FAS157"),
                new InputDef(2, "AnnBase_", "TAX"),
                new InputDef(3, "AnnAttrib_", "GAAP"),
                new InputDef(4, "AnnForecast_", "STAT107"),
                new InputDef(5, "AnnForecast_", "STAT133")
            };

            var queuesList = new List<string>()
            {
                "ESFS_001",
                "Azure_001"
            };

            var jobQueue = new Queue<Job>();
            int jobId = 1;
            int completedJobs = 0;
            var runnersList = new List<GridQueue>();

            foreach (var item in inputsList)
            {
                jobQueue.Enqueue(new Job(jobId, item, 750));
                jobId++;
            }

            int numJobs = jobQueue.Count;

            foreach (var queue in queuesList)
            {
                runnersList.Add(new GridQueue(queue));
            }

            foreach (var gridQ in runnersList)
            {
                var nextJob = jobQueue.Dequeue();
                gridQ.CurrentJob = nextJob;
                gridQ.Run();
                completedJobs++;
            }

            while (completedJobs < numJobs)
            {
                foreach (var gridQ in runnersList)
                {
                    if (gridQ.IsAvailable && jobQueue.Count > 0)
                    {
                        var nextJob = jobQueue.Dequeue();
                        gridQ.CurrentJob = nextJob;
                        gridQ.Run();
                        completedJobs++;
                    }
                }

                System.Threading.Thread.Sleep(3000);
            }


            Console.WriteLine($"[{DateTime.Now}]: All jobs finished.");
        }
    }

    public static class Helpers
    {
        public static Queue<Job> CreateJobQueue(List<Job> jobList)
        {
            var jobQueue = new Queue<Job>();



            return jobQueue;
        }

        public static string CombineCLArgs(string[] args)
        {
            string combinedStr = string.Join(" ", args);

            return combinedStr;
        }

        public static List<Job> CreateJobList(string combinedArgs)
        {
            var jobList = new List<Job>();

            combinedArgs = combinedArgs.Trim();

            var inputToList = combinedArgs.Split("/id");

            return jobList;
        }

        public enum JobStatus
        {
            Unknown = -1,
            Pending = 0,
            Running,
            Success,
            Failure
        }
    }
}
