using Unity.Entities;

namespace EcsTestSite.Systems.Groups
{
	[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
	public sealed partial class InputSystemGroup : ComponentSystemGroup
	{
	}
}