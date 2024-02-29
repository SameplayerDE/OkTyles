namespace Bembelbuben.Core.UserInterface;

public class Image : UserInterfaceNode
{

    public float Scale = 1.0f;
    public string ImageIdentifier = string.Empty;
    
    public Image(string identifier) : base(UserInterfaceNodeType.Image)
    {
        ImageIdentifier = identifier;
    }

    public Image SetScale(float value)
    {
        Scale = value;
        return this;
    }

    public Image SetImageIdentifier(string identifier)
    {
        ImageIdentifier = identifier;
        return this;
    }
    
}