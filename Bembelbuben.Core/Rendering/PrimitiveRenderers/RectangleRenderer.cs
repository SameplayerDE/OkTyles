using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bembelbuben.Core.Rendering.PrimitiveRenderers
{
    public static class RectangleRenderer
    {
        private static readonly VertexPositionColorTexture[] _vertices;
        private static readonly int[] _indicesF;
        private static readonly int[] _indicesH;

        static RectangleRenderer()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            _vertices = new VertexPositionColorTexture[4];
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            _indicesF = new[]
            {
                0, 1, 2,
                2, 1, 3
            };
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            _indicesH = new[]
            {
                0, 1,
                1, 3,
                3, 2,
                2, 0
            };
        }

        public static void DrawRectF(
            GraphicsDevice graphicsDevice,
            Effect effect,
            Matrix world, Matrix view, Matrix projection,
            Color color,
            Vector2 position, Vector2 size,
            float radius = 0f,
            float zLayer = 0f)
        {
            var (x, y) = position;
            var (width, height) = size;

            var a = new Vector3(x, y, zLayer);
            var b = new Vector3(x + width, y, zLayer);
            var c = new Vector3(x, y + height, zLayer);
            var d = new Vector3(x + width, y + height, zLayer);

            _vertices[0].Position = a;
            _vertices[0].Color = color;

            _vertices[1].Position = b;
            _vertices[1].Color = color;

            _vertices[2].Position = c;
            _vertices[2].Color = color;

            _vertices[3].Position = d;
            _vertices[3].Color = color;

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
                    PrimitiveType.TriangleList,
                    _vertices, 0, 4,
                    _indicesF, 0,
                    2
                );
            }
        }

        public static void DrawRectF(
            GraphicsDevice graphicsDevice,
            Effect effect,
            Matrix world, Matrix view, Matrix projection,
            Color color,
            Rectangle rectangle,
            float radius = 0f,
            float zLayer = 0f)
        {
            var position = rectangle.Location.ToVector2();
            var size = rectangle.Size.ToVector2();
            
            DrawRectF(
                graphicsDevice,
                effect,
                world, view, projection,
                color,
                position, size,
                radius,
                zLayer
            );
        }
    }
}