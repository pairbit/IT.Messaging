using System.IO;
using System.Reflection;
using System.Resources;

namespace IT.Messaging.Redis.Internal;

internal static class Lua
{
    internal static readonly string QueueRollback;
    internal static readonly string ListMoveAll;
    internal static readonly string ListRightPopLeftPushAll;

    static Lua()
    {
        var assembly = Assembly.GetExecutingAssembly();

        QueueRollback = assembly.GetLua("QueueRollback");
        ListMoveAll = assembly.GetLua("ListMoveAll");
        ListRightPopLeftPushAll = assembly.GetLua("ListRightPopLeftPushAll");
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