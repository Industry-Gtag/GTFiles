using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag;
using GorillaTag.Gravity;
using Photon.Pun;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020006C1 RID: 1729
public class GameEntity : MonoBehaviour
{
	// Token: 0x17000448 RID: 1096
	// (get) Token: 0x06002B1E RID: 11038 RVA: 0x000E73BF File Offset: 0x000E55BF
	// (set) Token: 0x06002B1F RID: 11039 RVA: 0x000E73C7 File Offset: 0x000E55C7
	[DebugReadout]
	public GameEntityId id { get; internal set; }

	// Token: 0x17000449 RID: 1097
	// (get) Token: 0x06002B20 RID: 11040 RVA: 0x000E73D0 File Offset: 0x000E55D0
	// (set) Token: 0x06002B21 RID: 11041 RVA: 0x000E73D8 File Offset: 0x000E55D8
	[DebugReadout]
	public int typeId { get; internal set; }

	// Token: 0x1700044A RID: 1098
	// (get) Token: 0x06002B22 RID: 11042 RVA: 0x000E73E1 File Offset: 0x000E55E1
	// (set) Token: 0x06002B23 RID: 11043 RVA: 0x000E73E9 File Offset: 0x000E55E9
	[DebugReadout]
	public long createData { get; set; }

	// Token: 0x1700044B RID: 1099
	// (get) Token: 0x06002B24 RID: 11044 RVA: 0x000E73F2 File Offset: 0x000E55F2
	// (set) Token: 0x06002B25 RID: 11045 RVA: 0x000E73FA File Offset: 0x000E55FA
	[DebugReadout]
	public GameEntityId createdByEntityId { get; set; }

	// Token: 0x1700044C RID: 1100
	// (get) Token: 0x06002B26 RID: 11046 RVA: 0x000E7403 File Offset: 0x000E5603
	// (set) Token: 0x06002B27 RID: 11047 RVA: 0x000E740B File Offset: 0x000E560B
	[DebugReadout]
	public int heldByActorNumber { get; internal set; }

	// Token: 0x1700044D RID: 1101
	// (get) Token: 0x06002B28 RID: 11048 RVA: 0x000E7414 File Offset: 0x000E5614
	// (set) Token: 0x06002B29 RID: 11049 RVA: 0x000E741C File Offset: 0x000E561C
	[DebugReadout]
	public int snappedByActorNumber { get; internal set; }

	// Token: 0x1700044E RID: 1102
	// (get) Token: 0x06002B2A RID: 11050 RVA: 0x000E7425 File Offset: 0x000E5625
	[DebugReadout]
	public int slotIndex
	{
		get
		{
			if (this.heldByHandIndex == -1)
			{
				return GameSnappable.GetJointToSnapIndex(this.snappedJoint);
			}
			return this.heldByHandIndex;
		}
	}

	// Token: 0x1700044F RID: 1103
	// (get) Token: 0x06002B2B RID: 11051 RVA: 0x000E7442 File Offset: 0x000E5642
	// (set) Token: 0x06002B2C RID: 11052 RVA: 0x000E744A File Offset: 0x000E564A
	[DebugReadout]
	public SnapJointType snappedJoint { get; internal set; }

	// Token: 0x17000450 RID: 1104
	// (get) Token: 0x06002B2D RID: 11053 RVA: 0x000E7453 File Offset: 0x000E5653
	// (set) Token: 0x06002B2E RID: 11054 RVA: 0x000E745B File Offset: 0x000E565B
	[DebugReadout]
	public int heldByHandIndex { get; internal set; }

	// Token: 0x17000451 RID: 1105
	// (get) Token: 0x06002B2F RID: 11055 RVA: 0x000E7464 File Offset: 0x000E5664
	// (set) Token: 0x06002B30 RID: 11056 RVA: 0x000E746C File Offset: 0x000E566C
	[DebugReadout]
	public int lastHeldByActorNumber { get; internal set; }

	// Token: 0x17000452 RID: 1106
	// (get) Token: 0x06002B31 RID: 11057 RVA: 0x000E7475 File Offset: 0x000E5675
	// (set) Token: 0x06002B32 RID: 11058 RVA: 0x000E747D File Offset: 0x000E567D
	[DebugReadout]
	public int onlyGrabActorNumber { get; internal set; }

