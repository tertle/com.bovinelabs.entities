// <copyright file="WorldExtensions.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Unity.Entities;
    using UnityEngine;

    /// <summary>
    /// Extensions for <see cref="World"/>.
    /// </summary>
    public static class WorldExtensions
    {
        private static bool setup;
        private static FieldInfo behaviourManagers;
        private static Action<World, Type, ScriptBehaviourManager> addTypeLookup;
        private static Action<World, ScriptBehaviourManager> removeManagerInternal;
        private static Action<ScriptBehaviourManager, World> createInstance;
        private static Action<World, int> setVersion;

        /// <summary>
        /// Support for adding existing managers to a world.
        /// </summary>
        /// <param name="world">The <see cref="World"/> to add a <see cref="ScriptBehaviourManager"/> to.</param>
        /// <param name="manager">The <see cref="ScriptBehaviourManager"/> to add to a <see cref="World"/>.</param>
        public static void AddManager(this World world, ScriptBehaviourManager manager)
        {
            if (!setup)
            {
                Setup();
            }

            var managers = (List<ScriptBehaviourManager>)behaviourManagers.GetValue(world);
            managers.Add(manager);

            addTypeLookup(world, manager.GetType(), manager);

            try
            {
                createInstance(manager, world);
            }
            catch (Exception ex)
            {
                removeManagerInternal(world, manager);
                Debug.LogError($"Manager {manager} threw an exception {ex}");
                throw;
            }

            setVersion(world, world.Version + 1);
        }

        private static void Setup()
        {
            behaviourManagers = CreateBehaviourManagerGet();
            addTypeLookup = CreateAddTypeLookup();
            removeManagerInternal = CreateRemoveManagerInternal();
            createInstance = CreateCreateInstance();
            setVersion = CreateVersionSetter();

            setup = true;
        }

        private static FieldInfo CreateBehaviourManagerGet()
        {
            var addTypeLookupMethodInfo = typeof(World).GetField("m_BehaviourManagers", BindingFlags.NonPublic | BindingFlags.Instance);

            if (addTypeLookupMethodInfo == null)
            {
                throw new NullReferenceException("AddTypeLookup changed");
            }

            return addTypeLookupMethodInfo;
        }

        private static Action<World, Type, ScriptBehaviourManager> CreateAddTypeLookup()
        {
            var addTypeLookupMethodInfo = typeof(World).GetMethod(
                "AddTypeLookup",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (addTypeLookupMethodInfo == null)
            {
                throw new NullReferenceException("AddTypeLookup changed");
            }

            return (Action<World, Type, ScriptBehaviourManager>)Delegate.CreateDelegate(typeof(Action<World, Type, ScriptBehaviourManager>), null, addTypeLookupMethodInfo);
        }

        private static Action<World, ScriptBehaviourManager> CreateRemoveManagerInternal()
        {
            var removeManagerInternalMethodInfo = typeof(World).GetMethod(
                "RemoveManagerInteral",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (removeManagerInternalMethodInfo == null)
            {
                throw new NullReferenceException("RemoveManagerInternal changed");
            }

            return (Action<World, ScriptBehaviourManager>)Delegate.CreateDelegate(typeof(Action<World, ScriptBehaviourManager>), null, removeManagerInternalMethodInfo);
        }

        private static Action<ScriptBehaviourManager, World> CreateCreateInstance()
        {
            var createInstanceMethodInfo = typeof(ScriptBehaviourManager).GetMethod(
                "CreateInstance",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (createInstanceMethodInfo == null)
            {
                throw new NullReferenceException("CreateInstance changed");
            }

            return (Action<ScriptBehaviourManager, World>)Delegate.CreateDelegate(typeof(Action<ScriptBehaviourManager, World>), null, createInstanceMethodInfo);
        }

        private static Action<World, int> CreateVersionSetter()
        {
            var versionFieldInfo = typeof(World).GetProperty(
                nameof(World.Version),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            if (versionFieldInfo == null)
            {
                throw new NullReferenceException("Version changed");
            }

            var methodInfo = versionFieldInfo.SetMethod;

            return (Action<World, int>)Delegate.CreateDelegate(typeof(Action<World, int>), null, methodInfo);
        }
    }
}