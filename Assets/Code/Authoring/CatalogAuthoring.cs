using System.Collections.Generic;
using EcsTestSite.Config;
using Unity.Entities;
using UnityEngine;

namespace EcsTestSite.Authoring
{
	public sealed class CatalogAuthoring : MonoBehaviour
	{
		public CatalogConfig Catalog;
	}
	
	public class ConfigBaker : Baker<CatalogAuthoring>
	{
		private Dictionary<AbilityConfig, Entity> _abilityMap;
		private Dictionary<CharacterConfig, Entity> _characterMap;
		private DynamicBuffer<AbilityCatalogEntry> _abilityRegistry;
		private DynamicBuffer<CharacterCatalogEntry> _characterRegistry;
		
		public override void Bake(CatalogAuthoring authoring)
		{
			try
			{
				var catalog = GetEntity(TransformUsageFlags.None);
				AddComponent<CatalogTag>(catalog);

				_abilityRegistry = AddBuffer<AbilityCatalogEntry>(catalog);
				_characterRegistry = AddBuffer<CharacterCatalogEntry>(catalog);

				_abilityMap = new Dictionary<AbilityConfig, Entity>(128);
				_characterMap = new Dictionary<CharacterConfig, Entity>(128);

				foreach (var ability in authoring.Catalog.Abilities)
				{
					EnsureAbility(ability);
					DependsOn(ability);
				}

				foreach (var character in authoring.Catalog.Characters)
				{
					EnsureCharacter(character);
					DependsOn(character);
				}
			}
			finally
			{
				_abilityMap = null;
				_characterMap = null;
				_abilityRegistry = default;
				_characterRegistry = default;
			}
		}

		private Entity EnsureAbility(AbilityConfig so)
		{
			if (_abilityMap.TryGetValue(so, out var existing))
				return existing;

			var def = CreateAdditionalEntity(TransformUsageFlags.None);

			AddComponent(def, new AbilityDef
			{
				ID = so.StableID,
				Damage = so.Damage,
				Range = so.Range,
			});

			_abilityRegistry.Add(new AbilityCatalogEntry {ID = so.StableID, Def = def});

			_abilityMap[so] = def;
			return def;
		}

		private Entity EnsureCharacter(CharacterConfig so)
		{
			if (_characterMap.TryGetValue(so, out var existing))
				return existing;
			
			var def = CreateAdditionalEntity(TransformUsageFlags.None);

			AddComponent(def, new CharacterDef
			{
				ID = so.StableID,
				ControlledByPlayer = so.ControlledByPlayer,
				Health = so.Health
			});

			var buf = AddBuffer<CharacterAbilityRef>(def);
			foreach (var ability in so.Abilities)
			{
				var abilityDef = EnsureAbility(ability);
				buf.Add(new CharacterAbilityRef {Def = abilityDef});
				DependsOn(ability);
			}

			_characterRegistry.Add(new CharacterCatalogEntry {ID = so.StableID, Def = def});
			
			_characterMap[so] = def;
			return def;
		}
	}
}