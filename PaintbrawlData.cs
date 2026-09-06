using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020005C8 RID: 1480
[NetworkStructWeaved(61)]
[StructLayout(LayoutKind.Explicit, Size = 244)]
public struct PaintbrawlData : INetworkStruct
{
	// Token: 0x170003E7 RID: 999
	// (get) Token: 0x06002548 RID: 9544 RVA: 0x000C79B8 File Offset: 0x000C5BB8
	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(1, 20)]
	public NetworkArray<int> playerLivesArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@20>(ref this._playerLivesArray), 20, ElementReaderWriterInt32.GetInstance());
		}
	}

	// Token: 0x170003E8 RID: 1000
	// (get) Token: 0x06002549 RID: 9545 RVA: 0x000C79E0 File Offset: 0x000C5BE0
	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(21, 20)]
	public NetworkArray<int> playerActorNumberArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@20>(ref this._playerActorNumberArray), 20, ElementReaderWriterInt32.GetInstance());
		}
	}

	// Token: 0x170003E9 RID: 1001
	// (get) Token: 0x0600254A RID: 9546 RVA: 0x000C7A08 File Offset: 0x000C5C08
	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus))]
	[NetworkedWeaved(41, 20)]
	public NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus> playerStatusArray
	{
		get
		{
			return new NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus>(Native.ReferenceToPointer<FixedStorage@20>(ref this._playerStatusArray), 20, ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus.GetInstance());
		}
	}

	// Token: 0x040030DC RID: 12508
	[FieldOffset(0)]
	public GorillaPaintbrawlManager.PaintbrawlState currentPaintbrawlState;

	// Token: 0x040030DD RID: 12509
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@20 _playerLivesArray;

	// Token: 0x040030DE RID: 12510
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(84)]
	private FixedStorage@20 _playerActorNumberArray;

	// Token: 0x040030DF RID: 12511
	[FixedBufferProperty(typeof(NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus>), typeof(UnityArraySurrogate@ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(164)]
	private FixedStorage@20 _playerStatusArray;
}
