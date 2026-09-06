using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001207 RID: 4615
	[RequireComponent(typeof(VRRigCollection))]
	public class CosmeticTryOnNotifier : MonoBehaviour
	{
		// Token: 0x060074FE RID: 29950 RVA: 0x0025FCA8 File Offset: 0x0025DEA8
		private void Awake()
		{
			if (!base.TryGetComponent<VRRigCollection>(out this.m_vrrigCollection))
			{
				this.m_vrrigCollection = this.AddComponent<VRRigCollection>();
			}
			VRRigCollection vrrigCollection = this.m_vrrigCollection;
			vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.PlayerEnteredTryOnSpace));
			VRRigCollection vrrigCollection2 = this.m_vrrigCollection;
			vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.PlayerLeftTryOnSpace));
		}

		// Token: 0x060074FF RID: 29951 RVA: 0x0025FD20 File Offset: 0x0025DF20
		private void PlayerEnteredTryOnSpace(RigContainer playerRig)
		{
			CosmeticTryOnNotifier.Mode mode = this.mode;
			if (mode == CosmeticTryOnNotifier.Mode.TRY_ON)
			{
				PlayerCosmeticsSystem.SetRigTryOn(true, playerRig);
				return;
			}
			if (mode != CosmeticTryOnNotifier.Mode.ENABLE_LIST)
			{
				return;
			}
			PlayerCosmeticsSystem.SetRigTemporarySpace(true, playerRig, this.unlockList.Strings);
		}

		// Token: 0x06007500 RID: 29952 RVA: 0x0025FD58 File Offset: 0x0025DF58
		private void PlayerLeftTryOnSpace(RigContainer playerRig)
		{
			CosmeticTryOnNotifier.Mode mode = this.mode;
			if (mode == CosmeticTryOnNotifier.Mode.TRY_ON)
			{
				PlayerCosmeticsSystem.SetRigTryOn(false, playerRig);
				return;
			}
			if (mode != CosmeticTryOnNotifier.Mode.ENABLE_LIST)
			{
				return;
			}
			PlayerCosmeticsSystem.SetRigTemporarySpace(false, playerRig, this.unlockList.Strings);
		}

		// Token: 0x040084AC RID: 33964
		private VRRigCollection m_vrrigCollection;

		// Token: 0x040084AD RID: 33965
		[SerializeField]
		private CosmeticTryOnNotifier.Mode mode;

		// Token: 0x040084AE RID: 33966
		[SerializeField]
		private StringList unlockList;

		// Token: 0x02001208 RID: 4616
		private enum Mode
		{
			// Token: 0x040084B0 RID: 33968
			TRY_ON,
			// Token: 0x040084B1 RID: 33969
			ENABLE_LIST,
			// Token: 0x040084B2 RID: 33970
			ENABLE_LIST_TITLEDATA
		}
	}
}
