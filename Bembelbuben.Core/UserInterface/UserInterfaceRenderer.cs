using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bembelbuben.Core.UserInterface;

public class UserInterfaceRenderer
{
    // Fields
    private Dictionary<string, Texture2D> _images;

    // Properties
    public SpriteFont Font { get; set; }
    public Texture2D ButtonTile { get; set; }

    // Constructor
    public UserInterfaceRenderer()
    {
        _images = new Dictionary<string, Texture2D>();
    }

    /// <summary>
    /// Add an image to the dictionary.
    /// </summary>
    /// <param name="identifier">The identifier for the image.</param>
    /// <param name="texture">The texture to add.</param>
    public void AddImage(string identifier, Texture2D texture)
    {
        if (!_images.ContainsKey(identifier))
        {
            _images.Add(identifier, texture);
        }
        else
        {
            Console.WriteLine($"An image with identifier '{identifier}' already exists in the dictionary.");
        }
    }

    /// <summary>
    /// Get an image from the dictionary.
    /// </summary>
    /// <param name="identifier">The identifier of the image to get.</param>
    /// <returns>The texture associated with the identifier, or null if not found.</returns>
    public Texture2D GetImage(string identifier)
    {
        if (_images.ContainsKey(identifier))
        {
            return _images[identifier];
        }
        else
        {
            Console.WriteLine($"No image found with identifier '{identifier}'.");
            return null;
        }
    }

    /// <summary>
    /// Calculate the layout for the UI node.
    /// </summary>
    /// <param name="node">The UI node to calculate layout for.</param>
    public void CalculateLayout(UserInterfaceNode node)
    {
        if (node == null || !node.IsVisible)
        {
            return;
        }

        switch (node.Type)
        {
            case UserInterfaceNodeType.HStack:
                CalculateHStackLayout((HStack)node);
                break;
            case UserInterfaceNodeType.VStack:
                CalculateVStackLayout((VStack)node);
                break;
            case UserInterfaceNodeType.ZStack:
                //CalculateZStackLayout((ZStack)node);
                break;
            case UserInterfaceNodeType.ScrollView:
                CalculateScrollViewLayout((ScrollView)node);
                break;
            case UserInterfaceNodeType.Button:
                CalculateButtonLayout((Button)node);
                break;
            case UserInterfaceNodeType.Label:
                CalculateLabelLayout((Label)node);
                break;
            case UserInterfaceNodeType.Image:
                CalculateImageLayout((Image)node);
                break;
            default:
                throw new ArgumentException("Unknown UI node type.");
        }
    }

    private void CalculateScrollViewLayout(ScrollView node)
    {
        var currentY = node.Y;
        var currentX = node.X;
        var totalWidth = 0f;
        var totalHeight = 0f;

        for (var index = 0; index < node.Children.Count; index++)
        {
            var child = node.Children[index];

            // Setze die Position des Kindes auf die aktuelle Y-Position des Buttons
            child.X = currentX;
            child.Y = currentY + node.Value;

            // Rendere das Kind und aktualisiere die Abmessungen des Buttons
            CalculateLayout(child);

            totalHeight += child.Height;
            totalWidth += child.Width;
        }

        // Setze die Breite und Höhe des Buttons auf die berechneten Werte
        node.Width = totalWidth;
        node.Height = totalHeight;
    }

    private void CalculateImageLayout(Image node)
    {
        var image = (Image)node;
        if (_images.TryGetValue(image.ImageIdentifier, out var texture))
        {
            image.Width = texture.Width * image.Scale;
            image.Height = texture.Height * image.Scale;
        }
        else
        {
            throw new NullReferenceException();
        }
    }

    private void CalculateLabelLayout(Label node)
    {
        var label = (Label)node;
        var text = "";
        text = Convert.ToString(label.Text) ?? "";
        var dimensions = Context.Fonts[label.Font].MeasureString(text);
        label.Height = dimensions.Y;
        label.Width = dimensions.X;
    }

