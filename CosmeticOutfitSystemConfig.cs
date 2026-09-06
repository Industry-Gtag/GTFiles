using System;
using UnityEngine;

// Token: 0x0200057E RID: 1406
[CreateAssetMenu(fileName = "CosmeticOutfitSystemConfig", menuName = "Gorilla Tag/Cosmetics/OutfitSystem", order = 0)]
public class CosmeticOutfitSystemConfig : ScriptableObject
{
	// Token: 0x04002EED RID: 12013
	public int nonSubscriberMaxOutfits = 5;

	// Token: 0x04002EEE RID: 12014
	public int subscriberMaxOutfits = 10;

	// Token: 0x04002EEF RID: 12015
	public string mothershipKey;

	// Token: 0x04002EF0 RID: 12016
	public char outfitSeparator;

	// Token: 0x04002EF1 RID: 12017
	public char itemSeparator;

	// Token: 0x04002EF2 RID: 12018
	public string selectedOutfitPref;
}
