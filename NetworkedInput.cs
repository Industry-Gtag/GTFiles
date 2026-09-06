using System;
using System.Runtime.InteropServices;
using Fusion;
using UnityEngine;

// Token: 0x0200043D RID: 1085
[NetworkInputWeaved(35)]
[StructLayout(LayoutKind.Explicit, Size = 140)]
public struct NetworkedInput : INetworkInput
{
	// Token: 0x04002498 RID: 9368
	[FieldOffset(0)]
	public Quaternion headRot_LS;

	// Token: 0x04002499 RID: 9369
	[FieldOffset(16)]
	public Vector3 rightHandPos_LS;

	// Token: 0x0400249A RID: 9370
	[FieldOffset(28)]
	public Quaternion rightHandRot_LS;

	// Token: 0x0400249B RID: 9371
	[FieldOffset(44)]
	public Vector3 leftHandPos_LS;

	// Token: 0x0400249C RID: 9372
	[FieldOffset(56)]
	public Quaternion leftHandRot_LS;

	// Token: 0x0400249D RID: 9373
	[FieldOffset(72)]
	public Vector3 rootPosition;

	// Token: 0x0400249E RID: 9374
	[FieldOffset(84)]
	public Quaternion rootRotation;

	// Token: 0x0400249F RID: 9375
	[FieldOffset(100)]
	public bool leftThumbTouch;

	// Token: 0x040024A0 RID: 9376
	[FieldOffset(104)]
	public bool leftThumbPress;

	// Token: 0x040024A1 RID: 9377
	[FieldOffset(108)]
	public float leftIndexValue;

	// Token: 0x040024A2 RID: 9378
	[FieldOffset(112)]
	public float leftMiddleValue;

	// Token: 0x040024A3 RID: 9379
	[FieldOffset(116)]
	public bool rightThumbTouch;

	// Token: 0x040024A4 RID: 9380
	[FieldOffset(120)]
	public bool rightThumbPress;

	// Token: 0x040024A5 RID: 9381
	[FieldOffset(124)]
	public float rightIndexValue;

	// Token: 0x040024A6 RID: 9382
	[FieldOffset(128)]
	public float rightMiddleValue;

	// Token: 0x040024A7 RID: 9383
	[FieldOffset(132)]
	public float scale;

	// Token: 0x040024A8 RID: 9384
	[FieldOffset(136)]
	public int handPoseData;
}
