using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OkTyles;

public class Camera
{
    private readonly float _minZoom = 1.0f;
    private readonly float _maxZoom = 10.0f;
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
    
    public void ZoomIn(float value = 1.0f)
    {
        value = Math.Abs(value);
        Zoom = Math.Clamp(Zoom + value, _minZoom, _maxZoom);
        UpdateTransformationMatrix();
    }


    
    public void ZoomOut(float value = 1.0f)
    {
        value = Math.Abs(value);
        Zoom = Math.Clamp(Zoom - value, _minZoom, _maxZoom);
        UpdateTransformationMatrix();
    }
    
    private void UpdateTransformationMatrix()
    {
        var viewportCenter = GraphicsDevice.Viewport.Bounds.Center.ToVector2() * 1;
        var translation = viewportCenter - new Vector2(X, Y);
        
        TransformationMatrix = Matrix.CreateTranslation(new Vector3(translation, 0));
        TransformationMatrix *= Matrix.CreateScale(Zoom);
    }
    
    public void Update(GameTime gameTime, float delta)
    {
        UpdateTransformationMatrix();
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