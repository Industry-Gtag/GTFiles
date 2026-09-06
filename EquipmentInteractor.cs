using System;
using System.Collections.Generic;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020004AD RID: 1197
public class EquipmentInteractor : MonoBehaviour
{
	// Token: 0x17000315 RID: 789
	// (get) Token: 0x06001D1A RID: 7450 RVA: 0x0009DD43 File Offset: 0x0009BF43
	public GorillaHandClimber BodyClimber
	{
		get
		{
			return this.bodyClimber;
		}
	}

	// Token: 0x17000316 RID: 790
	// (get) Token: 0x06001D1B RID: 7451 RVA: 0x0009DD4B File Offset: 0x0009BF4B
	public GorillaHandClimber LeftClimber
	{
		get
		{
			return this.leftClimber;
		}
	}

	// Token: 0x17000317 RID: 791
	// (get) Token: 0x06001D1C RID: 7452 RVA: 0x0009DD53 File Offset: 0x0009BF53
	public GorillaHandClimber RightClimber
	{
		get
		{
			return this.rightClimber;
		}
	}

	// Token: 0x06001D1D RID: 7453 RVA: 0x0009DD5C File Offset: 0x0009BF5C
	private void Awake()
	{
		if (EquipmentInteractor.instance == null)
		{
			EquipmentInteractor.instance = this;
			EquipmentInteractor.hasInstance = true;
		}
		else if (EquipmentInteractor.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.autoGrabLeft = true;
		this.autoGrabRight = true;
	}

	// Token: 0x06001D1E RID: 7454 RVA: 0x0009DDB0 File Offset: 0x0009BFB0
	private void OnDestroy()
	{
		if (EquipmentInteractor.instance == this)
		{
			EquipmentInteractor.hasInstance = false;
			EquipmentInteractor.instance = null;
		}
	}

	// Token: 0x06001D1F RID: 7455 RVA: 0x0009DDCF File Offset: 0x0009BFCF
	public void ReleaseRightHand()
	{
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.OnRelease(null, this.rightHand);
		}
		if (this.leftHandHeldEquipment != null)
		{
			this.leftHandHeldEquipment.OnRelease(null, this.rightHand);
		}
		this.autoGrabRight = true;
	}

