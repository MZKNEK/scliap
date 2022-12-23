namespace SCLIAP;

public class SimpleCLIArgsParser<T> where T : class, new()
{
    private Dictionary<string, OptionInfo<T>> _options = new();

    public void AddOption(string optionName, OptionInfo<T> info)
    {
        _options.Add(optionName, info);
        if (info.Alias is not null)
            _options.Add(info.Alias, info.MakeAlias());
    }

    public void AddDefaultHelpOptions(OptionInfo<T>.OptionAction action, string description
        = "prints help") => AddOption("-h", new(action, description, alias: "--help"));

    public string GetHelp() => string.Join("\n", _options.Where(x => x.Value.ShowInHelp)
        .Select(x => GetHelpEntry(x)));

    private string GetHelpEntry(KeyValuePair<string, OptionInfo<T>> x) =>
        $"{x.Key}{(x.Value.Alias is not null ? ", " : "\t") + x.Value.Alias ?? ""}\t{x.Value.Description}";

    public T Parse(string[] args)
    {
        var parsedArgs = new T();
        for (var i = 0; i < args.Length; i++)
        {
            var nextArg = (i+1 >= args.Length)
                ? string.Empty
                : args[i+1];

            if (!_options.ContainsKey(args[i]))
            {
                foreach (var o in ProcessCluster(args[i]))
                {
                    var param = _options[o];
                    ExecuteParam(param, nextArg, ref i, ref parsedArgs);
                }
            }
            else
            {
                var param = _options[args[i]];
                ExecuteParam(param, nextArg, ref i, ref parsedArgs);
            }
        }
        return parsedArgs;
    }

    private void ExecuteParam(OptionInfo<T> option, string nextArg, ref int index, ref T args)
    {
        if (option.NeedNextArgument && nextArg == string.Empty)
            return;

        if (option.Action is null)
            return;

        option.Action(args, nextArg);
        if (option.NeedNextArgument)
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