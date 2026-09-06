using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Critters.Scripts;
using Fusion;
using GorillaExtensions;
using GorillaNetworking;
using Photon.Pun;
using PlayFab;
using UnityEngine;
using Utilities;

// Token: 0x02000067 RID: 103
[NetworkBehaviourWeaved(0)]
public class CrittersManager : NetworkComponent, IRequestableOwnershipGuardCallbacks, IBuildValidation, ITickSystemTick
{
	// Token: 0x17000027 RID: 39
	// (get) Token: 0x06000204 RID: 516 RVA: 0x0000BCB0 File Offset: 0x00009EB0
	// (set) Token: 0x06000205 RID: 517 RVA: 0x0000BCB7 File Offset: 0x00009EB7
	public static bool hasInstance { get; private set; }

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x06000206 RID: 518 RVA: 0x0000BCBF File Offset: 0x00009EBF
	// (set) Token: 0x06000207 RID: 519 RVA: 0x0000BCC7 File Offset: 0x00009EC7
	public bool TickRunning { get; set; }

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x06000208 RID: 520 RVA: 0x0000BCD0 File Offset: 0x00009ED0
	public bool allowGrabbingEntireBag
	{
		get
		{
			if (!NetworkSystem.Instance.SessionIsPrivate)
			{
				return (CrittersManager.AllowGrabbingFlags.EntireBag & this.publicRoomGrabbingFlags) > CrittersManager.AllowGrabbingFlags.None;
			}
			return (CrittersManager.AllowGrabbingFlags.EntireBag & this.privateRoomGrabbingFlags) > CrittersManager.AllowGrabbingFlags.None;
		}
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x06000209 RID: 521 RVA: 0x0000BCF5 File Offset: 0x00009EF5
	public bool allowGrabbingOutOfHands
	{
		get
		{
			if (!NetworkSystem.Instance.SessionIsPrivate)
			{
				return (CrittersManager.AllowGrabbingFlags.OutOfHands & this.publicRoomGrabbingFlags) > CrittersManager.AllowGrabbingFlags.None;
			}
			return (CrittersManager.AllowGrabbingFlags.OutOfHands & this.privateRoomGrabbingFlags) > CrittersManager.AllowGrabbingFlags.None;
		}
	}

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x0600020A RID: 522 RVA: 0x0000BD1A File Offset: 0x00009F1A
	public bool allowGrabbingFromBags
	{
		get
		{
			if (!NetworkSystem.Instance.SessionIsPrivate)
			{
				return (CrittersManager.AllowGrabbingFlags.FromBags & this.publicRoomGrabbingFlags) > CrittersManager.AllowGrabbingFlags.None;
			}
			return (CrittersManager.AllowGrabbingFlags.FromBags & this.privateRoomGrabbingFlags) > CrittersManager.AllowGrabbingFlags.None;
		}
	}

	// Token: 0x0600020B RID: 523 RVA: 0x0000BD40 File Offset: 0x00009F40
	public void LoadGrabSettings()
	{
		PlayFabTitleDataCache.Instance.GetTitleData("PublicCrittersGrabSettings", delegate(string data)
		{
			int num;
			if (int.TryParse(data, out num))
			{
				this.publicRoomGrabbingFlags = (CrittersManager.AllowGrabbingFlags)num;
			}
		}, delegate(PlayFabError e)
		{
		}, false);
		PlayFabTitleDataCache.Instance.GetTitleData("PrivateCrittersGrabSettings", delegate(string data)
		{
			int num2;
			if (int.TryParse(data, out num2))
			{
				this.privateRoomGrabbingFlags = (CrittersManager.AllowGrabbingFlags)num2;
			}
		}, delegate(PlayFabError e)
		{
		}, false);
	}

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x0600020C RID: 524 RVA: 0x0000BDC3 File Offset: 0x00009FC3
	public bool LocalInZone
	{
		get
		{
			return this.localInZone;
		}
	}

	// Token: 0x14000006 RID: 6
	// (add) Token: 0x0600020D RID: 525 RVA: 0x0000BDCC File Offset: 0x00009FCC
	// (remove) Token: 0x0600020E RID: 526 RVA: 0x0000BE04 File Offset: 0x0000A004
	public event Action<CrittersManager.CritterEvent, int, Vector3, Quaternion> OnCritterEventReceived;

	// Token: 0x0600020F RID: 527 RVA: 0x0000BE3C File Offset: 0x0000A03C
	public bool BuildValidationCheck()
	{
		if (this.guard == null)
		{
			Debug.LogError("requestable owner guard missing", base.gameObject);
			return false;
		}
		if (this.crittersPool == null)
		{
			Debug.LogError("critters pool missing", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x06000210 RID: 528 RVA: 0x0000BE8A File Offset: 0x0000A08A
	protected override void Start()
	{
		base.Start();
		CrittersManager.instance.LoadGrabSettings();
		CrittersManager.CheckInitialize();
	}

	// Token: 0x06000211 RID: 529 RVA: 0x0000BEA4 File Offset: 0x0000A0A4
	public static void InitializeCrittersManager()
	{
		if (CrittersManager.hasInstance)
		{
			return;
		}
		CrittersManager.hasInstance = true;
		CrittersManager.instance = global::UnityEngine.Object.FindAnyObjectByType<CrittersManager>();
		CrittersManager.instance.crittersActors = new List<CrittersActor>();
		CrittersManager.instance.crittersPawns = new List<CrittersPawn>();
		CrittersManager.instance.awareOfActors = new Dictionary<CrittersPawn, List<CrittersActor>>();
		CrittersManager.instance.despawnableActors = new List<CrittersActor>();
		CrittersManager.instance.newlyDisabledActors = new List<CrittersActor>();
		CrittersManager.instance.rigActorSetups = new List<CrittersRigActorSetup>();
		CrittersManager.instance.rigSetupByRig = new Dictionary<VRRig, CrittersRigActorSetup>();
		CrittersManager.instance.updatesToSend = new List<int>();
		CrittersManager.instance.objList = new List<object>();
		CrittersManager.instance.lowPriorityPawnsToProcess = new List<CrittersActor>();
		CrittersManager.instance.actorSpawners = global::UnityEngine.Object.FindObjectsByType<CrittersActorSpawner>(FindObjectsSortMode.None).ToList<CrittersActorSpawner>();
		CrittersManager.instance._spawnRegions = CrittersRegion.Regions;
		CrittersManager.instance.poolCounts = new Dictionary<CrittersActor.CrittersActorType, int>();
		CrittersManager.instance.despawnDecayValue = new Dictionary<CrittersActor.CrittersActorType, float>();
		CrittersManager.instance.actorTypes = (CrittersActor.CrittersActorType[])Enum.GetValues(typeof(CrittersActor.CrittersActorType));
		CrittersManager.instance.poolIndexDict = new Dictionary<CrittersActor.CrittersActorType, int>();
		for (int i = 0; i < CrittersManager.instance.actorTypes.Length; i++)
		{
			CrittersManager.instance.poolCounts[CrittersManager.instance.actorTypes[i]] = 0;
			CrittersManager.instance.despawnDecayValue[CrittersManager.instance.actorTypes[i]] = 0f;
		}
		CrittersManager.instance.PopulatePools();
		List<CrittersRigActorSetup> list = global::UnityEngine.Object.FindObjectsByType<CrittersRigActorSetup>(FindObjectsSortMode.None).ToList<CrittersRigActorSetup>();
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].enabled)
			{
				CrittersManager.RegisterRigActorSetup(list[j]);
			}
		}
		CrittersActorGrabber[] array = global::UnityEngine.Object.FindObjectsByType<CrittersActorGrabber>(FindObjectsSortMode.None);
		for (int k = 0; k < array.Length; k++)
		{
			if (array[k].isLeft)
			{
				CrittersManager._leftGrabber = array[k];
			}
			else
			{
				CrittersManager._rightGrabber = array[k];
			}
		}
		if (CrittersManager.instance.guard.IsNotNull())
		{
			CrittersManager.instance.guard.AddCallbackTarget(CrittersManager.instance);
		}
		RoomSystem.JoinedRoomEvent += new Action(CrittersManager.instance.JoinedRoomEvent);
		RoomSystem.LeftRoomEvent += new Action(CrittersManager.instance.LeftRoomEvent);
	}

	// Token: 0x06000212 RID: 530 RVA: 0x0000C135 File Offset: 0x0000A335
	private new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06000213 RID: 531 RVA: 0x0000C149 File Offset: 0x0000A349
	private new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06000214 RID: 532 RVA: 0x0000C160 File Offset: 0x0000A360
	private void ResetRoom()
	{
		this.lastSpawnTime = 0.0;
		for (int i = 0; i < this.allActors.Count; i++)
		{
			CrittersActor crittersActor = this.allActors[i];
			if (crittersActor.gameObject.activeSelf)
			{
				if (this.persistentActors.Contains(this.allActors[i]))
				{
					this.allActors[i].Initialize();
				}
				else
				{
					crittersActor.gameObject.SetActive(false);
				}
			}
		}
		for (int j = 0; j < this.actorSpawners.Count; j++)
		{
			this.actorSpawners[j].DoReset();
		}
	}

	// Token: 0x06000215 RID: 533 RVA: 0x0000C20B File Offset: 0x0000A40B
	public void Tick()
	{
		this.HandleZonesAndOwnership();
		if (this.localInZone)
		{
			this.ProcessSpawning();
			this.ProcessActorBinLocations();
			this.ProcessRigSetups();
			this.ProcessCritterAwareness();
			this.ProcessDespawningIdles();
			this.ProcessActors();
		}
		this.ProcessNewlyDisabledActors();
	}

	// Token: 0x06000216 RID: 534 RVA: 0x0000C248 File Offset: 0x0000A448
	public void ProcessRigSetups()
	{
		if (!this.LocalAuthority())
		{
			return;
		}
		this.objList.Clear();
		for (int i = 0; i < this.rigActorSetups.Count; i++)
		{
			this.rigActorSetups[i].CheckUpdate(ref this.objList, false);
		}
		if (this.objList.Count > 0 && NetworkSystem.Instance.InRoom)
		{
			CrittersManager.instance.SendRPC("RemoteUpdatePlayerCrittersActorData", RpcTarget.Others, new object[] { this.objList.ToArray() });
		}
	}

	// Token: 0x06000217 RID: 535 RVA: 0x0000C2D8 File Offset: 0x0000A4D8
	private void ProcessCritterAwareness()
	{
		if (!this.LocalAuthority())
		{
			return;
		}
		int num = 0;
		this.lowPriorityPawnsToProcess.Clear();
		int i = 0;
		while (i < this.crittersPawns.Count)
		{
			CrittersPawn crittersPawn = this.crittersPawns[i];
			if (!this.awareOfActors.ContainsKey(crittersPawn))
			{
				this.awareOfActors[crittersPawn] = new List<CrittersActor>();
			}
			else
			{
				this.awareOfActors[crittersPawn].Clear();
			}
			this.nearbyActors.Clear();
			int num2 = this.actorBinIndices[crittersPawn];
			if (this.priorityBins[num2])
			{
				goto IL_00D9;
			}
			if (i >= this.lowPriorityIndex && num < this.lowPriorityActorsPerFrame)
			{
				this.lowPriorityPawnsToProcess.Add(this.crittersPawns[i]);
				num++;
				this.lowPriorityIndex++;
				if (this.lowPriorityIndex >= this.crittersPawns.Count)
				{
					this.lowPriorityIndex = 0;
					goto IL_00D9;
				}
				goto IL_00D9;
			}
			IL_01C4:
			i++;
			continue;
			IL_00D9:
			int num3 = Mathf.FloorToInt((float)(num2 / this.binXCount));
			int num4 = num2 % this.binXCount;
			for (int j = -1; j <= 1; j++)
			{
				for (int k = -1; k <= 1; k++)
				{
					if (num3 + j < this.binXCount && num3 + j >= 0 && num4 + k < this.binZCount && num4 + k >= 0)
					{
						this.nearbyActors.AddRange(this.actorBins[num4 + k + (num3 + j) * this.binXCount]);
					}
				}
			}
			for (int l = 0; l < this.nearbyActors.Count; l++)
			{
				if (this.crittersPawns[i].AwareOfActor(this.nearbyActors[l]))
				{
					this.awareOfActors[this.crittersPawns[i]].Add(this.nearbyActors[l]);
				}
			}
			goto IL_01C4;
		}
	}

	// Token: 0x06000218 RID: 536 RVA: 0x0000C4C0 File Offset: 0x0000A6C0
	private void ProcessSpawning()
	{
		if (!this.LocalAuthority())
		{
			return;
		}
		if (this.lastSpawnTime + this.spawnDelay <= (NetworkSystem.Instance.InRoom ? PhotonNetwork.Time : ((double)Time.time)))
		{
			int nextSpawnRegion = this.GetNextSpawnRegion();
			if (nextSpawnRegion >= 0)
			{
				this.SpawnCritter(nextSpawnRegion);
			}
			else
			{
				this.lastSpawnTime = (NetworkSystem.Instance.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			}
		}
		if (this.spawnerIndex >= this.actorSpawners.Count)
		{
			this.spawnerIndex = 0;
		}
		if (this.actorSpawners.Count == 0)
		{
			return;
		}
		this.actorSpawners[this.spawnerIndex].ProcessLocal();
		this.spawnerIndex++;
	}

	// Token: 0x06000219 RID: 537 RVA: 0x0000C580 File Offset: 0x0000A780
	private int GetNextSpawnRegion()
	{
		for (int i = 1; i <= this._spawnRegions.Count; i++)
		{
			int num = (this._currentRegionIndex + i) % this._spawnRegions.Count;
			CrittersRegion crittersRegion = this._spawnRegions[num];
			if (crittersRegion.CritterCount < crittersRegion.maxCritters)
			{
				this._currentRegionIndex = num;
				return this._currentRegionIndex;
			}
		}
		return -1;
	}

	// Token: 0x0600021A RID: 538 RVA: 0x0000C5E4 File Offset: 0x0000A7E4
	private void ProcessActorBinLocations()
	{
		if (this.LocalAuthority())
		{
			for (int i = 0; i < this.actorBins.Length; i++)
			{
				this.actorBins[i].Clear();
				this.priorityBins[i] = false;
			}
			for (int j = this.crittersActors.Count - 1; j >= 0; j--)
			{
				CrittersActor crittersActor = this.crittersActors[j];
				if (crittersActor == null)
				{
					this.crittersActors.RemoveAt(j);
				}
				else
				{
					Transform transform = crittersActor.transform;
					int num = Mathf.Clamp(Mathf.FloorToInt((transform.position.x - this.binDimensionXMin) / this.individualBinSide), 0, this.binXCount - 1);
					int num2 = Mathf.Clamp(Mathf.FloorToInt((transform.position.z - this.binDimensionZMin) / this.individualBinSide), 0, this.binZCount - 1);
					int num3 = num + num2 * this.binXCount;
					if (this.actorBinIndices.ContainsKey(crittersActor))
					{
						this.actorBinIndices[crittersActor] = num3;
					}
					else
					{
						this.actorBinIndices.Add(crittersActor, num3);
					}
					this.actorBins[num3].Add(crittersActor);
				}
			}
			for (int k = 0; k < RoomSystem.PlayersInRoom.Count; k++)
			{
				RigContainer rigContainer;
				if (VRRigCache.Instance.TryGetVrrig(RoomSystem.PlayersInRoom[k], out rigContainer))
				{
					Transform transform2 = rigContainer.Rig.transform;
					float num4 = (transform2.position.x - this.binDimensionXMin) / this.individualBinSide;
					float num5 = (transform2.position.z - this.binDimensionZMin) / this.individualBinSide;
					int num6 = Mathf.FloorToInt(num4);
					int num7 = Mathf.FloorToInt(num5);
					int num8 = ((num4 % 1f > 0.5f) ? 1 : (-1));
					int num9 = ((num5 % 1f > 0.5f) ? 1 : (-1));
					if (num6 < 0 || num6 >= this.binXCount || num7 < 0 || num7 >= this.binZCount)
					{
						return;
					}
					int num10 = num6 + num7 * this.binXCount;
					this.priorityBins[num10] = true;
					num8 = Mathf.Clamp(num6 + num8, 0, this.binXCount - 1);
					num9 = Mathf.Clamp(num7 + num9, 0, this.binZCount - 1);
					this.priorityBins[num8 + num7 * this.binXCount] = true;
					this.priorityBins[num6 + num9 * this.binXCount] = true;
					this.priorityBins[num8 + num9 * this.binXCount] = true;
				}
			}
		}
	}

	// Token: 0x0600021B RID: 539 RVA: 0x0000C864 File Offset: 0x0000AA64
	private void ProcessDespawningIdles()
	{
		for (int i = 0; i < this.actorTypes.Length; i++)
		{
			this.despawnDecayValue[this.actorTypes[i]] = Mathf.Lerp(this.despawnDecayValue[this.actorTypes[i]], (float)this.despawnThreshold, 1f - Mathf.Exp(-this.decayRate * (Time.realtimeSinceStartup - Time.deltaTime)));
		}
		if (!this.LocalAuthority())
		{
			return;
		}
		if (this.despawnableActors.Count == 0)
		{
			return;
		}
		int j = 0;
		while (j <= this.lowPriorityActorsPerFrame)
		{
			this.despawnIndex++;
			if (this.despawnIndex >= this.despawnableActors.Count)
			{
				this.despawnIndex = 0;
			}
			j++;
			CrittersActor crittersActor = this.despawnableActors[this.despawnIndex];
			if (this.despawnDecayValue[crittersActor.crittersActorType] >= (float)this.despawnThreshold && crittersActor.ShouldDespawn())
			{
				this.DespawnActor(crittersActor);
			}
		}
	}

	// Token: 0x0600021C RID: 540 RVA: 0x0000C960 File Offset: 0x0000AB60
	public void DespawnActor(CrittersActor actor)
	{
		int actorId = actor.actorId;
		if (!this.updatesToSend.Contains(actorId))
		{
			this.updatesToSend.Add(actorId);
		}
		actor.gameObject.SetActive(false);
	}

	// Token: 0x0600021D RID: 541 RVA: 0x0000C99C File Offset: 0x0000AB9C
	public void IncrementPoolCount(CrittersActor.CrittersActorType type)
	{
		int num;
		if (!this.poolCounts.TryGetValue(type, out num))
		{
			this.poolCounts[type] = 1;
		}
		else
		{
			this.poolCounts[type] = this.poolCounts[type] + 1;
		}
		float num2;
		if (!this.despawnDecayValue.TryGetValue(type, out num2))
		{
			this.despawnDecayValue[type] = 1f;
			return;
		}
		this.despawnDecayValue[type] = this.despawnDecayValue[type] + 1f;
	}

	// Token: 0x0600021E RID: 542 RVA: 0x0000CA24 File Offset: 0x0000AC24
	public void DecrementPoolCount(CrittersActor.CrittersActorType type)
	{
		int num;
		if (this.poolCounts.TryGetValue(type, out num))
		{
			this.poolCounts[type] = Mathf.Max(0, num - 1);
			return;
		}
		this.poolCounts[type] = 0;
	}

	// Token: 0x0600021F RID: 543 RVA: 0x0000CA64 File Offset: 0x0000AC64
	private void ProcessActors()
	{
		if (this.LocalAuthority())
		{
			for (int i = this.crittersActors.Count - 1; i >= 0; i--)
			{
				if (this.crittersActors[i].crittersActorType != CrittersActor.CrittersActorType.Creature || this.priorityBins[this.actorBinIndices[this.crittersActors[i]]] || this.lowPriorityPawnsToProcess.Contains(this.crittersActors[i]))
				{
					int actorId = this.crittersActors[i].actorId;
					if (this.crittersActors[i].ProcessLocal() && !this.updatesToSend.Contains(actorId))
					{
						this.updatesToSend.Add(actorId);
					}
				}
			}
			return;
		}
		for (int j = 0; j < this.crittersActors.Count; j++)
		{
			this.crittersActors[j].ProcessRemote();
		}
	}

	// Token: 0x06000220 RID: 544 RVA: 0x0000CB50 File Offset: 0x0000AD50
	private void ProcessNewlyDisabledActors()
	{
		for (int i = 0; i < this.newlyDisabledActors.Count; i++)
		{
			CrittersActor crittersActor = this.newlyDisabledActors[i];
			if (CrittersManager.instance.crittersActors.Contains(crittersActor))
			{
				CrittersManager.instance.crittersActors.Remove(crittersActor);
			}
			if (crittersActor.despawnWhenIdle && CrittersManager.instance.despawnableActors.Contains(crittersActor))
			{
				CrittersManager.instance.despawnableActors.Remove(crittersActor);
			}
			CrittersManager.instance.DecrementPoolCount(crittersActor.crittersActorType);
			crittersActor.SetTransformToDefaultParent(true);
		}
		this.newlyDisabledActors.Clear();
	}

	// Token: 0x06000221 RID: 545 RVA: 0x0000CC00 File Offset: 0x0000AE00
	public static void RegisterCritter(CrittersPawn crittersPawn)
	{
		CrittersManager.CheckInitialize();
		if (!CrittersManager.instance.crittersPawns.Contains(crittersPawn))
		{
			CrittersManager.instance.crittersPawns.Add(crittersPawn);
		}
	}

	// Token: 0x06000222 RID: 546 RVA: 0x0000CC30 File Offset: 0x0000AE30
	public static void RegisterRigActorSetup(CrittersRigActorSetup setup)
	{
		CrittersManager.CheckInitialize();
		if (!CrittersManager.instance.rigActorSetups.Contains(setup))
		{
			CrittersManager.instance.rigActorSetups.Add(setup);
		}
		CrittersManager.instance.rigSetupByRig.AddOrUpdate(setup.myRig, setup);
	}

	// Token: 0x06000223 RID: 547 RVA: 0x0000CC80 File Offset: 0x0000AE80
	public static void DeregisterCritter(CrittersPawn crittersPawn)
	{
		CrittersManager.CheckInitialize();
		CrittersManager.instance.SetCritterRegion(crittersPawn, 0);
		if (CrittersManager.instance.crittersPawns.Contains(crittersPawn))
		{
			CrittersManager.instance.crittersPawns.Remove(crittersPawn);
		}
	}

	// Token: 0x06000224 RID: 548 RVA: 0x0000CCBC File Offset: 0x0000AEBC
	public static void RegisterActor(CrittersActor actor)
	{
		CrittersManager.CheckInitialize();
		if (!CrittersManager.instance.crittersActors.Contains(actor))
		{
			CrittersManager.instance.crittersActors.Add(actor);
		}
		if (actor.despawnWhenIdle && !CrittersManager.instance.despawnableActors.Contains(actor))
		{
			CrittersManager.instance.despawnableActors.Add(actor);
		}
		if (CrittersManager.instance.newlyDisabledActors.Contains(actor))
		{
			CrittersManager.instance.newlyDisabledActors.Remove(actor);
		}
		CrittersManager.instance.IncrementPoolCount(actor.crittersActorType);
	}

	// Token: 0x06000225 RID: 549 RVA: 0x0000CD5B File Offset: 0x0000AF5B
	public static void DeregisterActor(CrittersActor actor)
	{
		CrittersManager.CheckInitialize();
		if (!CrittersManager.instance.newlyDisabledActors.Contains(actor))
		{
			CrittersManager.instance.newlyDisabledActors.Add(actor);
		}
	}

	// Token: 0x06000226 RID: 550 RVA: 0x0000CD88 File Offset: 0x0000AF88
	public static void CheckInitialize()
	{
		if (!CrittersManager.hasInstance)
		{
			CrittersManager.InitializeCrittersManager();
		}
	}

	// Token: 0x06000227 RID: 551 RVA: 0x0000CD96 File Offset: 0x0000AF96
	public static bool CritterAwareOfAny(CrittersPawn creature)
	{
		return CrittersManager.instance.awareOfActors[creature].Count > 0;
	}

	// Token: 0x06000228 RID: 552 RVA: 0x0000CDB4 File Offset: 0x0000AFB4
	public static bool AnyFoodNearby(CrittersPawn creature)
	{
		List<CrittersActor> list = CrittersManager.instance.awareOfActors[creature];
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].crittersActorType == CrittersActor.CrittersActorType.Food)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000229 RID: 553 RVA: 0x0000CDF8 File Offset: 0x0000AFF8
	public static CrittersFood ClosestFood(CrittersPawn creature)
	{
		float num = float.MaxValue;
		CrittersFood crittersFood = null;
		List<CrittersActor> list = CrittersManager.instance.awareOfActors[creature];
		for (int i = 0; i < list.Count; i++)
		{
			CrittersActor crittersActor = list[i];
			if (crittersActor.crittersActorType == CrittersActor.CrittersActorType.Food)
			{
				CrittersFood crittersFood2 = (CrittersFood)crittersActor;
				float sqrMagnitude = (creature.transform.position - crittersFood2.food.transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					crittersFood = crittersFood2;
					num = sqrMagnitude;
				}
			}
		}
		return crittersFood;
	}

	// Token: 0x0600022A RID: 554 RVA: 0x0000CE87 File Offset: 0x0000B087
	public static void PlayHaptics(AudioClip clip, float strength, bool isLeftHand)
	{
		(isLeftHand ? CrittersManager._leftGrabber : CrittersManager._rightGrabber).PlayHaptics(clip, strength);
	}

	// Token: 0x0600022B RID: 555 RVA: 0x0000CE9F File Offset: 0x0000B09F
	public static void StopHaptics(bool isLeftHand)
	{
		(isLeftHand ? CrittersManager._leftGrabber : CrittersManager._rightGrabber).StopHaptics();
	}

	// Token: 0x0600022C RID: 556 RVA: 0x0000CEB8 File Offset: 0x0000B0B8
	public CrittersPawn SpawnCritter(int regionIndex = -1)
	{
		CrittersRegion crittersRegion = ((regionIndex >= 0 && regionIndex < this._spawnRegions.Count) ? this._spawnRegions[regionIndex] : null);
		int randomCritterType = this.creatureIndex.GetRandomCritterType(crittersRegion);
		if (randomCritterType < 0)
		{
			return null;
		}
		Vector3 vector = (crittersRegion ? crittersRegion.GetSpawnPoint() : this._spawnRegions[0].transform.position);
		Quaternion quaternion = Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f);
		CrittersPawn crittersPawn = this.SpawnCritter(randomCritterType, vector, quaternion);
		this.SetCritterRegion(crittersPawn, crittersRegion);
		this.lastSpawnTime = (NetworkSystem.Instance.InRoom ? PhotonNetwork.Time : ((double)Time.time));
		return crittersPawn;
	}

