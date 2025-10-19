using EcsTestSite.Presentation;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using InputSystem = EcsTestSite.Systems.InputSystem;

namespace EcsTestSite.Input
{
	public sealed class InputSystemBootstrap : MonoBehaviour
	{
		private void Awake()
		{
			foreach (var w in World.All)
			{
				var system = w.GetExistingSystemManaged<InputSystem>();
				if (system == null)
					continue;
				
				system.Keyboard = Keyboard.current;
				system.Mouse = Mouse.current;
				system.Camera = FindAnyObjectByType<CameraRig>()?.Camera;
			}
		}
	}
}