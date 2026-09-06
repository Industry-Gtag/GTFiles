using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200080F RID: 2063
public class GRToolFlash : MonoBehaviour, IGameEntityDebugComponent, IGameEntityComponent
{
	// Token: 0x060034C3 RID: 13507 RVA: 0x00121C2F File Offset: 0x0011FE2F
	private void Awake()
	{
		this.state = GRToolFlash.State.Idle;
		this.stateTimeRemaining = -1f;
		this.gameHitter = base.GetComponent<GameHitter>();
	}

	// Token: 0x060034C4 RID: 13508 RVA: 0x00121C4F File Offset: 0x0011FE4F
	private void OnEnable()
	{
		this.StopFlash();
		this.SetState(GRToolFlash.State.Idle);
	}

	// Token: 0x060034C5 RID: 13509 RVA: 0x00121C5E File Offset: 0x0011FE5E
	public void OnEntityInit()
	{
		if (this.tool != null)
		{
			this.tool.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(this.tool);
		}
	}

	// Token: 0x060034C6 RID: 13510 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060034C7 RID: 13511 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060034C8 RID: 13512 RVA: 0x00121C94 File Offset: 0x0011FE94
	private void OnToolUpgraded(GRTool tool)
	{
		this.stunDuration = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.FlashStunDuration);
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage1))
		{
			this.flashSound = this.upgrade1FlashSound;
			this.flash = this.upgrade1FlashCone;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage2))
		{
			this.flashSound = this.upgrade2FlashSound;
			this.flash = this.upgrade2FlashCone;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage3))
		{
			this.flashSound = this.upgrade3FlashSound;
			this.flash = this.upgrade3FlashCone;
		}
	}

	// Token: 0x060034C9 RID: 13513 RVA: 0x00121D19 File Offset: 0x0011FF19
	private bool IsHeldLocal()
	{
		return this.item.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x060034CA RID: 13514 RVA: 0x00121D32 File Offset: 0x0011FF32
	public void OnUpdate(float dt)
	{
		if (this.IsHeldLocal())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x060034CB RID: 13515 RVA: 0x00121D4C File Offset: 0x0011FF4C
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

	// Token: 0x060034CC RID: 13516 RVA: 0x00121D80 File Offset: 0x0011FF80
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolFlash.State.Idle:
			if (this.tool.HasEnoughEnergy() && this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolFlash.State.Charging);
				this.activatedLocally = true;
				return;
			}
			break;
		case GRToolFlash.State.Charging:
		{
			bool flag = this.IsButtonHeld();
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolFlash.State.Flash);
				return;
			}
			if (!flag)
			{
				this.SetStateAuthority(GRToolFlash.State.Idle);
				this.activatedLocally = false;
				return;
			}
			break;
		}
		case GRToolFlash.State.Flash:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolFlash.State.Cooldown);
				return;
			}
			break;
		case GRToolFlash.State.Cooldown:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f && !this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolFlash.State.Idle);
				this.activatedLocally = false;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060034CD RID: 13517 RVA: 0x00121E68 File Offset: 0x00120068
	private void OnUpdateRemote(float dt)
	{
		GRToolFlash.State state = (GRToolFlash.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			if (this.state == GRToolFlash.State.Charging && state == GRToolFlash.State.Cooldown)
			{
				this.SetState(GRToolFlash.State.Flash);
				return;
			}
			if (this.state == GRToolFlash.State.Flash && state == GRToolFlash.State.Cooldown)
			{
				if (Time.time > this.timeLastFlashed + this.flashDuration)
				{
					this.SetState(GRToolFlash.State.Cooldown);
					return;
				}
			}
			else
			{
				this.SetState(state);
			}
		}
	}

	// Token: 0x060034CE RID: 13518 RVA: 0x00121ED0 File Offset: 0x001200D0
	private void SetStateAuthority(GRToolFlash.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060034CF RID: 13519 RVA: 0x00121EF4 File Offset: 0x001200F4
	private void SetState(GRToolFlash.State newState)
	{
		if (!this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case GRToolFlash.State.Idle:
			this.stateTimeRemaining = -1f;
			return;
		case GRToolFlash.State.Charging:
			this.StartCharge();
			this.stateTimeRemaining = this.chargeDuration;
			return;
		case GRToolFlash.State.Flash:
			this.StartFlash();
			this.stateTimeRemaining = this.flashDuration;
			return;
		case GRToolFlash.State.Cooldown:
			this.StopFlash();
			this.stateTimeRemaining = this.cooldownDuration;
			return;
		default:
			return;
		}
	}

	// Token: 0x060034D0 RID: 13520 RVA: 0x00121F78 File Offset: 0x00120178
	private void StartCharge()
	{
		this.audioSource.volume = this.chargeSoundVolume;
		this.audioSource.clip = this.chargeSound;
		this.audioSource.Play();
		if (this.IsHeldLocal())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, this.chargeDuration);
		}
	}

	// Token: 0x060034D1 RID: 13521 RVA: 0x00121FD0 File Offset: 0x001201D0
	private void StartFlash()
	{
		this.flash.SetActive(true);
		this.audioSource.volume = this.flashSoundVolume;
		this.audioSource.clip = this.flashSound;
		this.audioSource.Play();
		this.tool.UseEnergy();
		this.timeLastFlashed = Time.time;
		if (this.IsHeldLocal())
		{
			int num = Physics.SphereCastNonAlloc(this.shootFrom.position, 1f, this.shootFrom.rotation * Vector3.forward, this.tempHitResults, 5f, this.enemyLayerMask);
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this.tempHitResults[i];
				Rigidbody attachedRigidbody = raycastHit.collider.attachedRigidbody;
				if (attachedRigidbody != null)
				{
					GameHittable component = attachedRigidbody.GetComponent<GameHittable>();
					if (component != null && this.gameHitter != null)
					{
						GameHitData gameHitData = new GameHitData
						{
							hitTypeId = 1,
							hitEntityId = component.gameEntity.id,
							hitByEntityId = this.gameEntity.id,
							hitEntityPosition = component.gameEntity.transform.position,
							hitPosition = ((raycastHit.distance == 0f) ? this.shootFrom.position : raycastHit.point),
							hitImpulse = Vector3.zero,
							hitAmount = this.gameHitter.CalcHitAmount(GameHitType.Flash, component, this.gameEntity),
							hittablePoint = component.FindHittablePoint(raycastHit.collider)
						};
						component.RequestHit(gameHitData);
					}
				}
			}
		}
	}

	// Token: 0x060034D2 RID: 13522 RVA: 0x00122195 File Offset: 0x00120395
	private void StopFlash()
	{
		this.flash.SetActive(false);
	}

	// Token: 0x060034D3 RID: 13523 RVA: 0x001221A4 File Offset: 0x001203A4
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
		int num = gamePlayer.FindHandIndex(this.item.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? XRNode.LeftHand : XRNode.RightHand) > 0.25f;
	}

	// Token: 0x060034D4 RID: 13524 RVA: 0x00122204 File Offset: 0x00120404
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
		int num = gamePlayer.FindHandIndex(this.item.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), strength, duration);
	}

	// Token: 0x060034D5 RID: 13525 RVA: 0x00122258 File Offset: 0x00120458
	public bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 4L && ((int)newStateIndex != 2 || Time.time > this.timeLastFlashed + this.cooldownMinimum);
	}

	// Token: 0x060034D6 RID: 13526 RVA: 0x00122281 File Offset: 0x00120481
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("Stun Duration: <color=\"yellow\">{0}<color=\"white\">", this.stunDuration));
	}

	// Token: 0x040044B7 RID: 17591
	public GameEntity gameEntity;

	// Token: 0x040044B8 RID: 17592
	public GRTool tool;

	// Token: 0x040044B9 RID: 17593
	public GRAttributes attributes;

	// Token: 0x040044BA RID: 17594
	public GameObject flash;

	// Token: 0x040044BB RID: 17595
	public Transform shootFrom;

	// Token: 0x040044BC RID: 17596
	public LayerMask enemyLayerMask;

	// Token: 0x040044BD RID: 17597
	public AudioSource audioSource;

	// Token: 0x040044BE RID: 17598
	public AudioClip chargeSound;

	// Token: 0x040044BF RID: 17599
	public float chargeSoundVolume = 0.2f;

	// Token: 0x040044C0 RID: 17600
	public AudioClip flashSound;

	// Token: 0x040044C1 RID: 17601
	public AudioClip upgrade1FlashSound;

	// Token: 0x040044C2 RID: 17602
	public AudioClip upgrade2FlashSound;

	// Token: 0x040044C3 RID: 17603
	public AudioClip upgrade3FlashSound;

	// Token: 0x040044C4 RID: 17604
	public GameObject upgrade1FlashCone;

	// Token: 0x040044C5 RID: 17605
	public GameObject upgrade2FlashCone;

	// Token: 0x040044C6 RID: 17606
	public GameObject upgrade3FlashCone;

	// Token: 0x040044C7 RID: 17607
	public float flashSoundVolume = 1f;

	// Token: 0x040044C8 RID: 17608
	public float stunDuration;

	// Token: 0x040044C9 RID: 17609
	public GRToolFlash.UpgradeTypes upgradesApplied;

	// Token: 0x040044CA RID: 17610
	public float chargeDuration = 0.75f;

	// Token: 0x040044CB RID: 17611
	public float flashDuration = 0.1f;

	// Token: 0x040044CC RID: 17612
	public float cooldownDuration;

	// Token: 0x040044CD RID: 17613
	private float timeLastFlashed;

	// Token: 0x040044CE RID: 17614
	private float cooldownMinimum = 0.35f;

	// Token: 0x040044CF RID: 17615
	private bool activatedLocally;

	// Token: 0x040044D0 RID: 17616
	public GameEntity item;

	// Token: 0x040044D1 RID: 17617
	private GameHitter gameHitter;

	// Token: 0x040044D2 RID: 17618
	private GRToolFlash.State state;

	// Token: 0x040044D3 RID: 17619
	private float stateTimeRemaining;

	// Token: 0x040044D4 RID: 17620
	private RaycastHit[] tempHitResults = new RaycastHit[128];

	// Token: 0x02000810 RID: 2064
	[Flags]
	public enum UpgradeTypes
	{
		// Token: 0x040044D6 RID: 17622
		None = 1,
		// Token: 0x040044D7 RID: 17623
		UpagredA = 2,
		// Token: 0x040044D8 RID: 17624
		UpagredB = 4,
		// Token: 0x040044D9 RID: 17625
		UpagredC = 8
	}

	// Token: 0x02000811 RID: 2065
	private enum State
	{
		// Token: 0x040044DB RID: 17627
		Idle,
		// Token: 0x040044DC RID: 17628
		Charging,
		// Token: 0x040044DD RID: 17629
		Flash,
		// Token: 0x040044DE RID: 17630
		Cooldown,
		// Token: 0x040044DF RID: 17631
		Count
	}
}
