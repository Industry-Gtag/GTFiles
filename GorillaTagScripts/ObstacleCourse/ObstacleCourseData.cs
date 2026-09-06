using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x0200100D RID: 4109
	[NetworkStructWeaved(9)]
	[StructLayout(LayoutKind.Explicit, Size = 36)]
	public struct ObstacleCourseData : INetworkStruct
	{
		// Token: 0x170009CF RID: 2511
		// (get) Token: 0x0600666C RID: 26220 RVA: 0x0020ECF1 File Offset: 0x0020CEF1
		// (set) Token: 0x0600666D RID: 26221 RVA: 0x0020ECF9 File Offset: 0x0020CEF9
		public int ObstacleCourseCount { readonly get; set; }

		// Token: 0x170009D0 RID: 2512
		// (get) Token: 0x0600666E RID: 26222 RVA: 0x0020ED04 File Offset: 0x0020CF04
		[Networked]
		[Capacity(4)]
		[NetworkedWeavedArray(4, 1, typeof(ElementReaderWriterInt32))]
		[NetworkedWeaved(1, 4)]
		public NetworkArray<int> WinnerActorNumber
		{
			get
			{
				return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@4>(ref this._WinnerActorNumber), 4, ElementReaderWriterInt32.GetInstance());
			}
		}

		// Token: 0x170009D1 RID: 2513
		// (get) Token: 0x0600666F RID: 26223 RVA: 0x0020ED28 File Offset: 0x0020CF28
		[Networked]
		[Capacity(4)]
		[NetworkedWeavedArray(4, 1, typeof(ElementReaderWriterInt32))]
		[NetworkedWeaved(5, 4)]
		public NetworkArray<int> CurrentRaceState
		{
			get
			{
				return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@4>(ref this._CurrentRaceState), 4, ElementReaderWriterInt32.GetInstance());
			}
		}

		// Token: 0x06006670 RID: 26224 RVA: 0x0020ED4C File Offset: 0x0020CF4C
		public ObstacleCourseData(List<ObstacleCourse> courses)
		{
			this.ObstacleCourseCount = courses.Count;
			int[] array = new int[this.ObstacleCourseCount];
			int[] array2 = new int[this.ObstacleCourseCount];
			for (int i = 0; i < courses.Count; i++)
			{
				array[i] = courses[i].winnerActorNumber;
				array2[i] = (int)courses[i].currentState;
			}
			this.WinnerActorNumber.CopyFrom(array, 0, this.ObstacleCourseCount);
			this.CurrentRaceState.CopyFrom(array2, 0, this.ObstacleCourseCount);
		}

		// Token: 0x04007557 RID: 30039
		[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 4, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@4 _WinnerActorNumber;

		// Token: 0x04007558 RID: 30040
		[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 4, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(20)]
		private FixedStorage@4 _CurrentRaceState;
	}
}
