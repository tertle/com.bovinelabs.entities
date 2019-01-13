// <copyright file="NativeMultiHashMapExtensions.cs" company="Timothy Raines">
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
    public class NativeMultiHashMapExtensionsTest
    {
        [Test]
        public void GetEnumerator()
        {
            const int keys = 8;
            const int values = 8;

            var map = new NativeMultiHashMap<int, int>(keys * values, Allocator.TempJob);
            var result = new Dictionary<int, List<int>>();

            for (var i = 0; i < keys; i++)
            {
                for (var j = 0; j < values; j++)
                {
                    map.Add(i, j);
                }
            }

            using (NativeMultiHashMapEnumerator<int, int> e = map.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    KeyValuePair<int, int> kvp = e.Current;

                    if (!result.TryGetValue(kvp.Key, out var list))
                    {
                        list = result[kvp.Key] = new List<int>();
                    }

                    list.Add(kvp.Value);
                }
            }

            foreach (var kvp in result)
            {
                Assert.AreEqual(8, kvp.Value.Count);

                kvp.Value.Sort();

                for (var i = 0; i < values; i++)
                {
                    Assert.AreEqual(i, kvp.Value[i]);
                }
            }

            map.Dispose();
        }


    }
}