using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace _05_Distortion
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private SpriteFont m_SpriteFont;
        private Texture2D m_Background;
        private DistortionPostEffect m_DistortionPostEffect;

        private KeyboardState m_LastFrameKeyboardState;

        private Texture2D m_Distortion;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            Content.RootDirectory = "Content";

       
        }

       

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            m_DistortionPostEffect = new DistortionPostEffect(GraphicsDevice, Content);
            m_Background = Content.Load<Texture2D>("ruakaka");

            m_SpriteFont = Content.Load<SpriteFont>("SpriteFont1");

            m_Distortion = Content.Load<Texture2D>("distortion_001");

            m_DistortionPostEffect.DistortionTexture = m_Distortion;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            var KeyboardState = Keyboard.GetState(PlayerIndex.One);
            if (KeyboardState.IsKeyDown(Keys.Space) && !m_LastFrameKeyboardState.IsKeyDown(Keys.Space))
            {
                m_DistortionPostEffect.NextTechnique();
            }

       
            if (KeyboardState.IsKeyDown(Keys.A))
                m_DistortionPostEffect.Radius += 0.05f;

            if (KeyboardState.IsKeyDown(Keys.Z))
                m_DistortionPostEffect.Radius -= 0.05f;

            m_LastFrameKeyboardState = KeyboardState;


            if (KeyboardState.IsKeyDown(Keys.Enter))
            {
                m_DistortionPostEffect.TimeMS = 0;
            }

    
            m_DistortionPostEffect.Blur += -0.001f;
            m_DistortionPostEffect.Zoom += -0.001f;

            m_DistortionPostEffect.Wave += (float)(0.1f);

            m_DistortionPostEffect.TimeMS += gameTime.ElapsedGameTime.Milliseconds;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            m_DistortionPostEffect.BeginCapture();
                GraphicsDevice.Clear(Color.CornflowerBlue);
                spriteBatch.Begin();
                    spriteBatch.Draw(m_Background, Vector2.Zero, Color.White);
                spriteBatch.End();
            m_DistortionPostEffect.EndCapture();

            m_DistortionPostEffect.Draw(spriteBatch);

            spriteBatch.Begin();
            spriteBatch.DrawString(m_SpriteFont, "Press Space to change distortion style", new Vector2(10, 10), Color.White);

            spriteBatch.End();

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.F11))
                SaveScreenShot();

            base.Draw(gameTime);
        }

        private void SaveScreenShot()
        {
            int[] backBuffer = new int[GraphicsDevice.Viewport.Width * GraphicsDevice.Viewport.Height];
            GraphicsDevice.GetBackBufferData(backBuffer);

            //Copy into a texture 
            using (var texture = new Texture2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, GraphicsDevice.PresentationParameters.BackBufferFormat))
            {
                texture.SetData(backBuffer);

                var i = 0;
                var fileExists = true;
                var filename = "screenshot_001.jpg";
                while (fileExists)
                {
                    var fi = new FileInfo(String.Format("screenshot_{0}.jpg", i));
                    fileExists = fi.Exists;
                    if (!fileExists)
                        filename = fi.Name;
                    i++;
                }

                using (Stream stream = File.OpenWrite(filename))
                {
                    texture.SaveAsJpeg(stream, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                }
            }
        }
    }
}
