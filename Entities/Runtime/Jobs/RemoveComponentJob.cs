// <copyright file="RemoveComponentJob.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Jobs
{
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;

    /// <summary>
    /// The RemoveComponentJob.
    /// </summary>
    /// <typeparam name="T">The component type to remove.</typeparam>
    public struct RemoveComponentJob<T> : IJob
        where T : struct, IComponentData
    {
        /// <summary>
        /// The <see cref="EntityCommandBuffer"/>.
        /// </summary>
        public EntityCommandBuffer CommandBuffer;

        /// <summary>
        /// Queue of entities to remove a component from.
        /// </summary>
        public NativeQueue<Entity> Queue;

        /// <inheritdoc />
        public void Execute()
        {
            while (this.Queue.TryDequeue(out var entity))
            {
                this.CommandBuffer.RemoveComponent<T>(entity);
            }
        }
    }
}