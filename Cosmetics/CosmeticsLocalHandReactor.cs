using System;
using UnityEngine;
using UnityEngine.Events;

namespace Cosmetics
{
	// Token: 0x020011D7 RID: 4567
	public class CosmeticsLocalHandReactor : MonoBehaviour
	{
		// Token: 0x06007416 RID: 29718 RVA: 0x0025C26C File Offset: 0x0025A46C
		protected void Awake()
		{
			this.ownerRig = base.GetComponentInParent<VRRig>();
			if (this.ownerRig == null)
			{
				GorillaTagger componentInParent = base.GetComponentInParent<GorillaTagger>();
				if (componentInParent != null)
				{
					this.ownerRig = componentInParent.offlineVRRig;
					this.ownerIsLocal = this.ownerRig != null;
				}
			}
			if (this.ownerRig == null)
			{
				Debug.LogError("TriggerToggler: Disabling cannot find VRRig.");
				base.enabled = false;
				return;
			}
		}

		// Token: 0x06007417 RID: 29719 RVA: 0x0025C2E4 File Offset: 0x0025A4E4
		protected void LateUpdate()
		{
			if (this.ownerIsLocal)
			{
				if (Time.time < this.lastTriggerTime + this.cooldownTime)
				{
					return;
				}
				Transform transform = base.transform;
				if (Physics.OverlapSphereNonAlloc(base.transform.position, this.proximityThreshold * transform.lossyScale.x, this.colliders, this.handLayer) > 0)
				{
					GorillaTriggerColliderHandIndicator component = this.colliders[0].GetComponent<GorillaTriggerColliderHandIndicator>();
					if (component != null)
					{
						GorillaTagger.Instance.StartVibration(component.isLeftHand, this.hapticStrength, this.hapticDuration);
						UnityEvent<bool> unityEvent = this.onTrigger;
						if (unityEvent != null)
						{
							unityEvent.Invoke(component.isLeftHand);
						}
						this.lastTriggerTime = Time.time;
					}
				}
			}
		}

		// Token: 0x040083D9 RID: 33753
		[SerializeField]
		private float hapticStrength = 0.2f;

		// Token: 0x040083DA RID: 33754
		[SerializeField]
		private float hapticDuration = 0.2f;

		// Token: 0x040083DB RID: 33755
		[Tooltip("The distance threshold (in meters) for triggering the interaction.\nIf the hand enters this range, onTrigger is fired.")]
		public float proximityThreshold = 0.15f;

		// Token: 0x040083DC RID: 33756
		[Tooltip("Minimum time (in seconds) between consecutive triggers.\n")]
		[SerializeField]
		private float cooldownTime = 0.5f;

		// Token: 0x040083DD RID: 33757
		public UnityEvent<bool> onTrigger;

		// Token: 0x040083DE RID: 33758
		private VRRig ownerRig;

		// Token: 0x040083DF RID: 33759
		private bool ownerIsLocal;

		// Token: 0x040083E0 RID: 33760
		private float lastTriggerTime = float.MinValue;

		// Token: 0x040083E1 RID: 33761
		private readonly Collider[] colliders = new Collider[1];

		// Token: 0x040083E2 RID: 33762
		private LayerMask handLayer = 1024;
	}
}
