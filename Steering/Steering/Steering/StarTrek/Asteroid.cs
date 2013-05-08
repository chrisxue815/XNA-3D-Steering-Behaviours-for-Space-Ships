using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Steering
{
    public class Asteroid : Obstacle
    {
        private Game Game { get; set; }

        public Asteroid(float radius) : base(radius)
        {
            Game = XNAGame.Instance();
        }

        public override void LoadContent()
        {
            model = Game.Content.Load<Model>("StarTrek/asteroid1");
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            worldTransform = Matrix.CreateScale(radius) * Matrix.CreateTranslation(pos);

            if (model != null)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.World = worldTransform;
                        effect.DiffuseColor = Color;
                        effect.Projection = XNAGame.Instance().Camera.getProjection();
                        effect.View = XNAGame.Instance().Camera.getView();
                    }
                    mesh.Draw();
                }
            }  
        }

        public override void UnloadContent()
        {
        }
    }
}
