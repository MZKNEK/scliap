namespace SCLIAP;

public class OptionInfo<T> where T : class, new()
{
    public delegate void OptionAction(T argClass, string nextArg);

    public char? Name { get; init; }
    public bool ShowInHelp { get; init; }
    public string? LongName { get; init; }
    public string Description { get; init; }
    public OptionAction Action { get; init; }
    public bool NeedNextArgument { get; init; }

    public OptionInfo(OptionAction action, string desc, char? name = default!,
        bool needNextArgument = false, bool showInHelp = true, string? longName = default!)
    {
        if (name is null && string.IsNullOrEmpty(longName))
            throw new OptionInfoException($"{nameof(name)} or {nameof(longName)} must be set.");

        Name = name;
        Action = action;
        Description = desc;
        LongName = longName;
        ShowInHelp = showInHelp;
        NeedNextArgument = needNextArgument;
    }

    internal OptionInfo<T> MakeAlias() => new(this) { ShowInHelp = false, LongName = default!};

    private OptionInfo(OptionInfo<T> info)
    {
        Name = info.Name;
        Action = info.Action;
        LongName = info.LongName;
        ShowInHelp = info.ShowInHelp;
        Description = info.Description;
        NeedNextArgument = info.NeedNextArgument;
    }
}