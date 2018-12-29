// <copyright file="EventBatchSystem.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Common.Systems
{
    using System;
    using System.Collections.Generic;
    using BovineLabs.Common.Extensions;
    using BovineLabs.Common.Jobs;
    using JetBrains.Annotations;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Entities;
    using Unity.Jobs;
    using UnityEngine.Profiling;

    /// <summary>
    /// The BatchBarrierSystem.
    /// </summary>
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class EventBatchSystem : ComponentSystem
    {
        private readonly Dictionary<Type, IEventBatch> types = new Dictionary<Type, IEventBatch>();

        /// <summary>
        /// The interface for the batch systems.
        /// </summary>
        private interface IEventBatch : IDisposable
        {
            /// <summary>
            /// Updates the batch, destroy, create, set.
            /// </summary>
            /// <param name="entityManager">The <see cref="EntityManager"/>.</param>
            /// <returns>A <see cref="JobHandle"/>.</returns>
            JobHandle Update(EntityManager entityManager);

            /// <summary>
            /// Resets the batch for the next frame.
            /// </summary>
            void Reset();
        }

        /// <summary>
        /// Any component added to the returned <see cref="NativeQueue{T}"/> will be attached to a new entity as an event.
        /// These entities will be automatically destroyed 1 frame later.
        /// </summary>
        /// <typeparam name="T">The type of component data event.</typeparam>
        /// <param name="componentSystem">The component system getting the queue.</param>
        /// <returns>A <see cref="NativeQueue{T}"/> which any component that is added will be turned into a single frame event.</returns>
        public NativeQueue<T> CreateEventQueue<T>(JobComponentSystem componentSystem)
            where T : struct, IComponentData
        {
            if (!this.types.TryGetValue(typeof(T), out var create))
            {
                create = this.types[typeof(T)] = new EventBatch<T>();
            }

            return ((EventBatch<T>)create).GetNew(componentSystem);
        }

        /// <inheritdoc />
        protected override void OnDestroyManager()
        {
            foreach (var t in this.types)
            {
                t.Value.Dispose();
            }

            this.types.Clear();
        }

        /// <inheritdoc />
        protected override void OnUpdate()
        {
            var handles = new NativeArray<JobHandle>(this.types.Count, Allocator.TempJob);

            int index = 0;

            foreach (var t in this.types)
            {
                handles[index++] = t.Value.Update(this.EntityManager);
            }

            JobHandle.CompleteAll(handles);
            handles.Dispose();

            foreach (var t in this.types)
            {
                t.Value.Reset();
            }
        }

        private class EventBatch<T> : IEventBatch
            where T : struct, IComponentData
        {
            private readonly HashSet<JobComponentSystem> dependencies = new HashSet<JobComponentSystem>();
            private readonly List<NativeQueue<T>> queues = new List<NativeQueue<T>>();
            private readonly EntityArchetypeQuery query;

            private EntityArchetype archetype;
            private NativeArray<Entity> entities;

            public EventBatch()
            {
                this.query = new EntityArchetypeQuery
                {
                    Any = Array.Empty<ComponentType>(),
                    None = Array.Empty<ComponentType>(),
                    All = new[] { ComponentType.Create<T>() },
                };
            }

            public NativeQueue<T> GetNew(JobComponentSystem componentSystem)
            {
                // Having allocation leak warnings when using TempJob
                var queue = new NativeQueue<T>(Allocator.Persistent);
                this.queues.Add(queue);

                this.dependencies.Add(componentSystem);

                return queue;
            }

            /// <inheritdoc />
            public JobHandle Update(EntityManager entityManager)
            {
                this.CompleteDependencies();
                this.DestroyEntities(entityManager);

                if (!this.CreateEntities(entityManager))
                {
                    return default;
                }

                return this.SetComponentData(entityManager);
            }

            public void Reset()
            {
                foreach (var queue in this.queues)
                {
                    queue.Dispose();
                }

                this.queues.Clear();
            }

            public void Dispose()
            {
                if (this.entities.IsCreated)
                {
                    this.entities.Dispose();
                }

                this.Reset();
            }

            private JobHandle SetComponentData(EntityManager entityManager)
            {
                var componentType = entityManager.GetArchetypeChunkComponentType<T>(false);

                var chunks = entityManager.CreateArchetypeChunkArray(this.query, Allocator.TempJob);

                int startIndex = 0;

                var handles = new NativeArray<JobHandle>(this.queues.Count, Allocator.TempJob);

                // Create a job for each queue. This is designed so that these jobs can run simultaneously.
                for (var index = 0; index < this.queues.Count; index++)
                {
                    var queue = this.queues[index];
                    var job = new SetComponentDataJob
                    {
                        Chunks = chunks,
                        Queue = queue,
                        StartIndex = startIndex,
                        ComponentType = componentType,
                    };

                    startIndex += queue.Count;

                    handles[index] = job.Schedule();
                }

                var handle = JobHandle.CombineDependencies(handles);
                handles.Dispose();

                // Deallocate the chunk array
                handle = new DeallocateJob<NativeArray<ArchetypeChunk>>(chunks).Schedule(handle);

                return handle;
            }

            private bool CreateEntities(EntityManager entityManager)
            {
                var count = this.GetCount();

                if (count == 0)
                {
                    return false;
                }

                Profiler.BeginSample("CreateEntity");

                // Felt like Temp should be the allocator but gets disposed for some reason.
                this.entities = new NativeArray<Entity>(count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

                if (!this.archetype.Valid)
                {
                    this.archetype = entityManager.CreateArchetype(typeof(T));
                }

                entityManager.CreateEntity(this.archetype, this.entities);

                Profiler.EndSample();
                return true;
            }

            private void CompleteDependencies()
            {
                Profiler.BeginSample("Dependencies");

                JobHandle dependencyHandle = default;

                foreach (var system in this.dependencies)
                {
                    dependencyHandle = JobHandle.CombineDependencies(dependencyHandle, system.GetJobHandle());
                }

                dependencyHandle.Complete();
                this.dependencies.Clear();

                Profiler.EndSample();
            }

            private void DestroyEntities(EntityManager entityManager)
            {
                Profiler.BeginSample("DestroyEntity");

                if (this.entities.IsCreated)
                {
                    entityManager.DestroyEntity(this.entities);
                    this.entities.Dispose();
                }

                Profiler.EndSample();
            }

            private int GetCount()
            {
                var sum = 0;
                foreach (var i in this.queues)
                {
                    sum += i.Count;
                }

                return sum;
            }

            [BurstCompile]
            private struct SetComponentDataJob : IJob
            {
                public int StartIndex;

                public NativeQueue<T> Queue;

                [ReadOnly]
                public NativeArray<ArchetypeChunk> Chunks;

                [NativeDisableContainerSafetyRestriction]
                public ArchetypeChunkComponentType<T> ComponentType;

                /// <inheritdoc />
                public void Execute()
                {
                    this.GetIndexes(out var chunkIndex, out var entityIndex);

                    for (; chunkIndex < this.Chunks.Length; chunkIndex++)
                    {
                        var chunk = this.Chunks[chunkIndex];

                        var components = chunk.GetNativeArray(this.ComponentType);

                        while (this.Queue.TryDequeue(out var item) && entityIndex < components.Length)
                        {
                            components[entityIndex++] = item;
                        }

                        if (this.Queue.Count == 0)
                        {
                            return;
                        }

                        entityIndex = entityIndex < components.Length ? entityIndex : 0;
                    }
                }

                private void GetIndexes(out int chunkIndex, out int entityIndex)
                {
                    var sum = 0;

                    for (chunkIndex = 0; chunkIndex < this.Chunks.Length; chunkIndex++)
                    {
                        var chunk = this.Chunks[chunkIndex];

                        var length = chunk.Count;

                        if (sum + length < this.StartIndex)
                        {
                            sum += length;
                            continue;
                        }

                        entityIndex = this.StartIndex - sum;
                        return;
                    }

                    throw new ArgumentOutOfRangeException(nameof(this.StartIndex));
                }
            }
        }
    }
}