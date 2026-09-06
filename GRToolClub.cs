using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000809 RID: 2057
public class GRToolClub : MonoBehaviourTick, IGameHitter, IGameEntityDebugComponent, IGameEntityComponent
{
	// Token: 0x0600348B RID: 13451 RVA: 0x0012068D File Offset: 0x0011E88D
	private void Awake()
	{
		this.retractableSection.localPosition = new Vector3(0f, 0f, 0f);
	}

	// Token: 0x0600348C RID: 13452 RVA: 0x001206AE File Offset: 0x0011E8AE
	public new void OnEnable()
	{
		base.OnEnable();
		this.SetExtendedAmount(0f);
		this.gameHitter.hitFx = this.noPowerFx;
		this.gameHitter.damageAttribute = this.noPowerAttribute;
		this.SetState(GRToolClub.State.Idle);
	}

	// Token: 0x0600348D RID: 13453 RVA: 0x001206EA File Offset: 0x0011E8EA
	public void OnEntityInit()
	{
		if (this.tool != null)
		{
			this.tool.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(this.tool);
		}
	}

	// Token: 0x0600348E RID: 13454 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x0600348F RID: 13455 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06003490 RID: 13456 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnToolUpgraded(GRTool tool)
	{
	}

	// Token: 0x06003491 RID: 13457 RVA: 0x00120720 File Offset: 0x0011E920
	private void EnableImpactVFXForCurrentUpgradeLevel()
	{
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.BatonDamage1))
		{
			this.gameHitter.hitFx = this.upgrade1ImpactVFX;
			return;
		}
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.BatonDamage2))
		{
			this.gameHitter.hitFx = this.upgrade2ImpactVFX;
			return;
		}
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.BatonDamage3))
		{
			this.gameHitter.hitFx = this.upgrade3ImpactVFX;
			return;
		}
		this.gameHitter.hitFx = this.poweredImpactFx;
	}

	// Token: 0x06003492 RID: 13458 RVA: 0x001207A0 File Offset: 0x0011E9A0
	public override void Tick()
	{
		float deltaTime = Time.deltaTime;
		if (this.gameEntity.IsHeld())
		{
			if (this.gameEntity.IsHeldByLocalPlayer())
			{
				this.OnUpdateAuthority(deltaTime);
			}
			else
			{
				this.OnUpdateRemote(deltaTime);
			}
		}
		else
		{
			this.SetState(GRToolClub.State.Idle);
		}
		this.OnUpdateShared(deltaTime);
	}

	// Token: 0x06003493 RID: 13459 RVA: 0x001207F0 File Offset: 0x0011E9F0
	private void OnUpdateAuthority(float dt)
	{
		GRToolClub.State state = this.state;
		if (state != GRToolClub.State.Idle)
		{
			if (state != GRToolClub.State.Extended)
			{
				return;
			}
			if (!this.IsButtonHeld() || !this.tool.HasEnoughEnergy())
			{
				this.SetState(GRToolClub.State.Idle);
			}
		}
		else if (this.IsButtonHeld() && this.tool.HasEnoughEnergy())
		{
			this.SetState(GRToolClub.State.Extended);
			return;
		}
	}

	// Token: 0x06003494 RID: 13460 RVA: 0x00120850 File Offset: 0x0011EA50
	private void OnUpdateRemote(float dt)
	{
		GRToolClub.State state = (GRToolClub.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06003495 RID: 13461 RVA: 0x0012087C File Offset: 0x0011EA7C
	private void OnUpdateShared(float dt)
	{
		GRToolClub.State state = this.state;
		if (state != GRToolClub.State.Idle)
		{
			if (state != GRToolClub.State.Extended)
			{
				return;
			}
			if (this.extendedAmount < 1f)
			{
				float num = Mathf.MoveTowards(this.extendedAmount, 1f, 1f / this.extensionTime * Time.deltaTime);
				this.SetExtendedAmount(num);
			}
		}
		else if (this.extendedAmount > 0f)
		{
			float num2 = Mathf.MoveTowards(this.extendedAmount, 0f, 1f / this.extensionTime * Time.deltaTime);
			this.SetExtendedAmount(num2);
			return;
		}
	}

	// Token: 0x06003496 RID: 13462 RVA: 0x00120908 File Offset: 0x0011EB08
	private void SetExtendedAmount(float newExtendedAmount)
	{
		this.extendedAmount = newExtendedAmount;
		float num = Mathf.Lerp(this.retractableSectionMin, this.retractableSectionMax, this.extendedAmount);
		this.retractableSection.localPosition = new Vector3(0f, num, 0f);
	}

	// Token: 0x06003497 RID: 13463 RVA: 0x00120950 File Offset: 0x0011EB50
	private void SetState(GRToolClub.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		GRToolClub.State state = this.state;
		if (state != GRToolClub.State.Idle)
		{
		}
		this.state = newState;
		state = this.state;
		if (state != GRToolClub.State.Idle)
		{
			if (state == GRToolClub.State.Extended)
			{
				this.idleCollider.enabled = false;
				this.extendedCollider.enabled = true;
				for (int i = 0; i < this.meshAndMaterials.Count; i++)
				{
					MaterialUtils.SwapMaterial(this.meshAndMaterials[i], false);
				}
				this.humAudioSource.Play();
				this.dullLight.SetActive(true);
				this.audioSource.PlayOneShot(this.extendAudio, this.extendVolume);
				for (int j = 0; j < this.humParticleEffects.Count; j++)
				{
					this.humParticleEffects[j].gameObject.SetActive(true);
				}
				this.EnableImpactVFXForCurrentUpgradeLevel();
				this.gameHitter.damageAttribute = this.poweredAttribute;
				this.openHaptic.PlayIfHeldLocal(this.gameEntity);
			}
		}
		else
		{
			this.extendedCollider.enabled = false;
			this.idleCollider.enabled = true;
			for (int k = 0; k < this.meshAndMaterials.Count; k++)
			{
				MaterialUtils.SwapMaterial(this.meshAndMaterials[k], true);
			}
			this.humAudioSource.Stop();
			this.dullLight.SetActive(false);
			this.audioSource.PlayOneShot(this.retractAudio, this.retractVolume);
			for (int l = 0; l < this.humParticleEffects.Count; l++)
			{
				this.humParticleEffects[l].gameObject.SetActive(false);
			}
			this.gameHitter.hitFx = this.noPowerFx;
			this.gameHitter.damageAttribute = this.noPowerAttribute;
			this.closeHaptic.PlayIfHeldLocal(this.gameEntity);
		}
		if (this.gameEntity.IsHeldByLocalPlayer())
		{
			this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
		}
	}

	// Token: 0x06003498 RID: 13464 RVA: 0x00120B54 File Offset: 0x0011ED54
	private bool IsButtonHeld()
	{
		if (!this.gameEntity.IsHeldByLocalPlayer())
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

	// Token: 0x06003499 RID: 13465 RVA: 0x00120BB6 File Offset: 0x0011EDB6
	public void OnSuccessfulHit(GameHitData hitData)
	{
		if (this.state == GRToolClub.State.Extended)
		{
			this.tool.UseEnergy();
		}
	}

	// Token: 0x0600349A RID: 13466 RVA: 0x00120BCC File Offset: 0x0011EDCC
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("Knockback: <color=\"yellow\">x{0}<color=\"white\">", this.gameHitter.knockbackMultiplier));
	}

	// Token: 0x0400444B RID: 17483
	public GameEntity gameEntity;

	// Token: 0x0400444C RID: 17484
	public GameHitter gameHitter;

	// Token: 0x0400444D RID: 17485
	public GRTool tool;

	// Token: 0x0400444E RID: 17486
	public Rigidbody rigidBody;

	// Token: 0x0400444F RID: 17487
	public AudioSource audioSource;

	// Token: 0x04004450 RID: 17488
	public AudioSource humAudioSource;

	// Token: 0x04004451 RID: 17489
	public List<ParticleSystem> humParticleEffects = new List<ParticleSystem>();

	// Token: 0x04004452 RID: 17490
	public GRAttributes attributes;

	// Token: 0x04004453 RID: 17491
	public AudioClip extendAudio;

	// Token: 0x04004454 RID: 17492
	public float extendVolume = 0.5f;

	// Token: 0x04004455 RID: 17493
	public AudioClip retractAudio;

	// Token: 0x04004456 RID: 17494
	public float retractVolume = 0.5f;

	// Token: 0x04004457 RID: 17495
	public GameHitFx noPowerFx;

	// Token: 0x04004458 RID: 17496
	public GameHitFx poweredImpactFx;

	// Token: 0x04004459 RID: 17497
	public GameHitFx upgrade1ImpactVFX;

	// Token: 0x0400445A RID: 17498
	public GameHitFx upgrade2ImpactVFX;

	// Token: 0x0400445B RID: 17499
	public GameHitFx upgrade3ImpactVFX;

	// Token: 0x0400445C RID: 17500
	public GRAttributeType noPowerAttribute;

	// Token: 0x0400445D RID: 17501
	public GRAttributeType poweredAttribute;

	// Token: 0x0400445E RID: 17502
	public float minHitSpeed = 2.25f;

	// Token: 0x0400445F RID: 17503
	public GameObject dullLight;

	// Token: 0x04004460 RID: 17504
	public List<MeshAndMaterials> meshAndMaterials;

	// Token: 0x04004461 RID: 17505
	public Transform retractableSection;

	// Token: 0x04004462 RID: 17506
	public Collider idleCollider;

	// Token: 0x04004463 RID: 17507
	public Collider extendedCollider;

	// Token: 0x04004464 RID: 17508
	public float retractableSectionMin = -0.31f;

	// Token: 0x04004465 RID: 17509
	public float retractableSectionMax;

	// Token: 0x04004466 RID: 17510
	public float extensionTime = 0.15f;

	// Token: 0x04004467 RID: 17511
	[Header("Haptic")]
	public AbilityHaptic openHaptic;

	// Token: 0x04004468 RID: 17512
	public AbilityHaptic closeHaptic;

	// Token: 0x04004469 RID: 17513
	private float extendedAmount;

	// Token: 0x0400446A RID: 17514
	private GRToolClub.State state;

	// Token: 0x0200080A RID: 2058
	private enum State
	{
		// Token: 0x0400446C RID: 17516
		Idle,
		// Token: 0x0400446D RID: 17517
		Extended
	}
}
