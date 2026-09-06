using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Fusion;
using GorillaTag.Rendering;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

// Token: 0x02000705 RID: 1797
public class GhostReactor : MonoBehaviourTick, IBuildValidation
{
	// Token: 0x06002D5A RID: 11610 RVA: 0x000F4CF4 File Offset: 0x000F2EF4
	public static GhostReactor Get(GameEntity gameEntity)
	{
		GhostReactorManager ghostReactorManager = GhostReactorManager.Get(gameEntity);
		if (ghostReactorManager == null)
		{
			return null;
		}
		return ghostReactorManager.reactor;
	}

	// Token: 0x06002D5B RID: 11611 RVA: 0x000F4D1C File Offset: 0x000F2F1C
	private void Awake()
	{
		GhostReactor.instance = this;
		this.reviveStations = new List<GRReviveStation>();
		base.GetComponentsInChildren<GRReviveStation>(this.reviveStations);
		for (int i = 0; i < this.reviveStations.Count; i++)
		{
			this.reviveStations[i].Init(this, i);
		}
		this.vrRigs = new List<VRRig>();
		for (int j = 0; j < this.itemPurchaseStands.Count; j++)
		{
			if (this.itemPurchaseStands[j] == null)
			{
				Debug.LogErrorFormat("Null Item Purchase Stand {0}", new object[] { j });
			}
			else
			{
				this.itemPurchaseStands[j].Setup(j);
			}
		}
		for (int k = 0; k < this.toolPurchasingStations.Count; k++)
		{
			if (this.toolPurchasingStations[k] == null)
			{
				Debug.LogErrorFormat("Null Tool Purchasing Station {0}", new object[] { k });
			}
			else
			{
				this.toolPurchasingStations[k].PurchaseStationId = k;
			}
		}
		if (this.promotionBot != null)
		{
			this.promotionBot.Init(this);
		}
		this.randomGenerator = new SRand(Random.Range(0, int.MaxValue));
		this.handPrintMPB = new MaterialPropertyBlock();
		this.handPrintMPB.SetFloatArray("_HandPrintData", new float[1024]);
		this.bays = new List<GRBay>(32);
		base.GetComponentsInChildren<GRBay>(false, this.bays);
		this.storeDisplays = new List<GRUIStoreDisplay>();
		base.GetComponentsInChildren<GRUIStoreDisplay>(false, this.storeDisplays);
	}

