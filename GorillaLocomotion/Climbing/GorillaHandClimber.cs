using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x020011BA RID: 4538
	public class GorillaHandClimber : MonoBehaviour
	{
		// Token: 0x17000B36 RID: 2870
		// (get) Token: 0x06007273 RID: 29299 RVA: 0x00254BA8 File Offset: 0x00252DA8
		public bool isClimbingOrGrabbing
		{
			get
			{
				return this.isClimbing || this.grabber.isGrabbing;
			}
		}

		// Token: 0x06007274 RID: 29300 RVA: 0x00254BBF File Offset: 0x00252DBF
		private void Awake()
		{
			this.col = base.GetComponent<Collider>();
			this.grabber = base.GetComponent<GorillaGrabber>();
		}

		// Token: 0x06007275 RID: 29301 RVA: 0x00254BDC File Offset: 0x00252DDC
		public void CheckHandClimber()
		{
			for (int i = this.potentialClimbables.Count - 1; i >= 0; i--)
			{
				GorillaClimbable gorillaClimbable = this.potentialClimbables[i];
				if (gorillaClimbable == null || !gorillaClimbable.isActiveAndEnabled)
				{
					this.potentialClimbables.RemoveAt(i);
				}
				else if (gorillaClimbable.climbOnlyWhileSmall && !ZoneManagement.IsInZone(GTZone.monkeBlocksShared) && this.player.scale > 0.99f)
				{
					this.potentialClimbables.RemoveAt(i);
				}
			}
			bool grab = ControllerInputPoller.GetGrab(this.xrNode);
			bool grabRelease = ControllerInputPoller.GetGrabRelease(this.xrNode);
			if (!this.isClimbing)
			{
				if (this.queuedToBecomeValidToGrabAgain && Vector3.Distance(this.lastAutoReleasePos, this.handRoot.localPosition) >= 0.35f)
				{
					this.queuedToBecomeValidToGrabAgain = false;
				}
				if (grabRelease)
				{
					this.queuedToBecomeValidToGrabAgain = false;
					this.dontReclimbLast = null;
				}
				GorillaClimbable closestClimbable = this.GetClosestClimbable();
				if (!this.queuedToBecomeValidToGrabAgain && closestClimbable && grab && this.CanInitiateClimb() && closestClimbable != this.dontReclimbLast)
				{
					GorillaClimbableRef gorillaClimbableRef = closestClimbable as GorillaClimbableRef;
					if (gorillaClimbableRef != null)
					{
						this.player.BeginClimbing(gorillaClimbableRef.climb, this, gorillaClimbableRef);
					}
					else
					{
						this.player.BeginClimbing(closestClimbable, this, null);
					}
				}
			}
			else if (grabRelease && this.canRelease)
			{
				this.player.EndClimbing(this, false, false);
			}
			this.grabber.CheckGrabber(this.CanInitiateClimb() && grab);
		}

		// Token: 0x06007276 RID: 29302 RVA: 0x00254D54 File Offset: 0x00252F54
		private bool CanInitiateClimb()
		{
			return !this.isClimbing && !this.equipmentInteractor.GetIsHolding(this.xrNode) && !this.equipmentInteractor.builderPieceInteractor.GetIsHolding(this.xrNode) && !this.equipmentInteractor.IsGrabDisabled(this.xrNode) && !GamePlayerLocal.IsHandHolding(this.xrNode) && !this.player.inOverlay;
		}

		// Token: 0x06007277 RID: 29303 RVA: 0x00254DC4 File Offset: 0x00252FC4
		public void SetCanRelease(bool canRelease)
		{
			this.canRelease = canRelease;
		}

		// Token: 0x06007278 RID: 29304 RVA: 0x00254DD0 File Offset: 0x00252FD0
		public GorillaClimbable GetClosestClimbable()
		{
			if (this.potentialClimbables.Count == 0)
			{
				return null;
			}
			if (this.potentialClimbables.Count == 1)
			{
				return this.potentialClimbables[0];
			}
			Vector3 position = base.transform.position;
			Bounds bounds = this.col.bounds;
			float num = 0.15f;
			GorillaClimbable gorillaClimbable = null;
			foreach (GorillaClimbable gorillaClimbable2 in this.potentialClimbables)
			{
				float num2;
				if (gorillaClimbable2.colliderCache)
				{
					if (!bounds.Intersects(gorillaClimbable2.colliderCache.bounds))
					{
						continue;
					}
					Vector3 vector = gorillaClimbable2.colliderCache.ClosestPoint(position);
					num2 = Vector3.Distance(position, vector);
				}
				else
				{
					num2 = Vector3.Distance(position, gorillaClimbable2.transform.position);
				}
				if (num2 < num)
				{
					gorillaClimbable = gorillaClimbable2;
					num = num2;
				}
			}
			return gorillaClimbable;
		}

		// Token: 0x06007279 RID: 29305 RVA: 0x00254ED0 File Offset: 0x002530D0
		private void OnTriggerEnter(Collider other)
		{
			GorillaClimbable gorillaClimbable;
			if (other.TryGetComponent<GorillaClimbable>(out gorillaClimbable))
			{
				this.potentialClimbables.Add(gorillaClimbable);
				return;
			}
			GorillaClimbableRef gorillaClimbableRef;
			if (other.TryGetComponent<GorillaClimbableRef>(out gorillaClimbableRef))
			{
				this.potentialClimbables.Add(gorillaClimbableRef);
			}
		}

		// Token: 0x0600727A RID: 29306 RVA: 0x00254F0C File Offset: 0x0025310C
		private void OnTriggerExit(Collider other)
		{
			GorillaClimbable gorillaClimbable;
			if (other.TryGetComponent<GorillaClimbable>(out gorillaClimbable))
			{
				this.potentialClimbables.Remove(gorillaClimbable);
				return;
			}
			GorillaClimbableRef gorillaClimbableRef;
			if (other.TryGetComponent<GorillaClimbableRef>(out gorillaClimbableRef))
			{
				this.potentialClimbables.Remove(gorillaClimbableRef);
			}
		}

		// Token: 0x0600727B RID: 29307 RVA: 0x00254F48 File Offset: 0x00253148
		public void ForceStopClimbing(bool startingNewClimb = false, bool doDontReclimb = false)
		{
			this.player.EndClimbing(this, startingNewClimb, doDontReclimb);
		}

		// Token: 0x0400834E RID: 33614
		[SerializeField]
		private GTPlayer player;

		// Token: 0x0400834F RID: 33615
		[SerializeField]
		private EquipmentInteractor equipmentInteractor;

		// Token: 0x04008350 RID: 33616
		private List<GorillaClimbable> potentialClimbables = new List<GorillaClimbable>();

		// Token: 0x04008351 RID: 33617
		[Header("Non-hand input should have the component disabled")]
		public XRNode xrNode = XRNode.LeftHand;

		// Token: 0x04008352 RID: 33618
		[NonSerialized]
		public bool isClimbing;

		// Token: 0x04008353 RID: 33619
		[NonSerialized]
		public bool queuedToBecomeValidToGrabAgain;

		// Token: 0x04008354 RID: 33620
		[NonSerialized]
		public GorillaClimbable dontReclimbLast;

		// Token: 0x04008355 RID: 33621
		[NonSerialized]
		public Vector3 lastAutoReleasePos = Vector3.zero;

		// Token: 0x04008356 RID: 33622
		public GorillaGrabber grabber;

		// Token: 0x04008357 RID: 33623
		public Transform handRoot;

		// Token: 0x04008358 RID: 33624
		private const float DIST_FOR_CLEAR_RELEASE = 0.35f;

		// Token: 0x04008359 RID: 33625
		private const float DIST_FOR_GRAB = 0.15f;

		// Token: 0x0400835A RID: 33626
		private Collider col;

		// Token: 0x0400835B RID: 33627
		private bool canRelease = true;
	}
}
