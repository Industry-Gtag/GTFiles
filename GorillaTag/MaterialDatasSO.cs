using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020011F8 RID: 4600
	[CreateAssetMenu(fileName = "MaterialDatasSO", menuName = "Gorilla Tag/MaterialDatasSO")]
	public class MaterialDatasSO : ScriptableObject
	{
		// Token: 0x0400846B RID: 33899
		public List<GTPlayer.MaterialData> datas;

		// Token: 0x0400846C RID: 33900
		public List<HashWrapper> surfaceEffects;
	}
}
