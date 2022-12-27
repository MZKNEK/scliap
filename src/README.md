# SCLIAP

Simple CLI Arguments Parser

## Usage

Make class that will be storing your args.

```csharp
public class Arguments
{
    public bool Help { get; set; }
    public bool Verbose { get; set; }
    public DirectoryInfo? OutputPath { get; set; }

    public Arguments()
    {
        Help = true;
        Verbose = false;
        OutputPath = null;
    }
}
```

Create parser and add options.

```csharp
var parser = new SimpleCLIArgsParser<Arguments>();
// adds h option with help alias
parser.AddDefaultHelpOptions((arg, _) => { arg.Help = true; });
parser.AddOption(new((arg, _) => { arg.Verbose = true; },
    "adds more info to output",
    // single char as name
    name: 'v'));

// retrieve string that is after your argument and use it
parser.AddOption(new((arg, nextArg) =>
    {
        if (!Path.Exists(nextArg))
        {
            throw new Exception($"Path {nextArg} don't exist!");
        }
        arg.OutputPath = new(nextArg);
    },
    "setup output location",
    // name or longName is required
    name: 'o',
    needNextArgument: true,
    // setup long name(alias) for your option
    longName: "output"));
```

Parse input args and use them in your program.

```csharp
var parsedArgs = parser.Parse(args);
if (parsedArgs.Help)
{
    // easily provide help for your program
    Console.WriteLine(parser.GetHelp());
    return 0;
}

DoSomethingAndSaveItAt(parsedArgs.OutputPath);
```
