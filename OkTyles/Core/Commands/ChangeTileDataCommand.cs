using Bembelbuben.Core;

namespace OkTyles.Core.Commands;

public class ChangeTileDataCommand : Command
{
    private readonly World _world;
    private readonly int _x;
    private readonly int _y;
    private readonly int _layer;
    private readonly uint _prevTileData;
    private readonly uint _newTileData;

    public ChangeTileDataCommand(World world, int x, int y, int layer, uint prevTileData, uint newTileData)
    {
        _world = world;
        _x = x;
        _y = y;
        _layer = layer;
        _prevTileData = prevTileData;
        _newTileData = newTileData;
    }

    public override bool Execute()
    {
        _world.SetTileData(_x, _y, _layer, _newTileData);
        return true;
    }
    
    public override void Undo()
    {
        _world.SetTileData(_x, _y, _layer, _prevTileData);
    }
}
