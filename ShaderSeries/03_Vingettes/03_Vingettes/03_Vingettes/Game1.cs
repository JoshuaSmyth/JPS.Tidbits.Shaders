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

namespace _03_Vingettes
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
        private VingettePostEffect m_VingettePostEffect;

        private KeyboardState m_LastFrameKeyboardState;

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

            m_VingettePostEffect = new VingettePostEffect(GraphicsDevice, Content);
            m_Background = Content.Load<Texture2D>("ruakaka");

            m_SpriteFont = Content.Load<SpriteFont>("SpriteFont1");
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
                m_VingettePostEffect.SwitchStyle();
       
            if (KeyboardState.IsKeyDown(Keys.A))
                m_VingettePostEffect.Radius += 0.05f;

            if (KeyboardState.IsKeyDown(Keys.Z))
                m_VingettePostEffect.Radius -= 0.05f;

            if (KeyboardState.IsKeyDown(Keys.S) && !m_LastFrameKeyboardState.IsKeyDown(Keys.S))
                m_VingettePostEffect.IsSepia = !m_VingettePostEffect.IsSepia;

            if (KeyboardState.IsKeyDown(Keys.C) && !m_LastFrameKeyboardState.IsKeyDown(Keys.C))
                m_VingettePostEffect.IsCircular = !m_VingettePostEffect.IsCircular;
         
            m_LastFrameKeyboardState = KeyboardState;


            if (m_VingettePostEffect.CurrentTechnique == VingetteTechnique.FollowSpot)
            {
                var ms = Mouse.GetState();
                m_VingettePostEffect.Centre = new Vector2(ms.X / 1024.0f, ms.Y / 768.0f);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            m_VingettePostEffect.BeginCapture();
                GraphicsDevice.Clear(Color.CornflowerBlue);
                spriteBatch.Begin();
                    spriteBatch.Draw(m_Background, Vector2.Zero, Color.White);
                spriteBatch.End();
            m_VingettePostEffect.EndCapture();

            m_VingettePostEffect.Draw(spriteBatch);

            spriteBatch.Begin();
            spriteBatch.DrawString(m_SpriteFont, "Press Space to change vingette style", new Vector2(10, 10), Color.White);

                if (m_VingettePostEffect.CurrentTechnique == VingetteTechnique.FollowSpot)
                {
                    spriteBatch.DrawString(m_SpriteFont, "Move the mouse to follow the cursor", new Vector2(10, 40), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(m_SpriteFont, "A to increase radius", new Vector2(10, 40), Color.White);
                    spriteBatch.DrawString(m_SpriteFont, "Z to decrease radius", new Vector2(10, 60), Color.White);
                    spriteBatch.DrawString(m_SpriteFont, "S to toggle sepiatone", new Vector2(10, 80), Color.White);
                }

                if (m_VingettePostEffect.CurrentTechnique == VingetteTechnique.Vingette)
                    spriteBatch.DrawString(m_SpriteFont, "C to toggle Circular/Eliptical Vingette", new Vector2(10, 100), Color.White);

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
