using System;
using System.Collections.Generic;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x0200065D RID: 1629
public class BuilderRoomBoundary : GorillaTriggerBox
{
	// Token: 0x0600289A RID: 10394 RVA: 0x000DB980 File Offset: 0x000D9B80
	private void Awake()
	{
		foreach (SizeChangerTrigger sizeChangerTrigger in this.enableOnEnterTrigger)
		{
			sizeChangerTrigger.OnEnter += this.OnEnteredBoundary;
		}
		this.disableOnExitTrigger.OnExit += this.OnExitedBoundary;
	}

	// Token: 0x0600289B RID: 10395 RVA: 0x000DB9F4 File Offset: 0x000D9BF4
	private void OnDestroy()
	{
		foreach (SizeChangerTrigger sizeChangerTrigger in this.enableOnEnterTrigger)
		{
			sizeChangerTrigger.OnEnter -= this.OnEnteredBoundary;
		}
		this.disableOnExitTrigger.OnExit -= this.OnExitedBoundary;
	}

	// Token: 0x0600289C RID: 10396 RVA: 0x000DBA68 File Offset: 0x000D9C68
	public void OnEnteredBoundary(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null || !this.rigRef.isOfflineVRRig)
		{
			return;
		}
		BuilderTable builderTable;
		if (!BuilderTable.TryGetBuilderTableForZone(this.rigRef.zoneEntity.currentZone, out builderTable))
		{
			return;
		}
		if (!ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks))
		{
			return;
		}
		this.rigRef.EnableBuilderResizeWatch(true);
	}

	// Token: 0x0600289D RID: 10397 RVA: 0x000DBAEC File Offset: 0x000D9CEC
	public void OnExitedBoundary(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null || !this.rigRef.isOfflineVRRig)
		{
			return;
		}
		this.rigRef.EnableBuilderResizeWatch(false);
	}

	// Token: 0x040034E7 RID: 13543
	[SerializeField]
	private List<SizeChangerTrigger> enableOnEnterTrigger;

	// Token: 0x040034E8 RID: 13544
	[SerializeField]
	private SizeChangerTrigger disableOnExitTrigger;

	// Token: 0x040034E9 RID: 13545
	private VRRig rigRef;
}
