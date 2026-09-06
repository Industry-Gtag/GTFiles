using System;
using UnityEngine;

// Token: 0x020000D1 RID: 209
public class GorillaEventAnimation : MonoBehaviour
{
	// Token: 0x06000508 RID: 1288 RVA: 0x0001C104 File Offset: 0x0001A304
	private void Awake()
	{
		if (this._animation == null)
		{
			this._animation = base.GetComponentInChildren<Animation>();
		}
		this._animation.playAutomatically = false;
		for (int i = 0; i < this.clips.Length; i++)
		{
			this.clips[i].legacy = true;
		}
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x0001C158 File Offset: 0x0001A358
	private void OnDisable()
	{
		this._animation.enabled = false;
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x0001C168 File Offset: 0x0001A368
	public void PlayClipByIndex(int index, float startTime)
	{
		if (index < 0 || index >= this.clips.Length)
		{
			return;
		}
		if (!this._animation.enabled)
		{
			this._animation.enabled = true;
		}
		AnimationClip animationClip = this.clips[index];
		if (this._animation.GetClip(animationClip.name) == null)
		{
			this._animation.AddClip(animationClip, animationClip.name);
		}
		this._animation.Play(animationClip.name);
		this._animation[animationClip.name].time = startTime;
		this._clipIndex = index;
	}

	// Token: 0x040005C4 RID: 1476
	public Animation _animation;

	// Token: 0x040005C5 RID: 1477
	public float offsetTime;

	// Token: 0x040005C6 RID: 1478
	public int animationClipIndex;

	// Token: 0x040005C7 RID: 1479
	public AnimationClip[] clips;

	// Token: 0x040005C8 RID: 1480
	[NonSerialized]
	public int _clipIndex;
}
