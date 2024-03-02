namespace Bembelbuben.Core;

public class Binding<T>
{
    private T _value;
    public T Value
    {
        get => _value;
        set
        {
            if (Equals(_value, value)) return;
            _value = value;
            ValueChanged?.Invoke(value);
        }
    }

    public event Action<T> ValueChanged;

    public Binding(T initialValue)
    {
        _value = initialValue;
    }
}