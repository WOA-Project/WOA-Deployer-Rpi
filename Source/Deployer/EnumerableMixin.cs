using System;
using System.Collections.Generic;

namespace Deployer
{
    public static class EnumerableMixin
    {
        public static IEnumerable<T> SkipLastN<T>(this IEnumerable<T> source, int n)
        {
            var it = source.GetEnumerator();
            bool hasRemainingItems = false;
            var cache = new Queue<T>(n + 1);

            do
            {
                if (hasRemainingItems = it.MoveNext())
                {
                    cache.Enqueue(it.Current);
                    if (cache.Count > n)
                        yield return cache.Dequeue();
                }
            } while (hasRemainingItems);
        }


        public static IEnumerable<T> ZipLongest<T1, T2, T>(this IEnumerable<T1> first,
            IEnumerable<T2> second, Func<T1, T2, T> operation)
        {
            using (var iter1 = first.GetEnumerator())
            using (var iter2 = second.GetEnumerator())
            {
                while (iter1.MoveNext())
                {
                    if (iter2.MoveNext())
                    {
                        yield return operation(iter1.Current, iter2.Current);
                    }
                    else
                    {
                        yield return operation(iter1.Current, default(T2));
                    }
                }
                while (iter2.MoveNext())
                {
                    yield return operation(default(T1), iter2.Current);
                }
            }
        }
    }
}