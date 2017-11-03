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

namespace XNA_01
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager m_GraphicsDeviceManager;
        private SpriteBatch m_SpriteBatch;

        private Texture2D m_background;

        public Game1()
        {
            m_GraphicsDeviceManager = new GraphicsDeviceManager(this);
            m_GraphicsDeviceManager.PreferredBackBufferWidth = 1024;
            m_GraphicsDeviceManager.PreferredBackBufferHeight = 768;
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            m_SpriteBatch = new SpriteBatch(GraphicsDevice);
            m_background = Content.Load<Texture2D>("myImage");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            m_SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            m_SpriteBatch.Draw(m_background, Vector2.Zero, Color.White);
            m_SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}