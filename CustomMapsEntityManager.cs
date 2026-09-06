using System;
using Fusion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000A74 RID: 2676
[NetworkBehaviourWeaved(0)]
public class CustomMapsEntityManager : GameEntityManager
{
	// Token: 0x060044E1 RID: 17633 RVA: 0x00170394 File Offset: 0x0016E594
	private static bool IsOverrideEnabled()
	{
		GorillaServer instance = GorillaServer.Instance;
		return instance != null && instance.CheckIsVStumpGrabbablesFixEnabled();
	}

	// Token: 0x060044E2 RID: 17634 RVA: 0x001703BA File Offset: 0x0016E5BA
	public override bool IsPositionInManagerBounds(Vector3 pos)
	{
		return (CustomMapLoader.CanLoadEntities && CustomMapsEntityManager.IsOverrideEnabled()) || base.IsPositionInManagerBounds(pos);
	}

	// Token: 0x060044E3 RID: 17635 RVA: 0x001703D4 File Offset: 0x0016E5D4
	protected override bool IsInZone()
	{
		if (!CustomMapLoader.CanLoadEntities || !CustomMapsEntityManager.IsOverrideEnabled())
		{
			return base.IsInZone();
		}
		bool flag = true;
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			flag &= this.zoneComponents[i].IsZoneReady();
		}
		return flag;
	}

	// Token: 0x060044E5 RID: 17637 RVA: 0x0017042B File Offset: 0x0016E62B
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x060044E6 RID: 17638 RVA: 0x00170437 File Offset: 0x0016E637
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
