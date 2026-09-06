using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020009D8 RID: 2520
public class RadialBounds : MonoBehaviour
{
	// Token: 0x1700060C RID: 1548
	// (get) Token: 0x060040A9 RID: 16553 RVA: 0x001587B2 File Offset: 0x001569B2
	// (set) Token: 0x060040AA RID: 16554 RVA: 0x001587BA File Offset: 0x001569BA
	public Vector3 localCenter
	{
		get
		{
			return this._localCenter;
		}
		set
		{
			this._localCenter = value;
		}
	}

	// Token: 0x1700060D RID: 1549
	// (get) Token: 0x060040AB RID: 16555 RVA: 0x001587C3 File Offset: 0x001569C3
	// (set) Token: 0x060040AC RID: 16556 RVA: 0x001587CB File Offset: 0x001569CB
	public float localRadius
	{
		get
		{
			return this._localRadius;
		}
		set
		{
			this._localRadius = value;
		}
	}

	// Token: 0x1700060E RID: 1550
	// (get) Token: 0x060040AD RID: 16557 RVA: 0x001587D4 File Offset: 0x001569D4
	public Vector3 center
	{
		get
		{
			return base.transform.TransformPoint(this._localCenter);
		}
	}

	// Token: 0x1700060F RID: 1551
	// (get) Token: 0x060040AE RID: 16558 RVA: 0x001587E7 File Offset: 0x001569E7
	public float radius
	{
		get
		{
			return MathUtils.GetScaledRadius(this._localRadius, base.transform.lossyScale);
		}
	}

	// Token: 0x0400512E RID: 20782
	[SerializeField]
	private Vector3 _localCenter;

	// Token: 0x0400512F RID: 20783
	[SerializeField]
	private float _localRadius = 1f;

	// Token: 0x04005130 RID: 20784
	[Space]
	public UnityEvent<RadialBounds> onOverlapEnter;

	// Token: 0x04005131 RID: 20785
	public UnityEvent<RadialBounds> onOverlapExit;

	// Token: 0x04005132 RID: 20786
	public UnityEvent<RadialBounds, float> onOverlapStay;
}
