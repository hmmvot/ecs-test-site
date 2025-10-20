using EcsTestSite.Components;
using TMPro;
using UnityEngine;

namespace EcsTestSite.Presentation
{
	public sealed class CharacterView : EntityView
	{
		public GameObject AliveView;
		public GameObject DeadView;

		public TMP_Text Overtip;

		private Camera _camera;

		public override void OnInitialize()
		{
			_camera = Camera.main;
		}

		public override void OnUpdate()
		{
			bool isDead = EntityManager.HasComponent<DeadTag>(Entity) && EntityManager.IsComponentEnabled<DeadTag>(Entity);
			if (isDead && (AliveView.activeSelf || !DeadView.activeSelf))
			{
				AliveView.SetActive(false);
				DeadView.SetActive(true);
			}
			else if (!isDead && (DeadView.activeSelf || !AliveView.activeSelf))
			{
				DeadView.SetActive(false);
				AliveView.SetActive(true);
			}

			if (Overtip != null)
			{
				Overtip.transform.rotation = Quaternion.LookRotation(Overtip.transform.position - _camera.transform.position, Vector3.up);
				float d = Vector3.Distance(_camera.transform.position, Overtip.transform.position);
				float k = d * 0.02f;
				Overtip.transform.localScale = Vector3.one * k;
				
				int health = EntityManager.HasComponent<Health>(Entity)
					? EntityManager.GetComponentData<Health>(Entity).Value
					: 0;
				Overtip.text = $"HP={health}";
			}
		}
	}
}