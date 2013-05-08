using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steering
{
    public class Shot : IComparable<Shot>
    {
        public double EndTime { get; set; }
        public Action<double> InitialAction { get; set; }
        public Action<double> Action { get; set; }

        public int CompareTo(Shot other)
        {
            return EndTime < other.EndTime ? -1 : 1;
        }
    }
}
