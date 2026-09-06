using System;
using GorillaLocomotion.Gameplay;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200092A RID: 2346
public class SizeLayerChangerGrabable : MonoBehaviour, IGorillaGrabable
{
	// Token: 0x06003D78 RID: 15736 RVA: 0x0014D996 File Offset: 0x0014BB96
	public bool MomentaryGrabOnly()
	{
		return this.momentaryGrabOnly;
	}

	// Token: 0x06003D79 RID: 15737 RVA: 0x00023F0C File Offset: 0x0002210C
	bool IGorillaGrabable.CanBeGrabbed(GorillaGrabber grabber)
	{
		return true;
	}

	// Token: 0x06003D7A RID: 15738 RVA: 0x0014D9A0 File Offset: 0x0014BBA0
	void IGorillaGrabable.OnGrabbed(GorillaGrabber g, out Transform grabbedObject, out Vector3 grabbedLocalPosiiton)
	{
		if (this.grabChangesSizeLayer)
		{
			RigContainer rigContainer;
			VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer);
			rigContainer.Rig.sizeManager.currentSizeLayerMaskValue = this.grabbedSizeLayerMask.Mask;
		}
		grabbedObject = base.transform;
		grabbedLocalPosiiton = base.transform.InverseTransformPoint(g.transform.position);
	}

	// Token: 0x06003D7B RID: 15739 RVA: 0x0014DA08 File Offset: 0x0014BC08
	void IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
	{
		if (this.releaseChangesSizeLayer)
		{
			RigContainer rigContainer;
			VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer);
			rigContainer.Rig.sizeManager.currentSizeLayerMaskValue = this.releasedSizeLayerMask.Mask;
		}
	}

	// Token: 0x06003D7D RID: 15741 RVA: 0x00014B5B File Offset: 0x00012D5B
	string IGorillaGrabable.get_name()
	{
		return base.name;
	}

	// Token: 0x04004E3D RID: 20029
	[SerializeField]
	private bool grabChangesSizeLayer = true;

	// Token: 0x04004E3E RID: 20030
	[SerializeField]
	private bool releaseChangesSizeLayer = true;

	// Token: 0x04004E3F RID: 20031
	[SerializeField]
	private SizeLayerMask grabbedSizeLayerMask;

	// Token: 0x04004E40 RID: 20032
	[SerializeField]
	private SizeLayerMask releasedSizeLayerMask;

	// Token: 0x04004E41 RID: 20033
	[SerializeField]
	private bool momentaryGrabOnly = true;
}
