using Unity.Entities;
using Unity.Mathematics;

namespace EcsTestSite.Utility
{
	public interface IEntityWriter
	{
		Entity Entity { get; }
		void Add<T>(in T c) where T : unmanaged, IComponentData;
		void Add<T>() where T : unmanaged, IComponentData;
		void Set<T>(in T c) where T : unmanaged, IComponentData;
		void Enable<T>(bool on = true) where T : unmanaged, IEnableableComponent;
		void AddBuffer<T>() where T : unmanaged, IBufferElementData;
		void AddTransform(float3 position, quaternion rotation = default);
	}
}