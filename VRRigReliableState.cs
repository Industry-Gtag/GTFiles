using System;
using System.Collections.Generic;
using Fusion;
using GorillaNetworking;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000500 RID: 1280
public class VRRigReliableState : MonoBehaviour, IWrappedSerializable, INetworkStruct
{
	// Token: 0x1700037D RID: 893
	// (get) Token: 0x0600201C RID: 8220 RVA: 0x000AC976 File Offset: 0x000AAB76
	public bool HasBracelet
	{
		get
		{
			return this.braceletBeadColors.Count > 0;
		}
	}

	// Token: 0x1700037E RID: 894
	// (get) Token: 0x0600201D RID: 8221 RVA: 0x000AC986 File Offset: 0x000AAB86
	// (set) Token: 0x0600201E RID: 8222 RVA: 0x000AC98E File Offset: 0x000AAB8E
	public bool isDirty { get; private set; } = true;

	// Token: 0x0600201F RID: 8223 RVA: 0x000AC997 File Offset: 0x000AAB97
	private void Awake()
	{
		VRRig.newPlayerJoined = (Action)Delegate.Combine(VRRig.newPlayerJoined, new Action(this.SetIsDirty));
		RoomSystem.JoinedRoomEvent += new Action(this.SetIsDirty);
	}

	// Token: 0x06002020 RID: 8224 RVA: 0x000AC9D4 File Offset: 0x000AABD4
	private void OnDestroy()
	{
		VRRig.newPlayerJoined = (Action)Delegate.Remove(VRRig.newPlayerJoined, new Action(this.SetIsDirty));
	}

	// Token: 0x06002021 RID: 8225 RVA: 0x000AC9F6 File Offset: 0x000AABF6
	public void SetIsDirty()
	{
		this.isDirty = true;
	}

	// Token: 0x06002022 RID: 8226 RVA: 0x000AC9FF File Offset: 0x000AABFF
	public void SetIsNotDirty()
	{
		this.isDirty = false;
	}

