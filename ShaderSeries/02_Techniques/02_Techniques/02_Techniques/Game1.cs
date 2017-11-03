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

namespace _02_Techniques
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager m_GraphicsDeviceManager;
        private GraphicsDevice m_GraphicsDevice;
        SpriteBatch m_SpriteBatch;
        
        private Texture2D m_background;
        private Effect m_Effect;

        private SpriteFont m_SpriteFont;
        private KeyboardState m_KeyboardStateLastFrame;

        private Int32 currentTechnique = 0;
        private Int32 modTimer = 0;

        public Game1()
        {
            m_GraphicsDeviceManager = new GraphicsDeviceManager(this);
            m_GraphicsDeviceManager.PreferredBackBufferWidth = 1024;
            m_GraphicsDeviceManager.PreferredBackBufferHeight = 768;
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            Window.Title = "Techniques";
        }

        protected override void LoadContent()
        {
            m_SpriteBatch = new SpriteBatch(GraphicsDevice);
            m_GraphicsDevice = GraphicsDevice;
            m_background = Content.Load<Texture2D>("testimage2");
            m_Effect = Content.Load<Effect>("scanlines");
            m_SpriteFont = Content.Load<SpriteFont>("SpriteFont1");
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            var keyboardState = Keyboard.GetState(PlayerIndex.One);
            if (keyboardState.IsKeyDown(Keys.Space) && !m_KeyboardStateLastFrame.IsKeyDown(Keys.Space))
            {
                currentTechnique = (currentTechnique + 1)%m_Effect.Techniques.Count;
                m_Effect.CurrentTechnique = m_Effect.Techniques[currentTechnique];
                modTimer = 0;   // Reset timer
            }

            m_KeyboardStateLastFrame = keyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Update pulse (Want to generate a value between 0 and 1)
            const float speed = 0.008f;
            modTimer += (Int32) gameTime.ElapsedGameTime.TotalMilliseconds;
            var rads = (modTimer * speed) / Math.PI;
            var a = Math.Sin(rads);
            var mod = (float)Math.Abs(a);
            m_Effect.Parameters["Pulse"].SetValue(mod);
            m_Effect.Parameters["ImageHeight"].SetValue(m_GraphicsDevice.Viewport.Height);
            m_Effect.Parameters["Contrast"].SetValue(1.0f);
            m_Effect.Parameters["Brightness"].SetValue(0.2f);
            m_Effect.Parameters["DesaturationAmount"].SetValue(1.0f);

            m_SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, m_Effect);
                m_SpriteBatch.Draw(m_background, new Rectangle(0, 0, m_GraphicsDevice.Viewport.Width, m_GraphicsDevice.Viewport.Height), new Rectangle(0, 0, m_background.Width, m_background.Height), Color.White);
            m_SpriteBatch.End();
       
            m_SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                m_SpriteBatch.DrawString(m_SpriteFont,"Press Space to switch Techniques", new Vector2(20,20),  Color.White);
                m_SpriteBatch.DrawString(m_SpriteFont, m_Effect.CurrentTechnique.Name, new Vector2(20, 50), Color.White);
            m_SpriteBatch.End();

            if (m_KeyboardStateLastFrame.IsKeyDown(Keys.F11))
                SaveScreenShot();

            base.Draw(gameTime);
        }

        private void SaveScreenShot ()
        {
            int[] backBuffer = new int[GraphicsDevice.Viewport.Width * GraphicsDevice.Viewport.Height];
            GraphicsDevice.GetBackBufferData(backBuffer);

            //Copy into a texture 
            using(var texture = new Texture2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, GraphicsDevice.PresentationParameters.BackBufferFormat))
            {
                texture.SetData(backBuffer);

                var i = 0;
                var fileExists = true;
                var filename = "screenshot_001.jpg";
                while(fileExists)
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
