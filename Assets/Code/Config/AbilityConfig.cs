using EcsTestSite.Presentation;
using EcsTestSite.Utility;
using Unity.Entities;
using UnityEngine;

namespace EcsTestSite.Config
{
	[CreateAssetMenu(menuName = CreateAssetMenuPaths.AbilityConfig)]
	public sealed class AbilityConfig : Config
	{
		public int Damage;
		public int Range;
		
		public ProjectileView ProjectilePrefab;
	}
	
	public struct AbilityDef : IComponentData
	{
		public ulong ID;
		public int Damage;
		public int Range;
	}

	public struct AbilityCatalogEntry : IBufferElementData
	{
		public ulong ID;
		public Entity Def;
	}
}