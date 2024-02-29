using Bembelbuben.Core;
using Bembelbuben.Core.UserInterface;

public class Label : UserInterfaceNode
{
    public string Font;
    public string Text;
    public Binding<object>? TextBinding; // Optional, kann null sein

    public Label(object? text = null, string font = "default") : base(UserInterfaceNodeType.Label)
    {
        Font = font;
        Text = Convert.ToString(text) ?? "";
    }

    public Label SetTextBinding(Binding<object> binding)
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
    
    public Label SetFont(string font)
    {
        Font = font;
        return this;
    }
}