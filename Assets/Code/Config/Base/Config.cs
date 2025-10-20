using UnityEngine;

namespace EcsTestSite.Config
{
	public abstract class Config : ScriptableObject
	{
		public ulong StableID
			=> (ulong)GetInstanceID();
	}
}