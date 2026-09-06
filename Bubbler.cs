using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020005A6 RID: 1446
public class Bubbler : TransferrableObject
{
	// Token: 0x060024A0 RID: 9376 RVA: 0x000C4A24 File Offset: 0x000C2C24
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.hasParticleSystem = this.bubbleParticleSystem != null;
		if (this.hasParticleSystem)
		{
			this.bubbleParticleArray = new ParticleSystem.Particle[this.bubbleParticleSystem.main.maxParticles];
			this.bubbleParticleSystem.trigger.SetCollider(0, GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<SphereCollider>());
			this.bubbleParticleSystem.trigger.SetCollider(1, GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<SphereCollider>());
		}
		this.initialTriggerDuration = 0.05f;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x060024A1 RID: 9377 RVA: 0x000C4AC8 File Offset: 0x000C2CC8
	internal override void OnEnable()
	{
		base.OnEnable();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.hasBubblerAudio = this.bubblerAudio != null && this.bubblerAudio.clip != null;
		this.hasPopBubbleAudio = this.popBubbleAudio != null && this.popBubbleAudio.clip != null;
		this.hasFan = this.fan != null;
		this.hasActiveOnlyComponent = this.gameObjectActiveOnlyWhileTriggerDown != null;
	}

	// Token: 0x060024A2 RID: 9378 RVA: 0x000C4B58 File Offset: 0x000C2D58
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
		{
			this.bubbleParticleSystem.Stop();
		}
		if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
		{
			this.bubblerAudio.GTStop();
		}
	}

	// Token: 0x060024A3 RID: 9379 RVA: 0x000C4BAC File Offset: 0x000C2DAC
	internal override void OnDisable()
	{
		base.OnDisable();
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
		{
			this.bubbleParticleSystem.Stop();
		}
		if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
		{
			this.bubblerAudio.GTStop();
		}
		this.currentParticles.Clear();
		this.particleInfoDict.Clear();
	}

	// Token: 0x060024A4 RID: 9380 RVA: 0x000C4C1C File Offset: 0x000C2E1C
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x060024A5 RID: 9381 RVA: 0x000C4C2A File Offset: 0x000C2E2A
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!this._worksInWater && GTPlayer.Instance.InWater)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
	}

	// Token: 0x060024A6 RID: 9382 RVA: 0x000C4C50 File Offset: 0x000C2E50
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (!this.IsMyItem() && base.myOnlineRig != null && base.myOnlineRig.muted)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
		bool flag2 = this.itemState != TransferrableObject.ItemStates.State0;
		Behaviour[] array = this.behavioursToEnableWhenTriggerPressed;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = flag2;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
			{
				this.bubbleParticleSystem.Stop();
			}
			if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
			{
				this.bubblerAudio.GTStop();
			}
			if (this.hasActiveOnlyComponent)
			{
				this.gameObjectActiveOnlyWhileTriggerDown.SetActive(false);
			}
		}
		else
		{
			if (this.hasParticleSystem && !this.bubbleParticleSystem.isEmitting)
			{
				this.bubbleParticleSystem.Play();
			}
			if (this.hasBubblerAudio && !this.bubblerAudio.isPlaying)
			{
				this.bubblerAudio.GTPlay();
			}
			if (this.hasActiveOnlyComponent && !this.gameObjectActiveOnlyWhileTriggerDown.activeSelf)
			{
				this.gameObjectActiveOnlyWhileTriggerDown.SetActive(true);
			}
			if (this.IsMyItem())
			{
				this.initialTriggerPull = Time.time;
				GorillaTagger.Instance.StartVibration(flag, this.triggerStrength, this.initialTriggerDuration);
				if (Time.time > this.initialTriggerPull + this.initialTriggerDuration)
				{
					GorillaTagger.Instance.StartVibration(flag, this.ongoingStrength, Time.deltaTime);
				}
			}
			if (this.hasFan)
			{
				if (!this.fanYaxisinstead)
				{
					float num = this.fan.transform.localEulerAngles.z + this.rotationSpeed * Time.fixedDeltaTime;
					this.fan.transform.localEulerAngles = new Vector3(0f, 0f, num);
				}
				else
				{
					float num2 = this.fan.transform.localEulerAngles.y + this.rotationSpeed * Time.fixedDeltaTime;
					this.fan.transform.localEulerAngles = new Vector3(0f, num2, 0f);
				}
			}
		}
		if (this.hasParticleSystem && (!this.allBubblesPopped || this.itemState == TransferrableObject.ItemStates.State1))
		{
			int particles = this.bubbleParticleSystem.GetParticles(this.bubbleParticleArray);
			this.allBubblesPopped = particles <= 0;
			if (!this.allBubblesPopped)
			{
				for (int j = 0; j < particles; j++)
				{
					if (this.currentParticles.Contains(this.bubbleParticleArray[j].randomSeed))
					{
						this.currentParticles.Remove(this.bubbleParticleArray[j].randomSeed);
					}
				}
				foreach (uint num3 in this.currentParticles)
				{
					if (this.particleInfoDict.TryGetValue(num3, out this.outPosition))
					{
						if (this.hasPopBubbleAudio)
						{
							GTAudioSourceExtensions.GTPlayClipAtPoint(this.popBubbleAudio.clip, this.outPosition);
						}
						this.particleInfoDict.Remove(num3);
					}
				}
				this.currentParticles.Clear();
				for (int k = 0; k < particles; k++)
				{
					if (this.particleInfoDict.TryGetValue(this.bubbleParticleArray[k].randomSeed, out this.outPosition))
					{
						this.particleInfoDict[this.bubbleParticleArray[k].randomSeed] = this.bubbleParticleArray[k].position;
					}
					else
					{
						this.particleInfoDict.Add(this.bubbleParticleArray[k].randomSeed, this.bubbleParticleArray[k].position);
					}
					this.currentParticles.Add(this.bubbleParticleArray[k].randomSeed);
				}
			}
		}
	}

	// Token: 0x060024A7 RID: 9383 RVA: 0x000C505C File Offset: 0x000C325C
	public override void OnActivate()
	{
		base.OnActivate();
		this.itemState = TransferrableObject.ItemStates.State1;
	}

	// Token: 0x060024A8 RID: 9384 RVA: 0x0004D2DC File Offset: 0x0004B4DC
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x060024A9 RID: 9385 RVA: 0x000C506B File Offset: 0x000C326B
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x060024AA RID: 9386 RVA: 0x000C5076 File Offset: 0x000C3276
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04003012 RID: 12306
	[SerializeField]
	private bool _worksInWater = true;

	// Token: 0x04003013 RID: 12307
	public ParticleSystem bubbleParticleSystem;

	// Token: 0x04003014 RID: 12308
	private ParticleSystem.Particle[] bubbleParticleArray;

	// Token: 0x04003015 RID: 12309
	public AudioSource bubblerAudio;

	// Token: 0x04003016 RID: 12310
	public AudioSource popBubbleAudio;

	// Token: 0x04003017 RID: 12311
	private List<uint> currentParticles = new List<uint>();

	// Token: 0x04003018 RID: 12312
	private Dictionary<uint, Vector3> particleInfoDict = new Dictionary<uint, Vector3>();

	// Token: 0x04003019 RID: 12313
	private Vector3 outPosition;

	// Token: 0x0400301A RID: 12314
	private bool allBubblesPopped;

	// Token: 0x0400301B RID: 12315
	public bool disableActivation;

	// Token: 0x0400301C RID: 12316
	public bool disableDeactivation;

	// Token: 0x0400301D RID: 12317
	public float rotationSpeed = 5f;

	// Token: 0x0400301E RID: 12318
	public GameObject fan;

	// Token: 0x0400301F RID: 12319
	public bool fanYaxisinstead;

	// Token: 0x04003020 RID: 12320
	public float ongoingStrength = 0.005f;

	// Token: 0x04003021 RID: 12321
	public float triggerStrength = 0.2f;

	// Token: 0x04003022 RID: 12322
	private float initialTriggerPull;

	// Token: 0x04003023 RID: 12323
	private float initialTriggerDuration;

	// Token: 0x04003024 RID: 12324
	private bool hasBubblerAudio;

	// Token: 0x04003025 RID: 12325
	private bool hasPopBubbleAudio;

	// Token: 0x04003026 RID: 12326
	public GameObject gameObjectActiveOnlyWhileTriggerDown;

	// Token: 0x04003027 RID: 12327
	public Behaviour[] behavioursToEnableWhenTriggerPressed;

	// Token: 0x04003028 RID: 12328
	private bool hasParticleSystem;

	// Token: 0x04003029 RID: 12329
	private bool hasFan;

	// Token: 0x0400302A RID: 12330
	private bool hasActiveOnlyComponent;

	// Token: 0x020005A7 RID: 1447
	private enum BubblerState
	{
		// Token: 0x0400302C RID: 12332
		None = 1,
		// Token: 0x0400302D RID: 12333
		Bubbling
	}
}
