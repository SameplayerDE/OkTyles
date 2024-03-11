using Bembelbuben.Core.Rendering.PrimitiveRenderers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bembelbuben.Core.Rendering;

    public static class PrimitiveRenderer
    {
        public static GraphicsDevice GraphicsDevice;
        public static BasicEffect BasicEffect;
        public static Matrix World, View, Projection;
        public static float Scale;
        public static Vector3 ViewOffset;

        private static bool _initialised;

        static PrimitiveRenderer()
        {
            _initialised = false;

            Scale = 1f;
            ViewOffset = Vector3.Zero;
        }

        public static void Initialise(GraphicsDevice graphicsDevice)
        {
            if (_initialised) return;
            _initialised = true;
            GraphicsDevice = graphicsDevice;
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            BasicEffect = new BasicEffect(GraphicsDevice);
            BasicEffect.VertexColorEnabled = true;
        }

        public static void UpdateDefaultCamera()
        {
            var (width, height) = GraphicsDevice.Viewport.Bounds.Size.ToVector2();
            
            PrimitiveRenderer.World = Matrix.Identity;
            PrimitiveRenderer.View = Matrix.CreateLookAt(
                ViewOffset + Vector3.Backward,
                ViewOffset + Vector3.Forward,
                Vector3.Up
            );
            PrimitiveRenderer.Projection = Matrix.CreateOrthographic(
                width * Scale,
                -height * Scale,
                0.1f, 10f
            );
        }

        #region TextureRenderer

        public static void DrawTexture(
            Effect effect,
            Color color, Texture2D texture2D,
            Rectangle rectangle,
            float rotation = 0f,
            float zLayer = 0f)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            TextureRenderer.DrawTexture(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color, texture2D,
                rectangle,
                rotation, zLayer
            );
        }

        public static void DrawTexture(
            Effect effect,
            Color color, Texture2D texture2D,
            Vector2 position,
            float rotation = 0f, float zLayer = 0f)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            TextureRenderer.DrawTexture(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color, texture2D,
                position,
                rotation, zLayer
            );
        }

        public static void DrawTexture(
            Effect effect,
            Color color, Texture2D texture2D,
            Rectangle rectangle, Vector2 origin,
            float rotation = 0f,
            float zLayer = 0f)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            TextureRenderer.DrawTexture(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color, texture2D,
                rectangle, origin,
                rotation, zLayer
            );
        }

        public static void DrawTexture(
            Effect effect,
            Color color, Texture2D texture2D,
            Vector2 position, Vector2 origin,
            float rotation = 0f, float zLayer = 0f)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            TextureRenderer.DrawTexture(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color, texture2D,
                position, origin,
                rotation, zLayer
            );
        }

        #endregion

        #region ArrowRenderer

        public static void DrawArrowF(
            Effect effect,
            Color color,
            Vector2 position, Vector2 direction,
            float length = 1f, float tipScale = 1f,
            float zLayer = 0f)
        {
            ArrowRenderer.DrawArrowF(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                position, direction,
                length,
                tipScale, zLayer
            );
        }

        #endregion

        #region LineRenderer

        public static void DrawLine(
            Effect effect,
            Color color,
            Vector3 a, Vector3 b)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            LineRenderer.DrawLine(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                a, b
            );
        }

        public static void DrawLine(
            Effect effect,
            Color color,
            Vector2 a, Vector2 b,
            float zLayer = 0f)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            LineRenderer.DrawLine(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                a, b,
                zLayer
            );
        }

        #endregion

        #region RectangleRenderer

        public static void DrawRectF(
            Effect effect,
            Color color,
            Vector2 position, Vector2 size,
            float radius = 0,
            float zLayer = 0)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            RectangleRenderer.DrawRectF(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                position, size,
                radius,
                zLayer
            );
        }

        public static void DrawRectF(
            Effect effect,
            Color color,
            Rectangle rectangle,
            float radius = 0,
            float zLayer = 0)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            RectangleRenderer.DrawRectF(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                rectangle,
                radius,
                zLayer
            );
        }

        #endregion

        #region QuadRenderer

        public static void DrawQuadF(
            Effect effect,
            Color color,
            Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            QuadRenderer.DrawQuadF(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                a, b, c, d
            );
        }

        public static void DrawQuadF(
            Effect effect,
            Color color,
            Vector2 position, Vector2 size,
            float rotation = 0f,
            float zLayer = 0)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            QuadRenderer.DrawQuadF(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                position, size,
                rotation, zLayer
            );
        }

        public static void DrawQuadF(
            Effect effect,
            Color color,
            Vector2 position, Vector2 size, Vector2 origin,
            float rotation = 0f,
            float zLayer = 0)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            QuadRenderer.DrawQuadF(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                position, size, origin,
                rotation, zLayer
            );
        }

        public static void DrawQuadF(
            Effect effect,
            Color color,
            Point position, Point size,
            float zLayer = 0)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            QuadRenderer.DrawQuadF(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                position, size,
                zLayer
            );
        }

        public static void DrawQuadF(
            Effect effect,
            Color color,
            Rectangle rectangle,
            float zLayer = 0)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            QuadRenderer.DrawQuadF(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                rectangle,
                zLayer
            );
        }

        public static void DrawQuadH(
            Effect effect,
            Color color,
            Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            QuadRenderer.DrawQuadH(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                a, b, c, d
            );
        }

        public static void DrawQuadH(
            Effect effect,
            Color color,
            Vector2 position, Vector2 size,
            float zLayer = 0)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            QuadRenderer.DrawQuadH(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                position, size,
                zLayer
            );
        }

        public static void DrawQuadH(
            Effect effect,
            Color color,
            Point position, Point size,
            float zLayer = 0)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            QuadRenderer.DrawQuadH(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                position, size,
                zLayer
            );
        }

        public static void DrawQuadH(
            Effect effect,
            Color color,
            Rectangle rectangle,
            float zLayer = 0)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            QuadRenderer.DrawQuadH(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                rectangle,
                zLayer
            );
        }

        #endregion

        #region TriangleRenderer

        public static void DrawTriangleF(
            Effect effect,
            Color color,
            Vector3 a, Vector3 b, Vector3 c)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            TriangleRenderer.DrawTriangleF(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                a, b, c
            );
        }

        public static void DrawTriangleF(
            Effect effect,
            Color color,
            Vector2 a, Vector2 b, Vector2 c,
            float zLayer = 0)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            TriangleRenderer.DrawTriangleF(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                a, b, c,
                zLayer
            );
        }
        
        public static void DrawTriangleF(
            Effect effect,
            Color color,
            Vector2 a, Vector2 b, Vector2 c,
            Vector2 origin, float rotation,
            float zLayer = 0f)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            TriangleRenderer.DrawTriangleF(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                a, b, c, 
                origin, rotation,
                zLayer
            );
        }
        
        public static void DrawTriangleF(
            Effect effect,
            Color color,
            Vector2 a, Vector2 b, Vector2 c,
            float rotation,
            float zLayer = 0f)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            TriangleRenderer.DrawTriangleF(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                a, b, c, 
                rotation,
                zLayer
            );
        }

        public static void DrawTriangleF(
            Effect effect,
            Color color,
            Point a, Point b, Point c,
            float zLayer = 0)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            TriangleRenderer.DrawTriangleF(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                a, b, c,
                zLayer
            );
        }

        public static void DrawTriangleH(
            Effect effect,
            Color color,
            Vector3 a, Vector3 b, Vector3 c)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            TriangleRenderer.DrawTriangleH(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                a, b, c
            );
        }

        public static void DrawTriangleH(
            Effect effect,
            Color color,
            Vector2 a, Vector2 b, Vector2 c,
            float zLayer = 0)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            TriangleRenderer.DrawTriangleH(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                a, b, c,
                zLayer
            );
        }

        public static void DrawTriangleH(
            Effect effect,
            Color color,
            Point a, Point b, Point c,
            float zLayer = 0)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            TriangleRenderer.DrawTriangleH(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                a, b, c,
                zLayer
            );
        }

        #endregion

        #region CircleRenderer

        public static void DrawCircleF(
            Effect effect,
            Color color,
            Vector2 center,
            float radius,
            int resolution = 3,
            float zLayer = 0f)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            CircleRender.DrawCircleF(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                center,
                radius,
                resolution,
                zLayer
            );
        }

        public static void DrawCircleH(
            Effect effect,
            Color color,
            Vector2 center,
            float radius,
            int resolution = 3,
            float zLayer = 0f)
        {
            if (!_initialised) throw new Exception("PrimitiveRender has never been initialised");
            CircleRender.DrawCircleH(
                GraphicsDevice,
                effect ?? BasicEffect,
                World, View, Projection,
                color,
                center,
                radius,
                resolution,
                zLayer
            );
        }

        #endregion
    }