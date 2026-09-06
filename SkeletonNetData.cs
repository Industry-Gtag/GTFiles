using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020001D3 RID: 467
[NetworkStructWeaved(11)]
[StructLayout(LayoutKind.Explicit, Size = 44)]
public struct SkeletonNetData : INetworkStruct
{
	// Token: 0x1700012A RID: 298
	// (get) Token: 0x06000C68 RID: 3176 RVA: 0x00044139 File Offset: 0x00042339
	// (set) Token: 0x06000C69 RID: 3177 RVA: 0x00044141 File Offset: 0x00042341
	public int CurrentState { readonly get; set; }

	// Token: 0x1700012B RID: 299
	// (get) Token: 0x06000C6A RID: 3178 RVA: 0x0004414A File Offset: 0x0004234A
	// (set) Token: 0x06000C6B RID: 3179 RVA: 0x0004415C File Offset: 0x0004235C
	[Networked]
	[NetworkedWeaved(1, 3)]
	public unsafe Vector3 Position
	{
		readonly get
		{
			return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._Position);
		}
		set
		{
			*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._Position) = value;
		}
	}

	// Token: 0x1700012C RID: 300
	// (get) Token: 0x06000C6C RID: 3180 RVA: 0x0004416F File Offset: 0x0004236F
	// (set) Token: 0x06000C6D RID: 3181 RVA: 0x00044181 File Offset: 0x00042381
	[Networked]
	[NetworkedWeaved(4, 4)]
	public unsafe Quaternion Rotation
	{
		readonly get
		{
			return *(Quaternion*)Native.ReferenceToPointer<FixedStorage@4>(ref this._Rotation);
		}
		set
		{
			*(Quaternion*)Native.ReferenceToPointer<FixedStorage@4>(ref this._Rotation) = value;
		}
	}

	// Token: 0x1700012D RID: 301
	// (get) Token: 0x06000C6E RID: 3182 RVA: 0x00044194 File Offset: 0x00042394
	// (set) Token: 0x06000C6F RID: 3183 RVA: 0x0004419C File Offset: 0x0004239C
	public int CurrentNode { readonly get; set; }

	// Token: 0x1700012E RID: 302
	// (get) Token: 0x06000C70 RID: 3184 RVA: 0x000441A5 File Offset: 0x000423A5
	// (set) Token: 0x06000C71 RID: 3185 RVA: 0x000441AD File Offset: 0x000423AD
	public int NextNode { readonly get; set; }

	// Token: 0x1700012F RID: 303
	// (get) Token: 0x06000C72 RID: 3186 RVA: 0x000441B6 File Offset: 0x000423B6
	// (set) Token: 0x06000C73 RID: 3187 RVA: 0x000441BE File Offset: 0x000423BE
	public int AngerPoint { readonly get; set; }

	// Token: 0x06000C74 RID: 3188 RVA: 0x000441C7 File Offset: 0x000423C7
	public SkeletonNetData(int state, Vector3 pos, Quaternion rot, int cNode, int nNode, int angerPoint)
	{
		this.CurrentState = state;
		this.Position = pos;
		this.Rotation = rot;
		this.CurrentNode = cNode;
		this.NextNode = nNode;
		this.AngerPoint = angerPoint;
	}

	// Token: 0x04000F1D RID: 3869
	[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ElementReaderWriterVector3), 0, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@3 _Position;

	// Token: 0x04000F1E RID: 3870
	[FixedBufferProperty(typeof(Quaternion), typeof(UnityValueSurrogate@ReaderWriter@UnityEngine_Quaternion), 0, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(16)]
	private FixedStorage@4 _Rotation;
}
