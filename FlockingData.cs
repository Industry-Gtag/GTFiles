using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020006A3 RID: 1699
[NetworkStructWeaved(337)]
[StructLayout(LayoutKind.Explicit, Size = 1348)]
public struct FlockingData : INetworkStruct
{
	// Token: 0x17000440 RID: 1088
	// (get) Token: 0x06002A69 RID: 10857 RVA: 0x000E4E3C File Offset: 0x000E303C
	// (set) Token: 0x06002A6A RID: 10858 RVA: 0x000E4E44 File Offset: 0x000E3044
	public int count { readonly get; set; }

	// Token: 0x17000441 RID: 1089
	// (get) Token: 0x06002A6B RID: 10859 RVA: 0x000E4E50 File Offset: 0x000E3050
	[Networked]
	[Capacity(30)]
	[NetworkedWeavedLinkedList(30, 3, typeof(ElementReaderWriterVector3))]
	[NetworkedWeaved(1, 153)]
	public NetworkLinkedList<Vector3> Positions
	{
		get
		{
			return new NetworkLinkedList<Vector3>(Native.ReferenceToPointer<FixedStorage@153>(ref this._Positions), 30, ElementReaderWriterVector3.GetInstance());
		}
	}

	// Token: 0x17000442 RID: 1090
	// (get) Token: 0x06002A6C RID: 10860 RVA: 0x000E4E78 File Offset: 0x000E3078
	[Networked]
	[Capacity(30)]
	[NetworkedWeavedLinkedList(30, 4, typeof(ReaderWriter@UnityEngine_Quaternion))]
	[NetworkedWeaved(154, 183)]
	public NetworkLinkedList<Quaternion> Rotations
	{
		get
		{
			return new NetworkLinkedList<Quaternion>(Native.ReferenceToPointer<FixedStorage@183>(ref this._Rotations), 30, ReaderWriter@UnityEngine_Quaternion.GetInstance());
		}
	}

	// Token: 0x06002A6D RID: 10861 RVA: 0x000E4EA0 File Offset: 0x000E30A0
	public FlockingData(List<Flocking> items)
	{
		this.count = items.Count;
		foreach (Flocking flocking in items)
		{
			this.Positions.Add(flocking.pos);
			this.Rotations.Add(flocking.rot);
		}
	}

	// Token: 0x04003738 RID: 14136
	[FixedBufferProperty(typeof(NetworkLinkedList<Vector3>), typeof(UnityLinkedListSurrogate@ElementReaderWriterVector3), 30, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@153 _Positions;

	// Token: 0x04003739 RID: 14137
	[FixedBufferProperty(typeof(NetworkLinkedList<Quaternion>), typeof(UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Quaternion), 30, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(616)]
	private FixedStorage@183 _Rotations;
}
