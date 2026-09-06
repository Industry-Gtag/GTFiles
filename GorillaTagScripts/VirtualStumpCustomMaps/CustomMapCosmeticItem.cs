using System;
using GorillaNetworking.Store;
using GT_CustomMapSupportRuntime;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000FC7 RID: 4039
	[Serializable]
	public struct CustomMapCosmeticItem
	{
		// Token: 0x0400732A RID: 29482
		public GTObjectPlaceholder.ECustomMapCosmeticItem customMapItemSlot;

		// Token: 0x0400732B RID: 29483
		public HeadModel_CosmeticStand.BustType bustType;

		// Token: 0x0400732C RID: 29484
		public string playFabID;
	}
}
