using System;
using System.Collections;
using System.Threading;
using UnityEngine;

// Token: 0x0200086C RID: 2156
public class GorillaDayNight : MonoBehaviour
{
	// Token: 0x060037D0 RID: 14288 RVA: 0x001318D8 File Offset: 0x0012FAD8
	public void Awake()
	{
		if (GorillaDayNight.instance == null)
		{
			GorillaDayNight.instance = this;
		}
		else if (GorillaDayNight.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.test = false;
		this.working = false;
		this.lerpValue = 0.5f;
		this.workingLightMapDatas = new LightmapData[3];
		this.workingLightMapData = new LightmapData();
		this.workingLightMapData.lightmapColor = this.lightmapDatas[0].lightTextures[0];
		this.workingLightMapData.lightmapDir = this.lightmapDatas[0].dirTextures[0];
	}

	// Token: 0x060037D1 RID: 14289 RVA: 0x0013197C File Offset: 0x0012FB7C
	public void Update()
	{
		if (this.test)
		{
			this.test = false;
			base.StartCoroutine(this.LightMapSet(this.firstData, this.secondData, this.lerpValue));
		}
	}

	// Token: 0x060037D2 RID: 14290 RVA: 0x001319AC File Offset: 0x0012FBAC
	public void DoWork()
	{
		this.k = 0;
		while (this.k < this.lightmapDatas[this.firstData].lights.Length)
		{
			this.fromPixels = this.lightmapDatas[this.firstData].lights[this.k];
			this.toPixels = this.lightmapDatas[this.secondData].lights[this.k];
			this.mixedPixels = this.fromPixels;
			this.j = 0;
			while (this.j < this.mixedPixels.Length)
			{
				this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
				this.j++;
			}
			this.workingLightMapData.lightmapColor.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.fromPixels = this.lightmapDatas[this.firstData].dirs[this.k];
			this.toPixels = this.lightmapDatas[this.secondData].dirs[this.k];
			this.mixedPixels = this.fromPixels;
			this.j = 0;
			while (this.j < this.mixedPixels.Length)
			{
				this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
				this.j++;
			}
			this.workingLightMapData.lightmapDir.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.workingLightMapDatas[this.k] = this.workingLightMapData;
			this.k++;
		}
		this.done = true;
	}

	// Token: 0x060037D3 RID: 14291 RVA: 0x00131BB8 File Offset: 0x0012FDB8
	public void DoLightsStep()
	{
		this.fromPixels = this.lightmapDatas[this.firstData].lights[this.k];
		this.toPixels = this.lightmapDatas[this.secondData].lights[this.k];
		this.mixedPixels = this.fromPixels;
		this.j = 0;
		while (this.j < this.mixedPixels.Length)
		{
			this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
			this.j++;
		}
		this.finishedStep = true;
	}

	// Token: 0x060037D4 RID: 14292 RVA: 0x00131C7C File Offset: 0x0012FE7C
	public void DoDirsStep()
	{
		this.fromPixels = this.lightmapDatas[this.firstData].dirs[this.k];
		this.toPixels = this.lightmapDatas[this.secondData].dirs[this.k];
		this.mixedPixels = this.fromPixels;
		this.j = 0;
		while (this.j < this.mixedPixels.Length)
		{
			this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
			this.j++;
		}
		this.finishedStep = true;
	}

	// Token: 0x060037D5 RID: 14293 RVA: 0x00131D3F File Offset: 0x0012FF3F
	private IEnumerator LightMapSet(int setFirstData, int setSecondData, float setLerp)
	{
		this.working = true;
		this.firstData = setFirstData;
		this.secondData = setSecondData;
		this.lerpValue = setLerp;
		this.k = 0;
		while (this.k < this.lightmapDatas[this.firstData].lights.Length)
		{
			this.lightsThread = new Thread(new ThreadStart(this.DoLightsStep));
			this.lightsThread.Start();
			yield return new WaitUntil(() => this.finishedStep);
			this.finishedStep = false;
			this.workingLightMapData.lightmapColor.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapColor.Apply(false);
			this.dirsThread = new Thread(new ThreadStart(this.DoDirsStep));
			this.dirsThread.Start();
			yield return new WaitUntil(() => this.finishedStep);
			this.finishedStep = false;
			this.workingLightMapData.lightmapDir.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.workingLightMapDatas[this.k] = this.workingLightMapData;
			this.k++;
		}
		LightmapSettings.lightmaps = this.workingLightMapDatas;
		this.working = false;
		this.done = true;
		yield break;
	}

	// Token: 0x040047FE RID: 18430
	[OnEnterPlay_SetNull]
	public static volatile GorillaDayNight instance;

	// Token: 0x040047FF RID: 18431
	public GorillaLightmapData[] lightmapDatas;

	// Token: 0x04004800 RID: 18432
	private LightmapData[] workingLightMapDatas;

	// Token: 0x04004801 RID: 18433
	private LightmapData workingLightMapData;

	// Token: 0x04004802 RID: 18434
	public float lerpValue;

	// Token: 0x04004803 RID: 18435
	public bool done;

	// Token: 0x04004804 RID: 18436
	public bool finishedStep;

	// Token: 0x04004805 RID: 18437
	private Color[] fromPixels;

	// Token: 0x04004806 RID: 18438
	private Color[] toPixels;

	// Token: 0x04004807 RID: 18439
	private Color[] mixedPixels;

	// Token: 0x04004808 RID: 18440
	public int firstData;

	// Token: 0x04004809 RID: 18441
	public int secondData;

	// Token: 0x0400480A RID: 18442
	public int i;

	// Token: 0x0400480B RID: 18443
	public int j;

	// Token: 0x0400480C RID: 18444
	public int k;

	// Token: 0x0400480D RID: 18445
	public int l;

	// Token: 0x0400480E RID: 18446
	private Thread lightsThread;

	// Token: 0x0400480F RID: 18447
	private Thread dirsThread;

	// Token: 0x04004810 RID: 18448
	public bool test;

	// Token: 0x04004811 RID: 18449
	public bool working;
}
