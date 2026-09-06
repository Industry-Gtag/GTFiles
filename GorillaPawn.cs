using System;
using UnityEngine;

// Token: 0x020008A1 RID: 2209
[Obsolete]
public class GorillaPawn : MonoBehaviour
{
	// Token: 0x17000522 RID: 1314
	// (get) Token: 0x060039B1 RID: 14769 RVA: 0x0013A52E File Offset: 0x0013872E
	public VRRig rig
	{
		get
		{
			return this._rig;
		}
	}

	// Token: 0x17000523 RID: 1315
	// (get) Token: 0x060039B2 RID: 14770 RVA: 0x0013A536 File Offset: 0x00138736
	public ZoneEntityBSP zoneEntity
	{
		get
		{
			return this._zoneEntity;
		}
	}

	// Token: 0x17000524 RID: 1316
	// (get) Token: 0x060039B3 RID: 14771 RVA: 0x0013A53E File Offset: 0x0013873E
	public new Transform transform
	{
		get
		{
			return this._transform;
		}
	}

	// Token: 0x17000525 RID: 1317
	// (get) Token: 0x060039B4 RID: 14772 RVA: 0x0013A546 File Offset: 0x00138746
	public XformNode handLeft
	{
		get
		{
			return this._handLeftXform;
		}
	}

	// Token: 0x17000526 RID: 1318
	// (get) Token: 0x060039B5 RID: 14773 RVA: 0x0013A54E File Offset: 0x0013874E
	public XformNode handRight
	{
		get
		{
			return this._handRightXform;
		}
	}

	// Token: 0x17000527 RID: 1319
	// (get) Token: 0x060039B6 RID: 14774 RVA: 0x0013A556 File Offset: 0x00138756
	public XformNode body
	{
		get
		{
			return this._bodyXform;
		}
	}

	// Token: 0x17000528 RID: 1320
	// (get) Token: 0x060039B7 RID: 14775 RVA: 0x0013A55E File Offset: 0x0013875E
	public XformNode head
	{
		get
		{
			return this._headXform;
		}
	}

	// Token: 0x060039B8 RID: 14776 RVA: 0x0013A566 File Offset: 0x00138766
	private void Awake()
	{
		this.Setup(false);
	}

	// Token: 0x060039B9 RID: 14777 RVA: 0x0013A570 File Offset: 0x00138770
	private void Setup(bool force)
	{
		this._transform = base.transform;
		this._rig = base.GetComponentInChildren<VRRig>();
		if (!this._rig)
		{
			return;
		}
		this._zoneEntity = this._rig.zoneEntity;
		bool flag = force || this._handLeft.AsNull<Transform>() == null;
		bool flag2 = force || this._handRight.AsNull<Transform>() == null;
		bool flag3 = force || this._head.AsNull<Transform>() == null;
		if (!flag && !flag2 && !flag3)
		{
			return;
		}
		foreach (Transform transform in this._rig.mainSkin.bones)
		{
			string name = transform.name;
			if (flag3 && name.StartsWith("head", StringComparison.OrdinalIgnoreCase))
			{
				this._head = transform;
				this._headXform = new XformNode();
				this._headXform.localPosition = new Vector3(0f, 0.13f, 0.015f);
				this._headXform.radius = 0.12f;
				this._headXform.parent = transform;
			}
			else if (flag && name.StartsWith("hand.L", StringComparison.OrdinalIgnoreCase))
			{
				this._handLeft = transform;
				this._handLeftXform = new XformNode();
				this._handLeftXform.localPosition = new Vector3(-0.014f, 0.034f, 0f);
				this._handLeftXform.radius = 0.044f;
				this._handLeftXform.parent = transform;
			}
			else if (flag2 && name.StartsWith("hand.R", StringComparison.OrdinalIgnoreCase))
			{
				this._handRight = transform;
				this._handRightXform = new XformNode();
				this._handRightXform.localPosition = new Vector3(0.014f, 0.034f, 0f);
				this._handRightXform.radius = 0.044f;
				this._handRightXform.parent = transform;
			}
		}
	}

	// Token: 0x060039BA RID: 14778 RVA: 0x0013A777 File Offset: 0x00138977
	private bool CanRun()
	{
		if (GorillaPawn._gPawnActiveCount > 10)
		{
			Debug.LogError(string.Format("Cannot register more than {0} pawns.", 10));
			return false;
		}
		return true;
	}

	// Token: 0x060039BB RID: 14779 RVA: 0x0013A79C File Offset: 0x0013899C
	private void OnEnable()
	{
		if (!this.CanRun())
		{
			return;
		}
		this._id = -1;
		if (this._rig && this._rig.OwningNetPlayer != null)
		{
			this._id = this._rig.OwningNetPlayer.ActorNumber;
		}
		this._index = GorillaPawn._gPawnActiveCount++;
		GorillaPawn._gPawns[this._index] = this;
	}

	// Token: 0x060039BC RID: 14780 RVA: 0x0013A80C File Offset: 0x00138A0C
	private void OnDisable()
	{
		this._id = -1;
		if (!this.CanRun())
		{
			return;
		}
		if (this._index < 0 || this._index >= GorillaPawn._gPawnActiveCount - 1)
		{
			return;
		}
		int num = --GorillaPawn._gPawnActiveCount;
		GorillaPawn._gPawns.Swap(this._index, num);
		this._index = num;
	}

