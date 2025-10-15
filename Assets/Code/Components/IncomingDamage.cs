using Unity.Entities;

namespace EcsTestSite.Components
{
	[InternalBufferCapacity(8)]
	public struct IncomingDamage : IBufferElementData
	{
		public int Value;
	}
}