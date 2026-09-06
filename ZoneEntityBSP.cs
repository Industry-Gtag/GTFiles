using System;
using UnityEngine;

// Token: 0x02000E98 RID: 3736
public class ZoneEntityBSP : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x140000A5 RID: 165
	// (add) Token: 0x06005ABA RID: 23226 RVA: 0x001D8C44 File Offset: 0x001D6E44
	// (remove) Token: 0x06005ABB RID: 23227 RVA: 0x001D8C78 File Offset: 0x001D6E78
	public static event ZoneEntityBSP.PlayerZoneChange onPlayerZoneChange;

	// Token: 0x170008A9 RID: 2217
	// (get) Token: 0x06005ABC RID: 23228 RVA: 0x001D8CAB File Offset: 0x001D6EAB
	public VRRig entityRig
	{
		get
		{
			return this._entityRig;
		}
	}

	// Token: 0x170008AA RID: 2218
	// (get) Token: 0x06005ABD RID: 23229 RVA: 0x001D8CB3 File Offset: 0x001D6EB3
	public GTZone currentZone
	{
		get
		{
			ZoneDef zoneDef = this.currentNode;
			if (zoneDef == null)
			{
				return GTZone.none;
			}
			return zoneDef.zoneId;
		}
	}

	// Token: 0x170008AB RID: 2219
	// (get) Token: 0x06005ABE RID: 23230 RVA: 0x001D8CC7 File Offset: 0x001D6EC7
	public GTSubZone currentSubZone
	{
		get
		{
			ZoneDef zoneDef = this.currentNode;
			if (zoneDef == null)
			{
				return GTSubZone.none;
			}
			return zoneDef.subZoneId;
		}
	}

	// Token: 0x170008AC RID: 2220
	// (get) Token: 0x06005ABF RID: 23231 RVA: 0x001D8CDC File Offset: 0x001D6EDC
	public GroupJoinZoneAB GroupZone
	{
		get
		{
			ZoneDef zoneDef = this.currentNode;
			if (zoneDef == null)
			{
				return default(GroupJoinZoneAB);
			}
			return zoneDef.groupZoneAB;
		}
	}

	// Token: 0x06005AC0 RID: 23232 RVA: 0x001D8D02 File Offset: 0x001D6F02
	private void Start()
	{
		if (!this._entityRig.isOfflineVRRig)
		{
			this._emitTelemetry = false;
		}
		this.SliceUpdate();
	}

	// Token: 0x06005AC1 RID: 23233 RVA: 0x000837F8 File Offset: 0x000819F8
	public virtual void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
	}

	// Token: 0x06005AC2 RID: 23234 RVA: 0x00083801 File Offset: 0x00081A01
	public virtual void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
	}

	// Token: 0x06005AC3 RID: 23235 RVA: 0x001D8D20 File Offset: 0x001D6F20
	public void SliceUpdate()
	{
		if (this.isUpdateDisabled)
		{
			return;
		}
		ZoneDef zoneDef = ZoneGraphBSP.Instance.FindZoneAtPoint(base.transform.position);
		if (!zoneDef.IsSameZone(this.currentNode))
		{
			this.lastExitedNode = this.currentNode;
			this.currentNode = zoneDef;
			this.lastEnteredNode = zoneDef;
			if (this._entityRig != null)
			{
				bool isOfflineVRRig = this._entityRig.isOfflineVRRig;
			}
			GTZone gtzone = (this.lastExitedNode ? this.lastExitedNode.zoneId : GTZone.none);
			GTZone gtzone2 = (zoneDef ? zoneDef.zoneId : GTZone.none);
			if (gtzone != gtzone2)
			{
				ZoneEntityBSP.PlayerZoneChange playerZoneChange = ZoneEntityBSP.onPlayerZoneChange;
				if (playerZoneChange != null)
				{
					playerZoneChange(this._entityRig, gtzone, gtzone2);
				}
			}
			if (this._emitTelemetry)
			{
				ZoneDef zoneDef2 = this.lastEnteredNode;
				if (zoneDef2 != null && zoneDef2.trackEnter)
				{
					GorillaTelemetry.EnqueueZoneEvent(this.lastEnteredNode, GTZoneEventType.zone_enter);
				}
				ZoneDef zoneDef3 = this.lastExitedNode;
				if (zoneDef3 != null && zoneDef3.trackExit)
				{
					GorillaTelemetry.EnqueueZoneEvent(this.lastExitedNode, GTZoneEventType.zone_exit);
					return;
				}
			}
		}
		else if (this._emitTelemetry)
		{
			ZoneDef zoneDef4 = this.currentNode;
			if (zoneDef4 != null && zoneDef4.trackStay)
			{
				GorillaTelemetry.EnqueueZoneEvent(this.currentNode, GTZoneEventType.zone_stay);
			}
		}
	}

	// Token: 0x06005AC4 RID: 23236 RVA: 0x001D8E4E File Offset: 0x001D704E
	public void EnableZoneChanges()
	{
		this.isUpdateDisabled = false;
	}

	// Token: 0x06005AC5 RID: 23237 RVA: 0x001D8E57 File Offset: 0x001D7057
	public void DisableZoneChanges()
	{
		this.isUpdateDisabled = true;
	}

	// Token: 0x04006BC2 RID: 27586
	[Space]
	[SerializeField]
	private bool _emitTelemetry = true;

	// Token: 0x04006BC3 RID: 27587
	[Space]
	[SerializeField]
	private VRRig _entityRig;

	// Token: 0x04006BC4 RID: 27588
	[Space]
	[NonSerialized]
	public ZoneDef currentNode;

	// Token: 0x04006BC5 RID: 27589
	[NonSerialized]
	public ZoneDef lastEnteredNode;

	// Token: 0x04006BC6 RID: 27590
	[NonSerialized]
	public ZoneDef lastExitedNode;

	// Token: 0x04006BC7 RID: 27591
	private bool isUpdateDisabled;

	// Token: 0x02000E99 RID: 3737
	// (Invoke) Token: 0x06005AC8 RID: 23240
	public delegate void PlayerZoneChange(VRRig rig, GTZone fromZone, GTZone toZone);
}