    private void CalculateButtonLayout(Button node)
    {
        var button = (Button)node;
        var currentY = button.Y + button.PaddingTop;
        var currentX = button.X + button.PaddingLeft;
        var totalWidth = button.PaddingLeft + button.PaddingRight;
        var totalHeight = button.PaddingTop + button.PaddingBottom;

        for (var index = 0; index < button.Children.Count; index++)
        {
            var child = button.Children[index];

            // Setze die Position des Kindes auf die aktuelle Y-Position des Buttons
            child.X = currentX;
            child.Y = currentY;

            // Rendere das Kind und aktualisiere die Abmessungen des Buttons
            CalculateLayout(child);

            totalHeight += child.Height;
            totalWidth += child.Width;
        }

        // Setze die Breite und Höhe des Buttons auf die berechneten Werte
        button.Width = totalWidth;
        button.Height = totalHeight;
    }

    private void CalculateVStackLayout(VStack stack)
    {
        var currentY = stack.Y + stack.PaddingTop;
        var currentX = stack.X + stack.PaddingLeft;
        var maxWidth = 0f;
        var totalHeight = stack.PaddingTop + stack.PaddingBottom;

        for (var index = 0; index < stack.Children.Count; index++)
        {
            var child = stack.Children[index];

            if (!child.IsVisible)
            {
                continue;
            }

            child.X = currentX;
            child.Y = currentY;

            CalculateLayout(child);

            currentY += child.Height + stack.Spacing;
            totalHeight += child.Height;
            if (index != stack.Children.Count - 1)
            {
                totalHeight += stack.Spacing;
            }

            if (child.Width > maxWidth)
            {
                maxWidth = child.Width;
            }
        }

        stack.Width = maxWidth + stack.PaddingLeft + stack.PaddingRight;
        stack.Height = totalHeight;

        for (var index = 0; index < stack.Children.Count; index++)
        {
            var child = stack.Children[index];

            if (stack.Alignment == Alignment.Center)
            {
                if (child.Width < maxWidth)
                {
                    child.X = stack.X + stack.Width / 2 - child.Width / 2;
                    CalculateLayout(child);
                }
            }
        }
    }

    private void CalculateHStackLayout(HStack stack)
    {
        var currentY = stack.Y + stack.PaddingTop;
        var currentX = stack.X + stack.PaddingLeft;
        var maxHeight = 0f;
        var totalWidth = stack.PaddingLeft + stack.PaddingRight;

        for (var index = 0; index < stack.Children.Count; index++)
        {
            var child = stack.Children[index];

            if (!child.IsVisible)
            {
                continue;
            }

            child.X = currentX;
            child.Y = currentY;

            CalculateLayout(child);

            currentX += child.Width + stack.Spacing;
            totalWidth += child.Width;
            if (index != stack.Children.Count - 1)
            {
                totalWidth += stack.Spacing;
            }

            if (child.Height > maxHeight)
            {
                maxHeight = child.Height;
            }
        }

        stack.Height = maxHeight + stack.PaddingTop + stack.PaddingBottom;
        stack.Width = totalWidth;

        for (var index = 0; index < stack.Children.Count; index++)
        {
            var child = stack.Children[index];

            if (stack.Alignment == Alignment.Center)
            {
                if (child.Height < maxHeight)
                {
                    child.Y = stack.Y;
                    child.Y += stack.Height / 2;
                    child.Y -= child.Height / 2;
                    CalculateLayout(child);
                }
            }
        }
    }

