using EcsTestSite.Components;
using EcsTestSite.Utility;
using EcsTestSite.View.Utility;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EcsTestSite.View
{
	public sealed class InputManager : MonoBehaviour
	{
		private const float RAYCAST_HEIGHT = 100f;
		
		private static readonly RaycastHit[] _raycastHitsBuffer = new RaycastHit[64];
		
		private EntityManager _entityManager;
		private EntityQuery _playerQuery;
		
		private Keyboard _keyboard;
		private Mouse _mouse;
		private Camera _camera;
		
		private UnitView _unitUnderMouse;
		private Vector3? _positionUnderMouse;

		private void Start()
		{
			_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			_playerQuery = _entityManager.CreateEntityQuery(new EntityQueryDesc
			{
				All = new ComponentType[] {typeof(UnitTag), typeof(PlayerTag)},
			});
			
			_keyboard = Keyboard.current;
			_mouse = Mouse.current;
			_camera = FindAnyObjectByType<CameraRig>()?.Camera;
		}

		private void Update()
		{
			int count = RaycastFromMouse(_camera, Layers.ClickableMask, _raycastHitsBuffer);
			
			float closestTargetDistance = float.MaxValue;
			GameObject closestTarget = null;
			float closestGroundDistance = float.MaxValue;
			var closesGroundPoint = default(Vector3?);
			for (int i = 0; i < count; i++)
			{
				var hit = _raycastHitsBuffer[i];
				if (hit.collider.gameObject.layer == Layers.Target)
				{
					if (hit.distance < closestTargetDistance)
					{
						closestTarget = hit.collider.gameObject;
						closestTargetDistance = hit.distance;
					}
				}
				else if (hit.collider.gameObject.layer == Layers.Ground)
				{
					if (hit.distance < closestGroundDistance)
					{
						closesGroundPoint = hit.point;
						closestGroundDistance = hit.distance;
					}
				}
			}

			GameObject finalTarget;
			Vector3? finalGroundPoint;
			if (closestTarget != null && closesGroundPoint != null)
			{
				if (closestTargetDistance < closestGroundDistance)
				{
					finalTarget = closestTarget;
					finalGroundPoint = null;
				}
				else
				{
					finalTarget = null;
					finalGroundPoint = closesGroundPoint;
				}
			}
			else
			{
				finalTarget = closestTarget;
				finalGroundPoint = closesGroundPoint;
			}
			
			_unitUnderMouse = finalTarget?.GetComponentInParent<UnitView>();
			_positionUnderMouse = finalGroundPoint;

			if (_mouse.leftButton.wasPressedThisFrame)
			{
				if (_unitUnderMouse != null)
					HandleTargetClick(_unitUnderMouse);
				else if (_positionUnderMouse != null)
					HandleGroundClick(_positionUnderMouse.Value);
			}
		}

		private void HandleGroundClick(Vector3 point)
		{
			using var ecb = new EntityCommandBuffer(Allocator.Temp);
			foreach (var entity in _playerQuery.ToEntityArray(Allocator.Temp))
			{
				ecb.AddComponent(entity, new MovementDestination {Value = point});
			}
			
			ecb.Playback(_entityManager);
		}

		private void HandleTargetClick(UnitView target)
		{
			using var ecb = new EntityCommandBuffer(Allocator.Temp);
			foreach (var entity in _playerQuery.ToEntityArray(Allocator.Temp))
			{
				var writer = new EcbWriter(ecb, ecb.CreateEntity());
				ArchetypesFactory.BuildAbilityProjectileAttack(writer, entity, target.Entity);
			}
			
			ecb.Playback(_entityManager);
		}

		private static int RaycastFromMouse(Camera camera, LayerMask mask, RaycastHit[] hits)
		{
			var mouse = Mouse.current;
			if (mouse == null)
			{
				return 0;
			}

			var ray = camera.ScreenPointToRay(mouse.position.ReadValue());
			return Physics.RaycastNonAlloc(ray, hits, RAYCAST_HEIGHT, mask, QueryTriggerInteraction.Ignore);
		}
	}
}