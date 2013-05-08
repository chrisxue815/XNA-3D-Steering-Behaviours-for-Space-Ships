using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Steering
{
    public class SkyBox : Entity
    {
        public const float Radius = 0.2f;

        public string ModelName;

        public Texture2D Texture;
        public string TextureName;

        private XNAGame Game { get; set; }

        public SkyBox()
        {
            Game = XNAGame.Instance();
            ModelName = "StarTrek/models/skybox";
            TextureName = "StarTrek/textures/skybox";
        }

        public override void LoadContent()
        {
            model = Game.Content.Load<Model>(ModelName);
            Texture = Game.Content.Load<Texture2D>(TextureName);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            var state = new DepthStencilState { DepthBufferEnable = true };
            Game.GraphicsDevice.DepthStencilState = state;

            // Copy the transform of each bone of the skybox model into a matrix array
            var skytransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(skytransforms);

            foreach (var mesh in model.Meshes)
            {
                // Set mesh orientation, and camera projection
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.TextureEnabled = true;
                    effect.Texture = Texture;
                    effect.AmbientLightColor = new Vector3(1, 1, 1);
                    effect.World = skytransforms[mesh.ParentBone.Index] *
                                    Matrix.CreateScale(10000.0f) *
                                    Matrix.CreateTranslation(Game.Camera.pos);
                    effect.View = Game.Camera.view;
                    effect.Projection = Game.Camera.projection;
                }

                mesh.Draw();
            }
        }

        public override void UnloadContent()
        {
        }
    }
}
