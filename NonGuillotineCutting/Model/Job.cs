using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonGuillotineCutting.Model
{
    public class Job : ICloneable
    {
        public Bin Bin  { get; set; }
        public Rect[] Rects { get; set; }

		public object Clone()
		{
			return new Job() { Bin = new Bin() { H = Bin.H, W = Bin.W }, Rects = Rects.Select(x => new Rect() { H = x.H, W = x.W }).ToArray() };
		}
	}
}
