using System;
using GorillaExtensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001361 RID: 4961
	public class StickyCosmetic : MonoBehaviour
	{
		// Token: 0x06007C59 RID: 31833 RVA: 0x0028A04F File Offset: 0x0028824F
		private void Start()
		{
			this.endRigidbody.isKinematic = false;
			this.endRigidbody.useGravity = false;
			this.UpdateState(StickyCosmetic.ObjectState.Idle);
		}

		// Token: 0x06007C5A RID: 31834 RVA: 0x0028A070 File Offset: 0x00288270
		public void Extend()
		{
			if (this.currentState == StickyCosmetic.ObjectState.Idle || this.currentState == StickyCosmetic.ObjectState.Extending)
			{
				this.UpdateState(StickyCosmetic.ObjectState.Extending);
			}
		}

		// Token: 0x06007C5B RID: 31835 RVA: 0x0028A08A File Offset: 0x0028828A
		public void Retract()
		{
			this.UpdateState(StickyCosmetic.ObjectState.Retracting);
		}

		// Token: 0x06007C5C RID: 31836 RVA: 0x0028A094 File Offset: 0x00288294
		private void Extend_Internal()
		{
			if (this.endRigidbody.isKinematic)
			{
				return;
			}
			this.rayLength = Mathf.Lerp(0f, this.maxObjectLength, this.blendShapeCosmetic.GetBlendValue() / this.blendShapeCosmetic.maxBlendShapeWeight);
			this.endRigidbody.MovePosition(this.startPosition.position + this.startPosition.forward * this.rayLength);
		}

		// Token: 0x06007C5D RID: 31837 RVA: 0x0028A110 File Offset: 0x00288310
		private void Retract_Internal()
		{
			this.endRigidbody.isKinematic = false;
			Vector3 vector = Vector3.MoveTowards(this.endRigidbody.position, this.startPosition.position, this.retractSpeed * Time.fixedDeltaTime);
			this.endRigidbody.MovePosition(vector);
		}

		// Token: 0x06007C5E RID: 31838 RVA: 0x0028A160 File Offset: 0x00288360
		private void FixedUpdate()
		{
			switch (this.currentState)
			{
			case StickyCosmetic.ObjectState.Extending:
			{
				if (Time.time - this.extendingStartedTime > this.retractAfterSecond)
				{
					this.UpdateState(StickyCosmetic.ObjectState.AutoRetract);
				}
				this.Extend_Internal();
				RaycastHit raycastHit;
				if (Physics.Raycast(this.rayOrigin.position, this.rayOrigin.forward, out raycastHit, this.rayLength, this.collisionLayers))
				{
					this.endRigidbody.isKinematic = true;
					this.endRigidbody.transform.parent = null;
					UnityEvent unityEvent = this.onStick;
					if (unityEvent != null)
					{
						unityEvent.Invoke();
					}
					this.UpdateState(StickyCosmetic.ObjectState.Stuck);
				}
				break;
			}
			case StickyCosmetic.ObjectState.Retracting:
				if (Vector3.Distance(this.endRigidbody.position, this.startPosition.position) <= 0.01f)
				{
					this.endRigidbody.position = this.startPosition.position;
					Transform transform = this.endRigidbody.transform;
					transform.parent = this.endPositionParent;
					transform.localRotation = quaternion.identity;
					transform.localScale = Vector3.one;
					if (this.lastState == StickyCosmetic.ObjectState.AutoUnstuck || this.lastState == StickyCosmetic.ObjectState.AutoRetract)
					{
						this.UpdateState(StickyCosmetic.ObjectState.JustRetracted);
					}
					else
					{
						this.UpdateState(StickyCosmetic.ObjectState.Idle);
					}
				}
				else
				{
					this.Retract_Internal();
				}
				break;
			case StickyCosmetic.ObjectState.Stuck:
				if (this.endRigidbody.isKinematic && (this.endRigidbody.position - this.startPosition.position).IsLongerThan(this.autoRetractThreshold))
				{
					this.UpdateState(StickyCosmetic.ObjectState.AutoUnstuck);
				}
				break;
			case StickyCosmetic.ObjectState.AutoUnstuck:
				this.UpdateState(StickyCosmetic.ObjectState.Retracting);
				break;
			case StickyCosmetic.ObjectState.AutoRetract:
				this.UpdateState(StickyCosmetic.ObjectState.Retracting);
				break;
			}
			Debug.DrawRay(this.rayOrigin.position, this.rayOrigin.forward * this.rayLength, Color.red);
		}

		// Token: 0x06007C5F RID: 31839 RVA: 0x0028A340 File Offset: 0x00288540
		private void UpdateState(StickyCosmetic.ObjectState newState)
		{
			this.lastState = this.currentState;
			if (this.lastState == StickyCosmetic.ObjectState.Stuck && newState != this.currentState)
			{
				this.onUnstick.Invoke();
			}
			if (this.lastState != StickyCosmetic.ObjectState.Extending && newState == StickyCosmetic.ObjectState.Extending)
			{
				this.extendingStartedTime = Time.time;
			}
			this.currentState = newState;
		}

		// Token: 0x04008EAD RID: 36525
		[Tooltip("Optional reference to an UpdateBlendShapeCosmetic component. Used to drive extension length based on blend shape weight (e.g. finger flex input).")]
		[SerializeField]
		private UpdateBlendShapeCosmetic blendShapeCosmetic;

		// Token: 0x04008EAE RID: 36526
		[Tooltip("Defines which physics layers this sticky object can attach to when extending (checked via raycast).")]
		[SerializeField]
		private LayerMask collisionLayers;

		// Token: 0x04008EAF RID: 36527
		[Tooltip("Transform origin from which the raycast will be fired forward to detect stickable surfaces.")]
		[SerializeField]
		private Transform rayOrigin;

		// Token: 0x04008EB0 RID: 36528
		[Tooltip("Transform representing the start or base position of the sticky object (where extension originates).")]
		[SerializeField]
		private Transform startPosition;

		// Token: 0x04008EB1 RID: 36529
		[Tooltip("Rigidbody controlling the physical end of the sticky object (the part that extends and can attach).")]
		[SerializeField]
		private Rigidbody endRigidbody;

		// Token: 0x04008EB2 RID: 36530
		[Tooltip("Parent transform the end object will reattach to when fully retracted. This keeps local transform resets consistent.")]
		[SerializeField]
		private Transform endPositionParent;

		// Token: 0x04008EB3 RID: 36531
		[Tooltip("Maximum distance the object can extend from its start position (in meters).")]
		[SerializeField]
		private float maxObjectLength = 0.7f;

		// Token: 0x04008EB4 RID: 36532
		[Tooltip("If the sticky object remains stuck but the distance from start exceeds this threshold, it will automatically unstuck and begin retracting.")]
		[SerializeField]
		private float autoRetractThreshold = 1f;

		// Token: 0x04008EB5 RID: 36533
		[Tooltip("Speed (units per second) at which the end rigidbody retracts toward its start position when returning.")]
		[SerializeField]
		private float retractSpeed = 5f;

		// Token: 0x04008EB6 RID: 36534
		[Tooltip("If the sticky end remains extended but doesn’t stick to anything, it will automatically start retracting after this many seconds.")]
		[SerializeField]
		private float retractAfterSecond = 2f;

		// Token: 0x04008EB7 RID: 36535
		[Tooltip("Invoked when the sticky object successfully attaches to a surface.")]
		public UnityEvent onStick;

		// Token: 0x04008EB8 RID: 36536
		[Tooltip("Invoked when the sticky object becomes unstuck — either manually or automatically.")]
		public UnityEvent onUnstick;

		// Token: 0x04008EB9 RID: 36537
		private StickyCosmetic.ObjectState currentState;

		// Token: 0x04008EBA RID: 36538
		private float rayLength;

		// Token: 0x04008EBB RID: 36539
		private bool stick;

		// Token: 0x04008EBC RID: 36540
		private StickyCosmetic.ObjectState lastState;

		// Token: 0x04008EBD RID: 36541
		private float extendingStartedTime;

		// Token: 0x02001362 RID: 4962
		private enum ObjectState
		{
			// Token: 0x04008EBF RID: 36543
			Extending,
			// Token: 0x04008EC0 RID: 36544
			Retracting,
			// Token: 0x04008EC1 RID: 36545
			Stuck,
			// Token: 0x04008EC2 RID: 36546
			JustRetracted,
			// Token: 0x04008EC3 RID: 36547
			Idle,
			// Token: 0x04008EC4 RID: 36548
			AutoUnstuck,
			// Token: 0x04008EC5 RID: 36549
			AutoRetract
		}
	}
}
