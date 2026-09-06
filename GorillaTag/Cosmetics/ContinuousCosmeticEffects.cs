using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001303 RID: 4867
	public class ContinuousCosmeticEffects : MonoBehaviour
	{
		// Token: 0x06007A29 RID: 31273 RVA: 0x0027D9B2 File Offset: 0x0027BBB2
		public void ApplyAll(float f)
		{
			this.continuousProperties.ApplyAll(f);
		}

		// Token: 0x04008B97 RID: 35735
		[FormerlySerializedAs("properties")]
		[SerializeField]
		private ContinuousPropertyArray continuousProperties;
	}
}
