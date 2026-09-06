using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02001142 RID: 4418
	public class StoreBundleData : ScriptableObject
	{
		// Token: 0x06006EDF RID: 28383 RVA: 0x0023B390 File Offset: 0x00239590
		public void OnValidate()
		{
			if (this.playfabBundleID.Contains(' '))
			{
				Debug.LogError("ERROR THERE IS A SPACE IN THE PLAYFAB BUNDLE ID " + base.name);
			}
			if (this.bundleSKU.Contains(' '))
			{
				Debug.LogError("ERROR THERE IS A SPACE IN THE BUNDLE SKU " + base.name);
			}
		}

		// Token: 0x04007EEB RID: 32491
		public string playfabBundleID = "NULL";

		// Token: 0x04007EEC RID: 32492
		public string bundleSKU = "NULL SKU";

		// Token: 0x04007EED RID: 32493
		public NexusCreatorCode creatorCode;

		// Token: 0x04007EEE RID: 32494
		public Sprite bundleImage;

		// Token: 0x04007EEF RID: 32495
		public string bundleDescriptionText = "THE NULL_BUNDLE PACK WITH 10,000 SHINY ROCKS IN THIS LIMITED TIME DLC!";
	}
}
