using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bembelbuben.Core.Rendering.PrimitiveRenderers
{
    public static class TextureRenderer
    {
        public static void DrawTexture(
            GraphicsDevice graphicsDevice,
            Effect effect,
            Matrix world, Matrix view, Matrix projection,
            Color color, Texture2D texture2D,
            Rectangle rectangle,
            float rotation = 0f, float zLayer = 0f)
        {

            PrimitiveRenderer.BasicEffect.TextureEnabled = true;
            PrimitiveRenderer.BasicEffect.Texture = texture2D;
            
            QuadRenderer.DrawQuadF(
                graphicsDevice,
                effect,
                world, view, projection,
                color,
                rectangle,
                rotation, zLayer
            );
            
            PrimitiveRenderer.BasicEffect.TextureEnabled = false;
        }
        
        public static void DrawTexture(
            GraphicsDevice graphicsDevice,
            Effect effect,
            Matrix world, Matrix view, Matrix projection,
            Color color, Texture2D texture2D,
            Vector2 position,
            float rotation = 0f, float zLayer = 0f)
        {

            PrimitiveRenderer.BasicEffect.TextureEnabled = true;
            PrimitiveRenderer.BasicEffect.Texture = texture2D;
            
            QuadRenderer.DrawQuadF(
                graphicsDevice,
                effect,
                world, view, projection,
                color,
                position, texture2D.Bounds.Size.ToVector2(),
                rotation, zLayer
            );
            
            PrimitiveRenderer.BasicEffect.TextureEnabled = false;
        }
        
        public static void DrawTexture(
            GraphicsDevice graphicsDevice,
            Effect effect,
            Matrix world, Matrix view, Matrix projection,
            Color color, Texture2D texture2D,
            Rectangle rectangle, Vector2 origin,
            float rotation = 0f, float zLayer = 0f)
        {

            PrimitiveRenderer.BasicEffect.TextureEnabled = true;
            PrimitiveRenderer.BasicEffect.Texture = texture2D;
            
            QuadRenderer.DrawQuadF(
                graphicsDevice,
                effect,
                world, view, projection,
                color,
                rectangle, origin,
                rotation, zLayer
            );
            
            PrimitiveRenderer.BasicEffect.TextureEnabled = false;
        }
        
        public static void DrawTexture(
            GraphicsDevice graphicsDevice,
            Effect effect,
            Matrix world, Matrix view, Matrix projection,
            Color color, Texture2D texture2D,
            Vector2 position, Vector2 origin, 
            float rotation = 0f, float zLayer = 0f)
        {

            PrimitiveRenderer.BasicEffect.TextureEnabled = true;
            PrimitiveRenderer.BasicEffect.Texture = texture2D;
            
            QuadRenderer.DrawQuadF(
                graphicsDevice,
                effect,
                world, view, projection,
                color,
                position, texture2D.Bounds.Size.ToVector2(), origin,
                rotation, zLayer
            );
            
            PrimitiveRenderer.BasicEffect.TextureEnabled = false;
        }
        
    }
}