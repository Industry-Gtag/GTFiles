using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Fusion;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200086F RID: 2159
public abstract class GorillaGameManager : MonoBehaviourPunCallbacks, ITickSystemTick, IWrappedSerializable, INetworkStruct
{
	// Token: 0x060037E6 RID: 14310 RVA: 0x001320DA File Offset: 0x001302DA
	public static string GameModeEnumToName(GameModeType gameMode)
	{
		return gameMode.ToString();
	}

	// Token: 0x14000064 RID: 100
	// (add) Token: 0x060037E7 RID: 14311 RVA: 0x001320EC File Offset: 0x001302EC
	// (remove) Token: 0x060037E8 RID: 14312 RVA: 0x00132120 File Offset: 0x00130320
	public static event GorillaGameManager.OnTouchDelegate OnTouch;

	// Token: 0x170004FE RID: 1278
	// (get) Token: 0x060037E9 RID: 14313 RVA: 0x00132153 File Offset: 0x00130353
	public static GorillaGameManager instance
	{
		get
		{
			return global::GorillaGameModes.GameMode.ActiveGameMode;
		}
	}

	// Token: 0x170004FF RID: 1279
	// (get) Token: 0x060037EA RID: 14314 RVA: 0x0013215A File Offset: 0x0013035A
	// (set) Token: 0x060037EB RID: 14315 RVA: 0x00132162 File Offset: 0x00130362
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x060037EC RID: 14316 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void Awake()
	{
	}

	// Token: 0x060037ED RID: 14317 RVA: 0x00002C2D File Offset: 0x00000E2D
	private new void OnEnable()
	{
	}

	// Token: 0x060037EE RID: 14318 RVA: 0x00002C2D File Offset: 0x00000E2D
	private new void OnDisable()
	{
	}

	// Token: 0x060037EF RID: 14319 RVA: 0x0013216C File Offset: 0x0013036C
	public virtual void Tick()
	{
		if (this.lastCheck + this.checkCooldown < Time.time)
		{
			this.lastCheck = Time.time;
			if (NetworkSystem.Instance.IsMasterClient && !this.ValidGameMode())
			{
				global::GorillaGameModes.GameMode.ChangeGameFromProperty();
				return;
			}
			this.InfrequentUpdate();
		}
	}

	// Token: 0x060037F0 RID: 14320 RVA: 0x001321B9 File Offset: 0x001303B9
	public virtual void InfrequentUpdate()
	{
		global::GorillaGameModes.GameMode.RefreshPlayers();
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
	}

	// Token: 0x060037F1 RID: 14321 RVA: 0x001321D0 File Offset: 0x001303D0
	public virtual string GameModeName()
	{
		if (this._gameModeName == null)
		{
			this._gameModeName = this.GameType().ToString().ToUpper();
		}
		return this._gameModeName;
	}

