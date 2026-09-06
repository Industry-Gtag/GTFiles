using System;
using System.Runtime.InteropServices;
using Fusion;

// Token: 0x02000455 RID: 1109
[NetworkStructWeaved(1)]
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 4)]
public struct HitTargetStruct : INetworkStruct
{
	// Token: 0x06001A7A RID: 6778 RVA: 0x00094674 File Offset: 0x00092874
	public HitTargetStruct(int v)
	{
		this.Score = v;
	}

	// Token: 0x04002554 RID: 9556
	[FieldOffset(0)]
	public int Score;
}
