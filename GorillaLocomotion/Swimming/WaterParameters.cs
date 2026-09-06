using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x0200119B RID: 4507
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WaterParameters", order = 1)]
	public class WaterParameters : ScriptableObject
	{
		// Token: 0x04008255 RID: 33365
		[Header("Splash Effect")]
		public bool playSplashEffect = true;

		// Token: 0x04008256 RID: 33366
		public GameObject splashEffect;

		// Token: 0x04008257 RID: 33367
		public float splashEffectScale = 1f;

		// Token: 0x04008258 RID: 33368
		public bool sendSplashEffectRPCs;

		// Token: 0x04008259 RID: 33369
		public float splashSpeedRequirement = 0.8f;

		// Token: 0x0400825A RID: 33370
		public float bigSplashSpeedRequirement = 1.9f;

		// Token: 0x0400825B RID: 33371
		public Gradient splashColorBySpeedGradient;

		// Token: 0x0400825C RID: 33372
		[Header("Ripple Effect")]
		public bool playRippleEffect = true;

		// Token: 0x0400825D RID: 33373
		public GameObject rippleEffect;

		// Token: 0x0400825E RID: 33374
		public float rippleEffectScale = 1f;

		// Token: 0x0400825F RID: 33375
		public float defaultDistanceBetweenRipples = 0.75f;

		// Token: 0x04008260 RID: 33376
		public float minDistanceBetweenRipples = 0.2f;

		// Token: 0x04008261 RID: 33377
		public float minTimeBetweenRipples = 0.75f;

		// Token: 0x04008262 RID: 33378
		public Color rippleSpriteColor = Color.white;

		// Token: 0x04008263 RID: 33379
		[Header("Drip Effect")]
		public bool playDripEffect = true;

		// Token: 0x04008264 RID: 33380
		public float postExitDripDuration = 1.5f;

		// Token: 0x04008265 RID: 33381
		public float perDripTimeDelay = 0.2f;

		// Token: 0x04008266 RID: 33382
		public float perDripTimeRandRange = 0.15f;

		// Token: 0x04008267 RID: 33383
		public float perDripDefaultRadius = 0.01f;

		// Token: 0x04008268 RID: 33384
		public float perDripRadiusRandRange = 0.01f;

		// Token: 0x04008269 RID: 33385
		[Header("Misc")]
		public float recomputeSurfaceForColliderDist = 0.2f;

		// Token: 0x0400826A RID: 33386
		public bool allowBubblesInVolume;
	}
}
