using System.Reflection;
using System.Runtime.CompilerServices;

namespace SCLIAP;

public abstract class ArgsHelper<TSelf> where TSelf : class, new()
{
    public abstract SimpleCLIArgsParser<TSelf> Configure(Configuration config = default!);

    protected Action<TSelf, string> True(bool param1, bool? param2 = null,
        [CallerArgumentExpression(nameof(param1))]string paramName1 = default!,
        [CallerArgumentExpression(nameof(param2))]string paramName2 = default!)
    {
        return (arg, _) =>
        {
            SetTrue(arg, paramName1);
            if (param2 is not null)
                SetTrue(arg, paramName2);
        };
    }

    private static void SetTrue(TSelf arg, string paramName)
    {
        var t = arg.GetType();
        var m = t.GetProperty(paramName,
            BindingFlags.Instance |
            BindingFlags.NonPublic |
            BindingFlags.Static |
            BindingFlags.Public);

        if (m is not null)
        {
            m.SetValue(arg, true);
            return;
        }

        var f = t.GetField(paramName,
            BindingFlags.Instance |
            BindingFlags.NonPublic |
            BindingFlags.Static |
            BindingFlags.Public);

        if (f is not null)
            f.SetValue(arg, true);
    }
}
