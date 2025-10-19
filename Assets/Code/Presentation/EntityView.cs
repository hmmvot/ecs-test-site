using System;
using Unity.Entities;
using UnityEngine;

namespace EcsTestSite.Presentation
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
			
			OnInitialize();
		}

		public abstract void OnInitialize();
		public abstract void OnUpdate();
	}
}