using SCLIAP;

namespace SCLIAPTest;

public class ArgumentsFields : ArgsHelper<ArgumentsFields>
{
    public bool Help;
    internal bool Test;
    public bool Verbose { get; set; }
    internal DirectoryInfo? Output;
    internal DirectoryInfo? Input { get; set; }

    public ArgumentsFields()
    {
        Input = null;
        Test = false;
        Help = false;
        Output = null;
        Verbose = false;
    }

    public static ArgumentsFields Default => new();

    public override SimpleCLIArgsParser<ArgumentsFields> Configure() =>
        new SimpleCLIArgsParser<ArgumentsFields>()
        .AddDefaultHelpOptions(True(Help))
        .AddOption("-v", new(True(Verbose, Test),
            "enable verbose mode"));
}