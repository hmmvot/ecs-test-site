using EcsTestSite.Components;
using Unity.Collections;
using Unity.Entities;

namespace EcsTestSite.Systems
{
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	[UpdateAfter(typeof(ProjectileHitSystem))]
	public partial struct AbilityProjectileAttackCompleteSystem : ISystem
	{
		public void OnUpdate(ref SystemState state)
		{
			using var ecb = new EntityCommandBuffer(Allocator.Temp);

			foreach (var (_, projectile, target, damage, entity)
			         in SystemAPI.Query<
					         RefRO<AbilityProjectileAttack>,
					         RefRO<AbilityProjectile>,
					         RefRO<AbilityTarget>,
							 RefRO<DamageDefinition>>()
				         .WithEntityAccess())
			{
				var projectileEntity = projectile.ValueRO.Value;
				if (!state.EntityManager.HasComponent<ProjectileHit>(projectileEntity))
					continue;
				
				var damageAmount = damage.ValueRO.Value;
				var targetEntity = target.ValueRO.Value;
				var incomingDamageBuffer = !state.EntityManager.HasBuffer<IncomingDamage>(targetEntity)
					? ecb.AddBuffer<IncomingDamage>(targetEntity)
					: state.EntityManager.GetBuffer<IncomingDamage>(targetEntity);
				incomingDamageBuffer.Add(new IncomingDamage {Value = damageAmount});
				ecb.AddComponent<CleanupTag>(entity);
			}

			ecb.Playback(state.EntityManager);
		}
	}
}