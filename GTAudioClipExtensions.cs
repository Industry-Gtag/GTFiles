using System;
using UnityEngine;

// Token: 0x02000334 RID: 820
public static class GTAudioClipExtensions
{
	// Token: 0x06001447 RID: 5191 RVA: 0x0006D974 File Offset: 0x0006BB74
	public static float GetPeakMagnitude(this AudioClip audioClip)
	{
		if (audioClip == null)
		{
			return 0f;
		}
		float num = float.NegativeInfinity;
		float[] array = new float[audioClip.samples];
		audioClip.GetData(array, 0);
		foreach (float num2 in array)
		{
			num = Mathf.Max(num, Mathf.Abs(num2));
		}
		return num;
	}

	// Token: 0x06001448 RID: 5192 RVA: 0x0006D9D0 File Offset: 0x0006BBD0
	public static float GetRMSMagnitude(this AudioClip audioClip)
	{
		if (audioClip == null)
		{
			return 0f;
		}
		float num = 0f;
		float[] array = new float[audioClip.samples];
		audioClip.GetData(array, 0);
		foreach (float num2 in array)
		{
			num += num2 * num2;
		}
		return Mathf.Sqrt(num / (float)array.Length);
	}
}
