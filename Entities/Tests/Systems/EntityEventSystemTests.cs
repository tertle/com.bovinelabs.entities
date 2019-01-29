// <copyright file="EntityEventSystemTests.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Tests.Systems
{
    using BovineLabs.Entities.Systems;
    using NUnit.Framework;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Entities.Tests;

    /// <summary>
    /// The eventSystemTests.
    /// </summary>
    public class EntityEventSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// Test that only a single use of <see cref="EntityEventSystem.CreateEventQueue{T}"/> works correctly.
        /// </summary>
        [Test]
        public void CreateEventQueueSingleQueue()
        {
            var eventSystem = this.World.CreateManager<EntityEventSystem>();

            var queue = eventSystem.CreateEventQueue<TestData>(this.EmptySystem);

            var data0 = new TestData { Value = 0 };
            var data1 = new TestData { Value = 1 };
            var data2 = new TestData { Value = 2 };

            queue.Enqueue(data0);
            queue.Enqueue(data1);
            queue.Enqueue(data2);

            eventSystem.Update();

            var group = this.m_Manager.CreateComponentGroup(typeof(TestData));
            Assert.AreEqual(3, group.CalculateLength());

            var chunks = group.CreateArchetypeChunkArray(Allocator.TempJob);

            int count = 0;

            var testDataType = this.m_Manager.GetArchetypeChunkComponentType<TestData>(true);

            for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
            {
                var chunk = chunks[chunkIndex];
                var data = chunk.GetNativeArray(testDataType);

                for (var index = 0; index < chunk.Count; index++)
                {
                    Assert.AreEqual(count++, data[index].Value);
                }
            }

            chunks.Dispose();
        }

        /// <summary>
        /// Test that <see cref="EntityEventSystem.CreateEventQueue{T}"/> returns a different queue each time.
        /// </summary>
        [Test]
        public void CreateEventQueueReturnsDifferentQueues()
        {
            var eventSystem = this.World.CreateManager<EntityEventSystem>();

            Assert.AreNotEqual(
                eventSystem.CreateEventQueue<TestData>(this.EmptySystem),
                eventSystem.CreateEventQueue<TestData>(this.EmptySystem));
        }

        /// <summary>
        /// Test that multiple calls of <see cref="EntityEventSystem.CreateEventQueue{T}"/> works correctly.
        /// </summary>
        [Test]
        public void CreateEventQueueMultipleQueued()
        {
            var eventSystem = this.World.CreateManager<EntityEventSystem>();

            var queue1 = eventSystem.CreateEventQueue<TestData>(this.EmptySystem);
            var queue2 = eventSystem.CreateEventQueue<TestData>(this.EmptySystem);

            var data0 = new TestData { Value = 0 };
            var data1 = new TestData { Value = 1 };
            var data2 = new TestData { Value = 2 };

            queue1.Enqueue(data0);
            queue1.Enqueue(data1);
            queue2.Enqueue(data2);

            eventSystem.Update();

            var group = this.m_Manager.CreateComponentGroup(typeof(TestData));
            Assert.AreEqual(3, group.CalculateLength());

            var chunks = group.CreateArchetypeChunkArray(Allocator.TempJob);

            int count = 0;

            var testDataType = this.m_Manager.GetArchetypeChunkComponentType<TestData>(true);

            for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
            {
                var chunk = chunks[chunkIndex];
                var data = chunk.GetNativeArray(testDataType);

                for (var index = 0; index < chunk.Count; index++)
                {
                    var value = data[index].Value;
                    Assert.AreEqual(count++, value);
                }
            }

            chunks.Dispose();
        }

        /// <summary>
        /// Test that multiple calls of <see cref="EntityEventSystem.CreateEventQueue{T}"/> with different types works correctly.
        /// </summary>
        [Test]
        public void CreateEventQueueMultipleTypesQueued()
        {
            var eventSystem = this.World.CreateManager<EntityEventSystem>();

            var queue = eventSystem.CreateEventQueue<TestData>(this.EmptySystem);
            var queue2 = eventSystem.CreateEventQueue<TestData2>(this.EmptySystem);

            var data0 = new TestData { Value = 0 };
            var data1 = new TestData { Value = 1 };
            var data2 = new TestData { Value = 2 };

            var data20 = new TestData2 { Value = 0 };
            var data21 = new TestData2 { Value = 1 };
            var data22 = new TestData2 { Value = 2 };

            queue.Enqueue(data0);
            queue.Enqueue(data1);
            queue.Enqueue(data2);

            queue2.Enqueue(data20);
            queue2.Enqueue(data21);
            queue2.Enqueue(data22);

            eventSystem.Update();

            var group = this.m_Manager.CreateComponentGroup(typeof(TestData));
            Assert.AreEqual(3, group.CalculateLength());

            var group2 = this.m_Manager.CreateComponentGroup(typeof(TestData2));
            Assert.AreEqual(3, group2.CalculateLength());

            var chunks = group.CreateArchetypeChunkArray(Allocator.TempJob);

            int count = 0;

            var testDataType = this.m_Manager.GetArchetypeChunkComponentType<TestData>(true);

            for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
            {
                var chunk = chunks[chunkIndex];
                var data = chunk.GetNativeArray(testDataType);

                for (var index = 0; index < chunk.Count; index++)
                {
                    Assert.AreEqual(count++, data[index].Value);
                }
            }

            chunks.Dispose();

            var chunks2 = group2.CreateArchetypeChunkArray(Allocator.TempJob);

            int count2 = 0;

            var testData2Type = this.m_Manager.GetArchetypeChunkComponentType<TestData2>(true);

            for (var chunkIndex = 0; chunkIndex < chunks2.Length; chunkIndex++)
            {
                var chunk = chunks2[chunkIndex];
                var data = chunk.GetNativeArray(testData2Type);

                for (var index = 0; index < chunk.Count; index++)
                {
                    Assert.AreEqual(count2++, data[index].Value);
                }
            }

            chunks2.Dispose();
        }

        /// <summary>
        /// Tests that the <see cref="NativeArray{T}"/> passed to <see cref="EntityEventSystem.CreateBufferEvent{T,TD}"/>
        /// is deallocated after use.
        /// </summary>
        [Test]
        public void CreateBufferEventDeallocatesArray()
        {
            var eventSystem = this.World.CreateManager<EntityEventSystem>();
            var array = new NativeArray<BufferData>(1, Allocator.TempJob);

            eventSystem.CreateBufferEvent(new TestData { Value = 1 }, array);

            eventSystem.Update();

            // Doesn't work as a dispose check as of 0.0.12p21
            // Assert.IsFalse(array.IsCreated);
            Assert.Throws<System.InvalidOperationException>(() =>
            {
                // ReSharper disable once UnusedVariable
                var l = array[0];
            });
        }

        /// <summary>
        /// Tests that <see cref="EntityEventSystem.CreateBufferEvent{T,TD}"/> creates the event.
        /// </summary>
        [Test]
        public void CreateBufferEventCreates()
        {
            const int bufferLength = 10;

            var eventSystem = this.World.CreateManager<EntityEventSystem>();
            var array = new NativeArray<BufferData>(bufferLength, Allocator.TempJob);
            var array1 = new NativeArray<BufferData>(bufferLength, Allocator.TempJob);

            for (var index = 0; index < bufferLength; index++)
            {
                array[index] = new BufferData { Value = index };
                array1[index] = new BufferData { Value = -index };
            }

            eventSystem.CreateBufferEvent(new TestData { Value = 0 }, array);
            eventSystem.CreateBufferEvent(new TestData { Value = 1 }, array1);

            eventSystem.Update();

            var group = this.m_Manager.CreateComponentGroup(typeof(TestData), typeof(BufferData));
            Assert.AreEqual(2, group.CalculateLength());

            var chunks = group.CreateArchetypeChunkArray(Allocator.TempJob);

            int count = 0;

            var testDataType = this.m_Manager.GetArchetypeChunkComponentType<TestData>(true);
            var bufferDataType = this.m_Manager.GetArchetypeChunkBufferType<BufferData>(true);

            for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
            {
                var chunk = chunks[chunkIndex];
                var data = chunk.GetNativeArray(testDataType);
                var bufferAccessor = chunk.GetBufferAccessor(bufferDataType);

                for (var index = 0; index < chunk.Count; index++)
                {
                    var value = data[index].Value;
                    var buffer = bufferAccessor[index];

                    Assert.AreEqual(count++, value);

                    for (var i = 0; i < bufferLength; i++)
                    {
                        Assert.AreEqual((index == 0 ? 1 : -1) * i, buffer[i].Value);
                    }
                }
            }

            chunks.Dispose();
        }

        private struct TestData : IComponentData
        {
            public int Value;
        }

        private struct TestData2 : IComponentData
        {
            public int Value;
        }

        private struct BufferData : IBufferElementData
        {
            public int Value;
        }
    }
}