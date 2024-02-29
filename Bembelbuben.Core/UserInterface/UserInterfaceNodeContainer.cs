namespace Bembelbuben.Core.UserInterface;

public class UserInterfaceNodeContainer : UserInterfaceNode
{
    public List<UserInterfaceNode> Children;

    protected UserInterfaceNodeContainer(UserInterfaceNodeType type) : base(type)
    {
        Children = new List<UserInterfaceNode>();
    }

    public void Add(UserInterfaceNode node)
    {
        Children.Add(node);
    }
}