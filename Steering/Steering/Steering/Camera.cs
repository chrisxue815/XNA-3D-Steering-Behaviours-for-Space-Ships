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
    public class Camera : Entity
    {
        public Matrix projection;
        public Matrix view;
        private KeyboardState keyboardState;
        private MouseState mouseState;

        private int perspective = 1;
        private bool wasLeftButtonPressed = false;
        private Vector3 oldPosition;
        private Vector3 oldLook;
        private Vector3 oldUp;

        public Camera()
        {
            pos = new Vector3(0.0f, 30.0f, 50.0f);
            look = new Vector3(0.0f, 0.0f, -1.0f);
        }

        public override void LoadContent()
        {
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            float timeDelta = (float)(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);

            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            int mouseX = mouseState.X;
            int mouseY = mouseState.Y;

            int midX = GraphicsDeviceManager.DefaultBackBufferHeight / 2;
            int midY = GraphicsDeviceManager.DefaultBackBufferWidth / 2;

            int deltaX = mouseX - midX;
            int deltaY = mouseY - midY;

            CameraYaws(-deltaX / 100.0f);
            CameraPitches(-deltaY / 100.0f);
            Mouse.SetPosition(midX, midY);

            bool leftButtonPressed = (mouseState.LeftButton == ButtonState.Pressed);

            if (!wasLeftButtonPressed && leftButtonPressed)
            {
                Vector3 newTargetPos = pos + (look * 100.0f);
                XNAGame.Instance().Leader.Path.Waypoints.Add(newTargetPos);
            }

            wasLeftButtonPressed = leftButtonPressed;

            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                timeDelta *= 20.0f;
            }

            if (perspective != 1 && keyboardState.IsKeyDown(Keys.D1))
            {
                pos = oldPosition;
                look = oldLook;
                up = oldUp;
                perspective = 1;
            }
            if (perspective != 2 && keyboardState.IsKeyDown(Keys.D2))
            {
                oldPosition = pos;
                oldLook = look;
                oldUp = up;
                perspective = 2;
            }

            switch (perspective)
            {
                case 1:
                    if (keyboardState.IsKeyDown(Keys.W))
                    {
                        walk(timeDelta);
                    }
                    if (keyboardState.IsKeyDown(Keys.S))
                    {
                        walk(-timeDelta);
                    }
                    if (keyboardState.IsKeyDown(Keys.A))
                    {
                        strafe(-timeDelta);
                    }
                    if (keyboardState.IsKeyDown(Keys.D))
                    {
                        strafe(timeDelta);
                    }
                    break;
                case 2:
                    var leader = XNAGame.Instance().Leader;
                    pos = leader.pos - leader.look * 10;
                    look = leader.look;
                    up = leader.up;
                    break;
            }
            view = Matrix.CreateLookAt(pos, pos + look, up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), XNAGame.Instance().GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio, 1.0f, 10000.0f);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteFont spriteFont = XNAGame.Instance().SpriteFont;
            XNAGame.Instance().SpriteBatch.DrawString(spriteFont, "Pos: " + pos.X + " " + pos.Y + " " + pos.Z, new Vector2(500, 10), Color.White);
            XNAGame.Instance().SpriteBatch.DrawString(spriteFont, "Look: " + look.X + " " + look.Y + " " + look.Z, new Vector2(500, 30), Color.White);
            XNAGame.Instance().SpriteBatch.DrawString(spriteFont, "Right: " + right.X + " " + right.Y + " " + right.Z, new Vector2(500, 50), Color.White);
            XNAGame.Instance().SpriteBatch.DrawString(spriteFont, "Up: " + up.X + " " + up.Y + " " + up.Z, new Vector2(500, 70), Color.White);
        }

        public void CameraYaws(float angle)
        {
            var T = Matrix.CreateRotationY(angle);
            look = Vector3.Transform(look, T);
            up = Vector3.Transform(up, T);
            right = Vector3.Transform(right, T);
        }

        public void CameraPitches(float angle)
        {
            var T = Matrix.CreateFromAxisAngle(right, angle);
            up = Vector3.Transform(up, T);
            if (up.Y < 0) up.Y = 0;
            look = Vector3.Cross(up, right);
        }

        public Matrix getProjection()
        {
            return projection;
        }

        public Matrix getView()
        {
            return view;
        }
    }
}
