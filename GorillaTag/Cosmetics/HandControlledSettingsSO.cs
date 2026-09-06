using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001343 RID: 4931
	public class HandControlledSettingsSO : ScriptableObject
	{
		// Token: 0x17000C20 RID: 3104
		// (get) Token: 0x06007BB1 RID: 31665 RVA: 0x00286B90 File Offset: 0x00284D90
		private bool IsAngle
		{
			get
			{
				return this.rotationControl == HandControlledCosmetic.RotationControl.Angle;
			}
		}

		// Token: 0x17000C21 RID: 3105
		// (get) Token: 0x06007BB2 RID: 31666 RVA: 0x00286B9B File Offset: 0x00284D9B
		private bool IsTranslation
		{
			get
			{
				return this.rotationControl == HandControlledCosmetic.RotationControl.Translation;
			}
		}

		// Token: 0x04008DC0 RID: 36288
		private const string SENS_TT = "The difference between the current input and cached input is magnified by this number.";

		// Token: 0x04008DC1 RID: 36289
		public HandControlledCosmetic.RotationControl rotationControl;

		// Token: 0x04008DC2 RID: 36290
		[Tooltip("The difference between the current input and cached input is magnified by this number.")]
		public float inputSensitivity = 2f;

		// Token: 0x04008DC3 RID: 36291
		[Tooltip("The difference between the current input and cached input is magnified by this number.")]
		public AnimationCurve verticalSensitivity = AnimationCurve.Constant(0f, 1f, 2f);

		// Token: 0x04008DC4 RID: 36292
		[Tooltip("The difference between the current input and cached input is magnified by this number.")]
		public AnimationCurve horizontalSensitivity = AnimationCurve.Constant(0f, 1f, 2f);

		// Token: 0x04008DC5 RID: 36293
		[Tooltip("How quickly the cached input approaches the current input. A high value will function more like a mouse, while a low value will function more like a joystick.")]
		public float inputDecaySpeed = 1f;

		// Token: 0x04008DC6 RID: 36294
		[Tooltip("How quickly the cached input approaches the current input, as a function of distance. A high value will function more like a mouse, while a low value will function more like a joystick.")]
		public AnimationCurve inputDecayCurve = AnimationCurve.Constant(0f, 2f, 1f);

		// Token: 0x04008DC7 RID: 36295
		[Tooltip("How quickly the transform approaches the intended angle (smaller value = more lag).")]
		public float rotationSpeed = 20f;

		// Token: 0x04008DC8 RID: 36296
		[Tooltip("The transform's local rotation cannot exceed these euler angles.")]
		public Vector3 angleLimits = new Vector3(45f, 360f, 0f);
	}
}
