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

namespace Rumble
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager m_Graphics;
        SpriteBatch m_SpriteBatch;
        private RumblePostEffect m_PostEffect;
        private Texture2D m_Texture;

        private KeyboardState m_LastKeyboardState;
        private SpriteFont m_SpriteFont;
        public Game1()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            m_Graphics.PreferredBackBufferWidth = 800;
            m_Graphics.PreferredBackBufferHeight = 600;
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
            m_SpriteBatch = new SpriteBatch(GraphicsDevice);
            m_PostEffect = new RumblePostEffect(GraphicsDevice, Content);
            m_Texture = Content.Load<Texture2D>("bricks");
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
            var kbs = Keyboard.GetState(PlayerIndex.One);
            if (kbs.IsKeyDown(Keys.Space) && !m_LastKeyboardState.IsKeyDown(Keys.Space))
            {
                m_PostEffect.SwitchStyle();
            }

            m_PostEffect.Update(gameTime);

            
            m_LastKeyboardState = kbs;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            // Render Scene to texture
            m_PostEffect.BeginCapture();
                DrawScene(m_SpriteBatch);
            m_PostEffect.EndCapture();
            
            // Render Scene to screen
            m_PostEffect.Draw(m_SpriteBatch);

            // Draw Text
            m_SpriteBatch.Begin();
                m_SpriteBatch.DrawString(m_SpriteFont,"Current Technique: " + m_PostEffect.CurrentTechnique.ToString(), new Vector2(20,20), Color.Yellow);
            m_SpriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawScene(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
                spriteBatch.Draw(m_Texture,Vector2.Zero,Color.White);
            spriteBatch.End();
        }
    }
}
