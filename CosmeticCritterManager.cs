using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000682 RID: 1666
public class CosmeticCritterManager : NetworkSceneObject, ITickSystemTick
{
	// Token: 0x17000435 RID: 1077
	// (get) Token: 0x060029B3 RID: 10675 RVA: 0x000E12EF File Offset: 0x000DF4EF
	// (set) Token: 0x060029B4 RID: 10676 RVA: 0x000E12F6 File Offset: 0x000DF4F6
	public static CosmeticCritterManager Instance { get; private set; }

	// Token: 0x17000436 RID: 1078
	// (get) Token: 0x060029B5 RID: 10677 RVA: 0x000E12FE File Offset: 0x000DF4FE
	// (set) Token: 0x060029B6 RID: 10678 RVA: 0x000E1306 File Offset: 0x000DF506
	public bool TickRunning { get; set; }

	// Token: 0x060029B7 RID: 10679 RVA: 0x000E130F File Offset: 0x000DF50F
	private new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060029B8 RID: 10680 RVA: 0x000E1323 File Offset: 0x000DF523
	private new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x060029B9 RID: 10681 RVA: 0x000E1337 File Offset: 0x000DF537
	public void RegisterLocalHoldable(CosmeticCritterHoldable holdable)
	{
		this.localHoldables.Add(holdable);
	}

	// Token: 0x060029BA RID: 10682 RVA: 0x000E1345 File Offset: 0x000DF545
	public void RegisterIndependentSpawner(CosmeticCritterSpawnerIndependent spawner)
	{
		if (spawner.IsLocal)
		{
			this.localCritterSpawners.AddIfNew(spawner);
			return;
		}
		this.remoteCritterSpawners.AddIfNew(spawner);
	}

	// Token: 0x060029BB RID: 10683 RVA: 0x000E1368 File Offset: 0x000DF568
	public void UnregisterIndependentSpawner(CosmeticCritterSpawnerIndependent spawner)
	{
		if (spawner.IsLocal)
		{
			this.localCritterSpawners.Remove(spawner);
			return;
		}
		this.remoteCritterSpawners.Remove(spawner);
	}

	// Token: 0x060029BC RID: 10684 RVA: 0x000E138D File Offset: 0x000DF58D
	public void RegisterCatcher(CosmeticCritterCatcher catcher)
	{
		if (catcher.IsLocal)
		{
			this.localCritterCatchers.AddIfNew(catcher);
			return;
		}
		this.remoteCritterCatchers.AddIfNew(catcher);
	}

	// Token: 0x060029BD RID: 10685 RVA: 0x000E13B0 File Offset: 0x000DF5B0
	public void UnregisterCatcher(CosmeticCritterCatcher catcher)
	{
		if (catcher.IsLocal)
		{
			this.localCritterCatchers.Remove(catcher);
			return;
		}
		this.remoteCritterCatchers.Remove(catcher);
	}

	// Token: 0x060029BE RID: 10686 RVA: 0x000E13D8 File Offset: 0x000DF5D8
	public void RegisterTickForEachCritter(Type type, ICosmeticCritterTickForEach target)
	{
		List<ICosmeticCritterTickForEach> list;
		if (!this.tickForEachCritterOfType.TryGetValue(type, out list) || list == null)
		{
			list = new List<ICosmeticCritterTickForEach>();
			this.tickForEachCritterOfType.Add(type, list);
		}
		list.AddIfNew(target);
	}

	// Token: 0x060029BF RID: 10687 RVA: 0x000E1414 File Offset: 0x000DF614
	public void UnregisterTickForEachCritter(Type type, ICosmeticCritterTickForEach target)
	{
		List<ICosmeticCritterTickForEach> list;
		if (this.tickForEachCritterOfType.TryGetValue(type, out list) && list != null)
		{
			list.Remove(target);
		}
	}

	// Token: 0x060029C0 RID: 10688 RVA: 0x000E143C File Offset: 0x000DF63C
	private void ResetLocalCallLimiters()
	{
		int i = 0;
		while (i < this.localHoldables.Count)
		{
			if (this.localHoldables[i] == null)
			{
				this.localHoldables.RemoveAt(i);
			}
			else
			{
				this.localHoldables[i].ResetCallLimiter();
				i++;
			}
		}
	}

