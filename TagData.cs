using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020005D0 RID: 1488
[NetworkStructWeaved(22)]
[StructLayout(LayoutKind.Explicit, Size = 88)]
public struct TagData : INetworkStruct
{
	// Token: 0x170003F3 RID: 1011
	// (get) Token: 0x06002574 RID: 9588 RVA: 0x000C7F28 File Offset: 0x000C6128
	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(2, 20)]
	public NetworkArray<int> infectedPlayerList
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@20>(ref this._infectedPlayerList), 20, ElementReaderWriterInt32.GetInstance());
		}
	}

	// Token: 0x170003F4 RID: 1012
	// (get) Token: 0x06002575 RID: 9589 RVA: 0x000C7F4F File Offset: 0x000C614F
	// (set) Token: 0x06002576 RID: 9590 RVA: 0x000C7F57 File Offset: 0x000C6157
	public int currentItID { readonly get; set; }

	// Token: 0x040030EC RID: 12524
	[FieldOffset(4)]
	public NetworkBool isCurrentlyTag;

	// Token: 0x040030ED RID: 12525
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(8)]
	private FixedStorage@20 _infectedPlayerList;
}
