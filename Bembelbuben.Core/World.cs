namespace Bembelbuben.Core;

public class World
{
    public enum CollisionMask : byte
    {
        None = 0,
        Rectangle = 1,
        Circle = 2,
    }

    public int TileSize { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int LayerCount { get; private set; }
        
    public uint[][,] Layers;
        
    public World(int width, int height, int tileSize, int layerCount = 1)
    {
        TileSize = tileSize;
        Width = width;
        Height = height;
        LayerCount = layerCount;
            
        Layers = new uint[layerCount][,];
        for (int i = 0; i < layerCount; i++)
        {
            Layers[i] = new uint[width, height];
            InitializeLayer(Layers[i]);
        }
    }

    private void InitializeLayer(uint[,] layer)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                layer[x, y] = 0;
            }
        }
    }
        
    public void SetTileTexture(int x, int y, uint value, int layer = 0)
    {
        if (layer < 0 || layer >= LayerCount || x < 0 || x >= Width || y < 0 || y >= Height)
        {
            throw new IndexOutOfRangeException();
        }
        Layers[layer][x, y] &= 0b11111111_11111111_11111100_00000000;
        Layers[layer][x, y] |= value;
    }

    public void SetTileCollision(int x, int y, CollisionMask value, int layer = 0)
    {
        if (layer < 0 || layer >= LayerCount || x < 0 || x >= Width || y < 0 || y >= Height)
        {
            throw new IndexOutOfRangeException();
        }
        Layers[layer][x, y] &= 0b11111111_11111111_00000011_11111111;
        Layers[layer][x, y] |= (uint)value << 10;
    }
        
    public void SetTileMirror(int x, int y, uint value, int layer = 0)
    {
        if (layer < 0 || layer >= LayerCount || x < 0 || x >= Width || y < 0 || y >= Height)
        {
            throw new IndexOutOfRangeException();
        }
        value &= 0b00000000_00000000_00000000_00000011;
        Layers[layer][x, y] &= 0b11111111_11111100_11111111_11111111;
        Layers[layer][x, y] |= value << 16;
    }
        
    public uint GetTileTexture(int x, int y, int layer = 0)
    {
        if (layer < 0 || layer >= LayerCount || x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return 0;
        }
        var result = Layers[layer][x, y] & 0b00000000_00000000_00000011_11111111;
        return result;
    }

    public CollisionMask GetTileCollision(int x, int y, int layer = 0)
    {
        if (layer < 0 || layer >= LayerCount || x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return CollisionMask.Rectangle;
        }
        var result = Layers[layer][x, y] & 0b00000000_00000000_11111100_00000000;
        result >>= 10;
        return (CollisionMask)result;
    }
        
    public uint GetTileMirror(int x, int y, int layer = 0)
    {
        if (layer < 0 || layer >= LayerCount || x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return 0;
        }
        var result = Layers[layer][x, y] & 0b00000000_00000011_00000000_00000000;
        return result >> 16;
    }
}