	// Token: 0x17000453 RID: 1107
	// (get) Token: 0x06002B33 RID: 11059 RVA: 0x000E7486 File Offset: 0x000E5686
	// (set) Token: 0x06002B34 RID: 11060 RVA: 0x000E748E File Offset: 0x000E568E
	[DebugReadout]
	public GameEntityId attachedToEntityId { get; internal set; }

	// Token: 0x17000454 RID: 1108
	// (get) Token: 0x06002B35 RID: 11061 RVA: 0x000E7497 File Offset: 0x000E5697
	// (set) Token: 0x06002B36 RID: 11062 RVA: 0x000E749F File Offset: 0x000E569F
	public bool IsScenePlaced { get; internal set; }

	// Token: 0x14000055 RID: 85
	// (add) Token: 0x06002B37 RID: 11063 RVA: 0x000E74A8 File Offset: 0x000E56A8
	// (remove) Token: 0x06002B38 RID: 11064 RVA: 0x000E74E0 File Offset: 0x000E56E0
	public event GameEntity.StateChangedEvent OnStateChanged;

	// Token: 0x14000056 RID: 86
	// (add) Token: 0x06002B39 RID: 11065 RVA: 0x000E7518 File Offset: 0x000E5718
	// (remove) Token: 0x06002B3A RID: 11066 RVA: 0x000E7550 File Offset: 0x000E5750
	public event GameEntity.EntityDestroyedEvent onEntityDestroyed;

	// Token: 0x06002B3B RID: 11067 RVA: 0x000E7588 File Offset: 0x000E5788
	private void Awake()
	{
		this.id = GameEntityId.Invalid;
		this.rigidBody = base.GetComponent<Rigidbody>();
		if (this.gravityController == null)
		{
			if (base.TryGetComponent<MonkeGravityController>(out this.gravityController))
			{
				if (this.rigidBody != null)
				{
					this.gravityController.GlobalGravityIntent = this.rigidBody.useGravity;
					this.rigidBody.useGravity = false;
				}
			}
			else
			{
				this.gravityController = base.gameObject.AddComponent<MonkeGravityController>();
			}
		}
		this.heldByActorNumber = -1;
		this.heldByHandIndex = -1;
		this.onlyGrabActorNumber = -1;
		this.snappedByActorNumber = -1;
		this.attachedToEntityId = GameEntityId.Invalid;
		this.entityComponents = new List<IGameEntityComponent>(1);
		base.GetComponentsInChildren<IGameEntityComponent>(this.entityComponents);
		this.entitySerialize = new List<IGameEntitySerialize>(1);
		base.GetComponentsInChildren<IGameEntitySerialize>(this.entitySerialize);
		if (this.builtInEntities != null)
		{
			for (int i = 0; i < this.builtInEntities.Count; i++)
			{
				this.builtInEntities[i].isBuiltIn = true;
			}
		}
		XSceneRefTarget xsceneRefTarget;
		if (base.TryGetComponent<XSceneRefTarget>(out xsceneRefTarget) && xsceneRefTarget.UniqueID > 0)
		{
			this.IsScenePlaced = true;
			GameEntityManager.RegisterScenePlacedEntity(this);
		}
	}

	// Token: 0x06002B3C RID: 11068 RVA: 0x000E76B2 File Offset: 0x000E58B2
	private void Start()
	{
		if (this.IsScenePlaced && !PhotonNetwork.InRoom)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002B3D RID: 11069 RVA: 0x000E76D0 File Offset: 0x000E58D0
	public void Create(GameEntityManager manager, int netId, int typeId)
	{
		this.manager = manager;
		this.typeId = typeId;
		if (this.builtInEntities != null)
		{
			bool flag = netId < -1 && netId != int.MinValue;
			for (int i = 0; i < this.builtInEntities.Count; i++)
			{
				int num = (flag ? (netId - 1 - i) : (netId + 1 + i));
				manager.AddGameEntity(num, this.builtInEntities[i]);
				this.builtInEntities[i].Create(manager, num, -1);
			}
		}
	}

	// Token: 0x06002B3E RID: 11070 RVA: 0x000E7754 File Offset: 0x000E5954
	public void Init(long createData, int createdByEntityNetId)
	{
		this.createData = createData;
		this.createdByEntityId = this.manager.GetEntityIdFromNetId(createdByEntityNetId);
		for (int i = 0; i < this.entityComponents.Count; i++)
		{
			this.entityComponents[i].OnEntityInit();
		}
		for (int j = 0; j < this.builtInEntities.Count; j++)
		{
			this.builtInEntities[j].Init(0L, -1);
		}
	}

	// Token: 0x06002B3F RID: 11071 RVA: 0x000E77CC File Offset: 0x000E59CC
	public void OnDestroy()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		for (int i = 0; i < this.entityComponents.Count; i++)
		{
			this.entityComponents[i].OnEntityDestroy();
		}
		GameEntity.EntityDestroyedEvent entityDestroyedEvent = this.onEntityDestroyed;
		if (entityDestroyedEvent != null)
		{
			entityDestroyedEvent(this);
		}
		if (this.IsScenePlaced)
		{
			GameEntityManager.UnregisterScenePlacedEntity(this);
		}
	}

