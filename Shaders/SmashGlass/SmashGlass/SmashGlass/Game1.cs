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

namespace SmashGlass
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager m_Graphics;
        SpriteBatch m_SpriteBatch;

        private Texture2D m_Background;

        private BrokenGlassRenderer m_GlassRenderer;

        public Game1()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            m_Graphics.PreferMultiSampling = true;
            Content.RootDirectory = "Content";

            m_Graphics.PreferredBackBufferWidth = 1024;
            m_Graphics.PreferredBackBufferHeight = 768;
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
            m_Graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = 8;
            m_SpriteBatch = new SpriteBatch(GraphicsDevice);
            m_Background = Content.Load<Texture2D>("beach");
            m_GlassRenderer = new BrokenGlassRenderer(GraphicsDevice);
            m_GlassRenderer.Load(GraphicsDevice, Content);
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

            m_GlassRenderer.Update(gameTime.ElapsedGameTime.Milliseconds);

            var keyboardState = Keyboard.GetState(PlayerIndex.One);
            if (keyboardState.IsKeyDown(Keys.F1))
            {
                m_GlassRenderer.RenderMode = RenderMode.NoEffect;
                m_GlassRenderer.Reset();
            }

            if (keyboardState.IsKeyDown(Keys.F2))
            {
                m_GlassRenderer.RenderMode = RenderMode.Combined;
                m_GlassRenderer.Reset();
                
            }


            if (keyboardState.IsKeyDown(Keys.F3))
            {
                m_GlassRenderer.RenderMode = RenderMode.Standard;
                m_GlassRenderer.Reset();
            }

            if (keyboardState.IsKeyDown(Keys.F4))
            {
                m_GlassRenderer.RenderMode = RenderMode.Wireframe;
                m_GlassRenderer.Reset();
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

            RenderScene();

            PresentToScreen();

            base.Draw(gameTime);
        }

        private void PresentToScreen()
        {
            m_GlassRenderer.Draw();
        }

        private void RenderScene()
        {
            GraphicsDevice.SetRenderTarget(m_GlassRenderer.RenderTarget);
            m_SpriteBatch.Begin();

            if (m_GlassRenderer.RenderMode == RenderMode.NoEffect)
            {
                m_SpriteBatch.Draw(m_Background, Vector2.Zero, Color.White);
            }

            if (m_GlassRenderer.RenderMode == RenderMode.Standard)
            {
                m_SpriteBatch.Draw(m_Background, Vector2.Zero, Color.Red);
            }


            m_SpriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }
    }
}
