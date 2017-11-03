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

namespace Loading_a_3D_model
{


    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Model model;
        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 3, -12), new Vector3(0, 3, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), 1280 / 720.0f, 0.1f, 100f);

        private Effect Triplanar;
        private Effect TriplanarBlend;
        private Effect Standard;
        private Texture texture;

        private float z = -12;

        enum RenderMode
        {
            Standard = 0,
            Triplanar = 1,
            Blend = 2
        }

        RenderMode CurrentRenderMode = RenderMode.Standard;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            model = Content.Load<Model>("Cave_Tunnel_Pipe");
            Triplanar = Content.Load<Effect>("Triplanar");
            Standard = Content.Load<Effect>("Standard");
            TriplanarBlend = Content.Load<Effect>("TriplanarBlend");
            texture = Content.Load<Texture>("WallDIF");
            //texture = Content.Load<Texture>("texture");

            // TODO: use this.Content to load your game content here
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
                CurrentRenderMode = RenderMode.Standard;
            }
            if (keystate.IsKeyDown(Keys.D2)) {
                CurrentRenderMode = RenderMode.Triplanar;
            }
            if (keystate.IsKeyDown(Keys.D3)) {
                CurrentRenderMode = RenderMode.Blend;
            }
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            if (CurrentRenderMode == RenderMode.Triplanar) {
                var worldViewProjParam = Triplanar.Parameters["WorldViewProjection"];

                var textParam = Triplanar.Parameters["gTex0"];
                var camPos = Triplanar.Parameters["CameraPosition"];
                camPos.SetValue(new Vector3(0, 3, z));
                textParam.SetValue(texture);
            
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (var part in mesh.MeshParts)
                    {
                        graphics.GraphicsDevice.SetVertexBuffer(part.VertexBuffer, part.VertexOffset);
                        graphics.GraphicsDevice.Indices = part.IndexBuffer;
                        worldViewProjParam.SetValue(world*view*projection);
                        Triplanar.CurrentTechnique.Passes[0].Apply();
                        graphics.GraphicsDevice.DrawIndexedPrimitives(
                        PrimitiveType.TriangleList, 0, 0,
                        part.NumVertices, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
            if (CurrentRenderMode == RenderMode.Blend) {
                var worldViewProjParam = TriplanarBlend.Parameters["WorldViewProjection"];

                var textParam = TriplanarBlend.Parameters["gTex0"];
                var camPos = TriplanarBlend.Parameters["CameraPosition"];
                camPos.SetValue(new Vector3(0, 3, z));
                textParam.SetValue(texture);
            
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (var part in mesh.MeshParts)
                    {
                        graphics.GraphicsDevice.SetVertexBuffer(part.VertexBuffer, part.VertexOffset);
                        graphics.GraphicsDevice.Indices = part.IndexBuffer;
                        worldViewProjParam.SetValue(world*view*projection);
                        TriplanarBlend.CurrentTechnique.Passes[0].Apply();
                        graphics.GraphicsDevice.DrawIndexedPrimitives(
                        PrimitiveType.TriangleList, 0, 0,
                        part.NumVertices, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
            if (CurrentRenderMode == RenderMode.Standard)
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
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            graphics.GraphicsDevice.Clear(Color.Black);
            view = Matrix.CreateLookAt(new Vector3(0, 3, z), new Vector3(0, 3, z+12), Vector3.UnitY);
            
            DrawModel(model, Matrix.CreateTranslation(new Vector3(0, 0, 36f)), view, projection);
            DrawModel(model, Matrix.CreateTranslation(new Vector3(0, 0, 24f)), view, projection);
            DrawModel(model, Matrix.CreateTranslation(new Vector3(0, 0, 12f)), view, projection);
            DrawModel(model, Matrix.CreateTranslation(new Vector3(0, 0, 0f)), view, projection);
            DrawModel(model, Matrix.CreateTranslation(new Vector3(0, 0, -12f)), view, projection);
            DrawModel(model, Matrix.CreateTranslation(new Vector3(0, 0, -24f)), view, projection);
            DrawModel(model, Matrix.CreateTranslation(new Vector3(0, 0, -36f)), view, projection);
            DrawModel(model, Matrix.CreateTranslation(new Vector3(0, 0, -48f)), view, projection);
            DrawModel(model, Matrix.CreateTranslation(new Vector3(0, 0, -60f)), view, projection);
        }
    }
}
