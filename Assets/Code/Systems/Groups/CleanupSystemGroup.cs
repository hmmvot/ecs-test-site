using Unity.Entities;

namespace EcsTestSite.Systems.Groups
{
	[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
	public sealed partial class CleanupSystemGroup : ComponentSystemGroup
	{
	}
}