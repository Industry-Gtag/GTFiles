using System;
using System.Runtime.InteropServices;
using Fusion;

// Token: 0x0200021F RID: 543
[NetworkStructWeaved(3)]
[StructLayout(LayoutKind.Explicit, Size = 12)]
public struct BeeSwarmData : INetworkStruct
{
	// Token: 0x1700014B RID: 331
	// (get) Token: 0x06000E4A RID: 3658 RVA: 0x0004E6CC File Offset: 0x0004C8CC
	// (set) Token: 0x06000E4B RID: 3659 RVA: 0x0004E6D4 File Offset: 0x0004C8D4
	public int TargetActorNumber { readonly get; set; }

	// Token: 0x1700014C RID: 332
	// (get) Token: 0x06000E4C RID: 3660 RVA: 0x0004E6DD File Offset: 0x0004C8DD
	// (set) Token: 0x06000E4D RID: 3661 RVA: 0x0004E6E5 File Offset: 0x0004C8E5
	public int CurrentState { readonly get; set; }

	// Token: 0x1700014D RID: 333
	// (get) Token: 0x06000E4E RID: 3662 RVA: 0x0004E6EE File Offset: 0x0004C8EE
	// (set) Token: 0x06000E4F RID: 3663 RVA: 0x0004E6F6 File Offset: 0x0004C8F6
	public float CurrentSpeed { readonly get; set; }

	// Token: 0x06000E50 RID: 3664 RVA: 0x0004E6FF File Offset: 0x0004C8FF
	public BeeSwarmData(int actorNr, int state, float speed)
	{
		this.TargetActorNumber = actorNr;
		this.CurrentState = state;
		this.CurrentSpeed = speed;
	}
}
