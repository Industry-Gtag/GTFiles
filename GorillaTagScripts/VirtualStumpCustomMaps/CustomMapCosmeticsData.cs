using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using GorillaNetworking.Store;
using GT_CustomMapSupportRuntime;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000FC8 RID: 4040
	[CreateAssetMenu(menuName = "ScriptableObjects/CustomMapCosmeticDataSO", order = 0)]
	[Serializable]
	public class CustomMapCosmeticsData : ScriptableObject
	{
		// Token: 0x06006469 RID: 25705 RVA: 0x002049AA File Offset: 0x00202BAA
		public void OnEnable()
		{
			this.initializedFromTitleData = false;
		}

		// Token: 0x0600646A RID: 25706 RVA: 0x002049B3 File Offset: 0x00202BB3
		public void OnDestroy()
		{
			if (PlayFabTitleDataCache.Instance.IsNotNull())
			{
				PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnTitleDataUpdated));
			}
		}

		// Token: 0x0600646B RID: 25707 RVA: 0x002049DC File Offset: 0x00202BDC
		public bool TryGetItem(GTObjectPlaceholder.ECustomMapCosmeticItem customMapItemSlot, out CustomMapCosmeticItem foundItem)
		{
			if (!this.initializedFromTitleData)
			{
				this.UpdateFromTitleData();
			}
			foundItem = new CustomMapCosmeticItem
			{
				bustType = HeadModel_CosmeticStand.BustType.Disabled,
				playFabID = "INVALID"
			};
			for (int i = 0; i < this.customMapCosmeticItemList.Count; i++)
			{
				if (this.customMapCosmeticItemList[i].customMapItemSlot == customMapItemSlot)
				{
					foundItem = this.customMapCosmeticItemList[i];
					return true;
				}
			}
			for (int j = 0; j < this.fallbackItems.Count; j++)
			{
				if (this.fallbackItems[j].customMapItemSlot == customMapItemSlot)
				{
					foundItem = this.fallbackItems[j];
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600646C RID: 25708 RVA: 0x00204A98 File Offset: 0x00202C98
		private void UpdateFromTitleData()
		{
			if (this.initializedFromTitleData)
			{
				return;
			}
			if (PlayFabTitleDataCache.Instance.IsNull())
			{
				return;
			}
			PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnTitleDataUpdated));
			PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnTitleDataUpdated));
			if (PlayFabTitleDataCache.Instance == null)
			{
				Debug.LogError("[CustomMapCosmeticsData::UpdateFromTitleData] TitleData not available, using fallback item data.");
				this.initializedFromTitleData = true;
				return;
			}
			PlayFabTitleDataCache.Instance.GetTitleData(this.titleDataKey, new Action<string>(this.OnGetCosmeticsDataFromTitleData), new Action<PlayFabError>(this.OnPlayFabError), false);
			this.initializedFromTitleData = true;
		}

		// Token: 0x0600646D RID: 25709 RVA: 0x00204B40 File Offset: 0x00202D40
		private void OnTitleDataUpdated(string updatedKey)
		{
			if (updatedKey == this.titleDataKey)
			{
				this.initializedFromTitleData = false;
				this.UpdateFromTitleData();
			}
		}

		// Token: 0x0600646E RID: 25710 RVA: 0x00204B60 File Offset: 0x00202D60
		private void OnGetCosmeticsDataFromTitleData(string cosmeticsData)
		{
			string[] array = cosmeticsData.Split("|", StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				string text2 = text;
				text2 = text2.RemoveAll('\\', StringComparison.OrdinalIgnoreCase);
				text2 = text2.Trim('"');
				CustomMapCosmeticItem itemFromJson = JsonUtility.FromJson<CustomMapCosmeticItem>(text2);
				this.customMapCosmeticItemList.RemoveAll((CustomMapCosmeticItem item) => item.customMapItemSlot == itemFromJson.customMapItemSlot);
				this.customMapCosmeticItemList.Add(itemFromJson);
			}
		}

		// Token: 0x0600646F RID: 25711 RVA: 0x00204BDA File Offset: 0x00202DDA
		private void OnPlayFabError(PlayFabError error)
		{
			Debug.LogError("[CustomMapCosmeticsData::OnPlayFabError] failed to retrieve CosmeticsData from PlayFab: " + error.ErrorMessage);
		}

		// Token: 0x0400732D RID: 29485
		[SerializeField]
		private List<CustomMapCosmeticItem> fallbackItems;

		// Token: 0x0400732E RID: 29486
		[SerializeField]
		private List<CustomMapCosmeticItem> customMapCosmeticItemList;

		// Token: 0x0400732F RID: 29487
		public string titleDataKey = "CustomMapCosmeticData";

		// Token: 0x04007330 RID: 29488
		private bool initializedFromTitleData;
	}
}
