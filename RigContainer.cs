using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaNetworking;
using GorillaTag.Audio;
using Newtonsoft.Json;
using Photon.Voice.PUN;
using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine;

// Token: 0x02000852 RID: 2130
[RequireComponent(typeof(VRRig), typeof(VRRigReliableState))]
public class RigContainer : MonoBehaviour
{
	// Token: 0x170004D0 RID: 1232
	// (get) Token: 0x060036D0 RID: 14032 RVA: 0x0012E099 File Offset: 0x0012C299
	// (set) Token: 0x060036D1 RID: 14033 RVA: 0x0012E0A1 File Offset: 0x0012C2A1
	public bool Initialized { get; private set; }

	// Token: 0x170004D1 RID: 1233
	// (get) Token: 0x060036D2 RID: 14034 RVA: 0x0012E0AA File Offset: 0x0012C2AA
	public VRRig Rig
	{
		get
		{
			return this.vrrig;
		}
	}

	// Token: 0x170004D2 RID: 1234
	// (get) Token: 0x060036D3 RID: 14035 RVA: 0x0012E0B2 File Offset: 0x0012C2B2
	public VRRigReliableState ReliableState
	{
		get
		{
			return this.reliableState;
		}
	}

	// Token: 0x170004D3 RID: 1235
	// (get) Token: 0x060036D4 RID: 14036 RVA: 0x0012E0BA File Offset: 0x0012C2BA
	public Transform SpeakerHead
	{
		get
		{
			return this.speakerHead;
		}
	}

	// Token: 0x170004D4 RID: 1236
	// (get) Token: 0x060036D5 RID: 14037 RVA: 0x0012E0C2 File Offset: 0x0012C2C2
	public AudioSource ReplacementVoiceSource
	{
		get
		{
			return this.replacementVoiceSource;
		}
	}

	// Token: 0x170004D5 RID: 1237
	// (get) Token: 0x060036D6 RID: 14038 RVA: 0x0012E0CA File Offset: 0x0012C2CA
	public List<LoudSpeakerNetwork> LoudSpeakerNetworks
	{
		get
		{
			return this.loudSpeakerNetworks;
		}
	}

	// Token: 0x170004D6 RID: 1238
	// (get) Token: 0x060036D7 RID: 14039 RVA: 0x0012E0D2 File Offset: 0x0012C2D2
	public LCKSocialCameraFollower LckCococamFollower
	{
		get
		{
			return this.m_lckCococamFollower;
		}
	}

	// Token: 0x170004D7 RID: 1239
	// (get) Token: 0x060036D8 RID: 14040 RVA: 0x0012E0DA File Offset: 0x0012C2DA
	public LCKSocialCameraFollower LCKTabletFollower
	{
		get
		{
			return this.m_lckTablet;
		}
	}

	// Token: 0x170004D8 RID: 1240
	// (get) Token: 0x060036D9 RID: 14041 RVA: 0x0012E0E2 File Offset: 0x0012C2E2
	// (set) Token: 0x060036DA RID: 14042 RVA: 0x0012E0EA File Offset: 0x0012C2EA
	public PhotonVoiceView Voice
	{
		get
		{
			return this.voiceView;
		}
		set
		{
			if (value == this.voiceView)
			{
				return;
			}
			if (this.voiceView != null)
			{
				this.voiceView.SpeakerInUse.enabled = false;
			}
			this.voiceView = value;
			this.RefreshVoiceChat();
		}
	}

	// Token: 0x170004D9 RID: 1241
	// (get) Token: 0x060036DB RID: 14043 RVA: 0x0012E127 File Offset: 0x0012C327
	public NetworkView netView
	{
		get
		{
			return this.vrrig.netView;
		}
	}

	// Token: 0x170004DA RID: 1242
	// (get) Token: 0x060036DC RID: 14044 RVA: 0x0012E134 File Offset: 0x0012C334
	public int CachedNetViewID
	{
		get
		{
			return this.m_cachedNetViewID;
		}
	}

	// Token: 0x170004DB RID: 1243
	// (get) Token: 0x060036DD RID: 14045 RVA: 0x0012E13C File Offset: 0x0012C33C
	public bool IsMuted
	{
		get
		{
			return this.muteReasons > RigContainer.MuteReason.None;
		}
	}

	// Token: 0x060036DE RID: 14046 RVA: 0x0012E147 File Offset: 0x0012C347
	public bool IsMutedFor(RigContainer.MuteReason reasons)
	{
		return (this.muteReasons & reasons) > RigContainer.MuteReason.None;
	}

