namespace SCLIAP;

public enum ArgsStyle
{
    POSIX,
    DOS
}

public enum InvalidOptionBehavior
{
    Ignore,
    ThrowException
}

public class Configuration
{
    public ArgsStyle Style { get; init; } = ArgsStyle.POSIX;
    public InvalidOptionBehavior Behavior { get; init; } = InvalidOptionBehavior.ThrowException;
}