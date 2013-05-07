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
using System.Threading;


namespace Steering
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class XNAGame : Microsoft.Xna.Framework.Game
    {
        static XNAGame instance = null;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Fighter camFighter;
        KeyboardState oldState;
        

        public Fighter CamFighter
        {
            get { return camFighter; }
            set { camFighter = value; }
        }
        Texture2D brTexture;

        private Ground ground;

        public Ground Ground
        {
            get { return ground; }
            set { ground = value; }
        }
        SpriteFont spriteFont;
        bool useCamFighter = false;
        bool wasKeyDown = false;
        float cumulSound = 50.0f;
        Song mysong;
        float scale = 1.0f;
        public SpriteFont SpriteFont
        {
            get { return spriteFont; }
            set { spriteFont = value; }
        }
        
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }
        private Camera camera;
        List<Entity> children = new List<Entity>();

        private Queue<Entity> updateQueue = new Queue<Entity>();

        public List<Entity> Children
        {
            get { return children; }
            set { children = value; }
        }
        private Fighter fighter;

        public Fighter Leader
        {
            get { return fighter; }
            set { fighter = value; }
        }

        public static XNAGame Instance()
        {
            return instance;
        }

        Space space;

        public Space Space
        {
            get { return space; }
            set { space = value; }
        }

        private MoviePlayer MoviePlayer { get; set; }

        public XNAGame()
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.ApplyChanges();
            graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
       
                        
            // TODO: Add your initialization logic here
            camera = new Camera();

            camera.pos = new Vector3(2, 20, 50);
            int midX = GraphicsDeviceManager.DefaultBackBufferHeight / 2;
            int midY = GraphicsDeviceManager.DefaultBackBufferWidth / 2;
            Mouse.SetPosition(midX, midY);
            children.Add(camera);
            Scenario.setUpArrive();
            space = new Space();
            oldState = Keyboard.GetState();

            MoviePlayer = new MoviePlayer();
            children.Add(MoviePlayer);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()

        {

            Vector2 a = new Vector2(10, 10);
            Vector2 b = new Vector2(20, 20);

            a = b;
            b.X = 30;
            b.Y = 30;
            

            mysong = Content.Load<Song>("138");
            brTexture = Content.Load<Texture2D>("BuckRogersDVD-box");

            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = XNAGame.Instance().Content.Load<SpriteFont>("Verdana");

            foreach (Entity child in children)
            {
                child.LoadContent();
            }            
            //MediaPlayer.Play(mysong);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            foreach (Entity child in children)
            {
                child.UnloadContent();
            }
        }

        public void ClearWorld()
        {
            children.Clear();
            camera.pos = new Vector3(2, 20, 50);
            children.Add(camera);
            children.Add(MoviePlayer);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            
            KeyboardState newState = Keyboard.GetState();
            if (newState.IsKeyDown(Keys.F1))
            {
                if (!oldState.IsKeyDown(Keys.F1))
                {
                    useCamFighter = !useCamFighter;
                }
            }
            if (newState.IsKeyDown(Keys.F2))
            {
                if (!oldState.IsKeyDown(Keys.F2))
                {
                    ClearWorld();
                    Scenario.setUpArrive();
                }
            }
            if (newState.IsKeyDown(Keys.F3))
            {
                if (!oldState.IsKeyDown(Keys.F3))
                {
                    ClearWorld();
                    Scenario.setUpPursuit();
                }
            }
            if (newState.IsKeyDown(Keys.F4))
            {
                if (!oldState.IsKeyDown(Keys.F4))
                {
                    ClearWorld();
                    Scenario.setUpWander();
                }
            }
            
            if (newState.IsKeyDown(Keys.F5))
            {
                if (!oldState.IsKeyDown(Keys.F5))
                {
                    ClearWorld();
                    Scenario.setUpStateMachineDemo();
                }
            }
            if (newState.IsKeyDown(Keys.F6))
            {
                if (!oldState.IsKeyDown(Keys.F6))
                {
                    ClearWorld();
                    Scenario.setUpBuckRogersDemo();
                }
            } 
            if (newState.IsKeyDown(Keys.F7))
            {
                if (!oldState.IsKeyDown(Keys.F7))
                {
                    ClearWorld();
                    Scenario.setUpFlockingDemo();
                }
            }

            if (newState.IsKeyDown(Keys.F8))
            {
                if (!oldState.IsKeyDown(Keys.F8))
                {
                    ClearWorld();
                    Scenario.setUpPathFindingDemo();
                }
            }
            
            if (useCamFighter)
            {
                camera.pos = camFighter.pos;
                camera.look = camFighter.look;
                camera.up = camFighter.up;
                camera.right = camFighter.right;
            }
            else
            {
                camera.up = Vector3.Up;
                camera.right = Vector3.Cross(Camera.look, Camera.up);
            }
            //space.Partition();
            for (int i = children.Count - 1; i >= 0; i--)
            {
                if (children[i].Alive == false)
                {
                    children.Remove(children[i]);
                }
                else
                {
                    children[i].Update(gameTime);
                }
            }

            oldState = newState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.Aqua);
            spriteBatch.Begin();

            // Allows the game to exit
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            cumulSound += timeDelta;
            if (cumulSound < 35.0f)
            {
                Vector2 centre = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
                Vector2 imgTL = new Vector2(centre.X - (brTexture.Width / 2), centre.Y - (brTexture.Height / 2));
                Vector2 imgCentre = new Vector2(brTexture.Width / 2, brTexture.Height / 2);
                spriteBatch.Draw(brTexture, imgTL + imgCentre, new Rectangle(0, 0, brTexture.Width, brTexture.Height), Color.White, 0.0f, imgCentre, scale, SpriteEffects.None, 0);
                scale += (float) (timeDelta / 40.0f);
            }
            else
            {                
                foreach (Entity child in children)
                {
                    DepthStencilState state = new DepthStencilState();
                    state.DepthBufferEnable = true;
                    GraphicsDevice.DepthStencilState = state;

                    if (child != camFighter)
                    {
                        child.Draw(gameTime);
                    }
                }
                // Draw any lines
                Line.Draw();
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public Camera Camera
        {
            get
            {
                return camera;
            }
            set
            {
                camera = value;
            }
        }

        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get
            {
                return graphics;
            }
        }
    }
}
