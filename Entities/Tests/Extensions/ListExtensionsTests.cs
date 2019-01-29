// <copyright file="ListExtensionsTests.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Tests.Extensions
{
    using System.Collections.Generic;
    using BovineLabs.Entities.Extensions;
    using NUnit.Framework;
    using Unity.Collections;

    /// <summary>
    /// Tests for the <see cref="Entities.Extensions.ListExtensions"/>.
    /// </summary>
    public class ListExtensionsTests
    {
        /// <summary>
        /// Tests AddRange for a NativeArray.
        /// </summary>
        [Test]
        public void AddRangeNativeArray()
        {
            var array = new NativeArray<int>(4, Allocator.TempJob)
            {
                [0] = 1,
                [1] = 2,
                [2] = 3,
                [3] = 4,
            };

            var list = new List<int> { 0 };
            list.AddRange(array, 3);

            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(2, list[2]);
            Assert.AreEqual(3, list[3]);

            array.Dispose();
        }

        /// <summary>
        /// Tests AddRange for a NativeList.
        /// </summary>
        [Test]
        public void AddRangeNativeList()
        {
            var nativeList = new NativeList<int>(Allocator.TempJob);
            nativeList.Add(1);
            nativeList.Add(2);
            nativeList.Add(3);

            var list = new List<int> { 0 };
            list.AddRange(nativeList);

            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(2, list[2]);
            Assert.AreEqual(3, list[3]);

            nativeList.Dispose();
        }
    }
}