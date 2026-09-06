using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x0200119C RID: 4508
	public class WaterSplashOverride : MonoBehaviour
	{
		// Token: 0x0400826B RID: 33387
		public bool suppressWaterEffects;

		// Token: 0x0400826C RID: 33388
		public bool playBigSplash;

		// Token: 0x0400826D RID: 33389
		public bool playDrippingEffect = true;

		// Token: 0x0400826E RID: 33390
		public bool scaleByPlayersScale;

		// Token: 0x0400826F RID: 33391
		public bool overrideBoundingRadius;

		// Token: 0x04008270 RID: 33392
		public float boundingRadiusOverride = 1f;
	}
}
