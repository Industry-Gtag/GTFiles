using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F87 RID: 3975
	[NetworkStructWeaved(13)]
	[StructLayout(LayoutKind.Explicit, Size = 52)]
	public struct FlowersDataStruct : INetworkStruct
	{
		// Token: 0x1700096B RID: 2411
		// (get) Token: 0x060062A4 RID: 25252 RVA: 0x001FBCDB File Offset: 0x001F9EDB
		// (set) Token: 0x060062A5 RID: 25253 RVA: 0x001FBCE3 File Offset: 0x001F9EE3
		public int FlowerCount { readonly get; set; }

		// Token: 0x1700096C RID: 2412
		// (get) Token: 0x060062A6 RID: 25254 RVA: 0x001FBCEC File Offset: 0x001F9EEC
		[Networked]
		[NetworkedWeavedLinkedList(1, 1, typeof(ElementReaderWriterByte))]
		[NetworkedWeaved(1, 6)]
		public NetworkLinkedList<byte> FlowerWateredData
		{
			get
			{
				return new NetworkLinkedList<byte>(Native.ReferenceToPointer<FixedStorage@6>(ref this._FlowerWateredData), 1, ElementReaderWriterByte.GetInstance());
			}
		}

		// Token: 0x1700096D RID: 2413
		// (get) Token: 0x060062A7 RID: 25255 RVA: 0x001FBD10 File Offset: 0x001F9F10
		[Networked]
		[NetworkedWeavedLinkedList(1, 1, typeof(ElementReaderWriterInt32))]
		[NetworkedWeaved(7, 6)]
		public NetworkLinkedList<int> FlowerStateData
		{
			get
			{
				return new NetworkLinkedList<int>(Native.ReferenceToPointer<FixedStorage@6>(ref this._FlowerStateData), 1, ElementReaderWriterInt32.GetInstance());
			}
		}

		// Token: 0x060062A8 RID: 25256 RVA: 0x001FBD34 File Offset: 0x001F9F34
		public FlowersDataStruct(List<Flower> allFlowers)
		{
			this.FlowerCount = allFlowers.Count;
			foreach (Flower flower in allFlowers)
			{
				this.FlowerWateredData.Add(flower.IsWatered ? 1 : 0);
				this.FlowerStateData.Add((int)flower.GetCurrentState());
			}
		}

		// Token: 0x04007163 RID: 29027
		[FixedBufferProperty(typeof(NetworkLinkedList<byte>), typeof(UnityLinkedListSurrogate@ElementReaderWriterByte), 1, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@6 _FlowerWateredData;

		// Token: 0x04007164 RID: 29028
		[FixedBufferProperty(typeof(NetworkLinkedList<int>), typeof(UnityLinkedListSurrogate@ElementReaderWriterInt32), 1, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(28)]
		private FixedStorage@6 _FlowerStateData;
	}
}
