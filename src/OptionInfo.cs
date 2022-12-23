namespace SCLIAP;

public class OptionInfo<T> where T : class, new()
{
    public delegate void OptionAction(T argClass, string nextArg);

    public string? Alias { get; init; }
    public bool ShowInHelp { get; init; }
    public string Description { get; init; }
    public OptionAction Action { get; init; }
    public bool NeedNextArgument { get; init; }

    public OptionInfo(OptionAction action, string desc,
        bool needNextArgument = false, bool showInHelp = true, string? alias = null)
    {
        Alias = alias;
        Action = action;
        Description = desc;
        ShowInHelp = showInHelp;
        NeedNextArgument = needNextArgument;
    }

    internal OptionInfo<T> MakeAlias() => new(this) { ShowInHelp = false, Alias = null};

    private OptionInfo(OptionInfo<T> info)
    {
        Alias = info.Alias;
        Action = info.Action;
        ShowInHelp = info.ShowInHelp;
        Description = info.Description;
        NeedNextArgument = info.NeedNextArgument;
    }
}