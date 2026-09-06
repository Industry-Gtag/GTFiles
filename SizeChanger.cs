using System;
using System.Collections.Generic;
using GT_CustomMapSupportRuntime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000925 RID: 2341
public class SizeChanger : GorillaTriggerBox
{
	// Token: 0x170005A1 RID: 1441
	// (get) Token: 0x06003D4A RID: 15690 RVA: 0x0014D270 File Offset: 0x0014B470
	public int SizeLayerMask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x170005A2 RID: 1442
	// (get) Token: 0x06003D4B RID: 15691 RVA: 0x0014D2B0 File Offset: 0x0014B4B0
	public SizeChanger.ChangerType MyType
	{
		get
		{
			return this.myType;
		}
	}

	// Token: 0x170005A3 RID: 1443
	// (get) Token: 0x06003D4C RID: 15692 RVA: 0x0014D2B8 File Offset: 0x0014B4B8
	public float MaxScale
	{
		get
		{
			return this.maxScale;
		}
	}

	// Token: 0x170005A4 RID: 1444
	// (get) Token: 0x06003D4D RID: 15693 RVA: 0x0014D2C0 File Offset: 0x0014B4C0
	public float MinScale
	{
		get
		{
			return this.minScale;
		}
	}

	// Token: 0x170005A5 RID: 1445
	// (get) Token: 0x06003D4E RID: 15694 RVA: 0x0014D2C8 File Offset: 0x0014B4C8
	public Transform StartPos
	{
		get
		{
			return this.startPos;
		}
	}

	// Token: 0x170005A6 RID: 1446
	// (get) Token: 0x06003D4F RID: 15695 RVA: 0x0014D2D0 File Offset: 0x0014B4D0
	public Transform EndPos
	{
		get
		{
			return this.endPos;
		}
	}

	// Token: 0x170005A7 RID: 1447
	// (get) Token: 0x06003D50 RID: 15696 RVA: 0x0014D2D8 File Offset: 0x0014B4D8
	public float StaticEasing
	{
		get
		{
			return this.staticEasing;
		}
	}

