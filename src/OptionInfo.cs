namespace SCLIAP;

public class OptionInfo<TSelf> where TSelf : class, new()
{
    public char? Name { get; init; }
    public bool ShowInHelp { get; init; }
    public string? LongName { get; init; }
    public string Description { get; init; }
    public bool NeedNextArgument { get; init; }
    public Action<TSelf, string> Action { get; init; }

    public OptionInfo(Action<TSelf, string> action, string desc, char? name = default!,
        bool needNextArgument = false, bool showInHelp = true, string? longName = default!)
    {
        if (name is null && string.IsNullOrEmpty(longName))
            throw new OptionInfoException($"{nameof(name)} or {nameof(longName)} must be set.",
                new ArgumentNullException($"{nameof(name)} and {nameof(longName)}"));

        Name = name;
        Action = action;
        Description = desc;
        LongName = longName;
        ShowInHelp = showInHelp;
        NeedNextArgument = needNextArgument;
    }

    internal OptionInfo<TSelf> MakeAlias() => new(this) { ShowInHelp = false, LongName = default!};

    private OptionInfo(OptionInfo<TSelf> info)
    {
        Name = info.Name;
        Action = info.Action;
        LongName = info.LongName;
        ShowInHelp = info.ShowInHelp;
        Description = info.Description;
        NeedNextArgument = info.NeedNextArgument;
    }
}