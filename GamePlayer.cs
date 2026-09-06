using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020006F2 RID: 1778
public class GamePlayer : MonoBehaviour
{
	// Token: 0x17000464 RID: 1124
	// (get) Token: 0x06002CBE RID: 11454 RVA: 0x000F194D File Offset: 0x000EFB4D
	// (set) Token: 0x06002CBF RID: 11455 RVA: 0x000F1955 File Offset: 0x000EFB55
	public bool DidJoinWithItems { get; set; }

	// Token: 0x17000465 RID: 1125
	// (get) Token: 0x06002CC0 RID: 11456 RVA: 0x000F195E File Offset: 0x000EFB5E
	// (set) Token: 0x06002CC1 RID: 11457 RVA: 0x000F1966 File Offset: 0x000EFB66
	public bool AdditionalDataInitialized { get; set; }

	// Token: 0x17000466 RID: 1126
	// (get) Token: 0x06002CC2 RID: 11458 RVA: 0x000F196F File Offset: 0x000EFB6F
	public bool IsSubscribed
	{
		get
		{
			if (Time.frameCount != this._lastSubscriptionCheck)
			{
				this._isSubscribed = SubscriptionManager.IsPlayerSubscribed(this.rig);
				this._lastSubscriptionCheck = Time.frameCount;
			}
			return this._isSubscribed;
		}
	}

	// Token: 0x06002CC3 RID: 11459 RVA: 0x000F19A0 File Offset: 0x000EFBA0
	private void Awake()
	{
		this.handTransforms[0] = this.leftHand;
		this.handTransforms[1] = this.rightHand;
		for (int i = 0; i < this.slots.Length; i++)
		{
			this.slots[i].entityId = GameEntityId.Invalid;
		}
		this.newJoinZoneLimiter = new CallLimiter(10, 10f, 0.5f);
		this.netImpulseLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netGrabLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netThrowLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netStateLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netSnapLimiter = new CallLimiter(25, 1f, 0.5f);
		if (this.snapPointManager == null)
		{
			this.snapPointManager = base.GetComponentInChildren<SuperInfectionSnapPointManager>(true);
			if (this.snapPointManager == null)
			{
				Debug.LogError("[GamePlayer]  ERROR!!!  Snappoints cannot function because the required `SuperInfectionSnapPointManager` could found in children.", this);
			}
		}
	}

	// Token: 0x06002CC4 RID: 11460 RVA: 0x000F1AB0 File Offset: 0x000EFCB0
	public void Clear()
	{
		for (int i = 0; i <= 1; i++)
		{
			if (this.slots[i].entityId != GameEntityId.Invalid && this.slots[i].entityManager != null)
			{
				this.slots[i].entityManager.RequestThrowEntity(this.slots[i].entityId, GamePlayer.IsLeftHand(i), GTPlayer.Instance.HeadCenterPosition, Vector3.zero, Vector3.zero);
			}
			this.ClearGrabbed(i);
		}
		for (int j = 2; j <= 3; j++)
		{
			if (this.slots[j].entityId != GameEntityId.Invalid && this.slots[j].entityManager != null)
			{
				bool flag = j != 2;
				GameEntityId entityId = this.slots[j].entityId;
				GameEntityManager entityManager = this.slots[j].entityManager;
				entityManager.RequestGrabEntity(entityId, flag, Vector3.zero, Quaternion.identity);
				entityManager.RequestThrowEntity(entityId, flag, GTPlayer.Instance.HeadCenterPosition, Vector3.zero, Vector3.zero);
			}
			this.ClearSlot(j);
		}
	}

	// Token: 0x06002CC5 RID: 11461 RVA: 0x000F1BF4 File Offset: 0x000EFDF4
	public void ResetData()
	{
		for (int i = 0; i < 4; i++)
		{
			this.ClearSlot(i);
		}
		this.DidJoinWithItems = false;
		this.AdditionalDataInitialized = false;
		this.SetInitializePlayer(false);
	}

	// Token: 0x06002CC6 RID: 11462 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnEnable()
	{
	}

	// Token: 0x06002CC7 RID: 11463 RVA: 0x000F1C29 File Offset: 0x000EFE29
	private void Start()
	{
		GamePlayer.InitializeStaticLookupCaches();
	}

