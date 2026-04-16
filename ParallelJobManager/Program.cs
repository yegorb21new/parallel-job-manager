using ParallelJobManager;
using System.CommandLine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelJobManager
{
    internal class Program
    {
        static async Task Main(string[] args)
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

            var jobsList = new List<Job>();
            var jobQueue = new Queue<Job>();
            int completedJobs = 0;
            var runnersList = new List<GridQueue>();

            foreach (var item in inputsList)
            {
                var currJob = new Job(item.Id, item, 750);

                jobQueue.Enqueue(currJob);
                jobsList.Add(currJob);
            }

            int numJobs = jobQueue.Count;

            foreach (var queue in queuesList)
            {
                runnersList.Add(new GridQueue(queue));
            }

            var scheduler = new Scheduler(jobQueue, runnersList);
            await scheduler.Run();


            int successJobs = jobsList.Count(x => x.Status == Helpers.JobStatus.Success);
            int failedJobs = jobsList.Count(x => x.Status == Helpers.JobStatus.Failure);
            var listFailedIDs = jobsList
                .Where(x => x.Status == Helpers.JobStatus.Failure)
                .Select(x => x.Id);

            Console.WriteLine($"[{DateTime.Now}]: All jobs finished, see job summary below:");
            Console.WriteLine($"[{DateTime.Now}]: ======================== RUN SUMMARY ========================");
            Console.WriteLine($"[{DateTime.Now}]: Total Jobs: {numJobs}");
            Console.WriteLine($"[{DateTime.Now}]: Succeeded: {successJobs}");
            Console.WriteLine($"[{DateTime.Now}]: Failed: {failedJobs}");
            Console.Write($"[{DateTime.Now}]: ");
            Console.WriteLine(listFailedIDs.Any() ? $"Failed Job IDs: {string.Join(", ", listFailedIDs)}" : "Failed Job IDs: None");
        }
    }
}
