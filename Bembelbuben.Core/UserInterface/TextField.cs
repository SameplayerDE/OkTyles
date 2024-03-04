namespace Bembelbuben.Core.UserInterface;

public class TextField : UserInterfaceNode
{
    public string Text;
    public Binding<object>? TextBinding;
    
    public TextField() : base(UserInterfaceNodeType.TextField)
    {
    }
    
    public TextField SetTextBinding(Binding<object> binding)
    {
        TextBinding = binding;
        TextBinding.ValueChanged += OnTextChanged;
        UpdateText();
        return this;
    }
    
    private void OnTextChanged(object value)
    {
        Text = Convert.ToString(value) ?? "";
    }

    private void UpdateText()
    {
        if (TextBinding != null)
        {
            Text = Convert.ToString(TextBinding.Value) ?? "";
        }
    }
}