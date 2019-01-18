// <copyright file="NativeHashMapExtensionsTests.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Tests.Extensions
{
    using System.Collections.Generic;
    using BovineLabs.Entities.Extensions;
    using NUnit.Framework;
    using Unity.Collections;

    /// <summary>
    /// The NativeMultiHashMapExtensions.
    /// </summary>
    public class NativeHashMapExtensionsTest
    {
        [Test]
        public void GetEnumerator()
        {
            const int elements = 64;

            var map = new NativeHashMap<int, int>(elements, Allocator.TempJob);
            var result = new Dictionary<int, int>();

            for (var i = 0; i < elements; i++)
            {
                map.TryAdd(i, i + 1);
            }

            // Quick remove test
            map.Remove(0);

            using (var enumerator = map.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<int, int> kvp = enumerator.Current;
                    result.Add(kvp.Key, kvp.Value);
                }
            }

            Assert.AreEqual(elements - 1, result.Count);

            for (var i = 1; i < elements; i++)
            {
                Assert.IsTrue(result.TryGetValue(i, out var value));
                Assert.AreEqual(i + 1, value);
            }

            map.Dispose();
        }
    }
}