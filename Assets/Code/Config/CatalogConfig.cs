using System.Linq;
using EcsTestSite.Utility;
using Unity.Entities;
using UnityEngine;

namespace EcsTestSite.Config
{
	[CreateAssetMenu(menuName = CreateAssetMenuPaths.CatalogConfig)]
	public sealed class CatalogConfig : Config
	{
		public CharacterConfig[] Characters = {};
		public AbilityConfig[] Abilities = {};
		
		public CharacterConfig GetCharacter(ulong stableID)
			=> Characters.FirstOrDefault(i => i.StableID == stableID);
		
		public AbilityConfig GetAbility(ulong stableID)
			=> Abilities.FirstOrDefault(i => i.StableID == stableID);
	}

	public struct CatalogTag : IComponentData
	{
	}
}