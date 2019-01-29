// <copyright file="TextAssetReader.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
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

        /// <summary>
        /// Initializes a new instance of the <see cref="TextAssetReader"/> class.
        /// </summary>
        /// <param name="textAsset">The <see cref="TextAsset"/>.</param>
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
                UnsafeUtilityExtensions.MemCpy(data, 0, fixedBuffer, this.offset, UnsafeUtility.SizeOf<byte>(), count);
                this.offset += count;
            }
        }
    }
}