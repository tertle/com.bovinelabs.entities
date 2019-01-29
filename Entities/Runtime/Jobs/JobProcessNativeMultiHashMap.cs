// <copyright file="JobProcessNativeMultiHashMap.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Jobs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using BovineLabs.Entities.Helpers;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Jobs;
    using Unity.Jobs.LowLevel.Unsafe;

    /// <summary>
    /// Iterates a NativeMultiHashMap.
    /// </summary>
    /// <typeparam name="TKey">The key.</typeparam>
    /// <typeparam name="TValue">The value.</typeparam>
    public interface IJobProcessNativeMultiHashMap<in TKey, in TValue>
        where TKey : struct, IEquatable<TKey>
        where TValue : struct
    {
        /// <summary>
        /// Called for every key, value pair of the <see cref="NativeMultiHashMap{TKey,TValue}"/>.
        /// </summary>
        /// <param name="key">The value of the key.</param>
        /// <param name="value">The value of the pair.</param>
        void Execute(TKey key, TValue value);
    }

    /// <summary>
    /// Provides extensions to schedule <see cref="IJobProcessNativeMultiHashMap{TKey, TValue}"/>.
    /// </summary>
    public static class JobProcessNativeMultiHashMapExtensions
    {
        /// <summary>
        /// Schedules the <see cref="IJobProcessNativeMultiHashMap{TKey, TValue}"/> in parallel.
        /// </summary>
        /// <typeparam name="TJob">Type of job.</typeparam>
        /// <typeparam name="TKey">Type of the key for the <see cref="NativeMultiHashMap{TKey,TValue}"/>.</typeparam>
        /// <typeparam name="TValue">Type of the value for the <see cref="NativeMultiHashMap{TKey,TValue}"/>.</typeparam>
        /// <param name="jobData">The job.</param>
        /// <param name="hashMap">The <see cref="NativeMultiHashMap{TKey,TValue}"/> to iterate.</param>
        /// <param name="minIndicesPerJobCount">The minIndicesPerJobCount.</param>
        /// <param name="dependsOn">Optional dependency.</param>
        /// <returns>A <see cref="JobHandle"/>.</returns>
        public static unsafe JobHandle Schedule<TJob, TKey, TValue>(
            this TJob jobData,
            NativeMultiHashMap<TKey, TValue> hashMap,
            int minIndicesPerJobCount,
            JobHandle dependsOn = default)
            where TJob : struct, IJobProcessNativeMultiHashMap<TKey, TValue>
            where TKey : struct, IEquatable<TKey>
            where TValue : struct
        {
            var fullData = new NativeMultiHashMapUniqueHashJobStruct<TJob, TKey, TValue>.JobMultiHashMap
            {
                HashMap = hashMap,
                JobData = jobData,
            };

            var scheduleParams = new JobsUtility.JobScheduleParameters(
                UnsafeUtility.AddressOf(ref fullData),
                NativeMultiHashMapUniqueHashJobStruct<TJob, TKey, TValue>.Initialize(),
                dependsOn,
                ScheduleMode.Batched);

            return JobsUtility.ScheduleParallelFor(
                ref scheduleParams,
                fullData.HashMap.Buffer->BucketCapacityMask + 1,
                minIndicesPerJobCount);
        }

        private unsafe struct NativeMultiHashMapUniqueHashJobStruct<TJob, TKey, TValue>
            where TJob : struct, IJobProcessNativeMultiHashMap<TKey, TValue>
            where TKey : struct, IEquatable<TKey>
            where TValue : struct
        {
            [SuppressMessage("ReSharper", "StaticMemberInGenericType", Justification = "Intended behaviour.")]
            private static IntPtr jobReflectionData;

            private delegate void ExecuteJobFunction(
                ref JobMultiHashMap fullData,
                IntPtr additionalPtr,
                IntPtr bufferRangePatchData,
                ref JobRanges ranges,
                int jobIndex);

            public static IntPtr Initialize()
            {
                if (jobReflectionData == IntPtr.Zero)
                {
                    jobReflectionData = JobsUtility.CreateJobReflectionData(
                        typeof(JobMultiHashMap),
                        typeof(TJob),
                        JobType.ParallelFor,
                        (ExecuteJobFunction)Execute);
                }

                return jobReflectionData;
            }

            private static void Execute(
                ref JobMultiHashMap fullData,
                IntPtr additionalPtr,
                IntPtr bufferRangePatchData,
                ref JobRanges ranges,
                int jobIndex)
            {
                while (true)
                {
                    if (!JobsUtility.GetWorkStealingRange(ref ranges, jobIndex, out var begin, out var end))
                    {
                        return;
                    }

                    var buckets = (int*)fullData.HashMap.Buffer->Buckets;
                    var nextPtrs = (int*)fullData.HashMap.Buffer->Next;
                    var keys = fullData.HashMap.Buffer->Keys;
                    var values = fullData.HashMap.Buffer->Values;

                    for (int i = begin; i < end; i++)
                    {
                        int entryIndex = buckets[i];
                        while (entryIndex != -1)
                        {
                            var key = UnsafeUtility.ReadArrayElement<TKey>(keys, entryIndex);
                            var value = UnsafeUtility.ReadArrayElement<TValue>(values, entryIndex);

                            fullData.JobData.Execute(key, value);
                            entryIndex = nextPtrs[entryIndex];
                        }
                    }
                }
            }

            internal struct JobMultiHashMap
            {
                public NativeMultiHashMapImposter<TKey, TValue> HashMap;

                public TJob JobData;
            }
        }
    }
}