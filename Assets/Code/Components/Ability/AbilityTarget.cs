using Unity.Entities;

namespace EcsTestSite.Components
{
	public struct AbilityTarget : IComponentData
	{
		public Entity Value;
	}
}