// Copyright (c) 2015-2016 Tamas Vajk. All Rights Reserved. Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using NodaTime;
using System.Threading;

namespace SkyScanner.Services.Base
{
    internal static class Retry
    {
        /// <summary>
        /// Retries an function several times until a certain number of expected and unexpected exceptions occurred or the result could be retrieved
        /// </summary>
        /// <typeparam name="T">Response type</typeparam>
        /// <typeparam name="TExpectedException">Expected exception type</typeparam>
        /// <param name="func">The function to do and retry</param>
        /// <param name="retryInterval">Interval to wait between retries</param>
        /// <param name="retryCountOnExpectedException">Number of retries upon any (expected and unexpected) exceptions</param>
        /// <param name="retryCountOnGenericException">Number of retries upon unexpected exceptions</param>
        /// <returns>Result of the function</returns>
        public static async Task<T> Do<T, TExpectedException>(Func<Task<T>> func, Duration retryInterval,
            int retryCountOnExpectedException = -1, int retryCountOnGenericException = 0,
            CancellationToken cancellationToken = default(CancellationToken))
            where TExpectedException : Exception
        {
            Exception lastException = null;
            var genericExceptionCount = 0;
            var expectedExceptionCount = 0;

            while ((retryCountOnGenericException == -1 || genericExceptionCount <= retryCountOnGenericException) &&
                   (retryCountOnExpectedException == -1 || expectedExceptionCount <= retryCountOnExpectedException))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(cancellationToken);
                }

                try
                {
                    return await func();
                }
                catch (TExpectedException exception)
                {
                    expectedExceptionCount++;
                    lastException = exception;
                    if (expectedExceptionCount < retryCountOnExpectedException)
                    {
                        Task.Delay(retryInterval.ToTimeSpan()).Wait();
                    }
                }
                catch (Exceptions.Exception)
                {
                    throw;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    genericExceptionCount++;
                    lastException = exception;
                    if (genericExceptionCount < retryCountOnGenericException)
                    {
                        Task.Delay(retryInterval.ToTimeSpan()).Wait();
                    }
                }
            }

            throw new Exceptions.Exception("Error executing function", lastException);
        }
    }
}