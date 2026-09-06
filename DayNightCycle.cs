using System;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

// Token: 0x02000A1B RID: 2587
public class DayNightCycle : MonoBehaviour
{
	// Token: 0x06004261 RID: 16993 RVA: 0x00161C9C File Offset: 0x0015FE9C
	public void Awake()
	{
		this.fromMap = new Texture2D(this._sunriseMap.width, this._sunriseMap.height);
		this.fromMap = LightmapSettings.lightmaps[0].lightmapColor;
		this.toMap = new Texture2D(this._dayMap.width, this._dayMap.height);
		this.toMap.SetPixels(this._dayMap.GetPixels());
		this.toMap.Apply();
		this.workBlockMix = new Color[this.subTextureSize * this.subTextureSize];
		this.newTexture = new Texture2D(this.fromMap.width, this.fromMap.height, this.fromMap.graphicsFormat, TextureCreationFlags.None);
		this.newData = new LightmapData();
		this.textureHeight = this.fromMap.height;
		this.textureWidth = this.fromMap.width;
		this.subTextureArray = new Texture2D[(int)Mathf.Pow((float)(this.textureHeight / this.subTextureSize), 2f)];
		Debug.Log("aaaa " + this.fromMap.format.ToString());
		Debug.Log("aaaa " + this.fromMap.graphicsFormat.ToString());
		this.startJob = false;
		this.startCoroutine = false;
		this.startedCoroutine = false;
		this.finishedCoroutine = false;
	}

	// Token: 0x06004262 RID: 16994 RVA: 0x00161E20 File Offset: 0x00160020
	public void Update()
	{
		if (this.startJob)
		{
			this.startJob = false;
			this.startTime = Time.realtimeSinceStartup;
			base.StartCoroutine(this.UpdateWork());
			this.timeTakenStartingJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
		}
		if (this.jobStarted && this.jobHandle.IsCompleted)
		{
			this.timeTakenDuringJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
			this.jobHandle.Complete();
			this.jobStarted = false;
			this.newTexture.SetPixels(this.job.mixedPixels.ToArray());
			this.newData.lightmapDir = LightmapSettings.lightmaps[0].lightmapDir;
			LightmapSettings.lightmaps = new LightmapData[] { this.newData };
			this.job.fromPixels.Dispose();
			this.job.toPixels.Dispose();
			this.job.mixedPixels.Dispose();
			this.timeTakenPostJob = Time.realtimeSinceStartup - this.startTime;
		}
		if (this.startCoroutine)
		{
			this.startCoroutine = false;
			this.startTime = Time.realtimeSinceStartup;
			this.newTexture = new Texture2D(this.fromMap.width, this.fromMap.height);
			base.StartCoroutine(this.UpdateWork());
		}
		if (this.startedCoroutine && this.finishedCoroutine)
		{
			this.startedCoroutine = false;
			this.finishedCoroutine = false;
			this.timeTakenDuringJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
			this.newData = LightmapSettings.lightmaps[0];
			this.newData.lightmapColor = this.fromMap;
			LightmapData[] lightmaps = LightmapSettings.lightmaps;
			lightmaps[0].lightmapColor = this.fromMap;
			LightmapSettings.lightmaps = lightmaps;
			this.timeTakenPostJob = Time.realtimeSinceStartup - this.startTime;
		}
	}

	// Token: 0x06004263 RID: 16995 RVA: 0x0016200E File Offset: 0x0016020E
	public IEnumerator UpdateWork()
	{
		yield return 0;
		this.timeTakenStartingJob = Time.realtimeSinceStartup - this.startTime;
		this.startTime = Time.realtimeSinceStartup;
		this.startedCoroutine = true;
		this.currentSubTexture = 0;
		int num;
		for (int i = 0; i < this.subTextureArray.Length; i = num + 1)
		{
			this.subTextureArray[i] = new Texture2D(this.subTextureSize, this.subTextureSize, this.fromMap.graphicsFormat, TextureCreationFlags.None);
			yield return 0;
			num = i;
		}
		for (int i = 0; i < this.textureWidth / this.subTextureSize; i = num + 1)
		{
			this.currentColumn = i;
			for (int j = 0; j < this.textureHeight / this.subTextureSize; j = num + 1)
			{
				this.currentRow = j;
				this.workBlockFrom = this.fromMap.GetPixels(i * this.subTextureSize, j * this.subTextureSize, this.subTextureSize, this.subTextureSize);
				this.workBlockTo = this.toMap.GetPixels(i * this.subTextureSize, j * this.subTextureSize, this.subTextureSize, this.subTextureSize);
				for (int k = 0; k < this.subTextureSize * this.subTextureSize - 1; k++)
				{
					this.workBlockMix[k] = Color.Lerp(this.workBlockFrom[k], this.workBlockTo[k], this.lerpAmount);
				}
				this.subTextureArray[j * (this.textureWidth / this.subTextureSize) + i].SetPixels(0, 0, this.subTextureSize, this.subTextureSize, this.workBlockMix);
				yield return 0;
				num = j;
			}
			num = i;
		}
		for (int i = 0; i < this.subTextureArray.Length; i = num + 1)
		{
			this.currentSubTexture = i;
			this.subTextureArray[i].Apply();
			yield return 0;
			Graphics.CopyTexture(this.subTextureArray[i], 0, 0, 0, 0, this.subTextureSize, this.subTextureSize, this.newTexture, 0, 0, i * this.subTextureSize % this.textureHeight, (int)Mathf.Floor((float)(this.subTextureSize * i / this.textureHeight)) * this.subTextureSize);
			yield return 0;
			num = i;
		}
		this.finishedCoroutine = true;
		yield break;
	}

