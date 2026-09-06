using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200071A RID: 1818
[NetworkBehaviourWeaved(0)]
public class GhostReactorManager : NetworkComponent, IGameEntityZoneComponent
{
	// Token: 0x06002DC4 RID: 11716 RVA: 0x000F8BC8 File Offset: 0x000F6DC8
	protected override void Awake()
	{
		base.Awake();
		this.noiseEventManager = base.GetComponent<GRNoiseEventManager>();
	}

	// Token: 0x06002DC5 RID: 11717 RVA: 0x000F8BDC File Offset: 0x000F6DDC
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
	}

	// Token: 0x06002DC6 RID: 11718 RVA: 0x0008E57B File Offset: 0x0008C77B
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
	}

	// Token: 0x06002DC7 RID: 11719 RVA: 0x000F8BEA File Offset: 0x000F6DEA
	public bool IsAuthority()
	{
		return this.gameEntityManager.IsAuthority();
	}

	// Token: 0x06002DC8 RID: 11720 RVA: 0x000F8BF7 File Offset: 0x000F6DF7
	private bool IsAuthorityPlayer(NetPlayer player)
	{
		return this.gameEntityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x06002DC9 RID: 11721 RVA: 0x000F8C05 File Offset: 0x000F6E05
	private bool IsAuthorityPlayer(Player player)
	{
		return this.gameEntityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x06002DCA RID: 11722 RVA: 0x000F8C13 File Offset: 0x000F6E13
	private Player GetAuthorityPlayer()
	{
		return this.gameEntityManager.GetAuthorityPlayer();
	}

	// Token: 0x06002DCB RID: 11723 RVA: 0x000F8C20 File Offset: 0x000F6E20
	public bool IsZoneActive()
	{
		return this.gameEntityManager.IsZoneActive();
	}

	// Token: 0x06002DCC RID: 11724 RVA: 0x000F8C2D File Offset: 0x000F6E2D
	public bool IsPositionInZone(Vector3 pos)
	{
		return this.gameEntityManager.IsPositionInManagerBounds(pos);
	}

	// Token: 0x06002DCD RID: 11725 RVA: 0x000F8C3B File Offset: 0x000F6E3B
	public bool IsValidClientRPC(Player sender)
	{
		return this.gameEntityManager.IsValidClientRPC(sender);
	}

	// Token: 0x06002DCE RID: 11726 RVA: 0x000F8C49 File Offset: 0x000F6E49
	public bool IsValidClientRPC(Player sender, int entityNetId)
	{
		return this.gameEntityManager.IsValidClientRPC(sender, entityNetId);
	}

	// Token: 0x06002DCF RID: 11727 RVA: 0x000F8C58 File Offset: 0x000F6E58
	public bool IsValidClientRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.gameEntityManager.IsValidClientRPC(sender, entityNetId, pos);
	}

	// Token: 0x06002DD0 RID: 11728 RVA: 0x000F8C68 File Offset: 0x000F6E68
	public bool IsValidClientRPC(Player sender, Vector3 pos)
	{
		return this.gameEntityManager.IsValidClientRPC(sender, pos);
	}

	// Token: 0x06002DD1 RID: 11729 RVA: 0x000F8C77 File Offset: 0x000F6E77
	public bool IsValidAuthorityRPC(Player sender)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(sender);
	}

	// Token: 0x06002DD2 RID: 11730 RVA: 0x000F8C85 File Offset: 0x000F6E85
	public bool IsValidAuthorityRPC(Player sender, int entityNetId)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(sender, entityNetId);
	}

	// Token: 0x06002DD3 RID: 11731 RVA: 0x000F8C94 File Offset: 0x000F6E94
	public bool IsValidAuthorityRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(sender, entityNetId, pos);
	}

	// Token: 0x06002DD4 RID: 11732 RVA: 0x000F8CA4 File Offset: 0x000F6EA4
	public bool IsValidAuthorityRPC(Player sender, Vector3 pos)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(sender, pos);
	}

	// Token: 0x06002DD5 RID: 11733 RVA: 0x000F8CB3 File Offset: 0x000F6EB3
	public static GhostReactorManager Get(GameEntity gameEntity)
	{
		if (gameEntity == null || gameEntity.manager == null)
		{
			return null;
		}
		return gameEntity.manager.ghostReactorManager;
	}

	// Token: 0x06002DD6 RID: 11734 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void RefreshShiftCredit()
	{
	}

	// Token: 0x06002DD7 RID: 11735 RVA: 0x000F8CDC File Offset: 0x000F6EDC
	[PunRPC]
	public void RefreshShiftCreditRPC(PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		if (this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.RefreshShiftCredit))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull())
		{
			return;
		}
		if (grplayer.mothershipId.IsNullOrEmpty())
		{
			return;
		}
		ProgressionManager.Instance.GetShiftCredit(grplayer.mothershipId);
	}

	// Token: 0x06002DD8 RID: 11736 RVA: 0x000F8D40 File Offset: 0x000F6F40
	public void SendMothershipId()
	{
		string mothershipId = MothershipClientContext.MothershipId;
	}

	// Token: 0x06002DD9 RID: 11737 RVA: 0x000F8D48 File Offset: 0x000F6F48
	[PunRPC]
	public void SendMothershipIdRPC(string mothershipId, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		if (this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.SendMothershipId))
		{
			return;
		}
		if (mothershipId.Length > 40)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull())
		{
			return;
		}
		if (!grplayer.mothershipId.IsNullOrEmpty())
		{
			return;
		}
		if (grplayer.mothershipId.IsNullOrEmpty())
		{
			grplayer.mothershipId = mothershipId.Trim();
			ProgressionManager.Instance.GetShiftCredit(grplayer.mothershipId);
		}
	}

	// Token: 0x06002DDA RID: 11738 RVA: 0x000F8DD0 File Offset: 0x000F6FD0
	public void RequestCollectItem(GameEntityId collectibleEntityId, GameEntityId collectorEntityId)
	{
		this.photonView.RPC("RequestCollectItemRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.gameEntityManager.GetNetIdFromEntityId(collectibleEntityId),
			this.gameEntityManager.GetNetIdFromEntityId(collectorEntityId)
		});
	}

	// Token: 0x06002DDB RID: 11739 RVA: 0x000F8E24 File Offset: 0x000F7024
	public void RequestDepositCollectible(GameEntityId collectibleEntityId)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(collectibleEntityId);
		if (gameEntity != null)
		{
			this.photonView.RPC("ApplyCollectItemRPC", RpcTarget.All, new object[]
			{
				this.gameEntityManager.GetNetIdFromEntityId(collectibleEntityId),
				-1,
				gameEntity.lastHeldByActorNumber
			});
		}
	}

	// Token: 0x06002DDC RID: 11740 RVA: 0x000F8E94 File Offset: 0x000F7094
	[PunRPC]
	public void RequestCollectItemRPC(int collectibleEntityNetId, int collectorEntityNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender, collectibleEntityNetId))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestCollectItemLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (!this.gameEntityManager.IsValidNetId(collectorEntityNetId) || !this.gameEntityManager.IsEntityNearEntity(collectibleEntityNetId, collectorEntityNetId, 16f))
		{
			return;
		}
		if (true)
		{
			this.photonView.RPC("ApplyCollectItemRPC", RpcTarget.All, new object[]
			{
				collectibleEntityNetId,
				collectorEntityNetId,
				info.Sender.ActorNumber
			});
		}
	}

	// Token: 0x06002DDD RID: 11741 RVA: 0x000F8F40 File Offset: 0x000F7140
	[PunRPC]
	public void ApplyCollectItemRPC(int collectibleEntityNetId, int collectorEntityNetId, int collectingPlayerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, collectibleEntityNetId) || this.reactor == null || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyCollectItem))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(collectingPlayerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (true)
		{
			GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectibleEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityIdFromNetId);
			if (gameEntity == null)
			{
				return;
			}
			GRCollectible component = gameEntity.GetComponent<GRCollectible>();
			if (component == null)
			{
				return;
			}
			GameEntityId entityIdFromNetId2 = this.gameEntityManager.GetEntityIdFromNetId(collectorEntityNetId);
			GameEntity gameEntity2 = this.gameEntityManager.GetGameEntity(entityIdFromNetId2);
			if (gameEntity2 != null)
			{
				GRToolCollector component2 = gameEntity2.GetComponent<GRToolCollector>();
				if (component2 != null && component2.tool != null)
				{
					component2.PerformCollection(component);
				}
			}
			else
			{
				ProgressionManager.Instance.DepositCore(component.type);
				this.ReportCoreCollection(grplayer, component.type);
				int count = this.reactor.vrRigs.Count;
				int num = component.energyValue / 4;
				for (int i = 0; i < count; i++)
				{
					GRPlayer.Get(this.reactor.vrRigs[i]).IncrementCoresCollectedGroup(num);
				}
				grplayer.IncrementCoresCollectedPlayer(num);
			}
			if (gameEntity != null && component != null)
			{
				component.InvokeOnCollected();
			}
			this.gameEntityManager.DestroyItemLocal(entityIdFromNetId);
		}
	}

	// Token: 0x06002DDE RID: 11742 RVA: 0x000F90B0 File Offset: 0x000F72B0
	public void RequestApplySeedExtractorState(int coreCount, int coresProcessedByOverdrive, int researchPoints, float coreProcessingPercentage, float overdriveSupply)
	{
		this.photonView.RPC("RequestApplySeedExtractorStateRPC", this.GetAuthorityPlayer(), new object[] { coreCount, coresProcessedByOverdrive, researchPoints, coreProcessingPercentage, overdriveSupply });
	}

	// Token: 0x06002DDF RID: 11743 RVA: 0x000F9108 File Offset: 0x000F7308
	[PunRPC]
	public void RequestApplySeedExtractorStateRPC(int coreCount, int coresProcessedByOverdrive, int researchPoints, float coreProcessingPercentage, float overdriveSupply, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.SeedExtractorAction) || coreCount < 0 || coresProcessedByOverdrive < 0 || researchPoints < 0 || !float.IsFinite(coreProcessingPercentage) || !float.IsFinite(overdriveSupply))
		{
			return;
		}
		if (info.Sender.ActorNumber != this.reactor.seedExtractor.CurrentPlayerActorNumber)
		{
			return;
		}
		this.photonView.RPC("ApplySeedExtractorStateRPC", RpcTarget.All, new object[]
		{
			info.Sender.ActorNumber,
			coreCount,
			coresProcessedByOverdrive,
			researchPoints,
			coreProcessingPercentage,
			overdriveSupply
		});
	}

	// Token: 0x06002DE0 RID: 11744 RVA: 0x000F91CC File Offset: 0x000F73CC
	[PunRPC]
	public void ApplySeedExtractorStateRPC(int playerActorNumber, int coreCount, int coresProcessedByOverdrive, int researchPoints, float coreProcessingPercentage, float overdriveSupply, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.SeedExtractorAction) || coreCount < 0 || coresProcessedByOverdrive < 0 || researchPoints < 0 || !float.IsFinite(coreProcessingPercentage) || !float.IsFinite(overdriveSupply))
		{
			return;
		}
		if (this.reactor != null && this.reactor.seedExtractor != null)
		{
			this.reactor.seedExtractor.ApplyState(playerActorNumber, coreCount, coresProcessedByOverdrive, researchPoints, coreProcessingPercentage, overdriveSupply);
		}
	}

	// Token: 0x06002DE1 RID: 11745 RVA: 0x000F9254 File Offset: 0x000F7454
	public void RequestDistillCollectible(GameEntityId collectibleEntityId, Player sender)
	{
		if (!this.IsValidAuthorityRPC(sender))
		{
			return;
		}
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(collectibleEntityId);
		if (gameEntity != null)
		{
			this.photonView.RPC("DistillItemRPC", RpcTarget.All, new object[]
			{
				this.gameEntityManager.GetNetIdFromEntityId(collectibleEntityId),
				gameEntity.lastHeldByActorNumber
			});
		}
	}

	// Token: 0x06002DE2 RID: 11746 RVA: 0x000F92BC File Offset: 0x000F74BC
	[PunRPC]
	public void DistillItemRPC(int collectibleEntityNetId, int collectingPlayerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, collectibleEntityNetId) || this.reactor == null || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.DistillItem))
		{
			return;
		}
		if (GRPlayer.Get(collectingPlayerActorNumber) == null)
		{
			return;
		}
		if (true)
		{
			GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectibleEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityIdFromNetId);
			if (gameEntity == null)
			{
				return;
			}
			GRCollectible component = gameEntity.GetComponent<GRCollectible>();
			if (component == null)
			{
				return;
			}
			Debug.LogWarning("Warning - NOT IMPLEMENTED - Return validating inserting core for distillery.");
			if (gameEntity != null && component != null)
			{
				component.InvokeOnCollected();
			}
			this.gameEntityManager.DestroyItemLocal(entityIdFromNetId);
		}
	}

	// Token: 0x06002DE3 RID: 11747 RVA: 0x000F936C File Offset: 0x000F756C
	public void RequestChargeTool(GameEntityId collectorEntityId, GameEntityId targetToolId, int targetEnergyDelta = 0, bool useCollectorEnergy = true)
	{
		this.photonView.RPC("RequestChargeToolRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.gameEntityManager.GetNetIdFromEntityId(collectorEntityId),
			this.gameEntityManager.GetNetIdFromEntityId(targetToolId),
			targetEnergyDelta,
			useCollectorEnergy
		});
	}

	// Token: 0x06002DE4 RID: 11748 RVA: 0x000F93D0 File Offset: 0x000F75D0
	[PunRPC]
	public void RequestChargeToolRPC(int collectorEntityNetId, int targetToolNetId, int targetEnergyDelta, bool useCollectorEnergy, PhotonMessageInfo info)
	{
		GamePlayer gamePlayer;
		if (!this.IsValidAuthorityRPC(info.Sender) || !this.gameEntityManager.IsValidNetId(collectorEntityNetId) || !this.gameEntityManager.IsValidNetId(targetToolNetId) || !this.gameEntityManager.IsEntityNearEntity(collectorEntityNetId, targetToolNetId, 16f) || !GamePlayer.TryGetGamePlayer(info.Sender.ActorNumber, out gamePlayer) || !this.gameEntityManager.IsPlayerHandNearEntity(gamePlayer, collectorEntityNetId, false, true, 16f) || !this.gameEntityManager.IsPlayerHandNearEntity(gamePlayer, targetToolNetId, false, true, 16f))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestChargeToolLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (true)
		{
			this.photonView.RPC("ApplyChargeToolRPC", RpcTarget.All, new object[] { collectorEntityNetId, targetToolNetId, targetEnergyDelta, useCollectorEnergy, info.Sender });
		}
	}

	// Token: 0x06002DE5 RID: 11749 RVA: 0x000F94D4 File Offset: 0x000F76D4
	[PunRPC]
	public void ApplyChargeToolRPC(int collectorEntityNetId, int targetToolNetId, int targetEnergyDelta, bool useCollectorEnergy, Player collectingPlayer, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyChargeTool) || !this.gameEntityManager.IsValidNetId(collectorEntityNetId) || !this.gameEntityManager.IsValidNetId(targetToolNetId))
		{
			return;
		}
		if (true)
		{
			GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectorEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityIdFromNetId);
			GameEntityId entityIdFromNetId2 = this.gameEntityManager.GetEntityIdFromNetId(targetToolNetId);
			GameEntity gameEntity2 = this.gameEntityManager.GetGameEntity(entityIdFromNetId2);
			if (gameEntity != null && gameEntity2 != null)
			{
				GRToolCollector component = gameEntity.GetComponent<GRToolCollector>();
				GRTool component2 = gameEntity2.GetComponent<GRTool>();
				if (component != null && component.tool != null && component2 != null)
				{
					int num = ((targetEnergyDelta != 0) ? targetEnergyDelta : 100);
					int num2 = Mathf.Max(component2.GetEnergyMax() - component2.energy, 0);
					int num3;
					if (!useCollectorEnergy)
					{
						num3 = Mathf.Min(num, num2);
						Debug.Log(string.Format("Apply SelfCharge {0}", num3));
					}
					else
					{
						num3 = Mathf.Min(Mathf.Min(component.tool.energy, num), num2);
					}
					if (num3 > 0)
					{
						if (useCollectorEnergy)
						{
							component.tool.SetEnergy(component.tool.energy - num3);
						}
						component2.RefillEnergy(num3, entityIdFromNetId);
						component.PlayChargeEffect(component2);
					}
				}
			}
		}
	}

	// Token: 0x06002DE6 RID: 11750 RVA: 0x000F9647 File Offset: 0x000F7847
	public void RequestDepositCurrency(GameEntityId collectorEntityId)
	{
		this.photonView.RPC("RequestDepositCurrencyRPC", this.GetAuthorityPlayer(), new object[] { this.gameEntityManager.GetNetIdFromEntityId(collectorEntityId) });
	}

	// Token: 0x06002DE7 RID: 11751 RVA: 0x000F967C File Offset: 0x000F787C
	[PunRPC]
	public void RequestDepositCurrencyRPC(int collectorEntityNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender, collectorEntityNetId))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestDepositCurrencyLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectorEntityNetId);
		this.gameEntityManager.GetGameEntity(entityIdFromNetId);
		GamePlayer gamePlayer;
		if (GamePlayer.TryGetGamePlayer(info.Sender.ActorNumber, out gamePlayer) && this.gameEntityManager.IsPlayerHandNearEntity(gamePlayer, collectorEntityNetId, false, true, 16f) && (grplayer.transform.position - this.reactor.currencyDepositor.transform.position).magnitude < 16f)
		{
			this.photonView.RPC("ApplyDepositCurrencyRPC", RpcTarget.All, new object[]
			{
				collectorEntityNetId,
				info.Sender.ActorNumber
			});
		}
	}

	// Token: 0x06002DE8 RID: 11752 RVA: 0x000F9778 File Offset: 0x000F7978
	[PunRPC]
	public void ApplyDepositCurrencyRPC(int collectorEntityNetId, int targetPlayerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, collectorEntityNetId) || this.reactor == null || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyDepositCurrency))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(targetPlayerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (true)
		{
			GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectorEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityIdFromNetId);
			if (gameEntity != null)
			{
				GRToolCollector component = gameEntity.GetComponent<GRToolCollector>();
				if (component != null && component.tool != null)
				{
					int energy = component.tool.energy;
					int energyDepositPerUse = component.energyDepositPerUse;
					if (energy >= energyDepositPerUse)
					{
						this.ReportCoreCollection(grplayer, ProgressionManager.CoreType.Core);
						int count = this.reactor.vrRigs.Count;
						int num = energyDepositPerUse / 4;
						for (int i = 0; i < count; i++)
						{
							GRPlayer.Get(this.reactor.vrRigs[i]).IncrementCoresCollectedGroup(num);
						}
						grplayer.IncrementCoresCollectedPlayer(num);
						int num2 = energy - energyDepositPerUse;
						component.tool.SetEnergy(num2);
						this.reactor.RefreshScoreboards();
						ProgressionManager.Instance.DepositCore(ProgressionManager.CoreType.Core);
						component.PlayChargeEffect(this.reactor.currencyDepositor);
					}
				}
			}
		}
	}

	// Token: 0x06002DE9 RID: 11753 RVA: 0x000F98C4 File Offset: 0x000F7AC4
	public void RequestEnemyHitPlayer(GhostReactor.EnemyType type, GameEntityId hitByEntityId, GRPlayer player, Vector3 hitPosition)
	{
		this.photonView.RPC("ApplyEnemyHitPlayerRPC", RpcTarget.All, new object[]
		{
			type,
			this.gameEntityManager.GetNetIdFromEntityId(hitByEntityId),
			hitPosition,
			Vector3.zero
		});
	}

	// Token: 0x06002DEA RID: 11754 RVA: 0x000F991C File Offset: 0x000F7B1C
	public void RequestEnemyHitPlayer(GhostReactor.EnemyType type, GameEntityId hitByEntityId, GRPlayer player, Vector3 hitPosition, Vector3 hitImpulse)
	{
		this.photonView.RPC("ApplyEnemyHitPlayerRPC", RpcTarget.All, new object[]
		{
			type,
			this.gameEntityManager.GetNetIdFromEntityId(hitByEntityId),
			hitPosition,
			hitImpulse
		});
	}

	// Token: 0x06002DEB RID: 11755 RVA: 0x000F9974 File Offset: 0x000F7B74
	[PunRPC]
	private void ApplyEnemyHitPlayerRPC(GhostReactor.EnemyType type, int entityNetId, Vector3 hitPosition, Vector3 hitImpulse, PhotonMessageInfo info)
	{
		if (!this.gameEntityManager.IsValidNetId(entityNetId))
		{
			return;
		}
		float num = 10000f;
		if ((in hitPosition).IsValid(in num))
		{
			float num2 = 10000f;
			if ((in hitImpulse).IsValid(in num2))
			{
				if (hitImpulse.magnitude > 50f)
				{
					return;
				}
				GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(entityNetId);
				GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
				if (grplayer == null || !grplayer.applyEnemyHitLimiter.CheckCallTime(Time.unscaledTime))
				{
					return;
				}
				this.OnEnemyHitPlayerInternal(type, entityIdFromNetId, grplayer, hitPosition, hitImpulse);
				return;
			}
		}
	}

	// Token: 0x06002DEC RID: 11756 RVA: 0x000F9A0C File Offset: 0x000F7C0C
	private void OnEnemyHitPlayerInternal(GhostReactor.EnemyType type, GameEntityId entityId, GRPlayer player, Vector3 hitPosition, Vector3 hitImpulse)
	{
		if (type == GhostReactor.EnemyType.Chaser || type == GhostReactor.EnemyType.Phantom || type == GhostReactor.EnemyType.Ranged || type == GhostReactor.EnemyType.CustomMapsEnemy)
		{
			player.OnPlayerHit(hitPosition, hitImpulse, this, entityId);
			GameHitter component = this.gameEntityManager.GetGameEntity(entityId).GetComponent<GameHitter>();
			if (component != null)
			{
				component.ApplyHitToPlayer(player, hitPosition);
			}
		}
	}

	// Token: 0x06002DED RID: 11757 RVA: 0x000F9A58 File Offset: 0x000F7C58
	public void ReportLocalPlayerHit()
	{
		base.GetView.RPC("ReportLocalPlayerHitRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06002DEE RID: 11758 RVA: 0x000F9A70 File Offset: 0x000F7C70
	[PunRPC]
	private void ReportLocalPlayerHitRPC(PhotonMessageInfo info)
	{
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || !grplayer.reportLocalHitLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		grplayer.ChangePlayerState(GRPlayer.GRPlayerState.Ghost, this);
	}

	// Token: 0x06002DEF RID: 11759 RVA: 0x000F9AB4 File Offset: 0x000F7CB4
	public void RequestPlayerRevive(GRReviveStation reviveStation, GRPlayer player)
	{
		if ((NetworkSystem.Instance.InRoom && this.IsAuthority()) || !NetworkSystem.Instance.InRoom)
		{
			base.GetView.RPC("ApplyPlayerRevivedRPC", RpcTarget.All, new object[]
			{
				reviveStation.Index,
				player.gamePlayer.rig.OwningNetPlayer.ActorNumber
			});
		}
	}

	// Token: 0x06002DF0 RID: 11760 RVA: 0x000F9B24 File Offset: 0x000F7D24
	[PunRPC]
	private void ApplyPlayerRevivedRPC(int reviveStationIndex, int playerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyPlayerRevived))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(playerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (reviveStationIndex < 0 || reviveStationIndex >= this.reactor.reviveStations.Count)
		{
			return;
		}
		GRReviveStation grreviveStation = this.reactor.reviveStations[reviveStationIndex];
		if (grreviveStation == null)
		{
			return;
		}
		grreviveStation.RevivePlayer(grplayer);
	}

	// Token: 0x06002DF1 RID: 11761 RVA: 0x000F9B9C File Offset: 0x000F7D9C
	public void RequestPlayerStateChange(GRPlayer player, GRPlayer.GRPlayerState newState)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			base.GetView.RPC("PlayerStateChangeRPC", RpcTarget.All, new object[]
			{
				PhotonNetwork.LocalPlayer.ActorNumber,
				player.gamePlayer.rig.OwningNetPlayer.ActorNumber,
				(int)newState
			});
			return;
		}
		player.ChangePlayerState(newState, this);
	}

	// Token: 0x06002DF2 RID: 11762 RVA: 0x000F9C10 File Offset: 0x000F7E10
	[PunRPC]
	private void PlayerStateChangeRPC(int playerResponsibleNumber, int playerActorNumber, int newState, PhotonMessageInfo info)
	{
		bool flag = this.IsValidClientRPC(info.Sender);
		bool flag2 = newState == 1 && info.Sender.ActorNumber == playerActorNumber && this.IsZoneActive();
		bool flag3 = newState == 0 && flag;
		if (!flag2 && !flag3)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(playerActorNumber);
		GRPlayer grplayer2 = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || grplayer2.IsNull() || !grplayer2.playerStateChangeLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (newState == 0 && playerResponsibleNumber != playerActorNumber)
		{
			GRPlayer grplayer3 = GRPlayer.Get(playerResponsibleNumber);
			if (grplayer3 != null && grplayer3 != grplayer)
			{
				grplayer3.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.Assists, 1f);
			}
		}
		grplayer.ChangePlayerState((GRPlayer.GRPlayerState)newState, this);
	}

	// Token: 0x06002DF3 RID: 11763 RVA: 0x000F9CD0 File Offset: 0x000F7ED0
	public void RequestGrantPlayerShield(GRPlayer player, int shieldHp, int shieldFlags)
	{
		base.GetView.RPC("RequestGrantPlayerShieldRPC", this.GetAuthorityPlayer(), new object[]
		{
			PhotonNetwork.LocalPlayer.ActorNumber,
			player.gamePlayer.rig.OwningNetPlayer.ActorNumber,
			shieldHp,
			shieldFlags
		});
	}

	// Token: 0x06002DF4 RID: 11764 RVA: 0x000F9D3C File Offset: 0x000F7F3C
	[PunRPC]
	private void RequestGrantPlayerShieldRPC(int shieldingPlayer, int playerToGrantShieldActorNumber, int shieldHp, int shieldFlags, PhotonMessageInfo info)
	{
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		GRPlayer grplayer2 = GRPlayer.Get(playerToGrantShieldActorNumber);
		if (!this.IsValidAuthorityRPC(info.Sender) || grplayer.IsNull() || !grplayer.fireShieldLimiter.CheckCallTime(Time.unscaledTime) || grplayer2.IsNull() || !grplayer2.CanActivateShield(shieldHp))
		{
			return;
		}
		base.GetView.RPC("ApplyGrantPlayerShieldRPC", RpcTarget.All, new object[] { shieldingPlayer, playerToGrantShieldActorNumber, shieldHp, shieldFlags });
	}

	// Token: 0x06002DF5 RID: 11765 RVA: 0x000F9DDC File Offset: 0x000F7FDC
	[PunRPC]
	private void ApplyGrantPlayerShieldRPC(int shieldingPlayer, int playerToGrantShieldActorNumber, int shieldHp, int shieldFlags, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.GrantPlayerShield))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(playerToGrantShieldActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (grplayer.TryActivateShield(shieldHp, shieldFlags))
		{
			GRPlayer grplayer2 = GRPlayer.Get(shieldingPlayer);
			if (grplayer2 != null)
			{
				grplayer2.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.Assists, 1f);
			}
		}
	}

	// Token: 0x06002DF6 RID: 11766 RVA: 0x000F9E40 File Offset: 0x000F8040
	public void RequestFireProjectile(GameEntityId entityId, Vector3 firingPosition, Vector3 targetPosition, double networkTime)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if ((NetworkSystem.Instance.InRoom && base.IsMine) || !NetworkSystem.Instance.InRoom)
		{
			base.GetView.RPC("RequestFireProjectileRPC", RpcTarget.All, new object[]
			{
				this.gameEntityManager.GetNetIdFromEntityId(entityId),
				firingPosition,
				targetPosition,
				networkTime
			});
		}
	}

	// Token: 0x06002DF7 RID: 11767 RVA: 0x000F9EC0 File Offset: 0x000F80C0
	[PunRPC]
	private void RequestFireProjectileRPC(int entityNetId, Vector3 firingPosition, Vector3 targetPosition, double networkTime, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, entityNetId, targetPosition) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.RequestFireProjectile) || !this.gameEntityManager.IsEntityNearPosition(entityNetId, firingPosition, 16f))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(entityNetId);
		this.OnRequestFireProjectileInternal(entityIdFromNetId, firingPosition, targetPosition, networkTime);
	}

	// Token: 0x06002DF8 RID: 11768 RVA: 0x000F9F1C File Offset: 0x000F811C
	private void OnRequestFireProjectileInternal(GameEntityId entityId, Vector3 firingPosition, Vector3 targetPosition, double networkTime)
	{
		GREnemyRanged gameComponent = this.gameEntityManager.GetGameComponent<GREnemyRanged>(entityId);
		if (gameComponent != null)
		{
			gameComponent.RequestRangedAttack(firingPosition, targetPosition, networkTime);
		}
		GRHazardTower gameComponent2 = this.gameEntityManager.GetGameComponent<GRHazardTower>(entityId);
		if (gameComponent2 != null)
		{
			gameComponent2.OnFire(firingPosition, targetPosition, networkTime);
		}
	}

	// Token: 0x06002DF9 RID: 11769 RVA: 0x000F9F6C File Offset: 0x000F816C
	[PunRPC]
	public void BroadcastHandprint(Vector3 pos, Quaternion orient, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		float num = 10000f;
		if (!(in pos).IsValid(in num) || !(in orient).IsValid())
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender);
		if (grplayer == null)
		{
			return;
		}
		if (!GameEntityManager.IsPlayerHandNearPosition(grplayer.gamePlayer, pos, false, true, 3f))
		{
			return;
		}
		if (info.Sender.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber && Time.time - this.LastHandprintTime <= 0.25f)
		{
			return;
		}
		this.LastHandprintTime = Time.time;
		this.reactor.AddHandprint(pos, orient);
	}

	// Token: 0x06002DFA RID: 11770 RVA: 0x000FA017 File Offset: 0x000F8217
	public void OnAbilityDie(GameEntity entity, float forcedRespawn = -1f)
	{
		if (this.reactor == null)
		{
			return;
		}
		this.reactor.OnAbilityDie(entity, forcedRespawn);
	}

	// Token: 0x06002DFB RID: 11771 RVA: 0x000FA038 File Offset: 0x000F8238
	public void RequestShiftStartAuthority(bool isFirstShift)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		if (!shiftManager.ShiftActive)
		{
			double time = PhotonNetwork.Time;
			SRand srand = new SRand(Mathf.FloorToInt(Time.time * 100f));
			int num = srand.NextInt(0, int.MaxValue);
			string text = Guid.NewGuid().ToString();
			this.photonView.RPC("ApplyShiftStartRPC", RpcTarget.All, new object[] { time, num, text, isFirstShift });
			shiftManager.RequestState(GhostReactorShiftManager.State.ShiftActive);
			ProgressionManager.Instance.StartOfShift(text, shiftManager.shiftRewardCoresForMothership, this.reactor.vrRigs.Count, this.reactor.GetDepthLevel());
		}
	}

	// Token: 0x06002DFC RID: 11772 RVA: 0x000FA120 File Offset: 0x000F8320
	[PunRPC]
	public void ApplyShiftStartRPC(double shiftStartTime, int randomSeed, string gameIdGuid, bool isFirstShift, PhotonMessageInfo info)
	{
		if (double.IsNaN(shiftStartTime) || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyShiftStart))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		int num = Math.Clamp(this.reactor.NumActivePlayers, 0, this.reactor.difficultyScalingPerPlayer.Count - 1);
		this.reactor.difficultyScalingForCurrentFloor = 1f;
		if (this.reactor.difficultyScalingPerPlayer.Count > 0)
		{
			this.reactor.difficultyScalingForCurrentFloor = this.reactor.difficultyScalingPerPlayer[num];
		}
		double num2 = PhotonNetwork.Time - shiftStartTime;
		if (num2 < 0.0 || num2 > 10.0)
		{
			return;
		}
		levelGenerator.Generate(randomSeed);
		if (this.gameEntityManager.IsAuthority())
		{
			if (this.activeSpawnSectionEntitiesCoroutine != null)
			{
				base.StopCoroutine(this.activeSpawnSectionEntitiesCoroutine);
			}
			this.activeSpawnSectionEntitiesCoroutine = base.StartCoroutine(this.SpawnSectionEntitiesCoroutine(this.reactor.difficultyScalingForCurrentFloor));
		}
		shiftManager.shiftStats.ResetShiftStats();
		shiftManager.ResetJudgment();
		shiftManager.RefreshShiftStatsDisplay();
		shiftManager.OnShiftStarted(gameIdGuid, shiftStartTime, true, isFirstShift);
		this.reactor.ClearAllHandprints();
		this.reactor.ClearAllRespawns();
	}

	// Token: 0x06002DFD RID: 11773 RVA: 0x000FA27B File Offset: 0x000F847B
	private IEnumerator SpawnSectionEntitiesCoroutine(float respawnCount)
	{
		int initialFrameCount = Time.frameCount;
		while (initialFrameCount == Time.frameCount)
		{
			yield return this.spawnSectionEntitiesWait;
		}
		if (this.gameEntityManager.IsAuthority())
		{
			this.reactor.levelGenerator.SpawnEntitiesInEachSection(respawnCount);
		}
		yield break;
	}

	// Token: 0x06002DFE RID: 11774 RVA: 0x000FA294 File Offset: 0x000F8494
	[PunRPC]
	public void RequestShiftEnd()
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		if (shiftManager == null || !shiftManager.ShiftActive)
		{
			return;
		}
		GhostReactorManager.tempEntitiesToDestroy.Clear();
		List<GameEntity> gameEntities = this.gameEntityManager.GetGameEntities();
		for (int i = 0; i < gameEntities.Count; i++)
		{
			GameEntity gameEntity = gameEntities[i];
			if (gameEntity != null && !this.ShouldEntitySurviveShift(gameEntity))
			{
				GhostReactorManager.tempEntitiesToDestroy.Add(gameEntity.id);
			}
		}
		this.gameEntityManager.RequestDestroyItems(GhostReactorManager.tempEntitiesToDestroy);
		this.photonView.RPC("ApplyShiftEndRPC", RpcTarget.Others, new object[] { PhotonNetwork.Time });
		levelGenerator.ClearLevelSections();
		shiftManager.OnShiftEnded(PhotonNetwork.Time, true, ZoneClearReason.JoinZone);
		shiftManager.CalculateShiftTotal();
		shiftManager.RevealJudgment(Mathf.FloorToInt((float)shiftManager.shiftStats.GetShiftStat(GRShiftStatType.EnemyDeaths) / 5f));
		shiftManager.RequestState(GhostReactorShiftManager.State.PostShift);
	}

	// Token: 0x06002DFF RID: 11775 RVA: 0x000FA3AA File Offset: 0x000F85AA
	public void SendRequestShiftEndRPC()
	{
		this.photonView.RPC("RequestShiftEnd", this.gameEntityManager.GetAuthorityPlayer(), Array.Empty<object>());
	}

	// Token: 0x06002E00 RID: 11776 RVA: 0x000FA3CC File Offset: 0x000F85CC
	[PunRPC]
	public void ApplyShiftEndRPC(double networkedTime, PhotonMessageInfo info)
	{
		if (!double.IsFinite(networkedTime) || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyShiftEnd))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		if (!shiftManager.ShiftActive)
		{
			return;
		}
		this.reactor.ClearAllRespawns();
		levelGenerator.ClearLevelSections();
		shiftManager.OnShiftEnded(networkedTime, true, ZoneClearReason.JoinZone);
		shiftManager.CalculateShiftTotal();
		shiftManager.RevealJudgment(Mathf.FloorToInt((float)shiftManager.shiftStats.GetShiftStat(GRShiftStatType.EnemyDeaths) / 5f));
	}

	// Token: 0x06002E01 RID: 11777 RVA: 0x000FA46C File Offset: 0x000F866C
	private bool ShouldEntitySurviveShift(GameEntity gameEntity)
	{
		if (gameEntity == null)
		{
			return true;
		}
		if (this.reactor == null)
		{
			return false;
		}
		if (this.IsEnemy(gameEntity))
		{
			return false;
		}
		if (gameEntity.GetComponent<GRBreakable>() != null || gameEntity.GetComponent<GRCollectibleDispenser>() != null || gameEntity.GetComponent<GRMetalEnergyGate>() != null || gameEntity.GetComponent<GRBarrierSpectral>() != null || gameEntity.GetComponent<GRSconce>() != null)
		{
			return false;
		}
		Collider safeZoneLimit = this.reactor.safeZoneLimit;
		Vector3 position = gameEntity.gameObject.transform.position;
		return safeZoneLimit.bounds.Contains(position) || gameEntity.GetComponent<GRBadge>() != null;
	}

	// Token: 0x06002E02 RID: 11778 RVA: 0x000FA52C File Offset: 0x000F872C
	private bool IsEnemy(GameEntity gameEntity)
	{
		return gameEntity.GetComponent<GREnemyChaser>() != null || gameEntity.GetComponent<GREnemyRanged>() != null || gameEntity.GetComponent<GREnemyPhantom>() != null || gameEntity.GetComponent<GREnemyPest>() != null || gameEntity.GetComponent<GREnemySummoner>() != null || gameEntity.GetComponent<GREnemyMonkeye>() != null || gameEntity.GetComponent<GREnemyBossMoon>() != null;
	}

	// Token: 0x06002E03 RID: 11779 RVA: 0x000FA59C File Offset: 0x000F879C
	public void InstantDeathForCurrentEnemies()
	{
		int num = 0;
		List<GameEntity> gameEntities = this.gameEntityManager.GetGameEntities();
		for (int i = 0; i < gameEntities.Count; i++)
		{
			if (!(gameEntities[i] == null))
			{
				GameEntity gameEntity = gameEntities[i];
				if (!(gameEntity.GetComponent<GREnemyBossMoon>() != null))
				{
					GREnemyChaser component = gameEntity.GetComponent<GREnemyChaser>();
					if (component != null)
					{
						component.InstantDeath();
						num++;
					}
					else
					{
						GREnemyRanged component2 = gameEntity.GetComponent<GREnemyRanged>();
						if (component2 != null)
						{
							component2.InstantDeath();
							num++;
						}
						else
						{
							GREnemyPest component3 = gameEntity.GetComponent<GREnemyPest>();
							if (component3 != null)
							{
								component3.InstantDeath();
								num++;
							}
							else
							{
								GREnemySummoner component4 = gameEntity.GetComponent<GREnemySummoner>();
								if (component4 != null)
								{
									component4.InstantDeath();
									num++;
								}
								else
								{
									GREnemyMonkeye component5 = gameEntity.GetComponent<GREnemyMonkeye>();
									if (component5 != null)
									{
										component5.InstantDeath();
										num++;
									}
								}
							}
						}
					}
				}
			}
		}
		Debug.Log(string.Format("Instant death for {0} enemies.", num));
	}

	// Token: 0x06002E04 RID: 11780 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void RequestRestoreBossHP()
	{
	}

	// Token: 0x06002E05 RID: 11781 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void RequestHurtBossHP()
	{
	}

	// Token: 0x06002E06 RID: 11782 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void RequestKillBossEyes()
	{
	}

	// Token: 0x06002E07 RID: 11783 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void RequestKillBossSummoned()
	{
	}

	// Token: 0x06002E08 RID: 11784 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void RequestGoBackBossPhase()
	{
	}

	// Token: 0x06002E09 RID: 11785 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void RequestAdvanceBossPhase()
	{
	}

	// Token: 0x06002E0A RID: 11786 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void RequestBossBehavior(GREnemyBossMoon.Behavior bossBehavior)
	{
	}

	// Token: 0x06002E0B RID: 11787 RVA: 0x000FA6A8 File Offset: 0x000F88A8
	public GameEntity GetBossEntity()
	{
		if (this.cachedBossEntity != null && this.cachedBossEntity.IsNotNull())
		{
			return this.cachedBossEntity;
		}
		if (this.gameEntityManager == null)
		{
			return null;
		}
		GameEntity gameEntity = null;
		List<GameEntity> gameEntities = this.gameEntityManager.GetGameEntities();
		for (int i = 0; i < gameEntities.Count; i++)
		{
			if (!(gameEntities[i] == null) && !(gameEntities[i].GetComponent<GREnemyBossMoon>() == null))
			{
				gameEntity = gameEntities[i];
				break;
			}
		}
		this.cachedBossEntity = gameEntity;
		return gameEntity;
	}

	// Token: 0x06002E0C RID: 11788 RVA: 0x000FA73B File Offset: 0x000F893B
	public void ClearCachedBossEntity()
	{
		this.cachedBossEntity = null;
	}

	// Token: 0x06002E0D RID: 11789 RVA: 0x000FA744 File Offset: 0x000F8944
	public void ReportEnemyDeath()
	{
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.EnemyDeaths);
		shiftManager.RefreshShiftStatsDisplay();
		PlayerGameEvents.MiscEvent("GRKillEnemy", 1);
	}

	// Token: 0x06002E0E RID: 11790 RVA: 0x000FA77C File Offset: 0x000F897C
	public void ReportCoreCollection(GRPlayer player, ProgressionManager.CoreType type)
	{
		Debug.Log("GhostReactorManager ReportCoreCollection");
		if (player == null)
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		if (type == ProgressionManager.CoreType.ChaosSeed)
		{
			shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.SentientCoresCollected);
		}
		else if (type == ProgressionManager.CoreType.SuperCore)
		{
			shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.CoresCollected);
			shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.CoresCollected);
			shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.CoresCollected);
			player.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.CoresDeposited, 3f);
			int count = this.reactor.vrRigs.Count;
			for (int i = 0; i < count; i++)
			{
				GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
				if (grplayer != null)
				{
					grplayer.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.EarnedCredits, 15f);
				}
			}
		}
		else
		{
			shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.CoresCollected);
			player.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.CoresDeposited, 1f);
			int count2 = this.reactor.vrRigs.Count;
			for (int j = 0; j < count2; j++)
			{
				GRPlayer grplayer2 = GRPlayer.Get(this.reactor.vrRigs[j]);
				if (grplayer2 != null)
				{
					grplayer2.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.EarnedCredits, 5f);
				}
			}
		}
		shiftManager.RefreshShiftStatsDisplay();
		PlayerGameEvents.MiscEvent("GRCollectCore", 1);
	}

	// Token: 0x06002E0F RID: 11791 RVA: 0x000FA8C8 File Offset: 0x000F8AC8
	public void ReportPlayerDeath(GRPlayer player)
	{
		if (this.reactor == null || player == null || this.reactor.zone == GTZone.customMaps)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.PlayerDeaths);
		shiftManager.RefreshShiftStatsDisplay();
		player.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.Deaths, 1f);
	}

	// Token: 0x06002E10 RID: 11792 RVA: 0x000FA924 File Offset: 0x000F8B24
	public void PromotionBotActivePlayerRequest(int state)
	{
		this.photonView.RPC("PromotionBotActivePlayerRequestRPC", this.GetAuthorityPlayer(), new object[] { state });
	}

	// Token: 0x06002E11 RID: 11793 RVA: 0x000FA94C File Offset: 0x000F8B4C
	[PunRPC]
	public void PromotionBotActivePlayerRequestRPC(int state, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.promotionBotLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GRUIPromotionBot promotionBot = this.reactor.promotionBot;
		if (promotionBot == null)
		{
			return;
		}
		if (state == 6)
		{
			if (promotionBot.currentPlayerActorNumber != -1)
			{
				return;
			}
			state = 1;
		}
		int actorNumber = info.Sender.ActorNumber;
		this.photonView.RPC("PromotionBotActivePlayerResponseRPC", RpcTarget.Others, new object[] { actorNumber, state });
		promotionBot.SetActivePlayerStateChange(actorNumber, state);
	}

	// Token: 0x06002E12 RID: 11794 RVA: 0x000FAA08 File Offset: 0x000F8C08
	[PunRPC]
	public void PromotionBotActivePlayerResponseRPC(int actorNumber, int state, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRUIPromotionBot promotionBot = this.reactor.promotionBot;
		if (GRPlayer.Get(info.Sender.ActorNumber) == null || promotionBot == null || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.PromotionBotResponse))
		{
			return;
		}
		promotionBot.SetActivePlayerStateChange(actorNumber, state);
	}

	// Token: 0x06002E13 RID: 11795 RVA: 0x000FAA78 File Offset: 0x000F8C78
	[PunRPC]
	public void BroadcastScoreboardPage(int scoreboardPage, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || !grplayer.scoreboardPageLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (GRUIScoreboard.ValidPage((GRUIScoreboard.ScoreboardScreen)scoreboardPage))
		{
			GhostReactor.instance.UpdateScoreboardScreen((GRUIScoreboard.ScoreboardScreen)scoreboardPage);
		}
	}

	// Token: 0x06002E14 RID: 11796 RVA: 0x000FAAD4 File Offset: 0x000F8CD4
	[PunRPC]
	public void BroadcastStartingProgression(int points, int redeemedPoints, double shiftJoinedTime, PhotonMessageInfo info)
	{
		if (double.IsNaN(shiftJoinedTime) || double.IsInfinity(shiftJoinedTime))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || !grplayer.progressionBroadcastLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		grplayer.SetProgressionData(points, redeemedPoints, false);
		grplayer.shiftJoinTime = Math.Clamp(shiftJoinedTime, PhotonNetwork.Time - 10.0, PhotonNetwork.Time);
	}

	// Token: 0x06002E15 RID: 11797 RVA: 0x000FAB58 File Offset: 0x000F8D58
	public void RequestPlayerAction(GhostReactorManager.GRPlayerAction playerAction)
	{
		this.photonView.RPC("RequestPlayerActionRPC", this.GetAuthorityPlayer(), new object[]
		{
			(int)playerAction,
			0,
			0
		});
	}

	// Token: 0x06002E16 RID: 11798 RVA: 0x000FAB91 File Offset: 0x000F8D91
	public void RequestPlayerAction(GhostReactorManager.GRPlayerAction playerAction, int param0)
	{
		this.photonView.RPC("RequestPlayerActionRPC", this.GetAuthorityPlayer(), new object[]
		{
			(int)playerAction,
			param0,
			0
		});
	}

	// Token: 0x06002E17 RID: 11799 RVA: 0x000FABCA File Offset: 0x000F8DCA
	public void RequestPlayerAction(GhostReactorManager.GRPlayerAction playerAction, int param0, int param1)
	{
		this.photonView.RPC("RequestPlayerActionRPC", this.GetAuthorityPlayer(), new object[]
		{
			(int)playerAction,
			param0,
			param1
		});
	}

	// Token: 0x06002E18 RID: 11800 RVA: 0x000FAC04 File Offset: 0x000F8E04
	public bool VerifyShuttleInteractability(GRPlayer player, int shuttleIdx, bool ignoreOwnership = false)
	{
		if (GRElevatorManager._instance == null)
		{
			return false;
		}
		GRShuttle shuttleById = GRElevatorManager._instance.GetShuttleById(shuttleIdx);
		return !(shuttleById == null) && shuttleById.IsShuttleInteractableByPlayer(player, ignoreOwnership);
	}

	// Token: 0x06002E19 RID: 11801 RVA: 0x000FAC40 File Offset: 0x000F8E40
	[PunRPC]
	public void RequestPlayerActionRPC(int playerAction, int param0, int param1, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestShiftStartLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		bool flag = false;
		switch (playerAction)
		{
		case 1:
			flag = !shiftManager.ShiftActive && shiftManager.authorizedToDelveDeeper;
			if (flag)
			{
				int num = this.reactor.GetDepthLevel() + 1;
				this.reactor.depthConfigIndex = this.reactor.PickLevelConfigForDepth(num);
				param0 = num;
				param1 = this.reactor.depthConfigIndex;
			}
			break;
		case 2:
			flag = true;
			break;
		case 3:
			flag = this.VerifyShuttleInteractability(grplayer, param0, true);
			param1 = info.Sender.ActorNumber;
			break;
		case 4:
			flag = this.VerifyShuttleInteractability(grplayer, param0, false);
			param1 = info.Sender.ActorNumber;
			break;
		case 5:
			flag = this.VerifyShuttleInteractability(grplayer, param0, false);
			param1 = info.Sender.ActorNumber;
			break;
		case 6:
			flag = this.VerifyShuttleInteractability(grplayer, param0, false);
			param1 = info.Sender.ActorNumber;
			break;
		case 7:
			flag = this.VerifyShuttleInteractability(grplayer, param0, false);
			param1 = info.Sender.ActorNumber;
			break;
		case 8:
			flag = this.VerifyShuttleInteractability(grplayer, param0, false);
			param1 = info.Sender.ActorNumber;
			break;
		case 9:
			flag = true;
			param0 = Mathf.Clamp(param0, 0, 1);
			param1 = info.Sender.ActorNumber;
			break;
		case 10:
			flag = true;
			param0 = Mathf.Clamp(param0, 0, 3);
			param1 = info.Sender.ActorNumber;
			break;
		case 11:
			flag = param0 == info.Sender.ActorNumber || this.IsAuthorityPlayer(info.Sender);
			if (this.reactor.seedExtractor.StationOpen && this.reactor.seedExtractor.CurrentPlayerActorNumber != info.Sender.ActorNumber)
			{
				playerAction = 13;
			}
			break;
		case 12:
			flag = this.IsAuthorityPlayer(info.Sender);
			break;
		case 13:
			flag = this.IsAuthorityPlayer(info.Sender);
			break;
		case 14:
		{
			GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(param1);
			if (this.IsAuthorityPlayer(info.Sender) && gameEntityFromNetId != null && gameEntityFromNetId.lastHeldByActorNumber == param0)
			{
				flag = true;
			}
			break;
		}
		case 15:
		{
			int num2 = param1;
			GameEntity gameEntityFromNetId2 = this.gameEntityManager.GetGameEntityFromNetId(num2);
			if (gameEntityFromNetId2 != null && this.reactor.seedExtractor.ValidateSeedDepositSucceeded(param0, param1))
			{
				this.gameEntityManager.RequestDestroyItem(gameEntityFromNetId2.id);
				flag = true;
			}
			break;
		}
		case 16:
			flag = info.Sender.ActorNumber == param0;
			break;
		}
		if (flag)
		{
			this.photonView.RPC("ApplyPlayerActionRPC", RpcTarget.All, new object[] { playerAction, param0, param1 });
		}
	}

	// Token: 0x06002E1A RID: 11802 RVA: 0x000FAF90 File Offset: 0x000F9190
	[PunRPC]
	public void ApplyPlayerActionRPC(int playerAction, int param0, int param1, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyShiftStart))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		this.gameEntityManager.IsAuthorityPlayer(info.Sender);
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestShiftStartLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		switch (playerAction)
		{
		case 1:
			this.reactor.SetNextDelveDepth(param0, param1);
			return;
		case 2:
			this.reactor.shiftManager.SetState((GhostReactorShiftManager.State)param0, false);
			return;
		case 3:
		{
			GRPlayer grplayer2 = GRPlayer.Get(param1);
			if (grplayer2 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer2, param0, true))
			{
				return;
			}
			GRShuttle shuttleById = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById != null)
			{
				shuttleById.OnOpenDoor();
				return;
			}
			break;
		}
		case 4:
		{
			GRPlayer grplayer3 = GRPlayer.Get(param1);
			if (grplayer3 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer3, param0, false))
			{
				return;
			}
			GRShuttle shuttleById2 = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById2 != null)
			{
				shuttleById2.OnCloseDoor();
				return;
			}
			break;
		}
		case 5:
		{
			GRPlayer grplayer4 = GRPlayer.Get(param1);
			if (grplayer4 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer4, param0, false))
			{
				return;
			}
			GRShuttle shuttleById3 = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById3 != null)
			{
				shuttleById3.OnLaunch();
				return;
			}
			break;
		}
		case 6:
		{
			GRPlayer grplayer5 = GRPlayer.Get(param1);
			if (grplayer5 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer5, param0, false))
			{
				return;
			}
			GRShuttle shuttleById4 = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById4 != null)
			{
				shuttleById4.OnArrive();
				return;
			}
			break;
		}
		case 7:
		{
			GRPlayer grplayer6 = GRPlayer.Get(param1);
			if (grplayer6 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer6, param0, false))
			{
				return;
			}
			GRShuttle shuttleById5 = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById5 != null)
			{
				shuttleById5.OnTargetLevelUp();
				return;
			}
			break;
		}
		case 8:
		{
			GRPlayer grplayer7 = GRPlayer.Get(param1);
			if (grplayer7 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer7, param0, false))
			{
				return;
			}
			GRShuttle shuttleById6 = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById6 != null)
			{
				shuttleById6.OnTargetLevelDown();
				return;
			}
			break;
		}
		case 9:
		{
			GRPlayer grplayer8 = GRPlayer.Get(param1);
			if (grplayer8 != null)
			{
				param0 = Mathf.Clamp(param0, 0, 1);
				grplayer8.dropPodLevel = param0;
				this.reactor.RefreshBays();
				grplayer8.RefreshShuttles();
				return;
			}
			break;
		}
		case 10:
		{
			GRPlayer grplayer9 = GRPlayer.Get(param1);
			if (grplayer9 != null)
			{
				param0 = Mathf.Clamp(param0, 0, 3);
				grplayer9.dropPodChasisLevel = param0;
				this.reactor.RefreshBays();
				grplayer9.RefreshShuttles();
				return;
			}
			break;
		}
		case 11:
			this.reactor.seedExtractor.CardSwipeSuccess();
			this.reactor.seedExtractor.OpenStation(param0);
			return;
		case 12:
			this.reactor.seedExtractor.CloseStation();
			return;
		case 13:
			this.reactor.seedExtractor.CardSwipeFail();
			return;
		case 14:
			this.reactor.seedExtractor.TryDepositSeed(param0, param1);
			return;
		case 15:
			this.reactor.seedExtractor.SeedDepositSucceeded(param0, param1);
			return;
		case 16:
			this.reactor.seedExtractor.SeedDepositFailed(param0, param1);
			break;
		default:
			return;
		}
	}

	// Token: 0x06002E1B RID: 11803 RVA: 0x000FB304 File Offset: 0x000F9504
	public GRToolUpgradePurchaseStationFull GetToolUpgradeStationFullForIndex(int idx)
	{
		if (this.reactor == null || this.reactor.toolUpgradePurchaseStationsFull == null || idx < 0 || idx >= this.reactor.toolUpgradePurchaseStationsFull.Count)
		{
			return null;
		}
		return this.reactor.toolUpgradePurchaseStationsFull[idx];
	}

	// Token: 0x06002E1C RID: 11804 RVA: 0x000FB356 File Offset: 0x000F9556
	public int GetIndexForToolUpgradeStationFull(GRToolUpgradePurchaseStationFull station)
	{
		if (this.reactor == null || this.reactor.toolUpgradePurchaseStationsFull == null)
		{
			return -1;
		}
		return this.reactor.toolUpgradePurchaseStationsFull.IndexOf(station);
	}

	// Token: 0x06002E1D RID: 11805 RVA: 0x000FB388 File Offset: 0x000F9588
	public void RequestNetworkShelfAndItemChange(GRToolUpgradePurchaseStationFull station, int shelf, int item)
	{
		int indexForToolUpgradeStationFull = this.GetIndexForToolUpgradeStationFull(station);
		if (indexForToolUpgradeStationFull == -1)
		{
			return;
		}
		this.photonView.RPC("ToolPurchaseV2_RPC", RpcTarget.Others, new object[]
		{
			GhostReactorManager.ToolPurchaseActionV2.SelectShelfAndItem,
			PhotonNetwork.LocalPlayer.ActorNumber,
			indexForToolUpgradeStationFull,
			shelf,
			item
		});
	}

	// Token: 0x06002E1E RID: 11806 RVA: 0x000FB3F0 File Offset: 0x000F95F0
	private void SelectToolShelfAndItemRPCRouted(int stationIndex, int shelf, int item, PhotonMessageInfo info)
	{
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		if (toolUpgradeStationFullForIndex.currentActivePlayerActorNumber == info.Sender.ActorNumber)
		{
			toolUpgradeStationFullForIndex.SetSelectedShelfAndItem(shelf, item, true);
		}
	}

	// Token: 0x06002E1F RID: 11807 RVA: 0x000FB42C File Offset: 0x000F962C
	public void RequestPurchaseToolOrUpgrade(GRToolUpgradePurchaseStationFull station, int shelf, int item)
	{
		int indexForToolUpgradeStationFull = this.GetIndexForToolUpgradeStationFull(station);
		if (indexForToolUpgradeStationFull == -1)
		{
			return;
		}
		this.photonView.RPC("ToolPurchaseV2_RPC", this.GetAuthorityPlayer(), new object[]
		{
			GhostReactorManager.ToolPurchaseActionV2.RequestPurchaseAuthority,
			PhotonNetwork.LocalPlayer.ActorNumber,
			indexForToolUpgradeStationFull,
			shelf,
			item
		});
	}

	// Token: 0x06002E20 RID: 11808 RVA: 0x000FB498 File Offset: 0x000F9698
	public void RequestPurchaseRPCRoutedAuthority(int stationIndex, int shelf, int item, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull())
		{
			return;
		}
		if (toolUpgradeStationFullForIndex.currentActivePlayerActorNumber != info.Sender.ActorNumber)
		{
			return;
		}
		ValueTuple<bool, bool> valueTuple = toolUpgradeStationFullForIndex.TryPurchaseAuthority(grplayer, shelf, item);
		bool item2 = valueTuple.Item1;
		if (!valueTuple.Item2)
		{
			return;
		}
		if (item2)
		{
			this.photonView.RPC("ToolPurchaseV2_RPC", RpcTarget.Others, new object[]
			{
				GhostReactorManager.ToolPurchaseActionV2.NotifyPurchaseSuccess,
				info.Sender.ActorNumber,
				stationIndex,
				shelf,
				item
			});
		}
		else
		{
			this.photonView.RPC("ToolPurchaseV2_RPC", RpcTarget.Others, new object[]
			{
				GhostReactorManager.ToolPurchaseActionV2.NotifyPurchaseFail,
				info.Sender.ActorNumber,
				stationIndex,
				shelf,
				item
			});
		}
		toolUpgradeStationFullForIndex.ToolPurchaseResponseLocal(grplayer, shelf, item, item2);
	}

	// Token: 0x06002E21 RID: 11809 RVA: 0x000FB5BC File Offset: 0x000F97BC
	public void NotifyPurchaseToolOrUpgradeRPCRouted(int actorNumber, int stationIndex, int shelf, int item, bool success, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender))
		{
			return;
		}
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(actorNumber);
		if (grplayer != null)
		{
			toolUpgradeStationFullForIndex.ToolPurchaseResponseLocal(grplayer, shelf, item, success);
		}
	}

	// Token: 0x06002E22 RID: 11810 RVA: 0x000FB608 File Offset: 0x000F9808
	public void RequestStationExclusivity(GRToolUpgradePurchaseStationFull station)
	{
		int indexForToolUpgradeStationFull = this.GetIndexForToolUpgradeStationFull(station);
		if (indexForToolUpgradeStationFull == -1)
		{
			return;
		}
		this.photonView.RPC("ToolPurchaseV2_RPC", this.GetAuthorityPlayer(), new object[]
		{
			GhostReactorManager.ToolPurchaseActionV2.RequestStationExclusivityAuthority,
			PhotonNetwork.LocalPlayer.ActorNumber,
			indexForToolUpgradeStationFull,
			0,
			0
		});
	}

	// Token: 0x06002E23 RID: 11811 RVA: 0x000FB674 File Offset: 0x000F9874
	public void SetActivePlayerAuthority(GRToolUpgradePurchaseStationFull station, int actorNumber)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int indexForToolUpgradeStationFull = this.GetIndexForToolUpgradeStationFull(station);
		if (indexForToolUpgradeStationFull == -1)
		{
			return;
		}
		station.SetActivePlayer(actorNumber);
		this.photonView.RPC("ToolPurchaseV2_RPC", RpcTarget.Others, new object[]
		{
			GhostReactorManager.ToolPurchaseActionV2.SetToolStationActivePlayer,
			PhotonNetwork.LocalPlayer.ActorNumber,
			indexForToolUpgradeStationFull,
			station.currentActivePlayerActorNumber,
			0
		});
	}

	// Token: 0x06002E24 RID: 11812 RVA: 0x000FB6F0 File Offset: 0x000F98F0
	public void RequestStationExclusivityRPCRoutedAuthority(int stationIndex, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		if (toolUpgradeStationFullForIndex.currentActivePlayerActorNumber != -1)
		{
			return;
		}
		this.SetActivePlayerAuthority(toolUpgradeStationFullForIndex, info.Sender.ActorNumber);
	}

	// Token: 0x06002E25 RID: 11813 RVA: 0x000FB73C File Offset: 0x000F993C
	public void SetToolStationActivePlayerRPCRouted(int stationIndex, int activeOwner, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender))
		{
			return;
		}
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		toolUpgradeStationFullForIndex.SetActivePlayer(activeOwner);
	}

	// Token: 0x06002E26 RID: 11814 RVA: 0x000FB774 File Offset: 0x000F9974
	public void BroadcastHandleAndSelectionWheelPosition(GRToolUpgradePurchaseStationFull station, int handlePos, int wheelPos)
	{
		int indexForToolUpgradeStationFull = this.GetIndexForToolUpgradeStationFull(station);
		if (indexForToolUpgradeStationFull == -1)
		{
			return;
		}
		if (NetworkSystem.Instance.LocalPlayer.ActorNumber != station.currentActivePlayerActorNumber)
		{
			return;
		}
		this.photonView.RPC("ToolPurchaseV2_RPC", RpcTarget.Others, new object[]
		{
			GhostReactorManager.ToolPurchaseActionV2.SetHandleAndSelectionWheelPosition,
			PhotonNetwork.LocalPlayer.ActorNumber,
			indexForToolUpgradeStationFull,
			handlePos,
			wheelPos
		});
	}

	// Token: 0x06002E27 RID: 11815 RVA: 0x000FB7F4 File Offset: 0x000F99F4
	public void SetHandleAndSelectionWheelPositionRPCRouted(int stationIndex, int handlePos, int wheelPos, PhotonMessageInfo info)
	{
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		if (info.Sender.ActorNumber != toolUpgradeStationFullForIndex.currentActivePlayerActorNumber)
		{
			return;
		}
		toolUpgradeStationFullForIndex.SetHandleAndSelectionWheelPositionRemote(handlePos, wheelPos);
	}

	// Token: 0x06002E28 RID: 11816 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void RequestHackToolStation()
	{
	}

	// Token: 0x06002E29 RID: 11817 RVA: 0x000FB830 File Offset: 0x000F9A30
	[PunRPC]
	public void ToolPurchaseV2_RPC(GhostReactorManager.ToolPurchaseActionV2 command, int initiatorID, int stationIndex, int param1, int param2, PhotonMessageInfo info)
	{
		if (this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ToolUpgradeStationAction))
		{
			return;
		}
		switch (command)
		{
		case GhostReactorManager.ToolPurchaseActionV2.RequestPurchaseAuthority:
			this.RequestPurchaseRPCRoutedAuthority(stationIndex, param1, param2, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.SelectShelfAndItem:
			this.SelectToolShelfAndItemRPCRouted(stationIndex, param1, param2, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.NotifyPurchaseFail:
			this.NotifyPurchaseToolOrUpgradeRPCRouted(initiatorID, stationIndex, param1, param2, false, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.NotifyPurchaseSuccess:
			this.NotifyPurchaseToolOrUpgradeRPCRouted(initiatorID, stationIndex, param1, param2, true, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.RequestStationExclusivityAuthority:
			this.RequestStationExclusivityRPCRoutedAuthority(stationIndex, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.SetToolStationActivePlayer:
			this.SetToolStationActivePlayerRPCRouted(stationIndex, param1, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.SetHandleAndSelectionWheelPosition:
			this.SetHandleAndSelectionWheelPositionRPCRouted(stationIndex, param1, param2, info);
			break;
		case GhostReactorManager.ToolPurchaseActionV2.SetToolStationHackedDebug:
			break;
		default:
			return;
		}
	}

	// Token: 0x06002E2A RID: 11818 RVA: 0x000FB8D3 File Offset: 0x000F9AD3
	public void ToolPurchaseStationRequest(int stationIndex, GhostReactorManager.ToolPurchaseStationAction action)
	{
		this.photonView.RPC("ToolPurchaseStationRequestRPC", this.GetAuthorityPlayer(), new object[] { stationIndex, action });
	}

	// Token: 0x06002E2B RID: 11819 RVA: 0x000FB904 File Offset: 0x000F9B04
	[PunRPC]
	public void ToolPurchaseStationRequestRPC(int stationIndex, GhostReactorManager.ToolPurchaseStationAction action, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		if (!this.IsValidAuthorityRPC(info.Sender) || stationIndex < 0 || stationIndex >= toolPurchasingStations.Count)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestToolPurchaseStationLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GRToolPurchaseStation grtoolPurchaseStation = toolPurchasingStations[stationIndex];
		if (grtoolPurchaseStation == null)
		{
			return;
		}
		switch (action)
		{
		case GhostReactorManager.ToolPurchaseStationAction.ShiftLeft:
			grtoolPurchaseStation.ShiftLeftAuthority();
			this.photonView.RPC("ToolPurchaseStationResponseRPC", RpcTarget.Others, new object[]
			{
				stationIndex,
				GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate,
				grtoolPurchaseStation.ActiveEntryIndex,
				0
			});
			this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate, grtoolPurchaseStation.ActiveEntryIndex, 0);
			return;
		case GhostReactorManager.ToolPurchaseStationAction.ShiftRight:
			grtoolPurchaseStation.ShiftRightAuthority();
			this.photonView.RPC("ToolPurchaseStationResponseRPC", RpcTarget.Others, new object[]
			{
				stationIndex,
				GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate,
				grtoolPurchaseStation.ActiveEntryIndex,
				0
			});
			this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate, grtoolPurchaseStation.ActiveEntryIndex, 0);
			return;
		case GhostReactorManager.ToolPurchaseStationAction.TryPurchase:
		{
			bool flag = false;
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetNetPlayerByID(info.Sender.ActorNumber), out rigContainer))
			{
				GRPlayer component = rigContainer.Rig.GetComponent<GRPlayer>();
				int num;
				if (component != null && grtoolPurchaseStation.TryPurchaseAuthority(component, out num))
				{
					this.photonView.RPC("ToolPurchaseStationResponseRPC", RpcTarget.Others, new object[]
					{
						stationIndex,
						GhostReactorManager.ToolPurchaseStationResponse.PurchaseSucceeded,
						info.Sender.ActorNumber,
						num
					});
					this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.PurchaseSucceeded, info.Sender.ActorNumber, num);
					flag = true;
				}
			}
			if (!flag)
			{
				this.photonView.RPC("ToolPurchaseStationResponseRPC", RpcTarget.Others, new object[]
				{
					stationIndex,
					GhostReactorManager.ToolPurchaseStationResponse.PurchaseFailed,
					info.Sender.ActorNumber,
					0
				});
				this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.PurchaseFailed, info.Sender.ActorNumber, 0);
			}
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06002E2C RID: 11820 RVA: 0x000FBB4C File Offset: 0x000F9D4C
	[PunRPC]
	public void ToolPurchaseStationResponseRPC(int stationIndex, GhostReactorManager.ToolPurchaseStationResponse responseType, int dataA, int dataB, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		if (!this.IsValidClientRPC(info.Sender) || stationIndex < 0 || stationIndex >= toolPurchasingStations.Count || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ToolPurchaseResponse))
		{
			return;
		}
		this.ToolPurchaseResponseLocal(stationIndex, responseType, dataA, dataB);
	}

	// Token: 0x06002E2D RID: 11821 RVA: 0x000FBBAC File Offset: 0x000F9DAC
	private void ToolPurchaseResponseLocal(int stationIndex, GhostReactorManager.ToolPurchaseStationResponse responseType, int dataA, int dataB)
	{
		if (this.reactor == null)
		{
			return;
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		if (stationIndex < 0 || stationIndex >= toolPurchasingStations.Count)
		{
			return;
		}
		GRToolPurchaseStation grtoolPurchaseStation = toolPurchasingStations[stationIndex];
		if (grtoolPurchaseStation == null)
		{
			return;
		}
		switch (responseType)
		{
		case GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate:
			grtoolPurchaseStation.OnSelectionUpdate(dataA);
			return;
		case GhostReactorManager.ToolPurchaseStationResponse.PurchaseSucceeded:
		{
			grtoolPurchaseStation.OnPurchaseSucceeded();
			GRPlayer grplayer = GRPlayer.Get(dataA);
			if (grplayer != null)
			{
				grplayer.IncrementCoresSpentPlayer(dataB);
				grplayer.AddItemPurchased(grtoolPurchaseStation.GetCurrentToolName());
				grplayer.SubtractShiftCredit(dataB);
				return;
			}
			break;
		}
		case GhostReactorManager.ToolPurchaseStationResponse.PurchaseFailed:
			grtoolPurchaseStation.OnPurchaseFailed();
			break;
		default:
			return;
		}
	}

	// Token: 0x06002E2E RID: 11822 RVA: 0x000FBC48 File Offset: 0x000F9E48
	public void ToolUpgradeStationRequestUpgrade(GRToolProgressionManager.ToolParts UpgradeID, int entityNetId)
	{
		this.photonView.RPC("ToolUpgradeStationRequestUpgradeRPC", this.GetAuthorityPlayer(), new object[] { UpgradeID, entityNetId });
	}

	// Token: 0x06002E2F RID: 11823 RVA: 0x000FBC78 File Offset: 0x000F9E78
	public void ToolSnapRequestUpgrade(int upgradeNetID, GRToolProgressionManager.ToolParts UpgradeID, int entityNetId)
	{
		this.photonView.RPC("ToolSnapRequestUpgradeRPC", this.GetAuthorityPlayer(), new object[] { upgradeNetID, UpgradeID, entityNetId });
	}

	// Token: 0x06002E30 RID: 11824 RVA: 0x000FBCB4 File Offset: 0x000F9EB4
	[PunRPC]
	public void ToolSnapRequestUpgradeRPC(int upgradeNetID, GRToolProgressionManager.ToolParts UpgradeID, int entityNetId, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ToolUpgradeStationAction))
		{
			return;
		}
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(this.gameEntityManager.GetEntityIdFromNetId(entityNetId));
		if (gameEntity != null)
		{
			Object component = gameEntity.GetComponent<GRTool>();
			GameEntity gameEntity2 = this.gameEntityManager.GetGameEntity(this.gameEntityManager.GetEntityIdFromNetId(upgradeNetID));
			if (component != null && gameEntity2 != null && GameEntityManager.IsPlayerHandNearPosition(grplayer.gamePlayer, gameEntity2.transform.position, false, true, 16f) && GameEntityManager.IsPlayerHandNearPosition(grplayer.gamePlayer, gameEntity2.transform.position, false, true, 16f))
			{
				this.photonView.RPC("UpgradeToolRemoteRPC", RpcTarget.All, new object[]
				{
					UpgradeID,
					entityNetId,
					false,
					info.Sender.ActorNumber
				});
				this.gameEntityManager.RequestDestroyItem(gameEntity2.id);
			}
		}
	}

	// Token: 0x06002E31 RID: 11825 RVA: 0x000FBDFC File Offset: 0x000F9FFC
	public void ToolUpgradeStationRequestUpgradeRPC(GRToolProgressionManager.ToolParts UpgradeID, int entityNetId, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002E32 RID: 11826 RVA: 0x000FBE0C File Offset: 0x000FA00C
	[PunRPC]
	public void UpgradeToolRemoteRPC(GRToolProgressionManager.ToolParts UpgradeID, int entityNetId, bool applyCost, int playerNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender))
		{
			return;
		}
		if (applyCost)
		{
			GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
			int num;
			if (grplayer != null && this.reactor.toolProgression.GetShiftCreditCost(UpgradeID, out num))
			{
				grplayer.SubtractShiftCredit(num);
			}
		}
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(this.gameEntityManager.GetEntityIdFromNetId(entityNetId));
		if (gameEntity != null)
		{
			GRTool component = gameEntity.GetComponent<GRTool>();
			if (component != null)
			{
				component.UpgradeTool(UpgradeID);
			}
		}
	}

	// Token: 0x06002E33 RID: 11827 RVA: 0x00023F0C File Offset: 0x0002210C
	private bool DoesUserHaveResearchUnlocked(int UserID, string ResearchID)
	{
		return true;
	}

	// Token: 0x06002E34 RID: 11828 RVA: 0x000FBE9B File Offset: 0x000FA09B
	public void ToolPlacedInUpgradeStation(GameEntity entity)
	{
		this.photonView.RPC("PlacedToolInUpgradeStationRPC", RpcTarget.All, new object[] { this.gameEntityManager.GetNetIdFromEntityId(entity.id) });
	}

	// Token: 0x06002E35 RID: 11829 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void PlacedToolInUpgradeStationRPC(int entityNetId, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002E36 RID: 11830 RVA: 0x000FBECD File Offset: 0x000FA0CD
	public void UpgradeToolAtToolStation()
	{
		this.photonView.RPC("UpgradeToolAtToolStationRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06002E37 RID: 11831 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void UpgradeToolAtToolStationRPC(PhotonMessageInfo info)
	{
	}

	// Token: 0x06002E38 RID: 11832 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void LocalEjectToolInUpgradeStation()
	{
	}

	// Token: 0x06002E39 RID: 11833 RVA: 0x000FBEE8 File Offset: 0x000FA0E8
	public void EntityEnteredDropZone(GameEntity entity)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GRUIStationEmployeeBadges employeeBadges = this.reactor.employeeBadges;
		long num = BitPackUtils.PackWorldPosForNetwork(entity.transform.position);
		int num2 = BitPackUtils.PackQuaternionForNetwork(entity.transform.rotation);
		if (entity.gameObject.GetComponent<GRBadge>() != null)
		{
			GRUIEmployeeBadgeDispenser gruiemployeeBadgeDispenser = employeeBadges.badgeDispensers[entity.gameObject.GetComponent<GRBadge>().dispenserIndex];
			if (gruiemployeeBadgeDispenser != null)
			{
				num = BitPackUtils.PackWorldPosForNetwork(gruiemployeeBadgeDispenser.GetSpawnPosition());
				num2 = BitPackUtils.PackQuaternionForNetwork(gruiemployeeBadgeDispenser.GetSpawnRotation());
			}
		}
		this.photonView.RPC("EntityEnteredDropZoneRPC", RpcTarget.All, new object[]
		{
			this.gameEntityManager.GetNetIdFromEntityId(entity.id),
			num,
			num2
		});
	}

	// Token: 0x06002E3A RID: 11834 RVA: 0x000FBFD0 File Offset: 0x000FA1D0
	[PunRPC]
	public void EntityEnteredDropZoneRPC(int entityNetId, long position, int rotation, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, entityNetId) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.EntityEnteredDropZone))
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "EntityEnteredDropZoneRPC");
		Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(position);
		float num = 10000f;
		if (!(in vector).IsValid(in num))
		{
			return;
		}
		Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(rotation);
		if (!(in quaternion).IsValid())
		{
			return;
		}
		if (!this.IsPositionInZone(vector))
		{
			return;
		}
		if ((vector - this.reactor.dropZone.transform.position).magnitude > 5f)
		{
			return;
		}
		this.LocalEntityEnteredDropZone(this.gameEntityManager.GetEntityIdFromNetId(entityNetId), vector, quaternion);
	}

	// Token: 0x06002E3B RID: 11835 RVA: 0x000FC07C File Offset: 0x000FA27C
	private void LocalEntityEnteredDropZone(GameEntityId entityId, Vector3 position, Quaternion rotation)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRDropZone dropZone = this.reactor.dropZone;
		Vector3 vector = dropZone.GetRepelDirectionWorld() * GhostReactor.DROP_ZONE_REPEL;
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityId);
		GamePlayer gamePlayer;
		if (gameEntity.heldByActorNumber >= 0 && GamePlayer.TryGetGamePlayer(gameEntity.heldByActorNumber, out gamePlayer))
		{
			int num = gamePlayer.FindHandIndex(entityId);
			gamePlayer.ClearGrabbedIfHeld(entityId, this.gameEntityManager);
			if (gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				GamePlayerLocal.instance.gamePlayer.ClearGrabbed(num);
				GamePlayerLocal.instance.ClearGrabbed(num);
			}
			gameEntity.heldByActorNumber = -1;
			gameEntity.heldByHandIndex = -1;
			Action onReleased = gameEntity.OnReleased;
			if (onReleased != null)
			{
				onReleased();
			}
		}
		gameEntity.transform.SetParent(null);
		gameEntity.transform.SetLocalPositionAndRotation(position, rotation);
		if (!(gameEntity.gameObject.GetComponent<GRBadge>() != null))
		{
			Rigidbody component = gameEntity.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = false;
				component.position = position;
				component.rotation = rotation;
				component.linearVelocity = vector;
				component.angularVelocity = Vector3.zero;
			}
		}
		dropZone.PlayEffect();
	}

	// Token: 0x06002E3C RID: 11836 RVA: 0x000FC1B0 File Offset: 0x000FA3B0
	public void RequestRecycleScanItem(GameEntityId gameEntityId)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netIdFromEntityId = this.gameEntityManager.GetNetIdFromEntityId(gameEntityId);
		if (netIdFromEntityId == -1)
		{
			return;
		}
		base.SendRPC("ApplyRecycleScanItemRPC", RpcTarget.All, new object[] { netIdFromEntityId });
	}

	// Token: 0x06002E3D RID: 11837 RVA: 0x000FC1F4 File Offset: 0x000FA3F4
	[PunRPC]
	public void ApplyRecycleScanItemRPC(int netId, PhotonMessageInfo info)
	{
		if (!this.IsZoneActive() || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplRecycleScanItem))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(netId);
		this.reactor.recycler.ScanItem(entityIdFromNetId);
	}

	// Token: 0x06002E3E RID: 11838 RVA: 0x000FC248 File Offset: 0x000FA448
	public void RequestRecycleItem(int lastHeldActorNumber, GameEntityId toolId, GRTool.GRToolType toolType)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.gameEntityManager == null)
		{
			return;
		}
		int netIdFromEntityId = this.gameEntityManager.GetNetIdFromEntityId(toolId);
		if (netIdFromEntityId == -1)
		{
			return;
		}
		base.SendRPC("ApplyRecycleItemRPC", RpcTarget.All, new object[] { lastHeldActorNumber, netIdFromEntityId, toolType });
	}

	// Token: 0x06002E3F RID: 11839 RVA: 0x000FC2AC File Offset: 0x000FA4AC
	[PunRPC]
	public void ApplyRecycleItemRPC(int lastHeldActorNumber, int toolNetId, GRTool.GRToolType toolType, PhotonMessageInfo info)
	{
		if (!this.IsZoneActive() || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyRecycleItem) || !this.gameEntityManager.IsEntityNearPosition(toolNetId, this.reactor.recycler.transform.position, 16f))
		{
			return;
		}
		int count = this.reactor.vrRigs.Count;
		Mathf.FloorToInt((float)this.reactor.recycler.GetRecycleValue(toolType) / (float)count);
		ProgressionManager.Instance.RecycleTool(toolType, this.reactor.vrRigs.Count);
		this.reactor.RefreshScoreboards();
		this.reactor.recycler.RecycleItem();
		this.gameEntityManager.DestroyItemLocal(this.gameEntityManager.GetEntityIdFromNetId(toolNetId));
	}

	// Token: 0x06002E40 RID: 11840 RVA: 0x000FC384 File Offset: 0x000FA584
	public void RequestSentientCorePerformJump(GameEntity entity, Vector3 startPos, Vector3 normal, Vector3 direction, float waitTime)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netIdFromEntityId = this.gameEntityManager.GetNetIdFromEntityId(entity.id);
		double num = PhotonNetwork.Time + (double)waitTime;
		base.SendRPC("SentientCorePerformJumpRPC", RpcTarget.All, new object[] { netIdFromEntityId, startPos, normal, direction, num });
	}

	// Token: 0x06002E41 RID: 11841 RVA: 0x000FC3F8 File Offset: 0x000FA5F8
	[PunRPC]
	public void SentientCorePerformJumpRPC(int entityNetId, Vector3 startPosition, Vector3 surfaceNormal, Vector3 jumpDirection, double jumpStartTime, PhotonMessageInfo info)
	{
		if (this.IsValidClientRPC(info.Sender, entityNetId, startPosition) && !this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplySentientCoreDestination))
		{
			float num = 10000f;
			if ((in startPosition).IsValid(in num))
			{
				float num2 = 10000f;
				if ((in surfaceNormal).IsValid(in num2))
				{
					float num3 = 10000f;
					if ((in jumpDirection).IsValid(in num3) && double.IsFinite(jumpStartTime) && PhotonNetwork.Time - jumpStartTime <= 5.0 && this.gameEntityManager.IsEntityNearPosition(entityNetId, startPosition, 16f))
					{
						GameEntity gameEntity = this.gameEntityManager.GetGameEntity(this.gameEntityManager.GetEntityIdFromNetId(entityNetId));
						if (gameEntity == null)
						{
							return;
						}
						GRSentientCore component = gameEntity.GetComponent<GRSentientCore>();
						if (component == null)
						{
							return;
						}
						component.PerformJump(startPosition, surfaceNormal, jumpDirection, jumpStartTime);
						return;
					}
				}
			}
		}
	}

	// Token: 0x06002E42 RID: 11842 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06002E43 RID: 11843 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06002E44 RID: 11844 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002E45 RID: 11845 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002E46 RID: 11846 RVA: 0x000FC4C9 File Offset: 0x000FA6C9
	protected void OnNewPlayerEnteredGhostReactor()
	{
		if (this.reactor == null)
		{
			return;
		}
		this.reactor.VRRigRefresh();
	}

	// Token: 0x06002E47 RID: 11847 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityZoneClear(GTZone zoneId)
	{
	}

	// Token: 0x06002E48 RID: 11848 RVA: 0x000FC4E8 File Offset: 0x000FA6E8
	public void OnZoneCreate()
	{
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (this.reactor.zone == GTZone.customMaps)
		{
			return;
		}
		int num = this.reactor.PickLevelConfigForDepth(grplayer.shuttleData.targetLevel);
		this.reactor.SetNextDelveDepth(grplayer.shuttleData.targetLevel, num);
		this.reactor.DelveToNextDepth();
		if (this.reactor.shiftManager != null)
		{
			this.reactor.shiftManager.SetState(GhostReactorShiftManager.State.WaitingForConnect, true);
		}
	}

	// Token: 0x06002E49 RID: 11849 RVA: 0x000FC580 File Offset: 0x000FA780
	public void OnZoneInit()
	{
		if (this.reactor == null)
		{
			return;
		}
		if (this.reactor.zone == GTZone.customMaps)
		{
			return;
		}
		this.reactor.VRRigRefresh();
		if (this.reactor.employeeTerminal != null)
		{
			this.reactor.employeeTerminal.Setup();
		}
		if (GRPlayer.Get(NetworkSystem.Instance.LocalPlayer.ActorNumber) != null)
		{
			this.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SetPodLevel, this.reactor.toolProgression.GetDropPodLevel());
			this.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SetPodChassisLevel, this.reactor.toolProgression.GetDropPodChasisLevel());
		}
	}

	// Token: 0x06002E4A RID: 11850 RVA: 0x000FC628 File Offset: 0x000FA828
	public void OnZoneClear(ZoneClearReason reason)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer component = GamePlayerLocal.instance.gamePlayer.GetComponent<GRPlayer>();
		if (component != null)
		{
			GRBadge badge = component.badge;
			if (badge != null && badge.IsAttachedToPlayer())
			{
				component.lastLeftWithBadgeAttachedTime = Time.timeAsDouble;
			}
			component.SendGameEndedTelemetry(false, reason);
		}
		if (this.reactor.levelGenerator != null)
		{
			this.reactor.levelGenerator.ClearLevelSections();
		}
		if (this.reactor.shiftManager != null)
		{
			this.reactor.shiftManager.OnShiftEnded(0.0, false, reason);
		}
		GRPlayer grplayer = GRPlayer.Get(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		if (grplayer != null)
		{
			grplayer.SetGooParticleSystemEnabled(false, false);
			grplayer.SetGooParticleSystemEnabled(true, false);
		}
	}

	// Token: 0x06002E4B RID: 11851 RVA: 0x000FC709 File Offset: 0x000FA909
	public bool IsZoneReady()
	{
		return this.reactor != null;
	}

	// Token: 0x06002E4C RID: 11852 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool ShouldClearZone()
	{
		return true;
	}

	// Token: 0x06002E4D RID: 11853 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnCreateGameEntity(GameEntity entity)
	{
	}

	// Token: 0x06002E4E RID: 11854 RVA: 0x000FC718 File Offset: 0x000FA918
	public void SerializeZoneData(BinaryWriter writer)
	{
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		GRUIPromotionBot promotionBot = this.reactor.promotionBot;
		GRUIScoreboard[] array = this.reactor.scoreboards.ToArray();
		writer.Write(this.reactor.depthLevel);
		writer.Write(this.reactor.depthConfigIndex);
		writer.Write(this.reactor.difficultyScalingForCurrentFloor);
		if (shiftManager != null)
		{
			writer.Write(shiftManager.ShiftActive);
			writer.Write(shiftManager.ShiftStartNetworkTime);
			shiftManager.shiftStats.Serialize(writer);
			writer.Write(shiftManager.ShiftId);
			writer.Write(shiftManager.stateStartTime);
			writer.Write((byte)shiftManager.GetState());
			writer.Write(levelGenerator.seed);
		}
		if (promotionBot != null)
		{
			writer.Write(promotionBot.GetCurrentPlayerActorNumber());
			writer.Write((int)promotionBot.currentState);
		}
		for (int i = 0; i < array.Length; i++)
		{
			writer.Write((int)array[i].currentScreen);
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		writer.Write(toolPurchasingStations.Count);
		for (int j = 0; j < toolPurchasingStations.Count; j++)
		{
			writer.Write(toolPurchasingStations[j].ActiveEntryIndex);
		}
		List<GRToolUpgradePurchaseStationFull> toolUpgradePurchaseStationsFull = this.reactor.toolUpgradePurchaseStationsFull;
		writer.Write(toolUpgradePurchaseStationsFull.Count);
		for (int k = 0; k < toolUpgradePurchaseStationsFull.Count; k++)
		{
			writer.Write(toolUpgradePurchaseStationsFull[k].SelectedShelf);
			writer.Write(toolUpgradePurchaseStationsFull[k].SelectedItem);
			writer.Write(toolUpgradePurchaseStationsFull[k].currentActivePlayerActorNumber);
		}
		List<GhostReactor.EntityTypeRespawnTracker> respawnQueue = this.reactor.respawnQueue;
		writer.Write(this.reactor.respawnQueue.Count);
		for (int l = 0; l < respawnQueue.Count; l++)
		{
			writer.Write(respawnQueue[l].entityTypeID);
			writer.Write(respawnQueue[l].entityCreateData);
			writer.Write(respawnQueue[l].entityNextRespawnTime);
		}
		bool flag = false;
		writer.Write(flag);
	}

	// Token: 0x06002E4F RID: 11855 RVA: 0x000FC960 File Offset: 0x000FAB60
	public void DeserializeZoneData(BinaryReader reader)
	{
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		GRUIPromotionBot promotionBot = this.reactor.promotionBot;
		GRUIScoreboard[] array = this.reactor.scoreboards.ToArray();
		int num = reader.ReadInt32();
		this.reactor.depthLevel = num;
		int num2 = reader.ReadInt32();
		this.reactor.depthConfigIndex = num2;
		float num3 = reader.ReadSingle();
		this.reactor.difficultyScalingForCurrentFloor = num3;
		if (shiftManager != null)
		{
			bool flag = reader.ReadBoolean();
			double num4 = reader.ReadDouble();
			shiftManager.shiftStats.Deserialize(reader);
			shiftManager.RefreshShiftStatsDisplay();
			string text = reader.ReadString();
			shiftManager.SetShiftId(text);
			shiftManager.stateStartTime = reader.ReadDouble();
			GhostReactorShiftManager.State state = (GhostReactorShiftManager.State)reader.ReadByte();
			shiftManager.SetState(state, true);
			int num5 = reader.ReadInt32();
			if (flag)
			{
				levelGenerator.Generate(num5);
				shiftManager.OnShiftStarted(text, num4, false, true);
				this.reactor.ClearAllHandprints();
			}
		}
		if (promotionBot != null)
		{
			int num6 = reader.ReadInt32();
			int num7 = reader.ReadInt32();
			promotionBot.SetActivePlayerStateChange(num6, num7);
		}
		for (int i = 0; i < array.Length; i++)
		{
			array[i].currentScreen = (GRUIScoreboard.ScoreboardScreen)reader.ReadInt32();
		}
		this.reactor.RefreshScoreboards();
		this.reactor.RefreshDepth();
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		int num8 = reader.ReadInt32();
		for (int j = 0; j < num8; j++)
		{
			int num9 = reader.ReadInt32();
			if (j < toolPurchasingStations.Count && toolPurchasingStations[j] != null)
			{
				toolPurchasingStations[j].OnSelectionUpdate(num9);
			}
		}
		List<GRToolUpgradePurchaseStationFull> toolUpgradePurchaseStationsFull = this.reactor.toolUpgradePurchaseStationsFull;
		int num10 = reader.ReadInt32();
		for (int k = 0; k < num10; k++)
		{
			int num11 = reader.ReadInt32();
			int num12 = reader.ReadInt32();
			int num13 = reader.ReadInt32();
			if (k < toolUpgradePurchaseStationsFull.Count && toolUpgradePurchaseStationsFull[k] != null)
			{
				toolUpgradePurchaseStationsFull[k].SetSelectedShelfAndItem(num11, num12, true);
				toolUpgradePurchaseStationsFull[k].SetActivePlayer(num13);
			}
		}
		List<GhostReactor.EntityTypeRespawnTracker> respawnQueue = this.reactor.respawnQueue;
		respawnQueue.Clear();
		int num14 = reader.ReadInt32();
		for (int l = 0; l < num14; l++)
		{
			respawnQueue.Add(new GhostReactor.EntityTypeRespawnTracker
			{
				entityTypeID = reader.ReadInt32(),
				entityCreateData = reader.ReadInt64(),
				entityNextRespawnTime = reader.ReadSingle()
			});
		}
		reader.ReadBoolean();
		this.reactor.VRRigRefresh();
	}

	// Token: 0x06002E50 RID: 11856 RVA: 0x000FCC07 File Offset: 0x000FAE07
	public long ProcessMigratedGameEntityCreateData(GameEntity entity, long createData)
	{
		return createData;
	}

	// Token: 0x06002E51 RID: 11857 RVA: 0x00002076 File Offset: 0x00000276
	public bool ValidateMigratedGameEntity(int netId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int actorNr)
	{
		return false;
	}

	// Token: 0x06002E52 RID: 11858 RVA: 0x000FCC0A File Offset: 0x000FAE0A
	public bool ValidateCreateMultipleItems(int zoneId, byte[] compressedStateData, int EntityCount)
	{
		return EntityCount <= 128;
	}

	// Token: 0x06002E53 RID: 11859 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool ValidateCreateItem(int nedId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int createdByEntityNetId)
	{
		return true;
	}

	// Token: 0x06002E54 RID: 11860 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool ValidateCreateItemBatchSize(int size)
	{
		return true;
	}

	// Token: 0x06002E55 RID: 11861 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void SerializeZoneEntityData(BinaryWriter writer, GameEntity entity)
	{
	}

	// Token: 0x06002E56 RID: 11862 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void DeserializeZoneEntityData(BinaryReader reader, GameEntity entity)
	{
	}

	// Token: 0x06002E57 RID: 11863 RVA: 0x000FCC18 File Offset: 0x000FAE18
	public void OnTapLocal(bool isLeftHand, Vector3 pos, Quaternion rot, GorillaSurfaceOverride surfaceOverride, Vector3 handVelocity)
	{
		if (this.reactor != null)
		{
			this.reactor.OnTapLocal(isLeftHand, pos, rot, surfaceOverride);
		}
		if (this.IsAuthority())
		{
			float num = Math.Clamp(handVelocity.magnitude / 8f, 0f, 1f);
			if (num > 0.25f)
			{
				GRNoiseEventManager.instance.AddNoiseEvent(pos, num, 1f);
			}
		}
	}

	// Token: 0x06002E58 RID: 11864 RVA: 0x000FCC84 File Offset: 0x000FAE84
	public void OnSharedTap(VRRig rig, Vector3 tapPos, float handTapSpeed)
	{
		if (this.IsAuthority())
		{
			float num = Math.Clamp(handTapSpeed / 8f, 0f, 1f);
			if (num > 0.25f)
			{
				GRNoiseEventManager.instance.AddNoiseEvent(tapPos, num, 1f);
			}
		}
	}

	// Token: 0x06002E59 RID: 11865 RVA: 0x000FCCCC File Offset: 0x000FAECC
	public void SerializeZonePlayerData(BinaryWriter writer, int actorNumber)
	{
		GRPlayer grplayer = GRPlayer.Get(actorNumber);
		grplayer.SerializeNetworkState(writer, grplayer.gamePlayer.rig.OwningNetPlayer);
	}

	// Token: 0x06002E5A RID: 11866 RVA: 0x000FCCF8 File Offset: 0x000FAEF8
	public void DeserializeZonePlayerData(BinaryReader reader, int actorNumber)
	{
		GRPlayer grplayer = GRPlayer.Get(actorNumber);
		GRPlayer.DeserializeNetworkStateAndBurn(reader, grplayer, this);
	}

	// Token: 0x06002E5B RID: 11867 RVA: 0x00002076 File Offset: 0x00000276
	public bool DebugIsToolStationHacked()
	{
		return false;
	}

	// Token: 0x17000474 RID: 1140
	// (get) Token: 0x06002E5C RID: 11868 RVA: 0x00002076 File Offset: 0x00000276
	public static bool AggroDisabled
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002E5F RID: 11871 RVA: 0x00002E6F File Offset: 0x0000106F
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06002E60 RID: 11872 RVA: 0x00002E7B File Offset: 0x0000107B
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04003AA4 RID: 15012
	private const string EVENT_CORE_COLLECTED = "GRCollectCore";

	// Token: 0x04003AA5 RID: 15013
	private const string EVENT_ENEMY_KILLED = "GRKillEnemy";

	// Token: 0x04003AA6 RID: 15014
	public const string EVENT_BREAKABLE_BROKEN = "GRSmashBreakable";

	// Token: 0x04003AA7 RID: 15015
	public const string EVENT_ENEMY_ARMOR_BREAK = "GRArmorBreak";

	// Token: 0x04003AA8 RID: 15016
	public const string NETWORK_ROOM_GR_DEPTH = "ghostReactorDepth";

	// Token: 0x04003AA9 RID: 15017
	public const int GHOSTREACTOR_ZONE_ID = 5;

	// Token: 0x04003AAA RID: 15018
	public const GTZone GT_ZONE_GHOSTREACTOR = GTZone.ghostReactor;

	// Token: 0x04003AAB RID: 15019
	public GameEntityManager gameEntityManager;

	// Token: 0x04003AAC RID: 15020
	public GameAgentManager gameAgentManager;

	// Token: 0x04003AAD RID: 15021
	public GRNoiseEventManager noiseEventManager;

	// Token: 0x04003AAE RID: 15022
	public PhotonView photonView;

	// Token: 0x04003AAF RID: 15023
	public GhostReactor reactor;

	// Token: 0x04003AB0 RID: 15024
	public CallLimitersList<CallLimiter, GhostReactorManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GhostReactorManager.RPC>();

	// Token: 0x04003AB1 RID: 15025
	private const float HandprintThrottleTime = 0.25f;

	// Token: 0x04003AB2 RID: 15026
	private float LastHandprintTime;

	// Token: 0x04003AB3 RID: 15027
	private Coroutine activeSpawnSectionEntitiesCoroutine;

	// Token: 0x04003AB4 RID: 15028
	private WaitForSeconds spawnSectionEntitiesWait = new WaitForSeconds(0.1f);

	// Token: 0x04003AB5 RID: 15029
	private static List<GameEntityId> tempEntitiesToDestroy = new List<GameEntityId>();

	// Token: 0x04003AB6 RID: 15030
	private GameEntity cachedBossEntity;

	// Token: 0x04003AB7 RID: 15031
	public GRToolUpgradeStation upgradeStation;

	// Token: 0x04003AB8 RID: 15032
	public static bool entityDebugEnabled = false;

	// Token: 0x04003AB9 RID: 15033
	public static bool noiseDebugEnabled = false;

	// Token: 0x04003ABA RID: 15034
	public static bool bayUnlockEnabled = false;

	// Token: 0x0200071B RID: 1819
	public enum RPC
	{
		// Token: 0x04003ABC RID: 15036
		ApplyCollectItem,
		// Token: 0x04003ABD RID: 15037
		ApplyChargeTool,
		// Token: 0x04003ABE RID: 15038
		ApplyDepositCurrency,
		// Token: 0x04003ABF RID: 15039
		ApplyPlayerRevived,
		// Token: 0x04003AC0 RID: 15040
		GrantPlayerShield,
		// Token: 0x04003AC1 RID: 15041
		RequestFireProjectile,
		// Token: 0x04003AC2 RID: 15042
		ApplyShiftStart,
		// Token: 0x04003AC3 RID: 15043
		ApplyShiftEnd,
		// Token: 0x04003AC4 RID: 15044
		ToolPurchaseResponse,
		// Token: 0x04003AC5 RID: 15045
		ApplyBreakableBroken,
		// Token: 0x04003AC6 RID: 15046
		EntityEnteredDropZone,
		// Token: 0x04003AC7 RID: 15047
		PromotionBotResponse,
		// Token: 0x04003AC8 RID: 15048
		DistillItem,
		// Token: 0x04003AC9 RID: 15049
		ApplySentientCoreDestination,
		// Token: 0x04003ACA RID: 15050
		Handprint,
		// Token: 0x04003ACB RID: 15051
		ApplyRecycleItem,
		// Token: 0x04003ACC RID: 15052
		ApplRecycleScanItem,
		// Token: 0x04003ACD RID: 15053
		SeedExtractorAction,
		// Token: 0x04003ACE RID: 15054
		ToolUpgradeStationAction,
		// Token: 0x04003ACF RID: 15055
		SendMothershipId,
		// Token: 0x04003AD0 RID: 15056
		RefreshShiftCredit
	}

	// Token: 0x0200071C RID: 1820
	public enum GRPlayerAction
	{
		// Token: 0x04003AD2 RID: 15058
		ButtonShiftStart,
		// Token: 0x04003AD3 RID: 15059
		DelveDeeper,
		// Token: 0x04003AD4 RID: 15060
		DelveState,
		// Token: 0x04003AD5 RID: 15061
		ShuttleOpen,
		// Token: 0x04003AD6 RID: 15062
		ShuttleClose,
		// Token: 0x04003AD7 RID: 15063
		ShuttleLaunch,
		// Token: 0x04003AD8 RID: 15064
		ShuttleArrive,
		// Token: 0x04003AD9 RID: 15065
		ShuttleTargetLevelUp,
		// Token: 0x04003ADA RID: 15066
		ShuttleTargetLevelDown,
		// Token: 0x04003ADB RID: 15067
		SetPodLevel,
		// Token: 0x04003ADC RID: 15068
		SetPodChassisLevel,
		// Token: 0x04003ADD RID: 15069
		SeedExtractorOpenStation,
		// Token: 0x04003ADE RID: 15070
		SeedExtractorCloseStation,
		// Token: 0x04003ADF RID: 15071
		SeedExtractorCardSwipeFail,
		// Token: 0x04003AE0 RID: 15072
		SeedExtractorTryDepositSeed,
		// Token: 0x04003AE1 RID: 15073
		SeedExtractorDepositSeedSucceeded,
		// Token: 0x04003AE2 RID: 15074
		SeedExtractorDepositSeedFailed,
		// Token: 0x04003AE3 RID: 15075
		DEBUG_ResetDepth,
		// Token: 0x04003AE4 RID: 15076
		DEBUG_DelveDeeper,
		// Token: 0x04003AE5 RID: 15077
		DEBUG_DelveShallower
	}

	// Token: 0x0200071D RID: 1821
	public enum ToolPurchaseActionV2
	{
		// Token: 0x04003AE7 RID: 15079
		RequestPurchaseAuthority,
		// Token: 0x04003AE8 RID: 15080
		SelectShelfAndItem,
		// Token: 0x04003AE9 RID: 15081
		NotifyPurchaseFail,
		// Token: 0x04003AEA RID: 15082
		NotifyPurchaseSuccess,
		// Token: 0x04003AEB RID: 15083
		RequestStationExclusivityAuthority,
		// Token: 0x04003AEC RID: 15084
		SetToolStationActivePlayer,
		// Token: 0x04003AED RID: 15085
		SetHandleAndSelectionWheelPosition,
		// Token: 0x04003AEE RID: 15086
		SetToolStationHackedDebug
	}

	// Token: 0x0200071E RID: 1822
	public enum ToolPurchaseStationAction
	{
		// Token: 0x04003AF0 RID: 15088
		ShiftLeft,
		// Token: 0x04003AF1 RID: 15089
		ShiftRight,
		// Token: 0x04003AF2 RID: 15090
		TryPurchase
	}

	// Token: 0x0200071F RID: 1823
	public enum ToolPurchaseStationResponse
	{
		// Token: 0x04003AF4 RID: 15092
		SelectionUpdate,
		// Token: 0x04003AF5 RID: 15093
		PurchaseSucceeded,
		// Token: 0x04003AF6 RID: 15094
		PurchaseFailed
	}
}
