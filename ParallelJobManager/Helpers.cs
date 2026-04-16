using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelJobManager
{
    public static class Helpers
    {
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
