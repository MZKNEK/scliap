# SCLIAP

[![Nuget](https://img.shields.io/nuget/v/SCLIAP)](https://www.nuget.org/packages/SCLIAP/) [![codecov](https://img.shields.io/codecov/c/github/MZKNEK/scliap?token=J00M8DUKGD)](https://codecov.io/gh/MZKNEK/scliap) [![Build status](https://img.shields.io/appveyor/build/MrZnake/scliap)](https://ci.appveyor.com/project/mrznake/scliap/branch/master) [![CodeFactor](https://www.codefactor.io/repository/github/mzknek/scliap/badge)](https://www.codefactor.io/repository/github/mzknek/scliap) [![License](https://img.shields.io/github/license/MZKNEK/scliap)](https://github.com/MZKNEK/scliap/blob/master/LICENSE)

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
// adds -h option with --help alias
parser.AddDefaultHelpOptions((arg, _) => { arg.Help = true; });
parser.AddOption("-v", new((arg, _) => { arg.Verbose = true; }, "adds additional information to output"));
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
    // setup alias for your oprion
    alias: "--output"));
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
