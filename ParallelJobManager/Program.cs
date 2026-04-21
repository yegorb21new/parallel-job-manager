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
                new InputDef(1, "AnnBase_", "FAS157", 2),
                new InputDef(2, "AnnBase_", "TAX", 2),
                new InputDef(3, "AnnAttrib_", "GAAP", 4),
                new InputDef(4, "AnnForecast_", "STAT107", 9),
                new InputDef(5, "AnnForecast_", "STAT133", 8)
            };

            var nodesList = new List<string>()
            {
                "ESFS_001",
                "Azure_001"
            };

            var jobsList = new List<Job>();
            var runnersList = new List<GridNode>();
            var prioritiesToRun = inputsList.Select(x => x.Priority).Distinct();
            var jobsByPriorityDict = new Dictionary<int, Queue<Job>>();

            foreach (int prio in prioritiesToRun)
            {
                jobsByPriorityDict.Add(prio, new Queue<Job>());
            }

            foreach (var item in inputsList)
            {
                var currJob = new Job(item.Id, item, 750);
                var currPrio = item.Priority;

                jobsByPriorityDict[currPrio].Enqueue(currJob);
                jobsList.Add(currJob);
            }


            int numJobs = jobsList.Count;

            foreach (var node in nodesList)
            {
                runnersList.Add(new GridNode(node));
            }

            var scheduler = new Scheduler(jobsByPriorityDict, runnersList);
            await scheduler.BeginOrchestration();


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
