namespace Bembelbuben.Core.UserInterface;

public class ScrollView : UserInterfaceNodeContainer
{

    public int Value;
    
    public ScrollView(IEnumerable<UserInterfaceNode> nodes) : base(UserInterfaceNodeType.ScrollView)
    {
        Children.AddRange(nodes);
        //IsVisible = true;
        //IsClickable = true;
    }
    
    public ScrollView(params UserInterfaceNode[] nodes) : base(UserInterfaceNodeType.ScrollView)
    {
        Children.AddRange(nodes);
        //IsVisible = true;
        //IsClickable = true;
    }
}