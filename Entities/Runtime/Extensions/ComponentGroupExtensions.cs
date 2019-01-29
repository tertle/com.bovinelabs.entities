// <copyright file="ComponentGroupExtensions.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Extensions
{
    using Unity.Entities;

    /// <summary>
    /// The ComponentSystemBaseExtensions.
    /// </summary>
    public static class ComponentGroupExtensions
    {
        public static T GetSingleton<T>(this ComponentGroup group)
            where T : struct, IComponentData
        {
            var array = group.GetComponentDataArray<T>();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (array.Length != 1)
            {
                throw new System.InvalidOperationException(
                    $"GetSingleton<{typeof(T)}>() requires that exactly one {typeof(T)} exists but there are {array.Length}.");
            }
#endif

            group.CompleteDependency();
            return array[0];
        }

        public static DynamicBuffer<T> GetSingletonBuffer<T>(this ComponentGroup group)
            where T : struct, IBufferElementData
        {
            var array = group.GetBufferArray<T>();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (array.Length != 1)
            {
                throw new System.InvalidOperationException(
                    $"GetSingleton<{typeof(T)}>() requires that exactly one {typeof(T)} exists but there are {array.Length}.");
            }
#endif

            group.CompleteDependency();
            return array[0];
        }

        public static void SetSingleton<T>(this ComponentGroup group, T value)
            where T : struct, IComponentData
        {
            var array = group.GetComponentDataArray<T>();
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (array.Length != 1)
            {
                throw new System.InvalidOperationException(
                    $"SetSingleton<{typeof(T)}>() requires that exactly one {typeof(T)} exists but there are {array.Length}.");
            }
#endif

            group.CompleteDependency();
            array[0] = value;
        }
    }
}