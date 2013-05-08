using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Steering
{
    public class MoviePlayer : Entity
    {
        private XNAGame Game { get; set; }
        private Video Video { get; set; }
        private VideoPlayer VideoPlayer { get; set; }
        private Texture2D Texture { get; set; }
        public TimeSpan PlayPosition { get { return VideoPlayer.PlayPosition; } }

        public MoviePlayer()
        {
            Game = XNAGame.Instance();
        }

        public override void LoadContent()
        {
            if (Video == null)
            {
                Video = Game.Content.Load<Video>("StarTrek/movie");
                VideoPlayer = new VideoPlayer();
                VideoPlayer.Play(Video);
            }
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            // Only call GetTexture if a video is playing or paused
            if (VideoPlayer.State != MediaState.Stopped)
                Texture = VideoPlayer.GetTexture();

            var width = 500;
            var height = Video.Height * width / Video.Width;
            var x = Game.GraphicsDevice.Viewport.X;
            var y = Game.GraphicsDevice.Viewport.Y + Game.GraphicsDevice.Viewport.Height - height;

            // Drawing to the rectangle will stretch the 
            // video to fill the screen
            var rect = new Rectangle(x, y, width, height);

            // Draw the video, if we have a texture to draw.
            if (Texture != null)
            {
                Game.SpriteBatch.Draw(Texture, rect, Color.White);
            }
        }

        public override void UnloadContent()
        {
        }
    }
}