	// Token: 0x06002B40 RID: 11072 RVA: 0x000E7828 File Offset: 0x000E5A28
	public GameEntity.RendererSet GetGrabbableRenderers()
	{
		if (this._grabbableRenderers == null)
		{
			this._grabbableRenderers = new GameEntity.RendererSet();
			this._meshFilters = new List<MeshFilter>();
			base.GetComponentsInChildren<MeshFilter>(true, this._meshFilters);
			base.GetComponentsInChildren<SkinnedMeshRenderer>(true, this._grabbableRenderers.skinnedRenderers);
			List<SkinnedMeshRenderer> skinnedRenderers = this._grabbableRenderers.skinnedRenderers;
			this.<GetGrabbableRenderers>g__RemoveNotOwnedComponents|103_0<MeshFilter>(this._meshFilters);
			this.<GetGrabbableRenderers>g__RemoveNotOwnedComponents|103_0<SkinnedMeshRenderer>(skinnedRenderers);
			foreach (GameObject gameObject in this.ignoreObjectGrabRenderers)
			{
				for (int j = 0; j < this._meshFilters.Count; j++)
				{
					if (this._meshFilters[j].gameObject == gameObject)
					{
						this._meshFilters.RemoveAtSwapBack(j--);
					}
				}
				for (int k = 0; k < skinnedRenderers.Count; k++)
				{
					if (skinnedRenderers[k].gameObject == gameObject)
					{
						skinnedRenderers.RemoveAtSwapBack(k--);
					}
				}
			}
			foreach (MeshFilter meshFilter in this._meshFilters)
			{
				MeshRenderer component = meshFilter.GetComponent<MeshRenderer>();
				if (component != null)
				{
					this._grabbableRenderers.renderers.Add(new ValueTuple<MeshFilter, MeshRenderer>(meshFilter, component));
				}
			}
		}
		return this._grabbableRenderers;
	}

	// Token: 0x06002B41 RID: 11073 RVA: 0x000E79A0 File Offset: 0x000E5BA0
	public Vector3 GetVelocity()
	{
		if (this.rigidBody == null)
		{
			return Vector3.zero;
		}
		return this.rigidBody.linearVelocity;
	}

	// Token: 0x06002B42 RID: 11074 RVA: 0x000E79C4 File Offset: 0x000E5BC4
	public void PlayCatchFx()
	{
		if (this.audioSource != null && this.audioSource.isActiveAndEnabled)
		{
			this.audioSource.volume = this.catchSoundVolume;
			this.audioSource.GTPlayOneShot(this.catchSound, 1f);
		}
	}

	// Token: 0x06002B43 RID: 11075 RVA: 0x000E7A14 File Offset: 0x000E5C14
	public void PlayThrowFx()
	{
		if (this.audioSource != null && this.audioSource.isActiveAndEnabled)
		{
			this.audioSource.volume = this.throwSoundVolume;
			this.audioSource.GTPlayOneShot(this.throwSound, 1f);
		}
	}

	// Token: 0x06002B44 RID: 11076 RVA: 0x000E7A64 File Offset: 0x000E5C64
	public void PlaySnapFx()
	{
		if (this.audioSource != null && this.audioSource.isActiveAndEnabled)
		{
			this.audioSource.volume = this.snapSoundVolume;
			this.audioSource.GTPlayOneShot(this.snapSound, 1f);
		}
	}

	// Token: 0x06002B45 RID: 11077 RVA: 0x000E7AB3 File Offset: 0x000E5CB3
	private bool IsGamePlayer(Collider collider)
	{
		return GamePlayer.GetGamePlayer(collider, false) != null;
	}

	// Token: 0x06002B46 RID: 11078 RVA: 0x000E7AC2 File Offset: 0x000E5CC2
	public long GetState()
	{
		return this.state;
	}

