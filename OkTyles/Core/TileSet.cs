using System;
using System.IO;
using Bembelbuben.Core;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace OkTyles.Core;

public class TileSet
{
    [JsonIgnore]
    public Texture2D Texture2D;
    
    public string TexturePath;
    public int TileCount;
    public int TileWidth;
    public int TileHeight;
    public int TilesPerRow;
    public int TilesPerColumn;

    public static TileSet ReadFromFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException();
        }
        var content = File.ReadAllText(path);
        var tileSet = JsonConvert.DeserializeObject<TileSet>(content);
        if (tileSet == null || Context.GraphicsDevice == null)
        {
            throw new NullReferenceException();
        }
        tileSet.Texture2D = EditorUtils.LoadTextureFromPath(Path.Combine(Path.GetDirectoryName(path) ?? string.Empty, tileSet.TexturePath), Context.GraphicsDevice);
        return tileSet;
    }
    
    public static void WriteToFile(TileSet tileSet, string path)
    {
        if (tileSet == null)
        {
            throw new NullReferenceException();
        }
        var json = JsonConvert.SerializeObject(tileSet, Formatting.Indented);
        var writer = File.CreateText(path);
        writer.Write(json);
        writer.Flush();
        writer.Close();
        writer.Dispose();
    }
}