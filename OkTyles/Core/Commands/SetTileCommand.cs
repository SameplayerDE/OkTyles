using Bembelbuben.Core;

namespace OkTyles.Core.Commands;

public class SetTileCommand : Command
{
    private readonly World _world;
    private readonly int _x;
    private readonly int _y;
    private readonly int _layer;
    private readonly uint _oldTileId;
    private readonly uint _newTileId;

    public SetTileCommand(World world, int x, int y, int layer, uint oldTileId, uint newTileId)
    {
        _world = world;
        _x = x;
        _y = y;
        _layer = layer;
        _oldTileId = oldTileId;
        _newTileId = newTileId;
    }

    public override bool Execute()
    {
        _world.SetTileTexture(_x, _y, _newTileId, _layer);
        return true;
    }
    
    public override void Undo()
    {
        _world.SetTileTexture(_x, _y, _oldTileId, _layer);
    }
}
