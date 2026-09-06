using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FXP;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02001157 RID: 4439
	public class StoreUpdater : MonoBehaviour
	{
		// Token: 0x17000AA1 RID: 2721
		// (get) Token: 0x06006F70 RID: 28528 RVA: 0x0023E7CF File Offset: 0x0023C9CF
		public DateTime DateTimeNowServerAdjusted
		{
			get
			{
				return GorillaComputer.instance.GetServerTime();
			}
		}

		// Token: 0x06006F71 RID: 28529 RVA: 0x0023E7DD File Offset: 0x0023C9DD
		public void Awake()
		{
			if (StoreUpdater.instance == null)
			{
				StoreUpdater.instance = this;
				return;
			}
			if (StoreUpdater.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06006F72 RID: 28530 RVA: 0x0023E811 File Offset: 0x0023CA11
		private void OnApplicationFocus(bool hasFocus)
		{
			if (hasFocus)
			{
				this.HandleHMDMounted();
				return;
			}
			this.HandleHMDUnmounted();
		}

		// Token: 0x06006F73 RID: 28531 RVA: 0x0023E824 File Offset: 0x0023CA24
		public void Initialize()
		{
			this.FindAllCosmeticItemPrefabs();
			OVRManager.HMDMounted += this.HandleHMDMounted;
			OVRManager.HMDUnmounted += this.HandleHMDUnmounted;
			OVRManager.HMDLost += this.HandleHMDUnmounted;
			OVRManager.HMDAcquired += this.HandleHMDMounted;
			if (this.bLoadFromJSON)
			{
				this.GetEventsFromTitleData();
			}
		}

		// Token: 0x06006F74 RID: 28532 RVA: 0x0023E88C File Offset: 0x0023CA8C
		public void OnDestroy()
		{
			OVRManager.HMDMounted -= this.HandleHMDMounted;
			OVRManager.HMDUnmounted -= this.HandleHMDUnmounted;
			OVRManager.HMDLost -= this.HandleHMDUnmounted;
			OVRManager.HMDAcquired -= this.HandleHMDMounted;
		}

		// Token: 0x06006F75 RID: 28533 RVA: 0x0023E8E0 File Offset: 0x0023CAE0
		private void HandleHMDUnmounted()
		{
			foreach (string text in this.pedestalUpdateCoroutines.Keys)
			{
				if (this.pedestalUpdateCoroutines[text] != null)
				{
					base.StopCoroutine(this.pedestalUpdateCoroutines[text]);
				}
			}
			foreach (string text2 in this.cosmeticItemPrefabsDictionary.Keys)
			{
				if (this.cosmeticItemPrefabsDictionary[text2] != null)
				{
					this.cosmeticItemPrefabsDictionary[text2].StopCountdownCoroutine();
				}
			}
		}

		// Token: 0x06006F76 RID: 28534 RVA: 0x0023E9B8 File Offset: 0x0023CBB8
		private void HandleHMDMounted()
		{
			foreach (string text in this.cosmeticItemPrefabsDictionary.Keys)
			{
				if (this.cosmeticItemPrefabsDictionary[text] != null && this.pedestalUpdateEvents.ContainsKey(text) && this.cosmeticItemPrefabsDictionary[text].gameObject.activeInHierarchy)
				{
					this.CheckEventsOnResume(this.pedestalUpdateEvents[text]);
					this.StartNextEvent(text, false);
				}
			}
		}

		// Token: 0x06006F77 RID: 28535 RVA: 0x0023EA60 File Offset: 0x0023CC60
		private void FindAllCosmeticItemPrefabs()
		{
			foreach (CosmeticItemPrefab cosmeticItemPrefab in Object.FindObjectsByType<CosmeticItemPrefab>(FindObjectsSortMode.None))
			{
				if (this.cosmeticItemPrefabsDictionary.ContainsKey(cosmeticItemPrefab.PedestalID))
				{
					Debug.LogWarning("StoreUpdater - Duplicate Pedestal ID " + cosmeticItemPrefab.PedestalID);
				}
				else
				{
					this.cosmeticItemPrefabsDictionary.Add(cosmeticItemPrefab.PedestalID, cosmeticItemPrefab);
				}
			}
		}

		// Token: 0x06006F78 RID: 28536 RVA: 0x0023EAC2 File Offset: 0x0023CCC2
		private IEnumerator HandlePedestalUpdate(StoreUpdateEvent updateEvent, bool playFX)
		{
			this.cosmeticItemPrefabsDictionary[updateEvent.PedestalID].SetStoreUpdateEvent(updateEvent, playFX);
			yield return new WaitForSeconds((float)(updateEvent.EndTimeUTC.ToUniversalTime() - this.DateTimeNowServerAdjusted).TotalSeconds);
			if (this.pedestalClearCartCoroutines.ContainsKey(updateEvent.PedestalID))
			{
				if (this.pedestalClearCartCoroutines[updateEvent.PedestalID] != null)
				{
					base.StopCoroutine(this.pedestalClearCartCoroutines[updateEvent.PedestalID]);
				}
				this.pedestalClearCartCoroutines[updateEvent.PedestalID] = base.StartCoroutine(this.HandleClearCart(updateEvent));
			}
			else
			{
				this.pedestalClearCartCoroutines.Add(updateEvent.PedestalID, base.StartCoroutine(this.HandleClearCart(updateEvent)));
			}
			if (this.cosmeticItemPrefabsDictionary[updateEvent.PedestalID].gameObject.activeInHierarchy)
			{
				this.pedestalUpdateEvents[updateEvent.PedestalID].RemoveAt(0);
				this.StartNextEvent(updateEvent.PedestalID, true);
			}
			yield break;
		}

		// Token: 0x06006F79 RID: 28537 RVA: 0x0023EADF File Offset: 0x0023CCDF
		private IEnumerator HandleClearCart(StoreUpdateEvent updateEvent)
		{
			float num = Math.Clamp((float)(updateEvent.EndTimeUTC.ToUniversalTime() - this.DateTimeNowServerAdjusted).TotalSeconds + 60f, 0f, 60f);
			yield return new WaitForSeconds(num);
			if (CosmeticsController.instance.RemoveItemFromCart(CosmeticsController.instance.GetItemFromDict(updateEvent.ItemName)))
			{
				CosmeticsController.instance.ClearCheckout(true);
				CosmeticsController.instance.UpdateShoppingCart();
				CosmeticsController.instance.UpdateWornCosmetics(true);
			}
			yield break;
		}

		// Token: 0x06006F7A RID: 28538 RVA: 0x0023EAF8 File Offset: 0x0023CCF8
		private void StartNextEvent(string pedestalID, bool playFX)
		{
			if (this.pedestalUpdateEvents[pedestalID].Count > 0)
			{
				Coroutine coroutine = base.StartCoroutine(this.HandlePedestalUpdate(this.pedestalUpdateEvents[pedestalID].First<StoreUpdateEvent>(), playFX));
				if (this.pedestalUpdateCoroutines.ContainsKey(pedestalID))
				{
					if (this.pedestalUpdateCoroutines[pedestalID] != null && this.pedestalUpdateCoroutines[pedestalID] != null)
					{
						base.StopCoroutine(this.pedestalUpdateCoroutines[pedestalID]);
					}
					this.pedestalUpdateCoroutines[pedestalID] = coroutine;
				}
				else
				{
					this.pedestalUpdateCoroutines.Add(pedestalID, coroutine);
				}
				if (this.pedestalUpdateEvents[pedestalID].Count == 0 && !this.bLoadFromJSON)
				{
					this.GetStoreUpdateEventsPlaceHolder(pedestalID);
					return;
				}
			}
			else if (!this.bLoadFromJSON)
			{
				this.GetStoreUpdateEventsPlaceHolder(pedestalID);
				this.StartNextEvent(pedestalID, true);
			}
		}

		// Token: 0x06006F7B RID: 28539 RVA: 0x0023EBD0 File Offset: 0x0023CDD0
		private void GetStoreUpdateEventsPlaceHolder(string PedestalID)
		{
			List<StoreUpdateEvent> list = new List<StoreUpdateEvent>();
			list = this.CreateTempEvents(PedestalID, 1, 15);
			this.CheckEvents(list);
			if (this.pedestalUpdateEvents.ContainsKey(PedestalID))
			{
				this.pedestalUpdateEvents[PedestalID].AddRange(list);
				return;
			}
			this.pedestalUpdateEvents.Add(PedestalID, list);
		}

		// Token: 0x06006F7C RID: 28540 RVA: 0x0023EC24 File Offset: 0x0023CE24
		private void CheckEvents(List<StoreUpdateEvent> updateEvents)
		{
			for (int i = 0; i < updateEvents.Count; i++)
			{
				if (updateEvents[i].EndTimeUTC.ToUniversalTime() < this.DateTimeNowServerAdjusted)
				{
					updateEvents.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x06006F7D RID: 28541 RVA: 0x0023EC6C File Offset: 0x0023CE6C
		private void CheckEventsOnResume(List<StoreUpdateEvent> updateEvents)
		{
			bool flag = false;
			for (int i = 0; i < updateEvents.Count; i++)
			{
				if (updateEvents[i].EndTimeUTC.ToUniversalTime() < this.DateTimeNowServerAdjusted)
				{
					if (Math.Clamp((float)(updateEvents[i].EndTimeUTC.ToUniversalTime() - this.DateTimeNowServerAdjusted).TotalSeconds + 60f, 0f, 60f) <= 0f)
					{
						flag ^= CosmeticsController.instance.RemoveItemFromCart(CosmeticsController.instance.GetItemFromDict(updateEvents[i].ItemName));
					}
					else if (this.pedestalClearCartCoroutines.ContainsKey(updateEvents[i].PedestalID))
					{
						if (this.pedestalClearCartCoroutines[updateEvents[i].PedestalID] != null)
						{
							base.StopCoroutine(this.pedestalClearCartCoroutines[updateEvents[i].PedestalID]);
						}
						this.pedestalClearCartCoroutines[updateEvents[i].PedestalID] = base.StartCoroutine(this.HandleClearCart(updateEvents[i]));
					}
					else
					{
						this.pedestalClearCartCoroutines.Add(updateEvents[i].PedestalID, base.StartCoroutine(this.HandleClearCart(updateEvents[i])));
					}
					updateEvents.RemoveAt(i);
					i--;
				}
			}
			if (flag)
			{
				CosmeticsController.instance.ClearCheckout(true);
				CosmeticsController.instance.UpdateShoppingCart();
				CosmeticsController.instance.UpdateWornCosmetics(true);
			}
		}

		// Token: 0x06006F7E RID: 28542 RVA: 0x0023EDFC File Offset: 0x0023CFFC
		private void GetEventsFromTitleData()
		{
			if (this.bUsePlaceHolderJSON)
			{
				DateTime dateTime = new DateTime(2024, 2, 13, 16, 0, 0, DateTimeKind.Utc);
				List<StoreUpdateEvent> list = StoreUpdateEvent.DeserializeFromJSonList(StoreUpdateEvent.SerializeArrayAsJSon(this.CreateTempEvents("Pedestal1", 2, 120, dateTime).ToArray()));
				this.HandleRecievingEventsFromTitleData(list);
				return;
			}
			PlayFabTitleDataCache.Instance.GetTitleData("TOTD", delegate(string result)
			{
				List<StoreUpdateEvent> list2 = StoreUpdateEvent.DeserializeFromJSonList(result);
				this.HandleRecievingEventsFromTitleData(list2);
			}, delegate(PlayFabError error)
			{
				Debug.Log("StoreUpdater - Error Title Data : " + error.ErrorMessage);
			}, false);
		}

		// Token: 0x06006F7F RID: 28543 RVA: 0x0023EE88 File Offset: 0x0023D088
		private void HandleRecievingEventsFromTitleData(List<StoreUpdateEvent> updateEvents)
		{
			this.CheckEvents(updateEvents);
			if (CosmeticsController.instance.GetItemFromDict("LBAEY.").isNullItem)
			{
				Debug.LogWarning("StoreUpdater - CosmeticsController is not initialized.  Reinitializing TitleData");
				this.GetEventsFromTitleData();
				return;
			}
			foreach (StoreUpdateEvent storeUpdateEvent in updateEvents)
			{
				if (this.pedestalUpdateEvents.ContainsKey(storeUpdateEvent.PedestalID))
				{
					this.pedestalUpdateEvents[storeUpdateEvent.PedestalID].Add(storeUpdateEvent);
				}
				else
				{
					this.pedestalUpdateEvents.Add(storeUpdateEvent.PedestalID, new List<StoreUpdateEvent>());
					this.pedestalUpdateEvents[storeUpdateEvent.PedestalID].Add(storeUpdateEvent);
				}
			}
			foreach (string text in this.pedestalUpdateEvents.Keys)
			{
				if (this.cosmeticItemPrefabsDictionary.ContainsKey(text))
				{
					this.StartNextEvent(text, false);
				}
			}
			foreach (string text2 in this.cosmeticItemPrefabsDictionary.Keys)
			{
				if (!this.pedestalUpdateEvents.ContainsKey(text2))
				{
					this.GetStoreUpdateEventsPlaceHolder(text2);
					this.StartNextEvent(text2, false);
				}
			}
		}

		// Token: 0x06006F80 RID: 28544 RVA: 0x0023F010 File Offset: 0x0023D210
		private void PrintJSONEvents()
		{
			string text = StoreUpdateEvent.SerializeArrayAsJSon(this.CreateTempEvents("Pedestal1", 5, 28).ToArray());
			foreach (StoreUpdateEvent storeUpdateEvent in StoreUpdateEvent.DeserializeFromJSonList(text))
			{
			}
			this.tempJson = text;
		}

		// Token: 0x06006F81 RID: 28545 RVA: 0x0023F07C File Offset: 0x0023D27C
		private List<StoreUpdateEvent> CreateTempEvents(string PedestalID, int minuteDelay, int totalEvents)
		{
			string[] array = new string[]
			{
				"LBAEY.", "LBAEZ.", "LBAFA.", "LBAFB.", "LBAFC.", "LBAFD.", "LBAFE.", "LBAFF.", "LBAFG.", "LBAFH.",
				"LBAFO.", "LBAFP.", "LBAFQ.", "LBAFR."
			};
			List<StoreUpdateEvent> list = new List<StoreUpdateEvent>();
			for (int i = 0; i < totalEvents; i++)
			{
				StoreUpdateEvent storeUpdateEvent = new StoreUpdateEvent(PedestalID, array[i % 14], DateTime.UtcNow + TimeSpan.FromMinutes((double)(minuteDelay * i)), DateTime.UtcNow + TimeSpan.FromMinutes((double)(minuteDelay * (i + 1))));
				list.Add(storeUpdateEvent);
			}
			return list;
		}

		// Token: 0x06006F82 RID: 28546 RVA: 0x0023F158 File Offset: 0x0023D358
		private List<StoreUpdateEvent> CreateTempEvents(string PedestalID, int minuteDelay, int totalEvents, DateTime startTime)
		{
			string[] array = new string[]
			{
				"LBAEY.", "LBAEZ.", "LBAFA.", "LBAFB.", "LBAFC.", "LBAFD.", "LBAFE.", "LBAFF.", "LBAFG.", "LBAFH.",
				"LBAFO.", "LBAFP.", "LBAFQ.", "LBAFR."
			};
			List<StoreUpdateEvent> list = new List<StoreUpdateEvent>();
			for (int i = 0; i < totalEvents; i++)
			{
				StoreUpdateEvent storeUpdateEvent = new StoreUpdateEvent(PedestalID, array[i % 14], startTime + TimeSpan.FromMinutes((double)(minuteDelay * i)), startTime + TimeSpan.FromMinutes((double)(minuteDelay * (i + 1))));
				list.Add(storeUpdateEvent);
			}
			return list;
		}

		// Token: 0x06006F83 RID: 28547 RVA: 0x0023F22B File Offset: 0x0023D42B
		public void PedestalAsleep(CosmeticItemPrefab pedestal)
		{
			if (this.pedestalUpdateCoroutines.ContainsKey(pedestal.PedestalID) && this.pedestalUpdateCoroutines[pedestal.PedestalID] != null)
			{
				base.StopCoroutine(this.pedestalUpdateCoroutines[pedestal.PedestalID]);
			}
		}

		// Token: 0x06006F84 RID: 28548 RVA: 0x0023F26C File Offset: 0x0023D46C
		public void PedestalAwakened(CosmeticItemPrefab pedestal)
		{
			if (!this.cosmeticItemPrefabsDictionary.ContainsKey(pedestal.PedestalID))
			{
				this.cosmeticItemPrefabsDictionary.Add(pedestal.PedestalID, pedestal);
			}
			if (this.pedestalUpdateEvents.ContainsKey(pedestal.PedestalID))
			{
				this.CheckEventsOnResume(this.pedestalUpdateEvents[pedestal.PedestalID]);
				this.StartNextEvent(pedestal.PedestalID, false);
			}
		}

		// Token: 0x04007F6C RID: 32620
		public static volatile StoreUpdater instance;

		// Token: 0x04007F6D RID: 32621
		private DateTime StoreItemsChangeTimeUTC;

		// Token: 0x04007F6E RID: 32622
		private Dictionary<string, CosmeticItemPrefab> cosmeticItemPrefabsDictionary = new Dictionary<string, CosmeticItemPrefab>();

		// Token: 0x04007F6F RID: 32623
		private Dictionary<string, List<StoreUpdateEvent>> pedestalUpdateEvents = new Dictionary<string, List<StoreUpdateEvent>>();

		// Token: 0x04007F70 RID: 32624
		private Dictionary<string, Coroutine> pedestalUpdateCoroutines = new Dictionary<string, Coroutine>();

		// Token: 0x04007F71 RID: 32625
		private Dictionary<string, Coroutine> pedestalClearCartCoroutines = new Dictionary<string, Coroutine>();

		// Token: 0x04007F72 RID: 32626
		private string tempJson;

		// Token: 0x04007F73 RID: 32627
		private bool bLoadFromJSON = true;

		// Token: 0x04007F74 RID: 32628
		private bool bUsePlaceHolderJSON;
	}
}
