using System;
using UnityEngine;

// Token: 0x020004E8 RID: 1256
public class RotationAnimation : MonoBehaviour, ITickSystemTick
{
	// Token: 0x17000334 RID: 820
	// (get) Token: 0x06001E8B RID: 7819 RVA: 0x000A3808 File Offset: 0x000A1A08
	// (set) Token: 0x06001E8C RID: 7820 RVA: 0x000A3810 File Offset: 0x000A1A10
	public bool TickRunning { get; set; }

	// Token: 0x06001E8D RID: 7821 RVA: 0x000A381C File Offset: 0x000A1A1C
	public void Tick()
	{
		Vector3 vector = Vector3.zero;
		vector.x = this.amplitude.x * this.x.Evaluate((Time.time - this.baseTime) * this.period.x % 1f);
		vector.y = this.amplitude.y * this.y.Evaluate((Time.time - this.baseTime) * this.period.y % 1f);
		vector.z = this.amplitude.z * this.z.Evaluate((Time.time - this.baseTime) * this.period.z % 1f);
		if (this.releaseSet)
		{
			float num = this.release.Evaluate(Time.time - this.releaseTime);
			vector *= num;
			if (num < Mathf.Epsilon)
			{
				base.enabled = false;
			}
		}
		base.transform.localRotation = Quaternion.Euler(vector) * this.baseRotation;
	}

	// Token: 0x06001E8E RID: 7822 RVA: 0x000A3936 File Offset: 0x000A1B36
	private void Awake()
	{
		this.baseRotation = base.transform.localRotation;
	}

	// Token: 0x06001E8F RID: 7823 RVA: 0x000A3949 File Offset: 0x000A1B49
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
		this.releaseSet = false;
		this.baseTime = Time.time;
	}

	// Token: 0x06001E90 RID: 7824 RVA: 0x000A3963 File Offset: 0x000A1B63
	public void ReleaseToDisable()
	{
		this.releaseSet = true;
		this.releaseTime = Time.time;
	}

	// Token: 0x06001E91 RID: 7825 RVA: 0x000A3977 File Offset: 0x000A1B77
	public void CancelRelease()
	{
		this.releaseSet = false;
	}

	// Token: 0x06001E92 RID: 7826 RVA: 0x000A3980 File Offset: 0x000A1B80
	private void OnDisable()
	{
		base.transform.localRotation = this.baseRotation;
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x040028C1 RID: 10433
	[SerializeField]
	private AnimationCurve x;

	// Token: 0x040028C2 RID: 10434
	[SerializeField]
	private AnimationCurve y;

	// Token: 0x040028C3 RID: 10435
	[SerializeField]
	private AnimationCurve z;

	// Token: 0x040028C4 RID: 10436
	[SerializeField]
	private AnimationCurve attack;

	// Token: 0x040028C5 RID: 10437
	[SerializeField]
	private AnimationCurve release;

	// Token: 0x040028C6 RID: 10438
	[SerializeField]
	private Vector3 amplitude = Vector3.one;

	// Token: 0x040028C7 RID: 10439
	[SerializeField]
	private Vector3 period = Vector3.one;

	// Token: 0x040028C8 RID: 10440
	private Quaternion baseRotation;

	// Token: 0x040028C9 RID: 10441
	private float baseTime;

	// Token: 0x040028CA RID: 10442
	private float releaseTime;

	// Token: 0x040028CB RID: 10443
	private bool releaseSet;
}
