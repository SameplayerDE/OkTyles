using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OkTyles;

public class Camera
{
    public GraphicsDevice GraphicsDevice;
    public float Zoom;
    public float X;
    public float Y;
    public Matrix TransformationMatrix;
    
    public Camera(GraphicsDevice graphicsDevice)
    {
        GraphicsDevice = graphicsDevice;
        X = 0;
        Y = 0;
        Zoom = 1;
    }
    
    public void ZoomIn()
    {
        Zoom = Math.Clamp(++Zoom, 1, 10);
    }

    public void ZoomOut()
    {
        Zoom = Math.Clamp(--Zoom, 1, 10);
    }
    
    public void Update(GameTime gameTime, float delta)
    {
        
        var viewportCenter = GraphicsDevice.Viewport.Bounds.Center.ToVector2() * 0;
        var translation = viewportCenter - new Vector2((float)X, (float)Y) * (float)Zoom;
        
        TransformationMatrix = Matrix.CreateScale((float)Zoom);
        TransformationMatrix *= Matrix.CreateTranslation(new Vector3(translation.ToPoint().ToVector2(), 0));
    }
    
    public Camera Copy()
    {
        return new Camera(GraphicsDevice)
        {
            Zoom = this.Zoom,
            X = this.X,
            Y = this.Y
        };
    }
    
}