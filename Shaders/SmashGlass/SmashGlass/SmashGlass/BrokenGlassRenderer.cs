using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SmashGlass
{
    public enum RenderMode
    {
        NoEffect = 0,
        Combined = 1,
        Standard = 2,
        Wireframe = 3
    }

    public class BrokenGlassRenderer
    {
        private readonly GraphicsDevice m_GraphicsDevice;
        private RenderTarget2D m_RenderTarget;
        private readonly SpriteBatch m_SpriteBatch;

        private Model m_OriginalModel;
        private GlassModelCombined m_GlassModelCombined;
        private GlassModel m_GlassModel;

        private BasicEffect m_BasicEffect;

        public RenderMode RenderMode;

        private Int32 m_Timer;

        private readonly Texture2D m_DummyTexture;

        public void Reset()
        {
            m_Timer = 0;
        }

        public BrokenGlassRenderer(GraphicsDevice graphicsDevice)
        {
            m_GraphicsDevice = graphicsDevice;
            m_SpriteBatch = new SpriteBatch(graphicsDevice);
            m_RenderTarget = new RenderTarget2D(graphicsDevice, 1024, 768);

            m_DummyTexture = new Texture2D(graphicsDevice, 1, 1);
            m_DummyTexture.SetData(new Color[] { Color.White });
        }

        public void Update(Int32 ms)
        {
            m_Timer += ms;
        }

        public void Draw()
        {

            if (RenderMode == RenderMode.NoEffect)
            {
             //   DrawOriginalMesh();
                DrawWithSpriteBatch();
            }

            if (RenderMode == RenderMode.Combined)
            {
                m_GraphicsDevice.Clear(Color.Black);
            
                m_GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None };
         
                m_GraphicsDevice.Clear(Color.White);
                DrawCombinedGlassMesh();
            }

            if (RenderMode == RenderMode.Standard)
            {
                m_GraphicsDevice.Clear(Color.White);
                m_GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None };
                DrawGlassMesh();
              //  DrawDebug();
            }

            if (RenderMode == RenderMode.Wireframe)
            {
                if (RenderMode == RenderMode.Wireframe)
                    m_GraphicsDevice.RasterizerState = new RasterizerState() { FillMode = FillMode.WireFrame };
               // m_GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None };
         
                m_GraphicsDevice.Clear(Color.White);
                DrawCombinedGlassMesh();
            }
        }

        private void DrawDebug()
        {
            var w = m_GraphicsDevice.Viewport.Width;
            var h = m_GraphicsDevice.Viewport.Height;

            m_SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            for (int i = 0; i < m_GlassModel.Meshes.Count; i++)
            {
                var mesh = m_GlassModel.Meshes[i];

                // Draw verticies
                foreach (var vertex in mesh.Verticies)
                {
                    var position = new Vector2(vertex.Position.X / 2.0f + 0.5f, vertex.Position.Y / 2.0f + 0.5f);

                    var rect = new Rectangle((Int32)(position.X * w - 2), (Int32)(h - position.Y * h - 2), 4, 4);
                    m_SpriteBatch.Draw(m_DummyTexture, rect, Color.Black);
                }

                // Draw centre
                {
                    var vertex = mesh.Centre;
                    var position = new Vector2(vertex.X / 2.0f + 0.5f, vertex.Y / 2.0f + 0.5f);

                    var rect = new Rectangle((Int32)(position.X * w - 2), (Int32)(h - position.Y * h - 2), 4, 4);

                    var color = Color.Yellow;
                    color.A = (byte) ( color.A * mesh.Dampining);
                    m_SpriteBatch.Draw(m_DummyTexture, rect, color);
                }
            }
            m_SpriteBatch.End();
        }

        Vector3 cameraPosition = new Vector3(0.01f, 0.01f, 2f);

        private void DrawGlassMesh()
        {
            var aspectRatio = m_GraphicsDevice.Viewport.AspectRatio;

            // Setup Effect Parameters
            m_BasicEffect.TextureEnabled = true;
            m_BasicEffect.Texture = RenderTarget;
            m_BasicEffect.VertexColorEnabled = false;
            m_BasicEffect.LightingEnabled = false;
            m_BasicEffect.World = Matrix.Identity;

            m_BasicEffect.View = Matrix.CreateLookAt(cameraPosition,
                Vector3.Zero, Vector3.Up);

            m_BasicEffect.Projection = Matrix.CreatePerspective(1, 1, 1.0f, 100.0f);
            // Setup Vertex and index buffers
            m_GraphicsDevice.SetVertexBuffer(m_GlassModel.VertexBuffer);
            m_GraphicsDevice.Indices = m_GlassModel.IndexBuffer;

            if (m_Timer > 200) 
                m_GraphicsDevice.Clear(Color.Black);

            foreach (var mesh in m_GlassModel.Meshes)
            {
               
                if (m_Timer > 200)
                {
                   
                    var t = m_Timer - 200.0f;

                   

                    var translation = Matrix.CreateTranslation(-1*mesh.Centre);
                    var translationBack = Matrix.CreateTranslation(mesh.Centre);

                    var z = (mesh.Dampining) * (t / 80.0f);
                    var x = (mesh.Dampining) * (t / 100.0f) * mesh.DirToCentre.X;
                    var y = (mesh.Dampining) * (t / 100.0f) * mesh.DirToCentre.Y;

                    var force = new Vector3(x, y, z);
                    var zTransform = Matrix.CreateTranslation(force);

                    var rotX = mesh.Rotation.X * (mesh.Dampining) * (t / 50.0f);
                    var rotY = mesh.Rotation.Y * (mesh.Dampining) * (t / 50.0f);
                    var rotZ = mesh.Rotation.Z * (mesh.Dampining) * (t / 50.0f);

                  

                    var rotMatX = Matrix.CreateRotationX(rotX);
                    var rotMatY = Matrix.CreateRotationY(rotY);
                    var rotMatZ = Matrix.CreateRotationZ(rotZ);

                    var rotMat = rotMatX * rotMatY * rotMatZ;
                    m_BasicEffect.World = translation*rotMat*translationBack*zTransform;
                }

                foreach (var part in mesh.Parts)
                {
                    for (int j = 0; j < m_BasicEffect.CurrentTechnique.Passes.Count; j++)
                    {
                        m_BasicEffect.CurrentTechnique.Passes[j].Apply();
                        m_GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, 0, part.NumVerticies, part.StartIndex, part.PrimitiveCount);
                    }    
                }
            }

        }

        private void DrawCombinedGlassMesh()
        {

            var primitiveCount = m_GlassModelCombined.PrimitiveCount;

          

            m_GraphicsDevice.Indices = m_GlassModelCombined.IndexBuffer;
            m_GraphicsDevice.SetVertexBuffer(m_GlassModelCombined.VertexBuffer);
            m_BasicEffect.TextureEnabled = true;
            m_BasicEffect.Texture = RenderTarget;
            m_BasicEffect.VertexColorEnabled = true;
            m_BasicEffect.LightingEnabled = false;
            m_BasicEffect.World = Matrix.Identity;

            m_BasicEffect.Projection = Matrix.Identity;
            m_BasicEffect.View = Matrix.Identity;

            var vertexCount = m_GlassModelCombined.VertexBuffer.VertexCount;
            foreach (EffectPass pass in m_BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                m_GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, 0, primitiveCount);
            }
        
        }

        private void DrawOriginalMesh()
        {
            var aspectRatio = m_GraphicsDevice.Viewport.AspectRatio;
            m_GraphicsDevice.Clear(Color.White);

            
            for (int i = 0; i < m_OriginalModel.Meshes.Count; i++)
            {
                var mesh = m_OriginalModel.Meshes[i];

               
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Texture = RenderTarget;
                    effect.World = Matrix.Identity;
               
                    effect.Projection = Matrix.Identity;
                    effect.View = Matrix.Identity;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }

        public void Load(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            m_OriginalModel = contentManager.Load<Model>("fractured_loose");

            m_GlassModelCombined = GlassModelCombined.FromModel(graphicsDevice, m_OriginalModel);
            m_GlassModel = GlassModel.FromModel(m_OriginalModel);


            m_BasicEffect = new BasicEffect(graphicsDevice) {TextureEnabled = true};
        }

        private void DrawWithSpriteBatch()
        {
            m_SpriteBatch.Begin();
                m_SpriteBatch.Draw(m_RenderTarget, Vector2.Zero, Color.White);
            m_SpriteBatch.End();
        }


        public RenderTarget2D RenderTarget
        {
            get { return m_RenderTarget; }
            set { m_RenderTarget = value; }
        }
    }
}
