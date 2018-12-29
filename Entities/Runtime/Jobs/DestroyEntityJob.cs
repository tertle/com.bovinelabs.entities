// <copyright file="DestroyEntityJob.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Common.Jobs
{
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;

    // TODO REMOVE BOTH

    /// <summary>
    /// Destroy an entity that has a component.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    [BurstCompile]
    public struct DestroyEntityJob<T> : IJobProcessComponentDataWithEntity<T>
        where T : struct, IComponentData
    {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref T data)
        {
            this.EntityCommandBuffer.DestroyEntity(index, entity);
        }
    }

    [BurstCompile]
    public struct DestroyEntityJob : IJobChunk
    {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;

        [ReadOnly]
        public ArchetypeChunkEntityType EntityType;

        /// <inheritdoc />
        public void Execute(ArchetypeChunk chunk, int chunkIndex)
        {
            var entities = chunk.GetNativeArray(this.EntityType);

            for (var index = 0; index < chunk.Count; index++)
            {
                this.EntityCommandBuffer.DestroyEntity(chunkIndex, entities[index]);
            }
        }
    }
}