using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005A9 RID: 1449
public class MonkeCandle : RubberDuck
{
	// Token: 0x060024AE RID: 9390 RVA: 0x000C5144 File Offset: 0x000C3344
	protected override void Start()
	{
		base.Start();
		if (!this.IsMyItem())
		{
			this.movingFxAudio.volume = this.movingFxAudio.volume * 0.5f;
			this.fxExplodeAudio.volume = this.fxExplodeAudio.volume * 0.5f;
		}
	}

	// Token: 0x060024AF RID: 9391 RVA: 0x000C5198 File Offset: 0x000C3398
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		if (!this.particleFX.isPlaying)
		{
			return;
		}
		int particles = this.particleFX.GetParticles(this.fxParticleArray);
		if (particles <= 0)
		{
			this.movingFxAudio.GTStop();
			if (this.currentParticles.Count == 0)
			{
				return;
			}
		}
		for (int i = 0; i < particles; i++)
		{
			if (this.currentParticles.Contains(this.fxParticleArray[i].randomSeed))
			{
				this.currentParticles.Remove(this.fxParticleArray[i].randomSeed);
			}
		}
		foreach (uint num in this.currentParticles)
		{
			if (this.particleInfoDict.TryGetValue(num, out this.outPosition))
			{
				this.fxExplodeAudio.transform.position = this.outPosition;
				this.fxExplodeAudio.GTPlayOneShot(this.fxExplodeAudio.clip, 1f);
				this.particleInfoDict.Remove(num);
			}
		}
		this.currentParticles.Clear();
		for (int j = 0; j < particles; j++)
		{
			if (j == 0)
			{
				this.movingFxAudio.transform.position = this.fxParticleArray[j].position;
			}
			if (this.particleInfoDict.TryGetValue(this.fxParticleArray[j].randomSeed, out this.outPosition))
			{
				this.particleInfoDict[this.fxParticleArray[j].randomSeed] = this.fxParticleArray[j].position;
			}
			else
			{
				this.particleInfoDict.Add(this.fxParticleArray[j].randomSeed, this.fxParticleArray[j].position);
				if (j == 0 && !this.movingFxAudio.isPlaying)
				{
					this.movingFxAudio.GTPlay();
				}
			}
			this.currentParticles.Add(this.fxParticleArray[j].randomSeed);
		}
	}

	// Token: 0x04003030 RID: 12336
	private ParticleSystem.Particle[] fxParticleArray = new ParticleSystem.Particle[20];

	// Token: 0x04003031 RID: 12337
	public AudioSource movingFxAudio;

	// Token: 0x04003032 RID: 12338
	public AudioSource fxExplodeAudio;

	// Token: 0x04003033 RID: 12339
	private List<uint> currentParticles = new List<uint>();

	// Token: 0x04003034 RID: 12340
	private Dictionary<uint, Vector3> particleInfoDict = new Dictionary<uint, Vector3>();

	// Token: 0x04003035 RID: 12341
	private Vector3 outPosition;
}
