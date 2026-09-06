using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001050 RID: 4176
	public class KnockbackTrigger : MonoBehaviour
	{
		// Token: 0x170009E5 RID: 2533
		// (get) Token: 0x06006835 RID: 26677 RVA: 0x00218C5A File Offset: 0x00216E5A
		public bool TriggeredThisFrame
		{
			get
			{
				return this.lastTriggeredFrame == Time.frameCount;
			}
		}

		// Token: 0x06006836 RID: 26678 RVA: 0x00218C6C File Offset: 0x00216E6C
		private void CheckZone()
		{
			if (!this.hasCheckedZone)
			{
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(VRRigCache.Instance.localRig.Rig.zoneEntity.currentZone, out builderTable))
				{
					this.ignoreScale = !builderTable.isTableMutable;
				}
				this.hasCheckedZone = true;
			}
		}

		// Token: 0x06006837 RID: 26679 RVA: 0x00218CBC File Offset: 0x00216EBC
		private void OnTriggerEnter(Collider other)
		{
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHead) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHand))
			{
				return;
			}
			this.CheckZone();
			if (!this.ignoreScale && this.onlySmallMonke && (double)VRRigCache.Instance.localRig.Rig.scaleFactor > 0.99)
			{
				return;
			}
			this.collidersEntered.Add(other);
			if (this.collidersEntered.Count > 1)
			{
				return;
			}
			Vector3 vector = this.triggerVolume.ClosestPoint(GorillaTagger.Instance.headCollider.transform.position);
			Vector3 vector2 = vector - base.transform.TransformPoint(this.triggerVolume.center);
			vector2 -= Vector3.Project(vector2, base.transform.TransformDirection(this.localAxis));
			float magnitude = vector2.magnitude;
			Vector3 vector3 = Vector3.up;
			if (magnitude >= 0.01f)
			{
				vector3 = vector2 / magnitude;
			}
			GTPlayer.Instance.SetMaximumSlipThisFrame();
			GTPlayer.Instance.ApplyKnockback(vector3, this.knockbackVelocity * VRRigCache.Instance.localRig.Rig.scaleFactor, false);
			if (this.impactFX != null)
			{
				ObjectPools.instance.Instantiate(this.impactFX, vector, true);
			}
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
			this.lastTriggeredFrame = Time.frameCount;
		}

		// Token: 0x06006838 RID: 26680 RVA: 0x00218E5E File Offset: 0x0021705E
		private void OnTriggerExit(Collider other)
		{
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHead) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHand))
			{
				return;
			}
			this.collidersEntered.Remove(other);
		}

		// Token: 0x06006839 RID: 26681 RVA: 0x00218E9A File Offset: 0x0021709A
		private void OnDisable()
		{
			this.collidersEntered.Clear();
		}

		// Token: 0x04007779 RID: 30585
		[SerializeField]
		private BoxCollider triggerVolume;

		// Token: 0x0400777A RID: 30586
		[SerializeField]
		private float knockbackVelocity;

		// Token: 0x0400777B RID: 30587
		[SerializeField]
		private Vector3 localAxis;

		// Token: 0x0400777C RID: 30588
		[SerializeField]
		private GameObject impactFX;

		// Token: 0x0400777D RID: 30589
		[SerializeField]
		private bool onlySmallMonke;

		// Token: 0x0400777E RID: 30590
		private bool hasCheckedZone;

		// Token: 0x0400777F RID: 30591
		private bool ignoreScale;

		// Token: 0x04007780 RID: 30592
		private int lastTriggeredFrame = -1;

		// Token: 0x04007781 RID: 30593
		private List<Collider> collidersEntered = new List<Collider>(4);
	}
}
