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

    public void SetTileData(int x, int y, int layer, uint value)
    {
        if (layer < 0 || layer >= LayerCount || x < 0 || x >= Width || y < 0 || y >= Height)
        {
            throw new IndexOutOfRangeException();
        }

        Layers[layer][x, y] = value;
    }
    
    public uint GetTileData(int x, int y, int layer)
    {
        if (layer < 0 || layer >= LayerCount || x < 0 || x >= Width || y < 0 || y >= Height)
        {
            throw new IndexOutOfRangeException();
        }

        return Layers[layer][x, y];
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

    public void Rotate(int x, int y, int layer)
    {
        var mirrorState = GetTileMirror(x, y, layer);
        uint mirrorValue = 0u;
        switch (mirrorState)
        {
            case 0b00:
                mirrorValue = 0b01;
                break;
            case 0b01:
                mirrorValue = 0b11;
                break;
            case 0b10:
                mirrorValue = 0b00;
                break;
            case 0b11:
                mirrorValue = 0b10;
                break;
            default:
                mirrorValue = 0u;
                break;
        }

        SetTileMirror(x, y, mirrorValue, layer);
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
    
    public void AddLayer()
    {
        Array.Resize(ref Layers, LayerCount + 1);
        Layers[LayerCount] = new uint[Width, Height];
        InitializeLayer(Layers[LayerCount]);
        LayerCount++;
    }

    public void RemoveLayer(int layerIndex)
    {
        if (layerIndex < 0 || layerIndex >= LayerCount)
        {
            throw new IndexOutOfRangeException();
        }
    
        // Shift layers down
        for (int i = layerIndex; i < LayerCount - 1; i++)
        {
            Layers[i] = Layers[i + 1];
        }

        // Resize the array
        Array.Resize(ref Layers, LayerCount - 1);
        LayerCount--;
    }
    
    public void SwapLayers(int layerIndex1, int layerIndex2)
    {
        if (layerIndex1 < 0 || layerIndex1 >= LayerCount || layerIndex2 < 0 || layerIndex2 >= LayerCount)
        {
            throw new IndexOutOfRangeException();
        }

        var tempLayer = Layers[layerIndex1];
        Layers[layerIndex1] = Layers[layerIndex2];
        Layers[layerIndex2] = tempLayer;
    }
    
}