// <copyright file="DestroyEntityJob.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Jobs
{
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;

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