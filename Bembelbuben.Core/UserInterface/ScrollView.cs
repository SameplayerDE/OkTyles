namespace Bembelbuben.Core.UserInterface;

public class ScrollView : UserInterfaceNodeContainer
{
    public ScrollView(IEnumerable<UserInterfaceNode> nodes) : base(UserInterfaceNodeType.ScrollView)
    {
        Children.AddRange(nodes);
    }
    
    public ScrollView(params UserInterfaceNode[] nodes) : base(UserInterfaceNodeType.ScrollView)
    {
        Children.AddRange(nodes);
    }
}