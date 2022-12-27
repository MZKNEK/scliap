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
        parser.AddOption(new((arg, _) => { arg.Verbose = true; },
            "enable verbose",
            name: 'v'));

        var o = parser.Parse(new string[] { "-hv" });
        Assert.IsTrue(o.Verbose);
        Assert.IsTrue(o.Help);
    }

    [TestMethod]
    public void FailToAddOptionTest()
    {
        var parser = new SimpleCLIArgsParser<ArgumentsFields>();

        Assert.ThrowsException<OptionInfoException>(() => { parser.AddOption(new((arg, _) => {}, "")); });
    }

    [TestMethod]
    public void AddOptionNextArgTest()
    {
        var parser = new SimpleCLIArgsParser<ArgumentsFields>();
        parser.AddOption(new((arg, nextArg) =>
                {
                    if (!Path.Exists(nextArg))
                    {
                        throw new Exception($"Path {nextArg} don't exist!");
                    }
                    arg.Output = new(nextArg);
                },
            "output path",
            name: 'o',
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

        parser.AddOption(new((arg, _) => { arg.Verbose = true; },
            "enable verbose",
            name: 'v'));

        parser.AddOption(new((arg, nextArg) =>
                {
                    if (!Path.Exists(nextArg))
                    {
                        throw new Exception($"Path {nextArg} don't exist!");
                    }
                    arg.Input = new(nextArg);
                },
            "input path",
            name: 'i',
            needNextArgument: true,
            showInHelp: false,
            longName: "input"));

        parser.AddOption(new((arg, nextArg) =>
                {
                    if (!Path.Exists(nextArg))
                    {
                        throw new Exception($"Path {nextArg} don't exist!");
                    }
                    arg.Output = new(nextArg);
                },
            "output path",
            name: 'o',
            needNextArgument: true,
            longName: "out"));

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
        parser.AddOption(new((arg, _) => { arg.Verbose = true; },
            "enable verbose",
            longName: "verbose"));

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
        parser.AddOption(new((arg, _) => { arg.Verbose = true; },
            "enable verbose",
            name: 'v',
            longName: "alias"));

        var o = parser.Parse(new string[] { "--alias" });
        Assert.IsTrue(o.Verbose);

        var o2 = parser.Parse(new string[] { "-v" });
        Assert.IsTrue(o2.Verbose);
    }

    [TestMethod]
    public void AddOptionBySetTrueTest()
    {
        var parser = ArgumentsFields.Default.Configure();

        var o = parser.Parse(new string[] { "-hv" });
        Assert.IsTrue(o.Help);
        Assert.IsTrue(o.Test);
        Assert.IsTrue(o.Verbose);
    }

    [TestMethod]
    public void RunOptionWithoutSecondArgTest()
    {
        var parser = ArgumentsFields.Default.Configure();
        parser.AddOption(new((arg, nextArg) =>
                {
                    if (!File.Exists(nextArg))
                    {
                        throw new Exception($"Missing file! ({nextArg})");
                    }
                    arg.Input = new(nextArg);
                },
            "",
            name: 'i',
            needNextArgument: true));

        var o = parser.Parse(new string[] { "-i" });
        Assert.IsNull(o.Input);
    }
}