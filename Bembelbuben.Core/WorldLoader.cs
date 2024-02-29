using Newtonsoft.Json;

namespace Bembelbuben.Core;

public static class WorldLoader
{
    public static World ReadFromFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException();
        }
        var content = File.ReadAllText(path);
        var world = JsonConvert.DeserializeObject<World>(content);
        if (world == null)
        {
            throw new NullReferenceException();
        }
        return world;
    }

    public static void WriteToFile(World world, string path)
    {
        if (world == null)
        {
            throw new NullReferenceException();
        }
        var json = JsonConvert.SerializeObject(world, Formatting.Indented);
        var writer = File.CreateText(path);
        writer.Write(json);
        writer.Flush();
        writer.Close();
        writer.Dispose();
    }
}