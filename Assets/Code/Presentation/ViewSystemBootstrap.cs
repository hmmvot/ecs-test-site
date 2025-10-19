using EcsTestSite.Systems;
using Unity.Entities;
using UnityEngine;

namespace EcsTestSite.Presentation
{
	public sealed class ViewSystemBootstrap : MonoBehaviour
	{
		public UnitView PlayerUnitPrefab;
		public UnitView EnemyUnitPrefab;
		public ProjectileView ProjectilePrefab;

		private void Awake()
		{
			foreach (var w in World.All)
			{
				var system = w.GetExistingSystemManaged<ViewSystem>();
				if (system == null)
					continue;
				
				system.PlayerUnitPrefab = PlayerUnitPrefab;
				system.EnemyUnitPrefab = EnemyUnitPrefab;
				system.ProjectilePrefab = ProjectilePrefab;
			}
		}
	}
}