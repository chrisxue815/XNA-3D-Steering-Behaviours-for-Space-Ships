using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class Obstacle : Sphere
    {
        public Obstacle(float radius)
            : base(radius)
        {
        }

        public bool isInside(Vector3 point)
        {
            return ((point - pos).Length() < Radius);
        }

        public bool isInside(Vector3 point, float border)
        {
            return ((point - pos).Length() < Radius + border);
        }
    }
}
