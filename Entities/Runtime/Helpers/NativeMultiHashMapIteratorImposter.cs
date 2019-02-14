// <copyright file="NativeMultiHashMapIteratorImposter.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Helpers
{
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    public struct NativeMultiHashMapIteratorImposter<TKey>
        where TKey : struct
    {
        public TKey key;
        public int NextEntryIndex;
        public int EntryIndex;

        public static unsafe implicit operator NativeMultiHashMapIteratorImposter<TKey>(NativeMultiHashMapIterator<TKey> it)
        {
            var ptr = UnsafeUtility.AddressOf(ref it);
            UnsafeUtility.CopyPtrToStructure(ptr, out NativeMultiHashMapIteratorImposter<TKey> imposter);
            return imposter;
        }
    }
}