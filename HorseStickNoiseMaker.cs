using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200021A RID: 538
public class HorseStickNoiseMaker : MonoBehaviour
{
	// Token: 0x06000E17 RID: 3607 RVA: 0x0004D314 File Offset: 0x0004B514
	protected void OnEnable()
	{
		if (!this.gorillaPlayerXform && !base.transform.TryFindByPath(this.gorillaPlayerXform_path, out this.gorillaPlayerXform, false))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"HorseStickNoiseMaker: DEACTIVATING! Could not find gorillaPlayerXform using path: \"",
				this.gorillaPlayerXform_path,
				"\"\nThis component's transform path: \"",
				base.transform.GetPath(),
				"\""
			}));
			base.gameObject.SetActive(false);
			return;
		}
		this.oldPos = this.gorillaPlayerXform.position;
		this.distElapsed = 0f;
		this.timeSincePlay = 0f;
	}

	// Token: 0x06000E18 RID: 3608 RVA: 0x0004D3BC File Offset: 0x0004B5BC
	protected void LateUpdate()
	{
		Vector3 position = this.gorillaPlayerXform.position;
		Vector3 vector = position - this.oldPos;
		this.distElapsed += vector.magnitude;
		this.timeSincePlay += Time.deltaTime;
		this.oldPos = position;
		if (this.distElapsed >= this.metersPerClip && this.timeSincePlay >= this.minSecBetweenClips)
		{
			this.soundBankPlayer.Play();
			this.distElapsed = 0f;
			this.timeSincePlay = 0f;
			if (this.particleFX != null)
			{
				this.particleFX.Play();
			}
		}
	}

	// Token: 0x040010E1 RID: 4321
	[Tooltip("Meters the object should traverse between playing a provided audio clip.")]
	public float metersPerClip = 4f;

	// Token: 0x040010E2 RID: 4322
	[Tooltip("Number of seconds that must elapse before playing another audio clip.")]
	public float minSecBetweenClips = 1.5f;

	// Token: 0x040010E3 RID: 4323
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x040010E4 RID: 4324
	[Tooltip("Transform assigned in Gorilla Player Networked Prefab to the Gorilla Player Networked parent to keep track of distance traveled.")]
	public Transform gorillaPlayerXform;

	// Token: 0x040010E5 RID: 4325
	[Delayed]
	public string gorillaPlayerXform_path;

	// Token: 0x040010E6 RID: 4326
	[Tooltip("Optional particle FX to spawn when sound plays")]
	public ParticleSystem particleFX;

	// Token: 0x040010E7 RID: 4327
	private Vector3 oldPos;

	// Token: 0x040010E8 RID: 4328
	private float timeSincePlay;

	// Token: 0x040010E9 RID: 4329
	private float distElapsed;
}
