using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ParallelJobManager.Helpers;


namespace ParallelJobManager
{
    public class Job
    {
        public int Id {  get; private set; }
        public InputDef Input { get; private set; }
        public JobStatus Status { get; set; }
        public int Cores { get; private set; }
        public int ExitCode { get; set; }
        public int AttemptCount { get; set; }

        public Job(int id, InputDef input, int cores) 
        {
            this.Id = id;
            this.Input = input;
            this.Status = JobStatus.Pending;
            this.Cores = cores;
            this.ExitCode = -1;
            this.AttemptCount = 0;
        }
    }
}
