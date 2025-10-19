using System;
using EcsTestSite.Components;
using Unity.Collections;
using Unity.Entities;

namespace EcsTestSite.Systems
{
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	[UpdateAfter(typeof(AbilityProjectileAttackCompleteSystem))]
	public partial struct DamageResolveSystem : ISystem
	{
		private BufferLookup<IncomingDamage> _incomingDamageLookup;
		
		public void OnCreate(ref SystemState state)
		{
			_incomingDamageLookup = state.GetBufferLookup<IncomingDamage>();
		}

		public void OnUpdate(ref SystemState state)
		{
			_incomingDamageLookup.Update(ref state);
			
			using var ecb = new EntityCommandBuffer(Allocator.Temp);
			foreach (var (health, entity) in SystemAPI
				         .Query<RefRW<Health>>()
				         .WithAll<IncomingDamage>()
				         .WithEntityAccess())
			{
				int currentDamage = 0;
				foreach (var incomingDamage in _incomingDamageLookup[entity])
					currentDamage += incomingDamage.Value;

				health.ValueRW.Value = Math.Max(0, health.ValueRO.Value - currentDamage);
				ecb.RemoveComponent<IncomingDamage>(entity);
				
				if (health.ValueRW.Value <= 0)
					ecb.AddComponent<DeadTag>(entity);
			}
			
			ecb.Playback(state.EntityManager);
		}
	}
}