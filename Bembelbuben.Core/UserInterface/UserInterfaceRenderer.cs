using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bembelbuben.Core.UserInterface;

public class UserInterfaceRenderer
{

    public SpriteFont Font;
    public Texture2D ButtonTile;

    public Dictionary<string, Texture2D> Images = new Dictionary<string, Texture2D>();

    public void Calculate(UserInterfaceNode node)
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

                Calculate(child);

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
                        child.Y +=  stack.Height / 2;
                        child.Y -=  child.Height / 2;
                        Calculate(child);
                    }
                }
                
            }
            
        }


        if (node.Type == UserInterfaceNodeType.VStack)
        {
            var stack = (VStack)node;
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

                Calculate(child);

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
                        Calculate(child);
                    }
                }
                
            }
        }
        
        if (node.Type == UserInterfaceNodeType.ZStack)
        {
            var stack = (ZStack)node;
            var currentY = stack.Y + stack.PaddingTop;
            var currentX = stack.X + stack.PaddingLeft;
            var maxWidth = 0f;
            var totalHeight = stack.PaddingTop + stack.PaddingBottom;

            for (var index = 0; index < stack.Children.Count; index++)
            {
                var child = stack.Children[index];
                child.X = currentX;
                child.Y = currentY;

                Calculate(child);

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
        }
        
        if (node.Type == UserInterfaceNodeType.Button)
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
                Calculate(child);
                
                totalHeight += child.Height;
                totalWidth += child.Width;
            }

            // Setze die Breite und Höhe des Buttons auf die berechneten Werte
            button.Width = totalWidth;
            button.Height = totalHeight;
            
            //spriteBatch.Draw(Context.Pixel, new Rectangle((int)button.X, (int)button.Y, (int)button.Width, (int)button.Height), Color.Black);
        }
        
        if (node.Type == UserInterfaceNodeType.Label)
        {
            var label = (Label)node;
            var text = "";
            text = Convert.ToString(label.Text) ?? "";
            var dimensions = Context.Fonts[label.Font].MeasureString(text);
            label.Height = dimensions.Y;
            label.Width = dimensions.X;
        }
        
        if (node.Type == UserInterfaceNodeType.Image)
        {
            var image = (Image)node;
            if (Images.TryGetValue(image.ImageIdentifier, out var texture))
            {
                image.Width = texture.Width * image.Scale;
                image.Height = texture.Height * image.Scale;
            }
            else
            {
                throw new NullReferenceException();
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
        
        if (node.Type == UserInterfaceNodeType.Button)
        {
            var button = (Button)node;
            spriteBatch.Draw(Context.Pixel, new Rectangle((int)button.X, (int)button.Y, (int)button.Width, (int)button.Height), Color.Black * 0.8f);

            foreach (var child in button.Children)
            {
                Draw(child, spriteBatch, gameTime, delta);
            }
        }
        
        if (node.Type == UserInterfaceNodeType.Label)
        {
            var label = (Label)node;
            spriteBatch.DrawString(Context.Fonts[label.Font], Convert.ToString(label.Text) ?? "", new Vector2(label.X, label.Y), Color.White);
        }
        
        if (node.Type == UserInterfaceNodeType.Image)
        {
            var image = (Image)node;
            if (Images.TryGetValue(image.ImageIdentifier, out var texture))
            {
                spriteBatch.Draw(texture, new Rectangle((int)image.X, (int)image.Y, (int)image.Width, (int)image.Height), Color.White);
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