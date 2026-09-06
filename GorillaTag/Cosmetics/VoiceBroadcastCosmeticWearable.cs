using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200136D RID: 4973
	public class VoiceBroadcastCosmeticWearable : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06007C94 RID: 31892 RVA: 0x0028B304 File Offset: 0x00289504
		private void Start()
		{
			VoiceBroadcastCosmetic[] componentsInChildren = base.GetComponentInParent<VRRig>().GetComponentsInChildren<VoiceBroadcastCosmetic>(true);
			this.voiceBroadcasters = new List<VoiceBroadcastCosmetic>();
			foreach (VoiceBroadcastCosmetic voiceBroadcastCosmetic in componentsInChildren)
			{
				if (voiceBroadcastCosmetic.talkingCosmeticType == this.talkingCosmeticType)
				{
					this.voiceBroadcasters.Add(voiceBroadcastCosmetic);
					voiceBroadcastCosmetic.SetWearable(this);
				}
			}
		}

		// Token: 0x06007C95 RID: 31893 RVA: 0x0028B35C File Offset: 0x0028955C
		public void OnEnable()
		{
			if (this.playerHeadCollider == null)
			{
				VRRig componentInParent = base.GetComponentInParent<VRRig>();
				this.playerHeadCollider = ((componentInParent != null) ? componentInParent.rigContainer.HeadCollider : null);
			}
			if (this.headDistanceActivation && this.playerHeadCollider != null)
			{
				GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			}
		}

		// Token: 0x06007C96 RID: 31894 RVA: 0x00012134 File Offset: 0x00010334
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x06007C97 RID: 31895 RVA: 0x0028B3B4 File Offset: 0x002895B4
		public void SliceUpdate()
		{
			if (Time.time - this.lastToggleTime >= this.toggleCooldown)
			{
				bool flag = (base.transform.position - this.playerHeadCollider.transform.position).sqrMagnitude <= this.headDistance * this.headDistance;
				if (flag != this.toggleState)
				{
					this.toggleState = flag;
					this.lastToggleTime = Time.time;
					if (flag)
					{
						UnityEvent unityEvent = this.onStartListening;
						if (unityEvent != null)
						{
							unityEvent.Invoke();
						}
					}
					else
					{
						UnityEvent unityEvent2 = this.onStopListening;
						if (unityEvent2 != null)
						{
							unityEvent2.Invoke();
						}
					}
					for (int i = 0; i < this.voiceBroadcasters.Count; i++)
					{
						this.voiceBroadcasters[i].SetListenState(flag);
					}
				}
			}
		}

		// Token: 0x06007C98 RID: 31896 RVA: 0x0028B47D File Offset: 0x0028967D
		public void OnCosmeticStartListening()
		{
			if (this.headDistanceActivation)
			{
				return;
			}
			UnityEvent unityEvent = this.onStartListening;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x06007C99 RID: 31897 RVA: 0x0028B498 File Offset: 0x00289698
		public void OnCosmeticStopListening()
		{
			if (this.headDistanceActivation)
			{
				return;
			}
			UnityEvent unityEvent = this.onStopListening;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x04008F2A RID: 36650
		public TalkingCosmeticType talkingCosmeticType;

		// Token: 0x04008F2B RID: 36651
		[SerializeField]
		private bool headDistanceActivation = true;

		// Token: 0x04008F2C RID: 36652
		[SerializeField]
		private float headDistance = 0.4f;

		// Token: 0x04008F2D RID: 36653
		[SerializeField]
		private float toggleCooldown = 0.5f;

		// Token: 0x04008F2E RID: 36654
		private bool toggleState;

		// Token: 0x04008F2F RID: 36655
		private float lastToggleTime;

		// Token: 0x04008F30 RID: 36656
		[SerializeField]
		private UnityEvent onStartListening;

		// Token: 0x04008F31 RID: 36657
		[SerializeField]
		private UnityEvent onStopListening;

		// Token: 0x04008F32 RID: 36658
		private List<VoiceBroadcastCosmetic> voiceBroadcasters;

		// Token: 0x04008F33 RID: 36659
		private Collider playerHeadCollider;
	}
}
