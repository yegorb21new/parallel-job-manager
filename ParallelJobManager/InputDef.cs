using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelJobManager
{
    public class InputDef
    {
        public int Id { get; private set; }
        public string Dataset { get; private set; }
        public string Batch { get; private set; }
        public int Priority { get; private set; }

        public InputDef(int id, string dataset, string batch, int priority)
        {
            this.Id = id;
            this.Dataset = dataset;
            this.Batch = batch;
            this.Priority = priority;
        }
    }
}
