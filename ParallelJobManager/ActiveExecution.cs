using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelJobManager
{
    public class ActiveExecution
    {
        public GridQueue Queue { get; set; }
        public Job Job { get; set; }
        public Task Task { get; set; }

        public ActiveExecution(GridQueue queue, Job job, Task task)
        {
            this.Queue = queue;
            this.Job = job;
            this.Task = task;
        }

    }
}
