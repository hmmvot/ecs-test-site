using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace EcsTestSite.Utility
{
    public struct BakerWriter : IEntityWriter
    {
        private IBaker _baker;
        public Entity Entity { get; }

        public BakerWriter(IBaker baker, Entity e)
        {
            _baker = baker;
            Entity = e;
        }

        public void Add<T>(in T c) where T : unmanaged, IComponentData 
            => _baker.AddComponent(Entity, c);
        
        public void Add<T>() where T : unmanaged, IComponentData 
            => _baker.AddComponent<T>(Entity);
        
        public void Set<T>(in T c) where T : unmanaged, IComponentData 
            => _baker.SetComponent(Entity, c);

        public void Enable<T>(bool on = true) where T : unmanaged, IEnableableComponent
            => _baker.SetComponentEnabled<T>(Entity, on);

        public void AddBuffer<T>() where T : unmanaged, IBufferElementData 
            => _baker.AddBuffer<T>(Entity);

        public void AddTransform(float3 position, quaternion rotation = default)
        {
            // baker already did it
        }
    }

    public struct EcbWriter : IEntityWriter
    {
        private EntityCommandBuffer _ecb;
        public Entity Entity { get; }

        public EcbWriter(EntityCommandBuffer ecb, Entity e)
        {
            _ecb = ecb;
            Entity = e;
        }

        public void Add<T>(in T c) where T : unmanaged, IComponentData 
            => _ecb.AddComponent(Entity, c);
        
        public void Add<T>() where T : unmanaged, IComponentData 
            => _ecb.AddComponent<T>(Entity);
        
        public void Set<T>(in T c) where T : unmanaged, IComponentData 
            => _ecb.SetComponent(Entity, c);

        public void Enable<T>(bool on = true) where T : unmanaged, IEnableableComponent
            => _ecb.SetComponentEnabled<T>(Entity, on);

        public void AddBuffer<T>() where T : unmanaged, IBufferElementData 
            => _ecb.AddBuffer<T>(Entity);

        public void AddTransform(float3 position, quaternion rotation = default)
        {
            _ecb.AddComponent(Entity, LocalTransform.FromPositionRotation(position, rotation));
            _ecb.AddComponent<LocalToWorld>(Entity);
        }
    }

    public struct EntityManagerWriter : IEntityWriter
    {
        private EntityManager _entityManager;
        public Entity Entity { get; }

        public EntityManagerWriter(EntityManager entityManager, Entity e)
        {
            _entityManager = entityManager;
            Entity = e;
        }

        public void Add<T>(in T c) where T : unmanaged, IComponentData
            => _entityManager.AddComponentData(Entity, c);

        public void Add<T>() where T : unmanaged, IComponentData
            => _entityManager.AddComponent<T>(Entity);

        public void Set<T>(in T c) where T : unmanaged, IComponentData
            => _entityManager.SetComponentData(Entity, c);

        public void Enable<T>(bool on = true) where T : unmanaged, IEnableableComponent
            => _entityManager.SetComponentEnabled<T>(Entity, on);

        public void AddBuffer<T>() where T : unmanaged, IBufferElementData
            => _entityManager.AddBuffer<T>(Entity);

        public void AddTransform(float3 position, quaternion rotation = default)
        {
            _entityManager.AddComponentData(Entity, LocalTransform.FromPositionRotation(position, rotation));
            _entityManager.AddComponent<LocalToWorld>(Entity);
        }
    }
}