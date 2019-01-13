// <copyright file="NativeMultiHashMapExtensions.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using BovineLabs.Entities.Jobs;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    /// <summary>
    /// The NativeMultiHashMapExtensions.
    /// </summary>
    public static class NativeMultiHashMapExtensions
    {
        /// <summary>
        /// Get an Enumerator for a <see cref="NativeMultiHashMap{TKey,TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">They key type of the hash map.</typeparam>
        /// <typeparam name="TValue">the value type of the hash map.</typeparam>
        /// <param name="hashMap">The hash map</param>
        /// <returns>The enumerator for the hash map.</returns>
        public static NativeMultiHashMapEnumerator<TKey, TValue> GetEnumerator<TKey, TValue>(
            this NativeMultiHashMap<TKey, TValue> hashMap)
            where TKey : struct, IEquatable<TKey>
            where TValue : struct

        {
            return new NativeMultiHashMapEnumerator<TKey, TValue>(ref hashMap);
        }

        /// <summary>
        /// A struct to handle enumerators of a <see cref="NativeMultiHashMap{TKey,TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The key.</typeparam>
        /// <typeparam name="TValue">The value.</typeparam>
        public unsafe struct NativeMultiHashMapEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
            where TKey : struct, IEquatable<TKey>
            where TValue : struct
        {
            private readonly NativeMultiHashMapImposter<TKey, TValue> hashMap;
            private readonly int* buckets;
            private readonly int* nextPtrs;
            private readonly byte* keys;
            private readonly byte* values;

            private int index;
            private int entryIndex;

            /// <summary>
            /// Initializes a new instance of the <see cref="NativeMultiHashMapEnumerator{TKey, TValue}"/> struct.
            /// </summary>
            /// <param name="hashMap">The hashmap to iterate.</param>
            internal NativeMultiHashMapEnumerator(ref NativeMultiHashMap<TKey, TValue> hashMap)
            {
                // Convert to imposter so we can access internal fields
                var imposter = (NativeMultiHashMapImposter<TKey, TValue>)hashMap;

                this.hashMap = hashMap;
                this.index = 0;
                this.entryIndex = -1;

                this.buckets = (int*)imposter.m_Buffer->buckets;
                this.nextPtrs = (int*)imposter.m_Buffer->next;
                this.keys = imposter.m_Buffer->keys;
                this.values = imposter.m_Buffer->values;
            }

            /// <inheritdoc />
            public KeyValuePair<TKey, TValue> Current => new KeyValuePair<TKey, TValue>(
                UnsafeUtility.ReadArrayElement<TKey>(this.keys, this.entryIndex),
                UnsafeUtility.ReadArrayElement<TValue>(this.values, this.entryIndex));

            /// <inheritdoc />
            object IEnumerator.Current => this.Current;

            /// <inheritdoc />
            public bool MoveNext()
            {
                var length = this.hashMap.m_Buffer->bucketCapacityMask + 1;

                for (; this.index < length; this.index++)
                {
                    this.entryIndex = this.entryIndex == -1
                        ? this.buckets[this.index]
                        : this.nextPtrs[this.entryIndex];

                    if (this.entryIndex != -1)
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <inheritdoc />
            public void Reset()
            {
                this.index = 0;
                this.entryIndex = -1;
            }

            /// <inheritdoc />
            public void Dispose()
            {
            }
        }
    }
}