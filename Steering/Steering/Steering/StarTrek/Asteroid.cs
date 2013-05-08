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

        private Texture2D Texture { get; set; }

        public Asteroid(float radius) : base(radius)
        {
            Game = XNAGame.Instance();
        }

        public override void LoadContent()
        {
            //model = Game.Content.Load<Model>("StarTrek/models/asteroid1");
            model = Game.Content.Load<Model>("sphere");
            Texture = Game.Content.Load<Texture2D>("StarTrek/textures/asteroid1");
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            return;

            worldTransform = Matrix.CreateScale(radius / 1494.79f) * Matrix.CreateTranslation(pos);

            if (model != null)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.Texture = Texture;
                        effect.World = worldTransform;
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
