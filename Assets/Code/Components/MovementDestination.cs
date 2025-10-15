using Unity.Entities;
using Unity.Mathematics;

namespace EcsTestSite.Components
{
	public struct MovementDestination : IComponentData
	{
		public float3 Value;
	}
}