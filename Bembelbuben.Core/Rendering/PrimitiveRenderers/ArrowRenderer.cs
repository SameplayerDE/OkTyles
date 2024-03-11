using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bembelbuben.Core.Rendering.PrimitiveRenderers
{
    public static class ArrowRenderer
    {
        private static readonly VertexPositionColorTexture[] _verticesLine;
        private static readonly VertexPositionColorTexture[] _verticesTip;

        static ArrowRenderer()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            _verticesLine = new VertexPositionColorTexture[2];

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            _verticesTip = new VertexPositionColorTexture[3];
        }

        public static void DrawArrowF(
            GraphicsDevice graphicsDevice,
            Effect effect,
            Matrix world, Matrix view, Matrix projection,
            Color color,
            Vector2 position, Vector2 direction,
            float length = 1f,
            float tipScale = 1f,
            float zLayer = 0f)
        {
            var a = position;
            var b = direction * length + a;

            var aTip = new Vector2(0, -1f * tipScale);
            var bTip = new Vector2(+1f * tipScale, 0);
            var cTip = new Vector2(0, +1f * tipScale);

            aTip += b;
            bTip += b;
            cTip += b;

            LineRenderer.DrawLine(
                graphicsDevice, effect,
                world, view, projection,
                color, a, b, zLayer
            );

            var rotation = (float)Math.Atan2(b.Y - a.Y, b.X - a.X);
            
            TriangleRenderer.DrawTriangleF(
                graphicsDevice, effect,
                world, view, projection,
                color,
                aTip,
                bTip,
                cTip,
                b,
                rotation, zLayer
            );

            /*if (effect is BasicEffect basicEffect)
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

                graphicsDevice.DrawUserPrimitives(
                    PrimitiveType.LineList,
                    _verticesLine, 0, 1
                );
                graphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList,
                    _verticesTip, 0, 1
                );
            }*/
        }
    }
}