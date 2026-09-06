using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020005CE RID: 1486
[NetworkStructWeaved(43)]
[StructLayout(LayoutKind.Explicit, Size = 172)]
public struct HuntData : INetworkStruct
{
	// Token: 0x170003EF RID: 1007
	// (get) Token: 0x0600256B RID: 9579 RVA: 0x000C7E3C File Offset: 0x000C603C
	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(3, 20)]
	public NetworkArray<int> currentHuntedArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@20>(ref this._currentHuntedArray), 20, ElementReaderWriterInt32.GetInstance());
		}
	}

	// Token: 0x170003F0 RID: 1008
	// (get) Token: 0x0600256C RID: 9580 RVA: 0x000C7E64 File Offset: 0x000C6064
	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(23, 20)]
	public NetworkArray<int> currentTargetArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@20>(ref this._currentTargetArray), 20, ElementReaderWriterInt32.GetInstance());
		}
	}

	// Token: 0x040030E5 RID: 12517
	[FieldOffset(0)]
	public NetworkBool huntStarted;

	// Token: 0x040030E6 RID: 12518
	[FieldOffset(4)]
	public NetworkBool waitingToStartNextHuntGame;

	// Token: 0x040030E7 RID: 12519
	[FieldOffset(8)]
	public int countDownTime;

	// Token: 0x040030E8 RID: 12520
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(12)]
	private FixedStorage@20 _currentHuntedArray;

	// Token: 0x040030E9 RID: 12521
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(92)]
	private FixedStorage@20 _currentTargetArray;
}
