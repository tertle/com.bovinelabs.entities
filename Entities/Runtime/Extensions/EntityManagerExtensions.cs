// <copyright file="EntityManagerExtensions.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Extensions
{
    using System;
    using System.Reflection;
    using Unity.Entities;

    /// <summary>
    /// Extensions for types within the <see cref="Unity.EntityManager"/> namespace.
    /// </summary>
    public static class EntityManagerExtensions
    {
        private static Action<EntityManager, Entity, ComponentType, object> setComponentObject;
        private static bool setup;

        /// <summary>
        /// Cached reflection for the internal method SetComponentObject within the EntityManager.
        /// </summary>
        /// <param name="entityManager"><see cref="EntityManager"/>.</param>
        /// <param name="entity">The <see cref="Entity"/> to set the object to.</param>
        /// <param name="componentType">The <see cref="ComponentType"/> of the object to set.</param>
        /// <param name="componentObject">The object to set.</param>
        public static void SetComponentObject(this EntityManager entityManager, Entity entity, ComponentType componentType, object componentObject)
        {
            if (!setup)
            {
                Setup();
            }

            setComponentObject.Invoke(entityManager, entity, componentType, componentObject);
        }

        private static void Setup()
        {
            setComponentObject = CreateSetComponentObject();

            setup = true;
        }

        private static Action<EntityManager, Entity, ComponentType, object> CreateSetComponentObject()
        {
            var addTypeLookupMethodInfo = typeof(EntityManager).GetMethod(
                "SetComponentObject",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (addTypeLookupMethodInfo == null)
            {
                throw new NullReferenceException("SetComponentObject changed");
            }

            return (Action<EntityManager, Entity, ComponentType, object>)Delegate.CreateDelegate(typeof(Action<EntityManager, Entity, ComponentType, object>), null, addTypeLookupMethodInfo);
        }
    }
}
