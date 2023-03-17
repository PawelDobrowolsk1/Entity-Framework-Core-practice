using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBoards.Entities
{
    public class Task : WorkItem
    {
        public string Activity { get; set; }
        public decimal RemaningWork { get; set; }
    }
}
