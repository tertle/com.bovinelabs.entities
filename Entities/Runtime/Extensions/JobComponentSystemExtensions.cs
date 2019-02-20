// <copyright file="JobComponentSystemExtensions.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
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
        private static FieldInfo barrierListFieldInfo;

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
            barrierListFieldInfo = CreateBarrierListGet();

            setup = true;
        }

        private static FieldInfo CreateBarrierListGet()
        {
            var fieldInfo =
                typeof(JobComponentSystem).GetField("m_BarrierList", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            if (fieldInfo == null)
            {
                throw new NullReferenceException("m_BarrierList changed");
            }

            return fieldInfo;
        }
    }
}
