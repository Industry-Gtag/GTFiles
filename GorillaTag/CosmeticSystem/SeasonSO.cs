using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x0200128D RID: 4749
	[CreateAssetMenu(fileName = "UntitledSeason_SeasonSO", menuName = "- Gorilla Tag/SeasonSO", order = 0)]
	public class SeasonSO : ScriptableObject
	{
		// Token: 0x040087B3 RID: 34739
		[Delayed]
		public GTDateTimeSerializable releaseDate = new GTDateTimeSerializable(1);

		// Token: 0x040087B4 RID: 34740
		[Delayed]
		public string seasonName;
	}
}
