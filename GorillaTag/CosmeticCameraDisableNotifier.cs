using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001206 RID: 4614
	[RequireComponent(typeof(VRRigCollection))]
	public class CosmeticCameraDisableNotifier : MonoBehaviour
	{
		// Token: 0x060074FA RID: 29946 RVA: 0x0025FBFC File Offset: 0x0025DDFC
		private void Awake()
		{
			if (!base.TryGetComponent<VRRigCollection>(out this._vrrigCollection))
			{
				this._vrrigCollection = this.AddComponent<VRRigCollection>();
			}
			VRRigCollection vrrigCollection = this._vrrigCollection;
			vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.PlayerEnteredTryOnSpace));
			VRRigCollection vrrigCollection2 = this._vrrigCollection;
			vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.PlayerLeftTryOnSpace));
		}

		// Token: 0x060074FB RID: 29947 RVA: 0x0025FC71 File Offset: 0x0025DE71
		private void PlayerEnteredTryOnSpace(RigContainer playerRig)
		{
			if (playerRig.Rig.isLocal)
			{
				this._cosmeticCamera.enabled = false;
			}
		}

		// Token: 0x060074FC RID: 29948 RVA: 0x0025FC8C File Offset: 0x0025DE8C
		private void PlayerLeftTryOnSpace(RigContainer playerRig)
		{
			if (playerRig.Rig.isLocal)
			{
				this._cosmeticCamera.enabled = true;
			}
		}

		// Token: 0x040084AA RID: 33962
		private VRRigCollection _vrrigCollection;

		// Token: 0x040084AB RID: 33963
		[SerializeField]
		private Camera _cosmeticCamera;
	}
}
