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
        public static NativeMultiHashMapEnumerator<TKey, TValue> GetEnumerator<TKey, TValue>(
            this NativeMultiHashMap<TKey, TValue> hashMap)
            where TKey : struct, IEquatable<TKey>
            where TValue : struct

        {
            return new NativeMultiHashMapEnumerator<TKey, TValue>(ref hashMap);
        }

        public unsafe struct NativeMultiHashMapEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
            where TKey : struct, IEquatable<TKey>
            where TValue : struct
        {
            private NativeMultiHashMapImposter<TKey, TValue> hashMap;
            private int index;
            private int* buckets;
            private int* nextPtrs;
            private byte* keys;
            private byte* values;

            private int entryIndex;

            internal NativeMultiHashMapEnumerator(ref NativeMultiHashMap<TKey, TValue> hashMap)
            {
                // Convert to imposter so we can access internal fields
                var imposter = (NativeMultiHashMapImposter<TKey, TValue>)hashMap;

                this.hashMap = hashMap;
                this.index = 0;

                this.buckets = (int*)imposter.m_Buffer->buckets;
                this.nextPtrs = (int*)imposter.m_Buffer->next;
                this.keys = imposter.m_Buffer->keys;
                this.values = imposter.m_Buffer->values;

                this.entryIndex = -1;
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
                this.index = 1;
            }

            /// <inheritdoc />
            public void Dispose()
            {
            }
        }
    }
}