	// Token: 0x060039BD RID: 14781 RVA: 0x0013A868 File Offset: 0x00138A68
	private void OnDestroy()
	{
		int num = GorillaPawn._gPawns.IndexOfRef(this);
		GorillaPawn._gPawns[num] = null;
		Array.Sort<GorillaPawn>(GorillaPawn._gPawns, new Comparison<GorillaPawn>(GorillaPawn.ComparePawns));
		int num2 = 0;
		while (num2 < GorillaPawn._gPawns.Length && GorillaPawn._gPawns[num2])
		{
			num2++;
		}
		GorillaPawn._gPawnActiveCount = num2;
	}

	// Token: 0x060039BE RID: 14782 RVA: 0x0013A8C8 File Offset: 0x00138AC8
	private static int ComparePawns(GorillaPawn x, GorillaPawn y)
	{
		bool flag = x.AsNull<GorillaPawn>() == null;
		bool flag2 = y.AsNull<GorillaPawn>() == null;
		if (flag && flag2)
		{
			return 0;
		}
		if (flag)
		{
			return 1;
		}
		if (flag2)
		{
			return -1;
		}
		return x._index.CompareTo(y._index);
	}

	// Token: 0x17000529 RID: 1321
	// (get) Token: 0x060039BF RID: 14783 RVA: 0x0013A911 File Offset: 0x00138B11
	public static GorillaPawn[] AllPawns
	{
		get
		{
			return GorillaPawn._gPawns;
		}
	}

	// Token: 0x1700052A RID: 1322
	// (get) Token: 0x060039C0 RID: 14784 RVA: 0x0013A918 File Offset: 0x00138B18
	public static int ActiveCount
	{
		get
		{
			return GorillaPawn._gPawnActiveCount;
		}
	}

	// Token: 0x1700052B RID: 1323
	// (get) Token: 0x060039C1 RID: 14785 RVA: 0x0013A91F File Offset: 0x00138B1F
	public static Matrix4x4[] ShaderData
	{
		get
		{
			return GorillaPawn._gShaderData;
		}
	}

	// Token: 0x060039C2 RID: 14786 RVA: 0x0013A928 File Offset: 0x00138B28
	public static void SyncPawnData()
	{
		Matrix4x4[] gShaderData = GorillaPawn._gShaderData;
		m4x4 m4x = default(m4x4);
		for (int i = 0; i < GorillaPawn._gPawnActiveCount; i++)
		{
			GorillaPawn gorillaPawn = GorillaPawn._gPawns[i];
			Vector4 worldPosition = gorillaPawn._headXform.worldPosition;
			Vector4 worldPosition2 = gorillaPawn._bodyXform.worldPosition;
			Vector4 worldPosition3 = gorillaPawn._handLeftXform.worldPosition;
			Vector4 worldPosition4 = gorillaPawn._handRightXform.worldPosition;
			m4x.SetRow0(ref worldPosition);
			m4x.SetRow1(ref worldPosition2);
			m4x.SetRow2(ref worldPosition3);
			m4x.SetRow3(ref worldPosition4);
			m4x.Push(ref gShaderData[i]);
		}
		for (int j = GorillaPawn._gPawnActiveCount; j < 10; j++)
		{
			MatrixUtils.Clear(ref gShaderData[j]);
		}
	}

	// Token: 0x040049C5 RID: 18885
	[SerializeField]
	private Transform _transform;

	// Token: 0x040049C6 RID: 18886
	[SerializeField]
	private Transform _handLeft;

	// Token: 0x040049C7 RID: 18887
	[SerializeField]
	private Transform _handRight;

	// Token: 0x040049C8 RID: 18888
	[SerializeField]
	private Transform _head;

	// Token: 0x040049C9 RID: 18889
	[Space]
	[SerializeField]
	private VRRig _rig;

	// Token: 0x040049CA RID: 18890
	[SerializeField]
	private ZoneEntityBSP _zoneEntity;

	// Token: 0x040049CB RID: 18891
	[Space]
	[SerializeField]
	private XformNode _handLeftXform;

	// Token: 0x040049CC RID: 18892
	[SerializeField]
	private XformNode _handRightXform;

	// Token: 0x040049CD RID: 18893
	[SerializeField]
	private XformNode _bodyXform;

	// Token: 0x040049CE RID: 18894
	[SerializeField]
	private XformNode _headXform;

	// Token: 0x040049CF RID: 18895
	[Space]
	private int _id;

	// Token: 0x040049D0 RID: 18896
	private int _index;

	// Token: 0x040049D1 RID: 18897
	private bool _invalid;

	// Token: 0x040049D2 RID: 18898
	public const int MAX_PAWNS = 10;

	// Token: 0x040049D3 RID: 18899
	private static GorillaPawn[] _gPawns = new GorillaPawn[10];

	// Token: 0x040049D4 RID: 18900
	private static int _gPawnActiveCount = 0;

	// Token: 0x040049D5 RID: 18901
	private static Matrix4x4[] _gShaderData = new Matrix4x4[10];
}