	// Token: 0x06002B47 RID: 11079 RVA: 0x000E7ACA File Offset: 0x000E5CCA
	public void RequestState(long newState)
	{
		this.RequestState(this.id, newState);
	}

	// Token: 0x06002B48 RID: 11080 RVA: 0x000E7AD9 File Offset: 0x000E5CD9
	public void RequestState(GameEntityId id, long newState)
	{
		this.manager.RequestState(id, newState);
	}

	// Token: 0x06002B49 RID: 11081 RVA: 0x000E7AE8 File Offset: 0x000E5CE8
	public bool IsAuthority()
	{
		return this.manager.IsAuthority();
	}

	// Token: 0x06002B4A RID: 11082 RVA: 0x000E7AF5 File Offset: 0x000E5CF5
	public bool IsValidToMigrate()
	{
		return this.manager.IsEntityValidToMigrate(this);
	}

	// Token: 0x06002B4B RID: 11083 RVA: 0x000E7B04 File Offset: 0x000E5D04
	public void SetState(long newState)
	{
		if (this.state != newState)
		{
			long num = this.state;
			this.state = newState;
			GameEntity.StateChangedEvent onStateChanged = this.OnStateChanged;
			if (onStateChanged != null)
			{
				onStateChanged(num, newState);
			}
			for (int i = 0; i < this.entityComponents.Count; i++)
			{
				this.entityComponents[i].OnEntityStateChange(num, newState);
			}
		}
	}

	// Token: 0x06002B4C RID: 11084 RVA: 0x000E7B64 File Offset: 0x000E5D64
	public GameEntityId MigrateToEntityManager(GameEntityManager newManager)
	{
		if (this.IsScenePlaced)
		{
			if (this.manager != null)
			{
				this.manager.ReleaseScenePlacedHold(this);
			}
			return this.id;
		}
		this.manager.RemoveGameEntity(this);
		this.manager = newManager;
		GameEntityId gameEntityId = newManager.AddGameEntity(this);
		this.id = gameEntityId;
		this.manager.InitItemLocal(this, this.createData, -1);
		return gameEntityId;
	}

	// Token: 0x06002B4D RID: 11085 RVA: 0x000E7BD0 File Offset: 0x000E5DD0
	public void MigrateHeldBy(int actorNumber)
	{
		if (this.heldByActorNumber >= 0)
		{
			this.heldByActorNumber = actorNumber;
		}
	}

	// Token: 0x06002B4E RID: 11086 RVA: 0x000E7BE2 File Offset: 0x000E5DE2
	public void MigrateSnappedBy(int actorNumber)
	{
		if (this.snappedByActorNumber >= 0)
		{
			this.snappedByActorNumber = actorNumber;
		}
	}

	// Token: 0x06002B4F RID: 11087 RVA: 0x000E7BF4 File Offset: 0x000E5DF4
	public int GetNetId(GameEntityId gameEntityId)
	{
		return this.manager.GetNetIdFromEntityId(gameEntityId);
	}

	// Token: 0x06002B50 RID: 11088 RVA: 0x000E7C02 File Offset: 0x000E5E02
	public int GetNetId()
	{
		return this.manager.GetNetIdFromEntityId(this.id);
	}

