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
        var x = (position.X - camera.X) * camera.Zoom;
        var y = (position.Y - camera.Y) * camera.Zoom;

        return new Vector2(x, y);
    }

    public static Vector2 MapScreenToWorld(Vector2 position, Camera camera)
    {
        var x = position.X / camera.Zoom + camera.X;
        var y = position.Y / camera.Zoom + camera.Y;

        return new Vector2(x, y);
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