	// Token: 0x040053F3 RID: 21491
	public Texture2D _dayMap;

	// Token: 0x040053F4 RID: 21492
	private Texture2D fromMap;

	// Token: 0x040053F5 RID: 21493
	public Texture2D _sunriseMap;

	// Token: 0x040053F6 RID: 21494
	private Texture2D toMap;

	// Token: 0x040053F7 RID: 21495
	public DayNightCycle.LerpBakedLightingJob job;

	// Token: 0x040053F8 RID: 21496
	public JobHandle jobHandle;

	// Token: 0x040053F9 RID: 21497
	public bool isComplete;

	// Token: 0x040053FA RID: 21498
	private float startTime;

	// Token: 0x040053FB RID: 21499
	public float timeTakenStartingJob;

	// Token: 0x040053FC RID: 21500
	public float timeTakenPostJob;

	// Token: 0x040053FD RID: 21501
	public float timeTakenDuringJob;

	// Token: 0x040053FE RID: 21502
	public LightmapData newData;

	// Token: 0x040053FF RID: 21503
	private Color[] fromPixels;

	// Token: 0x04005400 RID: 21504
	private Color[] toPixels;

	// Token: 0x04005401 RID: 21505
	private Color[] mixedPixels;

	// Token: 0x04005402 RID: 21506
	private LightmapData[] newDatas;

	// Token: 0x04005403 RID: 21507
	public Texture2D newTexture;

	// Token: 0x04005404 RID: 21508
	public int textureWidth;

	// Token: 0x04005405 RID: 21509
	public int textureHeight;

	// Token: 0x04005406 RID: 21510
	private Color[] workBlockFrom;

	// Token: 0x04005407 RID: 21511
	private Color[] workBlockTo;

	// Token: 0x04005408 RID: 21512
	private Color[] workBlockMix;

	// Token: 0x04005409 RID: 21513
	public int subTextureSize = 1024;

	// Token: 0x0400540A RID: 21514
	public Texture2D[] subTextureArray;

	// Token: 0x0400540B RID: 21515
	public bool startCoroutine;

	// Token: 0x0400540C RID: 21516
	public bool startedCoroutine;

	// Token: 0x0400540D RID: 21517
	public bool finishedCoroutine;

	// Token: 0x0400540E RID: 21518
	public bool startJob;

	// Token: 0x0400540F RID: 21519
	public float switchTimeTaken;

	// Token: 0x04005410 RID: 21520
	public bool jobStarted;

	// Token: 0x04005411 RID: 21521
	public float lerpAmount;

	// Token: 0x04005412 RID: 21522
	public int currentRow;

	// Token: 0x04005413 RID: 21523
	public int currentColumn;

	// Token: 0x04005414 RID: 21524
	public int currentSubTexture;

	// Token: 0x04005415 RID: 21525
	public int currentRowInSubtexture;

	// Token: 0x02000A1C RID: 2588
	public struct LerpBakedLightingJob : IJob
	{
		// Token: 0x06004265 RID: 16997 RVA: 0x00162030 File Offset: 0x00160230
		public void Execute()
		{
			for (int i = 0; i < this.fromPixels.Length; i++)
			{
				this.mixedPixels[i] = Color.Lerp(this.fromPixels[i], this.toPixels[i], 0.5f);
			}
		}

		// Token: 0x04005416 RID: 21526
		public NativeArray<Color> fromPixels;

		// Token: 0x04005417 RID: 21527
		public NativeArray<Color> toPixels;

		// Token: 0x04005418 RID: 21528
		public NativeArray<Color> mixedPixels;

		// Token: 0x04005419 RID: 21529
		public float lerpValue;
	}
}
