using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000B13 RID: 2835
public class ProximityReactor : MonoBehaviour
{
	// Token: 0x170006DF RID: 1759
	// (get) Token: 0x06004883 RID: 18563 RVA: 0x00185F2D File Offset: 0x0018412D
	public float proximityRange
	{
		get
		{
			return this.proximityMax - this.proximityMin;
		}
	}

	// Token: 0x170006E0 RID: 1760
	// (get) Token: 0x06004884 RID: 18564 RVA: 0x00185F3C File Offset: 0x0018413C
	public float distance
	{
		get
		{
			return this._distance;
		}
	}

	// Token: 0x170006E1 RID: 1761
	// (get) Token: 0x06004885 RID: 18565 RVA: 0x00185F44 File Offset: 0x00184144
	public float distanceLinear
	{
		get
		{
			return this._distanceLinear;
		}
	}

	// Token: 0x06004886 RID: 18566 RVA: 0x00185F4C File Offset: 0x0018414C
	public void SetRigFrom()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
		if (componentInParent != null)
		{
			this.from = componentInParent.transform;
		}
	}

	// Token: 0x06004887 RID: 18567 RVA: 0x00185F78 File Offset: 0x00184178
	public void SetRigTo()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
		if (componentInParent != null)
		{
			this.to = componentInParent.transform;
		}
	}

	// Token: 0x06004888 RID: 18568 RVA: 0x00185FA2 File Offset: 0x001841A2
	public void SetTransformFrom(Transform t)
	{
		this.from = t;
	}

	// Token: 0x06004889 RID: 18569 RVA: 0x00185FAB File Offset: 0x001841AB
	public void SetTransformTo(Transform t)
	{
		this.to = t;
	}

	// Token: 0x0600488A RID: 18570 RVA: 0x00185FB4 File Offset: 0x001841B4
	private void Setup()
	{
		this._distance = 0f;
		this._distanceLinear = 0f;
	}

	// Token: 0x0600488B RID: 18571 RVA: 0x00185FCC File Offset: 0x001841CC
	private void OnEnable()
	{
		this.Setup();
	}

	// Token: 0x0600488C RID: 18572 RVA: 0x00185FD4 File Offset: 0x001841D4
	private void Update()
	{
		if (!this.from || !this.to)
		{
			this._distance = 0f;
			this._distanceLinear = 0f;
			return;
		}
		Vector3 position = this.from.position;
		float magnitude = (this.to.position - position).magnitude;
		if (!this._distance.Approx(magnitude, 1E-06f))
		{
			UnityEvent<float> unityEvent = this.onProximityChanged;
			if (unityEvent != null)
			{
				unityEvent.Invoke(magnitude);
			}
		}
		this._distance = magnitude;
		float num = (this.proximityRange.Approx0(1E-06f) ? 0f : MathUtils.LinearUnclamped(magnitude, this.proximityMin, this.proximityMax, 0f, 1f));
		if (!this._distanceLinear.Approx(num, 1E-06f))
		{
			UnityEvent<float> unityEvent2 = this.onProximityChangedLinear;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(num);
			}
		}
		this._distanceLinear = num;
		if (this._distanceLinear < 0f)
		{
			UnityEvent<float> unityEvent3 = this.onBelowMinProximity;
			if (unityEvent3 != null)
			{
				unityEvent3.Invoke(magnitude);
			}
		}
		if (this._distanceLinear > 1f)
		{
			UnityEvent<float> unityEvent4 = this.onAboveMaxProximity;
			if (unityEvent4 == null)
			{
				return;
			}
			unityEvent4.Invoke(magnitude);
		}
	}

	// Token: 0x04005B0F RID: 23311
	public Transform from;

	// Token: 0x04005B10 RID: 23312
	public Transform to;

	// Token: 0x04005B11 RID: 23313
	[Space]
	public float proximityMin;

	// Token: 0x04005B12 RID: 23314
	public float proximityMax = 1f;

	// Token: 0x04005B13 RID: 23315
	[Space]
	[NonSerialized]
	private float _distance;

	// Token: 0x04005B14 RID: 23316
	[NonSerialized]
	private float _distanceLinear;

	// Token: 0x04005B15 RID: 23317
	[Space]
	public UnityEvent<float> onProximityChanged;

	// Token: 0x04005B16 RID: 23318
	public UnityEvent<float> onProximityChangedLinear;

	// Token: 0x04005B17 RID: 23319
	[Space]
	public UnityEvent<float> onBelowMinProximity;

	// Token: 0x04005B18 RID: 23320
	public UnityEvent<float> onAboveMaxProximity;
}
