using System;
using UnityEngine;

// Token: 0x02000596 RID: 1430
public class HotPepperFace : MonoBehaviour
{
	// Token: 0x06002433 RID: 9267 RVA: 0x000C27F7 File Offset: 0x000C09F7
	public void PlayFX(float delay)
	{
		if (delay < 0f)
		{
			this.PlayFX();
			return;
		}
		base.Invoke("PlayFX", delay);
	}

	// Token: 0x06002434 RID: 9268 RVA: 0x000C2814 File Offset: 0x000C0A14
	public void PlayFX()
	{
		this._faceMesh.SetActive(true);
		this._thermalSourceVolume.SetActive(true);
		this._fireFX.Play();
		this._flameSpeaker.GTPlay();
		this._breathSpeaker.GTPlay();
		base.Invoke("StopFX", this._effectLength);
	}

	// Token: 0x06002435 RID: 9269 RVA: 0x000C286B File Offset: 0x000C0A6B
	public void StopFX()
	{
		this._faceMesh.SetActive(false);
		this._thermalSourceVolume.SetActive(false);
		this._fireFX.Stop();
		this._flameSpeaker.GTStop();
		this._breathSpeaker.GTStop();
	}

	// Token: 0x04002F74 RID: 12148
	[SerializeField]
	private GameObject _faceMesh;

	// Token: 0x04002F75 RID: 12149
	[SerializeField]
	private ParticleSystem _fireFX;

	// Token: 0x04002F76 RID: 12150
	[SerializeField]
	private AudioSource _flameSpeaker;

	// Token: 0x04002F77 RID: 12151
	[SerializeField]
	private AudioSource _breathSpeaker;

	// Token: 0x04002F78 RID: 12152
	[SerializeField]
	private float _effectLength = 1.5f;

	// Token: 0x04002F79 RID: 12153
	[SerializeField]
	private GameObject _thermalSourceVolume;
}
