using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020006B6 RID: 1718
[NetworkBehaviourWeaved(0)]
public class GameAgentManager : NetworkComponent, ITickSystemTick
{
	// Token: 0x17000447 RID: 1095
	// (get) Token: 0x06002ADD RID: 10973 RVA: 0x000E649D File Offset: 0x000E469D
	// (set) Token: 0x06002ADE RID: 10974 RVA: 0x000E64A5 File Offset: 0x000E46A5
	public bool TickRunning { get; set; }

	// Token: 0x06002ADF RID: 10975 RVA: 0x000E64B0 File Offset: 0x000E46B0
	protected override void Awake()
	{
		this.agents = new List<GameAgent>(128);
		this.netIdsForDestination = new List<int>();
		this.destinationsForDestination = new List<Vector3>();
		this.netIdsForState = new List<int>();
		this.statesForState = new List<byte>();
		this.netIdsForBehavior = new List<int>();
		this.behaviorsForBehavior = new List<byte>();
		this.nextAgentIndexUpdate = 0;
		this.nextAgentIndexThink = 0;
	}

	// Token: 0x06002AE0 RID: 10976 RVA: 0x000E651D File Offset: 0x000E471D
	private new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		TickSystem<object>.AddCallbackTarget(this);
	}

	// Token: 0x06002AE1 RID: 10977 RVA: 0x000E652B File Offset: 0x000E472B
	private new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		TickSystem<object>.RemoveCallbackTarget(this);
	}

	// Token: 0x06002AE2 RID: 10978 RVA: 0x000E6539 File Offset: 0x000E4739
	public static GameAgentManager Get(GameEntity gameEntity)
	{
		if (!(gameEntity == null) && !(gameEntity.manager == null))
		{
			return gameEntity.manager.gameAgentManager;
		}
		return null;
	}

	// Token: 0x06002AE3 RID: 10979 RVA: 0x000E655F File Offset: 0x000E475F
	public List<GameAgent> GetAgents()
	{
		return this.agents;
	}

	// Token: 0x06002AE4 RID: 10980 RVA: 0x000E6567 File Offset: 0x000E4767
	public int GetGameAgentCount()
	{
		return this.agents.Count;
	}

	// Token: 0x06002AE5 RID: 10981 RVA: 0x000E6574 File Offset: 0x000E4774
	public void AddGameAgent(GameAgent gameAgent)
	{
		this.agents.Add(gameAgent);
	}

	// Token: 0x06002AE6 RID: 10982 RVA: 0x000E6582 File Offset: 0x000E4782
	public void RemoveGameAgent(GameAgent gameAgent)
	{
		this.agents.Remove(gameAgent);
	}

	// Token: 0x06002AE7 RID: 10983 RVA: 0x000E6591 File Offset: 0x000E4791
	public GameAgent GetGameAgent(GameEntityId id)
	{
		return this.entityManager.GetGameEntity(id).GetComponent<GameAgent>();
	}

	// Token: 0x06002AE8 RID: 10984 RVA: 0x000E65A4 File Offset: 0x000E47A4
	public void Tick()
	{
		if (this.IsAuthority())
		{
			int num = Mathf.Min(1, this.agents.Count);
			for (int i = 0; i < num; i++)
			{
				if (this.nextAgentIndexThink >= this.agents.Count)
				{
					this.nextAgentIndexThink = 0;
				}
				this.agents[this.nextAgentIndexThink].OnThink(Time.deltaTime);
				this.nextAgentIndexThink++;
			}
		}
		for (int j = 0; j < this.agents.Count; j++)
		{
			if (this.agents[j] != null)
			{
				this.agents[j].OnUpdate();
			}
		}
		if (this.IsAuthority())
		{
			if (this.netIdsForDestination.Count > 0 && Time.time > this.lastDestinationSentTime + this.destinationCooldown)
			{
				this.lastDestinationSentTime = Time.time;
				base.SendRPC("ApplyDestinationRPC", RpcTarget.All, new object[]
				{
					this.netIdsForDestination.ToArray(),
					this.destinationsForDestination.ToArray()
				});
				this.netIdsForDestination.Clear();
				this.destinationsForDestination.Clear();
			}
			if (this.netIdsForState.Count > 0 && Time.time > this.lastStateSentTime + this.stateCooldown)
			{
				this.lastStateSentTime = Time.time;
				base.SendRPC("ApplyStateRPC", RpcTarget.All, new object[]
				{
					this.netIdsForState.ToArray(),
					this.statesForState.ToArray()
				});
				this.netIdsForState.Clear();
				this.statesForState.Clear();
			}
			if (this.netIdsForBehavior.Count > 0 && Time.time > this.lastBehaviorSentTime + this.behaviorCooldown)
			{
				this.lastBehaviorSentTime = Time.time;
				base.SendRPC("ApplyBehaviorRPC", RpcTarget.All, new object[]
				{
					this.netIdsForBehavior.ToArray(),
					this.behaviorsForBehavior.ToArray()
				});
				this.netIdsForBehavior.Clear();
				this.behaviorsForBehavior.Clear();
			}
		}
	}

	// Token: 0x06002AE9 RID: 10985 RVA: 0x000E67B3 File Offset: 0x000E49B3
	public bool IsAuthority()
	{
		return this.entityManager.IsAuthority();
	}

	// Token: 0x06002AEA RID: 10986 RVA: 0x000E67C0 File Offset: 0x000E49C0
	public bool IsAuthorityPlayer(NetPlayer player)
	{
		return this.entityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x06002AEB RID: 10987 RVA: 0x000E67CE File Offset: 0x000E49CE
	public bool IsAuthorityPlayer(Player player)
	{
		return this.entityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x06002AEC RID: 10988 RVA: 0x000E67DC File Offset: 0x000E49DC
	public Player GetAuthorityPlayer()
	{
		return this.entityManager.GetAuthorityPlayer();
	}

	// Token: 0x06002AED RID: 10989 RVA: 0x000E67E9 File Offset: 0x000E49E9
	public bool IsZoneActive()
	{
		return this.entityManager.IsZoneActive();
	}

	// Token: 0x06002AEE RID: 10990 RVA: 0x000E67F6 File Offset: 0x000E49F6
	public bool IsPositionInManagerBounds(Vector3 pos)
	{
		return this.entityManager.IsPositionInManagerBounds(pos);
	}

	// Token: 0x06002AEF RID: 10991 RVA: 0x000E6804 File Offset: 0x000E4A04
	public bool IsValidClientRPC(Player sender)
	{
		return this.entityManager.IsValidClientRPC(sender);
	}

	// Token: 0x06002AF0 RID: 10992 RVA: 0x000E6812 File Offset: 0x000E4A12
	public bool IsValidClientRPC(Player sender, int entityNetId)
	{
		return this.entityManager.IsValidClientRPC(sender, entityNetId);
	}

	// Token: 0x06002AF1 RID: 10993 RVA: 0x000E6821 File Offset: 0x000E4A21
	public bool IsValidClientRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.entityManager.IsValidClientRPC(sender, entityNetId, pos);
	}

	// Token: 0x06002AF2 RID: 10994 RVA: 0x000E6831 File Offset: 0x000E4A31
	public bool IsValidClientRPC(Player sender, Vector3 pos)
	{
		return this.entityManager.IsValidClientRPC(sender, pos);
	}

	// Token: 0x06002AF3 RID: 10995 RVA: 0x000E6840 File Offset: 0x000E4A40
	public bool IsValidAuthorityRPC(Player sender)
	{
		return this.entityManager.IsValidAuthorityRPC(sender);
	}

	// Token: 0x06002AF4 RID: 10996 RVA: 0x000E684E File Offset: 0x000E4A4E
	public bool IsValidAuthorityRPC(Player sender, int entityNetId)
	{
		return this.entityManager.IsValidAuthorityRPC(sender, entityNetId);
	}

	// Token: 0x06002AF5 RID: 10997 RVA: 0x000E685D File Offset: 0x000E4A5D
	public bool IsValidAuthorityRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.entityManager.IsValidAuthorityRPC(sender, entityNetId, pos);
	}

	// Token: 0x06002AF6 RID: 10998 RVA: 0x000E686D File Offset: 0x000E4A6D
	public bool IsValidAuthorityRPC(Player sender, Vector3 pos)
	{
		return this.entityManager.IsValidAuthorityRPC(sender, pos);
	}

	// Token: 0x06002AF7 RID: 10999 RVA: 0x000E687C File Offset: 0x000E4A7C
	public void RequestDestination(GameAgent agent, Vector3 dest)
	{
		if (!this.IsAuthority())
		{
			Debug.LogError("RequestDestination should only be called from the master client");
			return;
		}
		int netIdFromEntityId = this.entityManager.GetNetIdFromEntityId(agent.entity.id);
		if (this.netIdsForDestination.Contains(netIdFromEntityId))
		{
			this.destinationsForDestination[this.netIdsForDestination.IndexOf(netIdFromEntityId)] = dest;
			return;
		}
		this.netIdsForDestination.Add(netIdFromEntityId);
		this.destinationsForDestination.Add(dest);
	}

	// Token: 0x06002AF8 RID: 11000 RVA: 0x000E68F4 File Offset: 0x000E4AF4
	[PunRPC]
	public void ApplyDestinationRPC(int[] netEntityId, Vector3[] dest, PhotonMessageInfo info)
	{
		if (!this.IsZoneActive() || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyDestination))
		{
			return;
		}
		if (netEntityId == null || dest == null || netEntityId.Length != dest.Length)
		{
			return;
		}
		int i = 0;
		while (i < netEntityId.Length)
		{
			if (this.IsValidClientRPC(info.Sender, netEntityId[i], dest[i]))
			{
				int num = i;
				float num2 = 10000f;
				if ((in dest[num]).IsValid(in num2))
				{
					i++;
					continue;
				}
			}
			return;
		}
		for (int j = 0; j < netEntityId.Length; j++)
		{
			GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(netEntityId[j]));
			if (gameEntity == null)
			{
				return;
			}
			GameAgent component = gameEntity.GetComponent<GameAgent>();
			if (component == null)
			{
				return;
			}
			component.ApplyDestination(dest[j]);
		}
	}

	// Token: 0x06002AF9 RID: 11001 RVA: 0x000E69B8 File Offset: 0x000E4BB8
	public void RequestState(GameAgent agent, byte state)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netIdFromEntityId = this.entityManager.GetNetIdFromEntityId(agent.entity.id);
		if (this.netIdsForState.Contains(netIdFromEntityId))
		{
			this.statesForState[this.netIdsForState.IndexOf(netIdFromEntityId)] = state;
			return;
		}
		this.netIdsForState.Add(netIdFromEntityId);
		this.statesForState.Add(state);
	}

	// Token: 0x06002AFA RID: 11002 RVA: 0x000E6A24 File Offset: 0x000E4C24
	[PunRPC]
	public void ApplyStateRPC(int[] netEntityId, byte[] state, PhotonMessageInfo info)
	{
		if (netEntityId == null || state == null || netEntityId.Length != state.Length || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyState))
		{
			return;
		}
		for (int i = 0; i < netEntityId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, netEntityId[i]))
			{
				return;
			}
			GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(netEntityId[i]));
			if (gameEntity == null)
			{
				return;
			}
			GameAgent component = gameEntity.GetComponent<GameAgent>();
			if (component == null)
			{
				return;
			}
			component.OnBodyStateChanged(state[i]);
		}
	}

	// Token: 0x06002AFB RID: 11003 RVA: 0x000E6AAC File Offset: 0x000E4CAC
	public void RequestBehavior(GameAgent agent, byte behavior)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netIdFromEntityId = this.entityManager.GetNetIdFromEntityId(agent.entity.id);
		if (this.netIdsForBehavior.Contains(netIdFromEntityId))
		{
			this.behaviorsForBehavior[this.netIdsForBehavior.IndexOf(netIdFromEntityId)] = behavior;
			return;
		}
		this.netIdsForBehavior.Add(netIdFromEntityId);
		this.behaviorsForBehavior.Add(behavior);
	}

	// Token: 0x06002AFC RID: 11004 RVA: 0x000E6B18 File Offset: 0x000E4D18
	[PunRPC]
	public void ApplyBehaviorRPC(int[] netEntityId, byte[] behavior, PhotonMessageInfo info)
	{
		if (netEntityId == null || behavior == null || netEntityId.Length != behavior.Length || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyBehaviour))
		{
			return;
		}
		for (int i = 0; i < netEntityId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, netEntityId[i]))
			{
				return;
			}
			GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(netEntityId[i]));
			if (gameEntity == null)
			{
				return;
			}
			GameAgent component = gameEntity.GetComponent<GameAgent>();
			if (component != null)
			{
				component.OnBehaviorStateChanged(behavior[i]);
			}
		}
	}

	// Token: 0x06002AFD RID: 11005 RVA: 0x000E6BA0 File Offset: 0x000E4DA0
	public void RequestTarget(GameAgent agent, NetPlayer player)
	{
		if (player == agent.targetPlayer)
		{
			return;
		}
		if (!this.IsAuthority())
		{
			return;
		}
		if (agent == null)
		{
			return;
		}
		agent.targetPlayer = player;
		base.SendRPC("ApplyTargetRPC", RpcTarget.Others, new object[]
		{
			this.entityManager.GetNetIdFromEntityId(agent.entity.id),
			(player == null) ? null : player.GetPlayerRef()
		});
	}

	// Token: 0x06002AFE RID: 11006 RVA: 0x000E6C10 File Offset: 0x000E4E10
	[PunRPC]
	public void ApplyTargetRPC(int agentNetId, Player player, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, agentNetId) || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyTarget) || player == null)
		{
			return;
		}
		GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(agentNetId));
		if (gameEntity == null)
		{
			return;
		}
		GameAgent component = gameEntity.GetComponent<GameAgent>();
		if (component == null)
		{
			return;
		}
		component.targetPlayer = NetPlayer.Get(player);
	}

	// Token: 0x06002AFF RID: 11007 RVA: 0x000E6C80 File Offset: 0x000E4E80
	public void RequestJump(GameAgent agent, Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (agent == null)
		{
			return;
		}
		agent.OnJumpRequested(start, end, heightScale, speedScale);
		base.SendRPC("ApplyJumpRPC", RpcTarget.Others, new object[]
		{
			this.entityManager.GetNetIdFromEntityId(agent.entity.id),
			start,
			end,
			heightScale,
			speedScale
		});
	}

	// Token: 0x06002B00 RID: 11008 RVA: 0x000E6D04 File Offset: 0x000E4F04
	[PunRPC]
	public void ApplyJumpRPC(int agentNetId, Vector3 start, Vector3 end, float heightScale, float speedScale, PhotonMessageInfo info)
	{
		if (this.IsValidClientRPC(info.Sender, agentNetId) && !this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyTarget))
		{
			float num = 10000f;
			if ((in start).IsValid(in num))
			{
				float num2 = 10000f;
				if ((in end).IsValid(in num2) && this.entityManager.IsPositionInManagerBounds(start) && this.entityManager.IsPositionInManagerBounds(end) && this.entityManager.IsEntityNearPosition(agentNetId, start, 16f) && heightScale <= 5f && speedScale <= 5f)
				{
					if ((end - start).sqrMagnitude > 625f)
					{
						return;
					}
					GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(agentNetId));
					if (gameEntity == null)
					{
						return;
					}
					GameAgent component = gameEntity.GetComponent<GameAgent>();
					if (component == null)
					{
						return;
					}
					component.OnJumpRequested(start, end, heightScale, speedScale);
					return;
				}
			}
		}
	}

	// Token: 0x06002B01 RID: 11009 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06002B02 RID: 11010 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06002B03 RID: 11011 RVA: 0x000E6DEC File Offset: 0x000E4FEC
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		int num = Mathf.Min(4, this.agents.Count);
		stream.SendNext(num);
		for (int i = 0; i < num; i++)
		{
			if (this.nextAgentIndexUpdate >= this.agents.Count)
			{
				this.nextAgentIndexUpdate = 0;
			}
			stream.SendNext(this.entityManager.GetNetIdFromEntityId(this.agents[this.nextAgentIndexUpdate].entity.id));
			long num2 = BitPackUtils.PackWorldPosForNetwork(this.agents[this.nextAgentIndexUpdate].transform.position);
			stream.SendNext(num2);
			int num3 = BitPackUtils.PackQuaternionForNetwork(this.agents[this.nextAgentIndexUpdate].transform.rotation);
			stream.SendNext(num3);
			this.nextAgentIndexUpdate++;
		}
	}

	// Token: 0x06002B04 RID: 11012 RVA: 0x000E6EDC File Offset: 0x000E50DC
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender))
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		for (int i = 0; i < num; i++)
		{
			int num2 = (int)stream.ReceiveNext();
			Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork((long)stream.ReceiveNext());
			Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork((int)stream.ReceiveNext());
			if (this.IsPositionInManagerBounds(vector) && this.entityManager.IsValidNetId(num2))
			{
				GameEntityId entityIdFromNetId = this.entityManager.GetEntityIdFromNetId(num2);
				GameAgent gameAgent = this.GetGameAgent(entityIdFromNetId);
				if (gameAgent != null)
				{
					gameAgent.ApplyNetworkUpdate(vector, quaternion);
				}
			}
		}
	}

	// Token: 0x06002B06 RID: 11014 RVA: 0x00002E6F File Offset: 0x0000106F
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06002B07 RID: 11015 RVA: 0x00002E7B File Offset: 0x0000107B
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x040037B7 RID: 14263
	public const float MAX_JUMP_DISTANCE = 25f;

	// Token: 0x040037B8 RID: 14264
	public GameEntityManager entityManager;

	// Token: 0x040037B9 RID: 14265
	public PhotonView photonView;

	// Token: 0x040037BA RID: 14266
	private List<GameAgent> agents;

	// Token: 0x040037BB RID: 14267
	private float lastDestinationSentTime;

	// Token: 0x040037BC RID: 14268
	private float destinationCooldown;

	// Token: 0x040037BD RID: 14269
	private List<int> netIdsForDestination;

	// Token: 0x040037BE RID: 14270
	private List<Vector3> destinationsForDestination;

	// Token: 0x040037BF RID: 14271
	private List<int> netIdsForState;

	// Token: 0x040037C0 RID: 14272
	private List<byte> statesForState;

	// Token: 0x040037C1 RID: 14273
	private float lastStateSentTime;

	// Token: 0x040037C2 RID: 14274
	private float stateCooldown;

	// Token: 0x040037C3 RID: 14275
	private List<int> netIdsForBehavior;

	// Token: 0x040037C4 RID: 14276
	private List<byte> behaviorsForBehavior;

	// Token: 0x040037C5 RID: 14277
	private float lastBehaviorSentTime;

	// Token: 0x040037C6 RID: 14278
	private float behaviorCooldown = 0.25f;

	// Token: 0x040037C7 RID: 14279
	private const int MAX_UPDATES_PER_FRAME = 4;

	// Token: 0x040037C8 RID: 14280
	private int nextAgentIndexUpdate;

	// Token: 0x040037C9 RID: 14281
	private const int MAX_THINK_PER_FRAME = 1;

	// Token: 0x040037CA RID: 14282
	private int nextAgentIndexThink;

	// Token: 0x040037CC RID: 14284
	public CallLimitersList<CallLimiter, GameAgentManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GameAgentManager.RPC>();

	// Token: 0x020006B7 RID: 1719
	public enum RPC
	{
		// Token: 0x040037CE RID: 14286
		ApplyDestination,
		// Token: 0x040037CF RID: 14287
		ApplyState,
		// Token: 0x040037D0 RID: 14288
		ApplyBehaviour,
		// Token: 0x040037D1 RID: 14289
		ApplyImpact,
		// Token: 0x040037D2 RID: 14290
		ApplyTarget
	}
}
