using System;
using System.Collections.Generic;
using GorillaGameModes;
using TMPro;
using UnityEngine;

// Token: 0x02000180 RID: 384
[DefaultExecutionOrder(1)]
public class SuperInfection : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x06000A21 RID: 2593 RVA: 0x00036357 File Offset: 0x00034557
	public bool IsAuthorityAndActive
	{
		get
		{
			return !(this.siManager == null) && this.siManager.gameEntityManager.IsAuthority() && this.siManager.gameEntityManager.IsZoneActive();
		}
	}

	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x06000A22 RID: 2594 RVA: 0x0003638D File Offset: 0x0003458D
	public float ResourceSpawnInterval
	{
		get
		{
			if (!Application.isPlaying)
			{
				return 0f;
			}
			return this.GetResourceSpawnInterval();
		}
	}

	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x06000A23 RID: 2595 RVA: 0x000363A2 File Offset: 0x000345A2
	public float TimeSinceLastSpawn
	{
		get
		{
			return Time.time - this._lastResourceSpawnTime;
		}
	}

	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x06000A24 RID: 2596 RVA: 0x000363B0 File Offset: 0x000345B0
	public float TimeToNextSpawn
	{
		get
		{
			if (!Application.isPlaying)
			{
				return 0f;
			}
			if (this._lastResourceSpawnTime <= 0f)
			{
				return 0f;
			}
			return this.GetResourceSpawnInterval() - (Time.time - this._lastResourceSpawnTime);
		}
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x000363E8 File Offset: 0x000345E8
	private void Awake()
	{
		this.resourceRegions = ((this.resourceNodeParent != null) ? this.resourceNodeParent.GetComponentsInChildren<SIResourceRegion>(true) : Array.Empty<SIResourceRegion>());
		this._resourcePrefabs = new List<SIResource>();
		foreach (SIResourceRegion siresourceRegion in this.resourceRegions)
		{
			if (!this._resourcePrefabs.Contains(siresourceRegion.resourcePrefab))
			{
				this._resourcePrefabs.Add(siresourceRegion.resourcePrefab);
			}
		}
		this.perRoundResourceRegions = ((this.perRoundResourceNodeParent != null) ? this.perRoundResourceNodeParent.GetComponentsInChildren<SIResourceRegion>(true) : Array.Empty<SIResourceRegion>());
		this.resourceResetHeight = ((this.resourceResetLoc != null) ? this.resourceResetLoc.position.y : float.MinValue);
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x000364B8 File Offset: 0x000346B8
	public void OnEnable()
	{
		this.siManager = SuperInfectionManager.GetSIManagerForZone(this.zone);
		if (this.siManager != null)
		{
			this.siManager.OnEnableZoneSuperInfection(this);
		}
		if (this.siManager == null || this.siManager.isActiveAndEnabled)
		{
			this.DisableStations();
		}
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnPlayerLeft += this.RemovePlayerGadgetsOnLeave;
		}
		for (int i = 0; i < this.siTerminals.Length; i++)
		{
			this.siTerminals[i].index = i;
		}
		for (int j = 0; j < this.siDeposits.Length; j++)
		{
			this.siDeposits[j].index = j;
		}
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x00036588 File Offset: 0x00034788
	public void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.siManager)
		{
			this.siManager.zoneSuperInfection = null;
		}
		this.DisableStations();
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnPlayerLeft -= this.RemovePlayerGadgetsOnLeave;
		}
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x000365F2 File Offset: 0x000347F2
	public void OnZoneInit()
	{
		this.RebuildRegionItemsFromEntities();
		this.EnableStations();
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x00036600 File Offset: 0x00034800
	public void OnZoneClear(ZoneClearReason reason)
	{
		if (reason != ZoneClearReason.JoinZone)
		{
			this.DisableStations();
			SIProgression.Instance.SendTelemetryData();
		}
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x00036618 File Offset: 0x00034818
	private void EnableStations()
	{
		for (int i = 0; i < this.siTerminals.Length; i++)
		{
			this.siTerminals[i].gameObject.SetActive(true);
			if (this.siTerminals[i].dispenser && this.siTerminals[i].dispenser.isTryOn && this.siManager != null)
			{
				this.siManager.RegisterTryOnDispenser();
			}
		}
		for (int j = 0; j < this.siDeposits.Length; j++)
		{
			this.siDeposits[j].gameObject.SetActive(true);
		}
		if (this.questBoard != null)
		{
			this.questBoard.gameObject.SetActive(true);
		}
		if (this.purchaseTerminal != null)
		{
			this.purchaseTerminal.gameObject.SetActive(true);
		}
		for (int k = 0; k < this.zoneObjects.Length; k++)
		{
			GameObject gameObject = this.zoneObjects[k];
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
			else
			{
				Debug.LogError("[GT/SuperInfection]  ERROR!!!  " + string.Format("null ref at `zoneObjects[{0}]`.", k));
			}
		}
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x00036740 File Offset: 0x00034940
	private void DisableStations()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		for (int i = 0; i < this.siTerminals.Length; i++)
		{
			if (this.siManager != null && this.siTerminals[i].dispenser && this.siTerminals[i].dispenser.isTryOn)
			{
				this.siManager.UnregisterTryOnDispenser();
			}
			this.siTerminals[i].gameObject.SetActive(false);
			this.siTerminals[i].Reset();
		}
		for (int j = 0; j < this.siDeposits.Length; j++)
		{
			this.siDeposits[j].gameObject.SetActive(false);
		}
		if (this.questBoard != null)
		{
			this.questBoard.gameObject.SetActive(false);
		}
		if (this.purchaseTerminal != null)
		{
			this.purchaseTerminal.gameObject.SetActive(false);
		}
		for (int k = 0; k < this.zoneObjects.Length; k++)
		{
			GameObject gameObject = this.zoneObjects[k];
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
			else
			{
				Debug.LogError("[GT/SuperInfection]  ERROR!!!  " + string.Format("null ref at `zoneObjects[{0}]`.", k));
			}
		}
	}

	// Token: 0x06000A2C RID: 2604 RVA: 0x0003687C File Offset: 0x00034A7C
	private void UpdateResources()
	{
		if (!this.siManager.gameEntityManager.IsZoneActive())
		{
			return;
		}
		if (this.retryCreatePerRoundResources)
		{
			this.CreatePerRoundResources();
		}
		if (Time.time >= this._nextResourceUpdateTime)
		{
			this.GetResourceSpawnInterval();
			foreach (SIResourceRegion siresourceRegion in this.resourceRegions)
			{
				for (int j = siresourceRegion.ItemCount - 1; j >= 0; j--)
				{
					GameEntity gameEntity = siresourceRegion.Items[j];
					if (!gameEntity)
					{
						siresourceRegion.Items.RemoveAt(j);
					}
					else if (gameEntity.transform.position.y < this.resourceResetHeight)
					{
						this.siManager.gameEntityManager.RequestDestroyItem(gameEntity.id);
					}
				}
			}
			this.CheckResourceSpawn();
			this._nextResourceUpdateTime = Time.time + 1f;
		}
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x0003695C File Offset: 0x00034B5C
	private void CheckResourceSpawn()
	{
		if (Time.time >= this.GetNextResourceSpawnTime())
		{
			SIResourceRegion siresourceRegion = null;
			float num = float.MaxValue;
			foreach (SIResourceRegion siresourceRegion2 in this.resourceRegions)
			{
				if (siresourceRegion2.ItemCount < siresourceRegion2.MaxItems && siresourceRegion2.LastSpawnTime < num)
				{
					siresourceRegion = siresourceRegion2;
					num = siresourceRegion2.LastSpawnTime;
				}
			}
			if (!siresourceRegion)
			{
				this._lastResourceSpawnTime = Time.time;
				return;
			}
			ValueTuple<bool, Vector3, Vector3> spawnPointWithNormal = siresourceRegion.GetSpawnPointWithNormal(5);
			if (!spawnPointWithNormal.Item1)
			{
				return;
			}
			if (siresourceRegion.resourcePrefab == null)
			{
				return;
			}
			float spawnPitchVariance = siresourceRegion.resourcePrefab.spawnPitchVariance;
			Quaternion quaternion = Quaternion.Euler(Random.Range(-spawnPitchVariance, spawnPitchVariance), (float)Random.Range(0, 360), Random.Range(-spawnPitchVariance, spawnPitchVariance));
			Quaternion quaternion2 = Quaternion.LookRotation(Vector3.ProjectOnPlane(Vector3.forward, spawnPointWithNormal.Item3), spawnPointWithNormal.Item3) * quaternion;
			GameEntity gameEntity = this.siManager.gameEntityManager.GetGameEntity(this.siManager.gameEntityManager.RequestCreateItem(siresourceRegion.resourcePrefab.gameObject.name.GetStaticHash(), spawnPointWithNormal.Item2, quaternion2, 0L));
			if (gameEntity)
			{
				GTDev.Log<string>(string.Format("Spawned {0} at {1}", gameEntity.name, spawnPointWithNormal.Item2), gameEntity, null);
				siresourceRegion.AddItem(gameEntity);
				siresourceRegion.LastSpawnTime = (this._lastResourceSpawnTime = Time.time);
				return;
			}
			GTDev.LogError<string>(string.Format("Failed to spawn {0} at {1}", siresourceRegion.resourcePrefab.gameObject.name, spawnPointWithNormal.Item2), null);
		}
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x00036B07 File Offset: 0x00034D07
	private float GetNextResourceSpawnTime()
	{
		if (this._lastResourceSpawnTime <= 0f)
		{
			return 0f;
		}
		return this._lastResourceSpawnTime + this.GetResourceSpawnInterval();
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x00036B29 File Offset: 0x00034D29
	private float GetResourceSpawnInterval()
	{
		return 3600f / (float)(this.perPlayerHourlyResourceRate * Mathf.Max(GameMode.ParticipatingPlayers.Count, this.minRoomPopulation));
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x00036B50 File Offset: 0x00034D50
	public void RemovePlayerGadgetsOnLeave(NetPlayer player)
	{
		SIPlayer siplayer = SIPlayer.Get(player.ActorNumber);
		if (siplayer == null)
		{
			return;
		}
		if (this.siManager.gameEntityManager.IsAuthority())
		{
			for (int i = siplayer.activePlayerGadgets.Count - 1; i >= 0; i--)
			{
				this.siManager.gameEntityManager.RequestDestroyItem(this.siManager.gameEntityManager.GetGameEntityFromNetId(siplayer.activePlayerGadgets[i]).id);
			}
		}
		siplayer.activePlayerGadgets.Clear();
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x00036BDC File Offset: 0x00034DDC
	public void RefreshStations(int actorNr)
	{
		for (int i = 0; i < this.siTerminals.Length; i++)
		{
			if (!(this.siTerminals[i].activePlayer == null) && this.siTerminals[i].activePlayer.gameObject.activeInHierarchy && this.siTerminals[i].activePlayer.ActorNr == actorNr)
			{
				this.siTerminals[i].techTree.UpdateState(this.siTerminals[i].techTree.currentState);
				this.siTerminals[i].resourceCollection.UpdateState(this.siTerminals[i].resourceCollection.currentState);
				this.siTerminals[i].dispenser.UpdateState(this.siTerminals[i].dispenser.currentState);
			}
		}
		if (SIPlayer.LocalPlayer.ActorNr == actorNr && this.purchaseTerminal != null)
		{
			this.purchaseTerminal.UpdateCurrentTechPoints();
		}
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x00036CE0 File Offset: 0x00034EE0
	public void SliceUpdate()
	{
		if (this.siManager == null)
		{
			this.siManager = SuperInfectionManager.GetSIManagerForZone(this.zone);
			return;
		}
		if (!NetworkSystem.Instance.InRoom || !SuperInfectionManager.IsSuperGameMode())
		{
			return;
		}
		if (this.siManager.gameEntityManager.IsAuthority())
		{
			for (int i = this.activeGadgets.Count - 1; i >= 0; i--)
			{
				if (this.activeGadgets[i] == null)
				{
					this.activeGadgets.RemoveAt(i);
				}
				else if (this.activeGadgets[i].transform.position.y < this.resourceResetHeight)
				{
					this.siManager.gameEntityManager.RequestDestroyItem(this.activeGadgets[i].gameEntity.id);
				}
			}
			this.UpdateResources();
		}
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x00036DC0 File Offset: 0x00034FC0
	private void RebuildRegionItemsFromEntities()
	{
		if (this.resourceRegions.Length + this.perRoundResourceRegions.Length == 0)
		{
			return;
		}
		int[] array = new int[this.resourceRegions.Length];
		for (int i = 0; i < this.resourceRegions.Length; i++)
		{
			this.resourceRegions[i].Items.Clear();
			array[i] = ((this.resourceRegions[i].resourcePrefab != null) ? this.resourceRegions[i].resourcePrefab.gameObject.name.GetStaticHash() : 0);
		}
		int[] array2 = new int[this.perRoundResourceRegions.Length];
		for (int j = 0; j < this.perRoundResourceRegions.Length; j++)
		{
			this.perRoundResourceRegions[j].Items.Clear();
			array2[j] = ((this.perRoundResourceRegions[j].resourcePrefab != null) ? this.perRoundResourceRegions[j].resourcePrefab.gameObject.name.GetStaticHash() : 0);
		}
		List<GameEntity> gameEntities = this.siManager.gameEntityManager.GetGameEntities();
		int num = 0;
		int num2 = 0;
		for (int k = 0; k < gameEntities.Count; k++)
		{
			GameEntity gameEntity = gameEntities[k];
			if (!(gameEntity == null) && !(gameEntity.GetComponent<SIResource>() == null))
			{
				int typeId = gameEntity.typeId;
				bool flag = false;
				SIResourceRegion siresourceRegion = null;
				int num3 = int.MaxValue;
				for (int l = 0; l < this.resourceRegions.Length; l++)
				{
					if (array[l] == typeId && this.resourceRegions[l].ItemCount < this.resourceRegions[l].MaxItems && this.resourceRegions[l].ItemCount < num3)
					{
						siresourceRegion = this.resourceRegions[l];
						num3 = this.resourceRegions[l].ItemCount;
					}
				}
				if (siresourceRegion != null)
				{
					siresourceRegion.AddItem(gameEntity);
					num++;
					flag = true;
				}
				if (!flag)
				{
					for (int m = 0; m < this.perRoundResourceRegions.Length; m++)
					{
						if (array2[m] == typeId && this.perRoundResourceRegions[m].ItemCount < this.perRoundResourceRegions[m].MaxItems)
						{
							this.perRoundResourceRegions[m].AddItem(gameEntity);
							num++;
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					num2++;
				}
			}
		}
		if (num2 > 0 && this.siManager.gameEntityManager.IsAuthority())
		{
			for (int n = gameEntities.Count - 1; n >= 0; n--)
			{
				GameEntity gameEntity2 = gameEntities[n];
				if (!(gameEntity2 == null) && !(gameEntity2.GetComponent<SIResource>() == null) && gameEntity2.heldByActorNumber == 0)
				{
					bool flag2 = false;
					int num4 = 0;
					while (num4 < this.resourceRegions.Length && !flag2)
					{
						flag2 = this.resourceRegions[num4].Items.Contains(gameEntity2);
						num4++;
					}
					int num5 = 0;
					while (num5 < this.perRoundResourceRegions.Length && !flag2)
					{
						flag2 = this.perRoundResourceRegions[num5].Items.Contains(gameEntity2);
						num5++;
					}
					if (!flag2)
					{
						this.siManager.gameEntityManager.RequestDestroyItem(gameEntity2.id);
					}
				}
			}
		}
		if (num > 0)
		{
			this._lastResourceSpawnTime = Time.time;
		}
	}

	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x06000A34 RID: 2612 RVA: 0x0003711D File Offset: 0x0003531D
	public List<SIResource> ResourcePrefabs
	{
		get
		{
			return this._resourcePrefabs;
		}
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x00037125 File Offset: 0x00035325
	public void AddGadget(SIGadget gadget)
	{
		this.activeGadgets.Add(gadget);
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x00037133 File Offset: 0x00035333
	public void RemoveGadget(SIGadget gadget)
	{
		this.activeGadgets.Remove(gadget);
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x00037142 File Offset: 0x00035342
	public void ResetPerRoundResources()
	{
		this.ClearPerRoundResources();
		this.CreatePerRoundResources();
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x00037150 File Offset: 0x00035350
	private void CreatePerRoundResources()
	{
		if (!this.siManager.gameEntityManager.IsZoneActive())
		{
			this.retryCreatePerRoundResources = true;
			return;
		}
		this.retryCreatePerRoundResources = false;
		foreach (SIResourceRegion siresourceRegion in this.perRoundResourceRegions)
		{
			for (int j = siresourceRegion.ItemCount; j < siresourceRegion.MaxItems; j++)
			{
				ValueTuple<bool, Vector3, Vector3> spawnPointWithNormal = siresourceRegion.GetSpawnPointWithNormal(5);
				Quaternion quaternion = Quaternion.LookRotation(Vector3.ProjectOnPlane(Vector3.forward, spawnPointWithNormal.Item3), spawnPointWithNormal.Item3) * Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f);
				GameEntity gameEntity = this.siManager.gameEntityManager.GetGameEntity(this.siManager.gameEntityManager.RequestCreateItem(siresourceRegion.resourcePrefab.gameObject.name.GetStaticHash(), spawnPointWithNormal.Item2, quaternion, 0L));
				if (gameEntity)
				{
					siresourceRegion.AddItem(gameEntity);
					if (!spawnPointWithNormal.Item1)
					{
						Rigidbody component = gameEntity.GetComponent<Rigidbody>();
						if (component != null)
						{
							component.isKinematic = false;
						}
					}
				}
				else
				{
					GTDev.LogError<string>(string.Format("Failed to spawn {0} at {1}", siresourceRegion.resourcePrefab.gameObject.name, spawnPointWithNormal.Item2), null);
				}
			}
		}
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x000372A0 File Offset: 0x000354A0
	private void ClearPerRoundResources()
	{
		foreach (SIResourceRegion siresourceRegion in this.perRoundResourceRegions)
		{
			for (int j = siresourceRegion.ItemCount - 1; j >= 0; j--)
			{
				GameEntity gameEntity = siresourceRegion.Items[j];
				if (!gameEntity)
				{
					siresourceRegion.Items.RemoveAt(j);
				}
				else if (gameEntity.lastHeldByActorNumber == 0 || !(SIPlayer.Get(gameEntity.lastHeldByActorNumber) != null))
				{
					this.siManager.gameEntityManager.RequestDestroyItem(gameEntity.id);
				}
			}
		}
	}

	// Token: 0x04000C61 RID: 3169
	private const string preLog = "[GT/SuperInfection]  ";

	// Token: 0x04000C62 RID: 3170
	private const string preErr = "[GT/SuperInfection]  ERROR!!!  ";

	// Token: 0x04000C63 RID: 3171
	public SICombinedTerminal[] siTerminals;

	// Token: 0x04000C64 RID: 3172
	public SIResourceDeposit[] siDeposits;

	// Token: 0x04000C65 RID: 3173
	public SIQuestBoard questBoard;

	// Token: 0x04000C66 RID: 3174
	public SIPurchaseTerminal purchaseTerminal;

	// Token: 0x04000C67 RID: 3175
	[Tooltip("Add miscellaneous zone objects here.  They'll be disabled when not in this mode.")]
	public GameObject[] zoneObjects;

	// Token: 0x04000C68 RID: 3176
	public Transform resourceNodeParent;

	// Token: 0x04000C69 RID: 3177
	public SIResourceRegion[] resourceRegions;

	// Token: 0x04000C6A RID: 3178
	public int perPlayerHourlyResourceRate = 20;

	// Token: 0x04000C6B RID: 3179
	[Tooltip("Resource generation rate varies based on population.  We'll assume at least this many players are present.")]
	public int minRoomPopulation = 4;

	// Token: 0x04000C6C RID: 3180
	public Transform perRoundResourceNodeParent;

	// Token: 0x04000C6D RID: 3181
	public SIResourceRegion[] perRoundResourceRegions;

	// Token: 0x04000C6E RID: 3182
	[NonSerialized]
	public SuperInfectionManager siManager;

	// Token: 0x04000C6F RID: 3183
	public Transform resourceResetLoc;

	// Token: 0x04000C70 RID: 3184
	private float resourceResetHeight;

	// Token: 0x04000C71 RID: 3185
	public List<SIGadget> activeGadgets = new List<SIGadget>();

	// Token: 0x04000C72 RID: 3186
	public GTZone zone;

	// Token: 0x04000C73 RID: 3187
	public SITechTreeSO techTreeSO;

	// Token: 0x04000C74 RID: 3188
	private bool retryCreatePerRoundResources;

	// Token: 0x04000C75 RID: 3189
	private float _nextResourceUpdateTime;

	// Token: 0x04000C76 RID: 3190
	private float _lastResourceSpawnTime;

	// Token: 0x04000C77 RID: 3191
	private int authorityActorNumber;

	// Token: 0x04000C78 RID: 3192
	public TextMeshProUGUI authorityName;

	// Token: 0x04000C79 RID: 3193
	private List<SIResource> _resourcePrefabs;
}
