using System;
using Fusion;
using UnityEngine;

// Token: 0x0200085A RID: 2138
[NetworkBehaviourWeaved(0)]
internal class VrrigReliableSerializer : GorillaWrappedSerializer
{
	// Token: 0x06003747 RID: 14151 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void OnBeforeDespawn()
	{
	}

	// Token: 0x06003748 RID: 14152 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void OnFailedSpawn()
	{
	}

	// Token: 0x06003749 RID: 14153 RVA: 0x0012FCC0 File Offset: 0x0012DEC0
	protected override bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		if (wrappedInfo.punInfo.Sender != wrappedInfo.punInfo.photonView.Owner || wrappedInfo.punInfo.photonView.IsRoomView)
		{
			return false;
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(wrappedInfo.Sender, out rigContainer))
		{
			outTargetObject = rigContainer.gameObject;
			outTargetType = typeof(VRRigReliableState);
			return true;
		}
		return false;
	}

	// Token: 0x0600374A RID: 14154 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void OnSuccesfullySpawned(PhotonMessageInfoWrapped info)
	{
	}

	// Token: 0x0600374C RID: 14156 RVA: 0x0012D761 File Offset: 0x0012B961
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x0600374D RID: 14157 RVA: 0x0012D76D File Offset: 0x0012B96D
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
