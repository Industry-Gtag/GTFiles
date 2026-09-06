using System;
using UnityEngine;

// Token: 0x020007B4 RID: 1972
public class GRFadeAndDestroyLight : MonoBehaviour
{
	// Token: 0x06003277 RID: 12919 RVA: 0x0011489F File Offset: 0x00112A9F
	private void Start()
	{
		if (this.gameLight != null)
		{
			this.fadeRate = this.gameLight.light.intensity / this.TimeToFade;
		}
		this.timeSinceLastUpdate = Time.time;
	}

	// Token: 0x06003278 RID: 12920 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEnable()
	{
	}

	// Token: 0x06003279 RID: 12921 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnDisable()
	{
	}

	// Token: 0x0600327A RID: 12922 RVA: 0x001148D8 File Offset: 0x00112AD8
	public void Update()
	{
		if (Time.time < this.timeSinceLastUpdate || Time.time > this.timeSinceLastUpdate + this.timeSlice)
		{
			this.timeSinceLastUpdate = Time.time;
			float num = this.gameLight.light.intensity;
			num -= this.timeSlice * this.fadeRate;
			if (num <= 0f)
			{
				base.gameObject.Destroy();
				return;
			}
			this.gameLight.light.intensity = num;
		}
	}

	// Token: 0x0400415C RID: 16732
	public float TimeToFade = 10f;

	// Token: 0x0400415D RID: 16733
	private float fadeRate;

	// Token: 0x0400415E RID: 16734
	public GameLight gameLight;

	// Token: 0x0400415F RID: 16735
	public float timeSlice = 0.1f;

	// Token: 0x04004160 RID: 16736
	public float timeSinceLastUpdate;
}
