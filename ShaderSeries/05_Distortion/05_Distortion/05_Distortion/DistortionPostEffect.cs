using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _05_Distortion
{
    public enum DistortionTechnique
    {
        DistortionWave = 0,
        DistortionTexture = 1,
        End = 2
    }

    public class DistortionPostEffect
    {
        private readonly GraphicsDevice m_GraphicsDevice;
        private RenderTarget2D m_RenderCapture;
        private Effect m_Effect;

        public float Radius { get; set; }

        public Vector2 Centre { get; set; }

        public DistortionTechnique DistortionTechnique { get; set; }

        public void NextTechnique()
        {
            var current = (Int32) DistortionTechnique;
            current++;
            current = current%(Int32) (DistortionTechnique.End);
            DistortionTechnique = (DistortionTechnique) current;

            TimeMS = 0;
        }

        public float Blur
        {
            get { return m_Blur; }
            set
            {
                m_Blur = value;
                m_Effect.Parameters["BlurWidth"].SetValue(value);
            }
        }

        public float Zoom
        {
            get { return m_Zoom; }
            set
            {
                m_Zoom = value;
                m_Effect.Parameters["BlurStart"].SetValue(value);
            }
        }

        public float Wave
        {
            get { return m_Wave; }
            set { m_Wave = value; }
        }

        public float TimeMS
        {
            get { return m_Time; }
            set { m_Time = value; }
        }

        public Texture2D DistortionTexture
        {
            get { return m_DistortionTexture; }
            set
            {
                m_DistortionTexture = value;
                m_Effect.Parameters["InputTexture"].SetValue(m_DistortionTexture);
            }
        }

        private EffectParameter BlurWidth;
        private float m_Blur;
        private float m_Zoom;
        private float m_Wave;
        private float m_Time;
        private Texture2D m_DistortionTexture;

        public DistortionPostEffect(GraphicsDevice graphicsDevice, ContentManager content)
        {
            m_GraphicsDevice = graphicsDevice;
            m_Effect = content.Load<Effect>("Distortion");
            m_RenderCapture = new RenderTarget2D(m_GraphicsDevice, m_GraphicsDevice.Viewport.Width,
                                                 m_GraphicsDevice.Viewport.Height);


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
            if (DistortionTechnique == DistortionTechnique.DistortionWave)
            {
                DrawDistortionWave(spriteBatch);
            }


            if (DistortionTechnique == DistortionTechnique.DistortionTexture)
            {
                DrawDistortionTexture();
            }
        }

        private void DrawDistortionTexture()
        {
            m_Effect.CurrentTechnique = m_Effect.Techniques["TextureDistort"];
            m_Wave = (float) (Math.PI);
            m_Zoom = 1;
            m_Effect.Parameters["BlurStart"].SetValue(1);
        }

        private void DrawDistortionWave(SpriteBatch spriteBatch)
        {
            if (m_Time < 200)
            {
                m_Effect.CurrentTechnique = m_Effect.Techniques["Distort"];
                m_Wave = (float) (Math.PI);
                m_Zoom = 1;
                m_Effect.Parameters["BlurStart"].SetValue(1);

                m_Effect.Parameters["Time"].SetValue(m_Time/1000.0f);
            }
            else
            {
                m_Effect.Parameters["Time"].SetValue(-1);
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp,DepthStencilState.Default, RasterizerState.CullNone, m_Effect);
                spriteBatch.Draw(m_RenderCapture, Vector2.Zero, Color.White);
            spriteBatch.End();
          
        }
    }
}
