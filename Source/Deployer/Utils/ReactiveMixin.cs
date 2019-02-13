using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Deployer.Utils
{
    public static class ReactiveMixin
    {
        // Licensed under the MIT license with <3 by GitHub

        /// <summary>
        /// An exponential back off strategy which starts with 1 second and then 4, 9, 16...
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly Func<int, TimeSpan> ExponentialBackoff = n => TimeSpan.FromSeconds(Math.Pow(n, 2));

        /// <summary>
        /// Returns a cold observable which retries (re-subscribes to) the source observable on error up to the 
        /// specified number of times or until it successfully terminates. Allows for customizable back off strategy.
        /// </summary>
        /// <param name="source">The source observable.</param>
        /// <param name="retryCount">The number of attempts of running the source observable before failing.</param>
        /// <param name="strategy">The strategy to use in backing off, exponential by default.</param>
        /// <param name="retryOnError">A predicate determining for which exceptions to retry. Defaults to all</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns>
        /// A cold observable which retries (re-subscribes to) the source observable on error up to the 
        /// specified number of times or until it successfully terminates.
        /// </returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static IObservable<T> RetryWithBackoffStrategy<T>(
            this IObservable<T> source,
            int retryCount = 3,
            Func<int, TimeSpan> strategy = null,
            Func<Exception, bool> retryOnError = null,
            IScheduler scheduler = null)
        {
            strategy = strategy ?? ExponentialBackoff;
            scheduler = scheduler ?? Scheduler.Default;

            if (retryOnError == null)
                retryOnError = e => true;

            int attempt = 0;

            return Observable.Defer(() =>
                {
                    return ((++attempt == 1) ? source : source.DelaySubscription(strategy(attempt - 1), scheduler))
                        .Select(item => new Tuple<bool, T, Exception>(true, item, null))
                        .Catch<Tuple<bool, T, Exception>, Exception>(e => retryOnError(e)
                            ? Observable.Throw<Tuple<bool, T, Exception>>(e)
                            : Observable.Return(new Tuple<bool, T, Exception>(false, default(T), e)));
                })
                .Retry(retryCount)
                .SelectMany(t => t.Item1
                    ? Observable.Return(t.Item2)
                    : Observable.Throw<T>(t.Item3));
        }
    }
}