using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200003F RID: 63
public class CosmeticPlaySoundOnColision : MonoBehaviour
{
	// Token: 0x06000102 RID: 258 RVA: 0x0000639C File Offset: 0x0000459C
	private void Awake()
	{
		this.transferrableObject = base.GetComponentInParent<TransferrableObject>();
		this.soundLookup = new Dictionary<int, int>();
		this.audioSource = base.GetComponent<AudioSource>();
		for (int i = 0; i < this.soundIdRemappings.Length; i++)
		{
			this.soundLookup.Add(this.soundIdRemappings[i].SoundIn, this.soundIdRemappings[i].SoundOut);
		}
	}

	// Token: 0x06000103 RID: 259 RVA: 0x00006404 File Offset: 0x00004604
	private void OnTriggerEnter(Collider other)
	{
		GorillaSurfaceOverride gorillaSurfaceOverride;
		if (this.speed >= this.minSpeed && other.TryGetComponent<GorillaSurfaceOverride>(out gorillaSurfaceOverride))
		{
			int num;
			if (this.soundLookup.TryGetValue(gorillaSurfaceOverride.overrideIndex, out num))
			{
				this.playSound(num, this.invokeEventOnOverideSound);
				return;
			}
			this.playSound(this.defaultSound, this.invokeEventOnDefaultSound);
		}
	}

	// Token: 0x06000104 RID: 260 RVA: 0x00006460 File Offset: 0x00004660
	private void playSound(int soundIndex, bool invokeEvent)
	{
		if (soundIndex > -1 && soundIndex < GTPlayer.Instance.materialData.Count)
		{
			if (this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
				if (this.invokeEventsOnAllClients || this.transferrableObject.IsMyItem())
				{
					this.OnStopPlayback.Invoke();
				}
				if (this.crWaitForStopPlayback != null)
				{
					base.StopCoroutine(this.crWaitForStopPlayback);
					this.crWaitForStopPlayback = null;
				}
			}
			this.audioSource.clip = GTPlayer.Instance.materialData[soundIndex].audio;
			this.audioSource.GTPlay();
			if (invokeEvent && (this.invokeEventsOnAllClients || this.transferrableObject.IsMyItem()))
			{
				this.OnStartPlayback.Invoke();
				this.crWaitForStopPlayback = base.StartCoroutine(this.waitForStopPlayback());
			}
		}
	}

	// Token: 0x06000105 RID: 261 RVA: 0x0000653C File Offset: 0x0000473C
	private IEnumerator waitForStopPlayback()
	{
		while (this.audioSource.isPlaying)
		{
			yield return null;
		}
		if (this.invokeEventsOnAllClients || this.transferrableObject.IsMyItem())
		{
			this.OnStopPlayback.Invoke();
		}
		this.crWaitForStopPlayback = null;
		yield break;
	}

	// Token: 0x06000106 RID: 262 RVA: 0x0000654B File Offset: 0x0000474B
	private void FixedUpdate()
	{
		this.speed = Vector3.Distance(base.transform.position, this.previousFramePosition) * Time.fixedDeltaTime * 100f;
		this.previousFramePosition = base.transform.position;
	}

	// Token: 0x0400010B RID: 267
	[GorillaSoundLookup]
	[SerializeField]
	private int defaultSound = 1;

	// Token: 0x0400010C RID: 268
	[SerializeField]
	private SoundIdRemapping[] soundIdRemappings;

	// Token: 0x0400010D RID: 269
	[SerializeField]
	private UnityEvent OnStartPlayback;

	// Token: 0x0400010E RID: 270
	[SerializeField]
	private UnityEvent OnStopPlayback;

	// Token: 0x0400010F RID: 271
	[SerializeField]
	private float minSpeed = 0.1f;

	// Token: 0x04000110 RID: 272
	private TransferrableObject transferrableObject;

	// Token: 0x04000111 RID: 273
	private Dictionary<int, int> soundLookup;

	// Token: 0x04000112 RID: 274
	private AudioSource audioSource;

	// Token: 0x04000113 RID: 275
	private Coroutine crWaitForStopPlayback;

	// Token: 0x04000114 RID: 276
	private float speed;

	// Token: 0x04000115 RID: 277
	private Vector3 previousFramePosition;

	// Token: 0x04000116 RID: 278
	[SerializeField]
	private bool invokeEventsOnAllClients;

	// Token: 0x04000117 RID: 279
	[SerializeField]
	private bool invokeEventOnOverideSound = true;

	// Token: 0x04000118 RID: 280
	[SerializeField]
	private bool invokeEventOnDefaultSound;
}
