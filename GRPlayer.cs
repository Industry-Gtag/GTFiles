using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x020007CD RID: 1997
public class GRPlayer : MonoBehaviourTick
{
	// Token: 0x1700049D RID: 1181
	// (get) Token: 0x060032D8 RID: 13016 RVA: 0x001168AA File Offset: 0x00114AAA
	public GRPlayer.GRPlayerState State
	{
		get
		{
			return this.state;
		}
	}

	// Token: 0x1700049E RID: 1182
	// (get) Token: 0x060032D9 RID: 13017 RVA: 0x001168B2 File Offset: 0x00114AB2
	public int Juice
	{
		get
		{
			return this.playerJuice;
		}
	}

	// Token: 0x1700049F RID: 1183
	// (get) Token: 0x060032DA RID: 13018 RVA: 0x001168BA File Offset: 0x00114ABA
	// (set) Token: 0x060032DB RID: 13019 RVA: 0x001168C2 File Offset: 0x00114AC2
	public int ShiftCreditCapIncreases { get; set; }

	// Token: 0x170004A0 RID: 1184
	// (get) Token: 0x060032DC RID: 13020 RVA: 0x001168CB File Offset: 0x00114ACB
	// (set) Token: 0x060032DD RID: 13021 RVA: 0x001168D3 File Offset: 0x00114AD3
	public int ShiftCreditCapIncreasesMax { get; set; }

	// Token: 0x170004A1 RID: 1185
	// (get) Token: 0x060032DE RID: 13022 RVA: 0x001168DC File Offset: 0x00114ADC
	public int ShiftCredits
	{
		get
		{
			return this.shiftCreditCache;
		}
	}

	// Token: 0x060032DF RID: 13023 RVA: 0x001168E4 File Offset: 0x00114AE4
	public bool HasXRayVision()
	{
		return this.xRayVisionRefCount > 0;
	}

	// Token: 0x170004A2 RID: 1186
	// (get) Token: 0x060032E0 RID: 13024 RVA: 0x001168EF File Offset: 0x00114AEF
	public int MaxHp
	{
		get
		{
			return this.maxHp;
		}
	}

	// Token: 0x170004A3 RID: 1187
	// (get) Token: 0x060032E1 RID: 13025 RVA: 0x001168F7 File Offset: 0x00114AF7
	public int MaxShieldHp
	{
		get
		{
			return this.maxShieldHp;
		}
	}

	// Token: 0x170004A4 RID: 1188
	// (get) Token: 0x060032E2 RID: 13026 RVA: 0x001168FF File Offset: 0x00114AFF
	public int Hp
	{
		get
		{
			return this.hp;
		}
	}

	// Token: 0x170004A5 RID: 1189
	// (get) Token: 0x060032E3 RID: 13027 RVA: 0x00116907 File Offset: 0x00114B07
	public int ShieldHp
	{
		get
		{
			return this.shieldHp;
		}
	}

	// Token: 0x170004A6 RID: 1190
	// (get) Token: 0x060032E4 RID: 13028 RVA: 0x0011690F File Offset: 0x00114B0F
	public int ShieldFlags
	{
		get
		{
			return this.shieldFlags;
		}
	}

	// Token: 0x170004A7 RID: 1191
	// (get) Token: 0x060032E5 RID: 13029 RVA: 0x00116917 File Offset: 0x00114B17
	public bool InStealthMode
	{
		get
		{
			return this.inStealthMode;
		}
	}

	// Token: 0x170004A8 RID: 1192
	// (get) Token: 0x060032E6 RID: 13030 RVA: 0x0011691F File Offset: 0x00114B1F
	public VRRig MyRig
	{
		get
		{
			return this.vrRig;
		}
	}

	// Token: 0x170004A9 RID: 1193
	// (get) Token: 0x060032E7 RID: 13031 RVA: 0x00116927 File Offset: 0x00114B27
	// (set) Token: 0x060032E8 RID: 13032 RVA: 0x0011692F File Offset: 0x00114B2F
	public float ShiftPlayTime
	{
		get
		{
			return this.shiftPlayTime;
		}
		set
		{
			this.shiftPlayTime = value;
		}
	}

	// Token: 0x170004AA RID: 1194
	// (get) Token: 0x060032E9 RID: 13033 RVA: 0x00116938 File Offset: 0x00114B38
	// (set) Token: 0x060032EA RID: 13034 RVA: 0x00116940 File Offset: 0x00114B40
	public int LastShiftCut
	{
		get
		{
			return this.lastShiftCut;
		}
		set
		{
			this.lastShiftCut = value;
		}
	}

	// Token: 0x170004AB RID: 1195
	// (get) Token: 0x060032EB RID: 13035 RVA: 0x00116949 File Offset: 0x00114B49
	// (set) Token: 0x060032EC RID: 13036 RVA: 0x00116951 File Offset: 0x00114B51
	public GRPlayer.ProgressionData CurrentProgression
	{
		get
		{
			return this.currentProgression;
		}
		set
		{
			this.currentProgression = value;
		}
	}

	// Token: 0x060032ED RID: 13037 RVA: 0x0011695C File Offset: 0x00114B5C
	private void Awake()
	{
		this.vrRig = base.GetComponent<VRRig>();
		this.lowHealthVisualPropertyBlock = new MaterialPropertyBlock();
		this.damageEffects = GTPlayer.Instance.mainCamera.GetComponent<GRPlayerDamageEffects>();
		this.lowHealthTintPropertyId = Shader.PropertyToID("_TintColor");
		this.isEmployee = false;
		this.SetHp(this.maxHp);
		this.SetShieldHp(0);
		this.state = GRPlayer.GRPlayerState.Alive;
		this.RefreshDamageVignetteVisual();
		this.shieldHeadVisual.gameObject.SetActive(false);
		this.shieldBodyVisual.gameObject.SetActive(false);
		this.shieldGameLight = this.shieldBodyVisual.gameObject.GetComponentInChildren<GameLight>(true);
		this.requestCollectItemLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestChargeToolLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestDepositCurrencyLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestShiftStartLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestToolPurchaseStationLimiter = new CallLimiter(25, 1f, 0.5f);
		this.applyEnemyHitLimiter = new CallLimiter(25, 1f, 0.5f);
		this.reportLocalHitLimiter = new CallLimiter(25, 1f, 0.5f);
		this.reportBreakableBrokenLimiter = new CallLimiter(25, 1f, 0.5f);
		this.playerStateChangeLimiter = new CallLimiter(25, 1f, 0.5f);
		this.promotionBotLimiter = new CallLimiter(25, 1f, 0.5f);
		this.progressionBroadcastLimiter = new CallLimiter(25, 1f, 0.5f);
		this.scoreboardPageLimiter = new CallLimiter(25, 1f, 0.5f);
		this.fireShieldLimiter = new CallLimiter(25, 1f, 0.5f);
		this.shuttleData = new GRPlayer.ShuttleData();
		this.lastLeftWithBadgeAttachedTime = -10000.0;
	}

	// Token: 0x060032EE RID: 13038 RVA: 0x00116B4C File Offset: 0x00114D4C
	private void Start()
	{
		if (this.gamePlayer != null && this.gamePlayer.IsLocal())
		{
			this.LoadMyProgression();
			ProgressionManager.Instance.OnGetShiftCredit += this.OnShiftCreditChanged;
			ProgressionManager.Instance.OnGetShiftCreditCapData += this.OnShiftCreditCapChanged;
			this.soak = new GhostReactorSoak();
			this.soak.Setup(this);
		}
		else
		{
			this.currentProgression = new GRPlayer.ProgressionData
			{
				points = 0,
				redeemedPoints = 0
			};
		}
		if (ProgressionManager.Instance != null)
		{
			ProgressionManager.Instance.OnGetShiftCredit += this.OnShiftCreditChanged;
			ProgressionManager.Instance.OnGetShiftCreditCapData += this.OnShiftCreditCapChanged;
		}
	}

	// Token: 0x060032EF RID: 13039 RVA: 0x00116C17 File Offset: 0x00114E17
	private new void OnDisable()
	{
		this.Reset();
	}

	// Token: 0x060032F0 RID: 13040 RVA: 0x00116C20 File Offset: 0x00114E20
	public void Reset()
	{
		this.SetHp(this.maxHp);
		this.SetShieldHp(0);
		this.state = GRPlayer.GRPlayerState.Alive;
		this.RefreshDamageVignetteVisual();
		this.RefreshPlayerVisuals();
		for (int i = 0; i < 8; i++)
		{
			this.synchronizedSessionStats[i] = 0f;
		}
	}

