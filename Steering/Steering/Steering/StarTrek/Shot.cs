using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steering
{
    public class Shot : IComparable<Shot>
    {
        public double StartTime { get; set; }
        public Action<double> InitialAction { get; set; }
        public Action<double> Action { get; set; }

        public int CompareTo(Shot other)
        {
            return StartTime < other.StartTime ? -1 : 1;
        }
    }
}