	// Token: 0x06002CC8 RID: 11464 RVA: 0x000F1C30 File Offset: 0x000EFE30
	public void MigrateHeldActorNumbers()
	{
		int actorNumber = this.rig.OwningNetPlayer.ActorNumber;
		for (int i = 0; i < 4; i++)
		{
			if (this.slots[i].entityManager != null)
			{
				GameEntity gameEntity = this.slots[i].entityManager.GetGameEntity(this.slots[i].entityId);
				if (gameEntity != null)
				{
					if (i <= 1)
					{
						gameEntity.MigrateHeldBy(actorNumber);
					}
					else
					{
						gameEntity.MigrateSnappedBy(actorNumber);
					}
				}
			}
		}
	}

	// Token: 0x06002CC9 RID: 11465 RVA: 0x000F1CB8 File Offset: 0x000EFEB8
	public void SetGrabbed(GameEntityId gameBallId, int handIndex, GameEntityManager gameEntityManager)
	{
		if (handIndex < 0 || handIndex > 1)
		{
			return;
		}
		this.SetSlot(handIndex, gameBallId, gameEntityManager);
	}

	// Token: 0x06002CCA RID: 11466 RVA: 0x000F1CCC File Offset: 0x000EFECC
	public void SetSnapped(GameEntityId entityId, int slotIndex, GameEntityManager gameEntityManager)
	{
		if (entityId.IsValid())
		{
			this.ClearSnappedIfSnapped(entityId, gameEntityManager);
			this.ClearGrabbedIfHeld(entityId, gameEntityManager);
		}
		this.SetSlot(slotIndex, entityId, gameEntityManager);
	}

	// Token: 0x06002CCB RID: 11467 RVA: 0x000F1CF0 File Offset: 0x000EFEF0
	public void SetSlot(int slotIndex, GameEntityId entityId, GameEntityManager manager)
	{
		if (slotIndex < 0 || slotIndex >= 4)
		{
			return;
		}
		if (entityId.IsValid())
		{
			manager.GetGameEntity(entityId);
		}
		GamePlayer.SlotData slotData = this.slots[slotIndex];
		slotData.entityId = entityId;
		slotData.entityManager = manager;
		this.slots[slotIndex] = slotData;
	}

	// Token: 0x06002CCC RID: 11468 RVA: 0x000F1D44 File Offset: 0x000EFF44
	public void ClearZone(GameEntityManager manager)
	{
		for (int i = 0; i < 4; i++)
		{
			if (this.slots[i].entityId != GameEntityId.Invalid && this.slots[i].entityManager == manager)
			{
				GameEntity gameEntity = this.slots[i].entityManager.GetGameEntity(this.slots[i].entityId);
				if (gameEntity != null)
				{
					Action onReleased = gameEntity.OnReleased;
					if (onReleased != null)
					{
						onReleased();
					}
				}
				this.ClearSlot(i);
			}
		}
		if (NetworkSystem.Instance.SessionIsPrivate)
		{
			this.DidJoinWithItems = false;
		}
	}

	// Token: 0x06002CCD RID: 11469 RVA: 0x000F1DF0 File Offset: 0x000EFFF0
	public void ClearGrabbedIfHeld(GameEntityId gameBallId, GameEntityManager manager)
	{
		for (int i = 0; i <= 1; i++)
		{
			if (this.slots[i].entityId == gameBallId && this.slots[i].entityManager == manager)
			{
				this.ClearGrabbed(i);
			}
		}
	}

	// Token: 0x06002CCE RID: 11470 RVA: 0x000F1E44 File Offset: 0x000F0044
	public void ClearSnappedIfSnapped(GameEntityId gameBallId, GameEntityManager manager)
	{
		for (int i = 2; i <= 3; i++)
		{
			if (this.slots[i].entityId == gameBallId && this.slots[i].entityManager == manager)
			{
				this.ClearSlot(i);
			}
		}
	}

