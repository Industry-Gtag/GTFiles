using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000BFF RID: 3071
public class LightningManager : MonoBehaviour
{
	// Token: 0x06004CF6 RID: 19702 RVA: 0x0019A9D6 File Offset: 0x00198BD6
	private void Start()
	{
		this.lightningAudio = base.GetComponent<AudioSource>();
		GorillaComputer instance = GorillaComputer.instance;
		instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
	}

	// Token: 0x06004CF7 RID: 19703 RVA: 0x0019AA0C File Offset: 0x00198C0C
	private void OnTimeChanged()
	{
		this.InitializeRng();
		if (this.lightningRunner != null)
		{
			base.StopCoroutine(this.lightningRunner);
		}
		this.lightningRunner = base.StartCoroutine(this.LightningEffectRunner());
	}

	// Token: 0x06004CF8 RID: 19704 RVA: 0x0019AA3C File Offset: 0x00198C3C
	private void GetHourStart(out long seed, out float timestampRealtime)
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		DateTime dateTime = new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, serverTime.Hour, 0, 0);
		timestampRealtime = Time.realtimeSinceStartup - (float)(serverTime - dateTime).TotalSeconds;
		seed = dateTime.Ticks;
	}

	// Token: 0x06004CF9 RID: 19705 RVA: 0x0019AA9C File Offset: 0x00198C9C
	private void InitializeRng()
	{
		long num;
		float num2;
		this.GetHourStart(out num, out num2);
		this.currentHourlySeed = num;
		this.rng = new SRand(num);
		this.lightningTimestampsRealtime.Clear();
		this.nextLightningTimestampIndex = -1;
		float num3 = num2;
		float num4 = 0f;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		while (num4 < 3600f)
		{
			float num5 = this.rng.NextFloat(this.minTimeBetweenFlashes, this.maxTimeBetweenFlashes);
			num4 += num5;
			num3 += num5;
			if (this.nextLightningTimestampIndex == -1 && num3 > realtimeSinceStartup)
			{
				this.nextLightningTimestampIndex = this.lightningTimestampsRealtime.Count;
			}
			this.lightningTimestampsRealtime.Add(num3);
		}
		this.lightningTimestampsRealtime[this.lightningTimestampsRealtime.Count - 1] = num2 + 3605f;
	}

	// Token: 0x06004CFA RID: 19706 RVA: 0x0019AB60 File Offset: 0x00198D60
	internal void DoLightningStrike()
	{
		BetterDayNightManager.instance.AnimateLightFlash(this.lightMapIndex, this.flashFadeInDuration, this.flashHoldDuration, this.flashFadeOutDuration);
		this.lightningAudio.clip = (ZoneManagement.IsInZone(GTZone.cave) ? this.muffledLightning : this.regularLightning);
		this.lightningAudio.GTPlay();
	}

	// Token: 0x06004CFB RID: 19707 RVA: 0x0019ABBD File Offset: 0x00198DBD
	private IEnumerator LightningEffectRunner()
	{
		for (;;)
		{
			if (this.lightningTimestampsRealtime.Count <= this.nextLightningTimestampIndex)
			{
				this.InitializeRng();
			}
			if (this.lightningTimestampsRealtime.Count > this.nextLightningTimestampIndex)
			{
				yield return new WaitForSecondsRealtime(this.lightningTimestampsRealtime[this.nextLightningTimestampIndex] - Time.realtimeSinceStartup);
				float num = this.lightningTimestampsRealtime[this.nextLightningTimestampIndex];
				this.nextLightningTimestampIndex++;
				if (Time.realtimeSinceStartup - num < 1f && this.lightningTimestampsRealtime.Count > this.nextLightningTimestampIndex)
				{
					this.DoLightningStrike();
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x0400604B RID: 24651
	public int lightMapIndex;

	// Token: 0x0400604C RID: 24652
	public float minTimeBetweenFlashes;

	// Token: 0x0400604D RID: 24653
	public float maxTimeBetweenFlashes;

	// Token: 0x0400604E RID: 24654
	public float flashFadeInDuration;

	// Token: 0x0400604F RID: 24655
	public float flashHoldDuration;

	// Token: 0x04006050 RID: 24656
	public float flashFadeOutDuration;

	// Token: 0x04006051 RID: 24657
	private AudioSource lightningAudio;

	// Token: 0x04006052 RID: 24658
	private SRand rng;

	// Token: 0x04006053 RID: 24659
	private long currentHourlySeed;

	// Token: 0x04006054 RID: 24660
	private List<float> lightningTimestampsRealtime = new List<float>();

	// Token: 0x04006055 RID: 24661
	private int nextLightningTimestampIndex;

	// Token: 0x04006056 RID: 24662
	public AudioClip regularLightning;

	// Token: 0x04006057 RID: 24663
	public AudioClip muffledLightning;

	// Token: 0x04006058 RID: 24664
	private Coroutine lightningRunner;
}
