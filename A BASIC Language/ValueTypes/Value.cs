namespace A_BASIC_Language.ValueTypes;

public abstract class Value
{
    public abstract bool IsOfType<T>() where T : Value;

    public abstract bool CanGetAsType<T>() where T : Value;

    public abstract object GetValueAsType<T>() where T : Value;

    public abstract bool CanActAsBool();

    public abstract bool GetBoolValue();
}