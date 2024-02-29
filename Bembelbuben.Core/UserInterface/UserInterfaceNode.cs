namespace Bembelbuben.Core.UserInterface
{
    public class UserInterfaceNode
    {
        public readonly UserInterfaceNodeType Type;
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public bool IsClickable = false;
        public bool IsVisible = true;
        public Binding<bool>? VisibilityBinding; // Optional, kann null sein

        protected UserInterfaceNode(UserInterfaceNodeType type)
        {
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
            Type = type;
        }

        public UserInterfaceNode SetVisible(bool value)
        {
            IsVisible = value;
            return this;
        }

        public UserInterfaceNode SetVisibilityBinding(Binding<bool> binding)
        {
            VisibilityBinding = binding;
            VisibilityBinding.ValueChanged += OnVisibilityChanged;
            UpdateVisibility();
            return this;
        }

        private void OnVisibilityChanged(bool value)
        {
            IsVisible = value;
        }

        private void UpdateVisibility()
        {
            if (VisibilityBinding != null)
            {
                IsVisible = VisibilityBinding.Value;
            }
        }
    }
}