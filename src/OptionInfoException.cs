namespace SCLIAP;

public class OptionInfoException : Exception
{
    public OptionInfoException()
        : base()
    {
    }

    public OptionInfoException(string message)
        : base(message)
    {
    }

    public OptionInfoException(string message, Exception inner)
        : base(message, inner)
    {
    }
}