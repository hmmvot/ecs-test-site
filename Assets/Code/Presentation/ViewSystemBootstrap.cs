using EcsTestSite.Config;
using EcsTestSite.Systems;
using Unity.Entities;
using UnityEngine;

namespace EcsTestSite.Presentation
{
	public sealed class ViewSystemBootstrap : MonoBehaviour
	{
		public CatalogConfig Catalog;
		public ProjectileView ProjectilePrefab;

		private void Awake()
		{
			foreach (var w in World.All)
			{
				var system = w.GetExistingSystemManaged<ViewSystem>();
				if (system == null)
					continue;
				
				system.Catalog = Catalog;
				system.ProjectilePrefab = ProjectilePrefab;
			}
		}
	}
}