	// Token: 0x060029C1 RID: 10689 RVA: 0x000E1494 File Offset: 0x000DF694
	private void ResetCosmeticCritters(NetPlayer player)
	{
		if (NetworkSystem.Instance.LocalPlayer != player)
		{
			return;
		}
		this.ResetLocalCallLimiters();
		for (int i = 0; i < this.activeCritters.Count; i++)
		{
			this.FreeCritter(this.activeCritters[i]);
		}
	}

	// Token: 0x060029C2 RID: 10690 RVA: 0x000E14E0 File Offset: 0x000DF6E0
	private void Awake()
	{
		if (CosmeticCritterManager.Instance != null && CosmeticCritterManager.Instance != this)
		{
			global::UnityEngine.Object.Destroy(this);
			return;
		}
		CosmeticCritterManager.Instance = this;
		this.localHoldables = new List<CosmeticCritterHoldable>();
		this.localCritterSpawners = new List<CosmeticCritterSpawnerIndependent>();
		this.remoteCritterSpawners = new List<CosmeticCritterSpawnerIndependent>();
		this.localCritterCatchers = new List<CosmeticCritterCatcher>();
		this.remoteCritterCatchers = new List<CosmeticCritterCatcher>();
		this.activeCritters = new List<CosmeticCritter>();
		this.activeCrittersPerType = new Dictionary<Type, int>();
		this.activeCrittersBySeed = new Dictionary<int, CosmeticCritter>();
		this.inactiveCrittersByType = new Dictionary<Type, Stack<CosmeticCritter>>();
		this.tickForEachCritterOfType = new Dictionary<Type, List<ICosmeticCritterTickForEach>>();
		NetworkSystem.Instance.OnPlayerJoined += this.ResetCosmeticCritters;
		NetworkSystem.Instance.OnPlayerLeft += this.ResetCosmeticCritters;
	}

	// Token: 0x060029C3 RID: 10691 RVA: 0x000E15C4 File Offset: 0x000DF7C4
	private void ReuseOrSpawnNewCritter(CosmeticCritterSpawner spawner, int seed, double time)
	{
		Type critterType = spawner.GetCritterType();
		Stack<CosmeticCritter> stack;
		CosmeticCritter cosmeticCritter;
		if (!this.inactiveCrittersByType.TryGetValue(critterType, out stack))
		{
			stack = new Stack<CosmeticCritter>();
			this.inactiveCrittersByType.Add(critterType, stack);
			cosmeticCritter = global::UnityEngine.Object.Instantiate<GameObject>(spawner.GetCritterPrefab(), base.transform).GetComponent<CosmeticCritter>();
		}
		else if (stack.TryPop(out cosmeticCritter))
		{
			cosmeticCritter.gameObject.SetActive(true);
		}
		else
		{
			cosmeticCritter = global::UnityEngine.Object.Instantiate<GameObject>(spawner.GetCritterPrefab(), base.transform).GetComponent<CosmeticCritter>();
		}
		cosmeticCritter.SetSeedSpawnerTypeAndTime(seed, spawner, critterType, time);
		this.activeCritters.Add(cosmeticCritter);
		if (!this.activeCrittersPerType.ContainsKey(critterType))
		{
			this.activeCrittersPerType.Add(critterType, 1);
		}
		else
		{
			Dictionary<Type, int> dictionary = this.activeCrittersPerType;
			Type type = critterType;
			dictionary[type]++;
		}
		this.activeCrittersBySeed.Add(seed, cosmeticCritter);
		Random.State state = Random.state;
		Random.InitState(seed);
		spawner.SetRandomVariables(cosmeticCritter);
		cosmeticCritter.SetRandomVariables();
		Random.state = state;
		spawner.OnSpawn(cosmeticCritter);
		cosmeticCritter.OnSpawn();
	}

	// Token: 0x060029C4 RID: 10692 RVA: 0x000E16CC File Offset: 0x000DF8CC
	private void FreeCritter(CosmeticCritter critter)
	{
		critter.OnDespawn();
		if (critter.Spawner != null)
		{
			critter.Spawner.OnDespawn(critter);
		}
		critter.gameObject.SetActive(false);
		Type cachedType = critter.CachedType;
		Stack<CosmeticCritter> stack;
		if (!this.inactiveCrittersByType.TryGetValue(cachedType, out stack))
		{
			stack = new Stack<CosmeticCritter>();
			this.inactiveCrittersByType.Add(cachedType, stack);
		}
		stack.Push(critter);
		this.activeCritters.Remove(critter);
		int num;
		if (this.activeCrittersPerType.TryGetValue(cachedType, out num))
		{
			this.activeCrittersPerType[cachedType] = Math.Max(num - 1, 0);
		}
		this.activeCrittersBySeed.Remove(critter.Seed);
	}