	// Token: 0x06002CCF RID: 11471 RVA: 0x000F1E96 File Offset: 0x000F0096
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameEntityId.Invalid, handIndex, null);
	}

	// Token: 0x06002CD0 RID: 11472 RVA: 0x000F1EA5 File Offset: 0x000F00A5
	public void ClearSlot(int slotIndex)
	{
		this.SetSlot(slotIndex, GameEntityId.Invalid, null);
	}

	// Token: 0x06002CD1 RID: 11473 RVA: 0x000F1EB4 File Offset: 0x000F00B4
	public bool IsGrabbingDisabled()
	{
		return this.grabbingDisabled;
	}

	// Token: 0x06002CD2 RID: 11474 RVA: 0x000F1EBC File Offset: 0x000F00BC
	public void DisableGrabbing(bool disable)
	{
		this.grabbingDisabled = disable;
	}

	// Token: 0x06002CD3 RID: 11475 RVA: 0x000F1EC5 File Offset: 0x000F00C5
	internal bool IsSlotOccupied(int slotIndex)
	{
		return this.slots[slotIndex].entityId.index != -1;
	}

	// Token: 0x06002CD4 RID: 11476 RVA: 0x000F1EE3 File Offset: 0x000F00E3
	public bool IsHoldingEntity(GameEntityId gameEntityId, bool isLeftHand)
	{
		return this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(isLeftHand)) == gameEntityId;
	}

	// Token: 0x06002CD5 RID: 11477 RVA: 0x000F1EF7 File Offset: 0x000F00F7
	public bool IsHoldingEntity(GameEntityManager gameEntityManager, bool isLeftHand)
	{
		return gameEntityManager.GetGameEntity(this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(isLeftHand))) != null;
	}

	// Token: 0x06002CD6 RID: 11478 RVA: 0x000F1F11 File Offset: 0x000F0111
	public bool IsHoldingEntity(GameEntityId gameEntityId)
	{
		return this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(true)) == gameEntityId || this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(false)) == gameEntityId;
	}

	// Token: 0x06002CD7 RID: 11479 RVA: 0x000F1F3B File Offset: 0x000F013B
	public void RequestDropAllSnapped()
	{
		this.Clear();
		this.snapPointManager.DropAllSnappedAuthority();
	}

	// Token: 0x06002CD8 RID: 11480 RVA: 0x000F1F4E File Offset: 0x000F014E
	public List<GameEntityId> HeldAndSnappedItems(GameEntityManager manager)
	{
		return this.IterateHeldAndSnappedItems(manager).ToList<GameEntityId>();
	}

	// Token: 0x06002CD9 RID: 11481 RVA: 0x000F1F5C File Offset: 0x000F015C
	public IEnumerable<GameEntityId> IterateHeldAndSnappedItems(GameEntityManager manager)
	{
		int num;
		for (int i = 0; i < 4; i = num)
		{
			if (this.slots[i].entityId != GameEntityId.Invalid && this.slots[i].entityManager == manager)
			{
				yield return this.slots[i].entityId;
			}
			num = i + 1;
		}
		yield break;
	}

	// Token: 0x06002CDA RID: 11482 RVA: 0x000F1F73 File Offset: 0x000F0173
	public List<GameEntity> HeldAndSnappedEntities(GameEntityManager ignoreEntitiesInManager = null)
	{
		return this.IterateHeldAndSnappedEntities(ignoreEntitiesInManager).ToList<GameEntity>();
	}

	// Token: 0x06002CDB RID: 11483 RVA: 0x000F1F81 File Offset: 0x000F0181
	public IEnumerable<GameEntity> IterateHeldAndSnappedEntities(GameEntityManager ignoreEntitiesInManager = null)
	{
		int num;
		for (int i = 0; i < 4; i = num)
		{
			if (this.slots[i].entityId != GameEntityId.Invalid && this.slots[i].entityManager != null)
			{
				if (this.slots[i].entityManager != ignoreEntitiesInManager)
				{
					GameEntity gameEntity = this.slots[i].entityManager.GetGameEntity(this.slots[i].entityId);
					yield return gameEntity;
				}
				else
				{
					this.slots[i].entityManager.GetGameEntity(this.slots[i].entityId);
				}
			}
			num = i + 1;
		}
		yield break;
	}

	// Token: 0x06002CDC RID: 11484 RVA: 0x000F1F98 File Offset: 0x000F0198
	public void DeleteGrabbedEntityLocal(int handIndex)
	{
		if (this.slots[handIndex].entityId != GameEntityId.Invalid && this.slots[handIndex].entityManager != null)
		{
			GameEntity gameEntity = this.slots[handIndex].entityManager.GetGameEntity(this.slots[handIndex].entityId);
			if (gameEntity != null)
			{
				if (gameEntity != null)
				{
					Action onReleased = gameEntity.OnReleased;
					if (onReleased != null)
					{
						onReleased();
					}
				}
				this.slots[handIndex].entityManager.DestroyItemLocal(this.slots[handIndex].entityId);
			}
		}
	}

	// Token: 0x06002CDD RID: 11485 RVA: 0x000F204C File Offset: 0x000F024C
	public int AuthorityMigrateToEntityManager(GameEntityManager newEntityManager)
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			GameEntityId entityId = this.slots[i].entityId;
			if (entityId != GameEntityId.Invalid && this.slots[i].entityManager != newEntityManager)
			{
				GameEntity gameEntity = this.slots[i].entityManager.GetGameEntity(entityId);
				if (gameEntity != null)
				{
					if (gameEntity.IsScenePlaced)
					{
						GameEntityManager entityManager = this.slots[i].entityManager;
						if (entityManager != null)
						{
							entityManager.ReleaseScenePlacedHold(gameEntity);
						}
						this.ClearSlot(i);
					}
					else
					{
						GameEntityId gameEntityId = gameEntity.MigrateToEntityManager(newEntityManager);
						GamePlayer.SlotData slotData = this.slots[i];
						slotData.entityManager = newEntityManager;
						slotData.entityId = gameEntityId;
						this.slots[i] = slotData;
						num++;
					}
				}
			}
		}
		return num;
	}

	// Token: 0x06002CDE RID: 11486 RVA: 0x000F2135 File Offset: 0x000F0335
	internal bool IsInSlot(int slotIndex, int entityIndex, GameEntityManager manager)
	{
		return this.slots[slotIndex].entityId.index == entityIndex && this.slots[slotIndex].entityManager == manager;
	}

	// Token: 0x06002CDF RID: 11487 RVA: 0x000F2169 File Offset: 0x000F0369
	internal bool TryGetSlotData(int slotIndex, out GamePlayer.SlotData out_slotData)
	{
		out_slotData = this.slots[slotIndex];
		return out_slotData.entityId.index != -1;
	}

	// Token: 0x06002CE0 RID: 11488 RVA: 0x000F2190 File Offset: 0x000F0390
	internal bool TryGetSlotEntity(int slotIndex, out GameEntity out_entity)
	{
		GamePlayer.SlotData slotData;
		if (!this.TryGetSlotData(slotIndex, out slotData))
		{
			out_entity = null;
			return false;
		}
		out_entity = slotData.entityManager.GetGameEntity(slotData.entityId);
		return out_entity != null;
	}

	// Token: 0x06002CE1 RID: 11489 RVA: 0x000F21C8 File Offset: 0x000F03C8
	public GameEntityId GetGameEntityId(bool isLeftHand)
	{
		return this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(isLeftHand));
	}

	// Token: 0x06002CE2 RID: 11490 RVA: 0x000F21D6 File Offset: 0x000F03D6
	public GameEntityId GetGrabbedGameEntityId(int handIndex)
	{
		if (handIndex < 0 || handIndex > 1)
		{
			return GameEntityId.Invalid;
		}
		return this.slots[handIndex].entityId;
	}

	// Token: 0x06002CE3 RID: 11491 RVA: 0x000F21F7 File Offset: 0x000F03F7
	public GameEntityId GetGrabbedGameEntityIdAndManager(int handIndex, out GameEntityManager manager)
	{
		if (handIndex < 0 || handIndex > 1)
		{
			manager = null;
			return GameEntityId.Invalid;
		}
		manager = this.slots[handIndex].entityManager;
		return this.slots[handIndex].entityId;
	}

	// Token: 0x06002CE4 RID: 11492 RVA: 0x000F2230 File Offset: 0x000F0430
	public GameEntity GetGrabbedGameEntity(int handIndex)
	{
		if (handIndex < 0 || handIndex > 1 || this.slots[handIndex].entityManager == null)
		{
			return null;
		}
		return this.slots[handIndex].entityManager.GetGameEntity(this.GetGrabbedGameEntityId(handIndex));
	}

	// Token: 0x06002CE5 RID: 11493 RVA: 0x000F2280 File Offset: 0x000F0480
	public int FindSlotIndex(GameEntityId entityId)
	{
		int num = -1;
		int num2 = 0;
		while (num2 < 4 && num == -1)
		{
			num = ((this.slots[num2].entityId == entityId) ? num2 : (-1));
			num2++;
		}
		return num;
	}

	// Token: 0x06002CE6 RID: 11494 RVA: 0x000F22C0 File Offset: 0x000F04C0
	public int FindHandIndex(GameEntityId entityId)
	{
		for (int i = 0; i <= 1; i++)
		{
			if (this.slots[i].entityId == entityId)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002CE7 RID: 11495 RVA: 0x000F22F8 File Offset: 0x000F04F8
	public int FindSnapIndex(GameEntityId entityId)
	{
		for (int i = 2; i <= 3; i++)
		{
			if (this.slots[i].entityId == entityId)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002CE8 RID: 11496 RVA: 0x000CAF4B File Offset: 0x000C914B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x06002CE9 RID: 11497 RVA: 0x000CAF51 File Offset: 0x000C9151
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06002CEA RID: 11498 RVA: 0x000F2330 File Offset: 0x000F0530
	[Obsolete("Method `GamePlayer.TryGetGamePlayer(Player)` is obsolete, use `TryGetGamePlayer(Player, out GamePlayer)` instead.")]
	public static VRRig GetRig(int actorNumber)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
		if (player == null)
		{
			return null;
		}
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom != null && currentRoom.GetPlayer(actorNumber, false) == null)
		{
			return null;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return null;
		}
		return rigContainer.Rig;
	}

	// Token: 0x06002CEB RID: 11499 RVA: 0x000F237C File Offset: 0x000F057C
	public static GamePlayer GetGamePlayer(Player player)
	{
		GamePlayer gamePlayer;
		GamePlayer.TryGetGamePlayer(player, out gamePlayer);
		return gamePlayer;
	}

	// Token: 0x06002CEC RID: 11500 RVA: 0x000F2393 File Offset: 0x000F0593
	public static bool TryGetGamePlayer(Player player, out GamePlayer gamePlayer)
	{
		if (player == null)
		{
			gamePlayer = null;
			return false;
		}
		return GamePlayer.TryGetGamePlayer(player.ActorNumber, out gamePlayer);
	}

	// Token: 0x06002CED RID: 11501 RVA: 0x000F23AC File Offset: 0x000F05AC
	[Obsolete("Method `GamePlayer.GetGamePlayer(actorNum)` is obsolete, use `TryGetGamePlayer(actorNum, out GamePlayer)` instead.")]
	public static GamePlayer GetGamePlayer(int actorNumber)
	{
		GamePlayer gamePlayer;
		GamePlayer.TryGetGamePlayer(actorNumber, out gamePlayer);
		return gamePlayer;
	}

	// Token: 0x06002CEE RID: 11502 RVA: 0x000F23C4 File Offset: 0x000F05C4
	public static bool TryGetGamePlayer(int actorNumber, out GamePlayer out_gamePlayer)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
		RigContainer rigContainer;
		if (player == null || VRRigCache.Instance == null || !VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			out_gamePlayer = null;
			return false;
		}
		return GamePlayer.TryGetGamePlayer(rigContainer.Rig, out out_gamePlayer);
	}

	// Token: 0x06002CEF RID: 11503 RVA: 0x000F2410 File Offset: 0x000F0610
	public static bool TryGetGamePlayer(VRRig rig, out GamePlayer out_gamePlayer)
	{
		if (rig == null)
		{
			out_gamePlayer = null;
			return false;
		}
		out_gamePlayer = rig.GamePlayerRef;
		return out_gamePlayer != null;
	}

	// Token: 0x06002CF0 RID: 11504 RVA: 0x000F243C File Offset: 0x000F063C
	public static GamePlayer GetGamePlayer(Collider collider, bool bodyOnly = false)
	{
		Transform transform = collider.transform;
		while (transform != null)
		{
			GamePlayer component = transform.GetComponent<GamePlayer>();
			if (component != null)
			{
				return component;
			}
			if (bodyOnly)
			{
				break;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x06002CF1 RID: 11505 RVA: 0x000F2478 File Offset: 0x000F0678
	public Transform GetHandTransform(int handIndex)
	{
		if (handIndex < 0 || handIndex > 1)
		{
			return null;
		}
		return this.handTransforms[handIndex];
	}

	// Token: 0x06002CF2 RID: 11506 RVA: 0x000F248C File Offset: 0x000F068C
	public bool TryGetSlotXform(int slotIndex, out Transform slotXform)
	{
		if (GamePlayer.IsGrabSlot(slotIndex))
		{
			slotXform = this.handTransforms[slotIndex];
		}
		else if (GamePlayer.IsSnapSlot(slotIndex))
		{
			SnapJointType snapIndexToJoint = GameSnappable.GetSnapIndexToJoint(slotIndex);
			SuperInfectionSnapPoint superInfectionSnapPoint = ((this.snapPointManager != null) ? this.snapPointManager.FindSnapPoint(snapIndexToJoint) : null);
			slotXform = ((superInfectionSnapPoint != null) ? superInfectionSnapPoint.transform : null);
		}
		else
		{
			slotXform = null;
		}
		return slotXform != null;
	}

	// Token: 0x06002CF3 RID: 11507 RVA: 0x000F24FB File Offset: 0x000F06FB
	public bool IsLocal()
	{
		return GamePlayerLocal.instance != null && GamePlayerLocal.instance.gamePlayer == this;
	}

	// Token: 0x06002CF4 RID: 11508 RVA: 0x000F2520 File Offset: 0x000F0720
	public void SerializeNetworkState(BinaryWriter writer, NetPlayer player, GameEntityManager manager)
	{
		string text = "";
		for (int i = 0; i < 4; i++)
		{
			if (this.slots[i].entityManager == manager)
			{
				int netIdFromEntityId = manager.GetNetIdFromEntityId(this.slots[i].entityId);
				writer.Write(netIdFromEntityId);
				long num = 0L;
				if (netIdFromEntityId != -1)
				{
					GameEntity gameEntity = manager.GetGameEntity(this.slots[i].entityId);
					if (gameEntity != null)
					{
						text += string.Format(" [{0}: {1}/{2}]", i, gameEntity.gameObject.name, netIdFromEntityId);
						num = BitPackUtils.PackHandPosRotForNetwork(gameEntity.transform.localPosition, gameEntity.transform.localRotation);
					}
				}
				writer.Write(num);
			}
			else
			{
				writer.Write(-1);
				writer.Write(0L);
			}
		}
		writer.Write(this.AdditionalDataInitialized);
	}

	// Token: 0x06002CF5 RID: 11509 RVA: 0x000F2618 File Offset: 0x000F0818
	public static void DeserializeNetworkState(BinaryReader reader, GamePlayer gamePlayer, GameEntityManager manager)
	{
		for (int i = 0; i < 4; i++)
		{
			int num = reader.ReadInt32();
			long num2 = reader.ReadInt64();
			if (num != -1)
			{
				GameEntityId entityIdFromNetId = manager.GetEntityIdFromNetId(num);
				if (entityIdFromNetId.IsValid())
				{
					GameEntity gameEntity = manager.GetGameEntity(entityIdFromNetId);
					if (num2 != 0L && !(gameEntity == null))
					{
						Vector3 vector;
						Quaternion quaternion;
						BitPackUtils.UnpackHandPosRotFromNetwork(num2, out vector, out quaternion);
						if (gamePlayer != null && gamePlayer.rig.OwningNetPlayer != null)
						{
							if (GamePlayer.IsGrabSlot(i))
							{
								manager.GrabEntityOnCreate(entityIdFromNetId, GamePlayer.IsLeftHand(i), vector, quaternion, gamePlayer.rig.OwningNetPlayer);
							}
							else
							{
								int num3 = -1;
								if (i == 2)
								{
									num3 = 1;
								}
								else if (i == 3)
								{
									num3 = 4;
								}
								manager.SnapEntityOnCreate(entityIdFromNetId, i == 2, vector, quaternion, num3, gamePlayer.rig.OwningNetPlayer);
							}
						}
					}
				}
			}
		}
		bool flag = reader.ReadBoolean();
		if (gamePlayer != null)
		{
			gamePlayer.SetInitializePlayer(flag);
		}
	}

	// Token: 0x06002CF6 RID: 11510 RVA: 0x000F2709 File Offset: 0x000F0909
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsSlot(int i)
	{
		return i >= 0 && i < 4;
	}

	// Token: 0x06002CF7 RID: 11511 RVA: 0x000F2715 File Offset: 0x000F0915
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsGrabSlot(int i)
	{
		return i >= 0 && i <= 1;
	}

	// Token: 0x06002CF8 RID: 11512 RVA: 0x000F2724 File Offset: 0x000F0924
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsSnapSlot(int i)
	{
		return i >= 2 && i <= 3;
	}

	// Token: 0x06002CF9 RID: 11513 RVA: 0x000F2733 File Offset: 0x000F0933
	internal static void InitializeStaticLookupCaches()
	{
		GamePlayer.lookupCache_actorNum_to_gamePlayer = new ValueTuple<int, GamePlayer>[20];
		GamePlayer.lookupCache_rigInstanceId_to_gamePlayer = new ValueTuple<int, GamePlayer>[20];
		if (VRRigCache.isInitialized)
		{
			GamePlayer.UpdateStaticLookupCaches();
		}
	}

	// Token: 0x06002CFA RID: 11514 RVA: 0x000F275C File Offset: 0x000F095C
	internal static void UpdateStaticLookupCaches()
	{
		if (GamePlayer.lookupCache_actorNum_to_gamePlayer == null)
		{
			return;
		}
		List<VRRig> list;
		using (ListPool<VRRig>.Get(out list))
		{
			if (list.Capacity < 20)
			{
				list.Capacity = 20;
			}
			VRRigCache.Instance.GetActiveRigs(list);
			if (list.Count > GamePlayer.lookupCache_actorNum_to_gamePlayer.Length)
			{
				int num = list.Count * 2;
				Array.Resize<ValueTuple<int, GamePlayer>>(ref GamePlayer.lookupCache_actorNum_to_gamePlayer, num);
				Array.Resize<ValueTuple<int, GamePlayer>>(ref GamePlayer.lookupCache_rigInstanceId_to_gamePlayer, num);
			}
			GamePlayer.staticLookupCachesCount = list.Count;
			if (GamePlayer.staticLookupCachesCount >= 1)
			{
				VRRig vrrig = list[0];
				if (vrrig == null)
				{
					throw new NullReferenceException("[GT/GamePlayer::_VRRigCache_OnActiveRigsChanged]  ERROR!!!  (should never happen) The VRRig at index 0 is expected to be the local rig but is null.");
				}
				int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
				GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
				GamePlayer.lookupCache_actorNum_to_gamePlayer[0] = new ValueTuple<int, GamePlayer>(actorNumber, gamePlayer);
				GamePlayer.lookupCache_rigInstanceId_to_gamePlayer[0] = new ValueTuple<int, GamePlayer>(vrrig.GetInstanceID(), gamePlayer);
			}
			for (int i = 1; i < GamePlayer.staticLookupCachesCount; i++)
			{
				VRRig vrrig2 = list[i];
				if (vrrig2 == null)
				{
					throw new NullReferenceException("[GT/GamePlayer::_VRRigCache_OnActiveRigsChanged]  ERROR!!!  (should never happen) An entry from `VRRigCache.Instance.GetActiveRigs(activeRigs)` is null but is expected to be ready and all entries not null at this stage.");
				}
				GamePlayer component = vrrig2.GetComponent<GamePlayer>();
				if (component == null)
				{
					throw new NullReferenceException("[GT/GamePlayer::_VRRigCache_OnActiveRigsChanged]  ERROR!!!  (should never happen) Could not get GamePlayer from rig which is expected to be ready at this stage.");
				}
				NetPlayer owningNetPlayer = vrrig2.OwningNetPlayer;
				int num2 = ((owningNetPlayer != null) ? owningNetPlayer.ActorNumber : int.MinValue);
				GamePlayer.lookupCache_actorNum_to_gamePlayer[i] = new ValueTuple<int, GamePlayer>(num2, component);
				GamePlayer.lookupCache_rigInstanceId_to_gamePlayer[i] = new ValueTuple<int, GamePlayer>(vrrig2.GetInstanceID(), component);
			}
			for (int j = GamePlayer.staticLookupCachesCount; j < GamePlayer.lookupCache_actorNum_to_gamePlayer.Length; j++)
			{
				GamePlayer.lookupCache_actorNum_to_gamePlayer[j] = new ValueTuple<int, GamePlayer>(0, null);
				GamePlayer.lookupCache_rigInstanceId_to_gamePlayer[j] = new ValueTuple<int, GamePlayer>(0, null);
			}
		}
	}

	// Token: 0x06002CFB RID: 11515 RVA: 0x000F2944 File Offset: 0x000F0B44
	public void SetInitializePlayer(bool initialized)
	{
		bool additionalDataInitialized = this.AdditionalDataInitialized;
		this.AdditionalDataInitialized = initialized;
		if (!additionalDataInitialized && this.AdditionalDataInitialized)
		{
			Action onPlayerInitialized = this.OnPlayerInitialized;
			if (onPlayerInitialized == null)
			{
				return;
			}
			onPlayerInitialized();
		}
	}

	// Token: 0x04003943 RID: 14659
	private const string preLog = "[GamePlayer]  ";

	// Token: 0x04003944 RID: 14660
	private const string preErr = "[GamePlayer]  ERROR!!!  ";

	// Token: 0x04003945 RID: 14661
	public VRRig rig;

	// Token: 0x04003946 RID: 14662
	public Transform leftHand;

	// Token: 0x04003947 RID: 14663
	public Transform rightHand;

	// Token: 0x04003948 RID: 14664
	public SuperInfectionSnapPointManager snapPointManager;

	// Token: 0x04003949 RID: 14665
	private readonly Transform[] handTransforms = new Transform[2];

	// Token: 0x0400394A RID: 14666
	private readonly GamePlayer.SlotData[] slots = new GamePlayer.SlotData[4];

	// Token: 0x0400394B RID: 14667
	public const int MAX_HANDS = 2;

	// Token: 0x0400394C RID: 14668
	public const int LEFT_HAND = 0;

	// Token: 0x0400394D RID: 14669
	public const int RIGHT_HAND = 1;

	// Token: 0x0400394E RID: 14670
	public const int GRAB_SLOT_FIRST = 0;

	// Token: 0x0400394F RID: 14671
	public const int GRAB_SLOT_LAST = 1;

	// Token: 0x04003950 RID: 14672
	public const int SNAP_SLOTS_COUNT = 2;

	// Token: 0x04003951 RID: 14673
	public const int SNAP_SLOTS_FIRST = 2;

	// Token: 0x04003952 RID: 14674
	public const int SNAP_SLOTS_LAST = 3;

	// Token: 0x04003953 RID: 14675
	public const int SNAP_SLOT_HAND_L = 2;

	// Token: 0x04003954 RID: 14676
	public const int SNAP_SLOT_HAND_R = 3;

	// Token: 0x04003955 RID: 14677
	public const int SLOTS_COUNT = 4;

	// Token: 0x04003956 RID: 14678
	public CallLimiter newJoinZoneLimiter;

	// Token: 0x04003957 RID: 14679
	public CallLimiter netImpulseLimiter;

	// Token: 0x04003958 RID: 14680
	public CallLimiter netGrabLimiter;

	// Token: 0x04003959 RID: 14681
	public CallLimiter netThrowLimiter;

	// Token: 0x0400395A RID: 14682
	public CallLimiter netStateLimiter;

	// Token: 0x0400395B RID: 14683
	public CallLimiter netSnapLimiter;

	// Token: 0x0400395E RID: 14686
	private int _lastSubscriptionCheck;

	// Token: 0x0400395F RID: 14687
	private bool _isSubscribed;

	// Token: 0x04003960 RID: 14688
	public Action OnPlayerInitialized;

	// Token: 0x04003961 RID: 14689
	public Action OnPlayerLeftZone;

	// Token: 0x04003962 RID: 14690
	private bool grabbingDisabled;

	// Token: 0x04003963 RID: 14691
	private const bool _k_MATTO__USE_STATIC_CACHE = false;

	// Token: 0x04003964 RID: 14692
	[OnEnterPlay_SetNull]
	private static ValueTuple<int, GamePlayer>[] lookupCache_actorNum_to_gamePlayer;

	// Token: 0x04003965 RID: 14693
	[OnEnterPlay_SetNull]
	private static ValueTuple<int, GamePlayer>[] lookupCache_rigInstanceId_to_gamePlayer;

	// Token: 0x04003966 RID: 14694
	[OnEnterPlay_Set(0)]
	private static int staticLookupCachesCount;

	// Token: 0x04003967 RID: 14695
	public const int INVALID_ACTOR_NUMBER = -2147483648;

	// Token: 0x020006F3 RID: 1779
	public struct SlotData
	{
		// Token: 0x04003968 RID: 14696
		public GameEntityId entityId;

		// Token: 0x04003969 RID: 14697
		public GameEntityManager entityManager;
	}
}
