using EcsTestSite.Components;
using EcsTestSite.Systems.Groups;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace EcsTestSite.Systems
{
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	public partial struct MovementSystem : ISystem
	{
		public void OnUpdate(ref SystemState state)
		{
			using var ecb = new EntityCommandBuffer(Allocator.Temp);

			float dt = SystemAPI.Time.DeltaTime;
			foreach (var (speed, destination, transform, entity)
			         in SystemAPI.Query<RefRO<MovementSpeed>, RefRO<MovementDestination>, RefRW<LocalTransform>>()
				         .WithEntityAccess())
			{
				float3 pos = transform.ValueRO.Position;
				float3 dst = destination.ValueRO.Value;
				float3 to  = dst - pos;
				float  dist = math.length(to);

				float step = speed.ValueRO.Value * dt;

				if (dist <= step + 1e-5f)
				{
					transform.ValueRW.Position = dst;
					ecb.RemoveComponent<MovementDestination>(entity);
					continue;
				}

				float3 dir = dist > 1e-6f ? to / dist : float3.zero;
				transform.ValueRW.Position = pos + dir * step;
			}

			ecb.Playback(state.EntityManager);
		}
	}
}