using EcsTestSite.Utility;
using Unity.Entities;
using UnityEngine;

namespace EcsTestSite.Authoring
{
	public sealed class UnitAuthoring : MonoBehaviour
	{
		public UnitType Type;

		private void OnDrawGizmos()
		{
			Gizmos.color = Type == UnitType.Player ? Color.green : Color.red;
			Gizmos.DrawWireSphere(transform.position + Vector3.up, 1f);
		}
	}
	
	public enum UnitType
	{
		Player = 0,
		Enemy = 1,
	}
	
	public class UnitBaker : Baker<UnitAuthoring>
	{
		public override void Bake(UnitAuthoring a)
		{
			ArchetypesFactory.BuildUnit(new BakerWriter(this, GetEntity(TransformUsageFlags.Dynamic)), a.Type);
		}
	}
}