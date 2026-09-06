using System;
using UnityEngine;

// Token: 0x0200008F RID: 143
public class PeriodicNoiseGenerator : MonoBehaviour
{
	// Token: 0x06000391 RID: 913 RVA: 0x00014CA5 File Offset: 0x00012EA5
	private void Awake()
	{
		this.noiseActor = base.GetComponentInParent<CrittersLoudNoise>();
		this.lastTime = Time.time;
		this.mR = base.GetComponentInChildren<MeshRenderer>();
	}

	// Token: 0x06000392 RID: 914 RVA: 0x00014CCC File Offset: 0x00012ECC
	private void Update()
	{
		if (!CrittersManager.instance.LocalAuthority())
		{
			return;
		}
		if (Time.time > this.lastTime + this.sleepDuration)
		{
			this.lastTime = Time.time + this.randomDuration * Random.value;
			this.noiseActor.SetTimeEnabled();
			this.noiseActor.soundEnabled = true;
			this.mR.sharedMaterial = this.solid;
		}
		if (!this.noiseActor.soundEnabled && this.mR.sharedMaterial != this.transparent)
		{
			this.mR.sharedMaterial = this.transparent;
		}
	}

	// Token: 0x04000413 RID: 1043
	public float sleepDuration;

	// Token: 0x04000414 RID: 1044
	public float randomDuration;

	// Token: 0x04000415 RID: 1045
	public float lastTime;

	// Token: 0x04000416 RID: 1046
	private CrittersLoudNoise noiseActor;

	// Token: 0x04000417 RID: 1047
	public Material transparent;

	// Token: 0x04000418 RID: 1048
	public Material solid;

	// Token: 0x04000419 RID: 1049
	private MeshRenderer mR;
}
