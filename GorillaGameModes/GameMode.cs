using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using UnityEngine;

namespace GorillaGameModes
{
	// Token: 0x02000F2E RID: 3886
	public class GameMode : MonoBehaviour
	{
		// Token: 0x06005F3C RID: 24380 RVA: 0x001E3DD8 File Offset: 0x001E1FD8
		private void Awake()
		{
			if (GameMode.instance.IsNull())
			{
				GameMode.instance = this;
				foreach (GorillaGameManager gorillaGameManager in base.gameObject.GetComponentsInChildren<GorillaGameManager>(true))
				{
					int num = (int)gorillaGameManager.GameType();
					string text = gorillaGameManager.GameTypeName();
					if (GameMode.gameModeTable.ContainsKey(num))
					{
						Debug.LogWarning("Duplicate gamemode type, skipping this instance", gorillaGameManager);
					}
					else
					{
						GameMode.gameModeTable.Add((int)gorillaGameManager.GameType(), gorillaGameManager);
						GameMode.gameModeKeyByName.Add(text, num);
						GameMode.gameModes.Add(gorillaGameManager);
						GameMode.gameModeNames.Add(text);
					}
				}
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x06005F3D RID: 24381 RVA: 0x001E3E7D File Offset: 0x001E207D
		private void OnDestroy()
		{
			if (GameMode.instance == this)
			{
				GameMode.instance = null;
			}
		}

		// Token: 0x140000A6 RID: 166
		// (add) Token: 0x06005F3E RID: 24382 RVA: 0x001E3E94 File Offset: 0x001E2094
		// (remove) Token: 0x06005F3F RID: 24383 RVA: 0x001E3EC8 File Offset: 0x001E20C8
		public static event GameMode.OnStartGameModeAction OnStartGameMode;

		// Token: 0x17000938 RID: 2360
		// (get) Token: 0x06005F40 RID: 24384 RVA: 0x001E3EFB File Offset: 0x001E20FB
		public static GorillaGameManager ActiveGameMode
		{
			get
			{
				return GameMode.activeGameMode;
			}
		}

		// Token: 0x17000939 RID: 2361
		// (get) Token: 0x06005F41 RID: 24385 RVA: 0x001E3F02 File Offset: 0x001E2102
		internal static GameModeSerializer ActiveNetworkHandler
		{
			get
			{
				return GameMode.activeNetworkHandler;
			}
		}

		// Token: 0x1700093A RID: 2362
		// (get) Token: 0x06005F42 RID: 24386 RVA: 0x001E3F09 File Offset: 0x001E2109
		public static GameModeZoneMapping GameModeZoneMapping
		{
			get
			{
				return GameMode.instance.gameModeZoneMapping;
			}
		}

		// Token: 0x1700093B RID: 2363
		// (get) Token: 0x06005F43 RID: 24387 RVA: 0x001E3F15 File Offset: 0x001E2115
		// (set) Token: 0x06005F44 RID: 24388 RVA: 0x001E3F1C File Offset: 0x001E211C
		public static GameModeType CurrentGameModeType { get; private set; } = GameModeType.None;

		// Token: 0x1700093C RID: 2364
		// (get) Token: 0x06005F45 RID: 24389 RVA: 0x001E3F24 File Offset: 0x001E2124
		public static int CurrentGameModeFlag
		{
			get
			{
				return 1 << (int)GameMode.CurrentGameModeType;
			}
		}

		// Token: 0x140000A7 RID: 167
		// (add) Token: 0x06005F46 RID: 24390 RVA: 0x001E3F30 File Offset: 0x001E2130
		// (remove) Token: 0x06005F47 RID: 24391 RVA: 0x001E3F64 File Offset: 0x001E2164
		public static event Action<List<NetPlayer>, List<NetPlayer>> ParticipatingPlayersChanged;

		// Token: 0x06005F48 RID: 24392 RVA: 0x001E3F98 File Offset: 0x001E2198
		static GameMode()
		{
			GameMode.StaticLoad();
		}

		// Token: 0x06005F49 RID: 24393 RVA: 0x001E403C File Offset: 0x001E223C
		[OnEnterPlay_Run]
		private static void StaticLoad()
		{
			RoomSystem.LeftRoomEvent += new Action(GameMode.ResetGameModes);
			RoomSystem.JoinedRoomEvent += new Action(GameMode.RefreshPlayers);
			RoomSystem.PlayersChangedEvent += new Action(GameMode.RefreshPlayers);
		}

		// Token: 0x06005F4A RID: 24394 RVA: 0x001E409A File Offset: 0x001E229A
		public static bool IsPlaying(GameModeType type)
		{
			return type == GameMode.CurrentGameModeType;
		}

		// Token: 0x06005F4B RID: 24395 RVA: 0x001E40A4 File Offset: 0x001E22A4
		internal static bool LoadGameModeFromProperty()
		{
			return GameMode.LoadGameMode(GameMode.FindGameModeFromRoomProperty());
		}

		// Token: 0x06005F4C RID: 24396 RVA: 0x001E40B0 File Offset: 0x001E22B0
		internal static bool ChangeGameFromProperty()
		{
			return GameMode.ChangeGameMode(GameMode.FindGameModeFromRoomProperty());
		}

		// Token: 0x06005F4D RID: 24397 RVA: 0x001E40BC File Offset: 0x001E22BC
		internal static bool LoadGameModeFromProperty(string prop)
		{
			return GameMode.LoadGameMode(GameMode.FindGameModeInPropertyString(prop));
		}

		// Token: 0x06005F4E RID: 24398 RVA: 0x001E40C9 File Offset: 0x001E22C9
		internal static bool ChangeGameFromProperty(string prop)
		{
			return GameMode.ChangeGameMode(GameMode.FindGameModeInPropertyString(prop));
		}

		// Token: 0x06005F4F RID: 24399 RVA: 0x001E40D8 File Offset: 0x001E22D8
		public static int GetGameModeKeyFromRoomProp()
		{
			string text = GameMode.FindGameModeFromRoomProperty();
			int num;
			if (string.IsNullOrEmpty(text) || !GameMode.gameModeKeyByName.TryGetValue(text, out num))
			{
				GTDev.LogWarning<string>("Unable to find game mode key for " + text, null);
				return -1;
			}
			return num;
		}

		// Token: 0x06005F50 RID: 24400 RVA: 0x001E4116 File Offset: 0x001E2316
		private static string FindGameModeFromRoomProperty()
		{
			if (!NetworkSystem.Instance.InRoom || string.IsNullOrEmpty(NetworkSystem.Instance.GameModeString))
			{
				return null;
			}
			return GameMode.FindGameModeInPropertyString(NetworkSystem.Instance.GameModeString);
		}

		// Token: 0x06005F51 RID: 24401 RVA: 0x001E4146 File Offset: 0x001E2346
		public static bool IsValidGameMode(string gameMode)
		{
			return !string.IsNullOrEmpty(gameMode) && GameMode.gameModeKeyByName.ContainsKey(gameMode);
		}

		// Token: 0x06005F52 RID: 24402 RVA: 0x001E415D File Offset: 0x001E235D
		private static string FindGameModeInPropertyString(string gmString)
		{
			return new string(GameModeString.GameTypeFromPropertyString(gmString));
		}

		// Token: 0x06005F53 RID: 24403 RVA: 0x001E416C File Offset: 0x001E236C
		public static bool LoadGameMode(string gameMode)
		{
			if (gameMode == null)
			{
				Debug.LogError("GAME MODE NULL");
				return false;
			}
			int num;
			if (!GameMode.gameModeKeyByName.TryGetValue(gameMode, out num))
			{
				Debug.LogWarning("Unable to find game mode key for " + gameMode);
				return false;
			}
			return GameMode.LoadGameMode(num);
		}

		// Token: 0x06005F54 RID: 24404 RVA: 0x001E41B0 File Offset: 0x001E23B0
		public static bool LoadGameMode(int key)
		{
			foreach (KeyValuePair<int, GorillaGameManager> keyValuePair in GameMode.gameModeTable)
			{
			}
			if (!GameMode.gameModeTable.ContainsKey(key))
			{
				Debug.LogWarning("Missing game mode for key " + key.ToString());
				return false;
			}
			PrefabType prefabType;
			VRRigCache.Instance.GetComponent<PhotonPrefabPool>().networkPrefabs.TryGetValue("GameMode", out prefabType);
			GameObject prefab = prefabType.prefab;
			if (prefab == null)
			{
				GTDev.LogError<string>("Unable to find game mode prefab to spawn", null);
				return false;
			}
			if (NetworkSystem.Instance.NetInstantiate(prefab, Vector3.zero, Quaternion.identity, true, 0, new object[] { key }, delegate(NetworkRunner runner, NetworkObject no)
			{
				no.GetComponent<GameModeSerializer>().Init(key);
			}).IsNull())
			{
				GTDev.LogWarning<string>("Unable to create GameManager with key " + key.ToString(), null);
				return false;
			}
			return true;
		}

		// Token: 0x06005F55 RID: 24405 RVA: 0x001E42CC File Offset: 0x001E24CC
		internal static bool ChangeGameMode(string gameMode)
		{
			if (gameMode == null)
			{
				return false;
			}
			int num;
			if (!GameMode.gameModeKeyByName.TryGetValue(gameMode, out num))
			{
				Debug.LogWarning("Unable to find game mode key for " + gameMode);
				return false;
			}
			return GameMode.ChangeGameMode(num);
		}

		// Token: 0x06005F56 RID: 24406 RVA: 0x001E4308 File Offset: 0x001E2508
		internal static bool ChangeGameMode(int key)
		{
			GorillaGameManager gorillaGameManager;
			if (!NetworkSystem.Instance.IsMasterClient || !GameMode.gameModeTable.TryGetValue(key, out gorillaGameManager) || gorillaGameManager == GameMode.activeGameMode)
			{
				return false;
			}
			if (GameMode.activeNetworkHandler.IsNotNull())
			{
				NetworkSystem.Instance.NetDestroy(GameMode.activeNetworkHandler.gameObject);
			}
			GameMode.StopGameModeSafe(GameMode.activeGameMode);
			GameMode.activeGameMode = null;
			GameMode.activeNetworkHandler = null;
			GameMode.CurrentGameModeType = GameModeType.None;
			return GameMode.LoadGameMode(key);
		}

		// Token: 0x06005F57 RID: 24407 RVA: 0x001E4384 File Offset: 0x001E2584
		internal static void SetupGameModeRemote(GameModeSerializer networkSerializer)
		{
			GorillaGameManager gameModeInstance = networkSerializer.GameModeInstance;
			bool flag = gameModeInstance != GameMode.activeGameMode;
			if (GameMode.activeGameMode.IsNotNull() && gameModeInstance.IsNotNull() && flag)
			{
				GameMode.StopGameModeSafe(GameMode.activeGameMode);
			}
			GameMode.activeNetworkHandler = networkSerializer;
			GameMode.activeGameMode = gameModeInstance;
			GameMode.activeGameMode.NetworkLinkSetup(networkSerializer);
			GameMode.CurrentGameModeType = GameMode.activeGameMode.GameType();
			if (!GameMode.activatedGameModes.Contains(GameMode.activeGameMode))
			{
				GameMode.activatedGameModes.Add(GameMode.activeGameMode);
			}
			if (flag)
			{
				GameMode.StartGameModeSafe(GameMode.activeGameMode);
				if (GameMode.OnStartGameMode != null)
				{
					GameMode.OnStartGameMode(GameMode.activeGameMode.GameType());
				}
			}
		}

		// Token: 0x06005F58 RID: 24408 RVA: 0x001E4435 File Offset: 0x001E2635
		internal static void RemoveNetworkLink(GameModeSerializer networkSerializer)
		{
			if (GameMode.activeGameMode.IsNotNull() && networkSerializer == GameMode.activeNetworkHandler)
			{
				GameMode.activeGameMode.NetworkLinkDestroyed(networkSerializer);
				GameMode.activeNetworkHandler = null;
				return;
			}
		}

		// Token: 0x06005F59 RID: 24409 RVA: 0x001E4462 File Offset: 0x001E2662
		public static GorillaGameManager GetGameModeInstance(GameModeType type)
		{
			return GameMode.GetGameModeInstance((int)type);
		}

		// Token: 0x06005F5A RID: 24410 RVA: 0x001E446C File Offset: 0x001E266C
		public static GorillaGameManager GetGameModeInstance(int type)
		{
			GorillaGameManager gorillaGameManager;
			if (GameMode.gameModeTable.TryGetValue(type, out gorillaGameManager))
			{
				if (gorillaGameManager == null)
				{
					Debug.LogError("Couldnt get mode from table");
					foreach (KeyValuePair<int, GorillaGameManager> keyValuePair in GameMode.gameModeTable)
					{
					}
				}
				return gorillaGameManager;
			}
			return null;
		}

		// Token: 0x06005F5B RID: 24411 RVA: 0x001E44DC File Offset: 0x001E26DC
		public static T GetGameModeInstance<T>(GameModeType type) where T : GorillaGameManager
		{
			return GameMode.GetGameModeInstance<T>((int)type);
		}

		// Token: 0x06005F5C RID: 24412 RVA: 0x001E44E4 File Offset: 0x001E26E4
		public static T GetGameModeInstance<T>(int type) where T : GorillaGameManager
		{
			T t = GameMode.GetGameModeInstance(type) as T;
			if (t != null)
			{
				return t;
			}
			return default(T);
		}

		// Token: 0x06005F5D RID: 24413 RVA: 0x001E4518 File Offset: 0x001E2718
		public static void ResetGameModes()
		{
			GameMode.CurrentGameModeType = GameModeType.None;
			GameMode.activeGameMode = null;
			GameMode.activeNetworkHandler = null;
			GameMode.optOutPlayers.Clear();
			GameMode.ParticipatingPlayers.Clear();
			for (int i = 0; i < GameMode.activatedGameModes.Count; i++)
			{
				GorillaGameManager gorillaGameManager = GameMode.activatedGameModes[i];
				GameMode.StopGameModeSafe(gorillaGameManager);
				GameMode.ResetGameModeSafe(gorillaGameManager);
			}
			GameMode.activatedGameModes.Clear();
		}

		// Token: 0x06005F5E RID: 24414 RVA: 0x001E4580 File Offset: 0x001E2780
		private static void StartGameModeSafe(GorillaGameManager gameMode)
		{
			try
			{
				gameMode.StartPlaying();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06005F5F RID: 24415 RVA: 0x001E45A8 File Offset: 0x001E27A8
		private static void StopGameModeSafe(GorillaGameManager gameMode)
		{
			try
			{
				gameMode.StopPlaying();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06005F60 RID: 24416 RVA: 0x001E45D0 File Offset: 0x001E27D0
		private static void ResetGameModeSafe(GorillaGameManager gameMode)
		{
			try
			{
				gameMode.ResetGame();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06005F61 RID: 24417 RVA: 0x001E45F8 File Offset: 0x001E27F8
		public static void ReportTag(NetPlayer player)
		{
			if (NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_ReportTag", false, new object[] { player.ActorNumber });
			}
		}

		// Token: 0x06005F62 RID: 24418 RVA: 0x001E4638 File Offset: 0x001E2838
		public static void ReportHit()
		{
			if (GorillaGameManager.instance.GameType() == GameModeType.Custom)
			{
				CustomGameMode.TaggedByEnvironment();
			}
			if (NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_ReportHit", false, Array.Empty<object>());
			}
		}

		// Token: 0x06005F63 RID: 24419 RVA: 0x001E4684 File Offset: 0x001E2884
		public static bool LocalIsTagged(NetPlayer player)
		{
			return !GameMode.ActiveGameMode.IsNull() && GameMode.ActiveGameMode.LocalIsTagged(player);
		}

		// Token: 0x06005F64 RID: 24420 RVA: 0x001E469F File Offset: 0x001E289F
		public static void BroadcastRoundComplete()
		{
			if (NetworkSystem.Instance.IsMasterClient && NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_BroadcastRoundComplete", true, Array.Empty<object>());
			}
		}

		// Token: 0x06005F65 RID: 24421 RVA: 0x001E46DC File Offset: 0x001E28DC
		public static void BroadcastTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
		{
			if (NetworkSystem.Instance.IsMasterClient && NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_BroadcastTag", true, new object[] { taggedPlayer.ActorNumber, taggingPlayer.ActorNumber });
			}
		}

		// Token: 0x1700093D RID: 2365
		// (get) Token: 0x06005F66 RID: 24422 RVA: 0x001E473F File Offset: 0x001E293F
		public static List<NetPlayer> ParticipatingPlayers
		{
			get
			{
				return GameMode._participatingPlayers;
			}
		}

		// Token: 0x06005F67 RID: 24423 RVA: 0x001E4748 File Offset: 0x001E2948
		public static void RefreshPlayers()
		{
			GameMode._oldPlayersCount = GameMode._participatingPlayers.Count;
			for (int i = 0; i < GameMode._oldPlayersCount; i++)
			{
				GameMode._oldPlayersBuffer[i] = GameMode._participatingPlayers[i];
			}
			GameMode._participatingPlayers.Clear();
			List<NetPlayer> playersInRoom = RoomSystem.PlayersInRoom;
			int num = Mathf.Min(playersInRoom.Count, 20);
			for (int j = 0; j < num; j++)
			{
				if (GameMode.CanParticipate(playersInRoom[j]))
				{
					GameMode.ParticipatingPlayers.Add(playersInRoom[j]);
				}
			}
			GameMode._tempRemovedPlayers.Clear();
			for (int k = 0; k < GameMode._oldPlayersCount; k++)
			{
				NetPlayer netPlayer = GameMode._oldPlayersBuffer[k];
				if (!GameMode.ContainsNetPlayer(GameMode._participatingPlayers, netPlayer))
				{
					GameMode._tempRemovedPlayers.Add(netPlayer);
				}
			}
			GameMode._tempAddedPlayers.Clear();
			int count = GameMode._participatingPlayers.Count;
			for (int l = 0; l < count; l++)
			{
				NetPlayer netPlayer2 = GameMode._participatingPlayers[l];
				if (!GameMode.ContainsNetPlayer(GameMode._oldPlayersBuffer, netPlayer2, GameMode._oldPlayersCount))
				{
					GameMode._tempAddedPlayers.Add(netPlayer2);
				}
			}
			if ((GameMode._tempAddedPlayers.Count > 0 || GameMode._tempRemovedPlayers.Count > 0) && GameMode.ParticipatingPlayersChanged != null)
			{
				GameMode.ParticipatingPlayersChanged(GameMode._tempAddedPlayers, GameMode._tempRemovedPlayers);
			}
		}

		// Token: 0x06005F68 RID: 24424 RVA: 0x001E48A0 File Offset: 0x001E2AA0
		private static bool ContainsNetPlayer(List<NetPlayer> list, NetPlayer candidate)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (list[i] == candidate)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005F69 RID: 24425 RVA: 0x001E48D0 File Offset: 0x001E2AD0
		private static bool ContainsNetPlayer(NetPlayer[] array, NetPlayer candidate, int length)
		{
			for (int i = 0; i < length; i++)
			{
				if (array[i] == candidate)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005F6A RID: 24426 RVA: 0x001E48F2 File Offset: 0x001E2AF2
		public static void OptOut(VRRig rig)
		{
			GameMode.OptOut(rig.creator.ActorNumber);
		}

		// Token: 0x06005F6B RID: 24427 RVA: 0x001E4904 File Offset: 0x001E2B04
		public static void OptOut(NetPlayer player)
		{
			GameMode.OptOut(player.ActorNumber);
		}

		// Token: 0x06005F6C RID: 24428 RVA: 0x001E4911 File Offset: 0x001E2B11
		public static void OptOut(int playerActorNumber)
		{
			if (GameMode.optOutPlayers.Add(playerActorNumber))
			{
				GameMode.RefreshPlayers();
			}
		}

		// Token: 0x06005F6D RID: 24429 RVA: 0x001E4925 File Offset: 0x001E2B25
		public static void OptIn(VRRig rig)
		{
			GameMode.OptIn(rig.creator.ActorNumber);
		}

		// Token: 0x06005F6E RID: 24430 RVA: 0x001E4937 File Offset: 0x001E2B37
		public static void OptIn(NetPlayer player)
		{
			GameMode.OptIn(player.ActorNumber);
		}

		// Token: 0x06005F6F RID: 24431 RVA: 0x001E4944 File Offset: 0x001E2B44
		public static void OptIn(int playerActorNumber)
		{
			if (GameMode.optOutPlayers.Remove(playerActorNumber))
			{
				GameMode.RefreshPlayers();
			}
		}

		// Token: 0x06005F70 RID: 24432 RVA: 0x001E4958 File Offset: 0x001E2B58
		private static bool CanParticipate(NetPlayer player)
		{
			return player.InRoom() && !GameMode.optOutPlayers.Contains(player.ActorNumber) && NetworkSystem.Instance.GetPlayerTutorialCompletion(player.ActorNumber) && (!(GorillaGameManager.instance != null) || GorillaGameManager.instance.CanPlayerParticipate(player));
		}

		// Token: 0x04006DBF RID: 28095
		[SerializeField]
		private GameModeZoneMapping gameModeZoneMapping;

		// Token: 0x04006DC1 RID: 28097
		[OnEnterPlay_SetNull]
		private static GameMode instance;

		// Token: 0x04006DC2 RID: 28098
		[OnEnterPlay_Clear]
		private static Dictionary<int, GorillaGameManager> gameModeTable = new Dictionary<int, GorillaGameManager>();

		// Token: 0x04006DC3 RID: 28099
		[OnEnterPlay_Clear]
		public static Dictionary<string, int> gameModeKeyByName = new Dictionary<string, int>();

		// Token: 0x04006DC4 RID: 28100
		[OnEnterPlay_Clear]
		private static Dictionary<int, FusionGameModeData> fusionTypeTable = new Dictionary<int, FusionGameModeData>();

		// Token: 0x04006DC5 RID: 28101
		[OnEnterPlay_Clear]
		public static List<GorillaGameManager> gameModes = new List<GorillaGameManager>(10);

		// Token: 0x04006DC6 RID: 28102
		[OnEnterPlay_Clear]
		public static readonly List<string> gameModeNames = new List<string>(10);

		// Token: 0x04006DC7 RID: 28103
		[OnEnterPlay_Clear]
		private static readonly List<GorillaGameManager> activatedGameModes = new List<GorillaGameManager>(13);

		// Token: 0x04006DC8 RID: 28104
		[OnEnterPlay_SetNull]
		private static GorillaGameManager activeGameMode = null;

		// Token: 0x04006DC9 RID: 28105
		[OnEnterPlay_SetNull]
		private static GameModeSerializer activeNetworkHandler = null;

		// Token: 0x04006DCC RID: 28108
		[OnEnterPlay_Clear]
		private static readonly HashSet<int> optOutPlayers = new HashSet<int>(20);

		// Token: 0x04006DCD RID: 28109
		[OnEnterPlay_Clear]
		private static readonly List<NetPlayer> _participatingPlayers = new List<NetPlayer>(20);

		// Token: 0x04006DCE RID: 28110
		private static readonly NetPlayer[] _oldPlayersBuffer = new NetPlayer[20];

		// Token: 0x04006DCF RID: 28111
		private static int _oldPlayersCount;

		// Token: 0x04006DD0 RID: 28112
		private static readonly List<NetPlayer> _tempAddedPlayers = new List<NetPlayer>(20);

		// Token: 0x04006DD1 RID: 28113
		private static readonly List<NetPlayer> _tempRemovedPlayers = new List<NetPlayer>(20);

		// Token: 0x02000F2F RID: 3887
		// (Invoke) Token: 0x06005F73 RID: 24435
		public delegate void OnStartGameModeAction(GameModeType newGameModeType);
	}
}
