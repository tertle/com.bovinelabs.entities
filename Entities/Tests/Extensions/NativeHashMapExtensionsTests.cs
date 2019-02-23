// <copyright file="NativeHashMapExtensionsTests.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
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
        public void TryReplaceValue()
        {
            using (var map = new NativeHashMap<int, int>(4, Allocator.TempJob))
            {
                map.TryAdd(2, 2);
                map.TryAdd(3, 2);
                map.TryAdd(4, 4);
                Assert.IsTrue(map.TryAdd(1, 2));

                Assert.IsTrue(map.TryGetValue(1, out var value));
                Assert.AreEqual(2, value);

                Assert.IsTrue(map.TryReplaceValue(1, 3));
                Assert.IsTrue(map.TryGetValue(1, out value));
                Assert.AreEqual(3, value);

                Assert.IsFalse(map.TryReplaceValue(0, 3));
            }
        }
    }
}