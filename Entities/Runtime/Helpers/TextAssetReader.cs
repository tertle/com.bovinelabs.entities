// <copyright file="TextAssetReader.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Helpers
{
    using System;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Entities.Serialization;
    using UnityEngine;

    /// <summary>
    /// The TextAssetReader.
    /// </summary>
    public unsafe class TextAssetReader : BinaryReader
    {
        private byte[] bytes;
        private int offset;

        public TextAssetReader(TextAsset textAsset)
        {
            this.bytes = textAsset.bytes;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.bytes = null;
        }

        /// <inheritdoc />
        public void ReadBytes(void* data, int count)
        {
            fixed (byte* fixedBuffer = this.bytes)
            {
                //UnsafeUtility.MemCpyStride(data, UnsafeUtility.SizeOf<byte>() * this.offset, fixedBuffer, 0, UnsafeUtility.SizeOf<byte>(), count);
                UnsafeUtilityExtensions.MemCpy(data, 0, fixedBuffer, this.offset, UnsafeUtility.SizeOf<byte>(), count);
                this.offset += count;
            }
        }
    }
}