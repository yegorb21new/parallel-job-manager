using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelJobManager
{
    public class GridNode
    {
        public Job CurrentJob { get; set; }
        public string NodeName { get; private set; }
        public bool IsAvailable => CurrentJob == null;

        public GridNode(string nodeName)
        {
            this.CurrentJob = null;
            this.NodeName = nodeName;
        }
    }
}
