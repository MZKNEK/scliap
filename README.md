# SCLIAP

[![Nuget](https://img.shields.io/nuget/v/SCLIAP)](https://www.nuget.org/packages/SCLIAP/) [![codecov](https://img.shields.io/codecov/c/github/MZKNEK/scliap?token=J00M8DUKGD)](https://codecov.io/gh/MZKNEK/scliap) [![Build status](https://img.shields.io/appveyor/build/MrZnake/scliap)](https://ci.appveyor.com/project/mrznake/scliap/branch/master) [![CodeFactor](https://www.codefactor.io/repository/github/mzknek/scliap/badge)](https://www.codefactor.io/repository/github/mzknek/scliap) [![License](https://img.shields.io/github/license/MZKNEK/scliap)](https://github.com/MZKNEK/scliap/blob/master/LICENSE)

Simple CLI Arguments Parser

## Usage

Make class that will be storing your args, and override the `Configure` method.

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

    public static Arguments Default => new();

    public override SimpleCLIArgsParser<Arguments> Configure() =>
        new SimpleCLIArgsParser<Arguments>()
        .AddDefaultHelpOptions(True(Help))
        .AddOption(new(True(Verbose),
            "enable verbose mode",
            name: 'v'))
        .AddOption(new((arg, nextArg) =>
            {
                if (!Path.Exists(nextArg))
                {
                    throw new Exception($"Path {nextArg} don't exist!");
                }
                arg.OutputPath = new(nextArg);
            }
        "set output location",
        name: 'o',
        needNextArgument: true);
}
```

Or create parser and add options externally. In this case you don't need to inherits from `ArgsHelper`.

```csharp
var parser = new SimpleCLIArgsParser<Arguments>();
// adds h option with help alias
parser.AddDefaultHelpOptions((arg, _) => { arg.Help = true; });

// hide option in help
parser.AddOption(new((arg, _) => { arg.Verbose = true; },
    "adds more info to output",
    // single char as name
    name: 'v',
    showInHelp: false));

// retrieve string that that follows it
parser.AddOption(new((arg, nextArg) =>
    {
        if (!Path.Exists(nextArg))
        {
            throw new Exception($"Path {nextArg} don't exist!");
        }
        arg.OutputPath = new(nextArg);
    },
    "set output location",
    // name or longName is required
    name: 'o',
    // for that you need set needNextArgument to true
    needNextArgument: true,
    // setup alias for your oprion
    longName: "output"));
```

Use the `Parse` method of the parser instance to parse arguments passed to your program.

```csharp
// with inherited ArgsHelper
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
