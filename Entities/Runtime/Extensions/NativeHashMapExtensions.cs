// <copyright file="NativeHashMapExtensions.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Extensions
{
    using System;
    using BovineLabs.Entities.Helpers;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    /// <summary>
    /// The NativeHashMapExtensions.
    /// </summary>
    public static class NativeHashMapExtensions
    {
        /// <summary>
        /// Based off https://forum.unity.com/threads/nativehashmap-tryreplacevalue.629512/
        /// </summary>
        public static unsafe bool TryReplaceValue<TKey, TValue>(this NativeHashMap<TKey, TValue> hashMap, TKey key, TValue item)
            where TKey : struct, IEquatable<TKey>
            where TValue : struct
        {
            var imposter = (NativeHashMapImposter<TKey, TValue>)hashMap;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(imposter.Safety);
#endif
            return NativeHashMapImposter<TKey, TValue>.TryReplaceValue(imposter.Buffer, key, item, false);
        }
    }
}