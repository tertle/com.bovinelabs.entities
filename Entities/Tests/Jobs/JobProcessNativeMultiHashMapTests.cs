// <copyright file="JobProcessNativeMultiHashMapTests.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Tests.Jobs
{
    using System;
    using System.Collections.Generic;
    using BovineLabs.Entities.Jobs;
    using NUnit.Framework;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities.Tests;

    /// <summary>
    /// The JobProcessNativeMultiHashMapTests.
    /// </summary>
    public class JobProcessNativeMultiHashMapTests : ECSTestsFixture
    {
        [Test]
        public void JobProcessNativeMultiHashMap()
        {
            const int keyCount = 2048;
            const int valueCount = 128;
            const int capacity = keyCount * valueCount;

            var expected = new Dictionary<double, HashSet<double>>(keyCount);

            var values = new NativeMultiHashMap<double, double>(capacity, Allocator.TempJob);

            var random = new Random();

            for (var i = 0; i < keyCount; i++)
            {
                var key = random.NextDouble();

                var valueSet = new HashSet<double>();
                expected.Add(key, valueSet);

                for (var j = 0; j < valueCount; j++)
                {
                    var value = random.NextDouble();

                    valueSet.Add(value);

                    values.Add(key, value);
                }
            }

            var results = new NativeMultiHashMap<double, double>(capacity, Allocator.TempJob);

            var job = new JobTest
            {
                Results = results.ToConcurrent(),
            };

            var handle = job.Schedule(values, 4);
            handle.Complete();

            Assert.AreEqual(capacity, results.Length);

            foreach (var ex in expected)
            {
                // Does the key exist
                Assert.IsTrue(results.TryGetFirstValue(ex.Key, out var first, out var it));

                var set = ex.Value;

                // Assert the value exists
                Assert.IsTrue(set.Remove(first));

                while (results.TryGetNextValue(out var item, ref it))
                {
                    // Assert the value exists
                    Assert.IsTrue(set.Remove(item));
                }

                // All values were added
                Assert.AreEqual(0, set.Count);
            }

            values.Dispose();
            results.Dispose();
        }

        [BurstCompile]
        private struct JobTest : IJobProcessNativeMultiHashMap<double, double>
        {
            public NativeMultiHashMap<double, double>.Concurrent Results;

            /// <inheritdoc />
            public void Execute(double key, double value)
            {
                this.Results.Add(key, value);
            }
        }
    }
}