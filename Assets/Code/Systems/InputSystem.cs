using EcsTestSite.Components;
using EcsTestSite.Presentation;
using EcsTestSite.Systems.Groups;
using EcsTestSite.Utility;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EcsTestSite.Systems
{
	[UpdateInGroup(typeof(InputSystemGroup))]
	public partial class InputSystem : SystemBase
	{
		private const float RAYCAST_HEIGHT = 100f;
		
		private static readonly RaycastHit[] _raycastHitsBuffer = new RaycastHit[64];
		
		private EntityQuery _playerQuery;

		private UnitView _unitUnderMouse;
		private Vector3? _positionUnderMouse;

		public Keyboard Keyboard { get; set; }
		public Mouse Mouse { get; set; }
		public Camera Camera { get; set; }

		protected override void OnCreate()
		{			
			_playerQuery = EntityManager.CreateEntityQuery(new EntityQueryDesc
			{
				All = new ComponentType[] {typeof(UnitTag), typeof(PlayerTag)},
			});
		}

		protected override void OnUpdate()
		{
			int count = RaycastFromMouse(Camera, Layers.ClickableMask, _raycastHitsBuffer);
			
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

			if (Mouse.leftButton.wasPressedThisFrame)
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
			
			ecb.Playback(EntityManager);
		}

		private void HandleTargetClick(UnitView target)
		{
			using var ecb = new EntityCommandBuffer(Allocator.Temp);
			foreach (var entity in _playerQuery.ToEntityArray(Allocator.Temp))
			{
				var writer = new EcbWriter(ecb, ecb.CreateEntity());
				ArchetypesFactory.BuildAbilityProjectileAttack(writer, entity, target.Entity);
			}
			
			ecb.Playback(EntityManager);
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