using System;
using UnityEngine;

// Token: 0x02000E3F RID: 3647
[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(AudioSource))]
public class LightningStrike : MonoBehaviour
{
	// Token: 0x0600591B RID: 22811 RVA: 0x001CF3A8 File Offset: 0x001CD5A8
	private void Initialize()
	{
		this.ps = base.GetComponent<ParticleSystem>();
		this.psMain = this.ps.main;
		this.psMain.playOnAwake = true;
		this.psMain.stopAction = ParticleSystemStopAction.Disable;
		this.psShape = this.ps.shape;
		this.psTrails = this.ps.trails;
		this.audioSource = base.GetComponent<AudioSource>();
		this.audioSource.playOnAwake = true;
	}

	// Token: 0x0600591C RID: 22812 RVA: 0x001CF424 File Offset: 0x001CD624
	public void Play(Vector3 p1, Vector3 p2, float beamWidthMultiplier, float audioVolume, float duration, Gradient colorOverLifetime)
	{
		if (this.ps == null)
		{
			this.Initialize();
		}
		base.transform.position = p1;
		base.transform.rotation = Quaternion.LookRotation(p1 - p2);
		this.psShape.radius = Vector3.Distance(p1, p2) * 0.5f;
		this.psShape.position = new Vector3(0f, 0f, -this.psShape.radius);
		this.psShape.randomPositionAmount = Mathf.Clamp(this.psShape.radius / 50f, 0f, 1f);
		this.psTrails.widthOverTrail = new ParticleSystem.MinMaxCurve(beamWidthMultiplier * 0.1f, beamWidthMultiplier);
		this.psTrails.colorOverLifetime = colorOverLifetime;
		this.psMain.duration = duration;
		this.audioSource.volume = Mathf.Clamp(this.psShape.radius / 5f, 0f, 1f) * audioVolume;
		base.gameObject.SetActive(true);
	}

	// Token: 0x04006947 RID: 26951
	public static SRand rand = new SRand("LightningStrike");

	// Token: 0x04006948 RID: 26952
	private ParticleSystem ps;

	// Token: 0x04006949 RID: 26953
	private ParticleSystem.MainModule psMain;

	// Token: 0x0400694A RID: 26954
	private ParticleSystem.ShapeModule psShape;

	// Token: 0x0400694B RID: 26955
	private ParticleSystem.TrailModule psTrails;

	// Token: 0x0400694C RID: 26956
	private AudioSource audioSource;
}
