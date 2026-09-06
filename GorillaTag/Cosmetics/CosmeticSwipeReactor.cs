using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200136F RID: 4975
	[RequireComponent(typeof(Collider))]
	public class CosmeticSwipeReactor : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x06007C9D RID: 31901 RVA: 0x0028B50C File Offset: 0x0028970C
		private void Awake()
		{
			this._rig = base.GetComponentInParent<VRRig>();
			if (this._rig == null && base.gameObject.GetComponentInParent<GTPlayer>() != null)
			{
				this._rig = GorillaTagger.Instance.offlineVRRig;
			}
			this.isLocal = this._rig != null && this._rig.isLocal;
			this.col = base.GetComponent<Collider>();
			switch (this.localSwipeAxis)
			{
			case CosmeticSwipeReactor.Axis.X:
				this.swipeDir = Vector3.right;
				return;
			case CosmeticSwipeReactor.Axis.Y:
				this.swipeDir = Vector3.up;
				return;
			case CosmeticSwipeReactor.Axis.Z:
				this.swipeDir = Vector3.forward;
				return;
			default:
				return;
			}
		}

		// Token: 0x06007C9E RID: 31902 RVA: 0x0028B5C4 File Offset: 0x002897C4
		private void OnTriggerEnter(Collider other)
		{
			if (!this.isLocal || !base.enabled)
			{
				return;
			}
			GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (component != null)
			{
				if (component.isLeftHand)
				{
					this.handIndicatorL = component;
					Vector3 vector = base.transform.InverseTransformPoint(component.transform.position);
					this.ResetProgress(true, vector);
					this.handInTriggerL = true;
				}
				else
				{
					this.handIndicatorR = component;
					Vector3 vector2 = base.transform.InverseTransformPoint(component.transform.position);
					this.ResetProgress(false, vector2);
					this.handInTriggerR = true;
				}
			}
			if ((this.handInTriggerL || this.handInTriggerR) && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06007C9F RID: 31903 RVA: 0x0028B674 File Offset: 0x00289874
		private void OnTriggerExit(Collider other)
		{
			if (!this.isLocal || !base.enabled)
			{
				return;
			}
			GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (component != null)
			{
				if (component.isLeftHand)
				{
					this.handInTriggerL = false;
					if (this.resetCooldownOnTriggerExit)
					{
						this.isCoolingDownL = false;
						this.cooldownEndL = double.MinValue;
					}
				}
				else
				{
					this.handInTriggerR = false;
					if (this.resetCooldownOnTriggerExit)
					{
						this.isCoolingDownR = false;
						this.cooldownEndR = double.MinValue;
					}
				}
			}
			if (!this.handInTriggerL && !this.handInTriggerR && this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}

		// Token: 0x17000C2B RID: 3115
		// (get) Token: 0x06007CA0 RID: 31904 RVA: 0x0028B714 File Offset: 0x00289914
		// (set) Token: 0x06007CA1 RID: 31905 RVA: 0x0028B71C File Offset: 0x0028991C
		public bool TickRunning { get; set; }

		// Token: 0x06007CA2 RID: 31906 RVA: 0x0028B728 File Offset: 0x00289928
		public void Tick()
		{
			if (this.handInTriggerL)
			{
				this.ProcessHandMovement(this.handIndicatorL, this.startPosL, ref this.lastFramePosL, ref this.swipingUpL, ref this.distanceL, ref this.isCoolingDownL, ref this.cooldownEndL);
			}
			if (this.handInTriggerR)
			{
				this.ProcessHandMovement(this.handIndicatorR, this.startPosR, ref this.lastFramePosR, ref this.swipingUpR, ref this.distanceR, ref this.isCoolingDownR, ref this.cooldownEndR);
			}
			if (!this.handInTriggerL && !this.handInTriggerR && this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}

		// Token: 0x06007CA3 RID: 31907 RVA: 0x0028B7C4 File Offset: 0x002899C4
		private void ResetProgress(bool left, Vector3 pos)
		{
			if (left)
			{
				this.startPosL = pos;
				this.lastFramePosL = this.startPosL;
				this.distanceL = 0f;
				return;
			}
			this.startPosR = pos;
			this.lastFramePosR = this.startPosR;
			this.distanceR = 0f;
		}

		// Token: 0x06007CA4 RID: 31908 RVA: 0x0028B814 File Offset: 0x00289A14
		private void ProcessHandMovement(GorillaTriggerColliderHandIndicator hand, Vector3 start, ref Vector3 last, ref bool swipingUp, ref float dist, ref bool isCoolingDown, ref double cooldownEndTime)
		{
			if (isCoolingDown)
			{
				if (Time.timeAsDouble < cooldownEndTime)
				{
					return;
				}
				isCoolingDown = false;
				cooldownEndTime = double.MinValue;
				this.ResetProgress(hand.isLeftHand, base.transform.InverseTransformPoint(hand.transform.position));
				return;
			}
			else
			{
				Vector3 vector = base.transform.InverseTransformPoint(hand.transform.position);
				float num = Mathf.Abs(this.GetAxisComponent(hand.currentVelocity));
				if (num < this.minimumVelocity * this._rig.scaleFactor || num > this.maximumVelocity * this._rig.scaleFactor)
				{
					this.ResetProgress(hand.isLeftHand, vector);
					return;
				}
				float num2 = this.GetAxisComponent(vector) - this.GetAxisComponent(last);
				if (num2 >= 0f && !swipingUp)
				{
					swipingUp = true;
					this.ResetProgress(hand.isLeftHand, vector);
					return;
				}
				if ((num2 < 0f) & swipingUp)
				{
					swipingUp = false;
					this.ResetProgress(hand.isLeftHand, vector);
					return;
				}
				if ((this.GetLateralMovement(start) - this.GetLateralMovement(vector)).sqrMagnitude > this.lateralMovementTolerance * this.lateralMovementTolerance)
				{
					this.ResetProgress(hand.isLeftHand, vector);
					return;
				}
				last = vector;
				dist += Mathf.Abs(num2);
				GorillaTagger.Instance.StartVibration(hand.isLeftHand, this.swipeHaptics.Evaluate(dist / this.swipeDistance), Time.deltaTime);
				if (dist >= this.swipeDistance)
				{
					if (swipingUp)
					{
						UnityEvent<bool> onSwipe = this.OnSwipe;
						if (onSwipe != null)
						{
							onSwipe.Invoke(hand.isLeftHand);
						}
						cooldownEndTime = Time.timeAsDouble + (double)this.swipeCooldown;
						isCoolingDown = true;
					}
					else
					{
						UnityEvent<bool> onReverseSwipe = this.OnReverseSwipe;
						if (onReverseSwipe != null)
						{
							onReverseSwipe.Invoke(hand.isLeftHand);
						}
						cooldownEndTime = Time.timeAsDouble + (double)this.swipeCooldown;
						isCoolingDown = true;
					}
					this.ResetProgress(hand.isLeftHand, vector);
				}
				return;
			}
		}

		// Token: 0x06007CA5 RID: 31909 RVA: 0x0028BA04 File Offset: 0x00289C04
		private float GetAxisComponent(Vector3 vec)
		{
			CosmeticSwipeReactor.Axis axis = this.localSwipeAxis;
			if (axis == CosmeticSwipeReactor.Axis.X)
			{
				return vec.x;
			}
			if (axis != CosmeticSwipeReactor.Axis.Y)
			{
				return vec.z;
			}
			return vec.y;
		}

		// Token: 0x06007CA6 RID: 31910 RVA: 0x0028BA38 File Offset: 0x00289C38
		private Vector2 GetLateralMovement(Vector3 vec)
		{
			CosmeticSwipeReactor.Axis axis = this.localSwipeAxis;
			if (axis == CosmeticSwipeReactor.Axis.X)
			{
				return new Vector2(vec.y, vec.z);
			}
			if (axis != CosmeticSwipeReactor.Axis.Y)
			{
				return new Vector2(vec.x, vec.y);
			}
			return new Vector2(vec.x, vec.z);
		}

		// Token: 0x04008F36 RID: 36662
		[SerializeField]
		private CosmeticSwipeReactor.Axis localSwipeAxis = CosmeticSwipeReactor.Axis.Y;

		// Token: 0x04008F37 RID: 36663
		private Vector3 swipeDir = Vector3.up;

		// Token: 0x04008F38 RID: 36664
		[Tooltip("Distance hand can move perpindicular to the swipe without cancelling the gesture")]
		[SerializeField]
		private float lateralMovementTolerance = 0.1f;

		// Token: 0x04008F39 RID: 36665
		[Tooltip("How far the hand has to move along the axis to count as a swipe\nThis distance must be contained within the trigger area")]
		[SerializeField]
		private float swipeDistance = 0.3f;

		// Token: 0x04008F3A RID: 36666
		[SerializeField]
		private float minimumVelocity = 0.1f;

		// Token: 0x04008F3B RID: 36667
		[SerializeField]
		private float maximumVelocity = 3f;

		// Token: 0x04008F3C RID: 36668
		[Tooltip("Delay after completing a swipe before starting the next")]
		[SerializeField]
		private float swipeCooldown = 0.25f;

		// Token: 0x04008F3D RID: 36669
		[SerializeField]
		private bool resetCooldownOnTriggerExit = true;

		// Token: 0x04008F3E RID: 36670
		[Tooltip("Amplitude of haptics from normalized swiped distance")]
		[SerializeField]
		private AnimationCurve swipeHaptics = AnimationCurve.EaseInOut(0f, 0.02f, 1f, 0.5f);

		// Token: 0x04008F3F RID: 36671
		public UnityEvent<bool> OnSwipe;

		// Token: 0x04008F40 RID: 36672
		public UnityEvent<bool> OnReverseSwipe;

		// Token: 0x04008F41 RID: 36673
		private VRRig _rig;

		// Token: 0x04008F42 RID: 36674
		private Collider col;

		// Token: 0x04008F43 RID: 36675
		private bool isLocal;

		// Token: 0x04008F44 RID: 36676
		private bool handInTriggerR;

		// Token: 0x04008F45 RID: 36677
		private bool handInTriggerL;

		// Token: 0x04008F46 RID: 36678
		private GorillaTriggerColliderHandIndicator handIndicatorR;

		// Token: 0x04008F47 RID: 36679
		private GorillaTriggerColliderHandIndicator handIndicatorL;

		// Token: 0x04008F48 RID: 36680
		private Vector3 startPosR;

		// Token: 0x04008F49 RID: 36681
		private Vector3 startPosL;

		// Token: 0x04008F4A RID: 36682
		private Vector3 lastFramePosR;

		// Token: 0x04008F4B RID: 36683
		private Vector3 lastFramePosL;

		// Token: 0x04008F4C RID: 36684
		private float distanceR;

		// Token: 0x04008F4D RID: 36685
		private float distanceL;

		// Token: 0x04008F4E RID: 36686
		private bool swipingUpL;

		// Token: 0x04008F4F RID: 36687
		private bool swipingUpR;

		// Token: 0x04008F50 RID: 36688
		private double cooldownEndL = double.MinValue;

		// Token: 0x04008F51 RID: 36689
		private double cooldownEndR = double.MinValue;

		// Token: 0x04008F52 RID: 36690
		private bool isCoolingDownL;

		// Token: 0x04008F53 RID: 36691
		private bool isCoolingDownR;

		// Token: 0x02001370 RID: 4976
		public enum Axis
		{
			// Token: 0x04008F56 RID: 36694
			X,
			// Token: 0x04008F57 RID: 36695
			Y,
			// Token: 0x04008F58 RID: 36696
			Z
		}
	}
}
