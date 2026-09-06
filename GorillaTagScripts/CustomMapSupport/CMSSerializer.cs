using System;
using System.Collections.Generic;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000FBD RID: 4029
	internal class CMSSerializer : GorillaSerializer
	{
		// Token: 0x06006425 RID: 25637 RVA: 0x00203386 File Offset: 0x00201586
		public void Awake()
		{
			if (CMSSerializer.instance != null)
			{
				Object.Destroy(this);
			}
			CMSSerializer.instance = this;
			CMSSerializer.hasInstance = true;
		}

		// Token: 0x06006426 RID: 25638 RVA: 0x002033AB File Offset: 0x002015AB
		public void OnEnable()
		{
			CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnCustomMapLoaded));
			CustomMapManager.OnMapLoadComplete.AddListener(new UnityAction<bool>(this.OnCustomMapLoaded));
		}

		// Token: 0x06006427 RID: 25639 RVA: 0x002033D9 File Offset: 0x002015D9
		public void OnDisable()
		{
			CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnCustomMapLoaded));
		}

		// Token: 0x06006428 RID: 25640 RVA: 0x002033F1 File Offset: 0x002015F1
		private void OnCustomMapLoaded(bool success)
		{
			if (success)
			{
				CMSSerializer.RequestSyncTriggerHistory();
			}
		}

		// Token: 0x06006429 RID: 25641 RVA: 0x002033FB File Offset: 0x002015FB
		public static void ResetSyncedMapObjects()
		{
			CMSSerializer.triggerHistory.Clear();
			CMSSerializer.triggerCounts.Clear();
			CMSSerializer.registeredTriggersPerScene.Clear();
			CMSSerializer.waitingForTriggerHistory = false;
			CMSSerializer.waitingForTriggerCounts = false;
		}

		// Token: 0x0600642A RID: 25642 RVA: 0x00203428 File Offset: 0x00201628
		public static void RegisterTrigger(string sceneName, CMSTrigger trigger)
		{
			Dictionary<byte, CMSTrigger> dictionary;
			if (CMSSerializer.registeredTriggersPerScene.TryGetValue(sceneName, out dictionary))
			{
				if (!dictionary.ContainsKey(trigger.GetID()))
				{
					dictionary.Add(trigger.GetID(), trigger);
					return;
				}
			}
			else
			{
				CMSSerializer.registeredTriggersPerScene.Add(sceneName, new Dictionary<byte, CMSTrigger> { 
				{
					trigger.GetID(),
					trigger
				} });
			}
		}

		// Token: 0x0600642B RID: 25643 RVA: 0x00203480 File Offset: 0x00201680
		private static bool TryGetRegisteredTrigger(byte triggerID, out CMSTrigger trigger)
		{
			trigger = null;
			foreach (KeyValuePair<string, Dictionary<byte, CMSTrigger>> keyValuePair in CMSSerializer.registeredTriggersPerScene)
			{
				if (keyValuePair.Value.TryGetValue(triggerID, out trigger))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600642C RID: 25644 RVA: 0x002034E8 File Offset: 0x002016E8
		public static void UnregisterTriggers(string forScene)
		{
			CMSSerializer.registeredTriggersPerScene.Remove(forScene);
		}

		// Token: 0x0600642D RID: 25645 RVA: 0x002034F6 File Offset: 0x002016F6
		public static void ResetTrigger(byte triggerID)
		{
			CMSSerializer.triggerCounts.Remove(triggerID);
		}

		// Token: 0x0600642E RID: 25646 RVA: 0x00203504 File Offset: 0x00201704
		private static void RequestSyncTriggerHistory()
		{
			if (!CMSSerializer.hasInstance || !NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			CMSSerializer.waitingForTriggerHistory = true;
			CMSSerializer.waitingForTriggerCounts = true;
			CMSSerializer.instance.SendRPC("RequestSyncTriggerHistory_RPC", false, Array.Empty<object>());
		}

		// Token: 0x0600642F RID: 25647 RVA: 0x00203554 File Offset: 0x00201754
		[PunRPC]
		private void RequestSyncTriggerHistory_RPC(PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "RequestSyncTriggerHistory_RPC");
			if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
			if (player.CheckSingleCallRPC(NetPlayer.SingleCallRPC.CMS_RequestTriggerHistory))
			{
				return;
			}
			player.ReceivedSingleCallRPC(NetPlayer.SingleCallRPC.CMS_RequestTriggerHistory);
			byte[] array = CMSSerializer.triggerHistory.ToArray();
			base.SendRPC("SyncTriggerHistory_RPC", info.Sender, new object[] { array });
			base.SendRPC("SyncTriggerCounts_RPC", info.Sender, new object[] { CMSSerializer.triggerCounts });
		}

		// Token: 0x06006430 RID: 25648 RVA: 0x002035EC File Offset: 0x002017EC
		[PunRPC]
		private void SyncTriggerHistory_RPC(byte[] syncedTriggerHistory, PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "SyncTriggerHistory_RPC");
			if (!NetworkSystem.Instance.InRoom || !info.Sender.IsMasterClient)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
			if (player.CheckSingleCallRPC(NetPlayer.SingleCallRPC.CMS_SyncTriggerHistory))
			{
				return;
			}
			player.ReceivedSingleCallRPC(NetPlayer.SingleCallRPC.CMS_SyncTriggerHistory);
			if (!CMSSerializer.waitingForTriggerHistory)
			{
				return;
			}
			CMSSerializer.triggerHistory.Clear();
			if (!syncedTriggerHistory.IsNullOrEmpty<byte>())
			{
				CMSSerializer.triggerHistory.AddRange(syncedTriggerHistory);
			}
			CMSSerializer.waitingForTriggerHistory = false;
			foreach (string text in CMSSerializer.scenesWaitingForTriggerHistory)
			{
				CMSSerializer.ProcessTriggerHistory(text);
			}
			CMSSerializer.scenesWaitingForTriggerHistory.Clear();
		}

		// Token: 0x06006431 RID: 25649 RVA: 0x002036B8 File Offset: 0x002018B8
		[PunRPC]
		private void SyncTriggerCounts_RPC(Dictionary<byte, byte> syncedTriggerCounts, PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "SyncTriggerCounts_RPC");
			if (!NetworkSystem.Instance.InRoom || !info.Sender.IsMasterClient)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
			if (player.CheckSingleCallRPC(NetPlayer.SingleCallRPC.CMS_SyncTriggerCounts))
			{
				return;
			}
			player.ReceivedSingleCallRPC(NetPlayer.SingleCallRPC.CMS_SyncTriggerCounts);
			if (!CMSSerializer.waitingForTriggerCounts)
			{
				return;
			}
			CMSSerializer.triggerCounts.Clear();
			if (syncedTriggerCounts != null && syncedTriggerCounts.Count > 0)
			{
				CMSSerializer.triggerCounts = syncedTriggerCounts;
			}
			CMSSerializer.waitingForTriggerCounts = false;
			foreach (string text in CMSSerializer.scenesWaitingForTriggerCounts)
			{
				CMSSerializer.ProcessTriggerCounts(text);
			}
			CMSSerializer.scenesWaitingForTriggerCounts.Clear();
		}

		// Token: 0x06006432 RID: 25650 RVA: 0x00203784 File Offset: 0x00201984
		public static void ProcessSceneLoad(string sceneName)
		{
			if (CMSSerializer.waitingForTriggerHistory)
			{
				CMSSerializer.scenesWaitingForTriggerHistory.Add(sceneName);
			}
			else
			{
				CMSSerializer.ProcessTriggerHistory(sceneName);
			}
			if (CMSSerializer.waitingForTriggerCounts)
			{
				CMSSerializer.scenesWaitingForTriggerCounts.Add(sceneName);
				return;
			}
			CMSSerializer.ProcessTriggerCounts(sceneName);
		}

		// Token: 0x06006433 RID: 25651 RVA: 0x002037BC File Offset: 0x002019BC
		private static void ProcessTriggerHistory(string forScene)
		{
			Dictionary<byte, CMSTrigger> dictionary;
			if (CMSSerializer.registeredTriggersPerScene.TryGetValue(forScene, out dictionary))
			{
				foreach (byte b in CMSSerializer.triggerHistory)
				{
					CMSTrigger cmstrigger;
					if (dictionary.TryGetValue(b, out cmstrigger))
					{
						cmstrigger.Trigger(1.0, false, true);
					}
				}
			}
			UnityEvent<string> onTriggerHistoryProcessedForScene = CMSSerializer.OnTriggerHistoryProcessedForScene;
			if (onTriggerHistoryProcessedForScene == null)
			{
				return;
			}
			onTriggerHistoryProcessedForScene.Invoke(forScene);
		}

		// Token: 0x06006434 RID: 25652 RVA: 0x00203844 File Offset: 0x00201A44
		private static void ProcessTriggerCounts(string forScene)
		{
			Dictionary<byte, CMSTrigger> dictionary;
			if (CMSSerializer.registeredTriggersPerScene.TryGetValue(forScene, out dictionary))
			{
				List<byte> list = new List<byte>();
				foreach (KeyValuePair<byte, byte> keyValuePair in CMSSerializer.triggerCounts)
				{
					CMSTrigger cmstrigger;
					if (dictionary.TryGetValue(keyValuePair.Key, out cmstrigger))
					{
						if (cmstrigger.numAllowedTriggers > 0)
						{
							cmstrigger.SetTriggerCount(keyValuePair.Value);
						}
						else
						{
							list.Add(keyValuePair.Key);
						}
					}
				}
				foreach (byte b in list)
				{
					CMSSerializer.triggerCounts.Remove(b);
				}
			}
		}

		// Token: 0x06006435 RID: 25653 RVA: 0x00203924 File Offset: 0x00201B24
		public static void RequestTrigger(byte triggerID)
		{
			if (!CMSSerializer.hasInstance)
			{
				return;
			}
			if (!NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
			{
				double num = (double)Time.time;
				if (NetworkSystem.Instance.InRoom)
				{
					num = PhotonNetwork.Time;
					CMSSerializer.instance.SendRPC("ActivateTrigger_RPC", true, new object[]
					{
						triggerID,
						NetworkSystem.Instance.LocalPlayer.ActorNumber
					});
				}
				CMSSerializer.instance.ActivateTrigger(triggerID, num, true);
				return;
			}
			CMSSerializer.instance.SendRPC("RequestTrigger_RPC", false, new object[] { triggerID });
		}

		// Token: 0x06006436 RID: 25654 RVA: 0x002039D4 File Offset: 0x00201BD4
		[PunRPC]
		private void RequestTrigger_RPC(byte triggerID, PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "RequestTrigger_RPC");
			if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[11].CallLimitSettings.CheckCallTime(Time.unscaledTime))
			{
				return;
			}
			CMSTrigger cmstrigger;
			if (CMSSerializer.TryGetRegisteredTrigger(triggerID, out cmstrigger))
			{
				if (!cmstrigger.CanTrigger())
				{
					return;
				}
				Vector3 position = cmstrigger.gameObject.transform.position;
				RigContainer rigContainer2;
				if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer2))
				{
					return;
				}
				if ((rigContainer2.Rig.bodyTransform.position - position).sqrMagnitude > cmstrigger.validationDistanceSquared)
				{
					return;
				}
			}
			base.SendRPC("ActivateTrigger_RPC", true, new object[]
			{
				triggerID,
				info.Sender.ActorNumber
			});
			this.ActivateTrigger(triggerID, info.SentServerTime, false);
		}

		// Token: 0x06006437 RID: 25655 RVA: 0x00203AEC File Offset: 0x00201CEC
		[PunRPC]
		private void ActivateTrigger_RPC(byte triggerID, int originatingPlayer, PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "ActivateTrigger_RPC");
			if (!NetworkSystem.Instance.InRoom || !info.Sender.IsMasterClient)
			{
				return;
			}
			if (info.SentServerTime < 0.0 || info.SentServerTime > 4294967.295)
			{
				return;
			}
			double num = (double)PhotonNetwork.GetPing() / 1000.0;
			if (!Utils.ValidateServerTime(info.SentServerTime, Math.Max(10.0, num * 2.0)))
			{
				return;
			}
			if (!CMSSerializer.ActivateTriggerCallLimiter.CheckCallTime(Time.unscaledTime))
			{
				return;
			}
			this.ActivateTrigger(triggerID, info.SentServerTime, NetworkSystem.Instance.LocalPlayer.ActorNumber == originatingPlayer);
		}

		// Token: 0x06006438 RID: 25656 RVA: 0x00203BB0 File Offset: 0x00201DB0
		private void ActivateTrigger(byte triggerID, double triggerTime = -1.0, bool originatedLocally = false)
		{
			CMSTrigger cmstrigger;
			bool flag = CMSSerializer.TryGetRegisteredTrigger(triggerID, out cmstrigger);
			if (!double.IsFinite(triggerTime))
			{
				triggerTime = -1.0;
			}
			byte b;
			bool flag2 = CMSSerializer.triggerCounts.TryGetValue(triggerID, out b);
			bool flag3 = !flag || cmstrigger.numAllowedTriggers > 0;
			if (flag2)
			{
				CMSSerializer.triggerCounts[triggerID] = ((b == byte.MaxValue) ? byte.MaxValue : (b += 1));
			}
			else if (flag3)
			{
				CMSSerializer.triggerCounts.Add(triggerID, 1);
			}
			CMSSerializer.triggerHistory.Remove(triggerID);
			CMSSerializer.triggerHistory.Add(triggerID);
			if (flag)
			{
				cmstrigger.Trigger(triggerTime, originatedLocally, false);
			}
		}

		// Token: 0x040072F4 RID: 29428
		[OnEnterPlay_SetNull]
		private static volatile CMSSerializer instance;

		// Token: 0x040072F5 RID: 29429
		[OnEnterPlay_Set(false)]
		private static bool hasInstance;

		// Token: 0x040072F6 RID: 29430
		private static Dictionary<string, Dictionary<byte, CMSTrigger>> registeredTriggersPerScene = new Dictionary<string, Dictionary<byte, CMSTrigger>>();

		// Token: 0x040072F7 RID: 29431
		private static List<byte> triggerHistory = new List<byte>();

		// Token: 0x040072F8 RID: 29432
		private static Dictionary<byte, byte> triggerCounts = new Dictionary<byte, byte>();

		// Token: 0x040072F9 RID: 29433
		private static bool waitingForTriggerHistory;

		// Token: 0x040072FA RID: 29434
		private static List<string> scenesWaitingForTriggerHistory = new List<string>();

		// Token: 0x040072FB RID: 29435
		private static bool waitingForTriggerCounts;

		// Token: 0x040072FC RID: 29436
		private static List<string> scenesWaitingForTriggerCounts = new List<string>();

		// Token: 0x040072FD RID: 29437
		private static CallLimiter ActivateTriggerCallLimiter = new CallLimiter(50, 1f, 0.5f);

		// Token: 0x040072FE RID: 29438
		public static UnityEvent<string> OnTriggerHistoryProcessedForScene = new UnityEvent<string>();
	}
}