	// Token: 0x06003D51 RID: 15697 RVA: 0x0014D2E0 File Offset: 0x0014B4E0
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
		this.myCollider = base.GetComponent<Collider>();
	}

	// Token: 0x06003D52 RID: 15698 RVA: 0x0014D304 File Offset: 0x0014B504
	public void OnEnable()
	{
		if (this.enterTrigger)
		{
			this.enterTrigger.OnEnter += this.OnTriggerEnter;
		}
		if (this.exitTrigger)
		{
			this.exitTrigger.OnExit += this.OnTriggerExit;
		}
		if (this.exitOnEnterTrigger)
		{
			this.exitOnEnterTrigger.OnEnter += this.OnTriggerExit;
		}
	}

	// Token: 0x06003D53 RID: 15699 RVA: 0x0014D380 File Offset: 0x0014B580
	public void OnDisable()
	{
		if (this.enterTrigger)
		{
			this.enterTrigger.OnEnter -= this.OnTriggerEnter;
		}
		if (this.exitTrigger)
		{
			this.exitTrigger.OnExit -= this.OnTriggerExit;
		}
		if (this.exitOnEnterTrigger)
		{
			this.exitOnEnterTrigger.OnEnter -= this.OnTriggerExit;
		}
	}

	// Token: 0x06003D54 RID: 15700 RVA: 0x0014D3F9 File Offset: 0x0014B5F9
	public void AddEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter += this.OnTriggerEnter;
		}
	}

	// Token: 0x06003D55 RID: 15701 RVA: 0x0014D415 File Offset: 0x0014B615
	public void RemoveEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter -= this.OnTriggerEnter;
		}
	}

	// Token: 0x06003D56 RID: 15702 RVA: 0x0014D431 File Offset: 0x0014B631
	public void AddExitOnEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter += this.OnTriggerExit;
		}
	}

	// Token: 0x06003D57 RID: 15703 RVA: 0x0014D44D File Offset: 0x0014B64D
	public void RemoveExitOnEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter -= this.OnTriggerExit;
		}
	}

	// Token: 0x06003D58 RID: 15704 RVA: 0x0014D46C File Offset: 0x0014B66C
	public void OnTriggerEnter(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		this.acceptRig(component);
	}

	// Token: 0x06003D59 RID: 15705 RVA: 0x0014D4A9 File Offset: 0x0014B6A9
	public void acceptRig(VRRig rig)
	{
		if (!rig.sizeManager.touchingChangers.Contains(this))
		{
			rig.sizeManager.touchingChangers.Add(this);
		}
		UnityAction onEnter = this.OnEnter;
		if (onEnter == null)
		{
			return;
		}
		onEnter();
	}

	// Token: 0x06003D5A RID: 15706 RVA: 0x0014D4E0 File Offset: 0x0014B6E0
	public void OnTriggerExit(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		this.unacceptRig(component);
	}

	// Token: 0x06003D5B RID: 15707 RVA: 0x0014D51D File Offset: 0x0014B71D
	public void unacceptRig(VRRig rig)
	{
		rig.sizeManager.touchingChangers.Remove(this);
		UnityAction onExit = this.OnExit;
		if (onExit == null)
		{
			return;
		}
		onExit();
	}

	// Token: 0x06003D5C RID: 15708 RVA: 0x0014D544 File Offset: 0x0014B744
	public Vector3 ClosestPoint(Vector3 position)
	{
		if (this.enterTrigger && this.exitTrigger)
		{
			Vector3 vector = this.enterTrigger.ClosestPoint(position);
			Vector3 vector2 = this.exitTrigger.ClosestPoint(position);
			if (Vector3.Distance(position, vector) >= Vector3.Distance(position, vector2))
			{
				return vector2;
			}
			return vector;
		}
		else
		{
			if (this.myCollider)
			{
				return this.myCollider.ClosestPoint(position);
			}
			return position;
		}
	}

	// Token: 0x06003D5D RID: 15709 RVA: 0x0014D5B4 File Offset: 0x0014B7B4
	public void SetScaleCenterPoint(Transform centerPoint)
	{
		this.scaleAwayFromPoint = centerPoint;
	}

	// Token: 0x06003D5E RID: 15710 RVA: 0x0014D5BD File Offset: 0x0014B7BD
	public bool TryGetScaleCenterPoint(out Vector3 centerPoint)
	{
		if (this.scaleAwayFromPoint != null)
		{
			centerPoint = this.scaleAwayFromPoint.position;
			return true;
		}
		centerPoint = Vector3.zero;
		return false;
	}

	// Token: 0x06003D5F RID: 15711 RVA: 0x0014D5EC File Offset: 0x0014B7EC
	public void CopyProperties(SizeChangerSettings settings)
	{
		SizeChanger.ChangerType changerType;
		switch (settings.type)
		{
		case SizeChangerSettings.ChangerType.Static:
			changerType = SizeChanger.ChangerType.Static;
			break;
		case SizeChangerSettings.ChangerType.Continuous:
			changerType = SizeChanger.ChangerType.Continuous;
			break;
		case SizeChangerSettings.ChangerType.Radius:
			changerType = SizeChanger.ChangerType.Radius;
			break;
		default:
			throw new Exception(string.Format("Unhandled SizeChangerSettings.ChangerType {0}", settings.type));
		}
		this.myType = changerType;
		this.staticEasing = settings.staticEasing;
		this.maxScale = settings.maxScale;
		this.minScale = settings.minScale;
		this.startPos = settings.startPos;
		this.endPos = settings.endPos;
		this.scaleAwayFromPoint = settings.scaleAwayFromPoint;
		this.alwaysControlWhenEntered = settings.alwaysControlWhenEntered;
		this.priority = settings.priority;
		this.startRadius = settings.startRadius;
		this.endRadius = settings.endRadius;
	}

	// Token: 0x04004E13 RID: 19987
	[SerializeField]
	private SizeChanger.ChangerType myType;

	// Token: 0x04004E14 RID: 19988
	[SerializeField]
	private float staticEasing;

	// Token: 0x04004E15 RID: 19989
	[SerializeField]
	private float maxScale;

	// Token: 0x04004E16 RID: 19990
	[SerializeField]
	private float minScale;

	// Token: 0x04004E17 RID: 19991
	private Collider myCollider;

	// Token: 0x04004E18 RID: 19992
	[SerializeField]
	private Transform startPos;

	// Token: 0x04004E19 RID: 19993
	[SerializeField]
	private Transform endPos;

	// Token: 0x04004E1A RID: 19994
	[SerializeField]
	private SizeChangerTrigger enterTrigger;

	// Token: 0x04004E1B RID: 19995
	[SerializeField]
	private SizeChangerTrigger exitTrigger;

	// Token: 0x04004E1C RID: 19996
	[SerializeField]
	private Transform scaleAwayFromPoint;

	// Token: 0x04004E1D RID: 19997
	[SerializeField]
	private SizeChangerTrigger exitOnEnterTrigger;

	// Token: 0x04004E1E RID: 19998
	public bool alwaysControlWhenEntered;

	// Token: 0x04004E1F RID: 19999
	public int priority;

	// Token: 0x04004E20 RID: 20000
	public bool aprilFoolsEnabled;

	// Token: 0x04004E21 RID: 20001
	public float startRadius;

	// Token: 0x04004E22 RID: 20002
	public float endRadius;

	// Token: 0x04004E23 RID: 20003
	public bool affectLayerA = true;

	// Token: 0x04004E24 RID: 20004
	public bool affectLayerB = true;

	// Token: 0x04004E25 RID: 20005
	public bool affectLayerC = true;

	// Token: 0x04004E26 RID: 20006
	public bool affectLayerD = true;

	// Token: 0x04004E27 RID: 20007
	public UnityAction OnExit;

	// Token: 0x04004E28 RID: 20008
	public UnityAction OnEnter;

	// Token: 0x04004E29 RID: 20009
	private HashSet<VRRig> unregisteredPresentRigs;

	// Token: 0x02000926 RID: 2342
	public enum ChangerType
	{
		// Token: 0x04004E2B RID: 20011
		Static,
		// Token: 0x04004E2C RID: 20012
		Continuous,
		// Token: 0x04004E2D RID: 20013
		Radius
	}
}
