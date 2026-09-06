using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02001284 RID: 4740
	[CreateAssetMenu(fileName = "Untitled_CosmeticSO", menuName = "- Gorilla Tag/CosmeticSO", order = 0)]
	public class CosmeticSO : ScriptableObject
	{
		// Token: 0x0600777A RID: 30586 RVA: 0x00023F0C File Offset: 0x0002210C
		private bool ShowPropHuntWeight()
		{
			return true;
		}

		// Token: 0x0600777B RID: 30587 RVA: 0x0026B773 File Offset: 0x00269973
		public void OnEnable()
		{
			this.info.debugCosmeticSOName = base.name;
		}

		// Token: 0x04008745 RID: 34629
		public CosmeticInfoV2 info = new CosmeticInfoV2("UNNAMED");

		// Token: 0x04008746 RID: 34630
		public int propHuntWeight = 1;
	}
}