	// Token: 0x060037F2 RID: 14322 RVA: 0x0013220C File Offset: 0x0013040C
	public virtual string GameModeNameRoomLabel()
	{
		string text;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_NONE_ROOM_LABEL", out text, "(NONE GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_NONE_ROOM_LABEL]");
		}
		return text;
	}

	// Token: 0x060037F3 RID: 14323 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void LocalTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool bodyHit, bool leftHand)
	{
	}

	// Token: 0x060037F4 RID: 14324 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void ReportTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
	}

	// Token: 0x060037F5 RID: 14325 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void HitPlayer(NetPlayer player)
	{
	}

	// Token: 0x060037F6 RID: 14326 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool CanAffectPlayer(NetPlayer player, bool thisFrame)
	{
		return false;
	}

	// Token: 0x060037F7 RID: 14327 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void HandleHandTap(NetPlayer tappingPlayer, Tappable hitTappable, bool leftHand, Vector3 handVelocity, Vector3 tapSurfaceNormal)
	{
	}

	// Token: 0x060037F8 RID: 14328 RVA: 0x00023F0C File Offset: 0x0002210C
	public virtual bool CanJoinFrienship(NetPlayer player)
	{
		return true;
	}

	// Token: 0x060037F9 RID: 14329 RVA: 0x00023F0C File Offset: 0x0002210C
	public virtual bool CanPlayerParticipate(NetPlayer player)
	{
		return true;
	}

	// Token: 0x060037FA RID: 14330 RVA: 0x00132237 File Offset: 0x00130437
	public virtual void HandleRoundComplete()
	{
		PlayerGameEvents.GameModeCompleteRound();
	}

	// Token: 0x060037FB RID: 14331 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void HandleTagBroadcast(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
	}

	// Token: 0x060037FC RID: 14332 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void HandleTagBroadcast(NetPlayer taggedPlayer, NetPlayer taggingPlayer, double tagTime)
	{
	}

	// Token: 0x060037FD RID: 14333 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
	{
	}

	// Token: 0x060037FE RID: 14334 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return false;
	}

	// Token: 0x060037FF RID: 14335 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool LocalIsTagged(NetPlayer player)
	{
		return false;
	}

	// Token: 0x06003800 RID: 14336 RVA: 0x00132240 File Offset: 0x00130440
	public virtual VRRig FindPlayerVRRig(NetPlayer player)
	{
		RigContainer rigContainer;
		if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return rigContainer.Rig;
		}
		return null;
	}

	// Token: 0x06003801 RID: 14337 RVA: 0x00132268 File Offset: 0x00130468
	public static VRRig StaticFindRigForPlayer(NetPlayer player)
	{
		VRRig vrrig = null;
		RigContainer rigContainer;
		if (GorillaGameManager.instance != null)
		{
			vrrig = GorillaGameManager.instance.FindPlayerVRRig(player);
		}
		else if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			vrrig = rigContainer.Rig;
		}
		return vrrig;
	}

	// Token: 0x06003802 RID: 14338 RVA: 0x001322A9 File Offset: 0x001304A9
	public virtual float[] LocalPlayerSpeed()
	{
		this.playerSpeed[0] = this.slowJumpLimit;
		this.playerSpeed[1] = this.slowJumpMultiplier;
		return this.playerSpeed;
	}

	// Token: 0x06003803 RID: 14339 RVA: 0x001322D0 File Offset: 0x001304D0
	public virtual void UpdatePlayerAppearance(VRRig rig)
	{
		ScienceExperimentManager instance = ScienceExperimentManager.instance;
		int num;
		if (instance != null && instance.GetMaterialIfPlayerInGame(rig.creator.ActorNumber, out num))
		{
			rig.ChangeMaterialLocal(num);
			return;
		}
		int num2 = this.MyMatIndex(rig.creator);
		rig.ChangeMaterialLocal(num2);
	}

	// Token: 0x06003804 RID: 14340 RVA: 0x00002076 File Offset: 0x00000276
	public virtual int MyMatIndex(NetPlayer forPlayer)
	{
		return 0;
	}

	// Token: 0x06003805 RID: 14341 RVA: 0x00116369 File Offset: 0x00114569
	public virtual int SpecialHandFX(NetPlayer player, RigContainer rigContainer)
	{
		return -1;
	}

	// Token: 0x06003806 RID: 14342 RVA: 0x0013231F File Offset: 0x0013051F
	public virtual bool ValidGameMode()
	{
		return NetworkSystem.Instance.InRoom && ((NetworkSystem.Instance.SessionIsPrivate && RoomSystem.IsVStumpRoom) || GameModeString.DoesPropertyStringContainGameMode(NetworkSystem.Instance.GameModeString, this.GameTypeName()));
	}

	// Token: 0x06003807 RID: 14343 RVA: 0x0013235C File Offset: 0x0013055C
	public static void OnInstanceReady(Action action)
	{
		GorillaParent.OnReplicatedClientReady(delegate
		{
			if (GorillaGameManager.instance)
			{
				action();
				return;
			}
			GorillaGameManager.onInstanceReady = (Action)Delegate.Combine(GorillaGameManager.onInstanceReady, action);
		});
	}

	// Token: 0x06003808 RID: 14344 RVA: 0x0013237A File Offset: 0x0013057A
	public static void ReplicatedClientReady()
	{
		GorillaGameManager.replicatedClientReady = true;
	}

	// Token: 0x06003809 RID: 14345 RVA: 0x00132382 File Offset: 0x00130582
	public static void OnReplicatedClientReady(Action action)
	{
		if (GorillaGameManager.replicatedClientReady)
		{
			action();
			return;
		}
		GorillaGameManager.onReplicatedClientReady = (Action)Delegate.Combine(GorillaGameManager.onReplicatedClientReady, action);
	}

	// Token: 0x17000500 RID: 1280
	// (get) Token: 0x0600380A RID: 14346 RVA: 0x001323A7 File Offset: 0x001305A7
	internal GameModeSerializer Serializer
	{
		get
		{
			return this.serializer;
		}
	}

	// Token: 0x0600380B RID: 14347 RVA: 0x001323AF File Offset: 0x001305AF
	internal virtual void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		this.serializer = netSerializer;
	}

	// Token: 0x0600380C RID: 14348 RVA: 0x001323B8 File Offset: 0x001305B8
	internal virtual void NetworkLinkDestroyed(GameModeSerializer netSerializer)
	{
		if (this.serializer == netSerializer)
		{
			this.serializer = null;
		}
	}

	// Token: 0x0600380D RID: 14349
	public abstract GameModeType GameType();

	// Token: 0x0600380E RID: 14350 RVA: 0x001323D0 File Offset: 0x001305D0
	public string GameTypeName()
	{
		return this.GameType().ToString();
	}

	// Token: 0x0600380F RID: 14351
	public abstract void AddFusionDataBehaviour(NetworkObject behaviour);

	// Token: 0x06003810 RID: 14352
	public abstract void OnSerializeRead(object newData);

	// Token: 0x06003811 RID: 14353
	public abstract object OnSerializeWrite();

	// Token: 0x06003812 RID: 14354
	public abstract void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06003813 RID: 14355
	public abstract void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06003814 RID: 14356 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void ResetGame()
	{
	}

	// Token: 0x06003815 RID: 14357 RVA: 0x001323F4 File Offset: 0x001305F4
	public virtual void StartPlaying()
	{
		TickSystem<object>.AddTickCallback(this);
		NetworkSystem.Instance.OnPlayerJoined += this.OnPlayerEnteredRoom;
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnMasterClientSwitchedEvent += this.OnMasterClientSwitched;
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
		GorillaTelemetry.PostGameModeEvent(GTGameModeEventType.game_mode_start, this.GameType());
	}

	// Token: 0x06003816 RID: 14358 RVA: 0x0013248C File Offset: 0x0013068C
	public virtual void StopPlaying()
	{
		TickSystem<object>.RemoveTickCallback(this);
		NetworkSystem.Instance.OnPlayerJoined -= this.OnPlayerEnteredRoom;
		NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnMasterClientSwitchedEvent -= this.OnMasterClientSwitched;
		this.lastCheck = 0f;
	}

	// Token: 0x06003817 RID: 14359 RVA: 0x00002C2D File Offset: 0x00000E2D
	public new virtual void OnMasterClientSwitched(Player newMaster)
	{
	}

	// Token: 0x06003818 RID: 14360 RVA: 0x00002C2D File Offset: 0x00000E2D
	public new virtual void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06003819 RID: 14361 RVA: 0x00002C2D File Offset: 0x00000E2D
	public new virtual void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x0600381A RID: 14362 RVA: 0x00132510 File Offset: 0x00130710
	public virtual void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
		if (this.lastTaggedActorNr.ContainsKey(otherPlayer.ActorNumber))
		{
			this.lastTaggedActorNr.Remove(otherPlayer.ActorNumber);
		}
	}

	// Token: 0x0600381B RID: 14363 RVA: 0x00132547 File Offset: 0x00130747
	public virtual void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
	}

	// Token: 0x0600381C RID: 14364 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void OnMasterClientSwitched(NetPlayer newMaster)
	{
	}

	// Token: 0x0600381D RID: 14365 RVA: 0x0013255C File Offset: 0x0013075C
	internal static void ForceStopGame_DisconnectAndDestroy()
	{
		Application.Quit();
		NetworkSystem instance = NetworkSystem.Instance;
		if (instance != null)
		{
			instance.ReturnToSinglePlayer();
		}
		Object.DestroyImmediate(PhotonNetworkController.Instance);
		Object.DestroyImmediate(GTPlayer.Instance);
		GameObject[] array = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i]);
		}
	}

	// Token: 0x0600381E RID: 14366 RVA: 0x001325B4 File Offset: 0x001307B4
	public void AddLastTagged(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		if (this.lastTaggedActorNr.ContainsKey(taggedPlayer.ActorNumber))
		{
			this.lastTaggedActorNr[taggedPlayer.ActorNumber] = taggingPlayer.ActorNumber;
			return;
		}
		this.lastTaggedActorNr.Add(taggedPlayer.ActorNumber, taggingPlayer.ActorNumber);
	}

	// Token: 0x0600381F RID: 14367 RVA: 0x00132604 File Offset: 0x00130804
	public void WriteLastTagged(PhotonStream stream)
	{
		stream.SendNext(this.lastTaggedActorNr.Count);
		foreach (KeyValuePair<int, int> keyValuePair in this.lastTaggedActorNr)
		{
			stream.SendNext(keyValuePair.Key);
			stream.SendNext(keyValuePair.Value);
		}
	}

	// Token: 0x06003820 RID: 14368 RVA: 0x0013268C File Offset: 0x0013088C
	public void ReadLastTagged(PhotonStream stream)
	{
		this.lastTaggedActorNr.Clear();
		int num = Mathf.Min((int)stream.ReceiveNext(), 20);
		for (int i = 0; i < num; i++)
		{
			this.lastTaggedActorNr.Add((int)stream.ReceiveNext(), (int)stream.ReceiveNext());
		}
	}

	// Token: 0x04004823 RID: 18467
	protected const string GAME_MODE_NONE_KEY = "GAME_MODE_NONE";

	// Token: 0x04004824 RID: 18468
	protected const string GAME_MODE_CASUAL_ROOM_LABEL_KEY = "GAME_MODE_CASUAL_ROOM_LABEL";

	// Token: 0x04004825 RID: 18469
	protected const string GAME_MODE_INFECTION_ROOM_LABEL_KEY = "GAME_MODE_INFECTION_ROOM_LABEL";

	// Token: 0x04004826 RID: 18470
	protected const string GAME_MODE_HUNT_ROOM_LABEL_KEY = "GAME_MODE_HUNT_ROOM_LABEL";

	// Token: 0x04004827 RID: 18471
	protected const string GAME_MODE_PAINTBRAWL_ROOM_LABEL_KEY = "GAME_MODE_PAINTBRAWL_ROOM_LABEL";

	// Token: 0x04004828 RID: 18472
	protected const string GAME_MODE_SUPER_INFECTION_ROOM_LABEL_KEY = "GAME_MODE_SUPER_INFECTION_ROOM_LABEL";

	// Token: 0x04004829 RID: 18473
	protected const string GAME_MODE_SUPER_CASUAL_ROOM_LABEL_KEY = "GAME_MODE_SUPER_CASUAL_ROOM_LABEL";

	// Token: 0x0400482A RID: 18474
	protected const string GAME_MODE_NONE_ROOM_LABEL_KEY = "GAME_MODE_NONE_ROOM_LABEL";

	// Token: 0x0400482B RID: 18475
	protected const string GAME_MODE_CUSTOM_ROOM_LABEL_KEY = "GAME_MODE_CUSTOM_ROOM_LABEL";

	// Token: 0x0400482C RID: 18476
	protected const string GAME_MODE_GHOST_ROOM_LABEL_KEY = "GAME_MODE_GHOST_ROOM_LABEL";

	// Token: 0x0400482D RID: 18477
	protected const string GAME_MODE_AMBUSH_ROOM_LABEL_KEY = "GAME_MODE_AMBUSH_ROOM_LABEL";

	// Token: 0x0400482E RID: 18478
	protected const string GAME_MODE_FREEZE_TAG_ROOM_LABEL_KEY = "GAME_MODE_FREEZE_TAG_ROOM_LABEL";

	// Token: 0x0400482F RID: 18479
	protected const string GAME_MODE_GUARDIAN_ROOM_LABEL_KEY = "GAME_MODE_GUARDIAN_ROOM_LABEL";

	// Token: 0x04004830 RID: 18480
	protected const string GAME_MODE_PROP_HUNT_ROOM_LABEL_KEY = "GAME_MODE_PROP_HUNT_ROOM_LABEL";

	// Token: 0x04004831 RID: 18481
	protected const string GAME_MODE_COMP_INF_ROOM_LABEL_KEY = "GAME_MODE_COMP_INF_ROOM_LABEL";

	// Token: 0x04004832 RID: 18482
	public const int k_defaultMatIndex = 0;

	// Token: 0x04004834 RID: 18484
	public float fastJumpLimit;

	// Token: 0x04004835 RID: 18485
	public float fastJumpMultiplier;

	// Token: 0x04004836 RID: 18486
	public float slowJumpLimit;

	// Token: 0x04004837 RID: 18487
	public float slowJumpMultiplier;

	// Token: 0x04004838 RID: 18488
	public float lastCheck;

	// Token: 0x04004839 RID: 18489
	public float checkCooldown = 3f;

	// Token: 0x0400483A RID: 18490
	public float tagDistanceThreshold = 4f;

	// Token: 0x0400483B RID: 18491
	private NetPlayer outPlayer;

	// Token: 0x0400483C RID: 18492
	private int outInt;

	// Token: 0x0400483D RID: 18493
	private VRRig tempRig;

	// Token: 0x0400483E RID: 18494
	public NetPlayer[] currentNetPlayerArray;

	// Token: 0x0400483F RID: 18495
	public float[] playerSpeed = new float[2];

	// Token: 0x04004840 RID: 18496
	public Dictionary<int, int> lastTaggedActorNr = new Dictionary<int, int>();

	// Token: 0x04004842 RID: 18498
	private string _gameModeName;

	// Token: 0x04004843 RID: 18499
	private static Action onInstanceReady;

	// Token: 0x04004844 RID: 18500
	private static bool replicatedClientReady;

	// Token: 0x04004845 RID: 18501
	private static Action onReplicatedClientReady;

	// Token: 0x04004846 RID: 18502
	private GameModeSerializer serializer;

	// Token: 0x02000870 RID: 2160
	// (Invoke) Token: 0x06003823 RID: 14371
	public delegate void OnTouchDelegate(NetPlayer taggedPlayer, NetPlayer taggingPlayer);
}