	// Token: 0x060032F1 RID: 13041 RVA: 0x00116C6C File Offset: 0x00114E6C
	private void SetHp(int newHp)
	{
		this.hp = Mathf.Max(newHp, 0);
	}

	// Token: 0x060032F2 RID: 13042 RVA: 0x00116C7B File Offset: 0x00114E7B
	private void SetShieldHp(int newShieldHp)
	{
		this.shieldHp = Mathf.Max(newShieldHp, 0);
	}

	// Token: 0x060032F3 RID: 13043 RVA: 0x00116C8C File Offset: 0x00114E8C
	public void OnShiftCreditCapChanged(string targetMothershipId, int newCap, int newCapMax)
	{
		if (this.mothershipId != null && targetMothershipId == this.mothershipId)
		{
			if (this.gamePlayer.IsLocal() && (newCap != this.ShiftCreditCapIncreases || newCapMax != this.ShiftCreditCapIncreasesMax) && GhostReactor.instance != null)
			{
				GhostReactor.instance.grManager.RefreshShiftCredit();
			}
			this.ShiftCreditCapIncreases = newCap;
			this.ShiftCreditCapIncreasesMax = newCapMax;
		}
	}

	// Token: 0x060032F4 RID: 13044 RVA: 0x00116CF8 File Offset: 0x00114EF8
	public void OnShiftCreditChanged(string targetMothershipId, int newShiftCredits)
	{
		if (this.mothershipId != null && targetMothershipId == this.mothershipId)
		{
			int num = this.shiftCreditCache;
			this.shiftCreditCache = newShiftCredits;
			if (GhostReactor.instance != null && this.gamePlayer.IsLocal() && num != newShiftCredits && GhostReactor.instance != null)
			{
				if (GhostReactor.instance.promotionBot != null)
				{
					GhostReactor.instance.promotionBot.Refresh();
				}
				if (GhostReactor.instance.grManager != null)
				{
					GhostReactor.instance.grManager.RefreshShiftCredit();
				}
			}
		}
		if (GhostReactor.instance != null)
		{
			GhostReactor.instance.RefreshScoreboards();
		}
	}

	// Token: 0x060032F5 RID: 13045 RVA: 0x00116DB0 File Offset: 0x00114FB0
	public void OnShiftCreditCapData(string targetMothershipId, int shiftCreditCapNumberOfIncreases, int shiftCreditMaxNumberOfIncreases)
	{
		if (this.mothershipId != null)
		{
			targetMothershipId == this.mothershipId;
		}
	}

	// Token: 0x060032F6 RID: 13046 RVA: 0x00116DC7 File Offset: 0x00114FC7
	public void SubtractShiftCredit(int shiftCreditDelta)
	{
		if (this.gamePlayer.IsLocal())
		{
			ProgressionManager.Instance.SubtractShiftCredit(shiftCreditDelta);
		}
	}

	// Token: 0x060032F7 RID: 13047 RVA: 0x00116DE4 File Offset: 0x00114FE4
	public void OnPlayerHit(Vector3 hitPosition, Vector3 hitImpulse, GhostReactorManager manager, GameEntityId hitByEntityId)
	{
		GameEntity gameEntity = manager.gameEntityManager.GetGameEntity(hitByEntityId);
		int num = 1;
		if (this.gamePlayer.IsLocal())
		{
			GTPlayer instance = GTPlayer.Instance;
			float magnitude = hitImpulse.magnitude;
			if (magnitude > 0f)
			{
				instance.ApplyKnockback(hitImpulse / magnitude, magnitude, true);
			}
		}
		if (this.State == GRPlayer.GRPlayerState.Alive)
		{
			if (this.shieldHp > 0)
			{
				if (gameEntity != null)
				{
					GRAttributes component = gameEntity.GetComponent<GRAttributes>();
					if (component != null)
					{
						num = component.CalculateFinalValueForAttribute(GRAttributeType.PlayerShieldDamage);
					}
				}
				this.SetShieldHp(this.shieldHp - num);
				if (this.shieldHp > 0)
				{
					if (this.shieldDamagedSound != null)
					{
						this.audioSource.PlayOneShot(this.shieldDamagedSound, this.shieldDamagedVolume);
					}
					this.shieldDamagedEffect.Play();
				}
				else
				{
					if (this.shieldDestroyedSound != null)
					{
						this.audioSource.PlayOneShot(this.shieldDestroyedSound, this.shieldDestroyedVolume);
					}
					this.shieldDestroyedEffect.Play();
				}
				this.RefreshPlayerVisuals();
				return;
			}
			if (gameEntity != null)
			{
				GRAttributes component2 = gameEntity.GetComponent<GRAttributes>();
				if (component2 != null)
				{
					num = component2.CalculateFinalValueForAttribute(GRAttributeType.PlayerDamage);
				}
			}
			Debug.Log(string.Format("GRPlayer OnPlayerHit, hit by: {0} damage: {1}, state: {2}, hp: {3}, shield hp: {4}", new object[] { hitByEntityId.index, num, this.state, this.hp, this.shieldHp }));
			this.PlayHitFx(hitPosition);
			this.SetHp(this.hp - num);
			this.RefreshDamageVignetteVisual();
			if (this.hp <= 0)
			{
				this.ChangePlayerState(GRPlayer.GRPlayerState.Ghost, manager);
			}
		}
	}

	// Token: 0x060032F8 RID: 13048 RVA: 0x00116F97 File Offset: 0x00115197
	public void OnPlayerRevive(GhostReactorManager manager)
	{
		this.SetHp(this.maxHp);
		this.RefreshDamageVignetteVisual();
		this.ChangePlayerState(GRPlayer.GRPlayerState.Alive, manager);
	}

