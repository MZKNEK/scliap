# SCLIAP

[![Nuget](https://img.shields.io/nuget/v/SCLIAP)](https://www.nuget.org/packages/SCLIAP/) [![codecov](https://img.shields.io/codecov/c/github/MZKNEK/scliap?token=J00M8DUKGD)](https://codecov.io/gh/MZKNEK/scliap) [![Build status](https://img.shields.io/appveyor/build/MrZnake/scliap)](https://ci.appveyor.com/project/mrznake/scliap/branch/master) [![CodeFactor](https://www.codefactor.io/repository/github/mzknek/scliap/badge)](https://www.codefactor.io/repository/github/mzknek/scliap) [![License](https://img.shields.io/github/license/MZKNEK/scliap)](https://github.com/MZKNEK/scliap/blob/master/LICENSE)

Simple CLI Arguments Parser

## Usage

Make class that will be storing your args, and configure parser.

```csharp
public class Arguments : ArgsHelper<Arguments>
{
    public bool Help;
    public bool Verbose;
    public DirectoryInfo? OutputPath;

    public Arguments()
    {
        Help = true;
        Verbose = false;
        OutputPath = null;
    }

    public static ArgumentsFields Default => new();

    public override SimpleCLIArgsParser<ArgumentsFields> Configure() =>
        new SimpleCLIArgsParser<ArgumentsFields>()
        .AddDefaultHelpOptions(True(Help))
        .AddOption("-v", new(True(Verbose),
            "enable verbose mode"))
        .AddOption("-o", new((arg, nextArg) =>
            {
                if (!Path.Exists(nextArg))
                {
                    throw new Exception($"Path {nextArg} don't exist!");
                }
                arg.OutputPath = new(nextArg);
            }
        "setup output location",
        needNextArgument: true);
}
```

Or create parser and add options externally. In this case you don't need to use `ArgsHelper`.

```csharp
var parser = new SimpleCLIArgsParser<Arguments>();
// adds -h option with --help alias
parser.AddDefaultHelpOptions((arg, _) => { arg.Help = true; });

// hide option from help
parser.AddOption("-v", new((arg, _) =>
    { arg.Verbose = true; },
    "adds more info to output",
    showInHelp: false));

// retrieve string that is after your argument and use it
parser.AddOption("-o", new((arg, nextArg) =>
    {
        if (!Path.Exists(nextArg))
        {
            throw new Exception($"Path {nextArg} don't exist!");
        }
        arg.OutputPath = new(nextArg);
    },
    "setup output location",
    // for that you need set this to true
    needNextArgument: true,
    // setup alias for your oprion
    alias: "--output"));
```

Parse input and use it in your program.

```csharp
// with implemented ArgsHelper
var parsedArgs = Arguments.Default.Configure().Parse(args);

// or externally
parsedArgs = parser.Parse(args);
if (parsedArgs.Help)
{
    // easily provide help for your program
    Console.WriteLine(parser.GetHelp());
    return 0;
}

DoSomethingAndSaveItAt(parsedArgs.OutputPath);
```
