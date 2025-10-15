using EcsTestSite.Components;
using EcsTestSite.Systems.Groups;
using Unity.Collections;
using Unity.Entities;

namespace EcsTestSite.Systems
{
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	[UpdateAfter(typeof(MovementSystem))]
	public partial struct ProjectileHitSystem : ISystem
	{
		public void OnUpdate(ref SystemState state)
		{
			using var ecb = new EntityCommandBuffer(Allocator.Temp);

			foreach (var (_, entity)
			         in SystemAPI.Query<RefRO<ProjectileTag>>()
				         .WithNone<MovementDestination, ProjectileHit>()
				         .WithEntityAccess())
			{
				ecb.AddComponent<ProjectileHit>(entity);
				ecb.AddComponent<CleanupTag>(entity);
			}

			ecb.Playback(state.EntityManager);
		}
	}
}