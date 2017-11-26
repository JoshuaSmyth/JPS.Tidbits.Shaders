using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Loading_a_3D_model
{
    public class PostProcessor
    {
        
        private class RenderCapture
        {
            RenderTarget2D renderTarget;
            GraphicsDevice graphicsDevice;

            public RenderCapture(GraphicsDevice GraphicsDevice)
	        {
                this.graphicsDevice = GraphicsDevice;
                renderTarget = new RenderTarget2D(GraphicsDevice,
                    GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
                    false, SurfaceFormat.Color, DepthFormat.Depth24);
	        }

            public void Begin()
            {
                graphicsDevice.SetRenderTarget(renderTarget);
            }

            public void End()
            {
                graphicsDevice.SetRenderTarget(null);
            }

            public Texture2D GetTexture()
            {
                return renderTarget;
            }
        }

        // Texture to process
        private RenderCapture Input { get; set; }

        // GraphicsDevice and SpriteBatch for drawing
        protected GraphicsDevice graphicsDevice;
        protected static SpriteBatch spriteBatch;

        public PostProcessor(GraphicsDevice graphicsDevice)
        {

            if (spriteBatch == null)
                spriteBatch = new SpriteBatch(graphicsDevice);

            this.graphicsDevice = graphicsDevice;

            Input = new RenderCapture(graphicsDevice);
        }

        public void Begin() {
            Input.Begin();
        }

        public void End() {
            Input.End();
        }

        // Draws the input texture using the pixel shader postprocessor
        public virtual void Draw(Effect effect)
        {
            // Set effect parameters if necessary
            if (effect.Parameters["ScreenWidth"] != null)
                effect.Parameters["ScreenWidth"].SetValue(
                    graphicsDevice.Viewport.Width);

            if (effect.Parameters["ScreenHeight"] != null)
                effect.Parameters["ScreenHeight"].SetValue(
                    graphicsDevice.Viewport.Height);

            // Initialize the spritebatch and effect
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            effect.CurrentTechnique.Passes[0].Apply();

            // For reach compatibility
            graphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;

            // Draw the input texture
            spriteBatch.Draw(Input.GetTexture(), Vector2.Zero, Color.White);
        
            // End the spritebatch and effect
            spriteBatch.End();

            // Clean up render states changed by the spritebatch
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
        }
    }

}
