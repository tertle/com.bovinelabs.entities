// <copyright file="NoAllocHelpersTests.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Tests.Helpers
{
    using System.Collections.Generic;
    using BovineLabs.Entities.Helpers;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="NoAllocHelpers"/>.
    /// </summary>
    public class NoAllocHelpersTests
    {
        /// <summary>
        /// Tests <see cref="NoAllocHelpers.ExtractArrayFromListT{T}"/>.
        /// </summary>
        [Test]
        public void ExtractArrayFromListTReturnsInternalList()
        {
            var list = new List<int>(5);
            for (var i = 0; i < 5; i++)
            {
                list.Add(0);
            }

            var array = NoAllocHelpers.ExtractArrayFromListT(list);
            array[3] = 4;

            Assert.AreEqual(4, list[3]);
        }

        /// <summary>
        /// Tests <see cref="NoAllocHelpers.ResizeList{T}"/>.
        /// </summary>
        [Test]
        public void ResizeListCorrectCount()
        {
            var list = new List<int>(5);

            NoAllocHelpers.ResizeList(list, 3);
            Assert.AreEqual(3, list.Count);
        }
    }
}