using System;
using System.IO;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02001154 RID: 4436
	[Serializable]
	public class StoreItem
	{
		// Token: 0x06006F62 RID: 28514 RVA: 0x0023E570 File Offset: 0x0023C770
		public static void SerializeItemsAsJSON(StoreItem[] items)
		{
			string text = "";
			foreach (StoreItem storeItem in items)
			{
				text = text + JsonUtility.ToJson(storeItem) + ";";
			}
			Debug.LogError(text);
			File.WriteAllText(Application.dataPath + "/Resources/StoreItems/FeaturedStoreItemsList.json", text);
		}

		// Token: 0x06006F63 RID: 28515 RVA: 0x0023E5C4 File Offset: 0x0023C7C4
		public static void ConvertCosmeticItemToSToreItem(CosmeticsController.CosmeticItem cosmeticItem, ref StoreItem storeItem)
		{
			storeItem.itemName = cosmeticItem.itemName;
			storeItem.itemCategory = (int)cosmeticItem.itemCategory;
			storeItem.itemPictureResourceString = cosmeticItem.itemPictureResourceString;
			storeItem.displayName = cosmeticItem.displayName;
			storeItem.overrideDisplayName = cosmeticItem.overrideDisplayName;
			storeItem.bundledItems = cosmeticItem.bundledItems;
			storeItem.canTryOn = cosmeticItem.canTryOn;
			storeItem.bothHandsHoldable = cosmeticItem.bothHandsHoldable;
			storeItem.AssetBundleName = "";
			storeItem.bUsesMeshAtlas = cosmeticItem.bUsesMeshAtlas;
			storeItem.MeshResourceName = cosmeticItem.meshResourceString;
			storeItem.MeshAtlasResourceName = cosmeticItem.meshAtlasResourceString;
			storeItem.MaterialResrouceName = cosmeticItem.materialResourceString;
		}

		// Token: 0x04007F55 RID: 32597
		public string itemName = "";

		// Token: 0x04007F56 RID: 32598
		public int itemCategory;

		// Token: 0x04007F57 RID: 32599
		public string itemPictureResourceString = "";

		// Token: 0x04007F58 RID: 32600
		public string displayName = "";

		// Token: 0x04007F59 RID: 32601
		public string overrideDisplayName = "";

		// Token: 0x04007F5A RID: 32602
		public string[] bundledItems = new string[0];

		// Token: 0x04007F5B RID: 32603
		public bool canTryOn;

		// Token: 0x04007F5C RID: 32604
		public bool bothHandsHoldable;

		// Token: 0x04007F5D RID: 32605
		public string AssetBundleName = "";

		// Token: 0x04007F5E RID: 32606
		public bool bUsesMeshAtlas;

		// Token: 0x04007F5F RID: 32607
		public string MeshAtlasResourceName = "";

		// Token: 0x04007F60 RID: 32608
		public string MeshResourceName = "";

		// Token: 0x04007F61 RID: 32609
		public string MaterialResrouceName = "";

		// Token: 0x04007F62 RID: 32610
		public Vector3 translationOffset = Vector3.zero;

		// Token: 0x04007F63 RID: 32611
		public Vector3 rotationOffset = Vector3.zero;

		// Token: 0x04007F64 RID: 32612
		public Vector3 scale = Vector3.one;
	}
}
