using System;
using Photon.Pun;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000010 RID: 16
public class ArcadeMachineJoystick : HandHold, ISnapTurnOverride, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x1700000A RID: 10
	// (get) Token: 0x0600002E RID: 46 RVA: 0x00002AEA File Offset: 0x00000CEA
	// (set) Token: 0x0600002F RID: 47 RVA: 0x00002AF2 File Offset: 0x00000CF2
	public bool heldByLocalPlayer { get; private set; }

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x06000030 RID: 48 RVA: 0x00002AFB File Offset: 0x00000CFB
	public bool IsHeldLeftHanded
	{
		get
		{
			return this.heldByLocalPlayer && this.xrNode == XRNode.LeftHand;
		}
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x06000031 RID: 49 RVA: 0x00002B10 File Offset: 0x00000D10
	// (set) Token: 0x06000032 RID: 50 RVA: 0x00002B18 File Offset: 0x00000D18
	public ArcadeButtons currentButtonState { get; private set; }

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x06000033 RID: 51 RVA: 0x00002B21 File Offset: 0x00000D21
	// (set) Token: 0x06000034 RID: 52 RVA: 0x00002B29 File Offset: 0x00000D29
	public int player { get; private set; }

	// Token: 0x06000035 RID: 53 RVA: 0x00002B32 File Offset: 0x00000D32
	public void Init(ArcadeMachine machine, int player)
	{
		this.machine = machine;
		this.player = player;
		this.guard = base.GetComponent<RequestableOwnershipGuard>();
		this.guard.AddCallbackTarget(this);
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00002B5C File Offset: 0x00000D5C
	public void BindController(bool leftHand)
	{
		this.xrNode = (leftHand ? XRNode.LeftHand : XRNode.RightHand);
		this.heldByLocalPlayer = true;
		if (!leftHand)
		{
			if (!this.snapTurn)
			{
				this.snapTurn = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
			}
			if (this.snapTurn != null)
			{
				this.snapTurnOverride = true;
				this.snapTurn.SetTurningOverride(this);
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.guard.TransferOwnership(PhotonNetwork.LocalPlayer, "");
		}
		else if (!this.guard.isMine)
		{
			this.guard.RequestOwnership(new Action(this.OnOwnershipSuccess), new Action(this.OnOwnershipFail));
		}
		ControllerInputPoller.AddUpdateCallback(new Action(this.OnInputUpdate));
		PlayerGameEvents.MiscEvent("PlayArcadeGame", 1);
	}

	// Token: 0x06000037 RID: 55 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnOwnershipSuccess()
	{
	}

	// Token: 0x06000038 RID: 56 RVA: 0x00002C2F File Offset: 0x00000E2F
	private void OnOwnershipFail()
	{
		this.ForceRelease();
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00002C37 File Offset: 0x00000E37
	public void UnbindController()
	{
		this.heldByLocalPlayer = false;
		if (this.snapTurnOverride)
		{
			this.snapTurnOverride = false;
			this.snapTurn.UnsetTurningOverride(this);
		}
		this.OnInputUpdate();
		ControllerInputPoller.RemoveUpdateCallback(new Action(this.OnInputUpdate));
	}

	// Token: 0x0600003A RID: 58 RVA: 0x00002C74 File Offset: 0x00000E74
	private void OnInputUpdate()
	{
		ArcadeButtons arcadeButtons = (ArcadeButtons)0;
		if (this.heldByLocalPlayer)
		{
			arcadeButtons |= ArcadeButtons.GRAB;
			if (ControllerInputPoller.Primary2DAxis(this.xrNode).y > 0.5f)
			{
				arcadeButtons |= ArcadeButtons.UP;
			}
			if (ControllerInputPoller.Primary2DAxis(this.xrNode).y < -0.5f)
			{
				arcadeButtons |= ArcadeButtons.DOWN;
			}
			if (ControllerInputPoller.Primary2DAxis(this.xrNode).x < -0.5f)
			{
				arcadeButtons |= ArcadeButtons.LEFT;
			}
			if (ControllerInputPoller.Primary2DAxis(this.xrNode).x > 0.5f)
			{
				arcadeButtons |= ArcadeButtons.RIGHT;
			}
			if (ControllerInputPoller.PrimaryButtonPress(this.xrNode))
			{
				arcadeButtons |= ArcadeButtons.B0;
			}
			if (ControllerInputPoller.SecondaryButtonPress(this.xrNode))
			{
				arcadeButtons |= ArcadeButtons.B1;
			}
			if (ControllerInputPoller.TriggerFloat(this.xrNode) > 0.5f)
			{
				arcadeButtons |= ArcadeButtons.TRIGGER;
			}
		}
		if (arcadeButtons != this.currentButtonState)
		{
			this.machine.OnJoystickStateChange(this.player, arcadeButtons);
		}
		this.currentButtonState = arcadeButtons;
	}

	// Token: 0x0600003B RID: 59 RVA: 0x00002D60 File Offset: 0x00000F60
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != info.photonView.Owner)
		{
			return;
		}
		ArcadeButtons arcadeButtons = (ArcadeButtons)((int)stream.ReceiveNext());
		if (arcadeButtons != this.currentButtonState && this.machine != null)
		{
			this.machine.OnJoystickStateChange(this.player, arcadeButtons);
		}
		this.currentButtonState = arcadeButtons;
		this.machine.ReadPlayerDataPUN(this.player, stream, info);
	}

	// Token: 0x0600003C RID: 60 RVA: 0x00002DD0 File Offset: 0x00000FD0
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext((int)this.currentButtonState);
		this.machine.WritePlayerDataPUN(this.player, stream, info);
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void ReceiveRemoteState(ArcadeButtons newState)
	{
	}

	// Token: 0x0600003E RID: 62 RVA: 0x00002DF6 File Offset: 0x00000FF6
	public bool TurnOverrideActive()
	{
		return this.snapTurnOverride;
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00002DFE File Offset: 0x00000FFE
	public override bool CanBeGrabbed(GorillaGrabber grabber)
	{
		return !this.machine.IsControllerInUse(this.player);
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00002E14 File Offset: 0x00001014
	public void ForceRelease()
	{
		this.heldByLocalPlayer = false;
		this.currentButtonState = (ArcadeButtons)0;
	}

	// Token: 0x06000041 RID: 65 RVA: 0x00002E24 File Offset: 0x00001024
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (this.heldByLocalPlayer && (toPlayer == null || !toPlayer.IsLocal))
		{
			this.ForceRelease();
		}
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00002E3F File Offset: 0x0000103F
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return !this.heldByLocalPlayer;
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00002E3F File Offset: 0x0000103F
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return !this.heldByLocalPlayer;
	}

	// Token: 0x06000044 RID: 68 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x06000045 RID: 69 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x04000028 RID: 40
	private XRNode xrNode;

	// Token: 0x0400002C RID: 44
	private ArcadeMachine machine;

	// Token: 0x0400002D RID: 45
	private RequestableOwnershipGuard guard;

	// Token: 0x0400002E RID: 46
	private GorillaSnapTurn snapTurn;

	// Token: 0x0400002F RID: 47
	private bool snapTurnOverride;
}
