using System;
using UnityEngine;

// Token: 0x02000218 RID: 536
public class VacuumHoldable : TransferrableObject
{
	// Token: 0x06000E0E RID: 3598 RVA: 0x0004D0A4 File Offset: 0x0004B2A4
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06000E0F RID: 3599 RVA: 0x0004D0B4 File Offset: 0x0004B2B4
	internal override void OnEnable()
	{
		base.OnEnable();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.hasAudioSource = this.audioSource != null && this.audioSource.clip != null;
	}

	// Token: 0x06000E10 RID: 3600 RVA: 0x0004D0EC File Offset: 0x0004B2EC
	internal override void OnDisable()
	{
		base.OnDisable();
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.particleFX.isPlaying)
		{
			this.particleFX.Stop();
		}
		if (this.hasAudioSource && this.audioSource.isPlaying)
		{
			this.audioSource.GTStop();
		}
	}

	// Token: 0x06000E11 RID: 3601 RVA: 0x0004D140 File Offset: 0x0004B340
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.particleFX.isPlaying)
		{
			this.particleFX.Stop();
		}
		if (this.hasAudioSource && this.audioSource.isPlaying)
		{
			this.audioSource.GTStop();
		}
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x0004D18C File Offset: 0x0004B38C
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x0004D19C File Offset: 0x0004B39C
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (!this.IsMyItem() && base.myOnlineRig != null && base.myOnlineRig.muted)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			if (this.particleFX.isPlaying)
			{
				this.particleFX.Stop();
			}
			if (this.hasAudioSource && this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
				return;
			}
		}
		else
		{
			if (!this.particleFX.isEmitting)
			{
				this.particleFX.Play();
			}
			if (this.hasAudioSource && !this.audioSource.isPlaying)
			{
				this.audioSource.GTPlay();
			}
			if (this.IsMyItem() && Time.time > this.activationStartTime + this.activationVibrationStartDuration)
			{
				GorillaTagger.Instance.StartVibration(this.currentState == TransferrableObject.PositionState.InLeftHand, this.activationVibrationLoopStrength, Time.deltaTime);
			}
		}
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x0004D290 File Offset: 0x0004B490
	public override void OnActivate()
	{
		base.OnActivate();
		this.itemState = TransferrableObject.ItemStates.State1;
		if (this.IsMyItem())
		{
			this.activationStartTime = Time.time;
			GorillaTagger.Instance.StartVibration(this.currentState == TransferrableObject.PositionState.InLeftHand, this.activationVibrationStartStrength, this.activationVibrationStartDuration);
		}
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x0004D2DC File Offset: 0x0004B4DC
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x040010D7 RID: 4311
	[Tooltip("Emission rate will be increase when the trigger button is pressed.")]
	public ParticleSystem particleFX;

	// Token: 0x040010D8 RID: 4312
	[Tooltip("Sound will loop and fade in/out volume when trigger pressed.")]
	public AudioSource audioSource;

	// Token: 0x040010D9 RID: 4313
	private float activationVibrationStartStrength = 0.8f;

	// Token: 0x040010DA RID: 4314
	private float activationVibrationStartDuration = 0.05f;

	// Token: 0x040010DB RID: 4315
	private float activationVibrationLoopStrength = 0.005f;

	// Token: 0x040010DC RID: 4316
	private float activationStartTime;

	// Token: 0x040010DD RID: 4317
	private bool hasAudioSource;

	// Token: 0x02000219 RID: 537
	private enum VacuumState
	{
		// Token: 0x040010DF RID: 4319
		None = 1,
		// Token: 0x040010E0 RID: 4320
		Active
	}
}
