using EcsTestSite.Components;
using EcsTestSite.Config;
using Unity.Entities;
using UnityEngine;

namespace EcsTestSite.Authoring
{
	public sealed class UnitAuthoring : MonoBehaviour
	{
		public CharacterConfig Config;

		private void OnDrawGizmos()
		{
			Gizmos.color = Config.ControlledByPlayer ? Color.green : Color.red;
			Gizmos.DrawWireSphere(transform.position + Vector3.up, 1f);
		}
	}
	
	public class UnitBaker : Baker<UnitAuthoring>
	{
		public override void Bake(UnitAuthoring a)
		{
			var entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new SpawnCharacter
			{
				DefID = a.Config.StableID,
				Position = a.transform.position,
				Rotation = a.transform.rotation,
			});

			DependsOn(a.Config);
		}
	}
}