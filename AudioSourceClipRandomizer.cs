using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000012 RID: 18
[RequireComponent(typeof(AudioSource))]
public class AudioSourceClipRandomizer : MonoBehaviour
{
	// Token: 0x0600004F RID: 79 RVA: 0x00002E83 File Offset: 0x00001083
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
		this.playOnAwake = this.source.playOnAwake;
		this.source.playOnAwake = false;
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00002EB0 File Offset: 0x000010B0
	public void Play()
	{
		int num = Random.Range(0, 60);
		if (GorillaComputer.instance != null)
		{
			num = GorillaComputer.instance.GetServerTime().Second;
		}
		this.source.clip = this.clips[num % this.clips.Length];
		this.source.GTPlay();
	}

	// Token: 0x06000051 RID: 81 RVA: 0x00002F11 File Offset: 0x00001111
	private void OnEnable()
	{
		if (this.playOnAwake)
		{
			this.Play();
		}
	}

	// Token: 0x04000031 RID: 49
	[SerializeField]
	private AudioClip[] clips;

	// Token: 0x04000032 RID: 50
	private AudioSource source;

	// Token: 0x04000033 RID: 51
	private bool playOnAwake;
}
