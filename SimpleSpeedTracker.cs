using System;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002F8 RID: 760
public class SimpleSpeedTracker : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x170001EB RID: 491
	// (get) Token: 0x06001361 RID: 4961 RVA: 0x000668A4 File Offset: 0x00064AA4
	private bool HasAxisFilter
	{
		get
		{
			return this.trackAxis > SimpleSpeedTracker.AxisFilter.None;
		}
	}

	// Token: 0x06001362 RID: 4962 RVA: 0x000668B0 File Offset: 0x00064AB0
	public void OnEnable()
	{
		if (this.target == null)
		{
			this.target = base.transform;
		}
		this.lastPos = this.target.position;
		this.lastSliceTime = Time.time;
		this.lastVelocity = Vector3.zero;
		this.lastRawSpeed = 0f;
		this.lastSpeed = 0f;
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001363 RID: 4963 RVA: 0x00019269 File Offset: 0x00017469
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001364 RID: 4964 RVA: 0x0006691C File Offset: 0x00064B1C
	public void SliceUpdate()
	{
		float num = Mathf.Max(1E-06f, Time.time - this.lastSliceTime);
		Vector3 position = this.target.position;
		Vector3 vector = (position - this.lastPos) / num;
		float num2;
		switch (this.trackAxis)
		{
		case SimpleSpeedTracker.AxisFilter.X:
			num2 = Vector3.Dot(vector, this.ResolveAxisRight());
			break;
		case SimpleSpeedTracker.AxisFilter.Y:
			num2 = Vector3.Dot(vector, this.ResolveAxisUp());
			break;
		case SimpleSpeedTracker.AxisFilter.Z:
			num2 = Vector3.Dot(vector, this.ResolveAxisForward());
			break;
		default:
			num2 = vector.magnitude;
			break;
		}
		this.lastSpeed = (this.useRawSpeed ? num2 : Mathf.Lerp(this.lastSpeed, num2, 1f - Mathf.Exp(-this.responsiveness * num)));
		float num3 = Mathf.Abs(this.lastSpeed);
		float num4 = this.postprocessCurve.Evaluate(num3);
		this.continuousProperties.ApplyAll(num4);
		float num5 = (this.useRawSpeed ? num2 : this.lastSpeed);
		UnityEvent<float> unityEvent = this.onSpeedUpdated;
		if (unityEvent != null)
		{
			unityEvent.Invoke(num5);
		}
		this.debugCurrentSpeed = num5;
		bool flag = Mathf.Abs(num5) >= this.eventThreshold;
		if (flag && !this.wasAboveThreshold)
		{
			UnityEvent unityEvent2 = this.onSpeedAboveThreshold;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke();
			}
		}
		else if (!flag && this.wasAboveThreshold)
		{
			UnityEvent unityEvent3 = this.onSpeedBelowThreshold;
			if (unityEvent3 != null)
			{
				unityEvent3.Invoke();
			}
		}
		this.wasAboveThreshold = flag;
		if (this.HasAxisFilter)
		{
			bool flag2 = num5 >= this.positiveThreshold;
			if (flag2 && !this.wasMovingPositive)
			{
				UnityEvent unityEvent4 = this.onAbovePositiveThreshold;
				if (unityEvent4 != null)
				{
					unityEvent4.Invoke();
				}
			}
			else if (!flag2 && this.wasMovingPositive)
			{
				UnityEvent unityEvent5 = this.onBelowPositiveThreshold;
				if (unityEvent5 != null)
				{
					unityEvent5.Invoke();
				}
			}
			this.wasMovingPositive = flag2;
			bool flag3 = num5 <= this.negativeThreshold;
			if (flag3 && !this.wasMovingNegative)
			{
				UnityEvent unityEvent6 = this.onAboveNegativeThreshold;
				if (unityEvent6 != null)
				{
					unityEvent6.Invoke();
				}
			}
			else if (!flag3 && this.wasMovingNegative)
			{
				UnityEvent unityEvent7 = this.onBelowNegativeThreshold;
				if (unityEvent7 != null)
				{
					unityEvent7.Invoke();
				}
			}
			this.wasMovingNegative = flag3;
		}
		this.lastVelocity = vector;
		this.lastRawSpeed = num2;
		this.lastPos = position;
		this.lastSliceTime = Time.time;
	}

	// Token: 0x06001365 RID: 4965 RVA: 0x00066B64 File Offset: 0x00064D64
	public float GetPostProcessSpeed()
	{
		return this.postprocessCurve.Evaluate(Mathf.Abs(this.lastSpeed));
	}

	// Token: 0x06001366 RID: 4966 RVA: 0x00066B7C File Offset: 0x00064D7C
	public float GetRawSpeed()
	{
		return this.lastRawSpeed;
	}

	// Token: 0x06001367 RID: 4967 RVA: 0x00066B84 File Offset: 0x00064D84
	public Vector3 GetWorldVelocity()
	{
		return this.lastVelocity;
	}

	// Token: 0x06001368 RID: 4968 RVA: 0x00066B8C File Offset: 0x00064D8C
	public Vector3 GetLocalVelocity()
	{
		if (this.useWorldAxes)
		{
			return this.lastVelocity;
		}
		if (this.target != null)
		{
			return this.target.InverseTransformDirection(this.lastVelocity);
		}
		return base.transform.InverseTransformDirection(this.lastVelocity);
	}

	// Token: 0x06001369 RID: 4969 RVA: 0x00066BD9 File Offset: 0x00064DD9
	public float GetSignedSpeedAlongForward(Transform reference)
	{
		if (reference == null)
		{
			return 0f;
		}
		return Vector3.Dot(this.lastVelocity, reference.forward);
	}

	// Token: 0x0600136A RID: 4970 RVA: 0x00066BFB File Offset: 0x00064DFB
	public float GetSignedSpeedX()
	{
		return Vector3.Dot(this.lastVelocity, this.ResolveAxisRight());
	}

	// Token: 0x0600136B RID: 4971 RVA: 0x00066C0E File Offset: 0x00064E0E
	public float GetSignedSpeedY()
	{
		return Vector3.Dot(this.lastVelocity, this.ResolveAxisUp());
	}

	// Token: 0x0600136C RID: 4972 RVA: 0x00066C21 File Offset: 0x00064E21
	public float GetSignedSpeedZ()
	{
		return Vector3.Dot(this.lastVelocity, this.ResolveAxisForward());
	}

	// Token: 0x0600136D RID: 4973 RVA: 0x00066C34 File Offset: 0x00064E34
	public Vector3 GetVelocityInAxisSpace()
	{
		Vector3 vector = this.ResolveAxisRight();
		Vector3 vector2 = this.ResolveAxisUp();
		Vector3 vector3 = this.ResolveAxisForward();
		return new Vector3(Vector3.Dot(this.lastVelocity, vector), Vector3.Dot(this.lastVelocity, vector2), Vector3.Dot(this.lastVelocity, vector3));
	}

	// Token: 0x0600136E RID: 4974 RVA: 0x00066C80 File Offset: 0x00064E80
	private Vector3 ResolveAxisRight()
	{
		if (this.useWorldAxes)
		{
			if (this.worldSpace != null)
			{
				return this.worldSpace.right;
			}
			return Vector3.right;
		}
		else
		{
			if (!(this.target != null))
			{
				return base.transform.right;
			}
			return this.target.right;
		}
	}

	// Token: 0x0600136F RID: 4975 RVA: 0x00066CDC File Offset: 0x00064EDC
	private Vector3 ResolveAxisUp()
	{
		if (this.useWorldAxes)
		{
			if (this.worldSpace != null)
			{
				return this.worldSpace.up;
			}
			return Vector3.up;
		}
		else
		{
			if (!(this.target != null))
			{
				return base.transform.up;
			}
			return this.target.up;
		}
	}

	// Token: 0x06001370 RID: 4976 RVA: 0x00066D38 File Offset: 0x00064F38
	private Vector3 ResolveAxisForward()
	{
		if (this.useWorldAxes)
		{
			if (this.worldSpace != null)
			{
				return this.worldSpace.forward;
			}
			return Vector3.forward;
		}
		else
		{
			if (!(this.target != null))
			{
				return base.transform.forward;
			}
			return this.target.forward;
		}
	}

	// Token: 0x040017A3 RID: 6051
	[Header("Settings")]
	[Tooltip("Transform whose movement speed is tracked. If left empty, uses this object’s transform.")]
	[SerializeField]
	private Transform target;

	// Token: 0x040017A4 RID: 6052
	[Tooltip("If enabled, speed and direction calculations use world (global) space, otherwise local space.\nUse Local Space when you want speed relative to the object’s facing direction (e.g., how fast a sword swings forward)")]
	[SerializeField]
	private bool useWorldAxes;

	// Token: 0x040017A5 RID: 6053
	[Tooltip("Optional transform defining a custom world reference.\nIf set, that transform’s Right/Up/Forward axes are treated as world axes.\nIf left empty, Unity’s global world axes are used.")]
	[SerializeField]
	private Transform worldSpace;

	// Token: 0x040017A6 RID: 6054
	[Tooltip("If true, uses raw instantaneous speed without smoothing.\nIf false, smooths speed using the Responsiveness setting below.")]
	[SerializeField]
	private bool useRawSpeed;

	// Token: 0x040017A7 RID: 6055
	[SerializeField]
	private float responsiveness = 10f;

	// Token: 0x040017A8 RID: 6056
	[SerializeField]
	private AnimationCurve postprocessCurve = AnimationCurve.Linear(0f, 0f, 10f, 10f);

	// Token: 0x040017A9 RID: 6057
	[Header("Axis Filter")]
	[Tooltip("Optionally restrict speed tracking to a single axis.\nWhen set, speed is signed: positive = moving along the axis, negative = moving against it.\nAxes are resolved using the Space settings above (Local vs World).")]
	[SerializeField]
	private SimpleSpeedTracker.AxisFilter trackAxis;

	// Token: 0x040017AA RID: 6058
	[Header("Property Output")]
	[SerializeField]
	private ContinuousPropertyArray continuousProperties;

	// Token: 0x040017AB RID: 6059
	[Header("Events")]
	[Tooltip("Speed threshold used to trigger the Above/Below events.\nWhen an axis filter is set, this compares against absolute speed on that axis.")]
	[SerializeField]
	private float eventThreshold = 1f;

	// Token: 0x040017AC RID: 6060
	public UnityEvent<float> onSpeedUpdated;

	// Token: 0x040017AD RID: 6061
	public UnityEvent onSpeedAboveThreshold;

	// Token: 0x040017AE RID: 6062
	public UnityEvent onSpeedBelowThreshold;

	// Token: 0x040017AF RID: 6063
	[Tooltip("Signed speed along the positive axis direction required to fire onAbovePositiveThreshold / onBelowPositiveThreshold.")]
	[SerializeField]
	private float positiveThreshold = 2f;

	// Token: 0x040017B0 RID: 6064
	public UnityEvent onAbovePositiveThreshold;

	// Token: 0x040017B1 RID: 6065
	public UnityEvent onBelowPositiveThreshold;

	// Token: 0x040017B2 RID: 6066
	[Tooltip("Signed speed threshold for the negative axis direction. Enter as a negative number.\nFires onAboveNegativeThreshold / onBelowNegativeThreshold when signed speed crosses this value.")]
	[SerializeField]
	private float negativeThreshold = -2f;

	// Token: 0x040017B3 RID: 6067
	public UnityEvent onAboveNegativeThreshold;

	// Token: 0x040017B4 RID: 6068
	public UnityEvent onBelowNegativeThreshold;

	// Token: 0x040017B5 RID: 6069
	[Header("Debug")]
	[Tooltip("Current displayed speed value (raw or smoothed). Signed when Axis Filter is set.")]
	public float debugCurrentSpeed;

	// Token: 0x040017B6 RID: 6070
	private float lastSpeed;

	// Token: 0x040017B7 RID: 6071
	private float lastRawSpeed;

	// Token: 0x040017B8 RID: 6072
	private Vector3 lastVelocity;

	// Token: 0x040017B9 RID: 6073
	private Vector3 lastPos;

	// Token: 0x040017BA RID: 6074
	private float lastSliceTime;

	// Token: 0x040017BB RID: 6075
	private bool wasAboveThreshold;

	// Token: 0x040017BC RID: 6076
	private bool wasMovingPositive;

	// Token: 0x040017BD RID: 6077
	private bool wasMovingNegative;

	// Token: 0x020002F9 RID: 761
	private enum AxisFilter
	{
		// Token: 0x040017BF RID: 6079
		None,
		// Token: 0x040017C0 RID: 6080
		X,
		// Token: 0x040017C1 RID: 6081
		Y,
		// Token: 0x040017C2 RID: 6082
		Z
	}
}