	// Token: 0x06001D20 RID: 7456 RVA: 0x0009DE0E File Offset: 0x0009C00E
	public void ReleaseLeftHand()
	{
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.OnRelease(null, this.leftHand);
		}
		if (this.leftHandHeldEquipment != null)
		{
			this.leftHandHeldEquipment.OnRelease(null, this.leftHand);
		}
		this.autoGrabLeft = true;
	}

	// Token: 0x06001D21 RID: 7457 RVA: 0x0009DE4D File Offset: 0x0009C04D
	public void ForceStopClimbing()
	{
		this.bodyClimber.ForceStopClimbing(false, false);
		this.leftClimber.ForceStopClimbing(false, false);
		this.rightClimber.ForceStopClimbing(false, false);
	}

	// Token: 0x06001D22 RID: 7458 RVA: 0x0009DE76 File Offset: 0x0009C076
	public bool GetIsHolding(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return this.leftHandHeldEquipment != null;
		}
		return this.rightHandHeldEquipment != null;
	}

	// Token: 0x06001D23 RID: 7459 RVA: 0x0009DE8F File Offset: 0x0009C08F
	public bool IsGrabDisabled(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return this.disableLeftGrab;
		}
		return this.disableRightGrab;
	}

	// Token: 0x06001D24 RID: 7460 RVA: 0x0009DEA4 File Offset: 0x0009C0A4
	public void InteractionPointDisabled(InteractionPoint interactionPoint)
	{
		if (this.iteratingInteractionPoints)
		{
			this.interactionPointsToRemove.Add(interactionPoint);
			return;
		}
		if (this.overlapInteractionPointsLeft != null)
		{
			this.overlapInteractionPointsLeft.Remove(interactionPoint);
		}
		if (this.overlapInteractionPointsRight != null)
		{
			this.overlapInteractionPointsRight.Remove(interactionPoint);
		}
	}

	// Token: 0x06001D25 RID: 7461 RVA: 0x0009DEF0 File Offset: 0x0009C0F0
	public bool CanGrabLeft()
	{
		return !this.disableLeftGrab && this.leftHandHeldEquipment == null && this.builderPieceInteractor.heldPiece[0] == null;
	}

	// Token: 0x06001D26 RID: 7462 RVA: 0x0009DF1B File Offset: 0x0009C11B
	public bool CanGrabRight()
	{
		return !this.disableRightGrab && this.rightHandHeldEquipment == null && this.builderPieceInteractor.heldPiece[1] == null;
	}

	// Token: 0x06001D27 RID: 7463 RVA: 0x0009DF48 File Offset: 0x0009C148
	private void LateUpdate()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.leftClimber.CheckHandClimber();
		this.rightClimber.CheckHandClimber();
		this.CheckInputValue(true);
		this.isLeftGrabbing = (this.wasLeftGrabPressed && this.grabValue > this.grabThreshold - this.grabHysteresis) || (!this.wasLeftGrabPressed && this.grabValue > this.grabThreshold + this.grabHysteresis);
		if (this.leftClimber && this.leftClimber.isClimbingOrGrabbing)
		{
			this.isLeftGrabbing = false;
		}
		this.CheckInputValue(false);
		this.isRightGrabbing = (this.wasRightGrabPressed && this.grabValue > this.grabThreshold - this.grabHysteresis) || (!this.wasRightGrabPressed && this.grabValue > this.grabThreshold + this.grabHysteresis);
		if (this.rightClimber && this.rightClimber.isClimbingOrGrabbing)
		{
			this.isRightGrabbing = false;
		}
		BuilderPiece builderPiece = this.builderPieceInteractor.heldPiece[0];
		BuilderPiece builderPiece2 = this.builderPieceInteractor.heldPiece[1];
		this.FireHandInteractions(this.leftHand, true, builderPiece);
		this.FireHandInteractions(this.rightHand, false, builderPiece2);
		if (!this.isRightGrabbing && this.wasRightGrabPressed)
		{
			this.ReleaseRightHand();
		}
		if (!this.isLeftGrabbing && this.wasLeftGrabPressed)
		{
			this.ReleaseLeftHand();
		}
		this.builderPieceInteractor.OnLateUpdate();
		if (GameBallPlayerLocal.instance != null)
		{
			GameBallPlayerLocal.instance.OnUpdateInteract();
		}
		if (GamePlayerLocal.instance != null)
		{
			GamePlayerLocal.instance.OnUpdateInteract();
		}
		this.wasLeftGrabPressed = this.isLeftGrabbing;
		this.wasRightGrabPressed = this.isRightGrabbing;
	}

	// Token: 0x06001D28 RID: 7464 RVA: 0x0009E118 File Offset: 0x0009C318
	private void FireHandInteractions(GameObject interactingHand, bool isLeftHand, BuilderPiece pieceInHand)
	{
		if (isLeftHand)
		{
			this.justGrabbed = (this.isLeftGrabbing && !this.wasLeftGrabPressed) || (this.isLeftGrabbing && this.autoGrabLeft);
			this.justReleased = this.leftHandHeldEquipment != null && !this.isLeftGrabbing && this.wasLeftGrabPressed;
		}
		else
		{
			this.justGrabbed = (this.isRightGrabbing && !this.wasRightGrabPressed) || (this.isRightGrabbing && this.autoGrabRight);
			this.justReleased = this.rightHandHeldEquipment != null && !this.isRightGrabbing && this.wasRightGrabPressed;
		}
		List<InteractionPoint> list = (isLeftHand ? this.overlapInteractionPointsLeft : this.overlapInteractionPointsRight);
		bool flag = (isLeftHand ? (this.leftHandHeldEquipment != null) : (this.rightHandHeldEquipment != null));
		bool flag2 = pieceInHand != null;
		bool flag3 = (isLeftHand ? this.disableLeftGrab : this.disableRightGrab);
		bool flag4 = !flag && !flag2 && !flag3;
		this.iteratingInteractionPoints = true;
		foreach (InteractionPoint interactionPoint in list)
		{
			if (flag4 && interactionPoint != null)
			{
				if (this.justGrabbed)
				{
					interactionPoint.Holdable.OnGrab(interactionPoint, interactingHand);
				}
				else
				{
					interactionPoint.Holdable.OnHover(interactionPoint, interactingHand);
				}
			}
			if (this.justReleased)
			{
				this.tempZone = interactionPoint.GetComponent<DropZone>();
				if (this.tempZone != null)
				{
					if (interactingHand == this.leftHand)
					{
						if (this.leftHandHeldEquipment != null)
						{
							this.leftHandHeldEquipment.OnRelease(this.tempZone, interactingHand);
						}
					}
					else if (this.rightHandHeldEquipment != null)
					{
						this.rightHandHeldEquipment.OnRelease(this.tempZone, interactingHand);
					}
				}
			}
		}
		this.iteratingInteractionPoints = false;
		foreach (InteractionPoint interactionPoint2 in this.interactionPointsToRemove)
		{
			if (this.overlapInteractionPointsLeft != null)
			{
				this.overlapInteractionPointsLeft.Remove(interactionPoint2);
			}
			if (this.overlapInteractionPointsRight != null)
			{
				this.overlapInteractionPointsRight.Remove(interactionPoint2);
			}
		}
		this.interactionPointsToRemove.Clear();
	}

	// Token: 0x06001D29 RID: 7465 RVA: 0x0009E370 File Offset: 0x0009C570
	public void UpdateHandEquipment(IHoldableObject newEquipment, bool forLeftHand)
	{
		if (forLeftHand)
		{
			if (newEquipment != null && newEquipment == this.rightHandHeldEquipment && !newEquipment.TwoHanded)
			{
				this.rightHandHeldEquipment = null;
			}
			if (this.leftHandHeldEquipment != null)
			{
				this.leftHandHeldEquipment.DropItemCleanup();
			}
			this.leftHandHeldEquipment = newEquipment;
			this.autoGrabLeft = false;
			return;
		}
		if (newEquipment != null && newEquipment == this.leftHandHeldEquipment && !newEquipment.TwoHanded)
		{
			this.leftHandHeldEquipment = null;
		}
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.DropItemCleanup();
		}
		this.rightHandHeldEquipment = newEquipment;
		this.autoGrabRight = false;
	}

	// Token: 0x06001D2A RID: 7466 RVA: 0x0009E3FC File Offset: 0x0009C5FC
	public void CheckInputValue(bool isLeftHand)
	{
		if (isLeftHand)
		{
			this.grabValue = ControllerInputPoller.GripFloat(XRNode.LeftHand);
			this.tempValue = ControllerInputPoller.TriggerFloat(XRNode.LeftHand);
		}
		else
		{
			this.grabValue = ControllerInputPoller.GripFloat(XRNode.RightHand);
			this.tempValue = ControllerInputPoller.TriggerFloat(XRNode.RightHand);
		}
		this.grabValue = Mathf.Max(this.grabValue, this.tempValue);
	}

	// Token: 0x06001D2B RID: 7467 RVA: 0x0009E455 File Offset: 0x0009C655
	public void ForceDropEquipment(IHoldableObject equipment)
	{
		if (this.rightHandHeldEquipment == equipment)
		{
			this.rightHandHeldEquipment = null;
		}
		if (this.leftHandHeldEquipment == equipment)
		{
			this.leftHandHeldEquipment = null;
		}
	}

	// Token: 0x06001D2C RID: 7468 RVA: 0x0009E477 File Offset: 0x0009C677
	public void ForceDropAnyEquipment()
	{
		this.rightHandHeldEquipment = null;
		this.leftHandHeldEquipment = null;
	}

	// Token: 0x06001D2D RID: 7469 RVA: 0x0009E488 File Offset: 0x0009C688
	public void ForceDropManipulatableObject(HoldableObject manipulatableObject)
	{
		if ((HoldableObject)this.rightHandHeldEquipment == manipulatableObject)
		{
			this.rightHandHeldEquipment.OnRelease(null, this.rightHand);
			this.rightHandHeldEquipment = null;
			this.autoGrabRight = false;
		}
		if ((HoldableObject)this.leftHandHeldEquipment == manipulatableObject)
		{
			this.leftHandHeldEquipment.OnRelease(null, this.leftHand);
			this.leftHandHeldEquipment = null;
			this.autoGrabLeft = false;
		}
	}

	// Token: 0x04002754 RID: 10068
	[OnEnterPlay_SetNull]
	public static volatile EquipmentInteractor instance;

	// Token: 0x04002755 RID: 10069
	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	// Token: 0x04002756 RID: 10070
	public IHoldableObject leftHandHeldEquipment;

	// Token: 0x04002757 RID: 10071
	public IHoldableObject rightHandHeldEquipment;

	// Token: 0x04002758 RID: 10072
	public BuilderPieceInteractor builderPieceInteractor;

	// Token: 0x04002759 RID: 10073
	public GameObject rightHand;

	// Token: 0x0400275A RID: 10074
	public GameObject leftHand;

	// Token: 0x0400275B RID: 10075
	public InputDevice leftHandDevice;

	// Token: 0x0400275C RID: 10076
	public InputDevice rightHandDevice;

	// Token: 0x0400275D RID: 10077
	public List<InteractionPoint> overlapInteractionPointsLeft = new List<InteractionPoint>();

	// Token: 0x0400275E RID: 10078
	public List<InteractionPoint> overlapInteractionPointsRight = new List<InteractionPoint>();

	// Token: 0x0400275F RID: 10079
	public float grabRadius;

	// Token: 0x04002760 RID: 10080
	public float grabThreshold = 0.7f;

	// Token: 0x04002761 RID: 10081
	public float grabHysteresis = 0.05f;

	// Token: 0x04002762 RID: 10082
	public bool wasLeftGrabPressed;

	// Token: 0x04002763 RID: 10083
	public bool wasRightGrabPressed;

	// Token: 0x04002764 RID: 10084
	public bool isLeftGrabbing;

	// Token: 0x04002765 RID: 10085
	public bool isRightGrabbing;

	// Token: 0x04002766 RID: 10086
	public bool justReleased;

	// Token: 0x04002767 RID: 10087
	public bool justGrabbed;

	// Token: 0x04002768 RID: 10088
	public bool disableLeftGrab;

	// Token: 0x04002769 RID: 10089
	public bool disableRightGrab;

	// Token: 0x0400276A RID: 10090
	public bool autoGrabLeft;

	// Token: 0x0400276B RID: 10091
	public bool autoGrabRight;

	// Token: 0x0400276C RID: 10092
	private float grabValue;

	// Token: 0x0400276D RID: 10093
	private float tempValue;

	// Token: 0x0400276E RID: 10094
	private DropZone tempZone;

	// Token: 0x0400276F RID: 10095
	private bool iteratingInteractionPoints;

	// Token: 0x04002770 RID: 10096
	private List<InteractionPoint> interactionPointsToRemove = new List<InteractionPoint>();

	// Token: 0x04002771 RID: 10097
	[SerializeField]
	private GorillaHandClimber bodyClimber;

	// Token: 0x04002772 RID: 10098
	[SerializeField]
	private GorillaHandClimber leftClimber;

	// Token: 0x04002773 RID: 10099
	[SerializeField]
	private GorillaHandClimber rightClimber;
}
