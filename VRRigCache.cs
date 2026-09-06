using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag;
using GorillaTagScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000859 RID: 2137
public class VRRigCache : MonoBehaviour
{
	// Token: 0x170004E5 RID: 1253
	// (get) Token: 0x06003719 RID: 14105 RVA: 0x0012EC1E File Offset: 0x0012CE1E
	// (set) Token: 0x0600371A RID: 14106 RVA: 0x0012EC25 File Offset: 0x0012CE25
	public static VRRigCache Instance { get; private set; }

	// Token: 0x170004E6 RID: 1254
	// (get) Token: 0x0600371B RID: 14107 RVA: 0x0012EC2D File Offset: 0x0012CE2D
	public Transform NetworkParent
	{
		get
		{
			return this.networkParent;
		}
	}

	// Token: 0x170004E7 RID: 1255
	// (get) Token: 0x0600371C RID: 14108 RVA: 0x0012EC35 File Offset: 0x0012CE35
	public static IReadOnlyList<RigContainer> ActiveRigContainers
	{
		get
		{
			return VRRigCache.m_activeRigContainers;
		}
	}

	// Token: 0x170004E8 RID: 1256
	// (get) Token: 0x0600371D RID: 14109 RVA: 0x0012EC3C File Offset: 0x0012CE3C
	public static IReadOnlyList<VRRig> ActiveRigs
	{
		get
		{
			return VRRigCache.m_activeRigs;
		}
	}

	// Token: 0x170004E9 RID: 1257
	// (get) Token: 0x0600371E RID: 14110 RVA: 0x0012EC43 File Offset: 0x0012CE43
	public static IReadOnlyList<VRRig> AllRigs
	{
		get
		{
			return VRRigCache.m_allRigs;
		}
	}

	// Token: 0x170004EA RID: 1258
	// (get) Token: 0x0600371F RID: 14111 RVA: 0x0012EC4A File Offset: 0x0012CE4A
	public static IReadOnlyList<RigContainer> AllRigContainers
	{
		get
		{
			return VRRigCache.m_allRigContainers;
		}
	}

	// Token: 0x170004EB RID: 1259
	// (get) Token: 0x06003720 RID: 14112 RVA: 0x0012EC51 File Offset: 0x0012CE51
	// (set) Token: 0x06003721 RID: 14113 RVA: 0x0012EC58 File Offset: 0x0012CE58
	public static bool isInitialized { get; private set; }

	// Token: 0x1400005E RID: 94
	// (add) Token: 0x06003722 RID: 14114 RVA: 0x0012EC60 File Offset: 0x0012CE60
	// (remove) Token: 0x06003723 RID: 14115 RVA: 0x0012EC94 File Offset: 0x0012CE94
	public static event Action OnActiveRigsChanged;

	// Token: 0x1400005F RID: 95
	// (add) Token: 0x06003724 RID: 14116 RVA: 0x0012ECC8 File Offset: 0x0012CEC8
	// (remove) Token: 0x06003725 RID: 14117 RVA: 0x0012ECFC File Offset: 0x0012CEFC
	public static event Action OnPostInitialize;

	// Token: 0x14000060 RID: 96
	// (add) Token: 0x06003726 RID: 14118 RVA: 0x0012ED30 File Offset: 0x0012CF30
	// (remove) Token: 0x06003727 RID: 14119 RVA: 0x0012ED64 File Offset: 0x0012CF64
	public static event Action OnPostSpawnRig;

	// Token: 0x14000061 RID: 97
	// (add) Token: 0x06003728 RID: 14120 RVA: 0x0012ED98 File Offset: 0x0012CF98
	// (remove) Token: 0x06003729 RID: 14121 RVA: 0x0012EDCC File Offset: 0x0012CFCC
	public static event Action<RigContainer> OnRigActivated;

