using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000375 RID: 885
[NetworkBehaviourWeaved(2)]
public class RandomTimedSeedManager : NetworkComponent, ITickSystemTick
{
	// Token: 0x17000225 RID: 549
	// (get) Token: 0x060015AE RID: 5550 RVA: 0x0007318A File Offset: 0x0007138A
	// (set) Token: 0x060015AF RID: 5551 RVA: 0x00073191 File Offset: 0x00071391
	public static RandomTimedSeedManager instance { get; private set; }

	// Token: 0x17000226 RID: 550
	// (get) Token: 0x060015B0 RID: 5552 RVA: 0x00073199 File Offset: 0x00071399
	// (set) Token: 0x060015B1 RID: 5553 RVA: 0x000731A1 File Offset: 0x000713A1
	public int seed { get; private set; }

	// Token: 0x17000227 RID: 551
	// (get) Token: 0x060015B2 RID: 5554 RVA: 0x000731AA File Offset: 0x000713AA
	// (set) Token: 0x060015B3 RID: 5555 RVA: 0x000731B2 File Offset: 0x000713B2
	public float currentSyncTime { get; private set; }

	// Token: 0x060015B4 RID: 5556 RVA: 0x000731BB File Offset: 0x000713BB
	protected override void Awake()
	{
		base.Awake();
		RandomTimedSeedManager.instance = this;
		this.seed = Random.Range(-1000000, -1000000);
		this.idealSyncTime = 0f;
		this.currentSyncTime = 0f;
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060015B5 RID: 5557 RVA: 0x000731FA File Offset: 0x000713FA
	public void AddCallbackOnSeedChanged(Action callback)
	{
		this.callbacksOnSeedChanged.Add(callback);
	}

	// Token: 0x060015B6 RID: 5558 RVA: 0x00073208 File Offset: 0x00071408
	public void RemoveCallbackOnSeedChanged(Action callback)
	{
		this.callbacksOnSeedChanged.Remove(callback);
	}

	// Token: 0x17000228 RID: 552
	// (get) Token: 0x060015B7 RID: 5559 RVA: 0x00073217 File Offset: 0x00071417
	// (set) Token: 0x060015B8 RID: 5560 RVA: 0x0007321F File Offset: 0x0007141F
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x060015B9 RID: 5561 RVA: 0x00073228 File Offset: 0x00071428
	void ITickSystemTick.Tick()
	{
		this.currentSyncTime += Time.deltaTime;
		this.idealSyncTime += Time.deltaTime;
		if (this.idealSyncTime > 1E+09f)
		{
			this.idealSyncTime -= 1E+09f;
			this.currentSyncTime -= 1E+09f;
		}
		if (!base.GetView.AmOwner)
		{
			this.currentSyncTime = Mathf.Lerp(this.currentSyncTime, this.idealSyncTime, 0.1f);
		}
	}

	// Token: 0x17000229 RID: 553
	// (get) Token: 0x060015BA RID: 5562 RVA: 0x000732B3 File Offset: 0x000714B3
	// (set) Token: 0x060015BB RID: 5563 RVA: 0x000732DD File Offset: 0x000714DD
	[Networked]
	[NetworkedWeaved(0, 2)]
	private unsafe RandomTimedSeedManager.RandomTimedSeedManagerData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing RandomTimedSeedManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(RandomTimedSeedManager.RandomTimedSeedManagerData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing RandomTimedSeedManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(RandomTimedSeedManager.RandomTimedSeedManagerData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060015BC RID: 5564 RVA: 0x00073308 File Offset: 0x00071508
	public override void WriteDataFusion()
	{
		this.Data = new RandomTimedSeedManager.RandomTimedSeedManagerData(this.seed, this.currentSyncTime);
	}

	// Token: 0x060015BD RID: 5565 RVA: 0x00073324 File Offset: 0x00071524
	public override void ReadDataFusion()
	{
		this.ReadDataShared(this.Data.seed, this.Data.currentSyncTime);
	}

	// Token: 0x060015BE RID: 5566 RVA: 0x00073353 File Offset: 0x00071553
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		stream.SendNext(this.seed);
		stream.SendNext(this.currentSyncTime);
	}

	// Token: 0x060015BF RID: 5567 RVA: 0x00073388 File Offset: 0x00071588
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		float num2 = (float)stream.ReceiveNext();
		this.ReadDataShared(num, num2);
	}

	// Token: 0x060015C0 RID: 5568 RVA: 0x000733C4 File Offset: 0x000715C4
	private void ReadDataShared(int seedVal, float testTime)
	{
		if (!float.IsFinite(testTime))
		{
			return;
		}
		this.seed = seedVal;
		if (testTime >= 0f && testTime <= 1E+09f)
		{
			if (this.idealSyncTime - testTime > 500000000f)
			{
				this.currentSyncTime = testTime;
			}
			this.idealSyncTime = testTime;
		}
		if (this.seed != this.cachedSeed && this.seed >= -1000000 && this.seed <= -1000000)
		{
			this.currentSyncTime = this.idealSyncTime;
			this.cachedSeed = this.seed;
			foreach (Action action in this.callbacksOnSeedChanged)
			{
				action();
			}
		}
	}

	// Token: 0x060015C2 RID: 5570 RVA: 0x000734A7 File Offset: 0x000716A7
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x060015C3 RID: 5571 RVA: 0x000734BF File Offset: 0x000716BF
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04001A8A RID: 6794
	private List<Action> callbacksOnSeedChanged = new List<Action>();

	// Token: 0x04001A8C RID: 6796
	private float idealSyncTime;

	// Token: 0x04001A8E RID: 6798
	private int cachedSeed;

	// Token: 0x04001A8F RID: 6799
	private const int SeedMin = -1000000;

	// Token: 0x04001A90 RID: 6800
	private const int SeedMax = -1000000;

	// Token: 0x04001A91 RID: 6801
	private const float MaxSyncTime = 1E+09f;

	// Token: 0x04001A93 RID: 6803
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 2)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private RandomTimedSeedManager.RandomTimedSeedManagerData _Data;

