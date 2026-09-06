using System;
using UnityEngine;

// Token: 0x020005F2 RID: 1522
public class AnimationEventListener : MonoBehaviour
{
	// Token: 0x060025E5 RID: 9701 RVA: 0x000C8F84 File Offset: 0x000C7184
	public void PlaySoundAtIndex(int index)
	{
		if (this.audioClips.Length <= index || index < 0)
		{
			return;
		}
		if (this.audioSource == null)
		{
			return;
		}
		if (this.audioClips[index] == null)
		{
			return;
		}
		this.audioSource.GTPlayOneShot(this.audioClips[index], 1f);
	}

	// Token: 0x060025E6 RID: 9702 RVA: 0x000C8FD9 File Offset: 0x000C71D9
	public void StopAudio()
	{
		if (this.audioSource == null)
		{
			return;
		}
		if (this.audioSource.isPlaying)
		{
			this.audioSource.Stop();
		}
	}

	// Token: 0x060025E7 RID: 9703 RVA: 0x000C9002 File Offset: 0x000C7202
	public void ActivateObject()
	{
		if (this.targetObject != null)
		{
			this.targetObject.SetActive(true);
		}
	}

	// Token: 0x060025E8 RID: 9704 RVA: 0x000C901E File Offset: 0x000C721E
	public void DeactivateObject()
	{
		if (this.targetObject != null)
		{
			this.targetObject.SetActive(false);
		}
	}

	// Token: 0x060025E9 RID: 9705 RVA: 0x000C903A File Offset: 0x000C723A
	public void ToggleObject()
	{
		if (this.targetObject != null)
		{
			this.targetObject.SetActive(!this.targetObject.activeSelf);
		}
	}

	// Token: 0x060025EA RID: 9706 RVA: 0x000C9063 File Offset: 0x000C7263
	public void PlayParticles()
	{
		if (this.particles != null && !this.particles.isPlaying)
		{
			this.particles.Play();
		}
	}

	// Token: 0x060025EB RID: 9707 RVA: 0x000C908B File Offset: 0x000C728B
	public void StopParticles()
	{
		if (this.particles != null && this.particles.isPlaying)
		{
			this.particles.Stop();
		}
	}

	// Token: 0x0400317F RID: 12671
	[Tooltip("Set this if calling ActivateObject, DeactivateObject, or ToggleObject")]
	[SerializeField]
	private GameObject targetObject;

	// Token: 0x04003180 RID: 12672
	[Tooltip("Set this if calling PlayParticles or StopParticles")]
	[SerializeField]
	private ParticleSystem particles;

	// Token: 0x04003181 RID: 12673
	[Tooltip("Set this if calling PlaySoundAtIndex or StopAudio")]
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003182 RID: 12674
	[Tooltip("Set this if calling PlaySoundAtIndex")]
	[SerializeField]
	private AudioClip[] audioClips;
}
