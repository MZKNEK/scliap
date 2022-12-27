namespace SCLIAP;

public class SimpleCLIArgsParser<T> where T : class, new()
{
    private readonly Configuration _config;
    private Dictionary<string, OptionInfo<T>> _options;

    public SimpleCLIArgsParser(Configuration config = default!)
    {
        _config = config ?? new();
        _options = new();
    }

    public SimpleCLIArgsParser<T> AddOption(OptionInfo<T> info)
    {
        if (_config.Style == ArgsStyle.DOS)
        {
            if (info.Name is null)
                throw new SCLIAPException("Long names are disabled in DOS style and name is missing.");

            _options.Add($"/{info.Name}", info);
        }
        else
        {
            if (info.Name is not null)
                _options.Add($"-{info.Name}", info);

            if (info.LongName is not null)
                _options.Add($"--{info.LongName}", info.MakeAlias());
        }
        return this;
    }

    public SimpleCLIArgsParser<T> AddDefaultHelpOptions(OptionInfo<T>.OptionAction action, string description
        = "prints help") => AddOption(new(action, description, name: 'h', longName: "help"));

    public string GetHelp() => string.Join("\n", _options.Where(x => x.Value.ShowInHelp)
        .Select(x => GetHelpEntry(x)));

    private string GetHelpEntry(KeyValuePair<string, OptionInfo<T>> x) =>
        $"{x.Key}{GetLongNameForHelp(x.Value)}\t{x.Value.Description}";

    private string GetLongNameForHelp(OptionInfo<T> o) =>
        string.IsNullOrEmpty(o.LongName) ? "\t" : $", --{o.LongName}";

    public T Parse(string[] args)
    {
        var parsedArgs = new T();
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
                if (_config.Style != ArgsStyle.UNIX)
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

    private void ExecuteParam(OptionInfo<T> option, string nextArg, ref int index, ref T args)
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