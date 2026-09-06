using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020004FF RID: 1279
[NetworkStructWeaved(21)]
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 84)]
public struct ReliableStateData : INetworkStruct
{
	// Token: 0x17000374 RID: 884
	// (get) Token: 0x0600200B RID: 8203 RVA: 0x000AC8CA File Offset: 0x000AAACA
	// (set) Token: 0x0600200C RID: 8204 RVA: 0x000AC8D2 File Offset: 0x000AAAD2
	public long Header { readonly get; set; }

	// Token: 0x17000375 RID: 885
	// (get) Token: 0x0600200D RID: 8205 RVA: 0x000AC8DC File Offset: 0x000AAADC
	[Networked]
	[Capacity(5)]
	[NetworkedWeavedArray(5, 2, typeof(ElementReaderWriterInt64))]
	[NetworkedWeaved(11, 10)]
	public NetworkArray<long> TransferrableStates
	{
		get
		{
			return new NetworkArray<long>(Native.ReferenceToPointer<FixedStorage@10>(ref this._TransferrableStates), 5, ElementReaderWriterInt64.GetInstance());
		}
	}

	// Token: 0x17000376 RID: 886
	// (get) Token: 0x0600200E RID: 8206 RVA: 0x000AC8FF File Offset: 0x000AAAFF
	// (set) Token: 0x0600200F RID: 8207 RVA: 0x000AC907 File Offset: 0x000AAB07
	public int WearablesPackedState { readonly get; set; }

	// Token: 0x17000377 RID: 887
	// (get) Token: 0x06002010 RID: 8208 RVA: 0x000AC910 File Offset: 0x000AAB10
	// (set) Token: 0x06002011 RID: 8209 RVA: 0x000AC918 File Offset: 0x000AAB18
	public int LThrowableProjectileIndex { readonly get; set; }

	// Token: 0x17000378 RID: 888
	// (get) Token: 0x06002012 RID: 8210 RVA: 0x000AC921 File Offset: 0x000AAB21
	// (set) Token: 0x06002013 RID: 8211 RVA: 0x000AC929 File Offset: 0x000AAB29
	public int RThrowableProjectileIndex { readonly get; set; }

	// Token: 0x17000379 RID: 889
	// (get) Token: 0x06002014 RID: 8212 RVA: 0x000AC932 File Offset: 0x000AAB32
	// (set) Token: 0x06002015 RID: 8213 RVA: 0x000AC93A File Offset: 0x000AAB3A
	public int SizeLayerMask { readonly get; set; }

	// Token: 0x1700037A RID: 890
	// (get) Token: 0x06002016 RID: 8214 RVA: 0x000AC943 File Offset: 0x000AAB43
	// (set) Token: 0x06002017 RID: 8215 RVA: 0x000AC94B File Offset: 0x000AAB4B
	public int RandomThrowableIndex { readonly get; set; }

	// Token: 0x1700037B RID: 891
	// (get) Token: 0x06002018 RID: 8216 RVA: 0x000AC954 File Offset: 0x000AAB54
	// (set) Token: 0x06002019 RID: 8217 RVA: 0x000AC95C File Offset: 0x000AAB5C
	public long PackedBeads { readonly get; set; }

	// Token: 0x1700037C RID: 892
	// (get) Token: 0x0600201A RID: 8218 RVA: 0x000AC965 File Offset: 0x000AAB65
	// (set) Token: 0x0600201B RID: 8219 RVA: 0x000AC96D File Offset: 0x000AAB6D
	public long PackedBeadsMoreThan6 { readonly get; set; }

	// Token: 0x04002AD8 RID: 10968
	[FixedBufferProperty(typeof(NetworkArray<long>), typeof(UnityArraySurrogate@ElementReaderWriterInt64), 5, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(44)]
	private FixedStorage@10 _TransferrableStates;
}
