// <copyright file="UnsafeUtilityExtensions.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Helpers
{
    using System;
    using Unity.Collections.LowLevel.Unsafe;

    /// <summary>
    /// The UnsafeUtilityExtensions.
    /// </summary>
    public static unsafe class UnsafeUtilityExtensions
    {
        public static void MemCpy(void* destination, int destinationOffset, void* source, int sourceOffset, int elementSize, long size)
        {
            destination = (byte*)((IntPtr)destination + (elementSize * destinationOffset));
            source = (byte*)((IntPtr)source + (elementSize * sourceOffset));
            UnsafeUtility.MemCpy(destination, source, size * elementSize);
        }
    }
}