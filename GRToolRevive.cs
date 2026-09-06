using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200081E RID: 2078
[RequireComponent(typeof(GameEntity))]
public class GRToolRevive : MonoBehaviour
{
	// Token: 0x06003552 RID: 13650 RVA: 0x001252AB File Offset: 0x001234AB
	private void Awake()
	{
		this.state = GRToolRevive.State.Idle;
	}

	// Token: 0x06003553 RID: 13651 RVA: 0x001252B4 File Offset: 0x001234B4
	private void OnEnable()
	{
		this.StopRevive();
		this.state = GRToolRevive.State.Idle;
	}

	// Token: 0x06003554 RID: 13652 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnDestroy()
	{
	}

	// Token: 0x06003555 RID: 13653 RVA: 0x001252C4 File Offset: 0x001234C4
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.gameEntity.IsHeldByLocalPlayer())
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x06003556 RID: 13654 RVA: 0x001252F4 File Offset: 0x001234F4
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolRevive.State.Idle:
			if (this.tool.HasEnoughEnergy() && this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolRevive.State.Reviving);
				return;
			}
			break;
		case GRToolRevive.State.Reviving:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolRevive.State.Cooldown);
				return;
			}
			break;
		case GRToolRevive.State.Cooldown:
			if (!this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolRevive.State.Idle);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06003557 RID: 13655 RVA: 0x0012536C File Offset: 0x0012356C
	private void OnUpdateRemote(float dt)
	{
		GRToolRevive.State state = (GRToolRevive.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06003558 RID: 13656 RVA: 0x00125396 File Offset: 0x00123596
	private void SetStateAuthority(GRToolRevive.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06003559 RID: 13657 RVA: 0x001253B8 File Offset: 0x001235B8
	private void SetState(GRToolRevive.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		if (this.state == GRToolRevive.State.Reviving)
		{
			this.StopRevive();
		}
		this.state = newState;
		GRToolRevive.State state = this.state;
		if (state != GRToolRevive.State.Idle)
		{
			if (state == GRToolRevive.State.Reviving)
			{
				this.StartRevive();
				this.stateTimeRemaining = this.reviveDuration;
				return;
			}
		}
		else
		{
			this.stateTimeRemaining = -1f;
		}
	}

	// Token: 0x0600355A RID: 13658 RVA: 0x00125414 File Offset: 0x00123614
	private void StartRevive()
	{
		this.reviveFx.SetActive(true);
		this.audioSource.volume = this.reviveSoundVolume;
		this.audioSource.clip = this.reviveSound;
		this.audioSource.Play();
		this.tool.UseEnergy();
		this.onHaptic.PlayIfHeldLocal(this.gameEntity);
		if (this.gameEntity.IsAuthority())
		{
			int num = Physics.SphereCastNonAlloc(this.shootFrom.position, 0.5f, this.shootFrom.rotation * Vector3.forward, this.tempHitResults, this.reviveDistance, this.playerLayerMask);
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this.tempHitResults[i];
				Rigidbody attachedRigidbody = raycastHit.collider.attachedRigidbody;
				if (!(attachedRigidbody == null))
				{
					GRPlayer component = attachedRigidbody.GetComponent<GRPlayer>();
					if (component != null && component.State != GRPlayer.GRPlayerState.Alive)
					{
						GhostReactorManager.Get(this.gameEntity).RequestPlayerStateChange(component, GRPlayer.GRPlayerState.Alive);
						return;
					}
				}
			}
		}
	}

	// Token: 0x0600355B RID: 13659 RVA: 0x00125526 File Offset: 0x00123726
	private void StopRevive()
	{
		this.reviveFx.SetActive(false);
		this.audioSource.Stop();
	}

	// Token: 0x0600355C RID: 13660 RVA: 0x00125540 File Offset: 0x00123740
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

	// Token: 0x0400458B RID: 17803
	public GameEntity gameEntity;

	// Token: 0x0400458C RID: 17804
	public GRTool tool;

	// Token: 0x0400458D RID: 17805
	[SerializeField]
	private Transform shootFrom;

	// Token: 0x0400458E RID: 17806
	[SerializeField]
	private LayerMask playerLayerMask;

	// Token: 0x0400458F RID: 17807
	[SerializeField]
	private float reviveDistance = 1.5f;

	// Token: 0x04004590 RID: 17808
	[SerializeField]
	private GameObject reviveFx;

	// Token: 0x04004591 RID: 17809
	[SerializeField]
	private float reviveSoundVolume;

	// Token: 0x04004592 RID: 17810
	[SerializeField]
	private AudioClip reviveSound;

	// Token: 0x04004593 RID: 17811
	[SerializeField]
	private float reviveDuration = 0.75f;

	// Token: 0x04004594 RID: 17812
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04004595 RID: 17813
	[Header("Haptic")]
	public AbilityHaptic onHaptic;

	// Token: 0x04004596 RID: 17814
	private GRToolRevive.State state;

	// Token: 0x04004597 RID: 17815
	private float stateTimeRemaining;

	// Token: 0x04004598 RID: 17816
	private RaycastHit[] tempHitResults = new RaycastHit[128];

	// Token: 0x0200081F RID: 2079
	private enum State
	{
		// Token: 0x0400459A RID: 17818
		Idle,
		// Token: 0x0400459B RID: 17819
		Reviving,
		// Token: 0x0400459C RID: 17820
		Cooldown
	}
}
