using System;
using System.Collections.Generic;
using GorillaNetworking;
using Modio;
using Modio.Mods;
using PlayFab;
using UnityEngine.Events;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000FC2 RID: 4034
	public static class CuratedDestinationsManager
	{
		// Token: 0x1700098D RID: 2445
		// (get) Token: 0x06006451 RID: 25681 RVA: 0x00204417 File Offset: 0x00202617
		public static bool IsLoading
		{
			get
			{
				return CuratedDestinationsManager.loadingCuratedMaps;
			}
		}

		// Token: 0x1700098E RID: 2446
		// (get) Token: 0x06006452 RID: 25682 RVA: 0x0020441E File Offset: 0x0020261E
		public static bool HasRetrievedCuratedMaps
		{
			get
			{
				return CuratedDestinationsManager.curatedMapsRetrieved;
			}
		}

		// Token: 0x06006453 RID: 25683 RVA: 0x00204425 File Offset: 0x00202625
		public static void RetrieveCuratedMaps(bool forceRefresh = false)
		{
			if (CuratedDestinationsManager.loadingCuratedMaps || (CuratedDestinationsManager.curatedMapsRetrieved && !forceRefresh))
			{
				return;
			}
			CuratedDestinationsManager.loadingCuratedMaps = true;
			PlayFabTitleDataCache.RegisterOnLoad(delegate(PlayFabTitleDataCache cache)
			{
				cache.GetTitleData("DestinationsCurated", new Action<string>(CuratedDestinationsManager.OnGetCuratedMapsTitleData), delegate(PlayFabError error)
				{
					GTDev.LogError<string>("[CuratedDestinationsManager::RetrieveCuratedMaps] Failed to retrieve curated maps from TitleData: " + error.ErrorMessage, null);
					CuratedDestinationsManager.FinishRetrieval(false);
				}, false);
			});
		}

		// Token: 0x06006454 RID: 25684 RVA: 0x00204464 File Offset: 0x00202664
		private static async void OnGetCuratedMapsTitleData(string data)
		{
			bool succeeded = true;
			try
			{
				CuratedDestinationsManager.curatedModIds.Clear();
				CuratedDestinationsManager.curatedMods.Clear();
				if (!data.IsNullOrEmpty())
				{
					if (data.Length >= 2 && data[0] == '"' && data[data.Length - 1] == '"')
					{
						data = data.Substring(1, data.Length - 2);
					}
					string[] array = data.Split(',', StringSplitOptions.None);
					foreach (string text in array)
					{
						long curatedModId;
						if (string.IsNullOrWhiteSpace(text) || !long.TryParse(text.Trim(), out curatedModId))
						{
							GTDev.LogError<string>("[CuratedDestinationsManager::OnGetCuratedMapsTitleData] Failed to parse curated map id as a long: " + text, null);
							CuratedDestinationsManager.AddEmptySlot();
						}
						else
						{
							ValueTuple<Error, Mod> valueTuple = await ModIOManager.GetMod(new ModId(curatedModId), false, null);
							if (valueTuple.Item1)
							{
								GTDev.LogError<string>("[CuratedDestinationsManager::OnGetCuratedMapsTitleData] Failed to get mod profile " + string.Format("for curated map {0}: {1}", curatedModId, valueTuple.Item1.GetMessage()), null);
								CuratedDestinationsManager.AddEmptySlot();
							}
							else if (valueTuple.Item2.Creator == null)
							{
								CuratedDestinationsManager.AddEmptySlot();
							}
							else if (UGCPermissionManager.FeaturedMapsOnly && !ModIOManager.IsFeaturedMap(valueTuple.Item2))
							{
								CuratedDestinationsManager.AddEmptySlot();
							}
							else
							{
								CuratedDestinationsManager.curatedModIds.Add(curatedModId);
								CuratedDestinationsManager.curatedMods.Add(valueTuple.Item2);
							}
						}
					}
					string[] array2 = null;
				}
			}
			catch (Exception ex)
			{
				GTDev.LogError<string>("[CuratedDestinationsManager::OnGetCuratedMapsTitleData] Failed to resolve curated maps: " + ex.Message, null);
				succeeded = false;
			}
			finally
			{
				CuratedDestinationsManager.FinishRetrieval(succeeded);
			}
		}

		// Token: 0x06006455 RID: 25685 RVA: 0x0020449B File Offset: 0x0020269B
		private static void AddEmptySlot()
		{
			CuratedDestinationsManager.curatedModIds.Add(-1L);
			CuratedDestinationsManager.curatedMods.Add(null);
		}

		// Token: 0x06006456 RID: 25686 RVA: 0x002044B4 File Offset: 0x002026B4
		private static void FinishRetrieval(bool succeeded)
		{
			CuratedDestinationsManager.curatedMapsRetrieved = succeeded;
			CuratedDestinationsManager.loadingCuratedMaps = false;
			GTDev.Log<string>(string.Format("[CuratedDestinationsManager::FinishRetrieval] succeeded {0} curatedSlotCount {1}", succeeded, CuratedDestinationsManager.curatedModIds.Count), null);
			UnityEvent onCuratedMapsUpdated = CuratedDestinationsManager.OnCuratedMapsUpdated;
			if (onCuratedMapsUpdated == null)
			{
				return;
			}
			onCuratedMapsUpdated.Invoke();
		}

		// Token: 0x06006457 RID: 25687 RVA: 0x00204501 File Offset: 0x00202701
		public static bool TryGetCuratedModId(CuratedDestinationsManager.CuratedDoorway doorway, out ModId modId)
		{
			return CuratedDestinationsManager.TryGetCuratedModId((int)doorway, out modId);
		}

		// Token: 0x06006458 RID: 25688 RVA: 0x0020450C File Offset: 0x0020270C
		private static bool TryGetCuratedModId(int doorwayIndex, out ModId modId)
		{
			modId = ModId.Null;
			if (doorwayIndex < 0 || doorwayIndex >= CuratedDestinationsManager.curatedModIds.Count || CuratedDestinationsManager.curatedModIds[doorwayIndex] <= 0L)
			{
				return false;
			}
			modId = new ModId(CuratedDestinationsManager.curatedModIds[doorwayIndex]);
			return true;
		}

		// Token: 0x06006459 RID: 25689 RVA: 0x0020455D File Offset: 0x0020275D
		public static void TryGetCuratedMod(CuratedDestinationsManager.CuratedDoorway doorway, out Mod mod)
		{
			CuratedDestinationsManager.TryGetCuratedMod((int)doorway, out mod);
		}

		// Token: 0x0600645A RID: 25690 RVA: 0x00204567 File Offset: 0x00202767
		private static bool TryGetCuratedMod(int doorwayIndex, out Mod mod)
		{
			mod = null;
			if (doorwayIndex < 0 || doorwayIndex >= CuratedDestinationsManager.curatedMods.Count || CuratedDestinationsManager.curatedMods[doorwayIndex] == null)
			{
				return false;
			}
			mod = CuratedDestinationsManager.curatedMods[doorwayIndex];
			return true;
		}

		// Token: 0x04007313 RID: 29459
		private const string CURATED_MAPS_PLAYFAB_KEY = "DestinationsCurated";

		// Token: 0x04007314 RID: 29460
		[OnEnterPlay_SetNew]
		public static UnityEvent OnCuratedMapsUpdated = new UnityEvent();

		// Token: 0x04007315 RID: 29461
		[OnEnterPlay_Set(false)]
		private static bool loadingCuratedMaps;

		// Token: 0x04007316 RID: 29462
		[OnEnterPlay_Set(false)]
		private static bool curatedMapsRetrieved;

		// Token: 0x04007317 RID: 29463
		[OnEnterPlay_Clear]
		private static readonly List<long> curatedModIds = new List<long>();

		// Token: 0x04007318 RID: 29464
		[OnEnterPlay_Clear]
		private static readonly List<Mod> curatedMods = new List<Mod>();

		// Token: 0x02000FC3 RID: 4035
		public enum CuratedDoorway
		{
			// Token: 0x0400731A RID: 29466
			Left,
			// Token: 0x0400731B RID: 29467
			Right
		}
	}
}