	// Token: 0x0600022D RID: 557 RVA: 0x0000CF74 File Offset: 0x0000B174
	public CrittersPawn SpawnCritter(int critterType, Vector3 position, Quaternion rotation)
	{
		CrittersPawn crittersPawn = (CrittersPawn)this.SpawnActor(CrittersActor.CrittersActorType.Creature, -1);
		crittersPawn.SetTemplate(critterType);
		crittersPawn.currentState = CrittersPawn.CreatureState.Idle;
		crittersPawn.MoveActor(position, Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f), false, true, true);
		crittersPawn.SetImpulseVelocity(Vector3.zero, Vector3.zero);
		crittersPawn.SetState(CrittersPawn.CreatureState.Spawning);
		if (NetworkSystem.Instance.InRoom && this.LocalAuthority())
		{
			base.SendRPC("RemoteSpawnCreature", RpcTarget.Others, new object[]
			{
				crittersPawn.actorId,
				crittersPawn.regionId,
				crittersPawn.visuals.Appearance.WriteToRPCData()
			});
		}
		return crittersPawn;
	}

	// Token: 0x0600022E RID: 558 RVA: 0x0000D033 File Offset: 0x0000B233
	public void DespawnCritter(CrittersPawn crittersPawn)
	{
		this.DeactivateActor(crittersPawn);
	}

	// Token: 0x0600022F RID: 559 RVA: 0x0000D03C File Offset: 0x0000B23C
	public void QueueDespawnAllCritters()
	{
		if (!this.LocalAuthority())
		{
			return;
		}
		foreach (CrittersPawn crittersPawn in this.crittersPawns)
		{
			crittersPawn.SetState(CrittersPawn.CreatureState.Despawning);
		}
	}

	// Token: 0x06000230 RID: 560 RVA: 0x0000D098 File Offset: 0x0000B298
	private void SetCritterRegion(CrittersPawn critter, CrittersRegion region)
	{
		this.SetCritterRegion(critter, region ? region.ID : 0);
	}

	// Token: 0x06000231 RID: 561 RVA: 0x0000D0B2 File Offset: 0x0000B2B2
	private void SetCritterRegion(CrittersPawn critter, int regionId)
	{
		if (critter.regionId != 0)
		{
			CrittersRegion.RemoveCritterFromRegion(critter);
		}
		if (regionId != 0)
		{
			CrittersRegion.AddCritterToRegion(critter, regionId);
		}
		critter.regionId = regionId;
	}

	// Token: 0x06000232 RID: 562 RVA: 0x0000D0D3 File Offset: 0x0000B2D3
	public void DeactivateActor(CrittersActor actor)
	{
		actor.gameObject.SetActive(false);
	}

	// Token: 0x06000233 RID: 563 RVA: 0x0000D0E4 File Offset: 0x0000B2E4
	private void CamCapture()
	{
		Camera component = base.GetComponent<Camera>();
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = component.targetTexture;
		component.Render();
		Texture2D texture2D = new Texture2D(component.targetTexture.width, component.targetTexture.height);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)component.targetTexture.width, (float)component.targetTexture.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = active;
		texture2D.EncodeToPNG();
		global::UnityEngine.Object.Destroy(texture2D);
	}

	// Token: 0x06000234 RID: 564 RVA: 0x0000D171 File Offset: 0x0000B371
	private IEnumerator RemoteDataInitialization(NetPlayer player, int actorNumber)
	{
		List<object> nonPlayerActorObjList = new List<object>();
		List<object> playerActorObjList = new List<object>();
		int worldActorDataCount = 0;
		int playerActorDataCount = 0;
		int num;
		for (int i = 0; i < this.allActors.Count; i = num + 1)
		{
			if (!NetworkSystem.Instance.InRoom || !this.LocalAuthority())
			{
				this.RemoveInitializingPlayer(actorNumber);
				yield break;
			}
			if (this.allActors[i].isOnPlayer)
			{
				num = playerActorDataCount;
				playerActorDataCount = num + 1;
				this.allActors[i].AddPlayerCrittersActorDataToList(ref playerActorObjList);
			}
			if (playerActorDataCount >= this.actorsPerInitializationCall || (i == this.allActors.Count - 1 && playerActorDataCount > 0))
			{
				if (!player.InRoom || player.ActorNumber != actorNumber)
				{
					this.RemoveInitializingPlayer(actorNumber);
					yield break;
				}
				if (NetworkSystem.Instance.InRoom && this.LocalAuthority())
				{
					base.SendRPC("RemoteUpdatePlayerCrittersActorData", player, new object[] { playerActorObjList.ToArray() });
				}
				playerActorObjList.Clear();
				playerActorDataCount = 0;
				yield return new WaitForSeconds(this.actorsInitializationCallCooldown);
			}
			num = i;
		}
		if (!player.InRoom || player.ActorNumber != actorNumber)
		{
			this.RemoveInitializingPlayer(actorNumber);
			yield break;
		}
		if (NetworkSystem.Instance.InRoom && this.LocalAuthority() && playerActorDataCount > 0)
		{
			base.SendRPC("RemoteUpdatePlayerCrittersActorData", player, new object[] { playerActorObjList.ToArray() });
		}
		for (int i = 0; i < this.allActors.Count; i = num + 1)
		{
			if (!player.InRoom || player.ActorNumber != actorNumber)
			{
				this.RemoveInitializingPlayer(actorNumber);
				yield break;
			}
			if (!NetworkSystem.Instance.InRoom || !this.LocalAuthority())
			{
				this.RemoveInitializingPlayer(actorNumber);
				yield break;
			}
			CrittersActor crittersActor = this.allActors[i];
			if (crittersActor.gameObject.activeSelf)
			{
				num = worldActorDataCount;
				worldActorDataCount = num + 1;
				if (crittersActor.parentActorId == -1)
				{
					crittersActor.UpdateImpulses(false, false);
					crittersActor.UpdateImpulseVelocity();
				}
				crittersActor.AddActorDataToList(ref nonPlayerActorObjList);
				if (worldActorDataCount >= this.actorsPerInitializationCall)
				{
					if (!player.InRoom || player.ActorNumber != actorNumber)
					{
						this.RemoveInitializingPlayer(actorNumber);
						yield break;
					}
					if (!NetworkSystem.Instance.InRoom || !this.LocalAuthority())
					{
						this.RemoveInitializingPlayer(actorNumber);
						yield break;
					}
					base.SendRPC("RemoteUpdateCritterData", player, new object[] { nonPlayerActorObjList.ToArray() });
					nonPlayerActorObjList.Clear();
					worldActorDataCount = 0;
					yield return new WaitForSeconds(this.actorsInitializationCallCooldown);
				}
			}
			num = i;
		}
		if (NetworkSystem.Instance.InRoom && this.LocalAuthority() && worldActorDataCount > 0)
		{
			base.SendRPC("RemoteUpdateCritterData", player, new object[] { nonPlayerActorObjList.ToArray() });
		}
		this.RemoveInitializingPlayer(actorNumber);
		yield break;
	}

	// Token: 0x06000235 RID: 565 RVA: 0x0000D18E File Offset: 0x0000B38E
	private IEnumerator DelayedInitialization(NetPlayer player, List<object> nonPlayerActorObjList)
	{
		yield return new WaitForSeconds(30f);
		base.SendRPC("RemoteUpdateCritterData", player, new object[] { nonPlayerActorObjList.ToArray() });
		yield break;
	}

	// Token: 0x06000236 RID: 566 RVA: 0x0000D1AB File Offset: 0x0000B3AB
	public void RemoveInitializingPlayer(int actorNumber)
	{
		if (this.updatingPlayers.Contains(actorNumber))
		{
			this.updatingPlayers.Remove(actorNumber);
		}
	}

	// Token: 0x06000237 RID: 567 RVA: 0x0000D1C8 File Offset: 0x0000B3C8
	private void JoinedRoomEvent()
	{
		if (this.localInZone && !this.LocalAuthority())
		{
			this.ResetRoom();
		}
		this.hasNewlyInitialized = false;
	}

	// Token: 0x06000238 RID: 568 RVA: 0x0000D1E7 File Offset: 0x0000B3E7
	private void LeftRoomEvent()
	{
		this.guard.TransferOwnership(NetworkSystem.Instance.LocalPlayer, "");
		if (this.LocalInZone)
		{
			this.ResetRoom();
		}
	}

	// Token: 0x06000239 RID: 569 RVA: 0x0000D214 File Offset: 0x0000B414
	[PunRPC]
	public void RequestDataInitialization(PhotonMessageInfo info)
	{
		if (!NetworkSystem.Instance.InRoom || !this.LocalAuthority())
		{
			return;
		}
		if (this.updatingPlayers == null)
		{
			this.updatingPlayers = new List<int>();
		}
		if (this.updatingPlayers.Contains(info.Sender.ActorNumber))
		{
			return;
		}
		this.updatingPlayers.Add(info.Sender.ActorNumber);
		base.StartCoroutine(this.RemoteDataInitialization(info.Sender, info.Sender.ActorNumber));
	}

	// Token: 0x0600023A RID: 570 RVA: 0x0000D29C File Offset: 0x0000B49C
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.SenderIsOwner(info))
		{
			this.OwnerSentError(info);
			return;
		}
		if (!this.localInZone)
		{
			return;
		}
		int num;
		if (!CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num))
		{
			return;
		}
		if (num > this.actorsPerInitializationCall)
		{
			return;
		}
		int num2 = 0;
		while (num2 < num && this.UpdateActorByType(stream))
		{
			num2++;
		}
	}

	// Token: 0x0600023B RID: 571 RVA: 0x0000D2F4 File Offset: 0x0000B4F4
	public bool UpdateActorByType(PhotonStream stream)
	{
		int num;
		CrittersActor crittersActor;
		return CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num) && num >= 0 && num < this.universalActorId && this.actorById.TryGetValue(num, out crittersActor) && crittersActor.UpdateSpecificActor(stream);
	}

	// Token: 0x0600023C RID: 572 RVA: 0x0000D33C File Offset: 0x0000B53C
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!ZoneManagement.IsInZone(GTZone.critters))
		{
			return;
		}
		using (GTProfiler.BeginSample("WriteDataPUNCrittersManager"))
		{
			int num = Mathf.Min(this.updatesToSend.Count, this.actorsPerInitializationCall);
			stream.SendNext(num);
			for (int i = 0; i < num; i++)
			{
				this.allActors[this.updatesToSend[i]].SendDataByCrittersActorType(stream);
			}
			this.updatesToSend.RemoveRange(0, num);
		}
	}

	// Token: 0x0600023D RID: 573 RVA: 0x0000D3D4 File Offset: 0x0000B5D4
	[PunRPC]
	public void RemoteCritterActorReleased(int releasedActorID, bool keepWorldPosition, Quaternion rotation, Vector3 position, Vector3 velocity, Vector3 angularVelocity, PhotonMessageInfo info)
	{
		if (!this.LocalAuthority())
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		if ((in rotation).IsValid())
		{
			float num = 10000f;
			if ((in position).IsValid(in num))
			{
				float num2 = 10000f;
				if ((in velocity).IsValid(in num2))
				{
					float num3 = 10000f;
					if ((in angularVelocity).IsValid(in num3))
					{
						this.CheckValidRemoteActorRelease(releasedActorID, keepWorldPosition, rotation, position, velocity, angularVelocity, info);
						return;
					}
				}
			}
		}
	}

	// Token: 0x0600023E RID: 574 RVA: 0x0000D450 File Offset: 0x0000B650
	[PunRPC]
	public void RemoteSpawnCreature(int actorID, int regionId, object[] spawnData, PhotonMessageInfo info)
	{
		if (!this.SenderIsOwner(info))
		{
			this.OwnerSentError(info);
			return;
		}
		if (!this.localInZone)
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		if (!CritterAppearance.ValidateData(spawnData))
		{
			return;
		}
		CrittersActor crittersActor;
		if (this.actorById.TryGetValue(actorID, out crittersActor))
		{
			CrittersPawn crittersPawn = (CrittersPawn)crittersActor;
			this.SetCritterRegion(crittersPawn, regionId);
			crittersPawn.SetSpawnData(spawnData);
		}
	}

	// Token: 0x0600023F RID: 575 RVA: 0x0000D4C0 File Offset: 0x0000B6C0
	[PunRPC]
	public void RemoteCrittersActorGrabbedby(int grabbedActorID, int grabberActorID, Quaternion offsetRotation, Vector3 offsetPosition, bool isGrabDisabled, PhotonMessageInfo info)
	{
		if (!this.LocalAuthority())
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		if ((in offsetRotation).IsValid())
		{
			float num = 10000f;
			if ((in offsetPosition).IsValid(in num))
			{
				this.CheckValidRemoteActorGrab(grabbedActorID, grabberActorID, offsetRotation, offsetPosition, isGrabDisabled, info);
				return;
			}
		}
	}

	// Token: 0x06000240 RID: 576 RVA: 0x0000D518 File Offset: 0x0000B718
	[PunRPC]
	public void RemoteUpdatePlayerCrittersActorData(object[] data, PhotonMessageInfo info)
	{
		if (!this.SenderIsOwner(info))
		{
			this.OwnerSentError(info);
			return;
		}
		if (!this.localInZone)
		{
			return;
		}
		if (data == null)
		{
			return;
		}
		CrittersActor crittersActor;
		for (int i = 0; i < data.Length; i += crittersActor.UpdatePlayerCrittersActorFromRPC(data, i))
		{
			int num;
			if (!CrittersManager.ValidateDataType<int>(data[i], out num))
			{
				return;
			}
			if (!this.actorById.TryGetValue(num, out crittersActor))
			{
				return;
			}
		}
	}

	// Token: 0x06000241 RID: 577 RVA: 0x0000D578 File Offset: 0x0000B778
	[PunRPC]
	public void RemoteUpdateCritterData(object[] data, PhotonMessageInfo info)
	{
		if (!this.SenderIsOwner(info))
		{
			this.OwnerSentError(info);
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		if (!this.localInZone)
		{
			return;
		}
		if (data == null)
		{
			return;
		}
		CrittersActor crittersActor;
		for (int i = 0; i < data.Length; i += crittersActor.UpdateFromRPC(data, i))
		{
			int num;
			if (!CrittersManager.ValidateDataType<int>(data[i], out num))
			{
				return;
			}
			if (!this.actorById.TryGetValue(num, out crittersActor))
			{
				return;
			}
		}
	}

	// Token: 0x06000242 RID: 578 RVA: 0x0000D5EC File Offset: 0x0000B7EC
	public CrittersActor SpawnActor(CrittersActor.CrittersActorType type, int subObjectIndex = -1)
	{
		List<CrittersActor> list;
		if (!this.actorPools.TryGetValue(type, out list))
		{
			return null;
		}
		int num = this.poolIndexDict[type];
		for (int i = 0; i < list.Count; i++)
		{
			if (!list[(i + num) % list.Count].gameObject.activeSelf)
			{
				num = (i + num) % list.Count;
				this.poolIndexDict[type] = num + 1;
				list[num].subObjectIndex = subObjectIndex;
				list[num].gameObject.SetActive(true);
				return list[num];
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			CrittersActor crittersActor = list[j];
			int num2 = this.actorBinIndices[crittersActor];
			if (!this.priorityBins[num2])
			{
				list[j].gameObject.SetActive(false);
				list[j].subObjectIndex = subObjectIndex;
				list[j].gameObject.SetActive(true);
				return list[j];
			}
		}
		return null;
	}

	// Token: 0x06000243 RID: 579 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06000244 RID: 580 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06000245 RID: 581 RVA: 0x0000D6F4 File Offset: 0x0000B8F4
	public void PopulatePools()
	{
		this.binDimensionXMin = this.crittersRange.position.x - this.crittersRange.localScale.x / 2f;
		this.binDimensionZMin = this.crittersRange.position.z - this.crittersRange.localScale.z / 2f;
		this.xLength = this.crittersRange.localScale.x;
		this.zLength = this.crittersRange.localScale.z;
		float num = this.xLength * this.zLength / (float)this.totalBinsApproximate;
		this.individualBinSide = Mathf.Sqrt(num);
		this.binXCount = Mathf.CeilToInt(this.xLength / this.individualBinSide);
		this.binZCount = Mathf.CeilToInt(this.zLength / this.individualBinSide);
		int num2 = this.binXCount * this.binZCount;
		this.actorBins = new List<CrittersActor>[num2];
		for (int i = 0; i < num2; i++)
		{
			this.actorBins[i] = new List<CrittersActor>();
		}
		this.priorityBins = new bool[num2];
		this.actorBinIndices = new Dictionary<CrittersActor, int>();
		this.nearbyActors = new List<CrittersActor>();
		this.allActors = new List<CrittersActor>();
		this.actorPools = new Dictionary<CrittersActor.CrittersActorType, List<CrittersActor>>();
		this.actorPools.Add(CrittersActor.CrittersActorType.Bag, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.Cage, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.Food, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.Creature, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.LoudNoise, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.Grabber, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.FoodSpawner, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.AttachPoint, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.StunBomb, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.BodyAttachPoint, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.NoiseMaker, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.StickyTrap, new List<CrittersActor>());
		this.actorPools.Add(CrittersActor.CrittersActorType.StickyGoo, new List<CrittersActor>());
		this.actorById = new Dictionary<int, CrittersActor>();
		this.universalActorId = 0;
		GameObject gameObject = new GameObject();
		gameObject.transform.parent = base.transform;
		this.poolParent = gameObject.transform;
		this.poolParent.name = "Critter Actors Pool Parent";
		List<CrittersActor> list;
		this.actorPools.TryGetValue(CrittersActor.CrittersActorType.Food, out list);
		this.persistentActors = global::UnityEngine.Object.FindObjectsByType<CrittersActor>(FindObjectsSortMode.InstanceID).ToList<CrittersActor>();
		this.persistentActors.Sort((CrittersActor x, CrittersActor y) => x.transform.position.magnitude.CompareTo(y.transform.position.magnitude));
		this.persistentActors.Sort((CrittersActor x, CrittersActor y) => x.gameObject.name.CompareTo(y.gameObject.name));
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.bagPrefab, CrittersActor.CrittersActorType.Bag, gameObject.transform, 80, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.cagePrefab, CrittersActor.CrittersActorType.Cage, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.foodPrefab, CrittersActor.CrittersActorType.Food, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.creaturePrefab, CrittersActor.CrittersActorType.Creature, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.noisePrefab, CrittersActor.CrittersActorType.LoudNoise, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.grabberPrefab, CrittersActor.CrittersActorType.Grabber, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.foodSpawnerPrefab, CrittersActor.CrittersActorType.FoodSpawner, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.bodyAttachPointPrefab, CrittersActor.CrittersActorType.BodyAttachPoint, gameObject.transform, 40, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, null, CrittersActor.CrittersActorType.AttachPoint, gameObject.transform, 0, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.stunBombPrefab, CrittersActor.CrittersActorType.StunBomb, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.noiseMakerPrefab, CrittersActor.CrittersActorType.NoiseMaker, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.stickyTrapPrefab, CrittersActor.CrittersActorType.StickyTrap, gameObject.transform, this.poolCount, this.persistentActors);
		this.UpdatePool<CrittersActor>(ref this.actorPools, this.stickyGooPrefab, CrittersActor.CrittersActorType.StickyGoo, gameObject.transform, this.poolCount, this.persistentActors);
	}

	// Token: 0x06000246 RID: 582 RVA: 0x0000DBC4 File Offset: 0x0000BDC4
	public void UpdatePool<T>(ref Dictionary<CrittersActor.CrittersActorType, List<T>> dict, GameObject prefab, CrittersActor.CrittersActorType crittersActorType, Transform parent, int poolAmount, List<CrittersActor> sceneActors) where T : CrittersActor
	{
		int num = 0;
		for (int i = 0; i < sceneActors.Count; i++)
		{
			if (sceneActors[i].crittersActorType == crittersActorType)
			{
				dict[crittersActorType].Add((T)((object)sceneActors[i]));
				sceneActors[i].actorId = this.universalActorId;
				this.actorById.Add(this.universalActorId, sceneActors[i]);
				this.allActors.Add(sceneActors[i]);
				this.universalActorId++;
				num++;
				if (sceneActors[i].enabled)
				{
					if (crittersActorType == CrittersActor.CrittersActorType.Creature)
					{
						CrittersManager.RegisterCritter(sceneActors[i] as CrittersPawn);
					}
					else
					{
						CrittersManager.RegisterActor(sceneActors[i]);
					}
				}
			}
		}
		for (int j = 0; j < poolAmount - num; j++)
		{
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(prefab);
			gameObject.transform.parent = parent;
			gameObject.name += j.ToString();
			gameObject.SetActive(false);
			T component = gameObject.GetComponent<T>();
			dict[crittersActorType].Add(component);
			component.actorId = this.universalActorId;
			component.SetDefaultParent(parent);
			this.actorById.Add(this.universalActorId, component);
			this.allActors.Add(component);
			this.universalActorId++;
		}
		this.poolIndexDict[crittersActorType] = 0;
	}

	// Token: 0x06000247 RID: 583 RVA: 0x0000DD58 File Offset: 0x0000BF58
	public void TriggerEvent(CrittersManager.CritterEvent eventType, int sourceActor, Vector3 position, Quaternion rotation)
	{
		Action<CrittersManager.CritterEvent, int, Vector3, Quaternion> onCritterEventReceived = this.OnCritterEventReceived;
		if (onCritterEventReceived != null)
		{
			onCritterEventReceived(eventType, sourceActor, position, rotation);
		}
		if (!this.LocalAuthority() || !NetworkSystem.Instance.InRoom)
		{
			return;
		}
		base.SendRPC("RemoteReceivedCritterEvent", RpcTarget.Others, new object[] { eventType, sourceActor, position, rotation });
	}

	// Token: 0x06000248 RID: 584 RVA: 0x0000DDC7 File Offset: 0x0000BFC7
	public void TriggerEvent(CrittersManager.CritterEvent eventType, int sourceActor, Vector3 position)
	{
		this.TriggerEvent(eventType, sourceActor, position, Quaternion.identity);
	}

	// Token: 0x06000249 RID: 585 RVA: 0x0000DDD8 File Offset: 0x0000BFD8
	[PunRPC]
	public void RemoteReceivedCritterEvent(CrittersManager.CritterEvent eventType, int sourceActor, Vector3 position, Quaternion rotation, PhotonMessageInfo info)
	{
		if (!this.localInZone)
		{
			return;
		}
		if (!this.SenderIsOwner(info))
		{
			this.OwnerSentError(info);
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		float num = 10000f;
		if (!(in position).IsValid(in num) || !(in rotation).IsValid())
		{
			return;
		}
		if (!this.critterEventCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		Action<CrittersManager.CritterEvent, int, Vector3, Quaternion> onCritterEventReceived = this.OnCritterEventReceived;
		if (onCritterEventReceived == null)
		{
			return;
		}
		onCritterEventReceived(eventType, sourceActor, position, rotation);
	}

	// Token: 0x0600024A RID: 586 RVA: 0x0000DE5A File Offset: 0x0000C05A
	public static bool ValidateDataType<T>(object obj, out T dataAsType)
	{
		if (obj is T)
		{
			dataAsType = (T)((object)obj);
			return true;
		}
		dataAsType = default(T);
		return false;
	}

	// Token: 0x0600024B RID: 587 RVA: 0x0000DE7C File Offset: 0x0000C07C
	public void CheckValidRemoteActorRelease(int releasedActorID, bool keepWorldPosition, Quaternion rotation, Vector3 position, Vector3 velocity, Vector3 angularVelocity, PhotonMessageInfo info)
	{
		CrittersActor crittersActor;
		if (!this.actorById.TryGetValue(releasedActorID, out crittersActor))
		{
			return;
		}
		CrittersActor crittersActor2 = this.TopLevelCritterGrabber(crittersActor);
		(ref rotation).SetValueSafe(in rotation);
		(ref position).SetValueSafe(in position);
		(ref velocity).SetValueSafe(in velocity);
		(ref angularVelocity).SetValueSafe(in angularVelocity);
		if (crittersActor2 != null && crittersActor2 is CrittersGrabber && crittersActor2.isOnPlayer && crittersActor2.rigPlayerId == info.Sender.ActorNumber)
		{
			crittersActor.Released(keepWorldPosition, rotation, position, velocity, angularVelocity);
		}
	}

	// Token: 0x0600024C RID: 588 RVA: 0x0000DF04 File Offset: 0x0000C104
	private void CheckValidRemoteActorGrab(int actorBeingGrabbedActorID, int grabbingActorID, Quaternion offsetRotation, Vector3 offsetPosition, bool isGrabDisabled, PhotonMessageInfo info)
	{
		CrittersActor crittersActor;
		CrittersActor crittersActor2;
		if (!this.actorById.TryGetValue(actorBeingGrabbedActorID, out crittersActor) || !this.actorById.TryGetValue(grabbingActorID, out crittersActor2))
		{
			return;
		}
		(ref offsetRotation).SetValueSafe(in offsetRotation);
		(ref offsetPosition).SetValueSafe(in offsetPosition);
		if ((crittersActor.transform.position - crittersActor2.transform.position).magnitude > this.maxGrabDistance || offsetPosition.magnitude > this.maxGrabDistance)
		{
			return;
		}
		if (((crittersActor2.crittersActorType == CrittersActor.CrittersActorType.Grabber && crittersActor2.isOnPlayer && crittersActor2.rigPlayerId == info.Sender.ActorNumber) || crittersActor2.crittersActorType != CrittersActor.CrittersActorType.Grabber) && crittersActor.AllowGrabbingActor(crittersActor2))
		{
			crittersActor.GrabbedBy(crittersActor2, true, offsetRotation, offsetPosition, isGrabDisabled);
		}
	}

	// Token: 0x0600024D RID: 589 RVA: 0x0000DFC8 File Offset: 0x0000C1C8
	private CrittersActor TopLevelCritterGrabber(CrittersActor baseActor)
	{
		CrittersActor crittersActor = null;
		this.actorById.TryGetValue(baseActor.parentActorId, out crittersActor);
		while (crittersActor != null && crittersActor.parentActorId != -1)
		{
			this.actorById.TryGetValue(crittersActor.parentActorId, out crittersActor);
		}
		return crittersActor;
	}

	// Token: 0x0600024E RID: 590 RVA: 0x0000E014 File Offset: 0x0000C214
	public static CapsuleCollider DuplicateCapsuleCollider(Transform targetTransform, CapsuleCollider sourceCollider)
	{
		if (sourceCollider == null)
		{
			return null;
		}
		CapsuleCollider capsuleCollider = new GameObject().AddComponent<CapsuleCollider>();
		capsuleCollider.transform.rotation = sourceCollider.transform.rotation;
		capsuleCollider.transform.position = sourceCollider.transform.position;
		capsuleCollider.transform.localScale = sourceCollider.transform.lossyScale;
		capsuleCollider.radius = sourceCollider.radius;
		capsuleCollider.height = sourceCollider.height;
		capsuleCollider.center = sourceCollider.center;
		capsuleCollider.gameObject.layer = targetTransform.gameObject.layer;
		capsuleCollider.transform.SetParent(targetTransform.transform);
		return capsuleCollider;
	}

	// Token: 0x0600024F RID: 591 RVA: 0x0000E0C4 File Offset: 0x0000C2C4
	private void HandleZonesAndOwnership()
	{
		bool flag = this.localInZone;
		this.localInZone = ZoneManagement.IsInZone(GTZone.critters);
		this.CheckOwnership();
		if (this.localInZone)
		{
			if (NetworkSystem.Instance.InRoom && this.guard.actualOwner != null && (!this.hasNewlyInitialized || !flag) && Time.time > this.lastRequest + this.initRequestCooldown && !this.LocalAuthority())
			{
				this.lastRequest = Time.time;
				this.hasNewlyInitialized = true;
				base.SendRPC("RequestDataInitialization", this.guard.actualOwner, Array.Empty<object>());
			}
			if (!flag)
			{
				this.poolParent.gameObject.SetActive(true);
				this.crittersPool.poolParent.gameObject.SetActive(true);
				return;
			}
		}
		else if (flag)
		{
			this.ResetRoom();
			this.poolParent.gameObject.SetActive(false);
			this.crittersPool.poolParent.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000250 RID: 592 RVA: 0x0000E1C0 File Offset: 0x0000C3C0
	private void CheckOwnership()
	{
		if (!PhotonNetwork.InRoom && base.IsMine)
		{
			if (this.guard.actualOwner == null || !this.guard.actualOwner.Equals(NetworkSystem.Instance.LocalPlayer))
			{
				this.guard.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
			}
			return;
		}
		if (this.allRigs == null)
		{
			if (!VRRigCache.isInitialized)
			{
				return;
			}
			this.allRigs = new List<VRRig>(VRRigCache.Instance.GetAllRigs());
		}
		if (!this.LocalAuthority())
		{
			return;
		}
		if (this.localInZone)
		{
			return;
		}
		int num = int.MaxValue;
		NetPlayer netPlayer = null;
		for (int i = 0; i < this.allRigs.Count; i++)
		{
			NetPlayer creator = this.allRigs[i].creator;
			if (creator != null && this.allRigs[i].zoneEntity.currentZone == GTZone.critters && creator.ActorNumber < num)
			{
				netPlayer = creator;
				num = creator.ActorNumber;
			}
		}
		if (netPlayer == null)
		{
			return;
		}
		this.guard.TransferOwnership(netPlayer, "");
	}

	// Token: 0x06000251 RID: 593 RVA: 0x0000E2C8 File Offset: 0x0000C4C8
	public bool LocalAuthority()
	{
		return !NetworkSystem.Instance.InRoom || (!(this.guard == null) && ((this.guard.actualOwner != null && this.guard.isTrulyMine) || (base.Owner != null && base.Owner.IsLocal) || this.guard.currentState == NetworkingState.IsOwner));
	}

	// Token: 0x06000252 RID: 594 RVA: 0x0000E334 File Offset: 0x0000C534
	private bool SenderIsOwner(PhotonMessageInfo info)
	{
		return (this.guard.actualOwner != null || base.Owner != null) && info.Sender != null && !this.LocalAuthority() && ((this.guard.actualOwner != null && this.guard.actualOwner.ActorNumber == info.Sender.ActorNumber) || (base.Owner != null && base.Owner.ActorNumber == info.Sender.ActorNumber));
	}

	// Token: 0x06000253 RID: 595 RVA: 0x0000E3B8 File Offset: 0x0000C5B8
	private void OwnerSentError(PhotonMessageInfo info)
	{
		NetPlayer owner = base.Owner;
	}

	// Token: 0x06000254 RID: 596 RVA: 0x0000E3C1 File Offset: 0x0000C5C1
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		NetPlayer localPlayer = NetworkSystem.Instance.LocalPlayer;
	}

	// Token: 0x06000255 RID: 597 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return false;
	}

	// Token: 0x06000256 RID: 598 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x06000257 RID: 599 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return false;
	}

	// Token: 0x06000258 RID: 600 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x0600025C RID: 604 RVA: 0x00002E6F File Offset: 0x0000106F
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x0600025D RID: 605 RVA: 0x00002E7B File Offset: 0x0000107B
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04000243 RID: 579
	public CritterIndex creatureIndex;

	// Token: 0x04000244 RID: 580
	public static volatile CrittersManager instance;

	// Token: 0x04000246 RID: 582
	public LayerMask movementLayers;

	// Token: 0x04000247 RID: 583
	public LayerMask objectLayers;

	// Token: 0x04000248 RID: 584
	public LayerMask containerLayer;

	// Token: 0x04000249 RID: 585
	[ReadOnly]
	public List<CrittersActor> crittersActors;

	// Token: 0x0400024A RID: 586
	[ReadOnly]
	public List<CrittersActor> allActors;

	// Token: 0x0400024B RID: 587
	[ReadOnly]
	public List<CrittersPawn> crittersPawns;

	// Token: 0x0400024C RID: 588
	[ReadOnly]
	public List<CrittersActor> despawnableActors;

	// Token: 0x0400024D RID: 589
	[ReadOnly]
	public List<CrittersActor> newlyDisabledActors;

	// Token: 0x0400024E RID: 590
	[ReadOnly]
	public List<CrittersRigActorSetup> rigActorSetups;

	// Token: 0x0400024F RID: 591
	[ReadOnly]
	public List<CrittersActorSpawner> actorSpawners;

	// Token: 0x04000250 RID: 592
	[NonSerialized]
	private List<CrittersActor> persistentActors = new List<CrittersActor>();

	// Token: 0x04000251 RID: 593
	public Dictionary<int, CrittersActor> actorById;

	// Token: 0x04000252 RID: 594
	public Dictionary<CrittersPawn, List<CrittersActor>> awareOfActors;

	// Token: 0x04000253 RID: 595
	public Dictionary<VRRig, CrittersRigActorSetup> rigSetupByRig;

	// Token: 0x04000254 RID: 596
	private int allActorsCount;

	// Token: 0x04000255 RID: 597
	public bool intialized;

	// Token: 0x04000256 RID: 598
	private List<int> updatesToSend;

	// Token: 0x04000257 RID: 599
	public int actorsPerInitializationCall = 5;

	// Token: 0x04000258 RID: 600
	public float actorsInitializationCallCooldown = 0.2f;

	// Token: 0x04000259 RID: 601
	public Transform poolParent;

	// Token: 0x0400025A RID: 602
	public List<object> objList;

	// Token: 0x0400025B RID: 603
	public double spawnDelay;

	// Token: 0x0400025C RID: 604
	private double lastSpawnTime;

	// Token: 0x0400025D RID: 605
	public float softJointGracePeriod = 0.1f;

	// Token: 0x0400025E RID: 606
	private List<CrittersRegion> _spawnRegions;

	// Token: 0x0400025F RID: 607
	private int _currentRegionIndex = -1;

	// Token: 0x04000260 RID: 608
	private static CrittersActorGrabber _rightGrabber;

	// Token: 0x04000261 RID: 609
	private static CrittersActorGrabber _leftGrabber;

	// Token: 0x04000262 RID: 610
	public float springForce = 1000f;

	// Token: 0x04000263 RID: 611
	public float springAngularForce = 100f;

	// Token: 0x04000264 RID: 612
	public float damperForce = 10f;

	// Token: 0x04000265 RID: 613
	public float damperAngularForce = 1f;

	// Token: 0x04000266 RID: 614
	public float lightMass = 0.05f;

	// Token: 0x04000267 RID: 615
	public float heavyMass = 2f;

	// Token: 0x04000268 RID: 616
	public float overlapDistanceMax = 0.01f;

	// Token: 0x04000269 RID: 617
	public float fastThrowThreshold = 3f;

	// Token: 0x0400026A RID: 618
	public float fastThrowMultiplier = 1.5f;

	// Token: 0x0400026B RID: 619
	private Dictionary<CrittersActor.CrittersActorType, int> poolIndexDict;

	// Token: 0x0400026D RID: 621
	public CrittersManager.AllowGrabbingFlags privateRoomGrabbingFlags;

	// Token: 0x0400026E RID: 622
	public CrittersManager.AllowGrabbingFlags publicRoomGrabbingFlags;

	// Token: 0x0400026F RID: 623
	public float MaxAttachSpeed = 0.04f;

	// Token: 0x04000270 RID: 624
	private float binDimensionXMin;

	// Token: 0x04000271 RID: 625
	private float binDimensionZMin;

	// Token: 0x04000272 RID: 626
	public Transform crittersRange;

	// Token: 0x04000273 RID: 627
	public int totalBinsApproximate = 400;

	// Token: 0x04000274 RID: 628
	private float xLength;

	// Token: 0x04000275 RID: 629
	private float zLength;

	// Token: 0x04000276 RID: 630
	private int binXCount;

	// Token: 0x04000277 RID: 631
	private int binZCount;

	// Token: 0x04000278 RID: 632
	private float individualBinSide;

	// Token: 0x04000279 RID: 633
	private List<CrittersActor>[] actorBins;

	// Token: 0x0400027A RID: 634
	private bool[] priorityBins;

	// Token: 0x0400027B RID: 635
	private Dictionary<CrittersActor, int> actorBinIndices;

	// Token: 0x0400027C RID: 636
	private List<CrittersActor> nearbyActors;

	// Token: 0x0400027D RID: 637
	private List<NetPlayer> playersToUpdate;

	// Token: 0x0400027E RID: 638
	public CrittersPool crittersPool;

	// Token: 0x0400027F RID: 639
	private int lowPriorityActorsPerFrame = 5;

	// Token: 0x04000280 RID: 640
	private int lowPriorityIndex;

	// Token: 0x04000281 RID: 641
	private int spawnerIndex;

	// Token: 0x04000282 RID: 642
	private int despawnIndex;

	// Token: 0x04000283 RID: 643
	private List<CrittersActor> lowPriorityPawnsToProcess;

	// Token: 0x04000284 RID: 644
	private Dictionary<CrittersActor.CrittersActorType, float> despawnDecayValue;

	// Token: 0x04000285 RID: 645
	public float decayRate = 60f;

	// Token: 0x04000286 RID: 646
	private CrittersActor.CrittersActorType[] actorTypes;

	// Token: 0x04000287 RID: 647
	public float maxGrabDistance = 25f;

	// Token: 0x04000288 RID: 648
	public RequestableOwnershipGuard guard;

	// Token: 0x04000289 RID: 649
	private List<VRRig> allRigs;

	// Token: 0x0400028A RID: 650
	private bool localInZone;

	// Token: 0x0400028B RID: 651
	private List<int> updatingPlayers;

	// Token: 0x0400028C RID: 652
	private bool hasNewlyInitialized;

	// Token: 0x0400028D RID: 653
	private float initRequestCooldown = 10f;

	// Token: 0x0400028E RID: 654
	private float lastRequest;

	// Token: 0x0400028F RID: 655
	public int poolCount = 100;

	// Token: 0x04000290 RID: 656
	public int despawnThreshold = 20;

	// Token: 0x04000291 RID: 657
	private Dictionary<CrittersActor.CrittersActorType, int> poolCounts;

	// Token: 0x04000292 RID: 658
	private Dictionary<CrittersActor.CrittersActorType, List<CrittersActor>> actorPools;

	// Token: 0x04000293 RID: 659
	public GameObject foodPrefab;

	// Token: 0x04000294 RID: 660
	public GameObject creaturePrefab;

	// Token: 0x04000295 RID: 661
	public GameObject noisePrefab;

	// Token: 0x04000296 RID: 662
	public GameObject grabberPrefab;

	// Token: 0x04000297 RID: 663
	public GameObject cagePrefab;

	// Token: 0x04000298 RID: 664
	public GameObject foodSpawnerPrefab;

	// Token: 0x04000299 RID: 665
	public GameObject stunBombPrefab;

	// Token: 0x0400029A RID: 666
	public GameObject bodyAttachPointPrefab;

	// Token: 0x0400029B RID: 667
	public GameObject bagPrefab;

	// Token: 0x0400029C RID: 668
	public GameObject noiseMakerPrefab;

	// Token: 0x0400029D RID: 669
	public GameObject stickyTrapPrefab;

	// Token: 0x0400029E RID: 670
	public GameObject stickyGooPrefab;

	// Token: 0x0400029F RID: 671
	public int universalActorId;

	// Token: 0x040002A0 RID: 672
	public int rigActorId;

	// Token: 0x040002A2 RID: 674
	private CallLimiter critterEventCallLimit = new CallLimiter(10, 0.5f, 0.5f);

	// Token: 0x02000068 RID: 104
	[Flags]
	public enum AllowGrabbingFlags
	{
		// Token: 0x040002A4 RID: 676
		None = 0,
		// Token: 0x040002A5 RID: 677
		OutOfHands = 1,
		// Token: 0x040002A6 RID: 678
		FromBags = 2,
		// Token: 0x040002A7 RID: 679
		EntireBag = 4
	}

	// Token: 0x02000069 RID: 105
	public enum CritterEvent
	{
		// Token: 0x040002A9 RID: 681
		StunExplosion,
		// Token: 0x040002AA RID: 682
		NoiseMakerTriggered,
		// Token: 0x040002AB RID: 683
		StickyDeployed,
		// Token: 0x040002AC RID: 684
		StickyTriggered
	}
}
