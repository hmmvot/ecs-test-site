using EcsTestSite.Components;
using EcsTestSite.Systems.Groups;
using Unity.Collections;
using Unity.Entities;

namespace EcsTestSite.Systems
{
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	public partial struct CleanupSystem : ISystem
	{
		public void OnUpdate(ref SystemState state)
		{
			using var ecb = new EntityCommandBuffer(Allocator.Temp);

			foreach (var (_, entity) in SystemAPI.Query<RefRO<CleanupTag>>().WithEntityAccess())
				ecb.DestroyEntity(entity);

			ecb.Playback(state.EntityManager);
		}
	}
}