namespace Bembelbuben.Core.UserInterface;

public class VStack : UserInterfaceNodeContainer
{

    public Alignment Alignment = Alignment.Left;
    public float Spacing = 0;
    public float PaddingLeft { get; private set; }
    public float PaddingTop { get; private set; }
    public float PaddingRight { get; private set; }
    public float PaddingBottom { get; private set; }
    
    public VStack(params UserInterfaceNode[] nodes) : base(UserInterfaceNodeType.VStack)
    {
        Children.AddRange(nodes);
    }

    public VStack SetSpacing(float value)
    {
        Spacing = value;
        return this;
    }
    
    public VStack SetAlignment(Alignment value)
    {
        Alignment = value;
        return this;
    }
    
    public VStack SetPadding(float value)
    {
        SetPaddingLeft(value);
        SetPaddingTop(value);
        SetPaddingRight(value);
        SetPaddingBottom(value);
        return this;
    }
    
    public VStack SetPaddingLeft(float value)
    {
        PaddingLeft = value;
        return this;
    }

    public VStack SetPaddingTop(float value)
    {
        PaddingTop = value;
        return this;
    }

    public VStack SetPaddingRight(float value)
    {
        PaddingRight = value;
        return this;
    }

    public VStack SetPaddingBottom(float value)
    {
        PaddingBottom = value;
        return this;
    }
    
}