using UnityEngine;

namespace EcsTestSite.Presentation
{
	public static class Layers
	{
		public static int Ground { get; private set; }
		public static int Target { get; private set; }

		public static LayerMask GroundMask { get; private set; }
		public static LayerMask TargetMask { get; private set; }
		
		public static LayerMask ClickableMask { get; private set; }

		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			Ground = LayerMask.NameToLayer("Ground");
			Target = LayerMask.NameToLayer("Target");
			GroundMask = LayerMask.GetMask("Ground");
			TargetMask = LayerMask.GetMask("Target");
			ClickableMask = GroundMask | TargetMask;
		}
	}
}