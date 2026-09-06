using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002B1 RID: 689
public class PlaySoundOnEnable : MonoBehaviour
{
	// Token: 0x060011E7 RID: 4583 RVA: 0x0006000D File Offset: 0x0005E20D
	private void Reset()
	{
		this._source = base.GetComponent<AudioSource>();
		if (this._source)
		{
			this._source.playOnAwake = false;
		}
	}

	// Token: 0x060011E8 RID: 4584 RVA: 0x00060034 File Offset: 0x0005E234
	private void OnEnable()
	{
		this.Play();
	}

	// Token: 0x060011E9 RID: 4585 RVA: 0x0006003C File Offset: 0x0005E23C
	private void OnDisable()
	{
		this.Stop();
	}

	// Token: 0x060011EA RID: 4586 RVA: 0x00060044 File Offset: 0x0005E244
	public void Play()
	{
		if (this._loop && this._clips.Length == 1 && this._loopDelay == Vector2.zero)
		{
			this._source.clip = this._clips[0];
			this._source.loop = true;
			this._source.GTPlay();
			return;
		}
		this._source.loop = false;
		if (this._loop)
		{
			base.StartCoroutine(this.DoLoop());
			return;
		}
		this._source.clip = this._clips[Random.Range(0, this._clips.Length)];
		this._source.GTPlay();
	}

	// Token: 0x060011EB RID: 4587 RVA: 0x000600EE File Offset: 0x0005E2EE
	private IEnumerator DoLoop()
	{
		while (base.enabled)
		{
			this._source.clip = this._clips[Random.Range(0, this._clips.Length)];
			this._source.GTPlay();
			while (this._source.isPlaying)
			{
				yield return null;
			}
			float num = Random.Range(this._loopDelay.x, this._loopDelay.y);
			if (num > 0f)
			{
				float waitEndTime = Time.time + num;
				while (Time.time < waitEndTime)
				{
					yield return null;
				}
			}
		}
		yield break;
	}

	// Token: 0x060011EC RID: 4588 RVA: 0x000600FD File Offset: 0x0005E2FD
	public void Stop()
	{
		this._source.GTStop();
		this._source.loop = false;
	}

	// Token: 0x0400157C RID: 5500
	[SerializeField]
	private AudioSource _source;

	// Token: 0x0400157D RID: 5501
	[SerializeField]
	private AudioClip[] _clips;

	// Token: 0x0400157E RID: 5502
	[SerializeField]
	private bool _loop;

	// Token: 0x0400157F RID: 5503
	[SerializeField]
	private Vector2 _loopDelay;
}
