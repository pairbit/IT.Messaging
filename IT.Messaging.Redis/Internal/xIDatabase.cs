using StackExchange.Redis;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace IT.Messaging.Redis.Internal;

public static class xIDatabase
{
    private static readonly RedisValue LEFT = "LEFT";
    private static readonly RedisValue RIGHT = "RIGHT";

    private static readonly RedisValue[] LEFT_LEFT = new[] { LEFT, LEFT };
    private static readonly RedisValue[] LEFT_RIGHT = new[] { LEFT, RIGHT };
    private static readonly RedisValue[] RIGHT_LEFT = new[] { RIGHT, LEFT };
    private static readonly RedisValue[] RIGHT_RIGHT = new[] { RIGHT, RIGHT };

    public static long ListRightPopLeftPushAll(this IDatabase db, RedisKey sourceKey, RedisKey destinationKey)
    {
        return (long)db.ScriptEvaluate(Lua.ListRightPopLeftPushAll, new RedisKey[] { sourceKey, destinationKey });
    }

    public static long ListMoveAll(this IDatabase db, RedisKey sourceKey, RedisKey destinationKey, ListSide sourceSide, ListSide destinationSide, CommandFlags flags = CommandFlags.None)
    {
        return (long)db.ScriptEvaluate(Lua.ListMoveAll, new RedisKey[] { sourceKey, destinationKey }, ToRedisValue(sourceSide, destinationSide), flags);
    }

    public static async Task<long> ListMoveAllAsync(this IDatabaseAsync db, RedisKey sourceKey, RedisKey destinationKey, ListSide sourceSide, ListSide destinationSide, CommandFlags flags = CommandFlags.None)
    {
        return (long)await db.ScriptEvaluateAsync(Lua.ListMoveAll, new RedisKey[] { sourceKey, destinationKey }, ToRedisValue(sourceSide, destinationSide), flags);
    }

    public static void QueueRollback(this IDatabase db, RedisKey sourceKey, RedisKey destinationKey)
    {
        //await db.ScriptEvaluateAsync(Lua.ListMoveAll, new RedisKey[] { sourceKey, destinationKey }, LEFT_RIGHT);
        db.ScriptEvaluate(Lua.QueueRollback, new RedisKey[] { sourceKey, destinationKey });
    }

    public static Task QueueRollbackAsync(this IDatabaseAsync db, RedisKey sourceKey, RedisKey destinationKey)
    {
        //await db.ScriptEvaluateAsync(Lua.ListMoveAll, new RedisKey[] { sourceKey, destinationKey }, LEFT_RIGHT);
        return db.ScriptEvaluateAsync(Lua.QueueRollback, new RedisKey[] { sourceKey, destinationKey });
    }

    public static Task QueueRollbackAsync(this IDatabaseAsync db, RedisKey sourceKey, RedisKey destinationKey, bool[] status)
    {
        //await db.ScriptEvaluateAsync(Lua.ListMoveAll, new RedisKey[] { sourceKey, destinationKey }, LEFT_RIGHT);
        return db.ScriptEvaluateAsync(Lua.QueueRollback, new RedisKey[] { sourceKey, destinationKey });
    }

    public static async Task<long> ListRemoveAllAsync(this IDatabaseAsync db, RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
    {
        return (long)await db.ScriptEvaluateAsync(Lua.ListRemoveAll, new RedisKey[] { key }, values, flags).ConfigureAwait(false);
    }

    public static long ListRemoveAll(this IDatabase db, RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
    {
        return (long)db.ScriptEvaluate(Lua.ListRemoveAll, new RedisKey[] { key }, values, flags);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static RedisValue[] ToRedisValue(ListSide sourceSide, ListSide destinationSide)
    {
        if (sourceSide == ListSide.Left)
        {
            if (destinationSide == ListSide.Left) return LEFT_LEFT;
            if (destinationSide == ListSide.Right) return LEFT_RIGHT;
            throw new ArgumentOutOfRangeException(nameof(destinationSide));
        }

        if (sourceSide == ListSide.Right)
        {
            if (destinationSide == ListSide.Left) return RIGHT_LEFT;
            if (destinationSide == ListSide.Right) return RIGHT_RIGHT;
            throw new ArgumentOutOfRangeException(nameof(destinationSide));
        }

        throw new ArgumentOutOfRangeException(nameof(sourceSide));
    }
}