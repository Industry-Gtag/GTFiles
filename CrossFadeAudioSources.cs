using System;
using UnityEngine;

// Token: 0x0200068B RID: 1675
public class CrossFadeAudioSources : MonoBehaviour, IRangedVariable<float>, IVariable<float>, IVariable
{
	// Token: 0x060029E7 RID: 10727 RVA: 0x000E23F9 File Offset: 0x000E05F9
	public void Play()
	{
		if (this.source1)
		{
			this.source1.Play();
		}
		if (this.source2)
		{
			this.source2.Play();
		}
	}

	// Token: 0x060029E8 RID: 10728 RVA: 0x000E242B File Offset: 0x000E062B
	public void Stop()
	{
		if (this.source1)
		{
			this.source1.Stop();
		}
		if (this.source2)
		{
			this.source2.Stop();
		}
	}

	// Token: 0x060029E9 RID: 10729 RVA: 0x000E2460 File Offset: 0x000E0660
	private void Update()
	{
		if (!this.source1 || !this.source2)
		{
			return;
		}
		float num = this._curve.Evaluate(this._lerp);
		float num2;
		if (this.tween)
		{
			num2 = MathUtils.Xlerp(this._lastT, num, Time.deltaTime, this.tweenSpeed);
		}
		else
		{
			num2 = (this.lerpByClipLength ? this._curve.Evaluate((float)this.source1.timeSamples / (float)this.source1.clip.samples) : num);
		}
		this._lastT = num2;
		this.source2.volume = num2;
		this.source1.volume = 1f - num2;
	}

	// Token: 0x060029EA RID: 10730 RVA: 0x000E2516 File Offset: 0x000E0716
	public float Get()
	{
		return this._lerp;
	}

	// Token: 0x060029EB RID: 10731 RVA: 0x000E251E File Offset: 0x000E071E
	public void Set(float f)
	{
		this._lerp = Mathf.Clamp01(f);
	}

	// Token: 0x17000437 RID: 1079
	// (get) Token: 0x060029EC RID: 10732 RVA: 0x000E252C File Offset: 0x000E072C
	// (set) Token: 0x060029ED RID: 10733 RVA: 0x00002C2D File Offset: 0x00000E2D
	public float Min
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	// Token: 0x17000438 RID: 1080
	// (get) Token: 0x060029EE RID: 10734 RVA: 0x000E2533 File Offset: 0x000E0733
	// (set) Token: 0x060029EF RID: 10735 RVA: 0x00002C2D File Offset: 0x00000E2D
	public float Max
	{
		get
		{
			return 1f;
		}
		set
		{
		}
	}

	// Token: 0x17000439 RID: 1081
	// (get) Token: 0x060029F0 RID: 10736 RVA: 0x000E2533 File Offset: 0x000E0733
	public float Range
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x1700043A RID: 1082
	// (get) Token: 0x060029F1 RID: 10737 RVA: 0x000E253A File Offset: 0x000E073A
	public AnimationCurve Curve
	{
		get
		{
			return this._curve;
		}
	}

	// Token: 0x0400367A RID: 13946
	[SerializeField]
	private float _lerp;

	// Token: 0x0400367B RID: 13947
	[SerializeField]
	private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400367C RID: 13948
	[Space]
	[SerializeField]
	private AudioSource source1;

	// Token: 0x0400367D RID: 13949
	[SerializeField]
	private AudioSource source2;

	// Token: 0x0400367E RID: 13950
	[Space]
	public bool lerpByClipLength;

	// Token: 0x0400367F RID: 13951
	public bool tween;

	// Token: 0x04003680 RID: 13952
	public float tweenSpeed = 16f;

	// Token: 0x04003681 RID: 13953
	private float _lastT;
}
