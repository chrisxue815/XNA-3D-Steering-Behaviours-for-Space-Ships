using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Steering
{
    public class Cell
    {
        public BoundingBox Box = new BoundingBox();
        int number;
        List<Cell> adjacent = new List<Cell>();

        public List<Cell> Adjacent
        {
            get { return adjacent; }
            set { adjacent = value; }
        }

        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        public bool Contains(Vector3 pos)
        {
            ContainmentType type = Box.Contains(pos);
            return ((type == ContainmentType.Contains) || (type == ContainmentType.Intersects));
        }
    }
}
