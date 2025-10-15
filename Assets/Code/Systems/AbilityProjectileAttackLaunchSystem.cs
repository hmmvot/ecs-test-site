using EcsTestSite.Components;
using EcsTestSite.Systems.Groups;
using EcsTestSite.Utility;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace EcsTestSite.Systems
{
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	[UpdateBefore(typeof(MovementSystem))]
	public partial struct AbilityProjectileAttackLaunchSystem : ISystem
	{
		public void OnUpdate(ref SystemState state)
		{
			using var ecb = new EntityCommandBuffer(Allocator.Temp);

			foreach (var (_, caster, target, entity)
			         in SystemAPI.Query<
					         RefRO<AbilityProjectileAttack>,
					         RefRO<AbilityCaster>,
					         RefRO<AbilityTarget>>()
				         .WithNone<AbilityProjectile>()
				         .WithEntityAccess())
			{
				var casterPosition = state.EntityManager.GetComponentData<LocalTransform>(caster.ValueRO.Value).Position;
				var targetPosition = state.EntityManager.GetComponentData<LocalTransform>(target.ValueRO.Value).Position;
				var projectile = ecb.CreateEntity();
				ArchetypesFactory.BuildProjectile(new EcbWriter(ecb, projectile), casterPosition, targetPosition);
				
				ecb.AddComponent(entity, new AbilityProjectile {Value = projectile});
			}
			
			ecb.Playback(state.EntityManager);
		}
	}
}