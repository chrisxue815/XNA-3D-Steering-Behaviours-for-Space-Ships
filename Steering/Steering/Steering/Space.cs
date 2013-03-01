using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Steering
{
    public class Space
    {
        List<Cell> cells = new List<Cell>();

        public List<Cell> Cells
        {
            get { return cells; }
            set { cells = value; }
        }

        public Space()
        {
            // generate the list of cells
            float cellWidth = Params.GetFloat("cell_width");
            float range = Params.GetFloat("world_range") + Params.GetFloat("cell_width");
            float worldWidth = range * 2;

            int num = 0;

            //for (float y = -range; y < range; y+= cellWidth)
            float y = 0;
            {
                for (float z = -range; z < range; z += cellWidth)
                {
                    for (float x = -range; x < range; x += cellWidth)
                    {
                        Cell cell = new Cell();
                        cell.Box.Min = new Vector3(x, y, z);
                        cell.Box.Max = new Vector3(x + cellWidth, y + cellWidth, z + cellWidth);
                        cell.Number = num++;
                        cells.Add(cell);
                    }
                }
            }
            // Set up adjacent cells
            foreach (Cell cell in cells)
            {
                BoundingBox expanded = cell.Box;
                Vector3 border = new Vector3(10, 10, 10);
                expanded.Min -= border;
                expanded.Max += border;
                foreach (Cell otherCell in cells)
                {
                    if (expanded.Intersects(otherCell.Box))
                    {
                        cell.Adjacent.Add(otherCell);
                    }
                }
            }            
        }

        public int FindCell(Vector3 pos)
        {
            pos.Y = 0;
            foreach (Cell cell in cells)
            {
                if (cell.Contains(pos))
                {
                    return cell.Number;
                }
            }
            return -1;
        }

        public void Partition()
        {
            List<Entity> entities = XNAGame.Instance().Children;
            foreach (Cell cell in cells)
            {
                cell.Entities.Clear();
            }
            foreach (Entity entity in entities)
            {
                if (entity is Fighter)
                {
                    int cell = FindCell(entity.pos);
                    if (cell != -1)
                    {
                        cells[cell].Entities.Add(entity);
                    }
                }
            }
        }
    }
}
