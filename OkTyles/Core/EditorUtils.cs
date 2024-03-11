using System;
using System.IO;
using Bembelbuben.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OkTyles.Core;

public static class EditorUtils
{

    public static Vector2 MapWorldToScreen(Vector2 position, Camera camera)
    {
       //var x = (position.X - camera.X) * camera.Zoom;
       //var y = (position.Y - camera.Y) * camera.Zoom;

       //return new Vector2(x, y);
       return MapWorldToScreen(position, camera.TransformationMatrix);
    }
    
    public static Vector2 MapScreenToWorld(Vector2 position, Camera camera)
    {
        //var x = position.X / camera.Zoom + camera.X;
        //var y = position.Y / camera.Zoom + camera.Y;
//
        //return new Vector2(x, y);
        return MapScreenToWorld(position, camera.TransformationMatrix);
    }

    public static Vector2 MapWorldToScreen(Vector2 position, Matrix transformationMatrix)
    {
        Vector2 transformedPosition = Vector2.Transform(position, transformationMatrix);
        return transformedPosition;
    }

    public static Vector2 MapScreenToWorld(Vector2 position, Matrix transformationMatrix)
    {
        // Wende die Umkehrung der Transformation auf die Bildschirmkoordinaten an
        Matrix inverseMatrix = Matrix.Invert(transformationMatrix);
        Vector2 transformedPosition = Vector2.Transform(position, inverseMatrix);
        return transformedPosition;
    }
    
    public static Texture2D LoadTextureFromPath(string path, GraphicsDevice graphicsDevice)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException();
        }

        if (Context.GraphicsDevice == null)
        {
            throw new NullReferenceException();
        }
        
        var stream = File.Open(path, FileMode.Open);
        var texture = Texture2D.FromStream(graphicsDevice, stream);
        
        stream.Close();
        stream.Dispose();

        return texture;
    }
}