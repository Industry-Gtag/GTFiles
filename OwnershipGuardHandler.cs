using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000CD6 RID: 3286
internal class OwnershipGuardHandler : IPunOwnershipCallbacks
{
	// Token: 0x0600516A RID: 20842 RVA: 0x001AF95A File Offset: 0x001ADB5A
	static OwnershipGuardHandler()
	{
		PhotonNetwork.AddCallbackTarget(OwnershipGuardHandler.callbackInstance);
	}

	// Token: 0x0600516B RID: 20843 RVA: 0x001AF97A File Offset: 0x001ADB7A
	internal static void RegisterView(PhotonView view)
	{
		if (view == null || OwnershipGuardHandler.guardedViews.Contains(view))
		{
			return;
		}
		OwnershipGuardHandler.guardedViews.Add(view);
	}

	// Token: 0x0600516C RID: 20844 RVA: 0x001AF9A0 File Offset: 0x001ADBA0
	internal static void RegisterViews(PhotonView[] photonViews)
	{
		for (int i = 0; i < photonViews.Length; i++)
		{
			OwnershipGuardHandler.RegisterView(photonViews[i]);
		}
	}

	// Token: 0x0600516D RID: 20845 RVA: 0x001AF9C5 File Offset: 0x001ADBC5
	internal static void RemoveView(PhotonView view)
	{
		if (view == null)
		{
			return;
		}
		OwnershipGuardHandler.guardedViews.Remove(view);
	}

	// Token: 0x0600516E RID: 20846 RVA: 0x001AF9E0 File Offset: 0x001ADBE0
	internal static void RemoveViews(PhotonView[] photonViews)
	{
		for (int i = 0; i < photonViews.Length; i++)
		{
			OwnershipGuardHandler.RemoveView(photonViews[i]);
		}
	}

	// Token: 0x0600516F RID: 20847 RVA: 0x001AFA08 File Offset: 0x001ADC08
	void IPunOwnershipCallbacks.OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
		if (!OwnershipGuardHandler.guardedViews.Contains(targetView))
		{
			return;
		}
		if (targetView.IsRoomView)
		{
			if (targetView.Owner != PhotonNetwork.MasterClient)
			{
				targetView.OwnerActorNr = 0;
				targetView.ControllerActorNr = 0;
				return;
			}
		}
		else if (targetView.OwnerActorNr != targetView.CreatorActorNr || targetView.ControllerActorNr != targetView.CreatorActorNr)
		{
			targetView.OwnerActorNr = targetView.CreatorActorNr;
			targetView.ControllerActorNr = targetView.CreatorActorNr;
		}
	}

	// Token: 0x06005170 RID: 20848 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	// Token: 0x06005171 RID: 20849 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IPunOwnershipCallbacks.OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	// Token: 0x04006374 RID: 25460
	private static HashSet<PhotonView> guardedViews = new HashSet<PhotonView>();

	// Token: 0x04006375 RID: 25461
	private static readonly OwnershipGuardHandler callbackInstance = new OwnershipGuardHandler();
}
