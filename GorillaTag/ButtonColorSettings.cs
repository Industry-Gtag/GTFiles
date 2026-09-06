using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200121B RID: 4635
	[CreateAssetMenu(fileName = "GorillaButtonColorSettings", menuName = "ScriptableObjects/GorillaButtonColorSettings", order = 0)]
	public class ButtonColorSettings : ScriptableObject
	{
		// Token: 0x0400857A RID: 34170
		public Color UnpressedColor;

		// Token: 0x0400857B RID: 34171
		public Color PressedColor;

		// Token: 0x0400857C RID: 34172
		[Tooltip("Optional\nThe time the change will be in effect")]
		public float PressedTime;
	}
}
