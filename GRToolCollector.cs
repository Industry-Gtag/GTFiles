using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

// Token: 0x0200080B RID: 2059
public class GRToolCollector : MonoBehaviour, IGameEntityDebugComponent, IGameEntityComponent
{
	// Token: 0x0600349C RID: 13468 RVA: 0x00120C4D File Offset: 0x0011EE4D
	private void Awake()
	{
		this.state = GRToolCollector.State.Idle;
		this.stateTimeRemaining = -1f;
	}

	// Token: 0x0600349D RID: 13469 RVA: 0x00120C61 File Offset: 0x0011EE61
	private void OnEnable()
	{
		this.SetState(GRToolCollector.State.Idle);
	}

	// Token: 0x0600349E RID: 13470 RVA: 0x00120C6A File Offset: 0x0011EE6A
	public void OnEntityInit()
	{
		if (this.tool != null)
		{
			this.tool.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(this.tool);
		}
		this.lastRechargeTime = (double)Time.time;
	}

	// Token: 0x0600349F RID: 13471 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060034A0 RID: 13472 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060034A1 RID: 13473 RVA: 0x00120CAC File Offset: 0x0011EEAC
	private void OnToolUpgraded(GRTool tool)
	{
		this.rechargeRate = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.RechargeRate);
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.CollectorBonus1))
		{
			this.vacuumSound = this.upgrade1vacuumSound;
			this.vacuumParticleEffect = this.upgrade1VacuumParticleEffect;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.CollectorBonus2))
		{
			this.vacuumSound = this.upgrade2vacuumSound;
			this.vacuumParticleEffect = this.upgrade2VacuumParticleEffect;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.CollectorBonus3))
		{
			this.vacuumSound = this.upgrade3vacuumSound;
			this.vacuumParticleEffect = this.upgrade3VacuumParticleEffect;
		}
	}

	// Token: 0x060034A2 RID: 13474 RVA: 0x00120D34 File Offset: 0x0011EF34
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x060034A3 RID: 13475 RVA: 0x00120D4D File Offset: 0x0011EF4D
	public void OnUpdate(float dt)
	{
		if (this.IsHeldLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x060034A4 RID: 13476 RVA: 0x00120D70 File Offset: 0x0011EF70
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeldLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x060034A5 RID: 13477 RVA: 0x00120DA4 File Offset: 0x0011EFA4
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolCollector.State.Idle:
		{
			bool flag = this.IsButtonHeld();
			this.waitingForButtonRelease = this.waitingForButtonRelease && flag;
			if (flag && !this.waitingForButtonRelease)
			{
				this.SetStateAuthority(GRToolCollector.State.Vacuuming);
				this.activatedLocally = true;
			}
			if (this.rechargeRate > 0f && Time.timeAsDouble > this.lastRechargeTime + (double)this.rechargeInterval)
			{
				this.gameEntity.manager.ghostReactorManager.RequestChargeTool(this.gameEntity.id, this.gameEntity.id, (int)(this.rechargeRate * this.rechargeInterval), false);
				this.lastRechargeTime = Time.timeAsDouble;
				if (this.passiveChargeParticleEffect != null)
				{
					this.passiveChargeParticleEffect.Play();
					return;
				}
			}
			break;
		}
		case GRToolCollector.State.Vacuuming:
		{
			bool flag2 = this.IsButtonHeld();
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolCollector.State.Collect);
				return;
			}
			if (!flag2)
			{
				this.SetStateAuthority(GRToolCollector.State.Idle);
				this.activatedLocally = false;
				return;
			}
			break;
		}
		case GRToolCollector.State.Collect:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolCollector.State.Cooldown);
				return;
			}
			break;
		case GRToolCollector.State.Cooldown:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.activatedLocally = false;
				this.waitingForButtonRelease = true;
				this.SetStateAuthority(GRToolCollector.State.Idle);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060034A6 RID: 13478 RVA: 0x00120F18 File Offset: 0x0011F118
	private void OnUpdateRemote(float dt)
	{
		GRToolCollector.State state = (GRToolCollector.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x060034A7 RID: 13479 RVA: 0x00120F42 File Offset: 0x0011F142
	private void SetStateAuthority(GRToolCollector.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060034A8 RID: 13480 RVA: 0x00120F64 File Offset: 0x0011F164
	private void SetState(GRToolCollector.State newState)
	{
		this.state = newState;
		switch (this.state)
		{
		case GRToolCollector.State.Idle:
			this.StopVacuum();
			this.stateTimeRemaining = -1f;
			this.lastRechargeTime = (double)Time.time;
			return;
		case GRToolCollector.State.Vacuuming:
			this.StartVacuum();
			this.stateTimeRemaining = this.chargeDuration;
			return;
		case GRToolCollector.State.Collect:
			this.TryCollect();
			this.stateTimeRemaining = this.collectDuration;
			return;
		case GRToolCollector.State.Cooldown:
			this.stateTimeRemaining = this.cooldownDuration;
			return;
		default:
			return;
		}
	}

	// Token: 0x060034A9 RID: 13481 RVA: 0x00120FE8 File Offset: 0x0011F1E8
	private void StartVacuum()
	{
		this.vacuumAudioSource.clip = this.vacuumSound;
		this.vacuumAudioSource.volume = this.vacuumSoundVolume;
		this.vacuumAudioSource.loop = true;
		this.vacuumAudioSource.Play();
		this.vacuumParticleEffect.Play();
		if (this.IsHeldLocal())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, this.chargeDuration);
		}
	}

	// Token: 0x060034AA RID: 13482 RVA: 0x00121057 File Offset: 0x0011F257
	private void StopVacuum()
	{
		this.vacuumAudioSource.loop = false;
		this.vacuumAudioSource.Stop();
		this.vacuumParticleEffect.Stop();
	}

	// Token: 0x060034AB RID: 13483 RVA: 0x0012107C File Offset: 0x0011F27C
	private void TryCollect()
	{
		if (this.IsHeldLocal())
		{
			int num = Physics.SphereCastNonAlloc(this.shootFrom.position, 0.2f, this.shootFrom.rotation * Vector3.forward, this.tempHitResults, 1f, this.collectibleLayerMask);
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this.tempHitResults[i];
				GameObject gameObject = null;
				Rigidbody attachedRigidbody = raycastHit.collider.attachedRigidbody;
				if (attachedRigidbody != null)
				{
					gameObject = attachedRigidbody.gameObject;
				}
				else
				{
					GameEntity gameEntity = GameEntity.Get(raycastHit.collider);
					if (gameEntity != null)
					{
						gameObject = gameEntity.gameObject;
					}
				}
				if (gameObject != null)
				{
					GRCollectible component = gameObject.GetComponent<GRCollectible>();
					if (component != null && component.type != ProgressionManager.CoreType.ChaosSeed && this.tool.energy < this.tool.GetEnergyMax())
					{
						GhostReactorManager.Get(this.gameEntity).RequestCollectItem(component.entity.id, this.gameEntity.id);
						return;
					}
				}
			}
			for (int j = 0; j < num; j++)
			{
				RaycastHit raycastHit2 = this.tempHitResults[j];
				GameObject gameObject2 = null;
				Rigidbody attachedRigidbody2 = raycastHit2.collider.attachedRigidbody;
				if (attachedRigidbody2 != null)
				{
					gameObject2 = attachedRigidbody2.gameObject;
				}
				else
				{
					GameEntity gameEntity2 = GameEntity.Get(raycastHit2.collider);
					if (gameEntity2 != null)
					{
						gameObject2 = gameEntity2.gameObject;
					}
				}
				if (gameObject2 != null)
				{
					if (gameObject2.GetComponent<GRCurrencyDepositor>() != null)
					{
						if (this.tool.energy > 0)
						{
							GhostReactorManager.Get(this.gameEntity).RequestDepositCurrency(this.gameEntity.id);
						}
						return;
					}
					GRTool component2 = gameObject2.GetComponent<GRTool>();
					if (!(component2 == null) && !(component2 == this.tool))
					{
						GameEntity component3 = gameObject2.GetComponent<GameEntity>();
						if (component2 != null && component3 != null)
						{
							GhostReactorManager.Get(this.gameEntity).RequestChargeTool(this.gameEntity.id, component3.id, 0, true);
							if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.CollectorBonus3) && this.tool.energy > 50)
							{
								List<GRTool> list = new List<GRTool>();
								this.gameEntity.manager.GetEntitiesWithComponentInRadius<GRTool>(base.transform.position, this.level3ChargeRadius, true, list);
								for (int k = 0; k < list.Count; k++)
								{
									GRTool grtool = list[k];
									if (!(grtool.GetComponent<GRToolCollector>() != null) && !(grtool.gameEntity == this.gameEntity) && !(grtool.gameEntity == component3))
									{
										GhostReactorManager.Get(this.gameEntity).RequestChargeTool(this.gameEntity.id, grtool.gameEntity.id, 0, false);
									}
								}
							}
							return;
						}
					}
				}
			}
		}
	}

	// Token: 0x060034AC RID: 13484 RVA: 0x0012138C File Offset: 0x0011F58C
	public void PerformCollection(GRCollectible collectible)
	{
		this.tool.RefillEnergy(collectible.energyValue + this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HarvestGain), collectible.entity.id);
		this.collectAudioSource.volume = this.collectSoundVolume;
		this.collectAudioSource.PlayOneShot(this.collectSound);
	}

	// Token: 0x060034AD RID: 13485 RVA: 0x001213E4 File Offset: 0x0011F5E4
	public void PlayChargeEffect(GRTool targetTool)
	{
		if (targetTool == null)
		{
			return;
		}
		if (targetTool == this.tool)
		{
			return;
		}
		this.collectAudioSource.volume = this.chargeBeamVolume;
		this.collectAudioSource.PlayOneShot(this.chargeBeamSound);
		for (int i = 0; i < targetTool.energyMeters.Count; i++)
		{
			if (targetTool.energyMeters[i].chargePoint != null)
			{
				this.lightningDispatcher.DispatchLightning(this.lightningDispatcher.transform.position, targetTool.energyMeters[i].chargePoint.position);
			}
			else
			{
				this.lightningDispatcher.DispatchLightning(this.lightningDispatcher.transform.position, targetTool.energyMeters[i].transform.position);
			}
		}
	}

	// Token: 0x060034AE RID: 13486 RVA: 0x001214C8 File Offset: 0x0011F6C8
	public void PlayChargeEffect(GRCurrencyDepositor targetDepositor)
	{
		if (targetDepositor == null)
		{
			return;
		}
		this.collectAudioSource.volume = this.chargeBeamVolume;
		this.collectAudioSource.PlayOneShot(this.chargeBeamSound);
		this.lightningDispatcher.DispatchLightning(this.lightningDispatcher.transform.position, targetDepositor.depositingChargePoint.position);
	}

	// Token: 0x060034AF RID: 13487 RVA: 0x00121528 File Offset: 0x0011F728
	private bool IsButtonHeld()
	{
		if (!this.IsHeldLocal())
		{
			return false;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? XRNode.LeftHand : XRNode.RightHand) > 0.25f;
	}

	// Token: 0x060034B0 RID: 13488 RVA: 0x00121588 File Offset: 0x0011F788
	private void PlayVibration(float strength, float duration)
	{
		if (!this.IsHeldLocal())
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), strength, duration);
	}

	// Token: 0x060034B1 RID: 13489 RVA: 0x001215DC File Offset: 0x0011F7DC
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("Recharge Rate: <color=\"yellow\">{0}<color=\"white\">", this.rechargeRate));
	}

	// Token: 0x0400446E RID: 17518
	public GameEntity gameEntity;

	// Token: 0x0400446F RID: 17519
	public GRTool tool;

	// Token: 0x04004470 RID: 17520
	public GRAttributes attributes;

	// Token: 0x04004471 RID: 17521
	public int energyDepositPerUse = 100;

	// Token: 0x04004472 RID: 17522
	public Transform shootFrom;

	// Token: 0x04004473 RID: 17523
	public LayerMask collectibleLayerMask;

	// Token: 0x04004474 RID: 17524
	public ParticleSystem vacuumParticleEffect;

	// Token: 0x04004475 RID: 17525
	public ParticleSystem upgrade1VacuumParticleEffect;

	// Token: 0x04004476 RID: 17526
	public ParticleSystem upgrade2VacuumParticleEffect;

	// Token: 0x04004477 RID: 17527
	public ParticleSystem upgrade3VacuumParticleEffect;

	// Token: 0x04004478 RID: 17528
	public ParticleSystem passiveChargeParticleEffect;

	// Token: 0x04004479 RID: 17529
	public AudioSource vacuumAudioSource;

	// Token: 0x0400447A RID: 17530
	public AudioClip vacuumSound;

	// Token: 0x0400447B RID: 17531
	public AudioClip upgrade1vacuumSound;

	// Token: 0x0400447C RID: 17532
	public AudioClip upgrade2vacuumSound;

	// Token: 0x0400447D RID: 17533
	public AudioClip upgrade3vacuumSound;

	// Token: 0x0400447E RID: 17534
	public float vacuumSoundVolume = 0.2f;

	// Token: 0x0400447F RID: 17535
	public AudioSource collectAudioSource;

	// Token: 0x04004480 RID: 17536
	[FormerlySerializedAs("flashSound")]
	public AudioClip collectSound;

	// Token: 0x04004481 RID: 17537
	[FormerlySerializedAs("flashSoundVolume")]
	public float collectSoundVolume = 1f;

	// Token: 0x04004482 RID: 17538
	public AudioClip chargeBeamSound;

	// Token: 0x04004483 RID: 17539
	public float chargeBeamVolume = 0.2f;

	// Token: 0x04004484 RID: 17540
	public LightningDispatcher lightningDispatcher;

	// Token: 0x04004485 RID: 17541
	public float chargeDuration = 0.75f;

	// Token: 0x04004486 RID: 17542
	[FormerlySerializedAs("flashDuration")]
	public float collectDuration = 0.1f;

	// Token: 0x04004487 RID: 17543
	public float cooldownDuration;

	// Token: 0x04004488 RID: 17544
	public AbilityHaptic collectHaptic;

	// Token: 0x04004489 RID: 17545
	[NonSerialized]
	public GhostReactorManager grManager;

	// Token: 0x0400448A RID: 17546
	private float rechargeRate;

	// Token: 0x0400448B RID: 17547
	public float rechargeInterval = 1f;

	// Token: 0x0400448C RID: 17548
	private double lastRechargeTime;

	// Token: 0x0400448D RID: 17549
	public float level3ChargeRadius = 4f;

	// Token: 0x0400448E RID: 17550
	private GRToolCollector.State state;

	// Token: 0x0400448F RID: 17551
	private float stateTimeRemaining;

	// Token: 0x04004490 RID: 17552
	private bool activatedLocally;

	// Token: 0x04004491 RID: 17553
	private bool waitingForButtonRelease;

	// Token: 0x04004492 RID: 17554
	private RaycastHit[] tempHitResults = new RaycastHit[128];

	// Token: 0x0200080C RID: 2060
	private enum State
	{
		// Token: 0x04004494 RID: 17556
		Idle,
		// Token: 0x04004495 RID: 17557
		Vacuuming,
		// Token: 0x04004496 RID: 17558
		Collect,
		// Token: 0x04004497 RID: 17559
		Cooldown
	}
}
