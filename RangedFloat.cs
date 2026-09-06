using System;
using UnityEngine;

// Token: 0x020009DC RID: 2524
public class RangedFloat : MonoBehaviour, IRangedVariable<float>, IVariable<float>, IVariable
{
	// Token: 0x17000610 RID: 1552
	// (get) Token: 0x060040BB RID: 16571 RVA: 0x00158D1B File Offset: 0x00156F1B
	public AnimationCurve Curve
	{
		get
		{
			return this._curve;
		}
	}

	// Token: 0x17000611 RID: 1553
	// (get) Token: 0x060040BC RID: 16572 RVA: 0x00158D23 File Offset: 0x00156F23
	public float Range
	{
		get
		{
			return this._max - this._min;
		}
	}

	// Token: 0x17000612 RID: 1554
	// (get) Token: 0x060040BD RID: 16573 RVA: 0x00158D32 File Offset: 0x00156F32
	// (set) Token: 0x060040BE RID: 16574 RVA: 0x00158D3A File Offset: 0x00156F3A
	public float Min
	{
		get
		{
			return this._min;
		}
		set
		{
			this._min = value;
		}
	}

	// Token: 0x17000613 RID: 1555
	// (get) Token: 0x060040BF RID: 16575 RVA: 0x00158D43 File Offset: 0x00156F43
	// (set) Token: 0x060040C0 RID: 16576 RVA: 0x00158D4B File Offset: 0x00156F4B
	public float Max
	{
		get
		{
			return this._max;
		}
		set
		{
			this._max = value;
		}
	}

	// Token: 0x17000614 RID: 1556
	// (get) Token: 0x060040C1 RID: 16577 RVA: 0x00158D54 File Offset: 0x00156F54
	// (set) Token: 0x060040C2 RID: 16578 RVA: 0x00158D89 File Offset: 0x00156F89
	public float normalized
	{
		get
		{
			if (!this.Range.Approx0(1E-06f))
			{
				return (this._value - this._min) / (this._max - this.Min);
			}
			return 0f;
		}
		set
		{
			this._value = this._min + Mathf.Clamp01(value) * (this._max - this._min);
		}
	}

	// Token: 0x17000615 RID: 1557
	// (get) Token: 0x060040C3 RID: 16579 RVA: 0x00158DAC File Offset: 0x00156FAC
	public float curved
	{
		get
		{
			return this._min + this._curve.Evaluate(this.normalized) * (this._max - this._min);
		}
	}

	// Token: 0x060040C4 RID: 16580 RVA: 0x00158DD4 File Offset: 0x00156FD4
	public float Get()
	{
		return this._value;
	}

	// Token: 0x060040C5 RID: 16581 RVA: 0x00158DDC File Offset: 0x00156FDC
	public void Set(float f)
	{
		this._value = Mathf.Clamp(f, this._min, this._max);
	}

	// Token: 0x04005148 RID: 20808
	[SerializeField]
	private float _value = 0.5f;

	// Token: 0x04005149 RID: 20809
	[SerializeField]
	private float _min;

	// Token: 0x0400514A RID: 20810
	[SerializeField]
	private float _max = 1f;

	// Token: 0x0400514B RID: 20811
	[SerializeField]
	private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
}
