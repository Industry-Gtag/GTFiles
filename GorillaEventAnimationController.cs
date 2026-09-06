using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000D2 RID: 210
[RequireComponent(typeof(Animation))]
public class GorillaEventAnimationController : MonoBehaviour
{
	// Token: 0x0600050C RID: 1292 RVA: 0x0001C204 File Offset: 0x0001A404
	private void Awake()
	{
		this.totalTime = 0f;
		this.bakedAnimationData = new Dictionary<AnimationClip, Dictionary<GorillaEventAnimation, List<GorillaEventAnimationController.ControlledAnimationKeyframeData>>>();
		for (int i = 0; i < this.bakedAnimKeyframeData.Count; i++)
		{
			AnimationClip clip = this.bakedAnimKeyframeData[i].clip;
			this.bakedAnimationData.Add(clip, new Dictionary<GorillaEventAnimation, List<GorillaEventAnimationController.ControlledAnimationKeyframeData>>());
			for (int j = 0; j < this.bakedAnimKeyframeData[i].gEAKeyframeData.Count; j++)
			{
				this.bakedAnimationData[clip].Add(this.bakedAnimKeyframeData[i].gEAKeyframeData[j].gEA, this.bakedAnimKeyframeData[i].gEAKeyframeData[j].keyframeData);
			}
			this.totalTime += clip.length;
		}
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x0001C2E4 File Offset: 0x0001A4E4
	private void Update()
	{
		AnimationState animationState = null;
		foreach (object obj in this.controllingAnimation)
		{
			AnimationState animationState2 = (AnimationState)obj;
			if (animationState2.weight == 1f)
			{
				animationState = animationState2;
				this.currentClip = animationState2.clip;
				break;
			}
		}
		if (!this.playAnimation)
		{
			if (this.controllingAnimation.isPlaying)
			{
				this.controllingAnimation.Stop();
			}
			return;
		}
		if (!this.controllingAnimation.enabled)
		{
			this.controllingAnimation.enabled = true;
		}
		if (this.currentClip != this.clips[this.animationClipIndex] || animationState == null || !this.controllingAnimation.isPlaying)
		{
			this.currentClip = this.clips[this.animationClipIndex];
			this.currentClip.legacy = true;
			while (this.lateStart > 0f && this.currentClip.length < this.lateStart && this.animationClipIndex < this.clips.Count - 1)
			{
				this.lateStart -= this.currentClip.length;
				List<AnimationClip> list = this.clips;
				int num = this.animationClipIndex + 1;
				this.animationClipIndex = num;
				this.currentClip = list[num];
				this.currentClip.legacy = true;
			}
			this.controllingAnimation.Play(this.currentClip.name);
			animationState = this.controllingAnimation[this.currentClip.name];
			animationState.time = Math.Min(this.lateStart, this.currentClip.length);
			this.lateStart = 0f;
		}
		float time = animationState.time;
		if (!this.bakedAnimationData.ContainsKey(this.currentClip))
		{
			return;
		}
		foreach (KeyValuePair<GorillaEventAnimation, List<GorillaEventAnimationController.ControlledAnimationKeyframeData>> keyValuePair in this.bakedAnimationData[this.currentClip])
		{
			GorillaEventAnimation key = keyValuePair.Key;
			List<GorillaEventAnimationController.ControlledAnimationKeyframeData> value = keyValuePair.Value;
			int i = 0;
			while (i < value.Count)
			{
				if (value.Count < 2 || i == value.Count - 1 || time < value[i].startTime || time < value[i + 1].startTime)
				{
					bool flag = false;
					int num2 = value[i].animationClipIndex;
					float num3 = time - value[i].startTime + value[i].startOffset;
					if (key.enabled != value[i].animEnabled)
					{
						key.enabled = value[i].animEnabled;
						flag = value[i].animEnabled;
					}
					if (value[i].animEnabled && (key._clipIndex != value[i].animationClipIndex || !key._animation.IsPlaying(key.clips[num2].name)))
					{
						flag = true;
					}
					if (flag)
					{
						key.PlayClipByIndex(value[i].animationClipIndex, num3);
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
		if (this.animationTime < this.totalTime)
		{
			this.animationTime += Time.deltaTime;
		}
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x0001C6A4 File Offset: 0x0001A8A4
	public void StartPlaying(float secondsPast)
	{
		this.lateStart = secondsPast;
		this.animationTime = secondsPast;
		this.animationClipIndex = 0;
		this.playAnimation = true;
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x0001C6CF File Offset: 0x0001A8CF
	public void StartPlaying()
	{
		this.StartPlaying(0f);
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x0001C6DC File Offset: 0x0001A8DC
	public void SetAnimationClip(int clip)
	{
		this.animationClipIndex = clip;
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x0001C6E8 File Offset: 0x0001A8E8
	public void StartPlayingClip(float secondsPast)
	{
		this.lateStart = secondsPast;
		this.animationTime = secondsPast;
		this.playAnimation = true;
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x0001C70C File Offset: 0x0001A90C
	public void StartPlayingClip()
	{
		this.StartPlayingClip(0f);
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x0001C71C File Offset: 0x0001A91C
	private void OnEnable()
	{
		if (this.suspended <= 0f)
		{
			return;
		}
		this.lateStart = this.animationTime + (Time.time - this.suspended);
		this.animationClipIndex = 0;
		this.playAnimation = true;
		this.suspended = 0f;
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x0001C769 File Offset: 0x0001A969
	private void OnDisable()
	{
		if (!this.playAnimation)
		{
			return;
		}
		this.suspended = Time.time;
		this.playAnimation = false;
		if (this.controllingAnimation.isPlaying)
		{
			this.controllingAnimation.Stop();
		}
	}

	// Token: 0x040005C9 RID: 1481
	public Animation controllingAnimation;

	// Token: 0x040005CA RID: 1482
	public bool playAnimation;

	// Token: 0x040005CB RID: 1483
	private float lateStart;

	// Token: 0x040005CC RID: 1484
	private float animationTime;

	// Token: 0x040005CD RID: 1485
	public int animationClipIndex;

	// Token: 0x040005CE RID: 1486
	public List<AnimationClip> clips;

	// Token: 0x040005CF RID: 1487
	private AnimationClip currentClip;

	// Token: 0x040005D0 RID: 1488
	private float totalTime;

	// Token: 0x040005D1 RID: 1489
	private Dictionary<AnimationClip, Dictionary<GorillaEventAnimation, List<GorillaEventAnimationController.ControlledAnimationKeyframeData>>> bakedAnimationData;

	// Token: 0x040005D2 RID: 1490
	[SerializeField]
	[HideInInspector]
	private List<GorillaEventAnimationController.AnimToGEAKeyframeData> bakedAnimKeyframeData;

	// Token: 0x040005D3 RID: 1491
	private float suspended;

	// Token: 0x020000D3 RID: 211
	[Serializable]
	public struct AnimToGEAKeyframeData
	{
		// Token: 0x040005D4 RID: 1492
		public AnimationClip clip;

		// Token: 0x040005D5 RID: 1493
		public List<GorillaEventAnimationController.GEAKeyframeData> gEAKeyframeData;
	}

	// Token: 0x020000D4 RID: 212
	[Serializable]
	public struct GEAKeyframeData
	{
		// Token: 0x040005D6 RID: 1494
		public GorillaEventAnimation gEA;

		// Token: 0x040005D7 RID: 1495
		public List<GorillaEventAnimationController.ControlledAnimationKeyframeData> keyframeData;
	}

	// Token: 0x020000D5 RID: 213
	[Serializable]
	public struct ControlledAnimationKeyframeData
	{
		// Token: 0x06000516 RID: 1302 RVA: 0x0001C79E File Offset: 0x0001A99E
		public ControlledAnimationKeyframeData(int _index, float _time, float _startOffset, bool _animEnabled)
		{
			this.animationClipIndex = _index;
			this.startTime = _time;
			this.startOffset = _startOffset;
			this.animEnabled = _animEnabled;
		}

		// Token: 0x040005D8 RID: 1496
		public int animationClipIndex;

		// Token: 0x040005D9 RID: 1497
		public float startTime;

		// Token: 0x040005DA RID: 1498
		public float startOffset;

		// Token: 0x040005DB RID: 1499
		public bool animEnabled;
	}
}
