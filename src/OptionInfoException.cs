namespace SCLIAP;

public class OptionInfoException : Exception
{
    public OptionInfoException()
        : base()
    {
    }

    public OptionInfoException(string message)
        : base($"OptionInfo: {message}")
    {
    }

    public OptionInfoException(string message, Exception inner)
        : base($"OptionInfo: {message}", inner)
    {
    }
}