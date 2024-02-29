using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bembelbuben.Core.Input;

public class InputHandle
{
    private KeyboardState _currKeyboardState;
    private KeyboardState _prevKeyboardState;

    private MouseState _currMouseState;
    private MouseState _prevMouseState;

    private bool _currMouseLeft;
    private bool _prevMouseLeft;

    private bool _currMouseRight;
    private bool _prevMouseRight;

    private bool _currMouseMiddle;
    private bool _prevMouseMiddle;

    private Point _currMousePosition;
    private Point _prevMousePosition;

    private int _currMouseWheelValue;
    private int _prevMouseWheelValue;
    
    public void Update(GameTime gameTime, float delta)
    {
        _prevKeyboardState = _currKeyboardState;
        _currKeyboardState = Keyboard.GetState();

        _prevMouseState = _currMouseState;
        _currMouseState = Mouse.GetState();

        _prevMousePosition = _currMousePosition;
        _currMousePosition = _currMouseState.Position;

        _prevMouseLeft = _currMouseLeft;
        _currMouseLeft = _currMouseState.LeftButton == ButtonState.Pressed;

        _prevMouseRight = _currMouseRight;
        _currMouseRight = _currMouseState.RightButton == ButtonState.Pressed;

        _prevMouseMiddle = _currMouseMiddle;
        _currMouseMiddle = _currMouseState.MiddleButton == ButtonState.Pressed;

        _prevMouseWheelValue = _currMouseWheelValue;
        _currMouseWheelValue = _currMouseState.ScrollWheelValue;

    }

    #region Keyboard

    public bool IsKeyPressed(Keys key)
    {
        return _currKeyboardState.IsKeyDown(key) && !_prevKeyboardState.IsKeyDown(key);
    }

    public bool IsKeyDown(Keys key)
    {
        return _currKeyboardState.IsKeyDown(key);
    }

    public bool IsKeyReleased(Keys key)
    {
        return !_currKeyboardState.IsKeyDown(key) && _prevKeyboardState.IsKeyDown(key);
    }

    #endregion

    #region Mouse

    public bool IsLeftMousePressed()
    {
        return _currMouseLeft && !_prevMouseLeft;
    }

    public bool IsRightMousePressed()
    {
        return _currMouseRight && !_prevMouseRight;
    }

    public bool IsMiddleMousePressed()
    {
        return _currMouseMiddle && !_prevMouseMiddle;
    }

    public bool IsLeftMouseDown()
    {
        return _currMouseLeft;
    }

    public bool IsRightMouseDown()
    {
        return _currMouseRight;
    }

    public bool IsMiddleMouseDown()
    {
        return _currMouseMiddle;
    }

    public Point GetMousePosition()
    {
        return _currMousePosition;
    }

    public Point GetMouseDelta()
    {
        return new Point(_currMousePosition.X - _prevMousePosition.X, _currMousePosition.Y - _prevMousePosition.Y);
    }

    public int GetMouseWheelValue()
    {
        return _currMouseWheelValue;
    }
    
    public int GetMouseWheelValueDelta()
    {
        return _currMouseWheelValue - _prevMouseWheelValue;
    }
    
    #endregion
    
}