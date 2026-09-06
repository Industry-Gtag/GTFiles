using System;
using GorillaExtensions;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020005AA RID: 1450
public class RubberDuck : TransferrableObject
{
	// Token: 0x170003D7 RID: 983
	// (get) Token: 0x060024B1 RID: 9393 RVA: 0x000C53F7 File Offset: 0x000C35F7
	// (set) Token: 0x060024B2 RID: 9394 RVA: 0x000C5409 File Offset: 0x000C3609
	public bool fxActive
	{
		get
		{
			return this.hasParticleFX && this._fxActive;
		}
		set
		{
			if (!this.hasParticleFX)
			{
				return;
			}
			this.pFXEmissionModule.enabled = value;
			this._fxActive = value;
		}
	}

	// Token: 0x170003D8 RID: 984
	// (get) Token: 0x060024B3 RID: 9395 RVA: 0x000C5427 File Offset: 0x000C3627
	public int SqueezeSound
	{
		get
		{
			if (this.squeezeSoundBank.Length > 1)
			{
				return this.squeezeSoundBank[Random.Range(0, this.squeezeSoundBank.Length)];
			}
			if (this.squeezeSoundBank.Length == 1)
			{
				return this.squeezeSoundBank[0];
			}
			return this.squeezeSound;
		}
	}

	// Token: 0x170003D9 RID: 985
	// (get) Token: 0x060024B4 RID: 9396 RVA: 0x000C5464 File Offset: 0x000C3664
	public int SqueezeReleaseSound
	{
		get
		{
			if (this.squeezeReleaseSoundBank.Length > 1)
			{
				return this.squeezeReleaseSoundBank[Random.Range(0, this.squeezeReleaseSoundBank.Length)];
			}
			if (this.squeezeReleaseSoundBank.Length == 1)
			{
				return this.squeezeReleaseSoundBank[0];
			}
			return this.squeezeReleaseSound;
		}
	}