	// Token: 0x14000062 RID: 98
	// (add) Token: 0x0600372A RID: 14122 RVA: 0x0012EE00 File Offset: 0x0012D000
	// (remove) Token: 0x0600372B RID: 14123 RVA: 0x0012EE34 File Offset: 0x0012D034
	public static event Action<RigContainer> OnRigDeactivated;

	// Token: 0x14000063 RID: 99
	// (add) Token: 0x0600372C RID: 14124 RVA: 0x0012EE68 File Offset: 0x0012D068
	// (remove) Token: 0x0600372D RID: 14125 RVA: 0x0012EE9C File Offset: 0x0012D09C
	public static event Action<RigContainer> OnRigNameChanged;

	// Token: 0x0600372E RID: 14126 RVA: 0x0012EED0 File Offset: 0x0012D0D0
	private void Awake()
	{
		this.InitializeVRRigCache();
		if (this.localRig != null && this.localRig.Rig != null)
		{
			VRRig rig = this.localRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Combine(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
			if (this.localRig.Rig.bodyRenderer != null)
			{
				this.localRig.Rig.bodyRenderer.SetupAsLocalPlayerBody();
			}
		}
		TickSystemTimer ensureNetworkObjectTimer = this.m_ensureNetworkObjectTimer;
		ensureNetworkObjectTimer.callback = (Action)Delegate.Combine(ensureNetworkObjectTimer.callback, new Action(this.InstantiateNetworkObject));
		NetworkedPlayerColourNotifier.SetLocalRigReference(this.localRig);
	}

	// Token: 0x0600372F RID: 14127 RVA: 0x0012EF88 File Offset: 0x0012D188
	private void OnDestroy()
	{
		if (VRRigCache.Instance == this)
		{
			VRRigCache.Instance = null;
		}
		VRRigCache.isInitialized = false;
		if (this.localRig != null && this.localRig.Rig != null)
		{
			VRRig rig = this.localRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
		}
	}

	// Token: 0x06003730 RID: 14128 RVA: 0x0012EFF4 File Offset: 0x0012D1F4
	public void InitializeVRRigCache()
	{
		if (VRRigCache.isInitialized || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (VRRigCache.Instance != null && VRRigCache.Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		VRRigCache.Instance = this;
		if (this.rigParent == null)
		{
			this.rigParent = base.transform;
		}
		if (this.networkParent == null)
		{
			this.networkParent = base.transform;
		}
		VRRigCache.m_allRigs.Add(this.localRig.Rig);
		VRRigCache.m_allRigContainers.Add(this.localRig);
		VRRigCache.m_activeRigContainers.Add(this.localRig);
		VRRigCache.m_activeRigs.Add(this.localRig.Rig);
		for (int i = 0; i < this.rigAmount; i++)
		{
			RigContainer rigContainer = this.SpawnRig();
			VRRigCache.freeRigs.Enqueue(rigContainer);
			rigContainer.Rig.BuildInitialize();
			rigContainer.Rig.transform.parent = null;
		}
		VRRigCache.isInitialized = true;
		Action onPostInitialize = VRRigCache.OnPostInitialize;
		if (onPostInitialize != null)
		{
			onPostInitialize();
		}
		Action onPostSpawnRig = VRRigCache.OnPostSpawnRig;
		if (onPostSpawnRig == null)
		{
			return;
		}
		onPostSpawnRig();
	}

	// Token: 0x06003731 RID: 14129 RVA: 0x0012F11C File Offset: 0x0012D31C
	private RigContainer SpawnRig()
	{
		if (this.rigTemplate.activeSelf)
		{
			this.rigTemplate.SetActive(false);
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.rigTemplate, this.rigParent, false);
		RigContainer rigContainer = ((gameObject != null) ? gameObject.GetComponent<RigContainer>() : null);
		VRRig vrrig = null;
		if (rigContainer.IsNotNull())
		{
			vrrig = rigContainer.Rig;
		}
		VRRigCache.m_allRigs.Add(vrrig);
		VRRigCache.m_allRigContainers.Add(rigContainer);
		return rigContainer;
	}

	// Token: 0x06003732 RID: 14130 RVA: 0x0012F189 File Offset: 0x0012D389
	internal bool TryGetVrrig(Player targetPlayer, out RigContainer playerRig)
	{
		return this.TryGetVrrig(NetworkSystem.Instance.GetPlayer(targetPlayer.ActorNumber), out playerRig);
	}

	// Token: 0x06003733 RID: 14131 RVA: 0x0012F1A2 File Offset: 0x0012D3A2
	internal bool TryGetVrrig(int targetPlayerId, out RigContainer playerRig)
	{
		return this.TryGetVrrig(NetworkSystem.Instance.GetPlayer(targetPlayerId), out playerRig);
	}

	// Token: 0x06003734 RID: 14132 RVA: 0x0012F1B8 File Offset: 0x0012D3B8
	internal bool TryGetVrrig(NetPlayer targetPlayer, out RigContainer playerRig)
	{
		playerRig = null;
		if (ApplicationQuittingState.IsQuitting)
		{
			return false;
		}
		if (targetPlayer == null || targetPlayer.IsNull)
		{
			GTDev.LogError<string>("[GT/VRRigCache]  ERROR!!!  TryGetVrrig: Supplied targetPlayer cannot be null!", null);
			return false;
		}
		if (targetPlayer.IsLocal)
		{
			playerRig = this.localRig;
			return true;
		}
		if (!targetPlayer.InRoom)
		{
			return false;
		}
		if (!VRRigCache.rigsInUse.TryGetValue(targetPlayer, out playerRig))
		{
			if (VRRigCache.freeRigs.Count <= 0)
			{
				return false;
			}
			playerRig = VRRigCache.freeRigs.Dequeue();
			playerRig.Creator = targetPlayer;
			VRRigCache.rigsInUse.Add(targetPlayer, playerRig);
			VRRigCache.m_activeRigContainers.Add(playerRig);
			VRRigCache.m_activeRigs.Add(playerRig.Rig);
			VRRig rig = playerRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
			VRRig rig2 = playerRig.Rig;
			rig2.OnNameChanged = (Action<RigContainer>)Delegate.Combine(rig2.OnNameChanged, VRRigCache.OnRigNameChanged);
			playerRig.gameObject.SetActive(true);
			playerRig.RigEvents.SendPostEnableEvent();
			if (!VRRigCache._isBatchingRigActivations)
			{
				GamePlayer.UpdateStaticLookupCaches();
			}
			Action<RigContainer> onRigActivated = VRRigCache.OnRigActivated;
			if (onRigActivated != null)
			{
				onRigActivated(playerRig);
			}
			if (!VRRigCache._isBatchingRigActivations)
			{
				Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
				if (onActiveRigsChanged != null)
				{
					onActiveRigsChanged();
				}
			}
		}
		return true;
	}

	// Token: 0x06003735 RID: 14133 RVA: 0x0012F2FC File Offset: 0x0012D4FC
	public void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		if (newPlayer.ActorNumber == -1)
		{
			Debug.LogError("LocalPlayer returned, vrrig no correctly initialised");
		}
		RigContainer rigContainer;
		this.TryGetVrrig(newPlayer, out rigContainer);
	}

	// Token: 0x06003736 RID: 14134 RVA: 0x0012F328 File Offset: 0x0012D528
	public void OnJoinedRoom()
	{
		VRRigCache._isBatchingRigActivations = true;
		foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			RigContainer rigContainer;
			this.TryGetVrrig(netPlayer, out rigContainer);
		}
		VRRigCache._isBatchingRigActivations = false;
		this.m_ensureNetworkObjectTimer.Start();
		GamePlayer.UpdateStaticLookupCaches();
		Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
		if (onActiveRigsChanged == null)
		{
			return;
		}
		onActiveRigsChanged();
	}

	// Token: 0x06003737 RID: 14135 RVA: 0x0012F388 File Offset: 0x0012D588
	public void OnPlayerLeftRoom(NetPlayer leavingPlayer)
	{
		if (leavingPlayer.IsNull)
		{
			Debug.LogError("Leaving players NetPlayer is Null");
			this.CheckForMissingPlayer();
		}
		RigContainer rigContainer;
		if (!VRRigCache.rigsInUse.TryGetValue(leavingPlayer, out rigContainer))
		{
			this.LogError("failed to find player's vrrig who left " + leavingPlayer.UserId);
			return;
		}
		rigContainer.gameObject.Disable();
		VRRig rig = rigContainer.Rig;
		rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
		VRRigCache.freeRigs.Enqueue(rigContainer);
		VRRigCache.rigsInUse.Remove(leavingPlayer);
		VRRigCache.m_activeRigContainers.Remove(rigContainer);
		VRRigCache.m_activeRigs.Remove(rigContainer.Rig);
		GamePlayer.UpdateStaticLookupCaches();
		Action<RigContainer> onRigDeactivated = VRRigCache.OnRigDeactivated;
		if (onRigDeactivated != null)
		{
			onRigDeactivated(rigContainer);
		}
		Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
		if (onActiveRigsChanged == null)
		{
			return;
		}
		onActiveRigsChanged();
	}

	// Token: 0x06003738 RID: 14136 RVA: 0x0012F45C File Offset: 0x0012D65C
	private void CheckForMissingPlayer()
	{
		foreach (KeyValuePair<NetPlayer, RigContainer> keyValuePair in VRRigCache.rigsInUse)
		{
			if (keyValuePair.Key == null || keyValuePair.Value == null)
			{
				Debug.LogError("Somehow null reference in rigsInUse");
			}
			else if (!keyValuePair.Key.InRoom)
			{
				keyValuePair.Value.gameObject.Disable();
				VRRig rig = keyValuePair.Value.Rig;
				rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
				VRRigCache.freeRigs.Enqueue(keyValuePair.Value);
				VRRigCache.rigsInUse.Remove(keyValuePair.Key);
				VRRigCache.m_activeRigContainers.Remove(keyValuePair.Value);
				VRRigCache.m_activeRigs.Remove(keyValuePair.Value.Rig);
				GamePlayer.UpdateStaticLookupCaches();
				Action<RigContainer> onRigDeactivated = VRRigCache.OnRigDeactivated;
				if (onRigDeactivated != null)
				{
					onRigDeactivated(keyValuePair.Value);
				}
				Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
				if (onActiveRigsChanged != null)
				{
					onActiveRigsChanged();
				}
			}
		}
	}

	// Token: 0x06003739 RID: 14137 RVA: 0x0012F5A0 File Offset: 0x0012D7A0
	public void OnLeftRoom()
	{
		this.m_ensureNetworkObjectTimer.Stop();
		Dictionary<NetPlayer, RigContainer> dictionary;
		using (DictionaryPool<NetPlayer, RigContainer>.Get(out dictionary))
		{
			dictionary.EnsureCapacity(VRRigCache.rigsInUse.Count);
			dictionary.Clear();
			foreach (KeyValuePair<NetPlayer, RigContainer> keyValuePair in VRRigCache.rigsInUse)
			{
				NetPlayer netPlayer;
				RigContainer rigContainer;
				keyValuePair.Deconstruct(out netPlayer, out rigContainer);
				NetPlayer netPlayer2 = netPlayer;
				RigContainer rigContainer2 = rigContainer;
				dictionary.Add(netPlayer2, rigContainer2);
			}
			foreach (KeyValuePair<NetPlayer, RigContainer> keyValuePair in dictionary)
			{
				NetPlayer netPlayer;
				RigContainer rigContainer;
				keyValuePair.Deconstruct(out netPlayer, out rigContainer);
				NetPlayer netPlayer3 = netPlayer;
				RigContainer rigContainer3 = rigContainer;
				if (!(rigContainer3 == null))
				{
					VRRig rig = VRRigCache.rigsInUse[netPlayer3].Rig;
					VRRig rig2 = rigContainer3.Rig;
					rig2.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig2.OnNameChanged, VRRigCache.OnRigNameChanged);
					rigContainer3.gameObject.Disable();
					VRRigCache.rigsInUse.Remove(netPlayer3);
					VRRigCache.freeRigs.Enqueue(rigContainer3);
				}
			}
			VRRigCache.m_activeRigContainers.Clear();
			VRRigCache.m_activeRigContainers.Add(this.localRig);
			VRRigCache.m_activeRigs.Clear();
			VRRigCache.m_activeRigs.Add(this.localRig.Rig);
			GamePlayer.UpdateStaticLookupCaches();
			if (VRRigCache.OnRigDeactivated != null)
			{
				foreach (RigContainer rigContainer4 in dictionary.Values)
				{
					VRRigCache.OnRigDeactivated(rigContainer4);
				}
			}
			Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
			if (onActiveRigsChanged != null)
			{
				onActiveRigsChanged();
			}
		}
	}

