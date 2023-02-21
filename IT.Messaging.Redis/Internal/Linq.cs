using System;

namespace IT.Messaging.Redis.Internal;

internal static class Linq
{
    public static TTo[] To<TFrom, TTo>(this TFrom[] array, Func<TFrom, TTo> convert)
    {
        var to = new TTo[array.Length];

        for (int i = 0; i < to.Length; i++)
        {
            to[i] = convert(array[i]);
        }

        return to;
    }
}