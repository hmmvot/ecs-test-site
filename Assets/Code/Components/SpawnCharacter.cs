using Unity.Entities;
using Unity.Mathematics;

namespace EcsTestSite.Components
{
	public struct SpawnCharacter : IComponentData
	{
		public ulong DefID;
		public float3 Position;
		public quaternion Rotation;
	}
}