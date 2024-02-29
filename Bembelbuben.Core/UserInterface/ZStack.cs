namespace Bembelbuben.Core.UserInterface;

public class ZStack : UserInterfaceNodeContainer
{

    public float Spacing = 0;
    public float PaddingLeft { get; private set; }
    public float PaddingTop { get; private set; }
    public float PaddingRight { get; private set; }
    public float PaddingBottom { get; private set; }
    
    public ZStack(params UserInterfaceNode[] nodes) : base(UserInterfaceNodeType.ZStack)
    {
        Children.AddRange(nodes);
    }

    public ZStack SetSpacing(float value)
    {
        Spacing = value;
        return this;
    }
    
    public ZStack SetPadding(float value)
    {
        SetPaddingLeft(value);
        SetPaddingTop(value);
        SetPaddingRight(value);
        SetPaddingBottom(value);
        return this;
    }
    
    public ZStack SetPaddingLeft(float value)
    {
        PaddingLeft = value;
        return this;
    }

    public ZStack SetPaddingTop(float value)
    {
        PaddingTop = value;
        return this;
    }

    public ZStack SetPaddingRight(float value)
    {
        PaddingRight = value;
        return this;
    }

    public ZStack SetPaddingBottom(float value)
    {
        PaddingBottom = value;
        return this;
    }
    
}