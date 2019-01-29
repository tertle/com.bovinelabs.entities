// <copyright file="NativeListExtensionsTests.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Tests.Extensions
{
    using BovineLabs.Entities.Extensions;
    using NUnit.Framework;
    using Unity.Collections;

    /// <summary>
    /// Tests for <see cref="NativeListExtensions"/>.
    /// </summary>
    public class NativeListExtensionsTests
    {
        /// <summary>
        /// Tests if the Insert extension inserts in the correct place and data is preserved.
        /// </summary>
        [Test]
        public void Insert()
        {
            var list = new NativeList<int>(Allocator.Temp);

            list.Add(1);
            list.Add(2);
            list.Add(3);

            list.Insert(4, 0);

            Assert.AreEqual(4, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(3, list[3]);

            list.Insert(5, 2);

            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(5, list[2]);
            Assert.AreEqual(2, list[3]);
            Assert.AreEqual(3, list[4]);

            list.Insert(6, 5);

            Assert.AreEqual(6, list[5]);

            list.Dispose();
        }

        /// <summary>
        /// Tests if the Remove extension removes the correct element and the data is preserved.
        /// </summary>
        [Test]
        public void RemoveAt()
        {
            var list = new NativeList<int>(Allocator.Temp);

            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);

            list.RemoveAt(0);

            Assert.AreEqual(3, list.Length);
            Assert.AreEqual(2, list[0]);
            Assert.AreEqual(3, list[1]);
            Assert.AreEqual(4, list[2]);

            list.RemoveAt(1);

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(2, list[0]);
            Assert.AreEqual(4, list[1]);
        }

        /// <summary>
        /// Tests if the RemoveRange extension removes the correct elements and the data is preserved.
        /// </summary>
        [Test]
        public void RemoveRange()
        {
            var list = new NativeList<int>(Allocator.Temp);

            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);

            list.RemoveRange(1, 2);

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(4, list[1]);
        }
    }
}