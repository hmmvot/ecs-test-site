using EcsTestSite.Components;
using EcsTestSite.Config;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace EcsTestSite.Systems.Initialization
{
	[UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
	public partial struct SpawnCharacterSystem : ISystem
	{
		private EntityQuery _catalogQuery;
		private NativeHashMap<ulong, Entity> _catalog;

		public void OnCreate(ref SystemState state)
		{
			_catalogQuery = state.GetEntityQuery(ComponentType.ReadOnly<CatalogTag>());
		}

		public void OnDestroy(ref SystemState state)
		{
			_catalog.Dispose();
		}

		public void OnUpdate(ref SystemState state)
		{
			if (!_catalog.IsCreated)
			{
				_catalog = new NativeHashMap<ulong, Entity>(128, Allocator.Persistent);
			
				using var entities = _catalogQuery.ToEntityArray(Allocator.Temp);
				foreach (var e in entities)
				{
					var entries = state.EntityManager.GetBuffer<CharacterCatalogEntry>(e, true);
					for (int i = 0; i < entries.Length; i++)
					{
						_catalog.Add(entries[i].ID, entries[i].Def);
					}
				}
			}

			using var ecb = new EntityCommandBuffer(Allocator.Temp);
			foreach (var (spawner, spawnerEntity) in SystemAPI.Query<RefRO<SpawnCharacter>>().WithEntityAccess())
			{
				var defEntity = _catalog[spawner.ValueRO.DefID];
				var characterDef = state.EntityManager.GetComponentData<CharacterDef>(defEntity);
				var abilitiesDef = state.EntityManager.GetBuffer<CharacterAbilityRef>(defEntity);
				
				var character = ecb.CreateEntity();
				ecb.AddComponent(character, new CharacterTag());
				ecb.AddComponent(character, characterDef);
				ecb.AddComponent(character, new Health {Value = characterDef.Health});
				ecb.AddComponent(character, new MovementSpeed {Value = 10f});
				ecb.AddComponent(character, new PlayerTag());
				ecb.SetComponentEnabled<PlayerTag>(character, characterDef.ControlledByPlayer);

				var abilities = ecb.AddBuffer<CharacterAbilityRef>(character);
				foreach (var characterAbilityRef in abilitiesDef)
				{
					abilities.Add(characterAbilityRef);
				}
				
				ecb.AddComponent(character, LocalTransform.FromPositionRotation(spawner.ValueRO.Position, spawner.ValueRO.Rotation));
				ecb.AddComponent<LocalToWorld>(character);
				
				ecb.AddComponent<CleanupTag>(spawnerEntity);
			}
			
			ecb.Playback(state.EntityManager);
		}
	}
}