	// Token: 0x06002023 RID: 8227 RVA: 0x000ACA08 File Offset: 0x000AAC08
	public void SharedStart(bool isOfflineVRRig_, BodyDockPositions bDock_)
	{
		this.isOfflineVRRig = isOfflineVRRig_;
		this.bDock = bDock_;
		this.activeTransferrableObjectIndex = new int[5];
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			this.activeTransferrableObjectIndex[i] = -1;
		}
		this.transferrablePosStates = new TransferrableObject.PositionState[5];
		this.transferrableItemStates = new TransferrableObject.ItemStates[5];
		this.transferableDockPositions = new BodyDockPositions.DropPositions[5];
	}

	// Token: 0x06002024 RID: 8228 RVA: 0x000ACA70 File Offset: 0x000AAC70
	public void RegisterCosmeticStateSyncTarget(VRRigReliableState.StateSyncSlots slot, ICosmeticStateSync target)
	{
		if (this.m_cosmeticStateTargets[(int)slot] != null)
		{
			Debug.LogWarning(string.Format("{0}-CosmeticStateSync: instance already registered at slot {1}, this will be overriden", "VRRigReliableState", slot));
		}
		this.m_cosmeticStateTargets[(int)slot] = target;
		if (this.bDock.myRig.isOfflineVRRig)
		{
			this.m_cosmeticStates[(int)slot] = target.StateValue;
			this.isDirty = true;
			return;
		}
		target.OnStateUpdate(this.m_cosmeticStates[(int)slot]);
	}

	// Token: 0x06002025 RID: 8229 RVA: 0x000ACAE4 File Offset: 0x000AACE4
	public void UnRegisterCosmeticStateSyncTarget(VRRigReliableState.StateSyncSlots slot, ICosmeticStateSync target)
	{
		if (this.m_cosmeticStateTargets[(int)slot] != target)
		{
			Debug.LogWarning(string.Format("{0}-CosmeticStateSync: target is not the value stored at slot {1}, ignoring", "VRRigReliableState", slot));
			return;
		}
		this.m_cosmeticStateTargets[(int)slot] = null;
		this.m_cosmeticStates[(int)slot] = -1;
		if (this.bDock.myRig.isOfflineVRRig)
		{
			this.isDirty = true;
		}
	}

	// Token: 0x06002026 RID: 8230 RVA: 0x000ACB44 File Offset: 0x000AAD44
	private void CopyStateSyncToSyncArray()
	{
		for (int i = 0; i < this.m_cosmeticStateTargets.Length; i++)
		{
			ICosmeticStateSync cosmeticStateSync = this.m_cosmeticStateTargets[i];
			int num = ((cosmeticStateSync != null) ? cosmeticStateSync.StateValue : (-1));
			if (num != this.m_cosmeticStates[i])
			{
				this.isDirty = true;
			}
			this.m_cosmeticStates[i] = num;
		}
	}

	// Token: 0x06002027 RID: 8231 RVA: 0x000ACB94 File Offset: 0x000AAD94
	public int GetCachedStateAtSlot(VRRigReliableState.StateSyncSlots slot)
	{
		if (slot < VRRigReliableState.StateSyncSlots.Hat || slot >= (VRRigReliableState.StateSyncSlots)this.m_cosmeticStates.Length)
		{
			return -1;
		}
		return this.m_cosmeticStates[(int)slot];
	}

	// Token: 0x06002028 RID: 8232 RVA: 0x000ACBBC File Offset: 0x000AADBC
	void IWrappedSerializable.OnSerializeRead(object newData)
	{
		this.Data = (ReliableStateData)newData;
		long header = this.Data.Header;
		int num;
		this.SetHeader(header, out num);
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			if ((header & (1L << (i & 31))) != 0L)
			{
				long num2 = this.Data.TransferrableStates[i];
				this.activeTransferrableObjectIndex[i] = (int)num2;
				this.transferrablePosStates[i] = (TransferrableObject.PositionState)((num2 >> 32) & 255L);
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)((num2 >> 40) & 255L);
				this.transferableDockPositions[i] = (BodyDockPositions.DropPositions)((num2 >> 48) & 255L);
			}
			else
			{
				this.activeTransferrableObjectIndex[i] = -1;
				this.transferrablePosStates[i] = TransferrableObject.PositionState.None;
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)0;
				this.transferableDockPositions[i] = BodyDockPositions.DropPositions.None;
			}
		}
		this.wearablesPackedStates = this.Data.WearablesPackedState;
		this.lThrowableProjectileIndex = this.Data.LThrowableProjectileIndex;
		this.rThrowableProjectileIndex = this.Data.RThrowableProjectileIndex;
		this.sizeLayerMask = this.Data.SizeLayerMask;
		this.randomThrowableIndex = this.Data.RandomThrowableIndex;
		this.braceletBeadColors.Clear();
		if (num > 0)
		{
			if (num <= 3)
			{
				int num3 = (int)this.Data.PackedBeads;
				this.braceletSelfIndex = num3 >> 30;
				VRRigReliableState.UnpackBeadColors((long)num3, 0, num, this.braceletBeadColors);
			}
			else
			{
				long packedBeads = this.Data.PackedBeads;
				this.braceletSelfIndex = (int)(packedBeads >> 60);
				if (num <= 6)
				{
					VRRigReliableState.UnpackBeadColors(packedBeads, 0, num, this.braceletBeadColors);
				}
				else
				{
					VRRigReliableState.UnpackBeadColors(packedBeads, 0, 6, this.braceletBeadColors);
					VRRigReliableState.UnpackBeadColors(this.Data.PackedBeadsMoreThan6, 6, num, this.braceletBeadColors);
				}
			}
		}
		this.bDock.RefreshTransferrableItems();
		this.bDock.myRig.UpdateFriendshipBracelet();
	}

	// Token: 0x06002029 RID: 8233 RVA: 0x000ACD98 File Offset: 0x000AAF98
	object IWrappedSerializable.OnSerializeWrite()
	{
		this.isDirty = false;
		ReliableStateData reliableStateData = default(ReliableStateData);
		long header = this.GetHeader();
		reliableStateData.Header = header;
		long[] array = this.GetTransferrableStates(header).ToArray();
		reliableStateData.TransferrableStates.CopyFrom(array, 0, array.Length);
		reliableStateData.WearablesPackedState = this.wearablesPackedStates;
		reliableStateData.LThrowableProjectileIndex = this.lThrowableProjectileIndex;
		reliableStateData.RThrowableProjectileIndex = this.rThrowableProjectileIndex;
		reliableStateData.SizeLayerMask = this.sizeLayerMask;
		reliableStateData.RandomThrowableIndex = this.randomThrowableIndex;
		if (this.braceletBeadColors.Count > 0)
		{
			long num = VRRigReliableState.PackBeadColors(this.braceletBeadColors, 0);
			if (this.braceletBeadColors.Count <= 3)
			{
				num |= (long)this.braceletSelfIndex << 30;
				reliableStateData.PackedBeads = num;
			}
			else
			{
				num |= (long)this.braceletSelfIndex << 60;
				reliableStateData.PackedBeads = num;
				if (this.braceletBeadColors.Count > 6)
				{
					reliableStateData.PackedBeadsMoreThan6 = VRRigReliableState.PackBeadColors(this.braceletBeadColors, 6);
				}
			}
		}
		this.Data = reliableStateData;
		return reliableStateData;
	}

	// Token: 0x0600202A RID: 8234 RVA: 0x000ACEB0 File Offset: 0x000AB0B0
	void IWrappedSerializable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		this.CopyStateSyncToSyncArray();
		if (!this.isDirty)
		{
			return;
		}
		this.isDirty = false;
		long header = this.GetHeader();
		stream.SendNext(header);
		foreach (long num in this.GetTransferrableStates(header))
		{
			stream.SendNext(num);
		}
		stream.SendNext(this.wearablesPackedStates);
		stream.SendNext(this.lThrowableProjectileIndex);
		stream.SendNext(this.rThrowableProjectileIndex);
		stream.SendNext(this.sizeLayerMask);
		stream.SendNext(this.randomThrowableIndex);
		foreach (int num2 in this.m_cosmeticStates)
		{
			stream.SendNext(num2);
		}
		if (this.braceletBeadColors.Count > 0)
		{
			long num3 = VRRigReliableState.PackBeadColors(this.braceletBeadColors, 0);
			if (this.braceletBeadColors.Count <= 3)
			{
				num3 |= (long)this.braceletSelfIndex << 30;
				stream.SendNext((int)num3);
				return;
			}
			num3 |= (long)this.braceletSelfIndex << 60;
			stream.SendNext(num3);
			if (this.braceletBeadColors.Count > 6)
			{
				stream.SendNext(VRRigReliableState.PackBeadColors(this.braceletBeadColors, 6));
			}
		}
	}

	// Token: 0x0600202B RID: 8235 RVA: 0x000AD03C File Offset: 0x000AB23C
	void IWrappedSerializable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		long num = (long)stream.ReceiveNext();
		this.isMicEnabled = (num & 32L) != 0L;
		this.isBraceletLeftHanded = (num & 64L) != 0L;
		this.isBuilderWatchEnabled = (num & 128L) != 0L;
		int num2 = (int)(num >> 12) & 15;
		this.lThrowableProjectileColor.r = (byte)(num >> 16);
		this.lThrowableProjectileColor.g = (byte)(num >> 24);
		this.lThrowableProjectileColor.b = (byte)(num >> 32);
		this.rThrowableProjectileColor.r = (byte)(num >> 40);
		this.rThrowableProjectileColor.g = (byte)(num >> 48);
		this.rThrowableProjectileColor.b = (byte)(num >> 56);
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			if ((num & (1L << (i & 31))) != 0L)
			{
				long num3 = (long)stream.ReceiveNext();
				this.activeTransferrableObjectIndex[i] = (int)num3;
				this.transferrablePosStates[i] = (TransferrableObject.PositionState)((num3 >> 32) & 255L);
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)((num3 >> 40) & 255L);
				this.transferableDockPositions[i] = (BodyDockPositions.DropPositions)((num3 >> 48) & 255L);
			}
			else
			{
				this.activeTransferrableObjectIndex[i] = -1;
				this.transferrablePosStates[i] = TransferrableObject.PositionState.None;
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)0;
				this.transferableDockPositions[i] = BodyDockPositions.DropPositions.None;
			}
		}
		this.wearablesPackedStates = (int)stream.ReceiveNext();
		this.lThrowableProjectileIndex = (int)stream.ReceiveNext();
		this.rThrowableProjectileIndex = (int)stream.ReceiveNext();
		this.sizeLayerMask = (int)stream.ReceiveNext();
		this.randomThrowableIndex = (int)stream.ReceiveNext();
		for (int j = 0; j < this.m_cosmeticStates.Length; j++)
		{
			int num4 = (int)stream.ReceiveNext();
			this.m_cosmeticStates[j] = num4;
			ICosmeticStateSync cosmeticStateSync = this.m_cosmeticStateTargets[j];
			if (cosmeticStateSync != null)
			{
				cosmeticStateSync.OnStateUpdate(num4);
			}
		}
		this.braceletBeadColors.Clear();
		if (num2 > 0)
		{
			if (num2 <= 3)
			{
				int num5 = (int)stream.ReceiveNext();
				this.braceletSelfIndex = num5 >> 30;
				VRRigReliableState.UnpackBeadColors((long)num5, 0, num2, this.braceletBeadColors);
			}
			else
			{
				long num6 = (long)stream.ReceiveNext();
				this.braceletSelfIndex = (int)(num6 >> 60);
				if (num2 <= 6)
				{
					VRRigReliableState.UnpackBeadColors(num6, 0, num2, this.braceletBeadColors);
				}
				else
				{
					VRRigReliableState.UnpackBeadColors(num6, 0, 6, this.braceletBeadColors);
					VRRigReliableState.UnpackBeadColors((long)stream.ReceiveNext(), 6, num2, this.braceletBeadColors);
				}
			}
		}
		this.bDock.RefreshTransferrableItems();
		this.bDock.myRig.UpdateFriendshipBracelet();
		this.bDock.myRig.EnableBuilderResizeWatch(this.isBuilderWatchEnabled);
	}

	// Token: 0x0600202C RID: 8236 RVA: 0x000AD2EC File Offset: 0x000AB4EC
	private long GetHeader()
	{
		long num = 0L;
		if (CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
		{
			for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
			{
				if (this.activeTransferrableObjectIndex[i] != -1 && (this.transferrablePosStates[i] == TransferrableObject.PositionState.InLeftHand || this.transferrablePosStates[i] == TransferrableObject.PositionState.InRightHand))
				{
					num |= (long)((ulong)((byte)(1 << i)));
				}
			}
		}
		else
		{
			for (int j = 0; j < this.activeTransferrableObjectIndex.Length; j++)
			{
				if (this.activeTransferrableObjectIndex[j] != -1)
				{
					num |= (long)((ulong)((byte)(1 << j)));
				}
			}
		}
		if (this.isBraceletLeftHanded)
		{
			num |= 64L;
		}
		if (this.isMicEnabled)
		{
			num |= 32L;
		}
		if (this.isBuilderWatchEnabled && !CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
		{
			num |= 128L;
		}
		num |= ((long)this.braceletBeadColors.Count & 15L) << 12;
		num |= (long)((long)((ulong)this.lThrowableProjectileColor.r) << 16);
		num |= (long)((long)((ulong)this.lThrowableProjectileColor.g) << 24);
		num |= (long)((long)((ulong)this.lThrowableProjectileColor.b) << 32);
		num |= (long)((long)((ulong)this.rThrowableProjectileColor.r) << 40);
		num |= (long)((long)((ulong)this.rThrowableProjectileColor.g) << 48);
		return num | (long)((long)((ulong)this.rThrowableProjectileColor.b) << 56);
	}

	// Token: 0x0600202D RID: 8237 RVA: 0x000AD434 File Offset: 0x000AB634
	private void SetHeader(long header, out int numBeadsToRead)
	{
		this.isMicEnabled = (header & 32L) != 0L;
		this.isBraceletLeftHanded = (header & 64L) != 0L;
		numBeadsToRead = (int)(header >> 12) & 15;
		this.lThrowableProjectileColor.r = (byte)(header >> 16);
		this.lThrowableProjectileColor.g = (byte)(header >> 24);
		this.lThrowableProjectileColor.b = (byte)(header >> 32);
		this.rThrowableProjectileColor.r = (byte)(header >> 40);
		this.rThrowableProjectileColor.g = (byte)(header >> 48);
		this.rThrowableProjectileColor.b = (byte)(header >> 56);
	}

	// Token: 0x0600202E RID: 8238 RVA: 0x000AD4CC File Offset: 0x000AB6CC
	private List<long> GetTransferrableStates(long header)
	{
		List<long> list = new List<long>();
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			if ((header & (1L << (i & 31))) != 0L && this.activeTransferrableObjectIndex[i] != -1)
			{
				long num = (long)((ulong)this.activeTransferrableObjectIndex[i]);
				num |= (long)this.transferrablePosStates[i] << 32;
				num |= (long)this.transferrableItemStates[i] << 40;
				num |= (long)this.transferableDockPositions[i] << 48;
				list.Add(num);
			}
		}
		return list;
	}

	// Token: 0x0600202F RID: 8239 RVA: 0x000AD548 File Offset: 0x000AB748
	private static long PackBeadColors(List<Color> beadColors, int fromIndex)
	{
		long num = 0L;
		int num2 = Mathf.Min(fromIndex + 6, beadColors.Count);
		int num3 = 0;
		for (int i = fromIndex; i < num2; i++)
		{
			long num4 = (long)FriendshipGroupDetection.PackColor(beadColors[i]);
			num |= num4 << num3;
			num3 += 10;
		}
		return num;
	}

	// Token: 0x06002030 RID: 8240 RVA: 0x000AD594 File Offset: 0x000AB794
	private static void UnpackBeadColors(long packed, int startIndex, int endIndex, List<Color> beadColorsResult)
	{
		int num = Mathf.Min(startIndex + 6, endIndex);
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			short num3 = (short)((packed >> num2) & 1023L);
			beadColorsResult.Add(FriendshipGroupDetection.UnpackColor(num3));
			num2 += 10;
		}
	}

	// Token: 0x04002AD9 RID: 10969
	[NonSerialized]
	private ICosmeticStateSync[] m_cosmeticStateTargets = new ICosmeticStateSync[4];

	// Token: 0x04002ADA RID: 10970
	[NonSerialized]
	private int[] m_cosmeticStates = new int[4];

	// Token: 0x04002ADB RID: 10971
	[NonSerialized]
	public int[] activeTransferrableObjectIndex;

	// Token: 0x04002ADC RID: 10972
	[NonSerialized]
	public TransferrableObject.PositionState[] transferrablePosStates;

	// Token: 0x04002ADD RID: 10973
	[NonSerialized]
	public TransferrableObject.ItemStates[] transferrableItemStates;

	// Token: 0x04002ADE RID: 10974
	[NonSerialized]
	public BodyDockPositions.DropPositions[] transferableDockPositions;

	// Token: 0x04002ADF RID: 10975
	[NonSerialized]
	public int wearablesPackedStates;

	// Token: 0x04002AE0 RID: 10976
	[NonSerialized]
	public int lThrowableProjectileIndex = -1;

	// Token: 0x04002AE1 RID: 10977
	[NonSerialized]
	public int rThrowableProjectileIndex = -1;

	// Token: 0x04002AE2 RID: 10978
	[NonSerialized]
	public Color32 lThrowableProjectileColor = Color.white;

	// Token: 0x04002AE3 RID: 10979
	[NonSerialized]
	public Color32 rThrowableProjectileColor = Color.white;

	// Token: 0x04002AE4 RID: 10980
	[NonSerialized]
	public int randomThrowableIndex;

	// Token: 0x04002AE5 RID: 10981
	[NonSerialized]
	public bool isMicEnabled;

	// Token: 0x04002AE6 RID: 10982
	private bool isOfflineVRRig;

	// Token: 0x04002AE7 RID: 10983
	private BodyDockPositions bDock;

	// Token: 0x04002AE8 RID: 10984
	[NonSerialized]
	public int sizeLayerMask = 1;

	// Token: 0x04002AE9 RID: 10985
	private const long IS_MIC_ENABLED_BIT = 32L;

	// Token: 0x04002AEA RID: 10986
	private const long BRACELET_LEFTHAND_BIT = 64L;

	// Token: 0x04002AEB RID: 10987
	private const long BUILDER_WATCH_ENABLED_BIT = 128L;

	// Token: 0x04002AEC RID: 10988
	private const int BRACELET_NUM_BEADS_SHIFT = 12;

	// Token: 0x04002AED RID: 10989
	private const int LPROJECTILECOLOR_R_SHIFT = 16;

	// Token: 0x04002AEE RID: 10990
	private const int LPROJECTILECOLOR_G_SHIFT = 24;

	// Token: 0x04002AEF RID: 10991
	private const int LPROJECTILECOLOR_B_SHIFT = 32;

	// Token: 0x04002AF0 RID: 10992
	private const int RPROJECTILECOLOR_R_SHIFT = 40;

	// Token: 0x04002AF1 RID: 10993
	private const int RPROJECTILECOLOR_G_SHIFT = 48;

	// Token: 0x04002AF2 RID: 10994
	private const int RPROJECTILECOLOR_B_SHIFT = 56;

	// Token: 0x04002AF3 RID: 10995
	private const int POS_STATES_SHIFT = 32;

	// Token: 0x04002AF4 RID: 10996
	private const int ITEM_STATES_SHIFT = 40;

	// Token: 0x04002AF5 RID: 10997
	private const int DOCK_POSITIONS_SHIFT = 48;

	// Token: 0x04002AF6 RID: 10998
	private const int BRACELET_SELF_INDEX_SHIFT = 60;

	// Token: 0x04002AF7 RID: 10999
	[NonSerialized]
	public bool isBraceletLeftHanded;

	// Token: 0x04002AF8 RID: 11000
	[NonSerialized]
	public int braceletSelfIndex;

	// Token: 0x04002AF9 RID: 11001
	[NonSerialized]
	public List<Color> braceletBeadColors = new List<Color>(10);

	// Token: 0x04002AFA RID: 11002
	[NonSerialized]
	public bool isBuilderWatchEnabled;

	// Token: 0x04002AFC RID: 11004
	private ReliableStateData Data;

	// Token: 0x02000501 RID: 1281
	public enum StateSyncSlots
	{
		// Token: 0x04002AFE RID: 11006
		Hat,
		// Token: 0x04002AFF RID: 11007
		Shirt,
		// Token: 0x04002B00 RID: 11008
		Face,
		// Token: 0x04002B01 RID: 11009
		Pants,
		// Token: 0x04002B02 RID: 11010
		Length
	}
}