	// Token: 0x06002D5C RID: 11612 RVA: 0x000F4EB0 File Offset: 0x000F30B0
	private new void OnEnable()
	{
		base.OnEnable();
		if (this.zone == GTZone.customMaps)
		{
			return;
		}
		GTDev.Log<string>(string.Format("GhostReactor::OnEnable getting manager for zone {0}", this.zone), null);
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.zone);
		if (managerForZone == null)
		{
			Debug.LogErrorFormat("No GameEntityManager found for zone {0}", new object[] { this.zone });
			return;
		}
		this.grManager = managerForZone.ghostReactorManager;
		if (this.grManager == null)
		{
			Debug.LogErrorFormat("No GhostReactorManager found for zone {0}", new object[] { this.zone });
			return;
		}
		this.grManager.reactor = this;
		this.grManager.gameEntityManager.boundsBoxCollider = this.boundsBoxCollider;
		if (GameLightingManager.instance != null && this.zone != GTZone.customMaps)
		{
			GameLightingManager.instance.ZoneEnableCustomDynamicLighting(true);
		}
		VRRigCache.OnRigActivated += this.OnVRRigsChanged;
		VRRigCache.OnRigDeactivated += this.OnVRRigsChanged;
		VRRigCache.OnRigNameChanged += this.OnVRRigsChanged;
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnLocalPlayerConnectedToRoom;
		}
		for (int i = 0; i < this.toolPurchasingStations.Count; i++)
		{
			this.toolPurchasingStations[i].Init(this.grManager, this);
		}
		if (this.debugUpgradeKiosk != null)
		{
			this.debugUpgradeKiosk.Init(this.grManager, this);
		}
		if (this.currencyDepositor != null)
		{
			this.currencyDepositor.Init(this);
		}
		if (this.distillery != null)
		{
			this.distillery.Init(this);
		}
		if (this.seedExtractor != null)
		{
			this.seedExtractor.Init(this.toolProgression, this);
		}
		if (this.levelGenerator != null)
		{
			this.levelGenerator.Init(this);
		}
		if (this.employeeBadges != null)
		{
			this.employeeBadges.Init(this);
		}
		if (this.toolProgression != null)
		{
			this.toolProgression.Init(this);
			this.toolProgression.OnProgressionUpdated += this.OnProgressionUpdated;
		}
		if (this.shiftManager != null)
		{
			this.shiftManager.Init(this.grManager);
		}
		for (int j = 0; j < this.toolUpgradePurchaseStationsFull.Count; j++)
		{
			this.toolUpgradePurchaseStationsFull[j].Init(this.toolProgression, this);
		}
		GRElevatorManager._instance.InitShuttles(this);
		if (this.recycler != null)
		{
			this.recycler.Init(this);
		}
		if (this.zoneShaderSettings != null)
		{
			this.zoneShaderSettings.BecomeActiveInstance(true);
		}
		for (int k = 0; k < this.bays.Count; k++)
		{
			this.bays[k].Setup(this);
		}
		for (int l = 0; l < this.storeDisplays.Count; l++)
		{
			this.storeDisplays[l].Setup(-1, this);
		}
		this.RefreshDepth();
	}

	// Token: 0x06002D5D RID: 11613 RVA: 0x000F51EE File Offset: 0x000F33EE
	public void EnableGhostReactorForVirtualStump()
	{
		GhostReactor.instance = this;
		this.RefreshReviveStations(false);
		this.OnEnable();
	}

	// Token: 0x06002D5E RID: 11614 RVA: 0x000F5204 File Offset: 0x000F3404
	public void RefreshReviveStations(bool searchScene = false)
	{
		this.reviveStations = new List<GRReviveStation>();
		base.GetComponentsInChildren<GRReviveStation>(this.reviveStations);
		if (searchScene)
		{
			this.reviveStations.AddRange(Object.FindObjectsByType<GRReviveStation>(FindObjectsInactive.Include, FindObjectsSortMode.None));
		}
		for (int i = 0; i < this.reviveStations.Count; i++)
		{
			this.reviveStations[i].Init(this, i);
		}
	}

	// Token: 0x06002D5F RID: 11615 RVA: 0x000F5268 File Offset: 0x000F3468
	private new void OnDisable()
	{
		base.OnDisable();
		if (this.zone == GTZone.customMaps)
		{
			return;
		}
		GameLightingManager.instance.ZoneEnableCustomDynamicLighting(false);
		VRRigCache.OnRigActivated -= this.OnVRRigsChanged;
		VRRigCache.OnRigDeactivated -= this.OnVRRigsChanged;
		VRRigCache.OnRigNameChanged -= this.OnVRRigsChanged;
		if (this.toolProgression != null)
		{
			this.toolProgression.OnProgressionUpdated -= this.OnProgressionUpdated;
		}
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnMultiplayerStarted -= this.OnLocalPlayerConnectedToRoom;
		}
	}

	// Token: 0x06002D60 RID: 11616 RVA: 0x000F5319 File Offset: 0x000F3519
	private void OnProgressionUpdated()
	{
		if (this.toolProgression != null)
		{
			this.UpdateLocalPlayerFromProgression();
		}
	}

	// Token: 0x06002D61 RID: 11617 RVA: 0x000F5330 File Offset: 0x000F3530
	public void UpdateLocalPlayerFromProgression()
	{
		GRPlayer local = GRPlayer.GetLocal();
		if (local != null)
		{
			int dropPodLevel = this.toolProgression.GetDropPodLevel();
			if (local.dropPodLevel != dropPodLevel)
			{
				local.dropPodLevel = dropPodLevel;
				Debug.LogFormat("Drop Pod UpdateLocalPlayerFromProgression Level {0} {1} {2}", new object[]
				{
					this.grManager.IsZoneActive(),
					local.dropPodLevel,
					local.dropPodChasisLevel
				});
				if (this.grManager.IsZoneActive())
				{
					this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SetPodLevel, dropPodLevel);
				}
			}
			int dropPodChasisLevel = this.toolProgression.GetDropPodChasisLevel();
			if (local.dropPodChasisLevel != dropPodChasisLevel)
			{
				local.dropPodChasisLevel = dropPodChasisLevel;
				Debug.LogFormat("Drop Pod UpdateLocalPlayerFromProgression Level {0} {1} {2}", new object[]
				{
					this.grManager.IsZoneActive(),
					local.dropPodLevel,
					local.dropPodChasisLevel
				});
				if (this.grManager.IsZoneActive())
				{
					this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SetPodChassisLevel, dropPodChasisLevel);
				}
			}
			if (local.badge)
			{
				local.badge.RefreshText(PhotonNetwork.LocalPlayer);
			}
			this.RefreshStore();
		}
	}

	// Token: 0x06002D62 RID: 11618 RVA: 0x000F5463 File Offset: 0x000F3663
	public GRPatrolPath GetPatrolPath(long createData)
	{
		if (this.levelGenerator == null)
		{
			return null;
		}
		return this.levelGenerator.GetPatrolPath(createData);
	}

	// Token: 0x06002D63 RID: 11619 RVA: 0x000F5484 File Offset: 0x000F3684
	public override void Tick()
	{
		if (this.grManager == null)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (this.grManager.gameEntityManager.IsAuthority())
		{
			if (Time.timeAsDouble - this.lastCollectibleDispenserUpdateTime > (double)this.collectibleDispenserUpdateFrequency)
			{
				this.lastCollectibleDispenserUpdateTime = Time.timeAsDouble;
				for (int i = 0; i < this.collectibleDispensers.Count; i++)
				{
					if (this.collectibleDispensers[i] != null && this.collectibleDispensers[i].ReadyToDispenseNewCollectible)
					{
						this.collectibleDispensers[i].RequestDispenseCollectible();
					}
				}
			}
			if (this.sleepableEntities.Count > 0)
			{
				this.sentientCoreUpdateIndex = Mathf.Max(0, this.sentientCoreUpdateIndex % this.sleepableEntities.Count);
				if (this.sentientCoreUpdateIndex < this.sleepableEntities.Count)
				{
					IGRSleepableEntity igrsleepableEntity = this.sleepableEntities[this.sentientCoreUpdateIndex];
					float num = igrsleepableEntity.WakeUpRadius * igrsleepableEntity.WakeUpRadius;
					float num2 = (igrsleepableEntity.WakeUpRadius + 0.5f) * (igrsleepableEntity.WakeUpRadius + 0.5f);
					bool flag = false;
					bool flag2 = false;
					for (int j = 0; j < this.vrRigs.Count; j++)
					{
						GRPlayer component = this.vrRigs[j].GetComponent<GRPlayer>();
						if (!(component == null) && component.State != GRPlayer.GRPlayerState.Ghost)
						{
							float sqrMagnitude = (igrsleepableEntity.Position - this.vrRigs[j].bodyTransform.position).sqrMagnitude;
							if (sqrMagnitude < num2)
							{
								flag = true;
							}
							if (sqrMagnitude < num)
							{
								flag2 = true;
								break;
							}
						}
					}
					bool flag3 = igrsleepableEntity.IsSleeping();
					if (flag3 && flag2)
					{
						igrsleepableEntity.WakeUp();
					}
					else if (!flag3 && !flag)
					{
						igrsleepableEntity.Sleep();
					}
					this.sentientCoreUpdateIndex++;
				}
			}
		}
		bool flag4 = false;
		foreach (GhostReactor.EntityTypeRespawnTracker entityTypeRespawnTracker in this.respawnQueue)
		{
			entityTypeRespawnTracker.entityNextRespawnTime -= Time.deltaTime;
			if (entityTypeRespawnTracker.entityNextRespawnTime < 0f)
			{
				entityTypeRespawnTracker.entityNextRespawnTime = 0f;
				flag4 = true;
				if (this.grManager.gameEntityManager.IsAuthority())
				{
					this.levelGenerator.RespawnEntity(entityTypeRespawnTracker.entityTypeID, entityTypeRespawnTracker.entityCreateData, GameEntityId.Invalid);
				}
			}
		}
		if (flag4)
		{
			this.respawnQueue.RemoveAll((GhostReactor.EntityTypeRespawnTracker e) => e.entityNextRespawnTime <= 0f);
		}
		this.UpdateHandprints(Time.deltaTime);
	}

	// Token: 0x06002D64 RID: 11620 RVA: 0x000F573C File Offset: 0x000F393C
	private void OnLocalPlayerConnectedToRoom()
	{
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer != null)
		{
			grplayer.Reset();
		}
		if (this.shiftManager != null)
		{
			this.shiftManager.shiftStats.ResetShiftStats();
			this.shiftManager.RefreshShiftStatsDisplay();
		}
	}

	// Token: 0x06002D65 RID: 11621 RVA: 0x000F578C File Offset: 0x000F398C
	private void OnVRRigsChanged(RigContainer container)
	{
		this.VRRigRefresh();
	}

	// Token: 0x06002D66 RID: 11622 RVA: 0x000F5794 File Offset: 0x000F3994
	public void VRRigRefresh()
	{
		if (this.isRefreshing)
		{
			return;
		}
		this.isRefreshing = true;
		this.vrRigs.Clear();
		this.vrRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		this.vrRigs.Sort(delegate(VRRig a, VRRig b)
		{
			if (a == null || a.OwningNetPlayer == null)
			{
				return 1;
			}
			if (b == null || b.OwningNetPlayer == null)
			{
				return -1;
			}
			return a.OwningNetPlayer.ActorNumber.CompareTo(b.OwningNetPlayer.ActorNumber);
		});
		if (this.promotionBot != null)
		{
			this.promotionBot.Refresh();
		}
		this.RefreshScoreboards();
		this.RefreshDepth();
		this.RefreshStore();
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer != null && this.vrRigs.Count > grplayer.maxNumberOfPlayersInShift)
		{
			grplayer.maxNumberOfPlayersInShift = this.vrRigs.Count;
		}
		this.isRefreshing = false;
	}

	// Token: 0x06002D67 RID: 11623 RVA: 0x000F5870 File Offset: 0x000F3A70
	public void UpdateScoreboardScreen(GRUIScoreboard.ScoreboardScreen newScreen)
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].SwitchToScreen(newScreen);
		}
		this.RefreshScoreboards();
	}

	// Token: 0x06002D68 RID: 11624 RVA: 0x000F58AC File Offset: 0x000F3AAC
	public void RefreshScoreboards()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			if (!(this.scoreboards[i] == null))
			{
				this.scoreboards[i].Refresh(this.vrRigs);
				if (this.shiftManager != null)
				{
					if (this.shiftManager.ShiftActive)
					{
						this.scoreboards[i].total.text = "-AWAITING SHIFT END-";
					}
					else if (this.shiftManager.ShiftTotalEarned < 0)
					{
						this.scoreboards[i].total.text = "-SHIFT NOT ACTIVE-";
					}
					else
					{
						this.scoreboards[i].total.text = this.shiftManager.ShiftTotalEarned.ToString();
					}
				}
			}
		}
	}

	// Token: 0x06002D69 RID: 11625 RVA: 0x000F5990 File Offset: 0x000F3B90
	public int GetItemCost(int entityTypeId)
	{
		int num;
		if (!this.grManager.gameEntityManager.PriceLookup(entityTypeId, out num))
		{
			return 100;
		}
		return num;
	}

	// Token: 0x06002D6A RID: 11626 RVA: 0x000F59B8 File Offset: 0x000F3BB8
	public void UpdateRemoteScoreboardScreen(GRUIScoreboard.ScoreboardScreen scoreboardPage)
	{
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.zone);
		if (managerForZone != null && managerForZone.ghostReactorManager != null)
		{
			managerForZone.ghostReactorManager.photonView.RPC("BroadcastScoreboardPage", RpcTarget.Others, new object[] { scoreboardPage });
		}
	}

	// Token: 0x06002D6B RID: 11627 RVA: 0x000F5A10 File Offset: 0x000F3C10
	public void SetNextDelveDepth(int newLevel, int newDepthConfigIndex)
	{
		this.depthLevel = newLevel;
		this.depthLevel = Mathf.Clamp(this.depthLevel, 0, this.levelGenerator.depthConfigs.Count);
		if (this.depthLevel >= 0 && this.zone == GTZone.ghostReactorDrill && PhotonNetwork.InRoom && !NetworkSystem.Instance.SessionIsPrivate && this.grManager.IsAuthority())
		{
			int joinDepthSectionFromLevel = GhostReactor.GetJoinDepthSectionFromLevel(this.depthLevel);
			Hashtable hashtable = new Hashtable { 
			{
				"ghostReactorDepth",
				joinDepthSectionFromLevel.ToString()
			} };
			Debug.LogFormat("GR Room Param Set {0} {1}", new object[]
			{
				"ghostReactorDepth",
				hashtable["ghostReactorDepth"]
			});
			PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
		}
		this.depthConfigIndex = newDepthConfigIndex;
	}

	// Token: 0x06002D6C RID: 11628 RVA: 0x000F5ADB File Offset: 0x000F3CDB
	public static int GetJoinDepthSectionFromLevel(int depthLevel)
	{
		if (depthLevel < 4)
		{
			return 0;
		}
		if (depthLevel < 10)
		{
			return 1;
		}
		if (depthLevel < 15)
		{
			return 2;
		}
		if (depthLevel < 20)
		{
			return 3;
		}
		if (depthLevel < 25)
		{
			return 5;
		}
		return 6;
	}

	// Token: 0x06002D6D RID: 11629 RVA: 0x000F5B00 File Offset: 0x000F3D00
	public void DelveToNextDepth()
	{
		if (this.shiftManager != null)
		{
			this.shiftManager.authorizedToDelveDeeper = false;
		}
		this.RefreshDepth();
	}

	// Token: 0x06002D6E RID: 11630 RVA: 0x000F5B24 File Offset: 0x000F3D24
	public int PickLevelConfigForDepth(int depthLevel)
	{
		if (this.zone == GTZone.customMaps)
		{
			return 0;
		}
		GhostReactorLevelDepthConfig depthLevelConfig = this.GetDepthLevelConfig(depthLevel);
		int num = 0;
		for (int i = 0; i < depthLevelConfig.options.Count; i++)
		{
			num += depthLevelConfig.options[i].weight;
		}
		int num2 = Random.Range(0, num + 1);
		for (int j = 0; j < depthLevelConfig.options.Count; j++)
		{
			if (depthLevelConfig.options[j].weight >= num2)
			{
				return j;
			}
			num2 -= depthLevelConfig.options[j].weight;
		}
		return 0;
	}

	// Token: 0x06002D6F RID: 11631 RVA: 0x000F5BC3 File Offset: 0x000F3DC3
	public void RefreshDepth()
	{
		if (this.shiftManager != null)
		{
			this.shiftManager.RefreshDepthDisplay();
		}
		this.RefreshBays();
	}

	// Token: 0x06002D70 RID: 11632 RVA: 0x000F5BE4 File Offset: 0x000F3DE4
	public int GetDepthLevel()
	{
		return this.depthLevel;
	}

	// Token: 0x06002D71 RID: 11633 RVA: 0x000F5BEC File Offset: 0x000F3DEC
	public int GetDepthConfigIndex()
	{
		return this.depthConfigIndex;
	}

	// Token: 0x06002D72 RID: 11634 RVA: 0x000F5BF4 File Offset: 0x000F3DF4
	public GhostReactorLevelDepthConfig GetDepthLevelConfig(int level)
	{
		if (this.levelGenerator == null)
		{
			return null;
		}
		level = Mathf.Clamp(level, 0, this.levelGenerator.depthConfigs.Count - 1);
		return this.levelGenerator.depthConfigs[level];
	}

	// Token: 0x06002D73 RID: 11635 RVA: 0x000F5C34 File Offset: 0x000F3E34
	public GhostReactorLevelGenConfig GetCurrLevelGenConfig()
	{
		if (this.levelGenerator == null)
		{
			return null;
		}
		int num = this.GetDepthLevel();
		num = Mathf.Clamp(num, 0, this.levelGenerator.depthConfigs.Count - 1);
		this.depthConfigIndex = Mathf.Clamp(this.depthConfigIndex, 0, this.levelGenerator.depthConfigs[num].options.Count - 1);
		return this.levelGenerator.depthConfigs[num].options[this.depthConfigIndex].levelConfig;
	}

	// Token: 0x06002D74 RID: 11636 RVA: 0x000F5CC8 File Offset: 0x000F3EC8
	public void RefreshStore()
	{
		for (int i = 0; i < this.storeDisplays.Count; i++)
		{
			this.storeDisplays[i].Setup(PhotonNetwork.LocalPlayer.ActorNumber, this);
		}
	}

	// Token: 0x06002D75 RID: 11637 RVA: 0x000F5D08 File Offset: 0x000F3F08
	public void RefreshBays()
	{
		for (int i = 0; i < this.bays.Count; i++)
		{
			this.bays[i].Refresh();
		}
	}

	// Token: 0x06002D76 RID: 11638 RVA: 0x000F5D3C File Offset: 0x000F3F3C
	public void UpdateHandprints(float deltaTime)
	{
		int num = this.handPrintData.Count - 1000;
		if (num > 0)
		{
			this.handPrintData.RemoveRange(0, num);
			this.handPrintLocations.RemoveRange(0, num);
		}
		float time = Time.time;
		int i = this.handPrintData.Count - 1;
		while (i >= 0)
		{
			this.handPrintData[i] = this.handPrintData[i] - deltaTime;
			if (i + this.handPrintCombineTestDelta >= this.handPrintData.Count)
			{
				goto IL_013E;
			}
			if (this.handPrintData[i + this.handPrintCombineTestDelta] <= this.handPrintFadeTime - 3f)
			{
				Matrix4x4 matrix4x = this.handPrintLocations[i];
				Matrix4x4 matrix4x2 = this.handPrintLocations[i + this.handPrintCombineTestDelta];
				Vector3 vector = new Vector3(matrix4x.m03 - matrix4x2.m03, matrix4x.m13 - matrix4x2.m13, matrix4x.m23 - matrix4x2.m23);
				if (vector.sqrMagnitude < this.handPrintScale * this.handPrintScale)
				{
					List<float> list = this.handPrintData;
					int num2 = i;
					list[num2] -= deltaTime * (float)this.handPrintData.Count * 50f;
					goto IL_013E;
				}
				goto IL_013E;
			}
			IL_0169:
			i--;
			continue;
			IL_013E:
			if (this.handPrintData[i] < 0f)
			{
				this.handPrintData.RemoveAt(i);
				this.handPrintLocations.RemoveAt(i);
				goto IL_0169;
			}
			goto IL_0169;
		}
		if (this.handPrintData.Count > 0)
		{
			this.handPrintCombineTestDelta = (this.handPrintCombineTestDelta + 1) % this.handPrintData.Count;
			if (this.handPrintCombineTestDelta == 0)
			{
				this.handPrintCombineTestDelta = 1;
			}
		}
		else
		{
			this.handPrintCombineTestDelta = 1;
		}
		if (this.handPrintMaterial != null)
		{
			this.handPrintMaterial.SetFloat("_FadeDuration", this.handPrintFadeTime);
			this.handPrintMaterial.enableInstancing = true;
		}
		int num3 = Mathf.Min(Math.Min(1000, 1023), this.handPrintLocations.Count);
		if (num3 > 0)
		{
			this.handPrintMPB.Clear();
			this.handPrintMPB.SetFloatArray("_HandPrintData", this.handPrintData.GetRange(0, num3));
			this.handPrintMPB.SetFloat("_FadeDuration", this.handPrintFadeTime);
			RenderParams renderParams = new RenderParams(this.handPrintMaterial)
			{
				shadowCastingMode = ShadowCastingMode.Off,
				receiveShadows = false,
				layer = base.gameObject.layer,
				matProps = this.handPrintMPB,
				worldBounds = new Bounds(Vector3.zero, Vector3.one * 2000f)
			};
			Graphics.RenderMeshInstanced<Matrix4x4>(in renderParams, this.handPrintMesh, 0, this.handPrintLocations.GetRange(0, num3), -1, 0);
		}
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer != null)
		{
			if (Time.time - this.handPrintTimeLeft >= this.handPrintInkTime)
			{
				grplayer.SetGooParticleSystemEnabled(true, false);
			}
			if (Time.time - this.handPrintTimeRight >= this.handPrintInkTime)
			{
				grplayer.SetGooParticleSystemEnabled(false, false);
			}
		}
	}

	// Token: 0x06002D77 RID: 11639 RVA: 0x000F605C File Offset: 0x000F425C
	public void OnTapLocal(bool isLeftHand, Vector3 pos, Quaternion orient, GorillaSurfaceOverride surfaceOverride)
	{
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer == null)
		{
			return;
		}
		if (!(surfaceOverride != null) || surfaceOverride.overrideIndex != 79)
		{
			float num = (isLeftHand ? this.handPrintTimeLeft : this.handPrintTimeRight);
			if (Time.time - num < this.handPrintInkTime && (Time.time < this.lastBroadcastHandTapTime || Time.time > this.lastBroadcastHandTapTime + this.broadcastHandTapDelay))
			{
				GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.zone);
				if (managerForZone != null && managerForZone.ghostReactorManager != null)
				{
					managerForZone.ghostReactorManager.photonView.RPC("BroadcastHandprint", RpcTarget.All, new object[] { pos, orient });
				}
				this.lastBroadcastHandTapTime = Time.time;
			}
			return;
		}
		grplayer.SetGooParticleSystemEnabled(isLeftHand, true);
		if (isLeftHand)
		{
			this.handPrintTimeLeft = Time.time;
			return;
		}
		this.handPrintTimeRight = Time.time;
	}

	// Token: 0x06002D78 RID: 11640 RVA: 0x000F6154 File Offset: 0x000F4354
	public void AddHandprint(Vector3 pos, Quaternion orient)
	{
		Matrix4x4 matrix4x = default(Matrix4x4);
		matrix4x.SetTRS(pos, orient * Quaternion.Euler(90f, 0f, 180f), Vector3.one * this.handPrintScale);
		this.handPrintLocations.Add(matrix4x);
		this.handPrintData.Add(this.handPrintFadeTime);
	}

	// Token: 0x06002D79 RID: 11641 RVA: 0x000F61B8 File Offset: 0x000F43B8
	public void ClearAllHandprints()
	{
		this.handPrintData.Clear();
		this.handPrintLocations.Clear();
	}

	// Token: 0x1700046B RID: 1131
	// (get) Token: 0x06002D7A RID: 11642 RVA: 0x000F61D0 File Offset: 0x000F43D0
	public int NumActivePlayers
	{
		get
		{
			return this.vrRigs.Count;
		}
	}

	// Token: 0x06002D7B RID: 11643 RVA: 0x000F61E0 File Offset: 0x000F43E0
	public void OnAbilityDie(GameEntity entity, float forcedRespawn = -1f)
	{
		GhostReactor.EnemyEntityCreateData enemyEntityCreateData = GhostReactor.EnemyEntityCreateData.Unpack(entity.createData);
		if (enemyEntityCreateData.respawnCount == 0)
		{
			return;
		}
		if (this.grManager.GetBossEntity() != null)
		{
			GREnemyBossMoon component = this.grManager.GetBossEntity().GetComponent<GREnemyBossMoon>();
			if (component != null && component.BossHasRevealed)
			{
				return;
			}
		}
		GhostReactor.EntityTypeRespawnTracker entityTypeRespawnTracker = new GhostReactor.EntityTypeRespawnTracker();
		entityTypeRespawnTracker.entityTypeID = entity.typeId;
		entityTypeRespawnTracker.entityCreateData = enemyEntityCreateData.Pack();
		entityTypeRespawnTracker.entityNextRespawnTime = ((forcedRespawn < 0f) ? this.respawnTime : forcedRespawn);
		this.respawnQueue.Add(entityTypeRespawnTracker);
	}

	// Token: 0x06002D7C RID: 11644 RVA: 0x000F627A File Offset: 0x000F447A
	public void ClearAllRespawns()
	{
		this.respawnQueue.Clear();
	}

	// Token: 0x06002D7D RID: 11645 RVA: 0x00023F0C File Offset: 0x0002210C
	bool IBuildValidation.BuildValidationCheck()
	{
		return true;
	}

	// Token: 0x040039D8 RID: 14808
	public static GhostReactor instance;

	// Token: 0x040039D9 RID: 14809
	public GTZone zone;

	// Token: 0x040039DA RID: 14810
	public Transform restartMarker;

	// Token: 0x040039DB RID: 14811
	public PhotonView photonView;

	// Token: 0x040039DC RID: 14812
	public AudioSource entryRoomAudio;

	// Token: 0x040039DD RID: 14813
	public AudioClip entryRoomDeathSound;

	// Token: 0x040039DE RID: 14814
	[FormerlySerializedAs("zoneLimit")]
	public BoxCollider boundsBoxCollider;

	// Token: 0x040039DF RID: 14815
	public BoxCollider safeZoneLimit;

	// Token: 0x040039E0 RID: 14816
	public List<GhostReactor.TempEnemySpawnInfo> tempSpawnEnemies;

	// Token: 0x040039E1 RID: 14817
	public GameEntity overrideEnemySpawn;

	// Token: 0x040039E2 RID: 14818
	public List<GameEntity> tempSpawnItems;

	// Token: 0x040039E3 RID: 14819
	public Transform tempSpawnItemsMarker;

	// Token: 0x040039E4 RID: 14820
	public List<GRUIBuyItem> itemPurchaseStands;

	// Token: 0x040039E5 RID: 14821
	public List<GRToolPurchaseStation> toolPurchasingStations;

	// Token: 0x040039E6 RID: 14822
	public GRDebugUpgradeKiosk debugUpgradeKiosk;

	// Token: 0x040039E7 RID: 14823
	public List<GRUIScoreboard> scoreboards;

	// Token: 0x040039E8 RID: 14824
	public List<GRCollectibleDispenser> collectibleDispensers = new List<GRCollectibleDispenser>();

	// Token: 0x040039E9 RID: 14825
	public List<IGRSleepableEntity> sleepableEntities = new List<IGRSleepableEntity>();

	// Token: 0x040039EA RID: 14826
	private List<GRBay> bays;

	// Token: 0x040039EB RID: 14827
	private List<GRUIStoreDisplay> storeDisplays;

	// Token: 0x040039EC RID: 14828
	public GRUIStationEmployeeBadges employeeBadges;

	// Token: 0x040039ED RID: 14829
	public GRUIEmployeeTerminal employeeTerminal;

	// Token: 0x040039EE RID: 14830
	public GhostReactorShiftManager shiftManager;

	// Token: 0x040039EF RID: 14831
	public GhostReactorLevelGenerator levelGenerator;

	// Token: 0x040039F0 RID: 14832
	public GRCurrencyDepositor currencyDepositor;

	// Token: 0x040039F1 RID: 14833
	public GRSeedExtractor seedExtractor;

	// Token: 0x040039F2 RID: 14834
	public GRDistillery distillery;

	// Token: 0x040039F3 RID: 14835
	public GRToolProgressionManager toolProgression;

	// Token: 0x040039F4 RID: 14836
	public GRToolUpgradeStation upgradeStation;

	// Token: 0x040039F5 RID: 14837
	public List<GRToolUpgradePurchaseStationFull> toolUpgradePurchaseStationsFull;

	// Token: 0x040039F6 RID: 14838
	public GRRecycler recycler;

	// Token: 0x040039F7 RID: 14839
	public List<GhostReactor.EntityTypeRespawnTracker> respawnQueue = new List<GhostReactor.EntityTypeRespawnTracker>();

	// Token: 0x040039F8 RID: 14840
	public List<float> difficultyScalingPerPlayer = new List<float>(10);

	// Token: 0x040039F9 RID: 14841
	public float respawnTime = 10f;

	// Token: 0x040039FA RID: 14842
	public float respawnMinDistToPlayer = 8f;

	// Token: 0x040039FB RID: 14843
	public float difficultyScalingForCurrentFloor = 1f;

	// Token: 0x040039FC RID: 14844
	public LayerMask envLayerMask;

	// Token: 0x040039FD RID: 14845
	public Material handPrintMaterial;

	// Token: 0x040039FE RID: 14846
	public Mesh handPrintMesh;

	// Token: 0x040039FF RID: 14847
	public float handPrintScale;

	// Token: 0x04003A00 RID: 14848
	public float handPrintInkTime = 30f;

	// Token: 0x04003A01 RID: 14849
	public float handPrintFadeTime = 600f;

	// Token: 0x04003A02 RID: 14850
	private const int handPrintMaxCount = 1000;

	// Token: 0x04003A03 RID: 14851
	private List<Matrix4x4> handPrintLocations = new List<Matrix4x4>(1000);

	// Token: 0x04003A04 RID: 14852
	private List<float> handPrintData = new List<float>(1000);

	// Token: 0x04003A05 RID: 14853
	private MaterialPropertyBlock handPrintMPB;

	// Token: 0x04003A06 RID: 14854
	[ReadOnly]
	public List<GRReviveStation> reviveStations;

	// Token: 0x04003A07 RID: 14855
	public List<GRVendingMachine> vendingMachines;

	// Token: 0x04003A08 RID: 14856
	public List<VRRig> vrRigs;

	// Token: 0x04003A09 RID: 14857
	private float collectibleDispenserUpdateFrequency = 3f;

	// Token: 0x04003A0A RID: 14858
	private double lastCollectibleDispenserUpdateTime = -10.0;

	// Token: 0x04003A0B RID: 14859
	private int sentientCoreUpdateIndex;

	// Token: 0x04003A0C RID: 14860
	private SRand randomGenerator;

	// Token: 0x04003A0D RID: 14861
	[ReadOnly]
	public int depthLevel;

	// Token: 0x04003A0E RID: 14862
	[ReadOnly]
	public int depthConfigIndex;

	// Token: 0x04003A0F RID: 14863
	public Dictionary<int, double> playerProgressionData;

	// Token: 0x04003A10 RID: 14864
	public GRDropZone dropZone;

	// Token: 0x04003A11 RID: 14865
	public static float DROP_ZONE_REPEL = 2.25f;

	// Token: 0x04003A12 RID: 14866
	public ZoneShaderSettings zoneShaderSettings;

	// Token: 0x04003A13 RID: 14867
	public GRUIPromotionBot promotionBot;

	// Token: 0x04003A14 RID: 14868
	private bool isRefreshing;

	// Token: 0x04003A15 RID: 14869
	public GhostReactorManager grManager;

	// Token: 0x04003A16 RID: 14870
	private float handPrintTimeLeft = -1000f;

	// Token: 0x04003A17 RID: 14871
	private float handPrintTimeRight = -1000f;

	// Token: 0x04003A18 RID: 14872
	private int handPrintCombineTestDelta = 1;

	// Token: 0x04003A19 RID: 14873
	private float lastBroadcastHandTapTime;

	// Token: 0x04003A1A RID: 14874
	private float broadcastHandTapDelay = 0.3f;

	// Token: 0x02000706 RID: 1798
	[Serializable]
	public class TempEnemySpawnInfo
	{
		// Token: 0x04003A1B RID: 14875
		public GameEntity prefab;

		// Token: 0x04003A1C RID: 14876
		public Transform spawnMarker;

		// Token: 0x04003A1D RID: 14877
		public int patrolPath;
	}

	// Token: 0x02000707 RID: 1799
	public class EntityTypeRespawnTracker
	{
		// Token: 0x04003A1E RID: 14878
		public int entityTypeID;

		// Token: 0x04003A1F RID: 14879
		public long entityCreateData;

		// Token: 0x04003A20 RID: 14880
		public float entityNextRespawnTime;
	}

	// Token: 0x02000708 RID: 1800
	public enum EntityGroupTypes
	{
		// Token: 0x04003A22 RID: 14882
		EnemyChaser,
		// Token: 0x04003A23 RID: 14883
		EnemyChaserArmored,
		// Token: 0x04003A24 RID: 14884
		EnemyRanged,
		// Token: 0x04003A25 RID: 14885
		EnemyRangedArmored,
		// Token: 0x04003A26 RID: 14886
		CollectibleFlower,
		// Token: 0x04003A27 RID: 14887
		BarrierEnergyCostGate,
		// Token: 0x04003A28 RID: 14888
		BarrierSpectralWall,
		// Token: 0x04003A29 RID: 14889
		HazardSpectralLiquid
	}

	// Token: 0x02000709 RID: 1801
	public enum EnemyType
	{
		// Token: 0x04003A2B RID: 14891
		Chaser,
		// Token: 0x04003A2C RID: 14892
		Ranged,
		// Token: 0x04003A2D RID: 14893
		Phantom,
		// Token: 0x04003A2E RID: 14894
		Environment,
		// Token: 0x04003A2F RID: 14895
		CustomMapsEnemy
	}

	// Token: 0x0200070A RID: 1802
	public struct EnemyEntityCreateData
	{
		// Token: 0x06002D82 RID: 11650 RVA: 0x000F636E File Offset: 0x000F456E
		private static long PackData(int value, int nbits, int shift)
		{
			return ((long)value & (long)((1 << nbits) - 1)) << shift;
		}

		// Token: 0x06002D83 RID: 11651 RVA: 0x000F6381 File Offset: 0x000F4581
		private static int UnpackData(long createData, int nbits, int shift)
		{
			return (int)((createData >> shift) & (long)((1 << nbits) - 1));
		}

		// Token: 0x06002D84 RID: 11652 RVA: 0x000F6394 File Offset: 0x000F4594
		public static GhostReactor.EnemyEntityCreateData Unpack(long bits)
		{
			return new GhostReactor.EnemyEntityCreateData
			{
				respawnCount = GhostReactor.EnemyEntityCreateData.UnpackData(bits, 8, 16),
				sectionIndex = GhostReactor.EnemyEntityCreateData.UnpackData(bits, 8, 8),
				patrolIndex = GhostReactor.EnemyEntityCreateData.UnpackData(bits, 8, 0)
			};
		}

		// Token: 0x06002D85 RID: 11653 RVA: 0x000F63D8 File Offset: 0x000F45D8
		public long Pack()
		{
			return GhostReactor.EnemyEntityCreateData.PackData(this.respawnCount, 8, 16) | GhostReactor.EnemyEntityCreateData.PackData(this.sectionIndex, 8, 8) | GhostReactor.EnemyEntityCreateData.PackData(this.patrolIndex, 8, 0);
		}

		// Token: 0x04003A30 RID: 14896
		public int respawnCount;

		// Token: 0x04003A31 RID: 14897
		public int sectionIndex;

		// Token: 0x04003A32 RID: 14898
		public int patrolIndex;
	}

	// Token: 0x0200070B RID: 1803
	public struct ToolEntityCreateData
	{
		// Token: 0x06002D86 RID: 11654 RVA: 0x000F636E File Offset: 0x000F456E
		private static long PackData(int value, int nbits, int shift)
		{
			return ((long)value & (long)((1 << nbits) - 1)) << shift;
		}

		// Token: 0x06002D87 RID: 11655 RVA: 0x000F6381 File Offset: 0x000F4581
		private static int UnpackData(long createData, int nbits, int shift)
		{
			return (int)((createData >> shift) & (long)((1 << nbits) - 1));
		}

		// Token: 0x06002D88 RID: 11656 RVA: 0x000F6404 File Offset: 0x000F4604
		public static GhostReactor.ToolEntityCreateData Unpack(long bits)
		{
			GhostReactor.ToolEntityCreateData toolEntityCreateData = default(GhostReactor.ToolEntityCreateData);
			toolEntityCreateData.stationIndex = GhostReactor.ToolEntityCreateData.UnpackData(bits, 8, 0) - 1;
			int num = GhostReactor.ToolEntityCreateData.UnpackData(bits, 8, 8);
			toolEntityCreateData.decayTime = 5f * (float)num;
			return toolEntityCreateData;
		}

		// Token: 0x06002D89 RID: 11657 RVA: 0x000F6443 File Offset: 0x000F4643
		public long Pack()
		{
			long num = GhostReactor.ToolEntityCreateData.PackData(this.stationIndex + 1, 8, 0);
			GhostReactor.ToolEntityCreateData.PackData((int)(this.decayTime / 5f), 8, 8);
			return num;
		}

		// Token: 0x04003A33 RID: 14899
		public int stationIndex;

		// Token: 0x04003A34 RID: 14900
		public float decayTime;
	}
}
