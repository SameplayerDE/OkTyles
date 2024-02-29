using Microsoft.Xna.Framework;

namespace Bembelbuben.Core.UserInterface;

public class HStack : UserInterfaceNodeContainer
{

    public Alignment Alignment = Alignment.Top;
    public float Spacing = 20;
    public float PaddingLeft { get; private set; }
    public float PaddingTop { get; private set; }
    public float PaddingRight { get; private set; }
    public float PaddingBottom { get; private set; }
    
    public HStack(IEnumerable<UserInterfaceNode> nodes) : base(UserInterfaceNodeType.HStack)
    {
        Children.AddRange(nodes);
    }
    
    public HStack(params UserInterfaceNode[] nodes) : base(UserInterfaceNodeType.HStack)
    {
        Children.AddRange(nodes);
    }

    public HStack SetAlignment(Alignment value)
    {
        Alignment = value;
        return this;
    }
    
    public HStack SetSpacing(float value)
    {
        Spacing = value;
        return this;
    }
    
    public HStack SetPadding(float value)
    {
        SetPaddingLeft(value);
        SetPaddingTop(value);
        SetPaddingRight(value);
        SetPaddingBottom(value);
        return this;
    }
    
    public HStack SetPaddingLeft(float value)
    {
        PaddingLeft = value;
        return this;
    }

    public HStack SetPaddingTop(float value)
    {
        PaddingTop = value;
        return this;
    }

    public HStack SetPaddingRight(float value)
    {
        PaddingRight = value;
        return this;
    }

    public HStack SetPaddingBottom(float value)
    {
        PaddingBottom = value;
        return this;
    }
    
}