	// Token: 0x060032F9 RID: 13049 RVA: 0x00116FB4 File Offset: 0x001151B4
	public void ChangePlayerState(GRPlayer.GRPlayerState newState, GhostReactorManager manager)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			newState = GRPlayer.GRPlayerState.Alive;
		}
		if (this.state == newState)
		{
			return;
		}
		this.state = newState;
		GRPlayer.GRPlayerState grplayerState = this.state;
		if (grplayerState != GRPlayer.GRPlayerState.Alive)
		{
			if (grplayerState == GRPlayer.GRPlayerState.Ghost)
			{
				this.SetHp(0);
				this.SetShieldHp(0);
				this.RefreshDamageVignetteVisual();
				if (this.playerTurnedGhostEffect != null)
				{
					this.playerTurnedGhostEffect.Play();
				}
				this.playerTurnedGhostSoundBank.Play();
				manager.ReportPlayerDeath(this);
				this.IncrementDeaths(1);
			}
		}
		else
		{
			this.SetHp(this.maxHp);
			this.RefreshDamageVignetteVisual();
			this.IncrementRevives(1);
			if (this.playerRevivedEffect != null)
			{
				this.playerRevivedEffect.Play();
			}
			if (this.audioSource != null && this.playerRevivedSound != null)
			{
				this.audioSource.PlayOneShot(this.playerRevivedSound, this.playerRevivedVolume);
			}
		}
		this.RefreshPlayerVisuals();
		if (this.vrRig.isLocal)
		{
			this.vrRigs.Clear();
			VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
			for (int i = 0; i < this.vrRigs.Count; i++)
			{
				this.vrRigs[i].GetComponent<GRPlayer>().RefreshPlayerVisuals();
			}
		}
	}

	// Token: 0x060032FA RID: 13050 RVA: 0x001170FC File Offset: 0x001152FC
	public void RefreshPlayerVisuals()
	{
		this.RefreshDamageVignetteVisual();
		GRPlayer.GRPlayerState grplayerState = this.state;
		if (grplayerState == GRPlayer.GRPlayerState.Alive)
		{
			this.gamePlayer.DisableGrabbing(false);
			if (this.badge != null)
			{
				this.badge.UnHide();
			}
			this.vrRig.ChangeMaterialLocal(0);
			this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Default);
			this.vrRig.SetInvisibleToLocalPlayer(false);
			if (this.vrRig.isLocal)
			{
				CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
				GameLightingManager.instance.SetDesaturateAndTintEnabled(false, Color.black);
				Color color = Color.black;
				GhostReactor instance = GhostReactor.instance;
				if (instance != null && instance.zone != GTZone.customMaps)
				{
					color = instance.GetCurrLevelGenConfig().ambientLight;
				}
				GameLightingManager.instance.SetAmbientLightDynamic(color);
			}
			if (this.shieldHp > 0)
			{
				this.shieldHeadVisual.gameObject.SetActive(true);
				this.shieldBodyVisual.gameObject.SetActive(true);
				Color color2 = this.shieldColorNormal;
				if ((this.shieldFlags & 1) != 0)
				{
					color2 = this.shieldColorLight;
				}
				else if ((this.shieldFlags & 2) != 0)
				{
					color2 = this.shieldColorStealth;
				}
				else if ((this.shieldFlags & 4) != 0)
				{
					color2 = this.shieldColorHeal;
				}
				Renderer component = this.shieldBodyVisual.GetComponent<Renderer>();
				if (component != null)
				{
					component.material.SetColor("_BaseColor", color2);
				}
				Renderer component2 = this.shieldHeadVisual.GetComponent<Renderer>();
				if (component2 != null)
				{
					component2.material.SetColor("_BaseColor", color2);
				}
			}
			else
			{
				this.shieldHeadVisual.gameObject.SetActive(false);
				this.shieldBodyVisual.gameObject.SetActive(false);
			}
			this.shieldGameLight.gameObject.SetActive((this.shieldFlags & 1) != 0);
			return;
		}
		if (grplayerState != GRPlayer.GRPlayerState.Ghost)
		{
			return;
		}
		if (this.vrRig.isLocal)
		{
			this.gamePlayer.RequestDropAllSnapped();
		}
		this.gamePlayer.DisableGrabbing(true);
		this.shieldHeadVisual.gameObject.SetActive(false);
		this.shieldBodyVisual.gameObject.SetActive(false);
		this.shieldGameLight.gameObject.SetActive(false);
		if (this.badge != null)
		{
			this.badge.Hide();
		}
		if (this.vrRig.isLocal)
		{
			GamePlayerLocal.instance.OnUpdateInteract();
			this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Skeleton);
			this.vrRig.ChangeMaterialLocal(13);
			this.vrRig.SetInvisibleToLocalPlayer(false);
			CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(true);
			GameLightingManager.instance.SetDesaturateAndTintEnabled(true, this.deathTintColor);
			GameLightingManager.instance.SetAmbientLightDynamic(this.deathAmbientLightColor);
			return;
		}
		if (VRRigCache.Instance.localRig.GetComponent<GRPlayer>().State == GRPlayer.GRPlayerState.Ghost)
		{
			this.vrRig.ChangeMaterialLocal(13);
			this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Skeleton);
			this.vrRig.SetInvisibleToLocalPlayer(false);
			return;
		}
		this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Invisible);
		this.vrRig.SetInvisibleToLocalPlayer(true);
	}

	// Token: 0x060032FB RID: 13051 RVA: 0x00117418 File Offset: 0x00115618
	public static GRPlayer Get(int actorNumber)
	{
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(actorNumber, out gamePlayer))
		{
			return null;
		}
		return gamePlayer.GetComponent<GRPlayer>();
	}

	// Token: 0x060032FC RID: 13052 RVA: 0x00117437 File Offset: 0x00115637
	public static GRPlayer Get(NetPlayer player)
	{
		if (player == null)
		{
			return null;
		}
		return GRPlayer.Get(player.ActorNumber);
	}

	// Token: 0x060032FD RID: 13053 RVA: 0x00117449 File Offset: 0x00115649
	public static GRPlayer Get(VRRig vrRig)
	{
		if (!(vrRig != null))
		{
			return null;
		}
		return vrRig.GetComponent<GRPlayer>();
	}

	// Token: 0x060032FE RID: 13054 RVA: 0x0011745C File Offset: 0x0011565C
	public static GRPlayer GetLocal()
	{
		return GRPlayer.Get(VRRig.LocalRig);
	}

	// Token: 0x060032FF RID: 13055 RVA: 0x00117468 File Offset: 0x00115668
	public void AttachBadge(GRBadge grBadge)
	{
		this.badge = grBadge;
		this.badge.transform.SetParent(this.badgeBodyAnchor);
		this.badge.GetComponent<Rigidbody>().isKinematic = true;
		this.badge.StartRetracting();
	}

	// Token: 0x06003300 RID: 13056 RVA: 0x001174A3 File Offset: 0x001156A3
	public bool CanActivateShield(int shieldHitPoints)
	{
		return this.state == GRPlayer.GRPlayerState.Alive && shieldHitPoints > 0;
	}

	// Token: 0x06003301 RID: 13057 RVA: 0x001174B4 File Offset: 0x001156B4
	public bool TryActivateShield(int shieldHitpoints, int shieldFlags)
	{
		if (this.state == GRPlayer.GRPlayerState.Alive)
		{
			if (this.shieldHp <= 0 && this.shieldActivatedSound != null)
			{
				this.audioSource.PlayOneShot(this.shieldActivatedSound, this.shieldActivatedVolume);
			}
			this.SetShieldHp(Mathf.Min(shieldHitpoints, this.maxShieldHp));
			this.shieldFlags = shieldFlags;
			this.inStealthMode = (shieldFlags & 2) != 0;
			if (this.inStealthMode)
			{
				if (this.damageEffects.stealthModeVisualRenderer != null)
				{
					this.damageEffects.stealthModeVisualRenderer.gameObject.SetActive(true);
				}
				this.shieldStealthModeEndTime = Time.timeAsDouble + (double)this.shieldStealthModeDuration;
			}
			if ((shieldFlags & 4) != 0)
			{
				this.SetHp(this.maxHp);
			}
			this.RefreshPlayerVisuals();
			return true;
		}
		return false;
	}

	// Token: 0x06003302 RID: 13058 RVA: 0x0011757D File Offset: 0x0011577D
	public void ClearStealthMode()
	{
		this.inStealthMode = false;
		if (this.damageEffects.stealthModeVisualRenderer != null)
		{
			this.damageEffects.stealthModeVisualRenderer.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003303 RID: 13059 RVA: 0x001175B0 File Offset: 0x001157B0
	public void SerializeNetworkState(BinaryWriter writer, NetPlayer player)
	{
		writer.Write((byte)this.state);
		writer.Write(this.hp);
		writer.Write(this.shieldHp);
		writer.Write(this.shiftJoinTime);
		writer.Write(this.isEmployee ? 1 : 0);
		writer.Write(this.CurrentProgression.points);
		writer.Write(this.CurrentProgression.redeemedPoints);
		writer.Write(this.dropPodLevel);
		writer.Write(this.dropPodChasisLevel);
		for (int i = 0; i < 8; i++)
		{
			writer.Write(this.synchronizedSessionStats[i]);
		}
	}

	// Token: 0x06003304 RID: 13060 RVA: 0x00117658 File Offset: 0x00115858
	public static void DeserializeNetworkStateAndBurn(BinaryReader reader, GRPlayer player, GhostReactorManager grManager)
	{
		GRPlayer.GRPlayerState grplayerState = (GRPlayer.GRPlayerState)reader.ReadByte();
		int num = reader.ReadInt32();
		int num2 = reader.ReadInt32();
		double num3 = reader.ReadDouble();
		bool flag = reader.ReadByte() > 0;
		int num4 = reader.ReadInt32();
		int num5 = reader.ReadInt32();
		int num6 = reader.ReadInt32();
		int num7 = reader.ReadInt32();
		for (int i = 0; i < 8; i++)
		{
			player.synchronizedSessionStats[i] = reader.ReadSingle();
		}
		if (player != null)
		{
			player.SetHp(num);
			player.SetShieldHp(num2);
			player.isEmployee = flag;
			player.ChangePlayerState(grplayerState, grManager);
			player.RefreshPlayerVisuals();
			if (!player.gamePlayer.IsLocal())
			{
				player.SetProgressionData(num4, num5, false);
				player.dropPodLevel = num6;
				player.dropPodChasisLevel = num7;
			}
			if (double.IsNaN(num3) || double.IsInfinity(num3))
			{
				player.shiftJoinTime = PhotonNetwork.Time;
			}
			else
			{
				player.shiftJoinTime = Math.Min(num3, PhotonNetwork.Time);
			}
		}
		if (grManager != null)
		{
			grManager.SendMothershipId();
		}
	}

	// Token: 0x06003305 RID: 13061 RVA: 0x00117760 File Offset: 0x00115960
	public void PlayHitFx(Vector3 attackLocation)
	{
		if (this.playerDamageAudioSource != null)
		{
			this.playerDamageAudioSource.PlayOneShot(this.playerDamageSound, this.playerDamageVolume);
		}
		if (this.bodyCenter != null)
		{
			Vector3 vector = attackLocation - this.bodyCenter.position;
			vector.y = 0f;
			Vector3 vector2 = vector.normalized * this.playerDamageOffsetDist;
			if (this.playerDamageEffect != null)
			{
				this.playerDamageEffect.transform.position = this.bodyCenter.position + vector2;
				this.playerDamageEffect.Play();
			}
			if (this.vrRig.isLocal)
			{
				Vector3 normalized = Vector3.ProjectOnPlane(GTPlayer.Instance.mainCamera.transform.forward, Vector3.up).normalized;
				vector = Vector3.ProjectOnPlane(vector, Vector3.up).normalized;
				float num = Vector3.SignedAngle(normalized, vector, Vector3.up);
				this.damageEffects.radialDamageEffect.transform.localRotation = Quaternion.Euler(0f, 0f, -num);
				this.damageEffects.radialDamageEffect.Play();
			}
		}
		if (this.gamePlayer == GamePlayerLocal.instance.gamePlayer)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength, 0.5f);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength, 0.5f);
		}
	}

	// Token: 0x06003306 RID: 13062 RVA: 0x001178E4 File Offset: 0x00115AE4
	public void SendGameStartedTelemetry(float timeIntoShift, bool wasPlayerInAtStart, int currentFloor)
	{
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		string titleNameFromLevel = GhostReactorProgression.GetTitleNameFromLevel(GhostReactorProgression.GetTitleLevel(this.CurrentProgression.redeemedPoints));
		GorillaTelemetry.GhostReactorShiftStart(this.gameId, this.ShiftCredits, timeIntoShift, wasPlayerInAtStart, this.vrRigs.Count + 1, currentFloor, titleNameFromLevel);
		this.wasPlayerInAtShiftStart = wasPlayerInAtStart;
		this.ResetGameTelemetryTracking();
	}

	// Token: 0x06003307 RID: 13063 RVA: 0x00117954 File Offset: 0x00115B54
	public void SendGameEndedTelemetry(bool isShiftActuallyEnding, ZoneClearReason zoneClearReason)
	{
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		GorillaTelemetry.GhostReactorGameEnd(this.gameId, this.ShiftCredits, this.totalCoresCollectedByPlayer, this.totalCoresCollectedByGroup, this.totalCoresSpentByPlayer, this.totalCoresSpentByGroup, this.totalGatesUnlocked, this.totalDeaths, this.totalItemsPurchased, this.lastShiftCut, isShiftActuallyEnding, this.timeIntoShiftAtJoin, (float)(PhotonNetwork.Time - (double)this.gameStartTime), this.wasPlayerInAtShiftStart, zoneClearReason, this.maxNumberOfPlayersInShift, this.vrRigs.Count + 1, this.totalItemTypesHeldThisShift, this.totalRevives, this.numShiftsPlayed);
		this.isFirstShift = true;
	}

	// Token: 0x06003308 RID: 13064 RVA: 0x00117A08 File Offset: 0x00115C08
	public void SendFloorStartedTelemetry(float timeIntoShift, bool wasPlayerInAtStart, int currentFloor, string floorPreset, string floorModifier)
	{
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		string titleNameFromLevel = GhostReactorProgression.GetTitleNameFromLevel(GhostReactorProgression.GetTitleLevel(this.CurrentProgression.redeemedPoints));
		GorillaTelemetry.GhostReactorFloorStart(this.gameId, this.ShiftCredits, timeIntoShift, wasPlayerInAtStart, this.vrRigs.Count + 1, titleNameFromLevel, currentFloor, floorPreset, floorModifier);
		this.wasPlayerInAtShiftStart = wasPlayerInAtStart;
	}

	// Token: 0x06003309 RID: 13065 RVA: 0x00117A74 File Offset: 0x00115C74
	public void SendFloorEndedTelemetry(bool isShiftActuallyEnding, float shiftStartTime, ZoneClearReason zoneClearReason, int currentFloor, string floorPreset, string floorModifier, bool objectivesCompleted, string section, int xpGained)
	{
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		GorillaTelemetry.GhostReactorFloorComplete(this.gameId, this.ShiftCredits, this.coresCollectedByPlayer, this.coresCollectedByGroup, this.coresSpentByPlayer, this.coresSpentByGroup, this.gatesUnlocked, this.deaths, this.itemsPurchased, this.lastShiftCut, isShiftActuallyEnding, this.timeIntoShiftAtJoin, (float)(PhotonNetwork.Time - (double)(this.timeIntoShiftAtJoin + shiftStartTime)), this.wasPlayerInAtShiftStart, zoneClearReason, this.maxNumberOfPlayersInShift, this.vrRigs.Count + 1, this.itemTypesHeldThisShift, this.revives, currentFloor, floorPreset, floorModifier, this.sentientCoresCollected, objectivesCompleted, section, xpGained);
	}

	// Token: 0x0600330A RID: 13066 RVA: 0x00117B2C File Offset: 0x00115D2C
	public void SendToolPurchasedTelemetry(string toolName, int toolLevel, int coresSpent, int shinyRocksSpent)
	{
		int num = -1;
		string text = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			num = instance.GetDepthLevel();
			text = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorToolPurchased(this.gameId, toolName, toolLevel, coresSpent, shinyRocksSpent, num, text);
	}

	// Token: 0x0600330B RID: 13067 RVA: 0x00117B80 File Offset: 0x00115D80
	public void SendRankUpTelemetry(string newRank)
	{
		int num = -1;
		string text = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			num = instance.GetDepthLevel();
			text = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorRankUp(this.gameId, newRank, num, text);
	}

	// Token: 0x0600330C RID: 13068 RVA: 0x00117BD0 File Offset: 0x00115DD0
	public void SendToolUpgradeTelemetry(string upgradeType, string toolName, int newLevel, int juiceSpent, int griftSpent, int coresSpent)
	{
		int num = -1;
		string text = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			num = instance.GetDepthLevel();
			text = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorToolUpgrade(this.gameId, upgradeType, toolName, newLevel, juiceSpent, griftSpent, coresSpent, num, text);
	}

	// Token: 0x0600330D RID: 13069 RVA: 0x00117C28 File Offset: 0x00115E28
	public void SendSeedDepositedTelemetry(string unlockTime, int seedsInQueue)
	{
		int num = -1;
		string text = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			num = instance.GetDepthLevel();
			text = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorChaosSeedStart(this.gameId, unlockTime, seedsInQueue, num, text);
	}

	// Token: 0x0600330E RID: 13070 RVA: 0x00117C78 File Offset: 0x00115E78
	public void SendJuiceCollectedTelemetry(int juiceCollected, int coresProcessedByOverdrive)
	{
		GorillaTelemetry.GhostReactorChaosJuiceCollected(this.gameId, juiceCollected, coresProcessedByOverdrive);
	}

	// Token: 0x0600330F RID: 13071 RVA: 0x00117C88 File Offset: 0x00115E88
	public void SendOverdrivePurchasedTelemetry(int shinyRocksUsed, int seedsInQueue)
	{
		int num = -1;
		string text = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			num = instance.GetDepthLevel();
			text = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorOverdrivePurchased(this.gameId, shinyRocksUsed, seedsInQueue, num, text);
	}

	// Token: 0x06003310 RID: 13072 RVA: 0x00117CD8 File Offset: 0x00115ED8
	public void SendPodUpgradeTelemetry(string toolName, int level, int shinyRocksSpent, int juiceSpent)
	{
		GorillaTelemetry.GhostReactorPodUpgradePurchased(this.gameId, toolName, level, shinyRocksSpent, juiceSpent);
	}

	// Token: 0x06003311 RID: 13073 RVA: 0x00117CEC File Offset: 0x00115EEC
	public void SendCreditsRefilledTelemetry(int shinyRocksSpent, int finalCredits)
	{
		int num = -1;
		string text = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			num = instance.GetDepthLevel();
			text = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorCreditsRefillPurchased(this.gameId, shinyRocksSpent, finalCredits, num, text);
	}

	// Token: 0x06003312 RID: 13074 RVA: 0x00117D3C File Offset: 0x00115F3C
	public void ResetTelemetryTracking(string newGameId, float timeSinceShiftStart)
	{
		this.gameId = newGameId;
		this.coresCollectedByPlayer = 0;
		this.coresCollectedByGroup = 0;
		this.gatesUnlocked = 0;
		this.deaths = 0;
		this.caughtByAnomaly = false;
		this.itemsPurchased = new List<string>();
		this.levelsUnlocked = new List<string>();
		this.sentientCoresCollected = 0;
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		this.maxNumberOfPlayersInShift = this.vrRigs.Count + 1;
		this.timeIntoShiftAtJoin = timeSinceShiftStart;
		this.itemsHeldThisShift.Clear();
		this.itemTypesHeldThisShift.Clear();
	}

	// Token: 0x06003313 RID: 13075 RVA: 0x00117DDC File Offset: 0x00115FDC
	public void ResetGameTelemetryTracking()
	{
		this.totalCoresCollectedByPlayer = 0;
		this.totalCoresCollectedByGroup = 0;
		this.totalGatesUnlocked = 0;
		this.totalDeaths = 0;
		this.totalItemsPurchased = new List<string>();
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		this.maxNumberOfPlayersIngame = this.vrRigs.Count + 1;
		this.totalItemsHeldThisShift.Clear();
		this.totalItemTypesHeldThisShift.Clear();
		this.numShiftsPlayed = 0;
		this.isFirstShift = false;
	}

	// Token: 0x06003314 RID: 13076 RVA: 0x00117E62 File Offset: 0x00116062
	public void IncrementCoresCollectedPlayer(int coreValue)
	{
		this.totalCoresCollectedByPlayer += coreValue;
		this.coresCollectedByPlayer += coreValue;
	}

	// Token: 0x06003315 RID: 13077 RVA: 0x00117E80 File Offset: 0x00116080
	public void IncrementCoresCollectedGroup(int coreValue)
	{
		this.totalCoresCollectedByGroup += coreValue;
		this.coresCollectedByGroup += coreValue;
	}

	// Token: 0x06003316 RID: 13078 RVA: 0x00117E9E File Offset: 0x0011609E
	public void IncrementCoresSpentPlayer(int coreValue)
	{
		this.totalCoresSpentByPlayer += coreValue;
		this.coresSpentByPlayer += coreValue;
	}

	// Token: 0x06003317 RID: 13079 RVA: 0x00117EBC File Offset: 0x001160BC
	public void IncrementCoresSpentGroup(int coreValue)
	{
		this.totalCoresSpentByGroup += coreValue;
		this.coresSpentByGroup += coreValue;
	}

	// Token: 0x06003318 RID: 13080 RVA: 0x00117EDA File Offset: 0x001160DA
	public void IncrementChaosSeedsCollected(int numSeeds)
	{
		this.sentientCoresCollected += numSeeds;
	}

	// Token: 0x06003319 RID: 13081 RVA: 0x00117EEA File Offset: 0x001160EA
	public void IncrementGatesUnlocked(int numGatesUnlocked)
	{
		this.gatesUnlocked += numGatesUnlocked;
		this.totalGatesUnlocked += numGatesUnlocked;
	}

	// Token: 0x0600331A RID: 13082 RVA: 0x00117F08 File Offset: 0x00116108
	public void IncrementDeaths(int numDeaths)
	{
		this.deaths += numDeaths;
		this.totalDeaths += numDeaths;
	}

	// Token: 0x0600331B RID: 13083 RVA: 0x00117F26 File Offset: 0x00116126
	public void IncrementRevives(int numRevives)
	{
		this.revives += numRevives;
		this.totalRevives += numRevives;
	}

	// Token: 0x0600331C RID: 13084 RVA: 0x00117F44 File Offset: 0x00116144
	public void IncrementShiftsPlayed(int numShifts)
	{
		this.numShiftsPlayed += numShifts;
	}

	// Token: 0x0600331D RID: 13085 RVA: 0x00117F54 File Offset: 0x00116154
	public void AddItemPurchased(string newItemPurchased)
	{
		this.itemsPurchased.Add(newItemPurchased);
		this.totalItemsPurchased.Add(newItemPurchased);
	}

	// Token: 0x0600331E RID: 13086 RVA: 0x00117F70 File Offset: 0x00116170
	public void GrabbedItem(GameEntityId id, string itemName)
	{
		if (this.itemsHeldThisShift.Contains(id))
		{
			return;
		}
		this.itemsHeldThisShift.Add(id);
		if (this.itemTypesHeldThisShift.ContainsKey(itemName))
		{
			this.itemTypesHeldThisShift[itemName] = this.itemTypesHeldThisShift[itemName] + 1;
		}
		else
		{
			this.itemTypesHeldThisShift[itemName] = 1;
		}
		if (this.totalItemsHeldThisShift.Contains(id))
		{
			return;
		}
		this.totalItemsHeldThisShift.Add(id);
		if (this.totalItemTypesHeldThisShift.ContainsKey(itemName))
		{
			this.totalItemTypesHeldThisShift[itemName] = this.totalItemTypesHeldThisShift[itemName] + 1;
			return;
		}
		this.totalItemTypesHeldThisShift[itemName] = 1;
	}

	// Token: 0x0600331F RID: 13087 RVA: 0x00118024 File Offset: 0x00116224
	public GRShuttle GetAssignedShuttle(bool isOnDrillovator)
	{
		GhostReactor instance = GhostReactor.instance;
		GRShuttle drillShuttleForPlayer = GRElevatorManager._instance.GetDrillShuttleForPlayer(this.gamePlayer.rig.OwningNetPlayer.ActorNumber);
		GRShuttle stagingShuttleForPlayer = GRElevatorManager._instance.GetStagingShuttleForPlayer(this.gamePlayer.rig.OwningNetPlayer.ActorNumber);
		if (!isOnDrillovator)
		{
			return stagingShuttleForPlayer;
		}
		return drillShuttleForPlayer;
	}

	// Token: 0x06003320 RID: 13088 RVA: 0x00118080 File Offset: 0x00116280
	public void RefreshShuttles()
	{
		GRShuttle grshuttle = this.GetAssignedShuttle(true);
		if (grshuttle != null)
		{
			grshuttle.Refresh();
		}
		grshuttle = this.GetAssignedShuttle(false);
		if (grshuttle != null)
		{
			grshuttle.Refresh();
		}
	}

	// Token: 0x06003321 RID: 13089 RVA: 0x001180BC File Offset: 0x001162BC
	public static GRPlayer GetFromUserId(string userId)
	{
		GRPlayer.tempRigs.Clear();
		GRPlayer.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GRPlayer.tempRigs);
		for (int i = 0; i < GRPlayer.tempRigs.Count; i++)
		{
			if (GRPlayer.tempRigs[i].OwningNetPlayer != null && GRPlayer.tempRigs[i].OwningNetPlayer.UserId == userId)
			{
				return GRPlayer.Get(GRPlayer.tempRigs[i].OwningNetPlayer);
			}
		}
		return null;
	}

	// Token: 0x06003322 RID: 13090 RVA: 0x0011814C File Offset: 0x0011634C
	[ContextMenu("Refresh Damage Vignette Visual")]
	public void RefreshDamageVignetteVisual()
	{
		if (this.vrRig.isLocal && this.currentHealthVisualValue != this.hp)
		{
			this.currentHealthVisualValue = this.hp;
			if (this.hp <= this.damageOverlayMaxHp && this.hp > 0)
			{
				if (this.lowHeathVisualCoroutine != null)
				{
					base.StopCoroutine(this.lowHeathVisualCoroutine);
				}
				this.damageEffects.lowHealthVisualRenderer.gameObject.SetActive(true);
				this.lowHeathVisualCoroutine = base.StartCoroutine(this.LowHeathVisualCoroutine());
				return;
			}
			this.damageEffects.lowHealthVisualRenderer.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003323 RID: 13091 RVA: 0x001181ED File Offset: 0x001163ED
	private IEnumerator LowHeathVisualCoroutine()
	{
		int index = this.hp - 1;
		if (index >= 0 && index < this.damageOverlayValues.Count)
		{
			float startTime = Time.time;
			while (Time.time - startTime < this.damageOverlayValues[index].effectDuration)
			{
				float num = Mathf.Clamp01((Time.time - startTime) / this.damageOverlayValues[index].effectDuration);
				float num2 = this.damageOverlayValues[index].effectCurve.Evaluate(num);
				Color tint = this.damageOverlayValues[index].tint;
				tint.a *= num2;
				this.damageEffects.lowHealthVisualRenderer.GetPropertyBlock(this.lowHealthVisualPropertyBlock);
				this.lowHealthVisualPropertyBlock.SetColor(this.lowHealthTintPropertyId, tint);
				this.damageEffects.lowHealthVisualRenderer.SetPropertyBlock(this.lowHealthVisualPropertyBlock);
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x06003324 RID: 13092 RVA: 0x001181FC File Offset: 0x001163FC
	public void SetGooParticleSystemEnabled(bool bIsLeftHand, bool newEnableState)
	{
		if (this.vrRig != null)
		{
			this.vrRig.SetGooParticleSystemStatus(bIsLeftHand, newEnableState);
		}
	}

	// Token: 0x06003325 RID: 13093 RVA: 0x0011821C File Offset: 0x0011641C
	public void SetAsFrozen(float duration)
	{
		if (GorillaTagger.Instance.currentStatus != GorillaTagger.StatusEffect.Frozen)
		{
			this.freezeDuration = duration;
			if (this.gamePlayer.rig.OwningNetPlayer.IsLocal)
			{
				GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, duration);
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
				GorillaTagger.Instance.offlineVRRig.PlayTaggedEffect();
				if (this.damageEffects.frozenVisualRenderer != null)
				{
					this.damageEffects.frozenVisualRenderer.gameObject.SetActive(true);
				}
				this.playerDamageAudioSource.PlayOneShot(this.playerFrozenSound, 1f);
			}
			this.gamePlayer.rig.UpdateFrozenEffect(true);
			base.Invoke("RemoveFrozen", duration);
		}
	}

	// Token: 0x06003326 RID: 13094 RVA: 0x00118310 File Offset: 0x00116510
	public void RemoveFrozen()
	{
		this.gamePlayer.rig.UpdateFrozenEffect(false);
		this.freezeDuration = 0f;
		if (this.damageEffects.frozenVisualRenderer != null)
		{
			this.damageEffects.frozenVisualRenderer.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003327 RID: 13095 RVA: 0x00118364 File Offset: 0x00116564
	public override void Tick()
	{
		if (this.lastPlayerPosition != Vector3.zero)
		{
			Vector3 position = this.vrRig.transform.position;
			float magnitude = (this.lastPlayerPosition - position).magnitude;
			this.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.DistanceTraveled, magnitude);
		}
		this.lastPlayerPosition = this.vrRig.transform.position;
		if (this.freezeDuration > 0f)
		{
			this.gamePlayer.rig.UpdateFrozen(Time.deltaTime, this.freezeDuration);
		}
		if (this.inStealthMode && Time.timeAsDouble > this.shieldStealthModeEndTime)
		{
			this.ClearStealthMode();
		}
		GRShuttle.UpdateGRPlayerShuttle(this);
		if (this.soak != null && this.soak.IsSoaking())
		{
			this.soak.OnUpdate();
		}
	}

	// Token: 0x06003328 RID: 13096 RVA: 0x00118430 File Offset: 0x00116630
	public void SetSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat stat, float amt)
	{
		this.synchronizedSessionStats[(int)stat] = amt;
	}

	// Token: 0x06003329 RID: 13097 RVA: 0x0011843B File Offset: 0x0011663B
	public void IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat stat, float amt)
	{
		this.synchronizedSessionStats[(int)stat] += amt;
	}

	// Token: 0x0600332A RID: 13098 RVA: 0x00118450 File Offset: 0x00116650
	public void ResetSynchronizedSessionStats()
	{
		for (int i = 0; i < 8; i++)
		{
			this.synchronizedSessionStats[i] = 0f;
		}
	}

	// Token: 0x0600332B RID: 13099 RVA: 0x00118478 File Offset: 0x00116678
	private void RequestSetMothershipUserData(string keyName, string value)
	{
		if (this.saveEquipmentInProgress)
		{
			Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: request already in progress");
			return;
		}
		this.saveEquipmentInProgress = true;
		try
		{
			if (!MothershipClientApiUnity.SetUserDataValue(keyName, value, new Action<SetUserDataResponse>(this.OnSetMothershipUserDataSuccess), new Action<MothershipError, int>(this.OnSetMothershipUserDataFail), ""))
			{
				Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: SetUserDataValue Fail");
				this.OnSetMothershipDataComplete(false);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: exception " + ex.Message);
			this.OnSetMothershipDataComplete(false);
		}
	}

	// Token: 0x0600332C RID: 13100 RVA: 0x00118508 File Offset: 0x00116708
	private void OnSetMothershipUserDataSuccess(SetUserDataResponse response)
	{
		GTDev.Log<string>("GRPlayer OnSetMothershipUserDataSuccess", null);
		this.OnSetMothershipDataComplete(true);
		response.Dispose();
	}

	// Token: 0x0600332D RID: 13101 RVA: 0x00118524 File Offset: 0x00116724
	private void OnSetMothershipUserDataFail(MothershipError error, int status)
	{
		string text = ((error == null) ? status.ToString() : error.Message);
		GTDev.LogError<string>("GRPlayer OnSetMothershipUserDataFail: " + text, null);
		this.OnSetMothershipDataComplete(false);
		if (error != null)
		{
			error.Dispose();
		}
	}

	// Token: 0x0600332E RID: 13102 RVA: 0x00118565 File Offset: 0x00116765
	private void OnSetMothershipDataComplete(bool success)
	{
		this.saveEquipmentInProgress = false;
	}

	// Token: 0x0600332F RID: 13103 RVA: 0x00118570 File Offset: 0x00116770
	public void RequestFetchMothershipUserData(string key)
	{
		if (!this.hasPulledEquipment)
		{
			try
			{
				if (!MothershipClientApiUnity.GetUserDataValue(key, new Action<MothershipUserData>(this.OnGetMothershipFetchUserDataSuccess), new Action<MothershipError, int>(this.OnGetMothershipFetchUserDataFail), ""))
				{
					Debug.LogError("GRPlayer RequestFetchMothershipUserData failed ");
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("GRPlayer RequestFetchMothershipUserData exception " + ex.Message);
			}
		}
	}

	// Token: 0x06003330 RID: 13104 RVA: 0x001185E0 File Offset: 0x001167E0
	private void OnGetMothershipFetchUserDataSuccess(MothershipUserData response)
	{
		GTDev.Log<string>("GRPlayer OnGetMothershipFetchUserDataSuccess", null);
		bool flag = response != null && response.value != null && response.value.Length > 0;
		if (response != null)
		{
		}
		if (response != null)
		{
			response.Dispose();
		}
	}

	// Token: 0x06003331 RID: 13105 RVA: 0x00118624 File Offset: 0x00116824
	private void OnGetMothershipFetchUserDataFail(MothershipError error, int status)
	{
		string text = ((error == null) ? status.ToString() : error.Message);
		GTDev.LogError<string>("GRPlayer OnGetMothershipFetchUserDataFail: " + text, null);
		if (error != null)
		{
			error.Dispose();
		}
	}

	// Token: 0x06003332 RID: 13106 RVA: 0x0011865E File Offset: 0x0011685E
	public bool IsDropPodUnlocked()
	{
		return this.dropPodLevel > 0;
	}

	// Token: 0x06003333 RID: 13107 RVA: 0x0011866C File Offset: 0x0011686C
	public int GetMaxDropFloor()
	{
		switch (this.dropPodChasisLevel + this.dropPodLevel)
		{
		case 0:
			return 1;
		case 1:
			return 5;
		case 2:
			return 10;
		case 3:
			return 15;
		case 4:
			return 20;
		default:
			return 0;
		}
	}

	// Token: 0x06003334 RID: 13108 RVA: 0x001186B1 File Offset: 0x001168B1
	public void CollectShiftCut()
	{
		this.SetProgressionData(this.currentProgression.points + this.LastShiftCut, this.currentProgression.redeemedPoints, true);
	}

	// Token: 0x06003335 RID: 13109 RVA: 0x001186D8 File Offset: 0x001168D8
	public bool AttemptPromotion()
	{
		ValueTuple<int, int, int, int> gradePointDetails = GhostReactorProgression.GetGradePointDetails(this.CurrentProgression.redeemedPoints);
		int item = gradePointDetails.Item3;
		int item2 = gradePointDetails.Item4;
		if (item - item2 < this.CurrentProgression.points - this.CurrentProgression.redeemedPoints)
		{
			this.SetProgressionData(this.currentProgression.points, this.currentProgression.points, false);
			return true;
		}
		return false;
	}

	// Token: 0x06003336 RID: 13110 RVA: 0x00118740 File Offset: 0x00116940
	public void SetProgressionData(int _points, int _redeemedPoints, bool saveProgression = false)
	{
		if (_points < 0 || _redeemedPoints < 0)
		{
			return;
		}
		this.currentProgression = new GRPlayer.ProgressionData
		{
			points = _points,
			redeemedPoints = _redeemedPoints
		};
		if (this.gamePlayer.IsLocal() && saveProgression)
		{
			this.SaveMyProgression();
		}
	}

	// Token: 0x06003337 RID: 13111 RVA: 0x0011878A File Offset: 0x0011698A
	public void LoadMyProgression()
	{
		GhostReactorProgression.instance.GetStartingProgression(this);
	}

	// Token: 0x06003338 RID: 13112 RVA: 0x00118797 File Offset: 0x00116997
	public void SaveMyProgression()
	{
		GhostReactorProgression.instance.SetProgression(this.LastShiftCut, this);
	}

	// Token: 0x040041E1 RID: 16865
	public const int MAX_CURRENCY = 500;

	// Token: 0x040041E2 RID: 16866
	public GamePlayer gamePlayer;

	// Token: 0x040041E3 RID: 16867
	private GRPlayer.GRPlayerState state;

	// Token: 0x040041E4 RID: 16868
	private int shiftCreditCache;

	// Token: 0x040041E5 RID: 16869
	public int startingShiftCreditCache;

	// Token: 0x040041E6 RID: 16870
	public int playerJuice;

	// Token: 0x040041E9 RID: 16873
	public double shiftJoinTime;

	// Token: 0x040041EA RID: 16874
	public bool isEmployee;

	// Token: 0x040041EB RID: 16875
	public AudioSource audioSource;

	// Token: 0x040041EC RID: 16876
	[Header("Hit / Revive Effects")]
	public ParticleSystem playerTurnedGhostEffect;

	// Token: 0x040041ED RID: 16877
	public SoundBankPlayer playerTurnedGhostSoundBank;

	// Token: 0x040041EE RID: 16878
	public ParticleSystem playerRevivedEffect;

	// Token: 0x040041EF RID: 16879
	public AudioClip playerRevivedSound;

	// Token: 0x040041F0 RID: 16880
	public float playerRevivedVolume = 1f;

	// Token: 0x040041F1 RID: 16881
	public AudioSource playerDamageAudioSource;

	// Token: 0x040041F2 RID: 16882
	public Transform bodyCenter;

	// Token: 0x040041F3 RID: 16883
	public ParticleSystem playerDamageEffect;

	// Token: 0x040041F4 RID: 16884
	public float playerDamageVolume = 1f;

	// Token: 0x040041F5 RID: 16885
	public AudioClip playerDamageSound;

	// Token: 0x040041F6 RID: 16886
	public float playerDamageOffsetDist = 0.25f;

	// Token: 0x040041F7 RID: 16887
	[ColorUsage(true, true)]
	[SerializeField]
	private Color deathTintColor;

	// Token: 0x040041F8 RID: 16888
	[ColorUsage(true, true)]
	[SerializeField]
	private Color deathAmbientLightColor;

	// Token: 0x040041F9 RID: 16889
	public GameLight shieldGameLight;

	// Token: 0x040041FA RID: 16890
	[Header("Attach")]
	public Transform attachEnemy;

	// Token: 0x040041FB RID: 16891
	[Header("Shield")]
	public Transform shieldHeadVisual;

	// Token: 0x040041FC RID: 16892
	public Transform shieldBodyVisual;

	// Token: 0x040041FD RID: 16893
	public AudioClip shieldActivatedSound;

	// Token: 0x040041FE RID: 16894
	public float shieldActivatedVolume = 0.5f;

	// Token: 0x040041FF RID: 16895
	public ParticleSystem shieldDamagedEffect;

	// Token: 0x04004200 RID: 16896
	public AudioClip shieldDamagedSound;

	// Token: 0x04004201 RID: 16897
	public float shieldDamagedVolume = 0.5f;

	// Token: 0x04004202 RID: 16898
	public ParticleSystem shieldDestroyedEffect;

	// Token: 0x04004203 RID: 16899
	public AudioClip shieldDestroyedSound;

	// Token: 0x04004204 RID: 16900
	public float shieldDestroyedVolume = 0.5f;

	// Token: 0x04004205 RID: 16901
	public float shieldStealthModeDuration = 20f;

	// Token: 0x04004206 RID: 16902
	private double shieldStealthModeEndTime;

	// Token: 0x04004207 RID: 16903
	public Color shieldColorNormal = new Color(0.42352942f, 0.25490198f, 1f, 0.45490196f);

	// Token: 0x04004208 RID: 16904
	public Color shieldColorLight = new Color(1f, 1f, 1f, 0.5f);

	// Token: 0x04004209 RID: 16905
	public Color shieldColorStealth = new Color(1f, 0.2f, 0f, 0.5f);

	// Token: 0x0400420A RID: 16906
	public Color shieldColorHeal = new Color(0f, 1f, 1f, 0.5f);

	// Token: 0x0400420B RID: 16907
	public int xRayVisionRefCount;

	// Token: 0x0400420C RID: 16908
	[Header("Badge")]
	public Transform badgeBodyAnchor;

	// Token: 0x0400420D RID: 16909
	[SerializeField]
	private Transform badgeBodyStringAttach;

	// Token: 0x0400420E RID: 16910
	[NonSerialized]
	public double lastLeftWithBadgeAttachedTime;

	// Token: 0x0400420F RID: 16911
	[Header("Health")]
	[SerializeField]
	private int maxHp = 1;

	// Token: 0x04004210 RID: 16912
	[SerializeField]
	private int maxShieldHp = 1;

	// Token: 0x04004211 RID: 16913
	public string mothershipId;

	// Token: 0x04004212 RID: 16914
	private int hp;

	// Token: 0x04004213 RID: 16915
	private int shieldHp;

	// Token: 0x04004214 RID: 16916
	private int shieldFlags;

	// Token: 0x04004215 RID: 16917
	private bool inStealthMode;

	// Token: 0x04004216 RID: 16918
	[Header("Damage Vignette")]
	[SerializeField]
	[Tooltip("First entry is 1 hp, second entry is 2 hp, etc.")]
	private List<GRPlayer.DamageOverlayValues> damageOverlayValues = new List<GRPlayer.DamageOverlayValues>();

	// Token: 0x04004217 RID: 16919
	[SerializeField]
	private int damageOverlayMaxHp = 1;

	// Token: 0x04004218 RID: 16920
	[HideInInspector]
	public GRBadge badge;

	// Token: 0x04004219 RID: 16921
	public CallLimiter requestCollectItemLimiter;

	// Token: 0x0400421A RID: 16922
	public CallLimiter requestChargeToolLimiter;

	// Token: 0x0400421B RID: 16923
	public CallLimiter requestDepositCurrencyLimiter;

	// Token: 0x0400421C RID: 16924
	public CallLimiter requestShiftStartLimiter;

	// Token: 0x0400421D RID: 16925
	public CallLimiter requestToolPurchaseStationLimiter;

	// Token: 0x0400421E RID: 16926
	public CallLimiter applyEnemyHitLimiter;

	// Token: 0x0400421F RID: 16927
	public CallLimiter reportLocalHitLimiter;

	// Token: 0x04004220 RID: 16928
	public CallLimiter reportBreakableBrokenLimiter;

	// Token: 0x04004221 RID: 16929
	public CallLimiter playerStateChangeLimiter;

	// Token: 0x04004222 RID: 16930
	public CallLimiter promotionBotLimiter;

	// Token: 0x04004223 RID: 16931
	public CallLimiter progressionBroadcastLimiter;

	// Token: 0x04004224 RID: 16932
	public CallLimiter scoreboardPageLimiter;

	// Token: 0x04004225 RID: 16933
	public CallLimiter fireShieldLimiter;

	// Token: 0x04004226 RID: 16934
	private VRRig vrRig;

	// Token: 0x04004227 RID: 16935
	private List<VRRig> vrRigs = new List<VRRig>();

	// Token: 0x04004228 RID: 16936
	private string gameId;

	// Token: 0x04004229 RID: 16937
	public int coresCollectedByPlayer;

	// Token: 0x0400422A RID: 16938
	public int coresCollectedByGroup;

	// Token: 0x0400422B RID: 16939
	public int coresSpentByPlayer;

	// Token: 0x0400422C RID: 16940
	public int coresSpentByGroup;

	// Token: 0x0400422D RID: 16941
	public int gatesUnlocked;

	// Token: 0x0400422E RID: 16942
	public int deaths;

	// Token: 0x0400422F RID: 16943
	public bool caughtByAnomaly;

	// Token: 0x04004230 RID: 16944
	public List<string> itemsPurchased;

	// Token: 0x04004231 RID: 16945
	public List<string> levelsUnlocked;

	// Token: 0x04004232 RID: 16946
	public float timeIntoShiftAtJoin;

	// Token: 0x04004233 RID: 16947
	public bool wasPlayerInAtShiftStart;

	// Token: 0x04004234 RID: 16948
	public int sentientCoresCollected;

	// Token: 0x04004235 RID: 16949
	public int maxNumberOfPlayersInShift;

	// Token: 0x04004236 RID: 16950
	public int revives;

	// Token: 0x04004237 RID: 16951
	public float[] synchronizedSessionStats = new float[8];

	// Token: 0x04004238 RID: 16952
	private HashSet<GameEntityId> itemsHeldThisShift = new HashSet<GameEntityId>();

	// Token: 0x04004239 RID: 16953
	private Dictionary<string, int> itemTypesHeldThisShift = new Dictionary<string, int>();

	// Token: 0x0400423A RID: 16954
	public int totalCoresCollectedByPlayer;

	// Token: 0x0400423B RID: 16955
	public int totalCoresCollectedByGroup;

	// Token: 0x0400423C RID: 16956
	public int totalCoresSpentByPlayer;

	// Token: 0x0400423D RID: 16957
	public int totalCoresSpentByGroup;

	// Token: 0x0400423E RID: 16958
	public int totalGatesUnlocked;

	// Token: 0x0400423F RID: 16959
	public int totalDeaths;

	// Token: 0x04004240 RID: 16960
	public List<string> totalItemsPurchased;

	// Token: 0x04004241 RID: 16961
	public float timeIntoGameAtJoin;

	// Token: 0x04004242 RID: 16962
	public bool wasPlayerInAtGameStart;

	// Token: 0x04004243 RID: 16963
	public int maxNumberOfPlayersIngame;

	// Token: 0x04004244 RID: 16964
	public int totalRevives;

	// Token: 0x04004245 RID: 16965
	public int numShiftsPlayed;

	// Token: 0x04004246 RID: 16966
	public float gameStartTime;

	// Token: 0x04004247 RID: 16967
	public bool isFirstShift = true;

	// Token: 0x04004248 RID: 16968
	private HashSet<GameEntityId> totalItemsHeldThisShift = new HashSet<GameEntityId>();

	// Token: 0x04004249 RID: 16969
	private Dictionary<string, int> totalItemTypesHeldThisShift = new Dictionary<string, int>();

	// Token: 0x0400424A RID: 16970
	private GRPlayerDamageEffects damageEffects;

	// Token: 0x0400424B RID: 16971
	private MaterialPropertyBlock lowHealthVisualPropertyBlock;

	// Token: 0x0400424C RID: 16972
	private int lowHealthTintPropertyId;

	// Token: 0x0400424D RID: 16973
	private int currentHealthVisualValue;

	// Token: 0x0400424E RID: 16974
	private Coroutine lowHeathVisualCoroutine;

	// Token: 0x0400424F RID: 16975
	public AudioClip playerFrozenSound;

	// Token: 0x04004250 RID: 16976
	public GRPlayer.ShuttleData shuttleData;

	// Token: 0x04004251 RID: 16977
	private GRPlayer.ProgressionData currentProgression;

	// Token: 0x04004252 RID: 16978
	private float shiftPlayTime;

	// Token: 0x04004253 RID: 16979
	private int lastShiftCut;

	// Token: 0x04004254 RID: 16980
	private GhostReactorSoak soak;

	// Token: 0x04004255 RID: 16981
	private static List<VRRig> tempRigs = new List<VRRig>(32);

	// Token: 0x04004256 RID: 16982
	private float freezeDuration;

	// Token: 0x04004257 RID: 16983
	private Vector3 lastPlayerPosition = Vector3.zero;

	// Token: 0x04004258 RID: 16984
	private bool saveEquipmentInProgress;

	// Token: 0x04004259 RID: 16985
	private bool hasPulledEquipment;

	// Token: 0x0400425A RID: 16986
	public int dropPodLevel;

	// Token: 0x0400425B RID: 16987
	public int dropPodChasisLevel;

	// Token: 0x020007CE RID: 1998
	public enum GRPlayerState
	{
		// Token: 0x0400425D RID: 16989
		Alive,
		// Token: 0x0400425E RID: 16990
		Ghost,
		// Token: 0x0400425F RID: 16991
		Shielded
	}

	// Token: 0x020007CF RID: 1999
	public enum GRPlayerShieldFlags
	{
		// Token: 0x04004261 RID: 16993
		Light = 1,
		// Token: 0x04004262 RID: 16994
		Stealth,
		// Token: 0x04004263 RID: 16995
		Heal = 4
	}

	// Token: 0x020007D0 RID: 2000
	public enum SynchronizedSessionStat
	{
		// Token: 0x04004265 RID: 16997
		CoresDeposited,
		// Token: 0x04004266 RID: 16998
		EarnedCredits,
		// Token: 0x04004267 RID: 16999
		SpentCredits,
		// Token: 0x04004268 RID: 17000
		DistanceTraveled,
		// Token: 0x04004269 RID: 17001
		Deaths,
		// Token: 0x0400426A RID: 17002
		Kills,
		// Token: 0x0400426B RID: 17003
		Assists,
		// Token: 0x0400426C RID: 17004
		TimeChaosExposure,
		// Token: 0x0400426D RID: 17005
		Count
	}

	// Token: 0x020007D1 RID: 2001
	[Serializable]
	private struct DamageOverlayValues
	{
		// Token: 0x0400426E RID: 17006
		public Color tint;

		// Token: 0x0400426F RID: 17007
		public float effectDuration;

		// Token: 0x04004270 RID: 17008
		public AnimationCurve effectCurve;
	}

	// Token: 0x020007D2 RID: 2002
	public enum ShuttleState
	{
		// Token: 0x04004272 RID: 17010
		Idle,
		// Token: 0x04004273 RID: 17011
		Moving,
		// Token: 0x04004274 RID: 17012
		WaitForLeaveRoom,
		// Token: 0x04004275 RID: 17013
		JoinRoom,
		// Token: 0x04004276 RID: 17014
		WaitForLeadPlayer,
		// Token: 0x04004277 RID: 17015
		Teleport,
		// Token: 0x04004278 RID: 17016
		TeleportToMyShuttleSafety,
		// Token: 0x04004279 RID: 17017
		PostTeleport
	}

	// Token: 0x020007D3 RID: 2003
	public class ShuttleData
	{
		// Token: 0x0400427A RID: 17018
		public string ownerUserId;

		// Token: 0x0400427B RID: 17019
		public int currShuttleId;

		// Token: 0x0400427C RID: 17020
		public int targetShuttleId;

		// Token: 0x0400427D RID: 17021
		public int targetLevel;

		// Token: 0x0400427E RID: 17022
		public GRPlayer.ShuttleState state;

		// Token: 0x0400427F RID: 17023
		public double stateStartTime;
	}

	// Token: 0x020007D4 RID: 2004
	[Serializable]
	public struct ProgressionData
	{
		// Token: 0x04004280 RID: 17024
		public int points;

		// Token: 0x04004281 RID: 17025
		public int redeemedPoints;
	}

	// Token: 0x020007D5 RID: 2005
	[Serializable]
	public struct ProgressionLevels
	{
		// Token: 0x04004282 RID: 17026
		public int tierId;

		// Token: 0x04004283 RID: 17027
		public string tierName;

		// Token: 0x04004284 RID: 17028
		public int grades;

		// Token: 0x04004285 RID: 17029
		public int pointsPerGrade;
	}
}
