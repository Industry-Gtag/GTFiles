using System;
using System.Linq;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020005C6 RID: 1478
public sealed class FlickerManager : MonoBehaviour
{
	// Token: 0x06002542 RID: 9538 RVA: 0x000C7824 File Offset: 0x000C5A24
	private void Awake()
	{
		if (this.FlickerDurations.Length % 2 != 0)
		{
			Debug.LogWarning("FlickerManager should have an even number of steps; removing last entry.");
			this.FlickerDurations = this.FlickerDurations.Take(this.FlickerDurations.Length - 1).ToArray<float>();
		}
		if (this.FlickerDurations.Length == 0)
		{
			Debug.LogWarning("No flicker durations set for FlickerManager, disabling.");
			Object.Destroy(this);
			return;
		}
	}

	// Token: 0x06002543 RID: 9539 RVA: 0x000C7884 File Offset: 0x000C5A84
	private void Update()
	{
		float serverTime = FlickerManager.GetServerTime();
		if (serverTime < this._nextFlickerTime)
		{
			return;
		}
		BetterDayNightManager.instance.AnimateLightFlash(this.LightmapIndex, this.FlickerFadeInDuration, this.FlickerDurations[this._flickerIndex], this.FlickerFadeOutDuration);
		this._nextFlickerTime = serverTime + this.FlickerDurations[this._flickerIndex + 1];
		this._flickerIndex = (this._flickerIndex + 2) % this.FlickerDurations.Length;
	}

	// Token: 0x06002544 RID: 9540 RVA: 0x000C78FC File Offset: 0x000C5AFC
	private static float GetServerTime()
	{
		return (float)(GorillaComputer.instance.GetServerTime() - GorillaComputer.instance.startupTime).TotalSeconds;
	}

	// Token: 0x040030CC RID: 12492
	public float[] FlickerDurations;

	// Token: 0x040030CD RID: 12493
	public float FlickerFadeInDuration;

	// Token: 0x040030CE RID: 12494
	public float FlickerFadeOutDuration;

	// Token: 0x040030CF RID: 12495
	public int LightmapIndex;

	// Token: 0x040030D0 RID: 12496
	private int _flickerIndex;

	// Token: 0x040030D1 RID: 12497
	private float _nextFlickerTime = float.MinValue;
}
