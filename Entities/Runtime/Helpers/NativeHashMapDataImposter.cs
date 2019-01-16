// <copyright file="NativeHashMapDataImposter.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Helpers
{
    using System.Runtime.InteropServices;
    using Unity.Jobs.LowLevel.Unsafe;

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct NativeHashMapDataImposter
    {
        public byte* Values;
        public byte* Keys;
        public byte* Next;
        public byte* Buckets;
        public int Capacity;

        public int BucketCapacityMask; // = bucket capacity - 1

        // Add padding between fields to ensure they are on separate cache-lines
        private fixed byte padding1[60];

        public fixed int FirstFreeTLS[JobsUtility.MaxJobThreadCount * IntsPerCacheLine];
        public int AllocatedIndexLength;

        // 64 is the cache line size on x86, arm usually has 32 - so it is possible to save some memory there
        public const int IntsPerCacheLine = JobsUtility.CacheLineSize / sizeof(int);
    }
}