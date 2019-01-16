// <copyright file="NativeHashMapExtensions.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using BovineLabs.Entities.Helpers;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    /// <summary>
    /// The NativeHashMapExtensions.
    /// </summary>
    public static class NativeHashMapExtensions
    {
        public static NativeHashMapEnumerator<TKey, TValue> GetEnumerator<TKey, TValue>(
            this NativeHashMap<TKey, TValue> hashMap)
            where TKey : struct, IEquatable<TKey>
            where TValue : struct

        {
            return new NativeHashMapEnumerator<TKey, TValue>(ref hashMap);
        }

        public unsafe struct NativeHashMapEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
            where TKey : struct, IEquatable<TKey>
            where TValue : struct
        {
            private readonly NativeHashMapImposter<TKey, TValue> hashMap;
            private readonly int* buckets;
            private readonly byte* keys;
            private readonly byte* values;

            private int index;

            internal NativeHashMapEnumerator(ref NativeHashMap<TKey, TValue> hashMap)
            {
                // Convert to imposter so we can access internal fields
                var imposter = (NativeHashMapImposter<TKey, TValue>)hashMap;

                this.hashMap = hashMap;
                this.index = -1;

                this.buckets = (int*)imposter.Buffer->Buckets;
                this.keys = imposter.Buffer->Keys;
                this.values = imposter.Buffer->Values;
            }

            /// <inheritdoc />
            public KeyValuePair<TKey, TValue> Current => new KeyValuePair<TKey, TValue>(
                UnsafeUtility.ReadArrayElement<TKey>(this.keys, this.index),
                UnsafeUtility.ReadArrayElement<TValue>(this.values, this.index));

            /// <inheritdoc />
            object IEnumerator.Current => this.Current;

            /// <inheritdoc />
            public bool MoveNext()
            {
                var length = this.hashMap.Buffer->BucketCapacityMask + 1;

                for (this.index += 1; this.index < length; this.index++)
                {
                    if (this.buckets[this.index] != -1)
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <inheritdoc />
            public void Reset()
            {
                this.index = -1;
            }

            /// <inheritdoc />
            public void Dispose()
            {
            }
        }
    }
}