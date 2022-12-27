namespace SCLIAP;

public class OptionInfoException : SCLIAPException
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