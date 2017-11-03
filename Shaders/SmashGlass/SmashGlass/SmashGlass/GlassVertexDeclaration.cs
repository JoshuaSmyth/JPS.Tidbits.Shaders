using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SmashGlass
{
    public struct GlassVertexDeclaration : IVertexType
    {
        public Vector3 Position;    

        public Vector3 Normal;

        public Vector2 TextureCoordinate;

        public Color Color;

       // public Vector3 CentreOfMass;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
           new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
           new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
           new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
           new VertexElement(32, VertexElementFormat.Color, VertexElementUsage.Color, 0)
         //  new VertexElement(36, VertexElementFormat.Vector3, VertexElementUsage.Normal, 1)
        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }
}
