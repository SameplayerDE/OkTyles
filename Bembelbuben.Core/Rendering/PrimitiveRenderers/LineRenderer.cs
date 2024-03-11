using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bembelbuben.Core.Rendering.PrimitiveRenderers
{
    public static class LineRenderer
    {
        private static readonly VertexPositionColorTexture[] _vertices;

        static LineRenderer()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            _vertices = new VertexPositionColorTexture[2];
            
            _vertices[0].TextureCoordinate = Vector2.Zero;
            _vertices[1].TextureCoordinate = Vector2.One;
        }

        public static void DrawLine(
            GraphicsDevice graphicsDevice,
            Effect effect,
            Matrix world, Matrix view, Matrix projection,
            Color color,
            Vector3 a, Vector3 b)
        {
            _vertices[0].Position = a;
            _vertices[0].Color = color;

            _vertices[1].Position = b;
            _vertices[1].Color = color;

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
                graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _vertices, 0, 1);
            }
        }

        public static void DrawLine(
            GraphicsDevice graphicsDevice,
            Effect effect,
            Matrix world, Matrix view, Matrix projection,
            Color color,
            Vector2 a, Vector2 b,
            float zLayer = 0)
        {
            var a3 = new Vector3(a, zLayer);
            var b3 = new Vector3(b, zLayer);

            DrawLine(
                graphicsDevice,
                effect,
                world, view, projection,
                color,
                a3, b3);
        }
    }
}