	// Token: 0x060029C5 RID: 10693 RVA: 0x000E177C File Offset: 0x000DF97C
	public void Tick()
	{
		for (int i = 0; i < this.activeCritters.Count; i++)
		{
			CosmeticCritter cosmeticCritter = this.activeCritters[i];
			if (cosmeticCritter.Expired())
			{
				this.FreeCritter(cosmeticCritter);
			}
			else
			{
				cosmeticCritter.Tick();
				List<ICosmeticCritterTickForEach> list;
				if (this.tickForEachCritterOfType.TryGetValue(cosmeticCritter.CachedType, out list))
				{
					for (int j = 0; j < list.Count; j++)
					{
						list[j].TickForEachCritter(cosmeticCritter);
					}
				}
				int k = 0;
				while (k < this.localCritterCatchers.Count)
				{
					CosmeticCritterCatcher cosmeticCritterCatcher = this.localCritterCatchers[k];
					CosmeticCritterAction localCatchAction = cosmeticCritterCatcher.GetLocalCatchAction(cosmeticCritter);
					if (localCatchAction != CosmeticCritterAction.None)
					{
						double num = (PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.timeAsDouble);
						cosmeticCritterCatcher.OnCatch(cosmeticCritter, localCatchAction, num);
						if ((localCatchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None)
						{
							this.FreeCritter(cosmeticCritter);
							i--;
						}
						if ((localCatchAction & CosmeticCritterAction.SpawnLinked) != CosmeticCritterAction.None && cosmeticCritterCatcher.GetLinkedSpawner() != null)
						{
							this.ReuseOrSpawnNewCritter(cosmeticCritterCatcher.GetLinkedSpawner(), cosmeticCritter.Seed + 1, num);
						}
						if (PhotonNetwork.InRoom && (localCatchAction & CosmeticCritterAction.RPC) != CosmeticCritterAction.None)
						{
							this.photonView.RPC("CosmeticCritterRPC", RpcTarget.Others, new object[] { localCatchAction, cosmeticCritterCatcher.OwnerID, cosmeticCritter.Seed });
							break;
						}
						break;
					}
					else
					{
						k++;
					}
				}
			}
		}
		for (int l = 0; l < this.localCritterSpawners.Count; l++)
		{
			CosmeticCritterSpawnerIndependent cosmeticCritterSpawnerIndependent = this.localCritterSpawners[l];
			int num2;
			if ((!this.activeCrittersPerType.TryGetValue(cosmeticCritterSpawnerIndependent.GetCritterType(), out num2) || num2 < cosmeticCritterSpawnerIndependent.GetCritter().GetGlobalMaxCritters()) && cosmeticCritterSpawnerIndependent.CanSpawnLocal())
			{
				int num3 = Random.Range(0, int.MaxValue);
				if (!this.activeCrittersBySeed.ContainsKey(num3))
				{
					this.ReuseOrSpawnNewCritter(cosmeticCritterSpawnerIndependent, num3, PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.timeAsDouble);
					if (PhotonNetwork.InRoom)
					{
						this.photonView.RPC("CosmeticCritterRPC", RpcTarget.Others, new object[]
						{
							CosmeticCritterAction.RPC | CosmeticCritterAction.Spawn,
							cosmeticCritterSpawnerIndependent.OwnerID,
							num3
						});
					}
				}
			}
		}
	}

	// Token: 0x060029C6 RID: 10694 RVA: 0x000E19C4 File Offset: 0x000DFBC4
	[PunRPC]
	private void CosmeticCritterRPC(CosmeticCritterAction action, int holdableID, int seed, PhotonMessageInfo info)
	{
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		MonkeAgent.IncrementRPCCall(photonMessageInfoWrapped, "CosmeticCritterRPC");
		if ((action & CosmeticCritterAction.RPC) == CosmeticCritterAction.None)
		{
			return;
		}
		if (action == (CosmeticCritterAction.RPC | CosmeticCritterAction.Spawn))
		{
			this.SpawnCosmeticCritterRPC(holdableID, seed, photonMessageInfoWrapped);
			return;
		}
		this.CatchCosmeticCritterRPC(action, holdableID, seed, photonMessageInfoWrapped);
	}

	// Token: 0x060029C7 RID: 10695 RVA: 0x000E1A04 File Offset: 0x000DFC04
	private void CatchCosmeticCritterRPC(CosmeticCritterAction catchAction, int catcherID, int seed, PhotonMessageInfoWrapped info)
	{
		CosmeticCritter cosmeticCritter;
		if (!this.activeCrittersBySeed.TryGetValue(seed, out cosmeticCritter))
		{
			return;
		}
		int i = 0;
		while (i < this.remoteCritterCatchers.Count)
		{
			CosmeticCritterCatcher cosmeticCritterCatcher = this.remoteCritterCatchers[i];
			if (cosmeticCritterCatcher.OwnerID == catcherID)
			{
				if (!cosmeticCritterCatcher.OwningPlayerMatches(info))
				{
					return;
				}
				if (cosmeticCritterCatcher.ValidateRemoteCatchAction(cosmeticCritter, catchAction, info.SentServerTime))
				{
					cosmeticCritterCatcher.OnCatch(cosmeticCritter, catchAction, info.SentServerTime);
					if ((catchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None)
					{
						this.FreeCritter(cosmeticCritter);
					}
					int num;
					if ((catchAction & CosmeticCritterAction.SpawnLinked) != CosmeticCritterAction.None && cosmeticCritterCatcher.GetLinkedSpawner() != null && (!this.activeCrittersPerType.TryGetValue(cosmeticCritterCatcher.GetLinkedSpawner().GetCritterType(), out num) || num < cosmeticCritterCatcher.GetLinkedSpawner().GetCritter().GetGlobalMaxCritters() + 1))
					{
						this.ReuseOrSpawnNewCritter(cosmeticCritterCatcher.GetLinkedSpawner(), seed + 1, info.SentServerTime);
					}
				}
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x060029C8 RID: 10696 RVA: 0x000E1AE8 File Offset: 0x000DFCE8
	private void SpawnCosmeticCritterRPC(int spawnerID, int seed, PhotonMessageInfoWrapped info)
	{
		if (this.activeCrittersBySeed.ContainsKey(seed))
		{
			return;
		}
		int i = 0;
		while (i < this.remoteCritterSpawners.Count)
		{
			CosmeticCritterSpawnerIndependent cosmeticCritterSpawnerIndependent = this.remoteCritterSpawners[i];
			if (cosmeticCritterSpawnerIndependent.OwnerID == spawnerID)
			{
				if (!cosmeticCritterSpawnerIndependent.OwningPlayerMatches(info))
				{
					return;
				}
				int num;
				if ((!this.activeCrittersPerType.TryGetValue(cosmeticCritterSpawnerIndependent.GetCritterType(), out num) || num < cosmeticCritterSpawnerIndependent.GetCritter().GetGlobalMaxCritters()) && cosmeticCritterSpawnerIndependent.CanSpawnRemote(info.SentServerTime))
				{
					this.ReuseOrSpawnNewCritter(cosmeticCritterSpawnerIndependent, seed, info.SentServerTime);
				}
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x04003645 RID: 13893
	private List<CosmeticCritterHoldable> localHoldables;

	// Token: 0x04003646 RID: 13894
	private List<CosmeticCritterSpawnerIndependent> localCritterSpawners;

	// Token: 0x04003647 RID: 13895
	private List<CosmeticCritterSpawnerIndependent> remoteCritterSpawners;

	// Token: 0x04003648 RID: 13896
	private List<CosmeticCritterCatcher> localCritterCatchers;

	// Token: 0x04003649 RID: 13897
	private List<CosmeticCritterCatcher> remoteCritterCatchers;

	// Token: 0x0400364A RID: 13898
	private List<CosmeticCritter> activeCritters;

	// Token: 0x0400364B RID: 13899
	private Dictionary<Type, int> activeCrittersPerType;

	// Token: 0x0400364C RID: 13900
	private Dictionary<int, CosmeticCritter> activeCrittersBySeed;

	// Token: 0x0400364D RID: 13901
	private Dictionary<Type, Stack<CosmeticCritter>> inactiveCrittersByType;

	// Token: 0x0400364E RID: 13902
	private Dictionary<Type, List<ICosmeticCritterTickForEach>> tickForEachCritterOfType;
}
