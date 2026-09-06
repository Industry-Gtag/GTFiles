using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02001193 RID: 4499
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerSwimmingParameters", order = 1)]
	public class PlayerSwimmingParameters : ScriptableObject
	{
		// Token: 0x040081DB RID: 33243
		[Header("Base Settings")]
		public float floatingWaterLevelBelowHead = 0.6f;

		// Token: 0x040081DC RID: 33244
		public float buoyancyFadeDist = 0.3f;

		// Token: 0x040081DD RID: 33245
		public bool extendBouyancyFromSpeed;

		// Token: 0x040081DE RID: 33246
		public float buoyancyExtensionDecayHalflife = 0.2f;

		// Token: 0x040081DF RID: 33247
		public float baseUnderWaterDampingHalfLife = 0.25f;

		// Token: 0x040081E0 RID: 33248
		public float swimUnderWaterDampingHalfLife = 1.1f;

		// Token: 0x040081E1 RID: 33249
		public AnimationCurve speedToBouyancyExtension = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040081E2 RID: 33250
		public Vector2 speedToBouyancyExtensionMinMax = Vector2.zero;

		// Token: 0x040081E3 RID: 33251
		public float swimmingVelocityOutOfWaterDrainRate = 3f;

		// Token: 0x040081E4 RID: 33252
		[Range(0f, 1f)]
		public float underwaterJumpsAsSwimVelocityFactor = 1f;

		// Token: 0x040081E5 RID: 33253
		[Range(0f, 1f)]
		public float swimmingHapticsStrength = 0.5f;

		// Token: 0x040081E6 RID: 33254
		[Header("Surface Jumping")]
		public bool allowWaterSurfaceJumps;

		// Token: 0x040081E7 RID: 33255
		public float waterSurfaceJumpHandSpeedThreshold = 1f;

		// Token: 0x040081E8 RID: 33256
		public float waterSurfaceJumpAmount;

		// Token: 0x040081E9 RID: 33257
		public float waterSurfaceJumpMaxSpeed = 1f;

		// Token: 0x040081EA RID: 33258
		public AnimationCurve waterSurfaceJumpPalmFacingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040081EB RID: 33259
		public AnimationCurve waterSurfaceJumpHandVelocityFacingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040081EC RID: 33260
		[Header("Diving")]
		public bool applyDiveSteering;

		// Token: 0x040081ED RID: 33261
		public bool applyDiveDampingMultiplier;

		// Token: 0x040081EE RID: 33262
		public float diveDampingMultiplier = 1f;

		// Token: 0x040081EF RID: 33263
		[Tooltip("In degrees")]
		public float maxDiveSteerAnglePerStep = 1f;

		// Token: 0x040081F0 RID: 33264
		public float diveVelocityAveragingWindow = 0.1f;

		// Token: 0x040081F1 RID: 33265
		public bool applyDiveSwimVelocityConversion;

		// Token: 0x040081F2 RID: 33266
		[Tooltip("In meters per second")]
		public float diveSwimVelocityConversionRate = 3f;

		// Token: 0x040081F3 RID: 33267
		public float diveMaxSwimVelocityConversion = 3f;

		// Token: 0x040081F4 RID: 33268
		public bool reduceDiveSteeringBelowVelocityPlane;

		// Token: 0x040081F5 RID: 33269
		public float reduceDiveSteeringBelowPlaneFadeStartDist = 0.4f;

		// Token: 0x040081F6 RID: 33270
		public float reduceDiveSteeringBelowPlaneFadeEndDist = 0.55f;

		// Token: 0x040081F7 RID: 33271
		public AnimationCurve palmFacingToRedirectAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040081F8 RID: 33272
		public Vector2 palmFacingToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x040081F9 RID: 33273
		public AnimationCurve swimSpeedToRedirectAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040081FA RID: 33274
		public Vector2 swimSpeedToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x040081FB RID: 33275
		public AnimationCurve swimSpeedToMaxRedirectAngle = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040081FC RID: 33276
		public Vector2 swimSpeedToMaxRedirectAngleMinMax = Vector2.zero;

		// Token: 0x040081FD RID: 33277
		public AnimationCurve handSpeedToRedirectAmount = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		// Token: 0x040081FE RID: 33278
		public Vector2 handSpeedToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x040081FF RID: 33279
		public AnimationCurve handAccelToRedirectAmount = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		// Token: 0x04008200 RID: 33280
		public Vector2 handAccelToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x04008201 RID: 33281
		public AnimationCurve nonDiveDampingHapticsAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04008202 RID: 33282
		public Vector2 nonDiveDampingHapticsAmountMinMax = Vector2.zero;
	}
}
