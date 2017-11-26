using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Loading_a_3D_model
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Model model;
        private Model ModelRocks;
        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 3, -12), new Vector3(0, 3, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), 1280 / 720.0f, 1f, 1000f);

        private Effect Standard;
        private Texture texture;
        private Texture Rocks;
        private float z = -12;
        private RenderTarget2D DepthRenderTarget;
        private PostProcessor m_PostProcessor;
        enum RenderMode
        {
            Standard = 0,
        }

        enum PostEffects
        {
            None = 0,
            BlackAndWhite
        }

        PostEffects CurrentPostEffect = PostEffects.None;
        private Effect PostEffectBlackAndWhite;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.PreparingDeviceSettings += (s,e) =>
            {
                e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 8;
            };
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            this.IsMouseVisible = true;
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
            var sw = new Stopwatch();
            sw.Start();
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            model = Content.Load<Model>("Cave_Tunnel_Pipe");
            ModelRocks = Content.Load<Model>("Cave_Tunnel_Rocks");
            Standard = Content.Load<Effect>("Standard");
            texture = Content.Load<Texture>("WallDIF");
            Rocks = Content.Load<Texture>("Rocks");
            sw.Stop();
            Console.WriteLine("Time taken to load content: " + sw.ElapsedMilliseconds + "ms");
            DepthRenderTarget = new RenderTarget2D(GraphicsDevice, 1280,720,false, SurfaceFormat.Single, DepthFormat.Depth24);

            PostEffectBlackAndWhite = Content.Load<Effect>("PostEffects/BlackAndWhite");
            m_PostProcessor = new PostProcessor(GraphicsDevice);
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
            base.Update(gameTime);

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            var keystate = Keyboard.GetState();
            if (keystate.IsKeyDown(Keys.A))
            {
                z+=0.1f;
            }
            if (keystate.IsKeyDown(Keys.Z))
            {
                z-=0.1f;
            }
            if (keystate.IsKeyDown(Keys.D1)) {
                CurrentPostEffect = PostEffects.None;
            }
            if (keystate.IsKeyDown(Keys.D2)) {
                CurrentPostEffect = PostEffects.BlackAndWhite;
            }
        }

        private void DrawModel(Model model, Texture texture, Matrix world, Matrix view, Matrix projection)
        {
            var worldParam = Standard.Parameters["World"];
            var viewParam = Standard.Parameters["View"];
            var projParam = Standard.Parameters["Projection"];
            var textParam = Standard.Parameters["gTex0"];
            var camPos = Standard.Parameters["CameraPosition"];
            camPos.SetValue(new Vector3(0, 3, z));
            textParam.SetValue(texture);
            
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    graphics.GraphicsDevice.SetVertexBuffer(part.VertexBuffer, part.VertexOffset);
                    graphics.GraphicsDevice.Indices = part.IndexBuffer;
                    worldParam.SetValue(world);
                    viewParam.SetValue(view);
                    projParam.SetValue(projection);
                    Standard.CurrentTechnique.Passes[0].Apply();
                    graphics.GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList, 0, 0,
                    part.NumVertices, part.StartIndex, part.PrimitiveCount);
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            graphics.GraphicsDevice.Clear(Color.Black);
            view = Matrix.CreateLookAt(new Vector3(0, 3, z), new Vector3(0, 3, z+12), Vector3.UnitY);
            
            if (CurrentPostEffect == PostEffects.BlackAndWhite)
            {
                m_PostProcessor.Begin();
                    DrawLevel();
                m_PostProcessor.End();
                m_PostProcessor.Draw(PostEffectBlackAndWhite);
            }
            else
            {
                DrawLevel();
            }
        }

        private void DrawLevel() {
            DrawModel(model, texture,Matrix.CreateTranslation(new Vector3(0, 0, 36f)), view, projection);
            DrawModel(model, texture,Matrix.CreateTranslation(new Vector3(0, 0, 24f)), view, projection);
            DrawModel(model, texture, Matrix.CreateTranslation(new Vector3(0, 0, 12f)), view, projection);
            DrawModel(model, texture, Matrix.CreateTranslation(new Vector3(0, 0, 0f)), view, projection);
            DrawModel(model, texture, Matrix.CreateTranslation(new Vector3(0, 0, -12f)), view, projection);
            DrawModel(model, texture,Matrix.CreateTranslation(new Vector3(0, 0, -24f)), view, projection);
            DrawModel(model, texture, Matrix.CreateTranslation(new Vector3(0, 0, -36f)), view, projection);
            DrawModel(model,texture, Matrix.CreateTranslation(new Vector3(0, 0, -48f)), view, projection);
            DrawModel(model,texture, Matrix.CreateTranslation(new Vector3(0, 0, -60f)), view, projection);

            DrawModel(ModelRocks, Rocks,Matrix.CreateTranslation(new Vector3(0, 0, 36f)), view, projection);
            DrawModel(ModelRocks, Rocks,Matrix.CreateTranslation(new Vector3(0, 0, 24f)), view, projection);
            DrawModel(ModelRocks, Rocks, Matrix.CreateTranslation(new Vector3(0, 0, 12f)), view, projection);
            DrawModel(ModelRocks, Rocks, Matrix.CreateTranslation(new Vector3(0, 0, 0f)), view, projection);
            DrawModel(ModelRocks, Rocks, Matrix.CreateTranslation(new Vector3(0, 0, -12f)), view, projection);
            DrawModel(ModelRocks, Rocks,Matrix.CreateTranslation(new Vector3(0, 0, -24f)), view, projection);
            DrawModel(ModelRocks, Rocks, Matrix.CreateTranslation(new Vector3(0, 0, -36f)), view, projection);
            DrawModel(ModelRocks,Rocks, Matrix.CreateTranslation(new Vector3(0, 0, -48f)), view, projection);
            DrawModel(ModelRocks,Rocks, Matrix.CreateTranslation(new Vector3(0, 0, -60f)), view, projection);
        }
    }
}
