using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Steering.StarTrek
{
    public class HighSpeedTrail : Entity
    {
        private const float Radius = 20f;
        private const float Speed = 20f;
        private const int MaxNumLines = 100;
        private const float MaxLength = 20f;
        private const float Forward = 100f;
        private const float MaxTimeToNextTrail = 0.5f;

        private XNAGame Game { get; set; }

        private Vector3 Origin { get; set; }
        private Vector3 Direction { get; set; }
        private VertexPositionNormalTexture[] PointList { get; set; }
        private short[] LineListIndices { get; set; }

        private Random Random { get; set; }
        private float TimeToNextTrail { get; set; }

        public HighSpeedTrail(Vector3 origin, Vector3 direction)
        {
            Game = XNAGame.Instance();

            Origin = origin;
            Direction = direction;
            Direction.Normalize();
            PointList = new VertexPositionNormalTexture[MaxNumLines * 2];
            LineListIndices = new short[MaxNumLines * 2];

            Random = new Random();
            TimeToNextTrail = MaxTimeToNextTrail;
        }

        public override void LoadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            var elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var startPoint = Origin - Forward * Direction;

            for (var i = 0; i < MaxNumLines; i++)
            {
                var length = (float)Random.NextDouble() % MaxLength;
                var pos = new Vector3();
                PointList[i] = new VertexPositionNormalTexture(pos, Vector3.Forward, new Vector2());
            }

            for (var i = 0; i < MaxNumLines; i++)
            {
                var pointPos = PointList[i].Position + elapsedSeconds * Speed * Direction;
                PointList[i] = new VertexPositionNormalTexture(pointPos, Vector3.Forward, Vector2.Zero);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                PrimitiveType.LineList,
                PointList,
                0,  // vertex buffer offset to add to each element of the index buffer
                9,  // number of vertices in pointList
                LineListIndices,  // the index buffer
                0,  // first index element to read
                8   // number of primitives to draw
            );
        }

        public override void UnloadContent()
        {
        }
    }
}
