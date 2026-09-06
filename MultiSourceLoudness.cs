using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000952 RID: 2386
public class MultiSourceLoudness : MonoBehaviour, ISpeakerLoudness
{
	// Token: 0x170005C5 RID: 1477
	// (get) Token: 0x06003EB3 RID: 16051 RVA: 0x0015222F File Offset: 0x0015042F
	// (set) Token: 0x06003EB4 RID: 16052 RVA: 0x00152237 File Offset: 0x00150437
	public bool IsSpeaking { get; private set; }

	// Token: 0x170005C6 RID: 1478
	// (get) Token: 0x06003EB5 RID: 16053 RVA: 0x00152240 File Offset: 0x00150440
	// (set) Token: 0x06003EB6 RID: 16054 RVA: 0x00152248 File Offset: 0x00150448
	public float Loudness { get; private set; }

	// Token: 0x170005C7 RID: 1479
	// (get) Token: 0x06003EB7 RID: 16055 RVA: 0x00152251 File Offset: 0x00150451
	// (set) Token: 0x06003EB8 RID: 16056 RVA: 0x00152259 File Offset: 0x00150459
	public bool IsMicEnabled { get; private set; }

	// Token: 0x06003EB9 RID: 16057 RVA: 0x00152262 File Offset: 0x00150462
	private void Awake()
	{
		this.RebuildSources();
	}

	// Token: 0x06003EBA RID: 16058 RVA: 0x0015226C File Offset: 0x0015046C
	public void RebuildSources()
	{
		this.sources.Clear();
		for (int i = 0; i < this.sourceBehaviours.Count; i++)
		{
			ISpeakerLoudness speakerLoudness = this.sourceBehaviours[i] as ISpeakerLoudness;
			if (speakerLoudness != null && speakerLoudness != this)
			{
				this.sources.Add(speakerLoudness);
			}
		}
	}

	// Token: 0x06003EBB RID: 16059 RVA: 0x001522BF File Offset: 0x001504BF
	public void AddSource(ISpeakerLoudness source)
	{
		if (source != null && source != this && !this.sources.Contains(source))
		{
			this.sources.Add(source);
		}
	}

	// Token: 0x06003EBC RID: 16060 RVA: 0x001522E2 File Offset: 0x001504E2
	public void RemoveSource(ISpeakerLoudness source)
	{
		this.sources.Remove(source);
	}

	// Token: 0x06003EBD RID: 16061 RVA: 0x001522F4 File Offset: 0x001504F4
	private void Update()
	{
		bool flag = false;
		bool flag2 = false;
		float num = 0f;
		for (int i = 0; i < this.sources.Count; i++)
		{
			ISpeakerLoudness speakerLoudness = this.sources[i];
			flag2 |= speakerLoudness.IsMicEnabled;
			if (speakerLoudness.IsSpeaking)
			{
				flag = true;
				if (speakerLoudness.Loudness > num)
				{
					num = speakerLoudness.Loudness;
				}
			}
		}
		this.IsSpeaking = flag;
		this.IsMicEnabled = flag2;
		this.Loudness = (flag ? (num * this.loudnessMultiplier) : 0f);
	}

	// Token: 0x04004F21 RID: 20257
	[Tooltip("Sources to combine. Each must be a component implementing ISpeakerLoudness (e.g. AudioSourceLoudness or GorillaSpeakerLoudness).")]
	[SerializeField]
	private List<MonoBehaviour> sourceBehaviours = new List<MonoBehaviour>();

	// Token: 0x04004F22 RID: 20258
	[Tooltip("Scales the combined loudness before it is reported. Use to tune the result onto the scale GorillaMouthFlap's volume thresholds expect.")]
	[SerializeField]
	private float loudnessMultiplier = 1f;

	// Token: 0x04004F23 RID: 20259
	private readonly List<ISpeakerLoudness> sources = new List<ISpeakerLoudness>();
}