	// Token: 0x06002B51 RID: 11089 RVA: 0x000E7C18 File Offset: 0x000E5E18
	public static GameEntity Get(Collider collider)
	{
		if (collider == null)
		{
			return null;
		}
		Transform transform = collider.transform;
		while (transform != null)
		{
			GameEntity component = transform.GetComponent<GameEntity>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x06002B52 RID: 11090 RVA: 0x000E7C5C File Offset: 0x000E5E5C
	public bool IsHeldByLocalPlayer()
	{
		return this.heldByActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002B53 RID: 11091 RVA: 0x000E7C75 File Offset: 0x000E5E75
	public bool IsSnappedByLocalPlayer()
	{
		return this.snappedByActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
	}

	// Token: 0x17000455 RID: 1109
	// (get) Token: 0x06002B54 RID: 11092 RVA: 0x000E7C8E File Offset: 0x000E5E8E
	public bool IsHeldOrSnappedByLocalPlayer
	{
		get
		{
			return this.AttachedPlayerActorNr == NetworkSystem.Instance.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x06002B55 RID: 11093 RVA: 0x000E7CA7 File Offset: 0x000E5EA7
	public bool IsHeld()
	{
		return this.heldByActorNumber != -1;
	}

	// Token: 0x17000456 RID: 1110
	// (get) Token: 0x06002B56 RID: 11094 RVA: 0x000E7CB5 File Offset: 0x000E5EB5
	public bool IsSnappedToHand
	{
		get
		{
			return (this.snappedJoint & (SnapJointType.HandL | SnapJointType.HandR)) > SnapJointType.None;
		}
	}

	// Token: 0x17000457 RID: 1111
	// (get) Token: 0x06002B57 RID: 11095 RVA: 0x000E7CC2 File Offset: 0x000E5EC2
	public int AttachedPlayerActorNr
	{
		get
		{
			if (this.heldByActorNumber == -1)
			{
				return this.snappedByActorNumber;
			}
			return this.heldByActorNumber;
		}
	}

	// Token: 0x06002B58 RID: 11096 RVA: 0x000E7CDC File Offset: 0x000E5EDC
	public int GetLastHeldByPlayerForEntityID(GameEntityId gameEntityId)
	{
		GameEntity gameEntity = this.manager.GetGameEntity(gameEntityId);
		if (gameEntity != null)
		{
			return gameEntity.lastHeldByActorNumber;
		}
		return 0;
	}

	// Token: 0x06002B59 RID: 11097 RVA: 0x000E7D07 File Offset: 0x000E5F07
	public bool WasLastHeldByLocalPlayer()
	{
		return this.lastHeldByActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002B5A RID: 11098 RVA: 0x000E7D20 File Offset: 0x000E5F20
	public bool IsAttachedToPlayer(NetPlayer player)
	{
		return player != null && (this.heldByActorNumber == player.ActorNumber || this.snappedByActorNumber == player.ActorNumber);
	}

	// Token: 0x17000458 RID: 1112
	// (get) Token: 0x06002B5B RID: 11099 RVA: 0x000E7D45 File Offset: 0x000E5F45
	public int EquippedSlotIndex
	{
		get
		{
			if (this.heldByHandIndex != -1)
			{
				return this.heldByHandIndex;
			}
			if ((this.snappedJoint & SnapJointType.HandL) != SnapJointType.None)
			{
				return 2;
			}
			if ((this.snappedJoint & SnapJointType.HandR) == SnapJointType.None)
			{
				return -1;
			}
			return 3;
		}
	}

	// Token: 0x17000459 RID: 1113
	// (get) Token: 0x06002B5C RID: 11100 RVA: 0x000E7D70 File Offset: 0x000E5F70
	public EHandedness EquippedHandedness
	{
		get
		{
			if (this.heldByHandIndex == 0 || (this.snappedJoint & SnapJointType.HandL) != SnapJointType.None)
			{
				return EHandedness.Left;
			}
			if (this.heldByHandIndex != 1 && (this.snappedJoint & SnapJointType.HandR) == SnapJointType.None)
			{
				return EHandedness.None;
			}
			return EHandedness.Right;
		}
	}

	// Token: 0x1700045A RID: 1114
	// (get) Token: 0x06002B5D RID: 11101 RVA: 0x000E7D9C File Offset: 0x000E5F9C
	public XRNode EquippedHandXRNode
	{
		get
		{
			if (this.heldByHandIndex == 0 || (this.snappedJoint & SnapJointType.HandL) != SnapJointType.None)
			{
				return XRNode.LeftHand;
			}
			if (this.heldByHandIndex != 1 && (this.snappedJoint & SnapJointType.HandR) == SnapJointType.None)
			{
				return (XRNode)(-1);
			}
			return XRNode.RightHand;
		}
	}

	// Token: 0x06002B5F RID: 11103 RVA: 0x000E7DF8 File Offset: 0x000E5FF8
	[CompilerGenerated]
	private void <GetGrabbableRenderers>g__RemoveNotOwnedComponents|103_0<T>(List<T> components) where T : Component
	{
		for (int i = 0; i < components.Count; i++)
		{
			if (this.manager.GetParentEntity<GameEntity>(components[i].transform) != this)
			{
				components.RemoveAtSwapBack(i--);
			}
		}
	}

	// Token: 0x040037E8 RID: 14312
	public const int Invalid = -1;

	// Token: 0x040037E9 RID: 14313
	public const int ScenePlacedTypeId = -2147483647;

	// Token: 0x040037EE RID: 14318
	public List<GameEntity> builtInEntities;

	// Token: 0x040037EF RID: 14319
	[NonSerialized]
	public bool isBuiltIn;

	// Token: 0x040037F0 RID: 14320
	public bool pickupable = true;

	// Token: 0x040037F1 RID: 14321
	public float pickupRangeFromSurface;

	// Token: 0x040037F2 RID: 14322
	[Tooltip("Renderers on these objects are ignored when determining grab bounds")]
	public GameObject[] ignoreObjectGrabRenderers;

	// Token: 0x040037F3 RID: 14323
	public bool canHoldingPlayerUpdateState;

	// Token: 0x040037F4 RID: 14324
	public bool canLastHoldingPlayerUpdateState;

	// Token: 0x040037F5 RID: 14325
	public bool canSnapPlayerUpdateState;

	// Token: 0x040037F6 RID: 14326
	public AudioSource audioSource;

	// Token: 0x040037F7 RID: 14327
	public AudioClip catchSound;

	// Token: 0x040037F8 RID: 14328
	public float catchSoundVolume = 0.5f;

	// Token: 0x040037F9 RID: 14329
	public AudioClip throwSound;

	// Token: 0x040037FA RID: 14330
	public float throwSoundVolume = 0.5f;

	// Token: 0x040037FB RID: 14331
	public AudioClip snapSound;

	// Token: 0x040037FC RID: 14332
	public float snapSoundVolume = 0.5f;

	// Token: 0x040037FD RID: 14333
	private Rigidbody rigidBody;

	// Token: 0x040037FE RID: 14334
	[SerializeField]
	public MonkeGravityController gravityController;

	// Token: 0x04003806 RID: 14342
	[NonSerialized]
	public GameEntityManager manager;

	// Token: 0x04003807 RID: 14343
	internal bool shouldDestroyOnZoneExit;

	// Token: 0x04003809 RID: 14345
	[NonSerialized]
	internal bool scenePlacedInitialized;

	// Token: 0x0400380A RID: 14346
	[NonSerialized]
	internal Vector3 scenePlacedHomePosition;

	// Token: 0x0400380B RID: 14347
	[NonSerialized]
	internal Quaternion scenePlacedHomeRotation;

	// Token: 0x0400380C RID: 14348
	[NonSerialized]
	internal float scenePlacedHomeScale;

	// Token: 0x0400380D RID: 14349
	public Action OnGrabbed;

	// Token: 0x0400380E RID: 14350
	public Action OnReleased;

	// Token: 0x0400380F RID: 14351
	public Action OnSnapped;

	// Token: 0x04003810 RID: 14352
	public Action OnUnsnapped;

	// Token: 0x04003811 RID: 14353
	public Action OnAttached;

	// Token: 0x04003812 RID: 14354
	public Action OnDetached;

	// Token: 0x04003813 RID: 14355
	public Action OnTick;

	// Token: 0x04003814 RID: 14356
	public float MinTimeBetweenTicks;

	// Token: 0x04003815 RID: 14357
	[NonSerialized]
	public float LastTickTime;

	// Token: 0x04003818 RID: 14360
	private long state;

	// Token: 0x04003819 RID: 14361
	private List<IGameEntityComponent> entityComponents;

	// Token: 0x0400381A RID: 14362
	public List<IGameEntitySerialize> entitySerialize;

	// Token: 0x0400381B RID: 14363
	private GameEntity.RendererSet _grabbableRenderers;

	// Token: 0x0400381C RID: 14364
	private List<MeshFilter> _meshFilters;

	// Token: 0x020006C2 RID: 1730
	public class RendererSet
	{
		// Token: 0x0400381D RID: 14365
		[TupleElementNames(new string[] { "filter", "renderer" })]
		public List<ValueTuple<MeshFilter, MeshRenderer>> renderers = new List<ValueTuple<MeshFilter, MeshRenderer>>();

		// Token: 0x0400381E RID: 14366
		public List<SkinnedMeshRenderer> skinnedRenderers = new List<SkinnedMeshRenderer>();
	}

	// Token: 0x020006C3 RID: 1731
	// (Invoke) Token: 0x06002B62 RID: 11106
	public delegate void StateChangedEvent(long prevState, long nextState);

	// Token: 0x020006C4 RID: 1732
	// (Invoke) Token: 0x06002B66 RID: 11110
	public delegate void EntityDestroyedEvent(GameEntity entity);
}
