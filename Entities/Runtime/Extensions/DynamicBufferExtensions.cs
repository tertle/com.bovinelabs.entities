// <copyright file="DynamicBufferExtensions.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Extensions
{
    using System;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Entities;

    /// <summary>
    /// The DynamicBufferExtensions.
    /// </summary>
    public static class DynamicBufferExtensions
    {
        public static bool Contains<T, TI>(this DynamicBuffer<T> buffer, TI item)
            where T : struct, IEquatable<TI>
            where TI : struct
        {
            return buffer.IndexOf(item) >= 0;
        }

        public static int IndexOf<T, TI>(this DynamicBuffer<T> buffer, TI item)
            where T : struct, IEquatable<TI>
            where TI : struct
        {
            var length = buffer.Length;

            for (int index = 0; index < length; ++index)
            {
                if (buffer[index].Equals(item))
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Remove an element from a <see cref="DynamicBuffer{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of NativeList</typeparam>
        /// <typeparam name="TI">The type of element.</typeparam>
        /// <param name="buffer">The DynamicBuffer.</param>
        /// <param name="element">The element.</param>
        /// <returns>True if removed, else false.</returns>
        public static bool Remove<T, TI>(this DynamicBuffer<T> buffer, TI element)
            where T : struct, IEquatable<TI>
            where TI : struct
        {
            var index = buffer.IndexOf(element);
            if (index < 0)
            {
                return false;
            }

            buffer.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Reverse a <see cref="DynamicBuffer{T}"/>.
        /// </summary>
        /// <typeparam name="T"><see cref="DynamicBuffer{T}"/>.</typeparam>
        /// <param name="buffer">The <see cref="DynamicBuffer{T}"/> to reverse.</param>
        public static void Reverse<T>(this DynamicBuffer<T> buffer)
            where T : struct
        {
            var length = buffer.Length;
            var index1 = 0;

            for (var index2 = length - 1; index1 < index2; --index2)
            {
                var obj = buffer[index1];
                buffer[index1] = buffer[index2];
                buffer[index2] = obj;
                ++index1;
            }
        }

        /// <summary>
        /// Resizes a <see cref="DynamicBuffer{T}"/> and then clears the memory.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="buffer">The <see cref="DynamicBuffer{T}"/> to resize.</param>
        /// <param name="length">Size to resize to.</param>
        public static unsafe void ResizeInitialized<T>(this DynamicBuffer<T> buffer, int length)
            where T : struct
        {
            buffer.ResizeUninitialized(length);
            UnsafeUtility.MemClear(buffer.GetUnsafePtr(), length * UnsafeUtility.SizeOf<T>());
        }

        /// <summary>
        /// Extension for <see cref="DynamicBuffer{T}"/> to copy to <see cref="NativeArray{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="buffer">The <see cref="DynamicBuffer{T}"/> to copy from.</param>
        /// <param name="nativeArray">The <see cref="NativeArray{T}"/> to copy to.</param>
        public static unsafe void CopyTo<T>(this DynamicBuffer<T> buffer, NativeArray<T> nativeArray)
            where T : struct
        {
            if (buffer.Length != nativeArray.Length)
            {
                throw new ArgumentException("array.Length does not match the length of this instance");
            }

            UnsafeUtility.MemCpy(nativeArray.GetUnsafePtr(), buffer.GetUnsafePtr(), buffer.Length * (long)UnsafeUtility.SizeOf<T>());
        }

        /// <summary>
        /// Fast element removal when order is not important. Replaces the element at <see cref="index"/> with
        /// the last element in the <see cref="DynamicBuffer{T}"/> and reduces length by 1.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="buffer">The <see cref="DynamicBuffer{T}"/> to remove from.</param>
        /// <param name="index">The index to remove.</param>
        public static void RemoveAtSwapBack<T>(this DynamicBuffer<T> buffer, int index)
            where T : struct
        {
            var newLength = buffer.Length - 1;
            buffer[index] = buffer[newLength];
            buffer.ResizeUninitialized(newLength);
        }

        public static void AddRange<T>(this DynamicBuffer<T> buffer, T[] array)
            where T : struct
        {
            var a = new NativeArray<T>(array, Allocator.Temp);
            buffer.AddRange(a);
            a.Dispose();
        }
    }
}