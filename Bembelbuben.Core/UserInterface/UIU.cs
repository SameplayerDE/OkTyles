namespace Bembelbuben.Core.UserInterface;

public static class UIU
{
    public static UserInterfaceNode HStack(params UserInterfaceNode[] nodes)
    {
        return new HStack(nodes);
    }

    public static UserInterfaceNode VStack(params UserInterfaceNode[] nodes)
    {
        return new VStack(nodes);
    }

    public static UserInterfaceNode Label(string text)
    {
        return new Label(text);
    }
    
    public static UserInterfaceNode Image(string identifier)
    {
        return new Image(identifier);
    }
}