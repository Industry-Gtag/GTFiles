using System;
using System.Runtime.InteropServices;
using Fusion;
using UnityEngine;

// Token: 0x02000454 RID: 1108
[NetworkStructWeaved(45)]
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 180)]
public struct InputStruct : INetworkStruct
{
	// Token: 0x04002532 RID: 9522
	[FieldOffset(0)]
	public int headRotation;

	// Token: 0x04002533 RID: 9523
	[FieldOffset(4)]
	public bool usingNewIK;

	// Token: 0x04002534 RID: 9524
	[FieldOffset(8)]
	public int bodyRotation;

	// Token: 0x04002535 RID: 9525
	[FieldOffset(12)]
	public short leftUpperArmRotation;

	// Token: 0x04002536 RID: 9526
	[FieldOffset(16)]
	public short rightUpperArmRotation;

	// Token: 0x04002537 RID: 9527
	[FieldOffset(20)]
	public long rightHandLong;

	// Token: 0x04002538 RID: 9528
	[FieldOffset(28)]
	public long leftHandLong;

	// Token: 0x04002539 RID: 9529
	[FieldOffset(36)]
	public long position;

	// Token: 0x0400253A RID: 9530
	[FieldOffset(44)]
	public int handPosition;

	// Token: 0x0400253B RID: 9531
	[FieldOffset(48)]
	public int rotation;

	// Token: 0x0400253C RID: 9532
	[FieldOffset(52)]
	public int packedFields;

	// Token: 0x0400253D RID: 9533
	[FieldOffset(56)]
	public short packedCompetitiveData;

	// Token: 0x0400253E RID: 9534
	[FieldOffset(60)]
	public Vector3 velocity;

	// Token: 0x0400253F RID: 9535
	[FieldOffset(72)]
	public int grabbedRopeIndex;

	// Token: 0x04002540 RID: 9536
	[FieldOffset(76)]
	public int ropeBoneIndex;

	// Token: 0x04002541 RID: 9537
	[FieldOffset(80)]
	public bool ropeGrabIsLeft;

	// Token: 0x04002542 RID: 9538
	[FieldOffset(84)]
	public bool ropeGrabIsBody;

	// Token: 0x04002543 RID: 9539
	[FieldOffset(88)]
	public Vector3 ropeGrabOffset;

	// Token: 0x04002544 RID: 9540
	[FieldOffset(100)]
	public bool movingSurfaceIsMonkeBlock;

	// Token: 0x04002545 RID: 9541
	[FieldOffset(104)]
	public long hoverboardPosRot;

	// Token: 0x04002546 RID: 9542
	[FieldOffset(112)]
	public short hoverboardColor;

	// Token: 0x04002547 RID: 9543
	[FieldOffset(116)]
	public long propHuntPosRot;

	// Token: 0x04002548 RID: 9544
	[FieldOffset(124)]
	public double serverTimeStamp;

	// Token: 0x04002549 RID: 9545
	[FieldOffset(132)]
	public short taggedById;

	// Token: 0x0400254A RID: 9546
	[FieldOffset(136)]
	public bool isGroundedHand;

	// Token: 0x0400254B RID: 9547
	[FieldOffset(140)]
	public bool isGroundedButt;

	// Token: 0x0400254C RID: 9548
	[FieldOffset(144)]
	public int leftHandGrabbedActorNumber;

	// Token: 0x0400254D RID: 9549
	[FieldOffset(148)]
	public bool leftGrabbedHandIsLeft;

	// Token: 0x0400254E RID: 9550
	[FieldOffset(152)]
	public int rightHandGrabbedActorNumber;

	// Token: 0x0400254F RID: 9551
	[FieldOffset(156)]
	public bool rightGrabbedHandIsLeft;

	// Token: 0x04002550 RID: 9552
	[FieldOffset(160)]
	public float lastTouchedGroundAtTime;

	// Token: 0x04002551 RID: 9553
	[FieldOffset(164)]
	public float lastHandTouchedGroundAtTime;

	// Token: 0x04002552 RID: 9554
	[FieldOffset(168)]
	public long packedGTPlayerStats;

	// Token: 0x04002553 RID: 9555
	[FieldOffset(176)]
	public int gtPlayerStatsFlags;
}
