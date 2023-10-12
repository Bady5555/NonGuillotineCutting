using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonGuillotineCutting.Model
{
    public class Job
    {
        public Bin Bin  { get; set; }
        public Rect[] Rects { get; set; }
    }
}
