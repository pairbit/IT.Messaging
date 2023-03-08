using System.IO;
using System.Reflection;
using System.Resources;

namespace IT.Messaging.Redis.Internal;

internal static class Lua
{
    internal static readonly string QueueRollback;

    static Lua()
    {
        var assembly = Assembly.GetExecutingAssembly();

        QueueRollback = assembly.GetLua("QueueRollback");
    }

    private static string GetLua(this Assembly assembly, string name)
    {
        name = $"IT.Messaging.Redis.Lua.{name}.lua";

        using var stream = assembly.GetManifestResourceStream(name);

        if (stream == null) throw new MissingManifestResourceException($"Script '{name}' not found");

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}