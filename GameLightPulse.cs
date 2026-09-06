using System;
using UnityEngine;

// Token: 0x020006F0 RID: 1776
public class GameLightPulse : GameLight, IGorillaSliceableSimple
{
	// Token: 0x06002CB4 RID: 11444 RVA: 0x000F168E File Offset: 0x000EF88E
	public new void Awake()
	{
		base.Awake();
		this.startingIntensity = this.light.intensity;
		this.offsetTime = Random.value / this.frequency;
	}

	// Token: 0x06002CB5 RID: 11445 RVA: 0x000F16B9 File Offset: 0x000EF8B9
	protected new void OnEnable()
	{
		base.OnEnable();
		GorillaSlicerSimpleManager.RegisterSliceable(this);
	}

	// Token: 0x06002CB6 RID: 11446 RVA: 0x000F16C7 File Offset: 0x000EF8C7
	protected new void OnDisable()
	{
		base.OnDisable();
		GorillaSlicerSimpleManager.UnregisterSliceable(this);
	}

	// Token: 0x06002CB7 RID: 11447 RVA: 0x000F16D8 File Offset: 0x000EF8D8
	public void SliceUpdate()
	{
		this.light.intensity = this.startingIntensity / 2f * Mathf.Sin((Time.time + this.offsetTime) * this.frequency * 2f * 3.1415927f % 6.2831855f) + this.startingIntensity / 2f;
	}

	// Token: 0x0400393C RID: 14652
	private float startingIntensity;

	// Token: 0x0400393D RID: 14653
	public float frequency;

	// Token: 0x0400393E RID: 14654
	private float offsetTime;
}
