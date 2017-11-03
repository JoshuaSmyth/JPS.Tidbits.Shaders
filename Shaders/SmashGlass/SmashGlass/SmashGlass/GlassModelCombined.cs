using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SmashGlass
{
    public class GlassModelCombined : IDisposable
    {
        public VertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;

        public short[] Indicies;
        private readonly int m_PrimitiveCount;

        public GlassVertexDeclaration[] Verticies;

        public GlassModelCombined(GraphicsDevice graphicsDevice, GlassVertexDeclaration[] verticies, short[] indicies, Int32 primitiveCount)
        {
            Verticies = verticies;
            Indicies = indicies;
            m_PrimitiveCount = primitiveCount;

            VertexBuffer = new VertexBuffer(graphicsDevice, GlassVertexDeclaration.VertexDeclaration, verticies.Length, BufferUsage.None);
            VertexBuffer.SetData(Verticies);


            IndexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indicies.Length, BufferUsage.None);
            IndexBuffer.SetData(indicies);
        }

        public int PrimitiveCount
        {
            get { return m_PrimitiveCount; }
        }

        private static Int32 GetVertexCount(Model model)
        {
            var rv = 0;
            var meshCount = model.Meshes.Count;
            for (int i = 0; i < meshCount; i++)
            {
                var part = model.Meshes[i].MeshParts[0];
                rv += part.NumVertices;
            }
            return rv;
        }

        public static Matrix GetAbsoluteTransform(ModelBone bone)
        {
            if (bone == null)
            {
                return Matrix.Identity;
            }
            return bone.Transform * GetAbsoluteTransform(bone.Parent);
        }

    

        public static GlassModelCombined FromModel(GraphicsDevice graphicsDevice, Model model)
        {
           
            // Combine Meshes
            var meshCount = model.Meshes.Count;
            var vertexCount = model.Meshes[01].MeshParts[0].VertexBuffer.VertexCount;

            // Declare new arrays
            var vertexData = new VertexPositionNormalTextureColor[vertexCount];

            var transforms = new Matrix[model.Bones.Count];

            model.CopyAbsoluteBoneTransformsTo(transforms);

            for (int i = 0; i < model.Meshes.Count; i++)
            {
                var mesh = model.Meshes[i];
                var part = mesh.MeshParts[0];
                var vertCount = part.NumVertices;
                
                var vertexElements = new VertexPositionNormalTextureColor[vertCount];
                var stride = VertexPositionNormalTextureColor.VertexDeclaration.VertexStride;
                model.Meshes[0].MeshParts[0].VertexBuffer.GetData(part.VertexOffset*stride, vertexElements, 0, vertCount, stride);


                var transform = transforms[mesh.ParentBone.Index];
                
                for (int j = 0; j < vertCount; j++)
                {
                    vertexData[part.VertexOffset + j] = vertexElements[j];

                }

            }

            var primitiveCount = 0;
            var indicies = new short[model.Meshes[0].MeshParts[0].IndexBuffer.IndexCount];
            model.Meshes[0].MeshParts[0].IndexBuffer.GetData(indicies);

            for (int i = 0; i < model.Meshes.Count; i++)
            {
                var part = model.Meshes[i].MeshParts[0];

                var startIndex = part.StartIndex;
                var indexCount = part.PrimitiveCount*3;
                var numVerts = part.NumVertices;

                primitiveCount += part.PrimitiveCount;
                var indexElements = new short[indexCount];
                part.IndexBuffer.GetData(startIndex*2, indexElements, 0, indexCount);

              
                for (int j = 0; j < indexCount; j++)
                {
                    indicies[j + startIndex] = (short) (indexElements[j] + startIndex);
                }
            }


            var r = new Random();
            var glassVertices = new GlassVertexDeclaration[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                var vert = vertexData[i];
                glassVertices[i].Position = vert.Position;
                glassVertices[i].Position.Z = 0;
                glassVertices[i].TextureCoordinate = vert.TextureCoordinate;
                glassVertices[i].Normal = vert.Normal;
                glassVertices[i].Color = vert.Color;
            }



            return new GlassModelCombined(graphicsDevice, glassVertices, indicies, primitiveCount);
        }

        public void Dispose()
        {
            if (VertexBuffer != null)
            {
                VertexBuffer.Dispose();
                VertexBuffer = null;
            }
        }
    }
}
