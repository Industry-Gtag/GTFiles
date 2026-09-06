using System;
using UnityEngine;

// Token: 0x02000E3B RID: 3643
public class LightningDispatcher : MonoBehaviour
{
	// Token: 0x1400009F RID: 159
	// (add) Token: 0x0600590D RID: 22797 RVA: 0x001CF194 File Offset: 0x001CD394
	// (remove) Token: 0x0600590E RID: 22798 RVA: 0x001CF1C8 File Offset: 0x001CD3C8
	public static event LightningDispatcher.DispatchLightningEvent RequestLightningStrike;

	// Token: 0x0600590F RID: 22799 RVA: 0x001CF1FC File Offset: 0x001CD3FC
	public void DispatchLightning(Vector3 p1, Vector3 p2)
	{
		if (LightningDispatcher.RequestLightningStrike != null)
		{
			LightningStrike lightningStrike = LightningDispatcher.RequestLightningStrike(p1, p2);
			float num = Mathf.Max(new float[]
			{
				base.transform.lossyScale.x,
				base.transform.lossyScale.y,
				base.transform.lossyScale.z
			});
			lightningStrike.Play(p1, p2, this.beamWidthCM * 0.01f * num, this.soundVolumeMultiplier / num, LightningStrike.rand.NextFloat(this.minDuration, this.maxDuration), this.colorOverLifetime);
		}
	}

	// Token: 0x0400693D RID: 26941
	[SerializeField]
	private float beamWidthCM = 1f;

	// Token: 0x0400693E RID: 26942
	[SerializeField]
	private float soundVolumeMultiplier = 1f;

	// Token: 0x0400693F RID: 26943
	[SerializeField]
	private float minDuration = 0.05f;

	// Token: 0x04006940 RID: 26944
	[SerializeField]
	private float maxDuration = 0.12f;

	// Token: 0x04006941 RID: 26945
	[SerializeField]
	private Gradient colorOverLifetime;

	// Token: 0x02000E3C RID: 3644
	// (Invoke) Token: 0x06005912 RID: 22802
	public delegate LightningStrike DispatchLightningEvent(Vector3 p1, Vector3 p2);
}
