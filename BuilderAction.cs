using System;
using UnityEngine;

// Token: 0x02000628 RID: 1576
public struct BuilderAction
{
	// Token: 0x040032D2 RID: 13010
	public BuilderActionType type;

	// Token: 0x040032D3 RID: 13011
	public int pieceId;

	// Token: 0x040032D4 RID: 13012
	public int parentPieceId;

	// Token: 0x040032D5 RID: 13013
	public Vector3 localPosition;

	// Token: 0x040032D6 RID: 13014
	public Quaternion localRotation;

	// Token: 0x040032D7 RID: 13015
	public byte twist;

	// Token: 0x040032D8 RID: 13016
	public sbyte bumpOffsetx;

	// Token: 0x040032D9 RID: 13017
	public sbyte bumpOffsetz;

	// Token: 0x040032DA RID: 13018
	public bool isLeftHand;

	// Token: 0x040032DB RID: 13019
	public int playerActorNumber;

	// Token: 0x040032DC RID: 13020
	public int parentAttachIndex;

	// Token: 0x040032DD RID: 13021
	public int attachIndex;

	// Token: 0x040032DE RID: 13022
	public SnapBounds attachBounds;

	// Token: 0x040032DF RID: 13023
	public SnapBounds parentAttachBounds;

	// Token: 0x040032E0 RID: 13024
	public Vector3 velocity;

	// Token: 0x040032E1 RID: 13025
	public Vector3 angVelocity;

	// Token: 0x040032E2 RID: 13026
	public int localCommandId;

	// Token: 0x040032E3 RID: 13027
	public int timeStamp;
}
