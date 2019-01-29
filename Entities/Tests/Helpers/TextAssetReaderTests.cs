// <copyright file="TextAssetReaderTests.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Tests.Helpers
{
    using System;
    using BovineLabs.Entities.Helpers;
    using NUnit.Framework;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using UnityEngine;

    /// <summary>
    /// The TextAssetReaderTests.
    /// </summary>
    public class TextAssetReaderTests
    {
        [Test]
        public unsafe void ReadBytes()
        {
            var textAsset = new TextAsset("0123");
            var reader = new TextAssetReader(textAsset);

            var bytes = new NativeArray<byte>(4, Allocator.TempJob);

            byte* destination = (byte*)bytes.GetUnsafePtr();
            reader.ReadBytes(destination, 4);

            for (var index = 0; index < 4; index++)
            {
                Assert.AreEqual(index, bytes[index] - 48);
            }

            bytes.Dispose();
        }

        [Test]
        public unsafe void ReadBytesInt()
        {
            var textAsset = new TextAsset("0123");
            var reader = new TextAssetReader(textAsset);

            for (byte index = 0; index < 4; index++)
            {
                byte value;
                reader.ReadBytes(&value, sizeof(byte));
                Assert.AreEqual(index, value - 48);
            }
        }
    }
}