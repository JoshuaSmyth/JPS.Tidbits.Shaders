using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Rumble
{
    public enum RumbleTechnique
    {
        None = 0,
        LightRumble = 1,
        HeavyRumble = 2,
        LightRgbRumble = 3,
        HeavyRgbRumble = 4
    }

    public class RumblePostEffect
    {
        private readonly GraphicsDevice m_GraphicsDevice;
        private RenderTarget2D m_RenderCapture;
        private Effect m_Effect;

        private RumbleTechnique m_CurrentTechnique;

        private const Int32 RumbleTime = 2000;
        private Int32 m_CurrentRumbleTime;
        private Int32 m_Amplitude;

        private Random m_Random = new Random(42);

        public RumbleTechnique CurrentTechnique
        {
            get { return m_CurrentTechnique; }
            set { m_CurrentTechnique = value; }
        }

        public RumblePostEffect(GraphicsDevice graphicsDevice, ContentManager content)
        {
            m_GraphicsDevice = graphicsDevice;
            m_CurrentRumbleTime = RumbleTime;
            m_Effect = content.Load<Effect>("Rumble");
            m_Effect.CurrentTechnique = m_Effect.Techniques["Technique1"];
            m_RenderCapture = new RenderTarget2D(m_GraphicsDevice, m_GraphicsDevice.Viewport.Width, m_GraphicsDevice.Viewport.Height);

            CurrentTechnique = RumbleTechnique.None;
        }

        public void BeginCapture()
        {
            m_GraphicsDevice.SetRenderTarget(m_RenderCapture);
        }

        public void EndCapture()
        {
            m_GraphicsDevice.SetRenderTarget(null);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentTechnique == RumbleTechnique.None ||
                CurrentTechnique == RumbleTechnique.LightRumble || 
                CurrentTechnique == RumbleTechnique.HeavyRumble)
            {
                m_Effect.CurrentTechnique = m_Effect.Techniques["Technique1"];
            }
            else
            {
                m_Effect.CurrentTechnique = m_Effect.Techniques["Technique2"];
            }
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, m_Effect);
                spriteBatch.Draw(m_RenderCapture, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        public void SwitchStyle()
        {
            var currentId = (Int32) CurrentTechnique;
            currentId = (currentId + 1)%5;

            CurrentTechnique = (RumbleTechnique) currentId;

            m_CurrentRumbleTime = RumbleTime;
            if (CurrentTechnique == RumbleTechnique.LightRumble)
                m_Amplitude = 20;

            if (CurrentTechnique == RumbleTechnique.HeavyRumble)
                m_Amplitude = 100;

            if (CurrentTechnique == RumbleTechnique.LightRgbRumble)
                m_Amplitude = 50;

            if (CurrentTechnique == RumbleTechnique.HeavyRgbRumble)
                m_Amplitude = 50;
        }

        public void Update(GameTime gameTime)
        {
            m_CurrentRumbleTime -= (Int32)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (CurrentTechnique == RumbleTechnique.None || m_CurrentRumbleTime <= 0)
            {
                m_Effect.Parameters["RumbleVector"].SetValue(Vector2.Zero);
                m_Effect.Parameters["RumbleVectorR"].SetValue(Vector2.Zero);
                m_Effect.Parameters["RumbleVectorG"].SetValue(Vector2.Zero);
                m_Effect.Parameters["RumbleVectorB"].SetValue(Vector2.Zero);
                return;
            }

            if (CurrentTechnique == RumbleTechnique.LightRumble || 
                CurrentTechnique == RumbleTechnique.HeavyRumble)
            {
                // Randomize the rumble
                var rX = m_Random.Next(0, m_Amplitude) * (m_CurrentRumbleTime / (float)RumbleTime);
                var rY = m_Random.Next(0, m_Amplitude) * (m_CurrentRumbleTime / (float)RumbleTime);

                m_Effect.Parameters["RumbleVector"].SetValue(new Vector2(rX / (float)m_GraphicsDevice.Viewport.Width, rY / (float)m_GraphicsDevice.Viewport.Height));
                return;
            }

            if (CurrentTechnique == RumbleTechnique.LightRgbRumble ||
                CurrentTechnique == RumbleTechnique.HeavyRgbRumble)
            {
                // Randomize the rumble
                var rX = m_Random.Next(0, m_Amplitude) * (m_CurrentRumbleTime / (float)RumbleTime);
                var rY = m_Random.Next(0, m_Amplitude) * (m_CurrentRumbleTime / (float)RumbleTime);

                m_Effect.Parameters["RumbleVectorR"].SetValue(new Vector2(rX / (float)m_GraphicsDevice.Viewport.Width, rY / (float)m_GraphicsDevice.Viewport.Height));

                var gX = m_Random.Next(0, m_Amplitude) * (m_CurrentRumbleTime / (float)RumbleTime);
                var gY = m_Random.Next(0, m_Amplitude) * (m_CurrentRumbleTime / (float)RumbleTime);

                m_Effect.Parameters["RumbleVectorG"].SetValue(new Vector2(gX / (float)m_GraphicsDevice.Viewport.Width, gY / (float)m_GraphicsDevice.Viewport.Height));

                var bX = m_Random.Next(0, m_Amplitude) * (m_CurrentRumbleTime / (float)RumbleTime);
                var bY = m_Random.Next(0, m_Amplitude) * (m_CurrentRumbleTime / (float)RumbleTime);

                m_Effect.Parameters["RumbleVectorB"].SetValue(new Vector2(bX / (float)m_GraphicsDevice.Viewport.Width, bY / (float)m_GraphicsDevice.Viewport.Height));

                return;
            }

        }
    }
}
