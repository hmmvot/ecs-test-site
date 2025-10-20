using System.Collections.Generic;
using EcsTestSite.Components;
using EcsTestSite.Config;
using EcsTestSite.Presentation;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace EcsTestSite.Systems
{
	[UpdateInGroup(typeof(PresentationSystemGroup))]
	public partial class ViewSystem : SystemBase
	{
		private EntityQuery _unitQuery;
		private EntityQuery _projectileQuery;
		
		private readonly Dictionary<Entity, EntityView> _entityViews = new();
		
		public CatalogConfig Catalog { get; set; }
		
		public ProjectileView ProjectilePrefab { get; set; }
		
		protected override void OnCreate()
		{
			_unitQuery = EntityManager.CreateEntityQuery(new EntityQueryDesc
			{
				All = new ComponentType[]
				{
					typeof(CharacterTag),
					typeof(CharacterDef),
					typeof(LocalToWorld),
				},
			});
			_projectileQuery = EntityManager.CreateEntityQuery(new EntityQueryDesc
			{
				All = new ComponentType[]
				{
					typeof(ProjectileTag),
					typeof(LocalToWorld),
				}
			});
		}

		protected override void OnDestroy()
		{
			foreach (var (_, view) in _entityViews)
			{
				if (view)
					Object.Destroy(view.gameObject);
			}

			_entityViews.Clear();
			_unitQuery.Dispose();
			_projectileQuery.Dispose();
		}

		protected override void OnUpdate()
		{
			using var units = _unitQuery.ToEntityArray(Allocator.Temp);
			foreach (var entity in units)
			{
				if (!_entityViews.ContainsKey(entity))
				{
					var config = Catalog.GetCharacter(EntityManager.GetComponentData<CharacterDef>(entity).ID);
					InstantiateView(entity, config.Prefab);
				}
			}
			
			using var projectiles = _projectileQuery.ToEntityArray(Allocator.Temp);
			foreach (var entity in projectiles)
			{
				if (!_entityViews.ContainsKey(entity))
					InstantiateView(entity, ProjectilePrefab);
			}

			using var _ = ListPool<Entity>.Get(out var toRemove);
			foreach (var (entity, view) in _entityViews)
			{
				if (!EntityManager.Exists(entity))
				{
					Object.Destroy(view.gameObject);
					toRemove.Add(entity);
					continue;
				}

				SyncTransform(entity, view);
				view.OnUpdate();
			}

			foreach (var e in toRemove)
				_entityViews.Remove(e);
		}

		private void InstantiateView(Entity entity, EntityView prefab)
		{
			var view = Object.Instantiate(prefab);
			view.Bind(entity, World);
			_entityViews[entity] = view;
			view.OnInitialize();
		}
		
		private void SyncTransform(Entity entity, EntityView view)
		{
			var localToWorld = EntityManager.GetComponentData<LocalToWorld>(entity);
			var t = view.transform;
			t.position = localToWorld.Position;
			t.rotation = localToWorld.Rotation;
		}
	}
}