	// Token: 0x060036DF RID: 14047 RVA: 0x0012E154 File Offset: 0x0012C354
	private RigContainer.MuteReason WithReasons(RigContainer.MuteReason reasons, bool muted)
	{
		if (!muted)
		{
			return this.muteReasons & ~reasons;
		}
		return this.muteReasons | reasons;
	}

	// Token: 0x060036E0 RID: 14048 RVA: 0x0012E16C File Offset: 0x0012C36C
	public void SetMuted(RigContainer.MuteReason reasons, bool muted)
	{
		RigContainer.MuteReason muteReason = this.WithReasons(reasons, muted);
		if (muteReason != this.muteReasons)
		{
			this.muteReasons = muteReason;
			this.RefreshVoiceChat();
		}
	}

	// Token: 0x170004DC RID: 1244
	// (get) Token: 0x060036E1 RID: 14049 RVA: 0x0012E198 File Offset: 0x0012C398
	// (set) Token: 0x060036E2 RID: 14050 RVA: 0x0012E1A5 File Offset: 0x0012C3A5
	public NetPlayer Creator
	{
		get
		{
			return this.vrrig.creator;
		}
		set
		{
			if (this.vrrig.isOfflineVRRig || (this.vrrig.creator != null && this.vrrig.creator.InRoom))
			{
				return;
			}
			this.vrrig.creator = value;
		}
	}

	// Token: 0x170004DD RID: 1245
	// (get) Token: 0x060036E3 RID: 14051 RVA: 0x0012E1E0 File Offset: 0x0012C3E0
	public SphereCollider HeadCollider
	{
		get
		{
			return this.headCollider;
		}
	}

	// Token: 0x170004DE RID: 1246
	// (get) Token: 0x060036E4 RID: 14052 RVA: 0x0012E1E8 File Offset: 0x0012C3E8
	public CapsuleCollider BodyCollider
	{
		get
		{
			return this.bodyCollider;
		}
	}

	// Token: 0x170004DF RID: 1247
	// (get) Token: 0x060036E5 RID: 14053 RVA: 0x0012E1F0 File Offset: 0x0012C3F0
	public VRRigEvents RigEvents
	{
		get
		{
			return this.rigEvents;
		}
	}

	// Token: 0x170004E0 RID: 1248
	// (get) Token: 0x060036E6 RID: 14054 RVA: 0x0012E1F8 File Offset: 0x0012C3F8
	// (set) Token: 0x060036E7 RID: 14055 RVA: 0x0012E227 File Offset: 0x0012C427
	public PlayerStatsReadonly PlayerStats
	{
		get
		{
			if (this.Rig.isOfflineVRRig)
			{
				return new PlayerStatsReadonly(GTPlayerStats.Ping, GTPlayerStats.FPS, GTPlayerStats.TargetFPS, GTPlayerStats.SystemPropertiesFlags);
			}
			return this.m_playerStats;
		}
		internal set
		{
			this.m_playerStats = value;
		}
	}

	// Token: 0x060036E8 RID: 14056 RVA: 0x0012E230 File Offset: 0x0012C430
	public bool GetIsPlayerAutoMuted()
	{
		return this.IsMutedFor(RigContainer.MuteReason.Auto);
	}

