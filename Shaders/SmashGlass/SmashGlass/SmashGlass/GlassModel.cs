using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SmashGlass
{
    public class GlassPart
    {
        public Int32 VertexOffset { get; set; }

        public Int32 NumVerticies { get; set; }

        public Int32 StartIndex { get; set; }

        public Int32 PrimitiveCount { get; set; }
    }

    public class GlassModelShard
    {
        public List<GlassPart> Parts;   // Should only be one part

        private readonly VertexPositionNormalTextureColor[] m_Verticies;

        public Vector3 Rotation { get; set; }

        public Vector3 Centre { get; set; }

        public Vector2 DirToCentre { get; set; }

        public float DistanceFromImpact { get; set; }

        public float Dampining { get; set; }

        public VertexPositionNormalTextureColor[] Verticies
        {
            get { return m_Verticies; }
        }

        public GlassModelShard(List<GlassPart> parts, 
                               Vector3 rotation, 
                               VertexPositionNormalTextureColor[] verticies)
        {
            Parts = parts;
            m_Verticies = verticies;
            Rotation = rotation;

           
            // Calculate centre
            var pos = Vector3.Zero;
            for (int i = 0; i < verticies.Length; i++)
            {
                pos += verticies[i].Position;
            }
            pos /= verticies.Length;


            var collisionPt = new Vector2(-0.625f, -0.55f);
            var pos2D = new Vector2(pos.X, pos.Y);

            DistanceFromImpact = Vector2.Distance(pos2D, collisionPt);
            Dampining = 1/DistanceFromImpact;
            Dampining*=0.05f;
       
            Centre = pos;

            DirToCentre = pos2D - collisionPt;
        }
    }

    public class GlassModel
    {
        private readonly VertexBuffer m_VertexBuffer;
        private readonly IndexBuffer m_IndexBuffer;
        public List<GlassModelShard> Meshes; 

        public GlassModel(VertexBuffer vertexBuffer,
                          IndexBuffer indexBuffer,
                          List<GlassModelShard> meshes)
        {
            m_VertexBuffer = vertexBuffer;
            m_IndexBuffer = indexBuffer;
            Meshes = meshes;
        }

        public VertexBuffer VertexBuffer
        {
            get { return m_VertexBuffer; }
        }

        public IndexBuffer IndexBuffer
        {
            get { return m_IndexBuffer; }
        }

        public static GlassModel FromModel(Model model)
        {
            var random = new Random();

            var meshLst = new List<GlassModelShard>();
            var vb = model.Meshes[0].MeshParts[0].VertexBuffer;
            var ib = model.Meshes[0].MeshParts[0].IndexBuffer;
            foreach (var mesh in model.Meshes)
            {
                var partLst = new List<GlassPart>();
                foreach (var part in mesh.MeshParts)
                {
                    var glassPart = new GlassPart
                        {
                            StartIndex = part.StartIndex,
                            NumVerticies = part.NumVertices,
                            PrimitiveCount = part.PrimitiveCount,
                            VertexOffset = part.VertexOffset
                        };

                    partLst.Add(glassPart);
                }

                var randZ = random.Next(1, 2);
                var randX = random.Next(1, 2);
                var randY = random.Next(1, 2);

                var flipX = random.Next(0, 2);
                if (flipX == 0)
                    randX *= -1;

                var flipY = random.Next(0, 2);
                if (flipY == 0)
                    randY *= -1;

                var flipZ = random.Next(0, 2);
                if (flipZ == 0)
                    randZ *= -1;
           
               
                // Get the centre of the shard
                var stride = vb.VertexDeclaration.VertexStride;
                var numVerticies = mesh.MeshParts[0].NumVertices;
                var vertexOffset = mesh.MeshParts[0].VertexOffset;

                var vertexData = new VertexPositionNormalTextureColor[numVerticies];
                mesh.MeshParts[0].VertexBuffer.GetData(vertexOffset * stride, vertexData, 0, numVerticies, stride);

                // Distance from impact pt centre

                var roation = new Vector3(randX, randY, randZ);

                meshLst.Add(new GlassModelShard(partLst, roation, vertexData));
            }

            return new GlassModel(vb, ib, meshLst);
        }
    }
}
