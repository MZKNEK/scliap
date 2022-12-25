using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCLIAP;

namespace SCLIAPTest;

[TestClass]
public class SCLIAPTest
{
    [TestMethod]
    public void CreateParserTest()
    {
        var parser = new SimpleCLIArgsParser<ArgumentsFields>();
        Assert.IsNotNull(parser);
    }

    [TestMethod]
    public void AddDefaultHelpTest()
    {
        var parser = new SimpleCLIArgsParser<ArgumentsFields>();
        parser.AddDefaultHelpOptions((arg, _) => { arg.Help = true; });

        var o = parser.Parse(new string[] { "-h" });
        Assert.IsTrue(o.Help);

        var o2 = parser.Parse(new string[] { "--help" });
        Assert.IsTrue(o2.Help);

        var o3 = parser.Parse(new string[] { "-trth" });
        Assert.IsTrue(o3.Help);
    }

    [TestMethod]
    public void AddOptionTest()
    {
        var parser = new SimpleCLIArgsParser<ArgumentsFields>();
        parser.AddDefaultHelpOptions((arg, _) => { arg.Help = true; });
        parser.AddOption(
            "-v",
            new((arg, _) => { arg.Verbose = true; },
            "enable verbose"));

        var o = parser.Parse(new string[] { "-hv" });
        Assert.IsTrue(o.Verbose);
        Assert.IsTrue(o.Help);
    }

    [TestMethod]
    public void AddOptionNextArgTest()
    {
        var parser = new SimpleCLIArgsParser<ArgumentsFields>();
        parser.AddOption(
            "-o",
            new((arg, nextArg) =>
                {
                    if (!Path.Exists(nextArg))
                    {
                        throw new Exception($"Path {nextArg} don't exist!");
                    }
                    arg.Output = new(nextArg);
                },
            "output path",
            needNextArgument: true));

        var  o = parser.Parse(new string[] { "-o", "./" });
        Assert.IsNotNull(o.Output);
        Assert.ThrowsException<Exception>(() => { parser.Parse(new string[] { "-o", "./src" }); });
    }

    [TestMethod]
    public void PrintsHelpAndHiddenOptionTest()
    {
        var parser = new SimpleCLIArgsParser<ArgumentsFields>();
        parser.AddDefaultHelpOptions((arg, _) => { arg.Help = true; });

        parser.AddOption(
            "-v",
            new((arg, _) => { arg.Verbose = true; },
            "enable verbose"));

        parser.AddOption(
            "-i",
            new((arg, nextArg) =>
                {
                    if (!Path.Exists(nextArg))
                    {
                        throw new Exception($"Path {nextArg} don't exist!");
                    }
                    arg.Input = new(nextArg);
                },
            "input path",
            needNextArgument: true,
            showInHelp: false,
            alias: "--input"));

        parser.AddOption(
            "-o",
            new((arg, nextArg) =>
                {
                    if (!Path.Exists(nextArg))
                    {
                        throw new Exception($"Path {nextArg} don't exist!");
                    }
                    arg.Output = new(nextArg);
                },
            "output path",
            needNextArgument: true,
            alias: "--out"));

        var o = parser.Parse(new string[] { "-hvo", "./", "--input", "./" });
        Assert.IsNotNull(o.Input);
        Assert.IsNotNull(o.Output);
        Assert.IsTrue(o.Verbose);
        Assert.IsTrue(o.Help);

        var help = "-h, --help\tprints help\n-v\t\tenable verbose\n-o, --out\toutput path";
        Assert.AreEqual(help, parser.GetHelp());
    }

    [TestMethod]
    public void AddOptionOnlyWithLongNameTest()
    {
        var parser = new SimpleCLIArgsParser<ArgumentsFields>();
        parser.AddOption(
            "--verbose",
            new((arg, _) => { arg.Verbose = true; },
            "enable verbose"));

        var o = parser.Parse(new string[] { "-verbose" });
        Assert.IsFalse(o.Verbose);

        var o2 = parser.Parse(new string[] { "-htverbose" });
        Assert.IsFalse(o2.Verbose);

        var o3 = parser.Parse(new string[] { "--verbose" });
        Assert.IsTrue(o3.Verbose);

        var o4 = parser.Parse(new string[] { "-trt", "--verbose" });
        Assert.IsTrue(o4.Verbose);
    }

    [TestMethod]
    public void AddOptionWithAliasTest()
    {
        var parser = new SimpleCLIArgsParser<ArgumentsFields>();
        parser.AddOption(
            "-v",
            new((arg, _) => { arg.Verbose = true; },
            "enable verbose",
            alias: "--alias"));

        var o = parser.Parse(new string[] { "--alias" });
        Assert.IsTrue(o.Verbose);

        var o2 = parser.Parse(new string[] { "-v" });
        Assert.IsTrue(o2.Verbose);
    }
}