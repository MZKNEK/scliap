namespace SCLIAP;

public enum ArgsStyle
{
    UNIX,
    DOS
}

public enum InvalidOptionBehavior
{
    Ignore,
    ThrowException
}

public class Configuration
{
    public ArgsStyle Style { get; init; } = ArgsStyle.UNIX;
    public InvalidOptionBehavior Behavior { get; init; } = InvalidOptionBehavior.Ignore;
}