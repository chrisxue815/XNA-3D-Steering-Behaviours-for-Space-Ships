using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace Steering
{

    class PathFinder
    {
        Dictionary<Vector3, Node> open = new Dictionary<Vector3, Node>();
        PriorityQueue<Node> openPQ = new PriorityQueue<Node>();

        Dictionary<Vector3, Node> closed = new Dictionary<Vector3, Node>();

        Vector3 start, end;
        Model sphere;

        public Path findPath(Vector3 start, Vector3 end)
        {
            long oldNow = DateTime.Now.Ticks;
            bool found = false;
            this.end.X = (float)Math.Round(start.X); this.end.Y = (float)Math.Round(start.Y); this.end.Z = (float)Math.Round(start.Z);
            this.start.X = (float)Math.Round(end.X); this.start.Y = (float)Math.Round(end.Y); this.start.Z = (float)Math.Round(end.Z);

            open.Clear();
            closed.Clear();

            Node first = new Node();
            first.f = first.g = first.h = 0.0f;
            first.pos = this.start;
            open[this.start] = first;
            openPQ.Enqueue(first);

            Node current = first;
            while (open.Count > 0)
            {

                current = openPQ.Dequeue();
                float min = current.f;

                /*
                // Get the top of the q
                float min = float.MaxValue;
                foreach (Node node in open.Values)
                {
                    if (node.f < min)
                    {
                        current = node;
                        min = node.f;
                    }                    
                }
                */
                if (current.pos.Equals(this.end))
                {
                    found = true;
                    break;
                }                
                addAdjacentNodes(current);
                open.Remove(current.pos);
                closed[current.pos] = current;
            }
            Path path = new Path();
            if (found)
            {
                while (!current.pos.Equals(this.start))
                {
                    path.Waypoints.Add(current.pos);
                    current = current.parent;
                }
                path.Waypoints.Add(current.pos);
            }
            long elapsed = DateTime.Now.Ticks - oldNow;
            System.Console.WriteLine("A * took: " + (elapsed / 10000) + " milliseconds");
            SmoothPath(path);
            return path;
        }

        private void addAdjacentNodes(Node current)
        {
            // Forwards
            Vector3 pos;
            pos.X = current.pos.X;
            pos.Y = current.pos.Y;
            pos.Z = current.pos.Z + 1;
            addIfValid(pos, current);

            // Forwards right
            pos.X = current.pos.X + 1;
            pos.Y = current.pos.Y;
            pos.Z = current.pos.Z + 1;
            addIfValid(pos, current);

            // Right
            pos.X = current.pos.X + 1;
            pos.Y = current.pos.Y;
            pos.Z = current.pos.Z;
            addIfValid(pos, current);

            // Backwards Right
            pos.X = current.pos.X + 1;
            pos.Y = current.pos.Y;
            pos.Z = current.pos.Z - 1;
            addIfValid(pos, current);

            // Backwards
            pos.X = current.pos.X;
            pos.Y = current.pos.Y;
            pos.Z = current.pos.Z - 1;
            addIfValid(pos, current);

            // Backwards Left
            pos.X = current.pos.X - 1;
            pos.Y = current.pos.Y;
            pos.Z = current.pos.Z - 1;
            addIfValid(pos, current);

            // Left
            pos.X = current.pos.X - 1;
            pos.Y = current.pos.Y;
            pos.Z = current.pos.Z;
            addIfValid(pos, current);

            // Forwards Left
            pos.X = current.pos.X - 1;
            pos.Y = current.pos.Y;
            pos.Z = current.pos.Z + 1;
            addIfValid(pos, current); 
        }

        private bool isNavigable(Vector3 pos)
        {
            foreach (Entity entity in XNAGame.Instance().Children)
            {
                if (entity is Obstacle)
                {
                    Obstacle obstacle = (Obstacle)entity;
                    if (obstacle.isInside(pos, 2.0f))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void addIfValid(Vector3 pos, Node parent)
        {
	
	        if ((isNavigable(pos)))
	        {		
		        if (!closed.ContainsKey(pos)) 			
		        {			        
			        if (!open.ContainsKey(pos))		
			        {
				        Node node = new Node();
				        node.pos = pos;
				        node.g = parent.g +  cost(node.pos, parent.pos);
				        node.h = heuristic(pos, end);
				        node.f = node.g + node.h;
				        node.parent = parent;
                        openPQ.Enqueue(node);
				        open[pos] = node;
			        }		
			        else 
			        {
                        // Edge relaxation?
                        Node node = open[pos];
				        float g = parent.g +  cost(node.pos, parent.pos);			
				        if (g < node.g)
				        {
					        node.g = g;
					        node.f = node.g + node.h;
					        node.parent = parent;
				        }
			        }
		        }
	        }
        }

        public void SmoothPath(Path path)
        {
            List<Vector3> wayPoints = path.Waypoints;

            if (wayPoints.Count < 3)
            {
                return;
            }

            int current;
            int middle;
            int last;
            int temp;

            current = 0;
            middle = current + 1;
            last = current + 2;

            while (last != wayPoints.Count)
            {

                Vector3 point0, point2;

                point0 = wayPoints[current];
                point2 = wayPoints[last];
                point0.Y = 0;
                point2.Y = 0;
                if ((RayTrace(point0, point2)))
                {
                    current++;
                    middle++;
                    last++;

                }
                else
                {
                    wayPoints.RemoveAt(middle);                    
                }
            }
        }

        private float heuristic(Vector3 v1, Vector3 v2)
        {
            return 10.0f * (Math.Abs(v2.X - v1.X) + Math.Abs(v2.Z - v1.Z));
        }

        private float cost(Vector3 v1, Vector3 v2)
        {
            int dist = (int) Math.Abs(v2.X - v1.X) + (int) Math.Abs(v2.Z - v1.Z);
	        return (dist == 1) ? 10 : 14;
        }

        private bool RayTrace(Vector3 point0, Vector3 point1)
        {
            List<Entity> list = XNAGame.Instance().Children;
            foreach (Entity entity in list)
            {
                if (entity is Obstacle)
                {
                    Obstacle o = (Obstacle)entity;
                    Ray ray = new Ray();
                    ray.look = point1 - point0;
                    ray.look.Normalize();
                    ray.pos = point0;
                    Vector3 intersectionPoint = new Vector3();
                    if (o.closestRayIntersects(ray, point0, ref intersectionPoint))
                    {
                        float dist = (intersectionPoint - point0).Length();
                        float rayLength = (point1 - point0).Length();
                        if (dist < rayLength)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
