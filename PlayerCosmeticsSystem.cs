using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using GorillaTagScripts;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

// Token: 0x02000D33 RID: 3379
internal class PlayerCosmeticsSystem : MonoBehaviour, ITickSystemPre
{
	// Token: 0x170007E9 RID: 2025
	// (get) Token: 0x0600538D RID: 21389 RVA: 0x001B817E File Offset: 0x001B637E
	// (set) Token: 0x0600538E RID: 21390 RVA: 0x001B8186 File Offset: 0x001B6386
	bool ITickSystemPre.PreTickRunning { get; set; }

	// Token: 0x0600538F RID: 21391 RVA: 0x001B8190 File Offset: 0x001B6390
	private void Awake()
	{
		if (PlayerCosmeticsSystem.instance == null)
		{
			PlayerCosmeticsSystem.instance = this;
			base.transform.SetParent(null, true);
			Object.DontDestroyOnLoad(this);
			this.inventory = new List<string>();
			this.inventory.Add("InventoryDict");
			this.inventory.Add(PlayerCosmeticsSystem.subscriptionKey);
			NetworkSystem.Instance.OnRaiseEvent += this.OnNetEvent;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06005390 RID: 21392 RVA: 0x001B8210 File Offset: 0x001B6410
	private void Start()
	{
		this.playerLookUpCooldown = Mathf.Max(this.playerLookUpCooldown, 3f);
		PlayFabTitleDataCache.Instance.GetTitleData("EnableTempCosmeticUnlocks", delegate(string data)
		{
			bool flag;
			if (bool.TryParse(data, out flag))
			{
				PlayerCosmeticsSystem.TempUnlocksEnabled = flag;
				return;
			}
			Debug.LogError("PlayerCosmeticsSystem: error parsing EnableTempCosmeticUnlocks data");
		}, delegate(PlayFabError error)
		{
		}, false);
	}

	// Token: 0x06005391 RID: 21393 RVA: 0x001B8281 File Offset: 0x001B6481
	private void OnDestroy()
	{
		if (PlayerCosmeticsSystem.instance == this)
		{
			PlayerCosmeticsSystem.instance = null;
		}
	}

	// Token: 0x06005392 RID: 21394 RVA: 0x001B8296 File Offset: 0x001B6496
	private void LookUpPlayerCosmetics(bool wait = false)
	{
		if (!this.isLookingUp)
		{
			TickSystem<object>.AddPreTickCallback(this);
			this.startSearchingTime = (wait ? Time.realtimeSinceStartup : float.MinValue);
			this.isLookingUp = true;
		}
	}

	// Token: 0x06005393 RID: 21395 RVA: 0x001B82C4 File Offset: 0x001B64C4
	public void PreTick()
	{
		if (PlayerCosmeticsSystem.playersToLookUp.Count < 1)
		{
			TickSystem<object>.RemovePreTickCallback(this);
			this.startSearchingTime = float.MinValue;
			this.isLookingUp = false;
			return;
		}
		if (this.startSearchingTime + this.playerLookUpCooldown > Time.realtimeSinceStartup)
		{
			return;
		}
		this.NewCosmeticsPath();
	}

	// Token: 0x06005394 RID: 21396 RVA: 0x001B8312 File Offset: 0x001B6512
	private void NewCosmeticsPath()
	{
		if (this.isLookingUpNew)
		{
			return;
		}
		base.StartCoroutine(this.NewCosmeticsPathCoroutine());
	}

	// Token: 0x06005395 RID: 21397 RVA: 0x001B832A File Offset: 0x001B652A
	private IEnumerator NewCosmeticsPathCoroutine()
	{
		this.isLookingUpNew = true;
		NetPlayer player = null;
		PlayerCosmeticsSystem.playerIDsList.Clear();
		PlayerCosmeticsSystem.playerActorNumberList.Clear();
		while (PlayerCosmeticsSystem.playersToLookUp.Count > 0)
		{
			player = PlayerCosmeticsSystem.playersToLookUp.Dequeue();
			string text = player.ActorNumber.ToString();
			if (player.InRoom() && !PlayerCosmeticsSystem.playerIDsList.Contains(text))
			{
				PlayerCosmeticsSystem.playerIDsList.Add(player.UserId);
				PlayerCosmeticsSystem.playerActorNumberList.Add(player.ActorNumber);
			}
		}
		int num;
		for (int i = 0; i < PlayerCosmeticsSystem.playerIDsList.Count; i = num + 1)
		{
			int j = i;
			global::PlayFab.ClientModels.GetSharedGroupDataRequest getSharedGroupDataRequest = new global::PlayFab.ClientModels.GetSharedGroupDataRequest();
			getSharedGroupDataRequest.Keys = this.inventory;
			getSharedGroupDataRequest.SharedGroupId = PlayerCosmeticsSystem.playerIDsList[j] + "Inventory";
			PlayFabClientAPI.GetSharedGroupData(getSharedGroupDataRequest, delegate(GetSharedGroupDataResult result)
			{
				if (!NetworkSystem.Instance.InRoom)
				{
					PlayerCosmeticsSystem.playersWaiting.Clear();
					return;
				}
				bool flag = false;
				foreach (KeyValuePair<string, global::PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
				{
					if (keyValuePair.Key == "InventoryDict")
					{
						if (Utils.PlayerInRoom(PlayerCosmeticsSystem.playerActorNumberList[j]))
						{
							this.tempCosmetics = keyValuePair.Value.Value;
							IUserCosmeticsCallback userCosmeticsCallback;
							if (!PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(PlayerCosmeticsSystem.playerActorNumberList[j], out userCosmeticsCallback))
							{
								PlayerCosmeticsSystem.userCosmeticsWaiting[PlayerCosmeticsSystem.playerActorNumberList[j]] = this.tempCosmetics;
							}
							else
							{
								userCosmeticsCallback.PendingUpdate = false;
								if (!userCosmeticsCallback.OnGetUserCosmetics(this.tempCosmetics))
								{
									PlayerCosmeticsSystem.playersToLookUp.Enqueue(player);
									userCosmeticsCallback.PendingUpdate = true;
								}
							}
						}
					}
					else if (keyValuePair.Key == PlayerCosmeticsSystem.subscriptionKey)
					{
						flag = true;
						NetPlayer netPlayer = null;
						foreach (NetPlayer netPlayer2 in NetworkSystem.Instance.AllNetPlayers)
						{
							if (netPlayer2.ActorNumber == PlayerCosmeticsSystem.playerActorNumberList[j])
							{
								netPlayer = netPlayer2;
								break;
							}
						}
						if (netPlayer != null)
						{
							bool flag2 = false;
							int num2 = 0;
							if (!string.IsNullOrEmpty(keyValuePair.Value.Value))
							{
								try
								{
									PlayerCosmeticsSystem.SharedSubscriptionData sharedSubscriptionData = JsonConvert.DeserializeObject<PlayerCosmeticsSystem.SharedSubscriptionData>(keyValuePair.Value.Value);
									flag2 = DateTimeOffset.UtcNow < sharedSubscriptionData.ExpirationTime;
									num2 = sharedSubscriptionData.TotalLifetimeSeconds / 86400;
								}
								catch (Exception ex)
								{
									Debug.LogError("Failed to deserialize subscription data for " + netPlayer.NickName + ": " + ex.Message);
								}
							}
							SubscriptionManager.UpdatePlayerSubscriptionData(netPlayer, flag2, num2);
						}
					}
				}
				if (!flag)
				{
					NetPlayer netPlayer3 = null;
					foreach (NetPlayer netPlayer4 in NetworkSystem.Instance.AllNetPlayers)
					{
						if (netPlayer4.ActorNumber == PlayerCosmeticsSystem.playerActorNumberList[j])
						{
							netPlayer3 = netPlayer4;
							break;
						}
					}
					if (netPlayer3 != null)
					{
						SubscriptionManager.UpdatePlayerSubscriptionData(netPlayer3, false, 0);
					}
				}
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
				}
			}, null, null);
			yield return new WaitForSecondsRealtime(this.getSharedGroupDataCooldown);
			num = i;
		}
		this.isLookingUpNew = false;
		yield break;
	}

	// Token: 0x06005396 RID: 21398 RVA: 0x001B8339 File Offset: 0x001B6539
	private void OnNetEvent(byte code, object data, int source)
	{
		if (code != 199 || source < 0)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(source);
		MonkeAgent.IncrementRPCCall(new PhotonMessageInfoWrapped(source, NetworkSystem.Instance.ServerTimestamp), "UpdatePlayerCosmetics");
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(player);
	}

	// Token: 0x170007EA RID: 2026
	// (get) Token: 0x06005397 RID: 21399 RVA: 0x001B8372 File Offset: 0x001B6572
	private static bool nullInstance
	{
		get
		{
			return PlayerCosmeticsSystem.instance == null || !PlayerCosmeticsSystem.instance;
		}
	}

	// Token: 0x170007EB RID: 2027
	// (get) Token: 0x06005398 RID: 21400 RVA: 0x001B838A File Offset: 0x001B658A
	// (set) Token: 0x06005399 RID: 21401 RVA: 0x001B8391 File Offset: 0x001B6591
	public static bool TempUnlocksEnabled { get; private set; } = false;

	// Token: 0x170007EC RID: 2028
	// (get) Token: 0x0600539A RID: 21402 RVA: 0x001B8399 File Offset: 0x001B6599
	// (set) Token: 0x0600539B RID: 21403 RVA: 0x001B83A0 File Offset: 0x001B65A0
	public static string[] TempUnlockCosmeticString { get; private set; } = Array.Empty<string>();

	// Token: 0x0600539C RID: 21404 RVA: 0x001B83A8 File Offset: 0x001B65A8
	public static void RegisterCosmeticCallback(int playerID, IUserCosmeticsCallback callback)
	{
		PlayerCosmeticsSystem.userCosmeticCallback[playerID] = callback;
		string text;
		if (PlayerCosmeticsSystem.userCosmeticsWaiting.TryGetValue(playerID, out text))
		{
			callback.PendingUpdate = false;
			callback.OnGetUserCosmetics(text);
			PlayerCosmeticsSystem.userCosmeticsWaiting.Remove(playerID);
		}
	}

	// Token: 0x0600539D RID: 21405 RVA: 0x001B83EB File Offset: 0x001B65EB
	public static void RemoveCosmeticCallback(int playerID)
	{
		if (PlayerCosmeticsSystem.userCosmeticCallback.ContainsKey(playerID))
		{
			PlayerCosmeticsSystem.userCosmeticCallback.Remove(playerID);
		}
	}

	// Token: 0x0600539E RID: 21406 RVA: 0x001B8408 File Offset: 0x001B6608
	public static void UpdatePlayerCosmetics(NetPlayer player)
	{
		if (player == null || player.IsLocal)
		{
			return;
		}
		PlayerCosmeticsSystem.playersToLookUp.Enqueue(player);
		IUserCosmeticsCallback userCosmeticsCallback;
		if (PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(player.ActorNumber, out userCosmeticsCallback))
		{
			userCosmeticsCallback.PendingUpdate = true;
		}
		if (!PlayerCosmeticsSystem.nullInstance)
		{
			PlayerCosmeticsSystem.instance.LookUpPlayerCosmetics(true);
		}
	}

	// Token: 0x0600539F RID: 21407 RVA: 0x001B845C File Offset: 0x001B665C
	public static void UpdatePlayerCosmetics(List<NetPlayer> players)
	{
		foreach (NetPlayer netPlayer in players)
		{
			if (netPlayer != null && !netPlayer.IsLocal)
			{
				PlayerCosmeticsSystem.playersToLookUp.Enqueue(netPlayer);
				IUserCosmeticsCallback userCosmeticsCallback;
				if (PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(netPlayer.ActorNumber, out userCosmeticsCallback))
				{
					userCosmeticsCallback.PendingUpdate = true;
				}
			}
		}
		if (!PlayerCosmeticsSystem.nullInstance)
		{
			PlayerCosmeticsSystem.instance.LookUpPlayerCosmetics(false);
		}
	}

	// Token: 0x060053A0 RID: 21408 RVA: 0x001B84E8 File Offset: 0x001B66E8
	public static void SetRigTryOn(bool inTryon, RigContainer rigRefg)
	{
		VRRig rig = rigRefg.Rig;
		rig.inTryOnRoom = inTryon;
		if (inTryon)
		{
			if (PlayerCosmeticsSystem.sinceLastTryOnEvent.HasElapsed(0.5f, true))
			{
				GorillaTelemetry.PostShopEvent(rig, GTShopEventType.item_try_on, rig.tryOnSet.items);
			}
			if (rig.isOfflineVRRig)
			{
				CosmeticsController.ClearTryOnCollectable();
			}
		}
		else if (rig.isOfflineVRRig)
		{
			rig.tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
			CosmeticsController.ClearTryOnCollectable();
			CosmeticsController.instance.ClearCheckout(false);
			CosmeticsController.instance.UpdateShoppingCart();
			CosmeticsController.instance.UpdateWornCosmetics(true);
			rig.myBodyDockPositions.RefreshTransferrableItems();
			return;
		}
		rig.LocalUpdateCosmeticsWithTryon(rig.cosmeticSet, rig.tryOnSet, false);
		rig.myBodyDockPositions.RefreshTransferrableItems();
	}

	// Token: 0x060053A1 RID: 21409 RVA: 0x001B85B0 File Offset: 0x001B67B0
	public static void SetRigTemporarySpace(bool enteringSpace, RigContainer rigRef, IReadOnlyList<string> cosmeticIds)
	{
		rigRef.Rig.inTempCosmSpace = enteringSpace;
		if (enteringSpace)
		{
			CosmeticsController.CosmeticSet currentWornSet = CosmeticsController.instance.currentWornSet;
			CosmeticsController.instance.tempUnlockedSet.CopyItemsIntoEmpty(currentWornSet);
			PlayerCosmeticsSystem.UnlockTemporaryCosmeticsForPlayer(rigRef, cosmeticIds);
			return;
		}
		PlayerCosmeticsSystem.LockTemporaryCosmeticsForPlayer(rigRef, cosmeticIds);
	}

	// Token: 0x060053A2 RID: 21410 RVA: 0x001B85FA File Offset: 0x001B67FA
	public static void UnlockTemporaryCosmeticsForPlayer(RigContainer rigRef)
	{
		PlayerCosmeticsSystem.UnlockTemporaryCosmeticsForPlayer(rigRef, PlayerCosmeticsSystem.TempUnlockCosmeticString);
	}

	// Token: 0x060053A3 RID: 21411 RVA: 0x001B8608 File Offset: 0x001B6808
	public static void UnlockTemporaryCosmeticsForPlayer(RigContainer rigRef, IReadOnlyList<string> cosmeticIds)
	{
		if (cosmeticIds == null)
		{
			Debug.LogError("PlayerCosmeticsSystem failed to unlock temporary cosmetics, cosmetic IDs are null");
			return;
		}
		VRRig rig = rigRef.Rig;
		foreach (string text in cosmeticIds)
		{
			if (rig.TemporaryCosmetics.Add(text) && rig.isOfflineVRRig && !rig.HasCosmetic(text))
			{
				CosmeticsController.instance.AddTempUnlockToWardrobe(text);
			}
		}
		Action onCosmeticsUpdated = CosmeticsController.instance.OnCosmeticsUpdated;
		if (onCosmeticsUpdated != null)
		{
			onCosmeticsUpdated();
		}
		if (rig.isOfflineVRRig)
		{
			CosmeticsController.instance.UpdateWornCosmetics(true);
			return;
		}
		rig.RefreshCosmetics();
	}

	// Token: 0x060053A4 RID: 21412 RVA: 0x001B86C0 File Offset: 0x001B68C0
	public static void LockTemporaryCosmeticsForPlayer(RigContainer rigRef)
	{
		PlayerCosmeticsSystem.LockTemporaryCosmeticsForPlayer(rigRef, PlayerCosmeticsSystem.TempUnlockCosmeticString);
	}

	// Token: 0x060053A5 RID: 21413 RVA: 0x001B86D0 File Offset: 0x001B68D0
	public static void LockTemporaryCosmeticsForPlayer(RigContainer rigRef, IReadOnlyList<string> cosmeticIds)
	{
		if (cosmeticIds == null)
		{
			Debug.LogError("PlayerCosmeticsSystem failed to unlock temporary cosmetics, cosmetic IDs are null");
			return;
		}
		VRRig rig = rigRef.Rig;
		foreach (string text in cosmeticIds)
		{
			if (rig.TemporaryCosmetics.Remove(text) && rig.isOfflineVRRig && !rig.HasCosmetic(text))
			{
				CosmeticsController.instance.RemoveTempUnlockFromWardrobe(text);
			}
		}
		Action onCosmeticsUpdated = CosmeticsController.instance.OnCosmeticsUpdated;
		if (onCosmeticsUpdated != null)
		{
			onCosmeticsUpdated();
		}
		if (rig.isOfflineVRRig)
		{
			CosmeticsController.instance.UpdateWornCosmetics(true);
			return;
		}
		rig.RefreshCosmetics();
	}

	// Token: 0x060053A6 RID: 21414 RVA: 0x001B8788 File Offset: 0x001B6988
	internal static void UnlockTemporaryCosmeticsGlobal(IReadOnlyList<string> cosmeticIds)
	{
		int count = cosmeticIds.Count;
		for (int i = 0; i < count; i++)
		{
			PlayerCosmeticsSystem.UnlockTemporaryCosmeticGlobal(cosmeticIds[i]);
		}
	}

	// Token: 0x060053A7 RID: 21415 RVA: 0x001B87B4 File Offset: 0x001B69B4
	internal static void UnlockTemporaryCosmeticGlobal(string cosmeticId)
	{
		int num = 0;
		if (PlayerCosmeticsSystem.k_tempUnlockedCosmetics.ContainsKey(cosmeticId))
		{
			num = PlayerCosmeticsSystem.k_tempUnlockedCosmetics[cosmeticId];
		}
		num++;
		PlayerCosmeticsSystem.k_tempUnlockedCosmetics[cosmeticId] = num;
	}

	// Token: 0x060053A8 RID: 21416 RVA: 0x001B87EC File Offset: 0x001B69EC
	internal static void LockTemporaryCosmeticsGlobal(IReadOnlyList<string> cosmeticIds)
	{
		int count = cosmeticIds.Count;
		for (int i = 0; i < count; i++)
		{
			PlayerCosmeticsSystem.LockTemporaryCosmeticGlobal(cosmeticIds[i]);
		}
	}

	// Token: 0x060053A9 RID: 21417 RVA: 0x001B8818 File Offset: 0x001B6A18
	internal static void LockTemporaryCosmeticGlobal(string cosmeticId)
	{
		if (!PlayerCosmeticsSystem.k_tempUnlockedCosmetics.ContainsKey(cosmeticId))
		{
			Debug.LogError("PlayerCosmeticsSystem: Unable to lock cosmetic, ID:-" + cosmeticId + " not found!");
			return;
		}
		int num = PlayerCosmeticsSystem.k_tempUnlockedCosmetics[cosmeticId];
		num--;
		PlayerCosmeticsSystem.k_tempUnlockedCosmetics[cosmeticId] = num;
	}

	// Token: 0x060053AA RID: 21418 RVA: 0x001B8864 File Offset: 0x001B6A64
	public static bool IsTemporaryCosmeticAllowed(VRRig rigRef, string cosmeticId)
	{
		int num;
		return rigRef.TemporaryCosmetics.Contains(cosmeticId) || (PlayerCosmeticsSystem.k_tempUnlockedCosmetics.TryGetValue(cosmeticId, out num) && num > 0);
	}

	// Token: 0x060053AB RID: 21419 RVA: 0x001B8898 File Offset: 0x001B6A98
	public static bool LocalIsTemporaryCosmetic(string cosmeticId)
	{
		VRRig rig = VRRigCache.Instance.localRig.Rig;
		return !rig.HasCosmetic(cosmeticId) && PlayerCosmeticsSystem.IsTemporaryCosmeticAllowed(rig, cosmeticId);
	}

	// Token: 0x060053AC RID: 21420 RVA: 0x001B88C7 File Offset: 0x001B6AC7
	public static bool LocalPlayerInTemporaryCosmeticSpace()
	{
		return VRRigCache.Instance.localRig.Rig.inTempCosmSpace;
	}

	// Token: 0x060053AD RID: 21421 RVA: 0x001B88DD File Offset: 0x001B6ADD
	public static void StaticReset()
	{
		PlayerCosmeticsSystem.playersToLookUp.Clear();
		PlayerCosmeticsSystem.userCosmeticCallback.Clear();
		PlayerCosmeticsSystem.userCosmeticsWaiting.Clear();
		PlayerCosmeticsSystem.playerIDsList.Clear();
		PlayerCosmeticsSystem.playersWaiting.Clear();
	}

	// Token: 0x0400651F RID: 25887
	public float playerLookUpCooldown = 3f;

	// Token: 0x04006520 RID: 25888
	public float getSharedGroupDataCooldown = 0.1f;

	// Token: 0x04006521 RID: 25889
	private float startSearchingTime = float.MinValue;

	// Token: 0x04006522 RID: 25890
	private bool isLookingUp;

	// Token: 0x04006523 RID: 25891
	private bool isLookingUpNew;

	// Token: 0x04006524 RID: 25892
	private string tempCosmetics;

	// Token: 0x04006525 RID: 25893
	private NetPlayer playerTemp;

	// Token: 0x04006526 RID: 25894
	private RigContainer tempRC;

	// Token: 0x04006527 RID: 25895
	private List<string> inventory;

	// Token: 0x04006528 RID: 25896
	private const string inventoryKey = "InventoryDict";

	// Token: 0x04006529 RID: 25897
	private static readonly string subscriptionKey = "subscriptions.fan_club";

	// Token: 0x0400652A RID: 25898
	private static PlayerCosmeticsSystem instance;

	// Token: 0x0400652B RID: 25899
	private static Queue<NetPlayer> playersToLookUp = new Queue<NetPlayer>(20);

	// Token: 0x0400652C RID: 25900
	private static Dictionary<int, IUserCosmeticsCallback> userCosmeticCallback = new Dictionary<int, IUserCosmeticsCallback>(20);

	// Token: 0x0400652D RID: 25901
	private static Dictionary<int, string> userCosmeticsWaiting = new Dictionary<int, string>(5);

	// Token: 0x0400652E RID: 25902
	private static List<string> playerIDsList = new List<string>(20);

	// Token: 0x0400652F RID: 25903
	private static List<int> playerActorNumberList = new List<int>(20);

	// Token: 0x04006530 RID: 25904
	private static List<int> playersWaiting = new List<int>();

	// Token: 0x04006531 RID: 25905
	private static TimeSince sinceLastTryOnEvent = 0f;

	// Token: 0x04006532 RID: 25906
	private static readonly Dictionary<string, int> k_tempUnlockedCosmetics = new Dictionary<string, int>(20);

	// Token: 0x02000D34 RID: 3380
	[Serializable]
	public class SharedSubscriptionData
	{
		// Token: 0x04006535 RID: 25909
		public string Sku;

		// Token: 0x04006536 RID: 25910
		public DateTimeOffset? ExpirationTime;

		// Token: 0x04006537 RID: 25911
		public int TotalLifetimeSeconds;
	}
}
