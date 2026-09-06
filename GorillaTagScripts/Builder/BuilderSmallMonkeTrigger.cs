using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x0200104C RID: 4172
	public class BuilderSmallMonkeTrigger : MonoBehaviour
	{
		// Token: 0x170009E3 RID: 2531
		// (get) Token: 0x06006817 RID: 26647 RVA: 0x00217EC4 File Offset: 0x002160C4
		public int overlapCount
		{
			get
			{
				return this.overlappingColliders.Count;
			}
		}

		// Token: 0x170009E4 RID: 2532
		// (get) Token: 0x06006818 RID: 26648 RVA: 0x00217ED1 File Offset: 0x002160D1
		public bool TriggeredThisFrame
		{
			get
			{
				return this.lastTriggeredFrame == Time.frameCount;
			}
		}

		// Token: 0x140000AE RID: 174
		// (add) Token: 0x06006819 RID: 26649 RVA: 0x00217EE0 File Offset: 0x002160E0
		// (remove) Token: 0x0600681A RID: 26650 RVA: 0x00217F18 File Offset: 0x00216118
		public event Action<int> onPlayerEnteredTrigger;

		// Token: 0x140000AF RID: 175
		// (add) Token: 0x0600681B RID: 26651 RVA: 0x00217F50 File Offset: 0x00216150
		// (remove) Token: 0x0600681C RID: 26652 RVA: 0x00217F88 File Offset: 0x00216188
		public event Action onTriggerFirstEntered;

		// Token: 0x140000B0 RID: 176
		// (add) Token: 0x0600681D RID: 26653 RVA: 0x00217FC0 File Offset: 0x002161C0
		// (remove) Token: 0x0600681E RID: 26654 RVA: 0x00217FF8 File Offset: 0x002161F8
		public event Action onTriggerLastExited;

		// Token: 0x0600681F RID: 26655 RVA: 0x00218030 File Offset: 0x00216230
		public void ValidateOverlappingColliders()
		{
			for (int i = this.overlappingColliders.Count - 1; i >= 0; i--)
			{
				if (this.overlappingColliders[i] == null || !this.overlappingColliders[i].gameObject.activeInHierarchy || !this.overlappingColliders[i].enabled)
				{
					this.overlappingColliders.RemoveAt(i);
				}
				else
				{
					VRRig vrrig = this.overlappingColliders[i].attachedRigidbody.gameObject.GetComponent<VRRig>();
					if (vrrig == null)
					{
						if (GTPlayer.Instance.bodyCollider == this.overlappingColliders[i] || GTPlayer.Instance.headCollider == this.overlappingColliders[i])
						{
							vrrig = GorillaTagger.Instance.offlineVRRig;
						}
						else
						{
							this.overlappingColliders.RemoveAt(i);
						}
					}
					if (!this.ignoreScale && vrrig != null && (double)vrrig.scaleFactor > 0.99)
					{
						this.overlappingColliders.RemoveAt(i);
					}
				}
			}
		}

		// Token: 0x06006820 RID: 26656 RVA: 0x00218154 File Offset: 0x00216354
		private void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody == null)
			{
				return;
			}
			VRRig vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (vrrig == null)
			{
				if (!(GTPlayer.Instance.bodyCollider == other) && !(GTPlayer.Instance.headCollider == other))
				{
					return;
				}
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (!this.hasCheckedZone)
			{
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(vrrig.zoneEntity.currentZone, out builderTable))
				{
					this.ignoreScale = !builderTable.isTableMutable;
				}
				this.hasCheckedZone = true;
			}
			if (!this.ignoreScale && (double)vrrig.scaleFactor > 0.99)
			{
				return;
			}
			if (vrrig != null)
			{
				Action<int> action = this.onPlayerEnteredTrigger;
				if (action != null)
				{
					action(vrrig.OwningNetPlayer.ActorNumber);
				}
			}
			bool flag = this.overlappingColliders.Count == 0;
			if (!this.overlappingColliders.Contains(other))
			{
				this.overlappingColliders.Add(other);
			}
			this.lastTriggeredFrame = Time.frameCount;
			if (flag)
			{
				Action action2 = this.onTriggerFirstEntered;
				if (action2 == null)
				{
					return;
				}
				action2();
			}
		}

		// Token: 0x06006821 RID: 26657 RVA: 0x00218273 File Offset: 0x00216473
		private void OnTriggerExit(Collider other)
		{
			if (this.overlappingColliders.Remove(other) && this.overlappingColliders.Count == 0)
			{
				Action action = this.onTriggerLastExited;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x04007746 RID: 30534
		private int lastTriggeredFrame = -1;

		// Token: 0x04007747 RID: 30535
		private List<Collider> overlappingColliders = new List<Collider>(20);

		// Token: 0x0400774B RID: 30539
		private bool hasCheckedZone;

		// Token: 0x0400774C RID: 30540
		private bool ignoreScale;
	}
}
