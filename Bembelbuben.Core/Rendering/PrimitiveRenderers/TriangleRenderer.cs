using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bembelbuben.Core.Rendering.PrimitiveRenderers
{
    public static class TriangleRenderer
    {
        private static readonly VertexPositionColorTexture[] _vertices;
        private static readonly int[] _indices;

        static TriangleRenderer()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            _vertices = new VertexPositionColorTexture[3];
            
            _vertices[0].TextureCoordinate = Vector2.Zero;
            _vertices[1].TextureCoordinate = Vector2.UnitX;
            _vertices[2].TextureCoordinate = Vector2.One;
            
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            _indices = new[]
            {
                0, 1,
                1, 2,
                2, 0
            };
        }

        public static void DrawTriangleF(
            GraphicsDevice graphicsDevice,
            Effect effect,
            Matrix world, Matrix view, Matrix projection,
            Color color,
            Vector3 a, Vector3 b, Vector3 c)
        {
            _vertices[0].Position = a;
            _vertices[0].Color = color;

            _vertices[1].Position = b;
            _vertices[1].Color = color;

            _vertices[2].Position = c;
            _vertices[2].Color = color;

            if (effect is BasicEffect basicEffect)
            {
                basicEffect.World = world;
                basicEffect.View = view;
                basicEffect.Projection = projection;
            }
            else
            {
                effect.Parameters["WorldViewProjection"]
                    ?.SetValue(world * view * projection);
            }

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, 1);
            }
        }

        public static void DrawTriangleF(
            GraphicsDevice graphicsDevice,
            Effect effect,
            Matrix world, Matrix view, Matrix projection,
            Color color,
            Vector2 a, Vector2 b, Vector2 c,
            float rotation = 0f,
            float zLayer = 0f)
        {
            //var a3 = new Vector3(a, zLayer);
            //var b3 = new Vector3(b, zLayer);
            //var c3 = new Vector3(c, zLayer);

            var (xorigin, yorigin) = new Vector2(0, 0);

            xorigin = Math.Min(a.X, Math.Min(b.X, c.X));
            yorigin = Math.Min(a.Y, Math.Min(b.Y, c.Y));

            DrawTriangleF(
                graphicsDevice,
                effect,
                world, view, projection,
                color,
                a, b, c,
                new Vector2(0, 0),
                rotation, zLayer
            );
        }
        
        public static void DrawTriangleF(
            GraphicsDevice graphicsDevice,
            Effect effect,
            Matrix world, Matrix view, Matrix projection,
            Color color,
            Vector2 a, Vector2 b, Vector2 c,
            Vector2 origin,
            float rotation = 0f,
            float zLayer = 0f)
        {

            var a3 = new Vector3(a, zLayer);
            var b3 = new Vector3(b, zLayer);
            var c3 = new Vector3(c, zLayer);

            a3 -= new Vector3(origin, zLayer);
            b3 -= new Vector3(origin, zLayer);
            c3 -= new Vector3(origin, zLayer);

            a3 = Vector3.Transform(a3, Matrix.CreateRotationZ(rotation));
            b3 = Vector3.Transform(b3, Matrix.CreateRotationZ(rotation));
            c3 = Vector3.Transform(c3, Matrix.CreateRotationZ(rotation));

            a3 += new Vector3(origin, zLayer);
            b3 += new Vector3(origin, zLayer);
            c3 += new Vector3(origin, zLayer);

            DrawTriangleF(
                graphicsDevice,
                effect,
                world, view, projection,
                color,
                a3, b3, c3
            );
        }

        public static void DrawTriangleF(
            GraphicsDevice graphicsDevice,
            Effect effect,
            Matrix world, Matrix view, Matrix projection,
            Color color,
            Point a, Point b, Point c,
            float zLayer = 0)
        {
            DrawTriangleF(
                graphicsDevice,
                effect,
                world, view, projection,
                color,
                a.ToVector2(), b.ToVector2(), c.ToVector2(),
                zLayer
            );
        }

        public static void DrawTriangleH(
            GraphicsDevice graphicsDevice,
            Effect effect,
            Matrix world, Matrix view, Matrix projection,
            Color color,
            Vector3 a, Vector3 b, Vector3 c)
        {
            _vertices[0].Position = a;
            _vertices[0].Color = color;

            _vertices[1].Position = b;
            _vertices[1].Color = color;

            _vertices[2].Position = c;
            _vertices[2].Color = color;

            if (effect is BasicEffect basicEffect)
            {
                basicEffect.World = world;
                basicEffect.View = view;
                basicEffect.Projection = projection;
            }
            else
            {
                effect.Parameters["WorldViewProjection"]
                    ?.SetValue(world * view * projection);
            }

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    _vertices, 0, 3,
                    _indices, 0,
                    3
                );
            }
        }

        public static void DrawTriangleH(
            GraphicsDevice graphicsDevice,
            Effect effect,
            Matrix world, Matrix view, Matrix projection,
            Color color,
            Vector2 a, Vector2 b, Vector2 c,
            float zLayer = 0)
        {
            var a3 = new Vector3(a, zLayer);
            var b3 = new Vector3(b, zLayer);
            var c3 = new Vector3(c, zLayer);

            DrawTriangleH(
                graphicsDevice,
                effect,
                world, view, projection,
                color,
                a3, b3, c3
            );
        }

        public static void DrawTriangleH(
            GraphicsDevice graphicsDevice,
            Effect effect,
            Matrix world, Matrix view, Matrix projection,
            Color color,
            Point a, Point b, Point c,
            float zLayer = 0)
        {
            DrawTriangleH(
                graphicsDevice,
                effect,
                world, view, projection,
                color,
                a.ToVector2(), b.ToVector2(), c.ToVector2(),
                zLayer
            );
        }
    }
}