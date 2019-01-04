# com.bovinelabs.entities

## Systems
* EntityEventSystem - System designed to easily and efficiently create simple events.

### How to use

Instead of using a EntityCommandBuffer, we get a NativeQueue<T> from CreateEventQueue where T is IComponentData, the event data. 
```c#
NativeQueue<T> EntityEventSystem.CreateEventQueue<T>(JobComponentSystem componentSystem)
```
It is safe to pass this queue to Jobs, using ToConcurrent() for parallal jobs. Once you have this queue, to create an event simply create, set and add a new type of T to the queue and the EntityEventSystem will handle the rest for you.

The EntityEventSystem will batch create create entities for all these components and then set the component data.

### Things to know
* Events live exactly 1 frame. They will be created in EntityEventSystem and 1 frame later destroyed in EntityEventSystem.
* Use of event will be delayed till the next frame. EntityEventSystem executes just before EndFrameBarrier.
* You can use CreateEventQueue for the same type from different systems or even multiple times from the same system.
  * In the case of the same system, it's slightly faster to reuse the same queue if systems have correct dependencies.
* The system calling CreateEventQueue passes a reference to itself to the EntityEventSystem and this is used to ensure dependencies are completed before EntityEventSystem is updated.

### CreateBufferEvent
```c#
void CreateBufferEvent<T, TB>(T component, NativeArray<TB> buffer)
```
Works similar to CreateEventQueue except it will add a component T and BufferArray<TB> to the entity.

## Containers
* NativeUnit<T> - Let's you effectively pass a 'reference' of a struct between jobs.

## Extensions
* EntityManager - SetComponentObject(Entity, ComponentType, object)
* World - AddManager for creating your own managers dynamically (dependency injection etc)
* List<T> - AddRange(NativeArray), AddRange(NativeList), AddRange(DynamicBuffer), AddRange(NativeSlice), AddRange(void*, int), Resize(int), EnsureLength(int, T)
* DynamicBuffer - Contains(T), IndexOf(T), Remove(T), Reverse(), ResizeInitialized(int), CopyTo(NativeArray<T>)
* NativeList - Reverse(), Insert(T, int), Remove(T), RemoveAt(int), RemoveRange(int, int)
