using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace GorillaTagScripts.Builder
{
	// Token: 0x0200104B RID: 4171
	public class BuilderSmallHandTrigger : MonoBehaviour
	{
		// Token: 0x170009E2 RID: 2530
		// (get) Token: 0x06006814 RID: 26644 RVA: 0x00217CF4 File Offset: 0x00215EF4
		public bool TriggeredThisFrame
		{
			get
			{
				return this.lastTriggeredFrame == Time.frameCount;
			}
		}

		// Token: 0x06006815 RID: 26645 RVA: 0x00217D04 File Offset: 0x00215F04
		private void OnTriggerEnter(Collider other)
		{
			if (!base.enabled)
			{
				return;
			}
			GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent == null)
			{
				return;
			}
			if (!this.hasCheckedZone)
			{
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(VRRigCache.Instance.localRig.Rig.zoneEntity.currentZone, out builderTable))
				{
					this.ignoreScale = !builderTable.isTableMutable;
				}
				this.hasCheckedZone = true;
			}
			if (this.onlySmallHands && !this.ignoreScale && (double)VRRigCache.Instance.localRig.Rig.scaleFactor > 0.99)
			{
				return;
			}
			if (this.requireMinimumVelocity)
			{
				float num = this.minimumVelocityMagnitude * GorillaTagger.Instance.offlineVRRig.scaleFactor;
				if (GTPlayer.Instance.GetHandVelocityTracker(componentInParent.isLeftHand).GetAverageVelocity(true, 0.1f, false).sqrMagnitude < num * num)
				{
					return;
				}
			}
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration * 1.5f);
			this.lastTriggeredFrame = Time.frameCount;
			UnityEvent triggeredEvent = this.TriggeredEvent;
			if (triggeredEvent != null)
			{
				triggeredEvent.Invoke();
			}
			if (this.timeline != null && (this.timeline.time == 0.0 || this.timeline.time >= this.timeline.duration))
			{
				this.timeline.Play();
			}
			if (this.animation != null && this.animation.clip != null)
			{
				this.animation.Play();
			}
		}

		// Token: 0x0400773C RID: 30524
		[Tooltip("Optional timeline to play to animate the thing getting activated, play sound, particles, etc...")]
		public PlayableDirector timeline;

		// Token: 0x0400773D RID: 30525
		[Tooltip("Optional animation to play")]
		public Animation animation;

		// Token: 0x0400773E RID: 30526
		private int lastTriggeredFrame = -1;

		// Token: 0x0400773F RID: 30527
		public bool onlySmallHands;

		// Token: 0x04007740 RID: 30528
		[SerializeField]
		protected bool requireMinimumVelocity;

		// Token: 0x04007741 RID: 30529
		[SerializeField]
		protected float minimumVelocityMagnitude = 0.1f;

		// Token: 0x04007742 RID: 30530
		private bool hasCheckedZone;

		// Token: 0x04007743 RID: 30531
		private bool ignoreScale;

		// Token: 0x04007744 RID: 30532
		internal UnityEvent TriggeredEvent = new UnityEvent();

		// Token: 0x04007745 RID: 30533
		[SerializeField]
		private BuilderPiece myPiece;
	}
}
