using Bembelbuben.Core.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bembelbuben.Core;

public static class Context
{
    public static Dictionary<string, SpriteFont> Fonts = new Dictionary<string, SpriteFont>();
    public static Texture2D Pixel;
    public static GraphicsDevice GraphicsDevice;
    public static ContentManager ContentManager;
    public static InputHandle Input;
}