	// Token: 0x02000376 RID: 886
	[NetworkStructWeaved(2)]
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	private struct RandomTimedSeedManagerData : INetworkStruct
	{
		// Token: 0x1700022A RID: 554
		// (get) Token: 0x060015C4 RID: 5572 RVA: 0x000734D3 File Offset: 0x000716D3
		// (set) Token: 0x060015C5 RID: 5573 RVA: 0x000734E1 File Offset: 0x000716E1
		[Networked]
		[NetworkedWeaved(0, 1)]
		public unsafe int seed
		{
			readonly get
			{
				return *(int*)Native.ReferenceToPointer<FixedStorage@1>(ref this._seed);
			}
			set
			{
				*(int*)Native.ReferenceToPointer<FixedStorage@1>(ref this._seed) = value;
			}
		}

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x060015C6 RID: 5574 RVA: 0x000734F0 File Offset: 0x000716F0
		// (set) Token: 0x060015C7 RID: 5575 RVA: 0x000734FE File Offset: 0x000716FE
		[Networked]
		[NetworkedWeaved(1, 1)]
		public unsafe float currentSyncTime
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._currentSyncTime);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._currentSyncTime) = value;
			}
		}

		// Token: 0x060015C8 RID: 5576 RVA: 0x0007350D File Offset: 0x0007170D
		public RandomTimedSeedManagerData(int seed, float currentSyncTime)
		{
			this.seed = seed;
			this.currentSyncTime = currentSyncTime;
		}

		// Token: 0x04001A94 RID: 6804
		[FixedBufferProperty(typeof(int), typeof(UnityValueSurrogate@ElementReaderWriterInt32), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@1 _seed;

		// Token: 0x04001A95 RID: 6805
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ElementReaderWriterSingle), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@1 _currentSyncTime;
	}
}
