namespace SCLIAP;

public class SCLIAPException : Exception
{
    public SCLIAPException()
        : base()
    {
    }

    public SCLIAPException(string message)
        : base(message)
    {
    }

    public SCLIAPException(string message, Exception inner)
        : base(message, inner)
    {
    }
}