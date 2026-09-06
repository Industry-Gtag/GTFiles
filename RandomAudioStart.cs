using System;
using UnityEngine;

// Token: 0x02000567 RID: 1383
public class RandomAudioStart : MonoBehaviour, IBuildValidation
{
	// Token: 0x06002326 RID: 8998 RVA: 0x000BCCA9 File Offset: 0x000BAEA9
	public bool BuildValidationCheck()
	{
		if (this.audioSource == null)
		{
			Debug.LogError("audio source is missing for RandomAudioStart, it won't work correctly", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x06002327 RID: 8999 RVA: 0x000BCCCC File Offset: 0x000BAECC
	private void OnEnable()
	{
		this.audioSource.time = Random.value * this.audioSource.clip.length;
	}

	// Token: 0x06002328 RID: 9000 RVA: 0x000BCCEF File Offset: 0x000BAEEF
	[ContextMenu("Assign Audio Source")]
	public void AssignAudioSource()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x04002E41 RID: 11841
	public AudioSource audioSource;
}