	// Token: 0x060036E9 RID: 14057 RVA: 0x0012E23C File Offset: 0x0012C43C
	public void UpdateAutomuteLevel(string autoMuteLevel)
	{
		if (autoMuteLevel.Equals("LOW", StringComparison.OrdinalIgnoreCase))
		{
			this.playerChatQuality = 1;
		}
		else if (autoMuteLevel.Equals("HIGH", StringComparison.OrdinalIgnoreCase))
		{
			this.playerChatQuality = 0;
		}
		else if (autoMuteLevel.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
		{
			this.playerChatQuality = 2;
		}
		else
		{
			this.playerChatQuality = 2;
		}
		this.RefreshVoiceChat();
	}

	// Token: 0x060036EA RID: 14058 RVA: 0x0012E29B File Offset: 0x0012C49B
	private void Awake()
	{
		this.loudSpeakerNetworks = new List<LoudSpeakerNetwork>();
	}

	// Token: 0x060036EB RID: 14059 RVA: 0x0012E2A8 File Offset: 0x0012C4A8
	private void Start()
	{
		if (this.Rig.isOfflineVRRig)
		{
			this.vrrig.creator = NetworkSystem.Instance.LocalPlayer;
			RoomSystem.JoinedRoomEvent += new Action(this.OnMultiPlayerStarted);
			RoomSystem.LeftRoomEvent += new Action(this.OnReturnedToSinglePlayer);
		}
		else
		{
			this.rigEvents.enableEvent += this.RigPostEnable;
		}
		this.Rig.rigContainer = this;
	}

	// Token: 0x060036EC RID: 14060 RVA: 0x0012E33D File Offset: 0x0012C53D
	private void RigPostEnable(RigContainer _)
	{
		this.vrrig.UpdateName();
	}

	// Token: 0x060036ED RID: 14061 RVA: 0x0012E34A File Offset: 0x0012C54A
	private void OnMultiPlayerStarted()
	{
		if (this.Rig.isOfflineVRRig)
		{
			this.vrrig.creator = NetworkSystem.Instance.GetLocalPlayer();
		}
	}

	// Token: 0x060036EE RID: 14062 RVA: 0x0012E36E File Offset: 0x0012C56E
	private void OnReturnedToSinglePlayer()
	{
		if (this.Rig.isOfflineVRRig)
		{
			RigContainer.CancelAutomuteRequest();
		}
	}

	// Token: 0x060036EF RID: 14063 RVA: 0x0012E384 File Offset: 0x0012C584
	private void OnDisable()
	{
		this.Initialized = false;
		this.muteReasons = RigContainer.MuteReason.None;
		this.voiceView = null;
		base.gameObject.transform.localPosition = Vector3.zero;
		base.gameObject.transform.localRotation = Quaternion.identity;
		this.vrrig.syncPos = base.gameObject.transform.position;
		this.vrrig.syncRotation = base.gameObject.transform.rotation;
	}

	// Token: 0x060036F0 RID: 14064 RVA: 0x0012E406 File Offset: 0x0012C606
	internal void InitializeNetwork(NetworkView netView, PhotonVoiceView voiceView, VRRigSerializer vrRigSerializer)
	{
		if (!netView || !voiceView)
		{
			return;
		}
		this.InitializeNetwork_Shared(netView, vrRigSerializer);
		this.Voice = voiceView;
		this.vrrig.voiceAudio = voiceView.SpeakerInUse.GetComponent<AudioSource>();
	}

	// Token: 0x060036F1 RID: 14065 RVA: 0x0012E440 File Offset: 0x0012C640
	private void InitializeNetwork_Shared(NetworkView netView, VRRigSerializer vrRigSerializer)
	{
		if (this.vrrig.netView)
		{
			MonkeAgent.instance.SendReport("inappropriate tag data being sent creating multiple vrrigs", this.Creator.UserId, this.Creator.NickName);
			if (this.vrrig.netView.IsMine)
			{
				NetworkSystem.Instance.NetDestroy(this.vrrig.gameObject);
			}
			else
			{
				this.vrrig.netView.gameObject.SetActive(false);
			}
		}
		this.vrrig.netView = netView;
		this.vrrig.rigSerializer = vrRigSerializer;
		this.vrrig.OwningNetPlayer = NetworkSystem.Instance.GetPlayer(NetworkSystem.Instance.GetOwningPlayerID(vrRigSerializer.gameObject));
		this.m_cachedNetViewID = netView.ViewID;
		if (!this.Initialized)
		{
			this.vrrig.NetInitialize();
			if (GorillaGameManager.instance != null && NetworkSystem.Instance.IsMasterClient)
			{
				int owningPlayerID = NetworkSystem.Instance.GetOwningPlayerID(vrRigSerializer.gameObject);
				bool playerTutorialCompletion = NetworkSystem.Instance.GetPlayerTutorialCompletion(owningPlayerID);
				GorillaGameManager.instance.NewVRRig(netView.Owner, netView.ViewID, playerTutorialCompletion);
			}
			if (!this.vrrig.isOfflineVRRig)
			{
				if (this.vrrig.InitializedCosmetics)
				{
					netView.SendRPC("RPC_RequestCosmetics", netView.Owner, Array.Empty<object>());
				}
				base.StartCoroutine(RigContainer.QueueAutomute(this.Creator));
			}
		}
		else if (!this.vrrig.isOfflineVRRig)
		{
			this.RefreshVoiceChat();
		}
		this.Initialized = true;
	}

	// Token: 0x060036F2 RID: 14066 RVA: 0x0012E5CF File Offset: 0x0012C7CF
	private static IEnumerator QueueAutomute(NetPlayer player)
	{
		RigContainer.playersToCheckAutomute.Add(player);
		if (!RigContainer.automuteQueued)
		{
			RigContainer.automuteQueued = true;
			yield return new WaitForSecondsRealtime(1f);
			while (RigContainer.waitingForAutomuteCallback)
			{
				yield return null;
			}
			RigContainer.automuteQueued = false;
			RigContainer.RequestAutomuteSettings();
		}
		yield break;
	}

	// Token: 0x060036F3 RID: 14067 RVA: 0x0012E5E0 File Offset: 0x0012C7E0
	private static void RequestAutomuteSettings()
	{
		if (RigContainer.playersToCheckAutomute.Count == 0)
		{
			return;
		}
		RigContainer.waitingForAutomuteCallback = true;
		RigContainer.playersToCheckAutomute.RemoveAll((NetPlayer player) => player == null);
		RigContainer.requestedAutomutePlayers = new List<NetPlayer>(RigContainer.playersToCheckAutomute);
		RigContainer.playersToCheckAutomute.Clear();
		string[] array = RigContainer.requestedAutomutePlayers.Select((NetPlayer x) => x.UserId).ToArray<string>();
		foreach (NetPlayer netPlayer in RigContainer.requestedAutomutePlayers)
		{
		}
		ExecuteFunctionRequest executeFunctionRequest = new ExecuteFunctionRequest();
		executeFunctionRequest.Entity = new EntityKey
		{
			Id = PlayFabSettings.staticPlayer.EntityId,
			Type = PlayFabSettings.staticPlayer.EntityType
		};
		executeFunctionRequest.FunctionName = "ShouldUserAutomutePlayer";
		executeFunctionRequest.FunctionParameter = string.Join(",", array);
		PlayFabCloudScriptAPI.ExecuteFunction(executeFunctionRequest, delegate(ExecuteFunctionResult result)
		{
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.FunctionResult.ToString());
			if (dictionary == null)
			{
				using (List<NetPlayer>.Enumerator enumerator2 = RigContainer.requestedAutomutePlayers.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						NetPlayer netPlayer2 = enumerator2.Current;
						if (netPlayer2 != null)
						{
							RigContainer.ReceiveAutomuteSettings(netPlayer2, "none");
						}
					}
					goto IL_00A6;
				}
			}
			foreach (NetPlayer netPlayer3 in RigContainer.requestedAutomutePlayers)
			{
				if (netPlayer3 != null)
				{
					string text;
					if (dictionary.TryGetValue(netPlayer3.UserId, out text))
					{
						RigContainer.ReceiveAutomuteSettings(netPlayer3, text);
					}
					else
					{
						RigContainer.ReceiveAutomuteSettings(netPlayer3, "none");
					}
				}
			}
			IL_00A6:
			RigContainer.requestedAutomutePlayers.Clear();
			RigContainer.waitingForAutomuteCallback = false;
		}, delegate(PlayFabError error)
		{
			foreach (NetPlayer netPlayer4 in RigContainer.requestedAutomutePlayers)
			{
				RigContainer.ReceiveAutomuteSettings(netPlayer4, "ERROR");
			}
			RigContainer.requestedAutomutePlayers.Clear();
			RigContainer.waitingForAutomuteCallback = false;
		}, null, null);
	}

	// Token: 0x060036F4 RID: 14068 RVA: 0x0012E744 File Offset: 0x0012C944
	private static void CancelAutomuteRequest()
	{
		RigContainer.playersToCheckAutomute.Clear();
		RigContainer.automuteQueued = false;
		if (RigContainer.requestedAutomutePlayers != null)
		{
			RigContainer.requestedAutomutePlayers.Clear();
		}
		RigContainer.waitingForAutomuteCallback = false;
	}

	// Token: 0x060036F5 RID: 14069 RVA: 0x0012E770 File Offset: 0x0012C970
	private static void ReceiveAutomuteSettings(NetPlayer player, string score)
	{
		RigContainer rigContainer;
		VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
		if (rigContainer != null)
		{
			rigContainer.UpdateAutomuteLevel(score);
		}
	}

	// Token: 0x060036F6 RID: 14070 RVA: 0x0012E79C File Offset: 0x0012C99C
	private void ProcessAutomute()
	{
		int @int = PlayerPrefs.GetInt("autoMute", 1);
		bool flag = !this.hasManualMute && this.playerChatQuality < @int;
		this.muteReasons = this.WithReasons(RigContainer.MuteReason.Auto, flag);
	}

	// Token: 0x060036F7 RID: 14071 RVA: 0x0012E7D8 File Offset: 0x0012C9D8
	public void RefreshVoiceChat()
	{
		if (this.Voice == null)
		{
			return;
		}
		this.ProcessAutomute();
		this.Voice.SpeakerInUse.enabled = !this.IsMuted && GorillaComputer.instance.voiceChatOn == "TRUE";
		this.replacementVoiceSource.mute = this.IsMuted || GorillaComputer.instance.voiceChatOn == "OFF";
	}

	// Token: 0x060036F8 RID: 14072 RVA: 0x0012E857 File Offset: 0x0012CA57
	public void AddLoudSpeakerNetwork(LoudSpeakerNetwork network)
	{
		if (this.loudSpeakerNetworks.Contains(network))
		{
			return;
		}
		this.loudSpeakerNetworks.Add(network);
	}

	// Token: 0x060036F9 RID: 14073 RVA: 0x0012E874 File Offset: 0x0012CA74
	public void RemoveLoudSpeakerNetwork(LoudSpeakerNetwork network)
	{
		this.loudSpeakerNetworks.Remove(network);
	}

	// Token: 0x060036FA RID: 14074 RVA: 0x0012E884 File Offset: 0x0012CA84
	public static void RefreshAllRigVoices()
	{
		RigContainer.staticTempRC = null;
		if (!NetworkSystem.Instance.InRoom || VRRigCache.Instance == null)
		{
			return;
		}
		foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			if (VRRigCache.Instance.TryGetVrrig(netPlayer, out RigContainer.staticTempRC))
			{
				RigContainer.staticTempRC.RefreshVoiceChat();
			}
		}
	}

	// Token: 0x0400475F RID: 18271
	[SerializeField]
	private VRRig vrrig;

	// Token: 0x04004760 RID: 18272
	[SerializeField]
	private VRRigReliableState reliableState;

	// Token: 0x04004761 RID: 18273
	[SerializeField]
	private Transform speakerHead;

	// Token: 0x04004762 RID: 18274
	[SerializeField]
	private AudioSource replacementVoiceSource;

	// Token: 0x04004763 RID: 18275
	private List<LoudSpeakerNetwork> loudSpeakerNetworks;

	// Token: 0x04004764 RID: 18276
	[SerializeField]
	private LCKSocialCameraFollower m_lckCococamFollower;

	// Token: 0x04004765 RID: 18277
	[SerializeField]
	private LCKSocialCameraFollower m_lckTablet;

	// Token: 0x04004766 RID: 18278
	private PhotonVoiceView voiceView;

	// Token: 0x04004767 RID: 18279
	private int m_cachedNetViewID;

	// Token: 0x04004768 RID: 18280
	private RigContainer.MuteReason muteReasons;

	// Token: 0x04004769 RID: 18281
	[SerializeField]
	private SphereCollider headCollider;

	// Token: 0x0400476A RID: 18282
	[SerializeField]
	private CapsuleCollider bodyCollider;

	// Token: 0x0400476B RID: 18283
	[SerializeField]
	private VRRigEvents rigEvents;

	// Token: 0x0400476C RID: 18284
	private PlayerStatsReadonly m_playerStats;

	// Token: 0x0400476D RID: 18285
	public bool hasManualMute;

	// Token: 0x0400476E RID: 18286
	public int playerChatQuality = 2;

	// Token: 0x0400476F RID: 18287
	private static List<NetPlayer> playersToCheckAutomute = new List<NetPlayer>();

	// Token: 0x04004770 RID: 18288
	private static bool automuteQueued = false;

	// Token: 0x04004771 RID: 18289
	private static List<NetPlayer> requestedAutomutePlayers;

	// Token: 0x04004772 RID: 18290
	private static bool waitingForAutomuteCallback = false;

	// Token: 0x04004773 RID: 18291
	private static RigContainer staticTempRC;

	// Token: 0x02000853 RID: 2131
	[Flags]
	public enum MuteReason
	{
		// Token: 0x04004775 RID: 18293
		None = 0,
		// Token: 0x04004776 RID: 18294
		Manual = 1,
		// Token: 0x04004777 RID: 18295
		Auto = 2,
		// Token: 0x04004778 RID: 18296
		Banned = 4,
		// Token: 0x04004779 RID: 18297
		OversizedStream = 8,
		// Token: 0x0400477A RID: 18298
		Room = 16
	}
}
