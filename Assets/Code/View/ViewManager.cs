using System.Collections.Generic;
using EcsTestSite.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Pool;

namespace EcsTestSite.View
{
	public sealed class ViewManager : MonoBehaviour
	{
		public UnitView PlayerUnitPrefab;
		public UnitView EnemyUnitPrefab;
		public ProjectileView ProjectilePrefab;
		
		private EntityManager _entityManager;
		private EntityQuery _unitQuery;
		private EntityQuery _projectileQuery;
		private readonly Dictionary<Entity, EntityView> _entityViews = new();

		private void Start()
		{
			_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			_unitQuery = _entityManager.CreateEntityQuery(new EntityQueryDesc
			{
				All = new ComponentType[]
				{
					typeof(UnitTag),
					typeof(LocalToWorld),
					typeof(PlayerTag)
				},
				Options = EntityQueryOptions.IgnoreComponentEnabledState
			});
			_projectileQuery = _entityManager.CreateEntityQuery(new EntityQueryDesc
			{
				All = new ComponentType[]
				{
					typeof(ProjectileTag),
					typeof(LocalToWorld),
				}
			});
		}

		private void Update()
		{
			using var units = _unitQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
			foreach (var entity in units)
			{
				if (!_entityViews.ContainsKey(entity))
				{
					var prefab = _entityManager.IsComponentEnabled<PlayerTag>(entity)
						? PlayerUnitPrefab
						: EnemyUnitPrefab;
					InstantiateView(entity, prefab);
				}
			}
			
			using var projectiles = _projectileQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
			foreach (var entity in projectiles)
			{
				if (!_entityViews.ContainsKey(entity))
					InstantiateView(entity, ProjectilePrefab);
			}

			using var _ = ListPool<Entity>.Get(out var toRemove);
			foreach (var kv in _entityViews)
			{
				if (_entityManager.Exists(kv.Key))
					continue;
				
				Destroy(kv.Value.gameObject);
				toRemove.Add(kv.Key);
			}

			foreach (var e in toRemove)
				_entityViews.Remove(e);
		}

		private void InstantiateView(Entity entity, EntityView prefab)
		{
			var view = Instantiate(prefab);
			view.Bind(entity, World.DefaultGameObjectInjectionWorld);
			_entityViews[entity] = view;
		}
	}
}