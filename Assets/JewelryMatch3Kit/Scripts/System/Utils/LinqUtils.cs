using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public static class LinqUtils
{
    public static bool AllNull<T>(this IEnumerable<T> seq)
    {
        var result = seq.All(i => i == null || i.Equals(null) || !seq.Any());
        return result;
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> seq)
    {
        var result = seq?.Where(i => i != null && !i.Equals(null));
        return result;
    }

    public static T TryGetElement<T>(this IEnumerable<T> seq, int index, T ifnull)
    {
        if (index < seq.Count())
        {
            var element = seq.ToArray()[index];
            return element;
        }
        return ifnull;
    }

    public static IEnumerable<T> ForEachY<T>(this IEnumerable<T> seq, Action<T> action)
    {
        seq?.ToList().ForEach(action);
        var result = seq;
        return result;
    }

    public static IEnumerable<T> CheckNull<T>(this IEnumerable<T> seq)
    {
        if (seq.Count() == 0)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            obj = Activator.CreateInstance<T>();
            list.Add(obj);
            return seq;
        }
        return seq;
    }

    public static IEnumerable<GameObject> SetActiveAll(this IEnumerable<GameObject> seq, bool active)
    {
        seq.ToList().ForEach(i => i.SetActive(active));
        var result = seq;
        return result;
    }

    public static T ElementAtOrDefault<T>(this IList<T> list, int index, T @default)
    {
        return index >= 0 && index < list.Count ? list[index] : @default;
    }

    public static T Addd<T>(this List<T> list, T newItem)
    {
        list.Add(newItem);
        return newItem;
    }

    public static T NextRandom<T>(this IEnumerable<T> source)
    {
        //        Random gen = new Random((int)DateTime.Now.Ticks);
        //        if (source.Any())
        //            return source.Skip(gen.Next(0, source.Count() - 1) - 1).Shuffle().WhereNotNull().Take(1).ToArray()[0];
        if (source.Count() > 0)
            return source.ToArray()[new Random().Next(source.Count())];
        return default(T);
    }

    public static IEnumerable<T> RandomizeOrder<T>(this IEnumerable<T> source)
    {
        Random rnd = new Random();
        return source.OrderBy<T, int>((item) => rnd.Next());
    }

    public static IEnumerable<T> SelectRandom<T>(this IEnumerable<T> source)
    {
        List<T> Remaining = new List<T>(source);
        while (Remaining.Count >= 1)
        {
            T temp = NextRandom<T>(Remaining);
            Remaining.Remove(temp);
            yield return temp;
        }
    }

    public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> source, int amount)
    {
        return source.RandomizeOrder().Take(amount);
    }

    public static IEnumerable<Transform> OrderByDistance(this IEnumerable<Transform> source)
    {
        // Vector3[] array = source.Select(i => i.position).ToArray();
        // int length = array.Length;

        // var temp = array[0];

        // for (int i = 0; i < length; i++)
        // {
        //     var item = array[i];
        //     for (int j = i + 1; j < length; j++)
        //     {
        //         var item1 = array[j];
        //         for (int k = j + 1; k < length; k++)
        //         {
        //             var item2 = array[k];
        //             var dist1 = Vector2.Distance(item, item1);
        //             var dist2 = Vector2.Distance(item, item2);
        //             if (dist1 > dist2)
        //             {
        //                 array[j] = item2;
        //                 array[k] = item1;
        //             }
        //         }
        //     }
        // }

        Transform[] array2 = source.ToArray();
        int length = array2.Length;

        var temp1 = array2[0];

        for (int i = 0; i < length; i++)
        {
            var item = array2[i];
            for (int j = i + 1; j < length; j++)
            {
                var item1 = array2[j];
                for (int k = j + 2; k < length; k++)
                {
                    var item2 = array2[k];
                    var dist1 = Vector2.Distance(item.position, item1.position);
                    var dist2 = Vector2.Distance(item.position, item2.position);
                    if (dist1 > dist2)
                    {
                        array2[j] = item2;
                        array2[k] = item1;
                    }
                }
            }
        }

        return array2.ToArray();

    }

    public static IEnumerable<R> Triplewise<T, R>(this IEnumerable<T> sequence, Func<T, T, T, R> selector)
    {
        using (var it = sequence.GetEnumerator())
        {
            if (!it.MoveNext())
                yield break;
            T prevprev = it.Current;
            if (!it.MoveNext())
                yield break;
            T prev = it.Current;
            while (it.MoveNext())
                yield return selector(prevprev, prevprev = prev, prev = it.Current);
        }
    }

}