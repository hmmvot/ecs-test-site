using EcsTestSite.Authoring;
using EcsTestSite.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace EcsTestSite.Utility
{
	public static class ArchetypesFactory
	{
		public static void BuildUnit<T>(T writer, UnitType type, float3 position = default, quaternion rotation = default) where T : IEntityWriter
		{			
			writer.Add(new UnitTag());
			writer.Add(new Health {Value = 10});
			writer.Add(new MovementSpeed {Value = 10f});
			writer.Add(new PlayerTag());
			writer.Enable<PlayerTag>(type == UnitType.Player);
			writer.AddTransform(position, rotation);
		}
		
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