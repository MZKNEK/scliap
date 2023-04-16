namespace SCLIAP;

public class SimpleCLIArgsParser<TSelf> where TSelf : class, new()
{
    private int _maxLongName;
    private readonly Configuration _config;
    private Dictionary<string, OptionInfo<TSelf>> _options;

    public SimpleCLIArgsParser(Configuration config = default!)
    {
        _config = config ?? new();
        _options = new();
        _maxLongName = 0;
    }

    public SimpleCLIArgsParser<TSelf> AddOption(OptionInfo<TSelf> info)
    {
        if (_config.Style == ArgsStyle.DOS)
        {
            if (info.Name is null)
                throw new SCLIAPException("Long names are disabled in DOS style and name is missing.");

            _options.Add($"/{info.Name}", info);
        }
        else
        {
            if (info.HasName)
                _options.Add($"-{info.Name}", info);

            if (info.LongName is not null)
            {
                if (info.ShowInHelp && _maxLongName < info.LongName.Length)
                    _maxLongName = info.LongName.Length;

                _options.Add($"--{info.LongName}", info.MakeAlias());
            }
        }
        return this;
    }

    public SimpleCLIArgsParser<TSelf> AddDefaultHelpOptions(Action<TSelf, string> action, string description
        = "prints help") => AddOption(new(action, description, name: 'h', longName: "help"));

    public string GetHelp() => string.Join("\n", _options.Where(x => x.Value.ShowInHelp)
        .Select(x => GetHelpEntry(x)));

    private string GetHelpEntry(KeyValuePair<string, OptionInfo<TSelf>> x) =>
        $"{GetNameForHelp(x)}{GetLongNameForHelp(x.Value)}\t{x.Value.Description}";

    private string GetLongNameForHelp(OptionInfo<TSelf> o) =>
        string.IsNullOrEmpty(o.LongName) ? "  \t" : o.HasName ? $",\t--{GetPaddedLongName(o)}" : $"--{GetPaddedLongName(o)}";

    private string GetPaddedLongName(OptionInfo<TSelf> o) =>
        o?.LongName?.PadRight(_maxLongName) ?? "";

    private string GetNameForHelp(KeyValuePair<string, OptionInfo<TSelf>> x) =>
        x.Value.HasName ? x.Key : "\t";

    public TSelf Parse(string[] args)
    {
        var parsedArgs = new TSelf();
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            var nextArg = (i+1 >= args.Length)
                ? string.Empty
                : args[i+1];

            if (_config.Style == ArgsStyle.DOS)
            {
                nextArg = string.Empty;
                if (arg.Contains(':'))
                {
                    if (!CheckArgInDOSStyle(ref arg, ref nextArg))
                        continue;
                }
            }

            if (!_options.ContainsKey(arg))
            {
                if (_config.Style != ArgsStyle.POSIX)
                    continue;

                foreach (var o in ProcessCluster(arg))
                {
                    var param = _options[o];
                    ExecuteParam(param, nextArg, ref i, ref parsedArgs);
                }
            }
            else
            {
                var param = _options[arg];
                ExecuteParam(param, nextArg, ref i, ref parsedArgs);
            }
        }
        return parsedArgs;
    }

    private bool CheckArgInDOSStyle(ref string arg, ref string nextArg)
    {
        var parts = arg.Split(':');
        var count = parts.Count();
        if (count != 2)
        {
            if (_config.Behavior == InvalidOptionBehavior.ThrowException)
                throw new SCLIAPException("Invalid argument format." +
                    $" Expected /[arg]:[value] in '{arg}'.");

            return false;
        }

        nextArg = parts[1];
        arg = parts[0];
        return true;
    }

    private void ExecuteParam(OptionInfo<TSelf> option, string nextArg, ref int index, ref TSelf args)
    {
        if (option.NeedNextArgument && nextArg == string.Empty)
        {
            if (_config.Behavior == InvalidOptionBehavior.ThrowException)
                throw new SCLIAPException($"Required value of" +
                    $"' {option.Name?.ToString() ?? option.LongName}' is missing.");

            return;
        }

        option.Action(args, nextArg);
        if (option.NeedNextArgument && _config.Style != ArgsStyle.DOS)
            index++;
    }

    private IEnumerable<string> ProcessCluster(string arg)
    {
        if (arg.StartsWith("-"))
        {
            var arr = arg.ToCharArray();
            for (var j = 1; j < arr.Length; j++)
            {
                if (arr[j] is '-') break;

                foreach (var a in _options)
                {
                    if (a.Key.Equals($"-{arr[j]}"))
                        yield return a.Key;
                }
            }
        }
    }
}