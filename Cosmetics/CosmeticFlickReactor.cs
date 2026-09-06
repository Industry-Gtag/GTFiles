using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace Cosmetics
{
	// Token: 0x020011D5 RID: 4565
	public class CosmeticFlickReactor : MonoBehaviour
	{
		// Token: 0x0600740E RID: 29710 RVA: 0x0025BDCE File Offset: 0x00259FCE
		private void Reset()
		{
			if (this.speedTracker == null)
			{
				this.speedTracker = base.GetComponent<SimpleSpeedTracker>();
			}
			if (this.rb == null)
			{
				this.rb = base.GetComponent<Rigidbody>();
			}
		}

		// Token: 0x0600740F RID: 29711 RVA: 0x0025BE04 File Offset: 0x0025A004
		private void Awake()
		{
			this.rig = base.GetComponentInParent<VRRig>();
			if (this.rig == null && base.gameObject.GetComponentInParent<GTPlayer>() != null)
			{
				this.rig = GorillaTagger.Instance.offlineVRRig;
			}
			this.isLocal = this.rig != null && this.rig.isLocal;
			this.ResetState();
			this.blockUntilTime = 0f;
			this.hasLastPosition = false;
		}

		// Token: 0x06007410 RID: 29712 RVA: 0x0025BE88 File Offset: 0x0025A088
		private void Update()
		{
			Vector3 vector = this.ResolveAxisDirection();
			if (vector.sqrMagnitude < 0.5f)
			{
				return;
			}
			float signedSpeedAlong = this.GetSignedSpeedAlong(vector);
			if (Mathf.Abs(signedSpeedAlong) >= this.minSpeedThreshold)
			{
				int num = ((signedSpeedAlong > 0f) ? 1 : (-1));
				if (num != this.lastPeakSign || Mathf.Abs(signedSpeedAlong) > Mathf.Abs(this.lastPeakSpeed))
				{
					if (this.lastPeakSign == 0 || num != -this.lastPeakSign)
					{
						this.lastPeakSign = num;
						this.lastPeakSpeed = signedSpeedAlong;
						this.lastPeakTime = Time.time;
						return;
					}
					float num2 = Time.time - this.lastPeakTime;
					float num3 = Mathf.Abs(this.lastPeakSpeed) + Mathf.Abs(signedSpeedAlong);
					bool flag = num2 <= this.flickWindowSeconds;
					bool flag2 = num3 >= this.directionChangeRequired;
					bool flag3 = Time.time >= this.blockUntilTime;
					if (flag && flag2 && flag3)
					{
						this.FireEvents(Mathf.Abs(signedSpeedAlong));
						this.blockUntilTime = Time.time + this.retriggerBufferSeconds;
						this.ResetState();
						return;
					}
					this.lastPeakSign = num;
					this.lastPeakSpeed = signedSpeedAlong;
					this.lastPeakTime = Time.time;
					return;
				}
			}
			else if (Time.time - this.lastPeakTime > this.flickWindowSeconds)
			{
				this.ResetState();
			}
		}

		// Token: 0x06007411 RID: 29713 RVA: 0x0025BFD4 File Offset: 0x0025A1D4
		private Vector3 ResolveAxisDirection()
		{
			switch (this.axisMode)
			{
			case CosmeticFlickReactor.AxisMode.X:
				if (!this.useWorldAxes)
				{
					return base.transform.right;
				}
				if (!(this.worldSpace != null))
				{
					return Vector3.right;
				}
				return this.worldSpace.right;
			case CosmeticFlickReactor.AxisMode.Y:
				if (!this.useWorldAxes)
				{
					return base.transform.up;
				}
				if (!(this.worldSpace != null))
				{
					return Vector3.up;
				}
				return this.worldSpace.up;
			case CosmeticFlickReactor.AxisMode.Z:
				if (!this.useWorldAxes)
				{
					return base.transform.forward;
				}
				if (!(this.worldSpace != null))
				{
					return Vector3.forward;
				}
				return this.worldSpace.forward;
			case CosmeticFlickReactor.AxisMode.CustomForward:
				if (!(this.axisReference != null))
				{
					return Vector3.zero;
				}
				return this.axisReference.forward;
			default:
				return Vector3.zero;
			}
		}

		// Token: 0x06007412 RID: 29714 RVA: 0x0025C0C4 File Offset: 0x0025A2C4
		private float GetSignedSpeedAlong(Vector3 axis)
		{
			Vector3 vector;
			if (this.speedTracker != null)
			{
				vector = this.speedTracker.GetWorldVelocity();
			}
			else if (this.rb != null)
			{
				vector = this.rb.linearVelocity;
			}
			else
			{
				if (!this.hasLastPosition)
				{
					this.lastPosition = base.transform.position;
					this.hasLastPosition = true;
					return 0f;
				}
				Vector3 vector2 = base.transform.position - this.lastPosition;
				float num = ((Time.deltaTime > Mathf.Epsilon) ? (1f / Time.deltaTime) : 0f);
				vector = vector2 * num;
				this.lastPosition = base.transform.position;
			}
			return Vector3.Dot(vector, axis.normalized);
		}

		// Token: 0x06007413 RID: 29715 RVA: 0x0025C18C File Offset: 0x0025A38C
		private void FireEvents(float currentAbsSpeed)
		{
			if (this.isLocal)
			{
				UnityEvent onFlickLocal = this.OnFlickLocal;
				if (onFlickLocal != null)
				{
					onFlickLocal.Invoke();
				}
			}
			UnityEvent onFlickShared = this.OnFlickShared;
			if (onFlickShared != null)
			{
				onFlickShared.Invoke();
			}
			if (this.maxSpeedThreshold > 0f)
			{
				float num = Mathf.InverseLerp(this.minSpeedThreshold, this.maxSpeedThreshold, currentAbsSpeed);
				UnityEvent<float> unityEvent = this.onFlickStrength;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(Mathf.Clamp01(num));
			}
		}

		// Token: 0x06007414 RID: 29716 RVA: 0x0025C1F9 File Offset: 0x0025A3F9
		private void ResetState()
		{
			this.lastPeakSign = 0;
			this.lastPeakSpeed = 0f;
			this.lastPeakTime = -9999f;
		}

		// Token: 0x040083BE RID: 33726
		[Header("Axis")]
		[Tooltip("Which single axis/direction to use for flick detection.\n- X/Y/Z use the axes defined by the Space settings below (Local vs World).\n- CustomForward uses axisReference.forward (ignores Space).")]
		[SerializeField]
		private CosmeticFlickReactor.AxisMode axisMode = CosmeticFlickReactor.AxisMode.Z;

		// Token: 0x040083BF RID: 33727
		[Tooltip("Used only when AxisMode = CustomForward. The forward/back of this transform defines the direction.")]
		[SerializeField]
		private Transform axisReference;

		// Token: 0x040083C0 RID: 33728
		[Header("Space")]
		[Tooltip("If enabled, X/Y/Z use world axes, otherwise local axes.\nUse Local for movement relative to the object’s facing.\nUse World for absolute directions independent of rotation.")]
		[SerializeField]
		private bool useWorldAxes;

		// Token: 0x040083C1 RID: 33729
		[Tooltip("Optional transform to define a custom world frame for X/Y/Z.\nIf assigned and Space is World, this transform’s Right/Up/Forward act as the world axes.\nIf not assigned, Unity’s global axes are used.")]
		[SerializeField]
		private Transform worldSpace;

		// Token: 0x040083C2 RID: 33730
		[Header("Velocity Source")]
		[Tooltip("Primary velocity tracker.")]
		[SerializeField]
		private SimpleSpeedTracker speedTracker;

		// Token: 0x040083C3 RID: 33731
		[Tooltip("Fallback velocity source if speedTracker is missing.")]
		[SerializeField]
		private Rigidbody rb;

		// Token: 0x040083C4 RID: 33732
		[Header("Thresholds")]
		[Tooltip("Minimum absolute signed speed along the chosen axis required to consider a object movement (m/s).")]
		[SerializeField]
		private float minSpeedThreshold = 2f;

		// Token: 0x040083C5 RID: 33733
		[Tooltip("Optional upper bound for mapping flick strength to 0–1.\nSet <= 0 to disable onFlickStrength.")]
		[SerializeField]
		private float maxSpeedThreshold;

		// Token: 0x040083C6 RID: 33734
		[Tooltip("How much back-and-forth reversal is required to register a flick.\nExample: 2.5 means => +1.3 then -1.2 within the window (|1.3| + |1.2| = 2.5).")]
		[SerializeField]
		private float directionChangeRequired = 2f;

		// Token: 0x040083C7 RID: 33735
		[Header("Timing")]
		[Tooltip("Max time allowed between the initial peak and its reversal (seconds).")]
		[SerializeField]
		private float flickWindowSeconds = 0.2f;

		// Token: 0x040083C8 RID: 33736
		[Tooltip("Buffer time after a successful flick during which no new flicks are allowed (seconds).")]
		[SerializeField]
		private float retriggerBufferSeconds = 0.15f;

		// Token: 0x040083C9 RID: 33737
		[Header("Events")]
		public UnityEvent OnFlickShared;

		// Token: 0x040083CA RID: 33738
		public UnityEvent OnFlickLocal;

		// Token: 0x040083CB RID: 33739
		public UnityEvent<float> onFlickStrength;

		// Token: 0x040083CC RID: 33740
		private Vector3 lastPosition;

		// Token: 0x040083CD RID: 33741
		private bool hasLastPosition;

		// Token: 0x040083CE RID: 33742
		private float lastPeakSpeed;

		// Token: 0x040083CF RID: 33743
		private float lastPeakTime = -999f;

		// Token: 0x040083D0 RID: 33744
		private int lastPeakSign;

		// Token: 0x040083D1 RID: 33745
		private float blockUntilTime;

		// Token: 0x040083D2 RID: 33746
		private VRRig rig;

		// Token: 0x040083D3 RID: 33747
		private bool isLocal;

		// Token: 0x020011D6 RID: 4566
		private enum AxisMode
		{
			// Token: 0x040083D5 RID: 33749
			X,
			// Token: 0x040083D6 RID: 33750
			Y,
			// Token: 0x040083D7 RID: 33751
			Z,
			// Token: 0x040083D8 RID: 33752
			CustomForward
		}
	}
}
