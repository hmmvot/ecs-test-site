using Unity.Entities;

namespace EcsTestSite.Systems.Groups
{
	[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
	public sealed partial class CleanupSystemGroup : ComponentSystemGroup
	{
		
	}
}