    public void Draw(UserInterfaceNode node, SpriteBatch spriteBatch, GameTime gameTime, float delta)
    {
        if (node == null)
        {
            return;
        }

        if (!node.IsVisible)
        {
            return;
        }

        if (node.Type == UserInterfaceNodeType.HStack)
        {
            var stack = (HStack)node;
            //spriteBatch.Draw(Context.Pixel, new Rectangle((int)stack.X, (int)stack.Y, (int)stack.Width, (int)stack.Height), stack.Tint * stack.Alpha); // Anpassen der Zeichenroutine für die Gesamtbreite
            foreach (var child in stack.Children)
            {
                Draw(child, spriteBatch, gameTime, delta);
            }
        }


        if (node.Type == UserInterfaceNodeType.VStack)
        {
            var stack = (VStack)node;
            //spriteBatch.Draw(Context.Pixel, new Rectangle((int)stack.X, (int)stack.Y, (int)stack.Width, (int)stack.Height), stack.Tint * stack.Alpha); // Anpassen der Zeichenroutine für die Gesamtbreite

            foreach (var child in stack.Children)
            {
                Draw(child, spriteBatch, gameTime, delta);
            }
        }

        if (node.Type == UserInterfaceNodeType.ZStack)
        {
            var stack = (ZStack)node;
            //spriteBatch.Draw(Context.Pixel, new Rectangle((int)stack.X, (int)stack.Y, (int)stack.Width, (int)stack.Height), Color.White);

            foreach (var child in stack.Children)
            {
                Draw(child, spriteBatch, gameTime, delta);
            }
        }

        if (node.Type == UserInterfaceNodeType.ScrollView)
        {
            var view = (ScrollView)node;
            //spriteBatch.Draw(Context.Pixel, new Rectangle((int)stack.X, (int)stack.Y, (int)stack.Width, (int)stack.Height), stack.Tint * stack.Alpha); // Anpassen der Zeichenroutine für die Gesamtbreite
            var prevRect = Context.GraphicsDevice.ScissorRectangle;
            var scrollViewArea =
                new Rectangle((int)view.X, (int)view.Y - view.Value, (int)view.Width, (int)view.Height);
            //Context.GraphicsDevice.ScissorRectangle = scrollViewArea;
            foreach (var child in view.Children)
            {
                Draw(child, spriteBatch, gameTime, delta);
            }
            //Context.GraphicsDevice.ScissorRectangle = prevRect;
        }

        if (node.Type == UserInterfaceNodeType.Button)
        {
            var button = (Button)node;
            spriteBatch.Draw(Context.Pixel,
                new Rectangle((int)button.X, (int)button.Y, (int)button.Width, (int)button.Height), Color.Black * 0.8f);

            foreach (var child in button.Children)
            {
                Draw(child, spriteBatch, gameTime, delta);
            }
        }

        if (node.Type == UserInterfaceNodeType.Label)
        {
            var label = (Label)node;
            spriteBatch.DrawString(Context.Fonts[label.Font], Convert.ToString(label.Text) ?? "",
                new Vector2(label.X, label.Y), Color.White);
        }

        if (node.Type == UserInterfaceNodeType.Image)
        {
            var image = (Image)node;
            if (_images.TryGetValue(image.ImageIdentifier, out var texture))
            {
                spriteBatch.Draw(texture,
                    new Rectangle((int)image.X, (int)image.Y, (int)image.Width, (int)image.Height), Color.White);
            }
            else
            {
                throw new NullReferenceException();
            }
        }
    }

    public void HandleInput(UserInterfaceNode node)
    {
        if (node == null)
        {
            return;
        }

        if (!node.IsVisible)
        {
            return;
        }
        
        if (node.Type == UserInterfaceNodeType.ScrollView)
        {
            var scrollView = (ScrollView)node;
            var scrollViewArea = new Rectangle((int)scrollView.X, (int)scrollView.Y - scrollView.Value,
                (int)scrollView.Width, (int)scrollView.Height);

            if (scrollViewArea.Contains(Context.Input.GetMousePosition()))
            {
                var delta = Math.Clamp(Context.Input.GetMouseWheelValueDelta(), -1, 1);
                scrollView.Value += delta;
            }
        }

        if (node.Type == UserInterfaceNodeType.Button)
        {
            var button = (Button)node;
            var buttonRect = new Rectangle((int)button.X, (int)button.Y, (int)button.Width, (int)button.Height);

            if (buttonRect.Contains(Context.Input.GetMousePosition()) && Context.Input.IsLeftMousePressed())
            {
                button.Invoke();
            }
        }

        if (node is UserInterfaceNodeContainer container)
        {
            foreach (var child in container.Children)
            {
                HandleInput(child);
            }
        }
    }

    public bool HitTest(UserInterfaceNode? node)
    {
        if (node == null)
        {
            throw new NullReferenceException();
        }

        if (!node.IsVisible)
        {
            return false;
        }

        if (!node.IsClickable)
        {
            if (node is UserInterfaceNodeContainer container)
            {
                foreach (var child in container.Children)
                {
                    if (HitTest(child))
                    {
                        var area = new Rectangle((int)child.X, (int)child.Y, (int)child.Width, (int)child.Height);
                        if (area.Contains(Context.Input.GetMousePosition()))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        return true;
    }
}