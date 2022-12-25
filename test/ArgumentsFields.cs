namespace SCLIAPTest;

public class ArgumentsFields
{
    public bool Help;
    private bool Hidden;
    public bool Verbose { get; set; }
    internal DirectoryInfo? Output;
    internal DirectoryInfo? Input { get; set; }

    public ArgumentsFields()
    {
        Input = null;
        Help = false;
        Output = null;
        Hidden = false;
        Verbose = false;
    }
}