	// Token: 0x0600373A RID: 14138 RVA: 0x0012F7CC File Offset: 0x0012D9CC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal VRRig[] GetAllRigs()
	{
		VRRig[] array = new VRRig[VRRigCache.rigsInUse.Count + VRRigCache.freeRigs.Count];
		int num = 0;
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			array[num] = rigContainer.Rig;
			num++;
		}
		foreach (RigContainer rigContainer2 in VRRigCache.freeRigs)
		{
			array[num] = rigContainer2.Rig;
			num++;
		}
		return array;
	}

	// Token: 0x0600373B RID: 14139 RVA: 0x0012F894 File Offset: 0x0012DA94
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void GetAllUsedRigs(List<VRRig> rigs)
	{
		if (rigs == null)
		{
			return;
		}
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			rigs.Add(rigContainer.Rig);
		}
	}

	// Token: 0x0600373C RID: 14140 RVA: 0x0012F8F4 File Offset: 0x0012DAF4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void GetActiveRigs(List<VRRig> rigsListToUpdate)
	{
		if (rigsListToUpdate == null)
		{
			return;
		}
		rigsListToUpdate.Clear();
		if (!VRRigCache.isInitialized)
		{
			return;
		}
		rigsListToUpdate.Add(VRRigCache.Instance.localRig.Rig);
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			rigsListToUpdate.Add(rigContainer.Rig);
		}
	}

	// Token: 0x0600373D RID: 14141 RVA: 0x0012F978 File Offset: 0x0012DB78
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void ApplyToAllRigs(Action<VRRig> action)
	{
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			action(rigContainer.Rig);
		}
		foreach (RigContainer rigContainer2 in VRRigCache.freeRigs)
		{
			action(rigContainer2.Rig);
		}
	}

	// Token: 0x0600373E RID: 14142 RVA: 0x0012FA1C File Offset: 0x0012DC1C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void ApplyToAllActiveRigs(Action<VRRig> action)
	{
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			action(rigContainer.Rig);
		}
	}

	// Token: 0x0600373F RID: 14143 RVA: 0x0012FA78 File Offset: 0x0012DC78
	internal int GetAllRigsHash()
	{
		int num = 0;
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			num += rigContainer.GetInstanceID();
		}
		foreach (RigContainer rigContainer2 in VRRigCache.freeRigs)
		{
			num += rigContainer2.GetInstanceID();
		}
		return num;
	}

	// Token: 0x06003740 RID: 14144 RVA: 0x0012FB1C File Offset: 0x0012DD1C
	internal void InstantiateNetworkObject()
	{
		if (this.localRig.netView.IsNotNull() || !NetworkSystem.Instance.InRoom)
		{
			return;
		}
		PrefabType prefabType;
		if (!VRRigCache.Instance.GetComponent<PhotonPrefabPool>().networkPrefabs.TryGetValue("Player Network Controller", out prefabType) || prefabType.prefab == null)
		{
			Debug.LogError("OnJoinedRoom: Unable to find player prefab to spawn");
			return;
		}
		GameObject gameObject = GTPlayer.Instance.gameObject;
		Color playerColor = this.localRig.Rig.playerColor;
		VRRigCache.rigRGBData[0] = playerColor.r;
		VRRigCache.rigRGBData[1] = playerColor.g;
		VRRigCache.rigRGBData[2] = playerColor.b;
		NetworkSystem.Instance.NetInstantiate(prefabType.prefab, gameObject.transform.position, gameObject.transform.rotation, false, 0, VRRigCache.rigRGBData, null);
	}

	// Token: 0x06003741 RID: 14145 RVA: 0x0012FBFF File Offset: 0x0012DDFF
	internal void OnVrrigSerializerSuccesfullySpawned()
	{
		GamePlayer.UpdateStaticLookupCaches();
		Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
		if (onActiveRigsChanged == null)
		{
			return;
		}
		onActiveRigsChanged();
	}

	// Token: 0x06003742 RID: 14146 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void LogInfo(string log)
	{
	}

	// Token: 0x06003743 RID: 14147 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void LogWarning(string log)
	{
	}

	// Token: 0x06003744 RID: 14148 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void LogError(string log)
	{
	}

	// Token: 0x04004787 RID: 18311
	private const string preLog = "[GT/VRRigCache] ";

	// Token: 0x04004788 RID: 18312
	private const string preErr = "[GT/VRRigCache]  ERROR!!!  ";

	// Token: 0x04004789 RID: 18313
	private const string preErrBeta = "[GT/VRRigCache]  ERROR!!!  (beta only log) ";

	// Token: 0x0400478A RID: 18314
	private const string preErrEd = "[GT/VRRigCache]  ERROR!!!  (editor only log) ";

	// Token: 0x0400478C RID: 18316
	public RigContainer localRig;

	// Token: 0x0400478D RID: 18317
	[SerializeField]
	private Transform rigParent;

	// Token: 0x0400478E RID: 18318
	[SerializeField]
	private Transform networkParent;

	// Token: 0x0400478F RID: 18319
	[SerializeField]
	private GameObject rigTemplate;

	// Token: 0x04004790 RID: 18320
	private int rigAmount = 19;

	// Token: 0x04004791 RID: 18321
	[SerializeField]
	private TickSystemTimer m_ensureNetworkObjectTimer = new TickSystemTimer(0.1f);

	// Token: 0x04004792 RID: 18322
	[OnEnterPlay_Clear]
	private static Queue<RigContainer> freeRigs = new Queue<RigContainer>(19);

	// Token: 0x04004793 RID: 18323
	[OnEnterPlay_Clear]
	private static Dictionary<NetPlayer, RigContainer> rigsInUse = new Dictionary<NetPlayer, RigContainer>(19);

	// Token: 0x04004794 RID: 18324
	[OnEnterPlay_Clear]
	private static readonly List<RigContainer> m_activeRigContainers = new List<RigContainer>(20);

	// Token: 0x04004795 RID: 18325
	[OnEnterPlay_Clear]
	private static readonly List<VRRig> m_activeRigs = new List<VRRig>(20);

	// Token: 0x04004796 RID: 18326
	[OnEnterPlay_Clear]
	private static readonly List<VRRig> m_allRigs = new List<VRRig>(21);

	// Token: 0x04004797 RID: 18327
	[OnEnterPlay_Clear]
	private static readonly List<RigContainer> m_allRigContainers = new List<RigContainer>(21);

	// Token: 0x04004798 RID: 18328
	[OnEnterPlay_Set(false)]
	private static bool _isBatchingRigActivations;

	// Token: 0x040047A0 RID: 18336
	private static object[] rigRGBData = new object[] { 0f, 0f, 0f };
}