	// Token: 0x060024B5 RID: 9397 RVA: 0x000C54A4 File Offset: 0x000C36A4
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		if (this.skinRenderer == null)
		{
			this.skinRenderer = base.GetComponentInChildren<SkinnedMeshRenderer>(true);
		}
		this.hasSkinRenderer = this.skinRenderer != null;
		this.myThreshold = 0.7f;
		this.hysterisis = 0.3f;
		this.hasParticleFX = this.particleFX != null;
		if (this.hasParticleFX)
		{
			this.pFXEmissionModule = this.particleFX.emission;
			this.pFXEmissionModule.rateOverTime = this.particleFXEmissionIdle;
		}
		this.fxActive = false;
	}

	// Token: 0x060024B6 RID: 9398 RVA: 0x000C5544 File Offset: 0x000C3744
	internal override void OnEnable()
	{
		base.OnEnable();
		if (this._events == null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			NetPlayer netPlayer = ((base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null));
			if (netPlayer != null)
			{
				this._events.Init(netPlayer);
			}
			else
			{
				Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
			}
		}
		if (this._events != null)
		{
			this._events.Activate += this.OnSqueezeActivate;
			this._events.Deactivate += this.OnSqueezeDeactivate;
		}
	}

	// Token: 0x060024B7 RID: 9399 RVA: 0x000C5634 File Offset: 0x000C3834
	internal override void OnDisable()
	{
		base.OnDisable();
		if (this._events != null)
		{
			this._events.Activate -= this.OnSqueezeActivate;
			this._events.Deactivate -= this.OnSqueezeDeactivate;
			this._events.Dispose();
			this._events = null;
		}
	}

	// Token: 0x060024B8 RID: 9400 RVA: 0x000C56AB File Offset: 0x000C38AB
	private void OnSqueezeActivate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != target)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		this.SqueezeActivateLocal();
	}

	// Token: 0x060024B9 RID: 9401 RVA: 0x000C56D2 File Offset: 0x000C38D2
	private void SqueezeActivateLocal()
	{
		this.PlayParticleFX(this.particleFXEmissionSqueeze);
		if (this._sfxActivate && !this._sfxActivate.isPlaying)
		{
			this._sfxActivate.PlayNext(0f, 1f);
		}
	}

	// Token: 0x060024BA RID: 9402 RVA: 0x000C570F File Offset: 0x000C390F
	private void OnSqueezeDeactivate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != target)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "OnSqueezeDeactivate");
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		this.SqueezeDeactivateLocal();
	}

	// Token: 0x060024BB RID: 9403 RVA: 0x000C5742 File Offset: 0x000C3942
	private void SqueezeDeactivateLocal()
	{
		this.PlayParticleFX(this.particleFXEmissionIdle);
	}

	// Token: 0x060024BC RID: 9404 RVA: 0x000C5750 File Offset: 0x000C3950
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		float num = 0f;
		if (base.InHand())
		{
			this.tempHandPos = ((base.myOnlineRig != null) ? base.myOnlineRig.ReturnHandPosition() : base.myRig.ReturnHandPosition());
			if (this.currentState == TransferrableObject.PositionState.InLeftHand)
			{
				num = (float)Mathf.FloorToInt((float)(this.tempHandPos % 10000) / 1000f);
			}
			else
			{
				num = (float)Mathf.FloorToInt((float)(this.tempHandPos % 10) / 1f);
			}
		}
		if (this.hasSkinRenderer)
		{
			this.skinRenderer.SetBlendShapeWeight(0, Mathf.Lerp(this.skinRenderer.GetBlendShapeWeight(0), num * 11.1f, this.blendShapeMaxWeight));
		}
		if (this.fxActive)
		{
			this.squeezeTimeElapsed += Time.deltaTime;
			this.pFXEmissionModule.rateOverTime = Mathf.Lerp(this.particleFXEmissionIdle, this.particleFXEmissionSqueeze, this.particleFXEmissionCooldownCurve.Evaluate(this.squeezeTimeElapsed));
			if (this.squeezeTimeElapsed > this.particleFXEmissionSqueeze)
			{
				this.fxActive = false;
			}
		}
	}

	// Token: 0x060024BD RID: 9405 RVA: 0x000C586C File Offset: 0x000C3A6C
	public override void OnActivate()
	{
		base.OnActivate();
		if (this.IsMyItem())
		{
			bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
			RigContainer localRig = VRRigCache.Instance.localRig;
			int num = this.SqueezeSound;
			localRig.Rig.PlayHandTapLocal(num, flag, 0.33f);
			if (localRig.netView)
			{
				localRig.netView.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { num, flag, 0.33f });
			}
			GorillaTagger.Instance.StartVibration(flag, this.squeezeStrength, Time.deltaTime);
		}
		if (this._raiseActivate)
		{
			if (RoomSystem.JoinedRoom)
			{
				RubberDuckEvents events = this._events;
				if (events == null)
				{
					return;
				}
				PhotonEvent activate = events.Activate;
				if (activate == null)
				{
					return;
				}
				activate.RaiseAll(Array.Empty<object>());
				return;
			}
			else
			{
				this.SqueezeActivateLocal();
			}
		}
	}

	// Token: 0x060024BE RID: 9406 RVA: 0x000C5948 File Offset: 0x000C3B48
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		if (this.IsMyItem())
		{
			bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
			int num = this.SqueezeReleaseSound;
			Debug.Log("Squeezy Deactivate: " + num.ToString());
			VRRigCache.Instance.localRig.Rig.PlayHandTapLocal(num, flag, 0.33f);
			RigContainer rigContainer;
			if (GorillaGameManager.instance && VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.LocalPlayer, out rigContainer))
			{
				rigContainer.Rig.netView.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { num, flag, 0.33f });
			}
			GorillaTagger.Instance.StartVibration(flag, this.releaseStrength, Time.deltaTime);
		}
		if (this._raiseDeactivate)
		{
			if (RoomSystem.JoinedRoom)
			{
				RubberDuckEvents events = this._events;
				if (events == null)
				{
					return;
				}
				PhotonEvent deactivate = events.Deactivate;
				if (deactivate == null)
				{
					return;
				}
				deactivate.RaiseAll(Array.Empty<object>());
				return;
			}
			else
			{
				this.SqueezeDeactivateLocal();
			}
		}
	}

	// Token: 0x060024BF RID: 9407 RVA: 0x000C5A54 File Offset: 0x000C3C54
	public void PlayParticleFX(float rate)
	{
		if (!this.hasParticleFX)
		{
			return;
		}
		if (this.currentState != TransferrableObject.PositionState.InLeftHand && this.currentState != TransferrableObject.PositionState.InRightHand)
		{
			return;
		}
		if (!this.fxActive)
		{
			this.fxActive = true;
		}
		this.squeezeTimeElapsed = 0f;
		this.pFXEmissionModule.rateOverTime = rate;
	}

	// Token: 0x060024C0 RID: 9408 RVA: 0x000C5AA8 File Offset: 0x000C3CA8
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x060024C1 RID: 9409 RVA: 0x000C5AB3 File Offset: 0x000C3CB3
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04003036 RID: 12342
	[DebugOption]
	public bool disableActivation;

	// Token: 0x04003037 RID: 12343
	[DebugOption]
	public bool disableDeactivation;

	// Token: 0x04003038 RID: 12344
	private SkinnedMeshRenderer skinRenderer;

	// Token: 0x04003039 RID: 12345
	[FormerlySerializedAs("duckieLerp")]
	public float blendShapeMaxWeight = 1f;

	// Token: 0x0400303A RID: 12346
	private int tempHandPos;

	// Token: 0x0400303B RID: 12347
	[GorillaSoundLookup]
	[SerializeField]
	private int squeezeSound = 75;

	// Token: 0x0400303C RID: 12348
	[GorillaSoundLookup]
	[SerializeField]
	private int squeezeReleaseSound = 76;

	// Token: 0x0400303D RID: 12349
	[GorillaSoundLookup]
	public int[] squeezeSoundBank;

	// Token: 0x0400303E RID: 12350
	[GorillaSoundLookup]
	public int[] squeezeReleaseSoundBank;

	// Token: 0x0400303F RID: 12351
	public float squeezeStrength = 0.05f;

	// Token: 0x04003040 RID: 12352
	public float releaseStrength = 0.03f;

	// Token: 0x04003041 RID: 12353
	public ParticleSystem particleFX;

	// Token: 0x04003042 RID: 12354
	[Tooltip("The emission rate of the particle effect when not squeezed.")]
	public float particleFXEmissionIdle = 0.8f;

	// Token: 0x04003043 RID: 12355
	[Tooltip("The emission rate of the particle effect when squeezed.")]
	public float particleFXEmissionSqueeze = 10f;

	// Token: 0x04003044 RID: 12356
	[Tooltip("The animation of the particle effect returning to the idle emission rate. X axis is time, Y axis is the emission lerp value where 0 is idle, 1 is squeezed.")]
	public AnimationCurve particleFXEmissionCooldownCurve;

	// Token: 0x04003045 RID: 12357
	private bool hasSkinRenderer;

	// Token: 0x04003046 RID: 12358
	private ParticleSystem.EmissionModule pFXEmissionModule;

	// Token: 0x04003047 RID: 12359
	private bool hasParticleFX;

	// Token: 0x04003048 RID: 12360
	private float squeezeTimeElapsed;

	// Token: 0x04003049 RID: 12361
	[SerializeField]
	private RubberDuckEvents _events;

	// Token: 0x0400304A RID: 12362
	[SerializeField]
	private bool _raiseActivate = true;

	// Token: 0x0400304B RID: 12363
	[SerializeField]
	private bool _raiseDeactivate = true;

	// Token: 0x0400304C RID: 12364
	[SerializeField]
	private SoundEffects _sfxActivate;

	// Token: 0x0400304D RID: 12365
	[SerializeField]
	private bool _fxActive;
}
