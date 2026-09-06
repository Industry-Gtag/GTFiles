using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020001CB RID: 459
[NetworkStructWeaved(21)]
[StructLayout(LayoutKind.Explicit, Size = 84)]
public struct GhostLabData : INetworkStruct
{
	// Token: 0x17000127 RID: 295
	// (get) Token: 0x06000C24 RID: 3108 RVA: 0x000421ED File Offset: 0x000403ED
	// (set) Token: 0x06000C25 RID: 3109 RVA: 0x000421F5 File Offset: 0x000403F5
	public int DoorState { readonly get; set; }

	// Token: 0x17000128 RID: 296
	// (get) Token: 0x06000C26 RID: 3110 RVA: 0x00042200 File Offset: 0x00040400
	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterNetworkBool))]
	[NetworkedWeaved(1, 20)]
	public NetworkArray<NetworkBool> OpenDoors
	{
		get
		{
			return new NetworkArray<NetworkBool>(Native.ReferenceToPointer<FixedStorage@20>(ref this._OpenDoors), 20, ElementReaderWriterNetworkBool.GetInstance());
		}
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x00042228 File Offset: 0x00040428
	public GhostLabData(int state, bool[] openDoors)
	{
		this.DoorState = state;
		for (int i = 0; i < openDoors.Length; i++)
		{
			bool flag = openDoors[i];
			this.OpenDoors.Set(i, flag);
		}
	}

	// Token: 0x04000ECA RID: 3786
	[FixedBufferProperty(typeof(NetworkArray<NetworkBool>), typeof(UnityArraySurrogate@ElementReaderWriterNetworkBool), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@20 _OpenDoors;
}
