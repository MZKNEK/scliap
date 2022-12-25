namespace SCLIAPTest;

public class ArgumentsFields
{
    public bool Help;
    public bool Verbose { get; set; }
    internal DirectoryInfo? Output;
    internal DirectoryInfo? Input { get; set; }

    public ArgumentsFields()
    {
        Input = null;
        Help = false;
        Output = null;
        Verbose = false;
    }
}