using EcsTestSite.Presentation;
using EcsTestSite.Utility;
using Unity.Entities;
using UnityEngine;

namespace EcsTestSite.Config
{
	[CreateAssetMenu(menuName = CreateAssetMenuPaths.CharacterConfig)]
	public sealed class CharacterConfig : Config
	{
		public bool ControlledByPlayer;
		public int Health = 10;
		public AbilityConfig[] Abilities = {};

		public CharacterView Prefab;
	}

	public struct CharacterDef : IComponentData
	{
		public ulong ID;
		public bool ControlledByPlayer;
		public int Health;
	}

	[InternalBufferCapacity(8)]
	public struct CharacterAbilityRef : IBufferElementData
	{
		public Entity Def;
	}
	
	public struct CharacterCatalogEntry : IBufferElementData
	{
		public ulong ID;
		public Entity Def;
	}
}