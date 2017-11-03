using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _03_Vingettes
{
    public enum VingetteTechnique
    {
        None = 0,
        Vingette = 1,
        HorizontalVingette = 2,
        VerticalVingette = 3,
        FollowSpot = 4
    }

    public class VingettePostEffect
    {
        private readonly GraphicsDevice m_GraphicsDevice;
        private RenderTarget2D m_RenderCapture;
        private Effect m_Effect;

        private VingetteTechnique m_CurrentTechnique;

        public float Radius { get; set; }

        public Boolean IsSepia { get; set; }

        public Boolean IsCircular { get; set; }

        public Boolean IsFollowMode { get; set; }

        public Vector2 Centre { get; set; }

        public VingetteTechnique CurrentTechnique
        {
            get { return m_CurrentTechnique; }
            set { m_CurrentTechnique = value; }
        }

        public VingettePostEffect(GraphicsDevice graphicsDevice, ContentManager content)
        {
            m_GraphicsDevice = graphicsDevice;
            m_Effect = content.Load<Effect>("Vingette");
            m_Effect.CurrentTechnique = m_Effect.Techniques["Vingette"];
            m_RenderCapture = new RenderTarget2D(m_GraphicsDevice, m_GraphicsDevice.Viewport.Width, m_GraphicsDevice.Viewport.Height);

            CurrentTechnique = VingetteTechnique.None;
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
            m_Effect.Parameters["R"].SetValue(Radius);
            m_Effect.Parameters["EnableSepia"].SetValue(IsSepia);
            m_Effect.Parameters["AspectRatio"].SetValue(1.3333f);
            m_Effect.Parameters["PreserveAspectRatio"].SetValue(IsCircular);
            m_Effect.Parameters["Centre"].SetValue(Centre);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, m_Effect);
                spriteBatch.Draw(m_RenderCapture, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        public void SwitchStyle()
        {
            var currentId = (Int32) CurrentTechnique;
            currentId = (currentId + 1)%5;

            CurrentTechnique = (VingetteTechnique) currentId;

            if (CurrentTechnique == VingetteTechnique.None)
            {
                m_Effect.CurrentTechnique = m_Effect.Techniques["Vingette"];
                Radius = 0.2f;
                IsCircular = false;
                Centre = new Vector2(0.5f,0.5f);
            }

            if (CurrentTechnique == VingetteTechnique.Vingette)
            {
                m_Effect.CurrentTechnique = m_Effect.Techniques["Vingette"];
                Radius = 1.8f;
                IsCircular = false;
                Centre = new Vector2(0.5f, 0.5f);
            }

            if (CurrentTechnique == VingetteTechnique.HorizontalVingette)
            {
                m_Effect.CurrentTechnique = m_Effect.Techniques["HorizontalVingette"];
                Radius = 2.8f;
                IsCircular = false;
                Centre = new Vector2(0.5f, 0.5f);
            }

            if (CurrentTechnique == VingetteTechnique.VerticalVingette)
            {
                m_Effect.CurrentTechnique = m_Effect.Techniques["VerticalVingette"];
                Radius = 2.8f;
                IsCircular = false;
                Centre = new Vector2(0.5f, 0.5f);
            }
            if (CurrentTechnique == VingetteTechnique.FollowSpot)
            {
                m_Effect.CurrentTechnique = m_Effect.Techniques["Vingette"];
                Radius = 5.0f;
                IsCircular = true;
                Centre = new Vector2(0.5f, 0.5f);
            }
        }
    }
}
