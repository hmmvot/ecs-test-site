using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace EcsTestSite.View
{
	public abstract class EntityView : MonoBehaviour
	{
		public World World { get; private set; }

		public Entity Entity { get; private set; }
		
		public EntityManager EntityManager
			=> World.EntityManager;

		public void Bind(Entity entity, World world)
		{
			if (World != null)
				throw new InvalidOperationException("EntityView is already bound");
			
			Entity = entity;
			World = world;
		}
		
		private void SyncTransform(Entity entity, EntityView view)
		{
			var localToWorld = EntityManager.GetComponentData<LocalToWorld>(entity);
			var t = view.transform;
			t.position = localToWorld.Position;
			t.rotation = localToWorld.Rotation;
		}

		private void LateUpdate()
		{
			if (!EntityManager.Exists(Entity))
				return;
			
			SyncTransform(Entity, this);
			OnLateUpdate();
		}
		
		protected abstract void OnLateUpdate();
	}
}