// <copyright file="JobComponentSystemExtensions.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Extensions
{
    using System;
    using System.Reflection;
    using Unity.Entities;
    using Unity.Jobs;

    /// <summary>
    /// Extensions for the <see cref="JobComponentSystem"/>.
    /// </summary>
    public static class JobComponentSystemExtensions
    {
        private static bool setup;
        private static PropertyInfo worldPropertyInfo;
        private static FieldInfo barrierListFieldInfo;

        /// <summary>
        /// Gets or creates a barrier and correctly adds it to the barrier list.
        /// </summary>
        /// <typeparam name="T">The type of the barrier to get or create.</typeparam>
        /// <param name="componentSystem">The <see cref="JobComponentSystem"/>.</param>
        /// <returns>The barrier.</returns>
        public static T GetBarrier<T>(this JobComponentSystem componentSystem)
            where T : BarrierSystem
        {
            if (!setup)
            {
                Setup();
            }

            var world = (World)worldPropertyInfo.GetValue(componentSystem);
            var barrierList = (BarrierSystem[])barrierListFieldInfo.GetValue(componentSystem);

            Array.Resize(ref barrierList, barrierList.Length + 1);

            var barrier = world.GetOrCreateManager<T>();
            barrierList[barrierList.Length - 1] = barrier;

            barrierListFieldInfo.SetValue(componentSystem, barrierList);

            return barrier;
        }

        public static void AddBarrier(this JobComponentSystem componentSystem, BarrierSystem barrier)
        {
            if (!setup)
            {
                Setup();
            }

            var barrierList = (BarrierSystem[])barrierListFieldInfo.GetValue(componentSystem);

            Array.Resize(ref barrierList, barrierList.Length + 1);

            barrierList[barrierList.Length - 1] = barrier;

            barrierListFieldInfo.SetValue(componentSystem, barrierList);
        }

        private static void Setup()
        {
            worldPropertyInfo = CreateWorldGet();
            barrierListFieldInfo = CreateBarrierListGet();

            setup = true;
        }

        private static PropertyInfo CreateWorldGet()
        {
            var propertyInfo =
                typeof(ComponentSystemBase).GetProperty("World", BindingFlags.NonPublic | BindingFlags.Instance);

            if (propertyInfo == null)
            {
                throw new NullReferenceException("World changed");
            }

            return propertyInfo;
        }

        private static FieldInfo CreateBarrierListGet()
        {
            var fieldInfo =
                typeof(JobComponentSystem).GetField("m_BarrierList", BindingFlags.NonPublic | BindingFlags.Instance);

            if (fieldInfo == null)
            {
                throw new NullReferenceException("m_BarrierList changed");
            }

            return fieldInfo;
        }
    }
}
