using EcsTestSite.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace EcsTestSite.Utility
{
	public static class ArchetypesFactory
	{
		public static void BuildAbilityProjectileAttack<T>(T writer, Entity caster, Entity target) where T : IEntityWriter
		{
			writer.Add<AbilityTag>();
			writer.Add<AbilityProjectileAttack>();
			writer.Add(new DamageDefinition {Value = 5});
			writer.Add(new AbilityCaster {Value = caster});
			writer.Add(new AbilityTarget {Value = target});
		}
		
		public static void BuildProjectile<T>(T writer, float3 origin, float3 destination) where T : IEntityWriter
		{
			writer.Add<ProjectileTag>();
			writer.Add(new MovementSpeed {Value = 25});
			writer.Add(new MovementDestination {Value = destination});
			writer.AddTransform(origin);
		}
	}
}