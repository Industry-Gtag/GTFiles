using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020004D3 RID: 1235
public class TransferrableObjectHoldablePart_Slide : TransferrableObjectHoldablePart
{
	// Token: 0x06001E09 RID: 7689 RVA: 0x000A1DB4 File Offset: 0x0009FFB4
	protected override void UpdateHeld(VRRig rig, bool isHeldLeftHand)
	{
		int num = (isHeldLeftHand ? 0 : 1);
		GTPlayer instance = GTPlayer.Instance;
		if (!rig.isOfflineVRRig)
		{
			Vector3 vector = instance.GetHandOffset(isHeldLeftHand) * rig.scaleFactor;
			VRMap vrmap = (isHeldLeftHand ? rig.leftHand : rig.rightHand);
			this._snapToLine.target.position = vrmap.GetExtrapolatedControllerPosition() - vector;
			return;
		}
		Transform controllerTransform = instance.GetControllerTransform(num == 0);
		Vector3 position = controllerTransform.position;
		Vector3 snappedPoint = this._snapToLine.GetSnappedPoint(position);
		if (this._maxHandSnapDistance > 0f && (controllerTransform.position - snappedPoint).IsLongerThan(this._maxHandSnapDistance))
		{
			this.OnRelease(null, isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
			return;
		}
		controllerTransform.position = snappedPoint;
		this._snapToLine.target.position = snappedPoint;
	}

	// Token: 0x0400286B RID: 10347
	[SerializeField]
	private float _maxHandSnapDistance;

	// Token: 0x0400286C RID: 10348
	[SerializeField]
	private SnapXformToLine _snapToLine;

	// Token: 0x0400286D RID: 10349
	private const int LEFT = 0;

	// Token: 0x0400286E RID: 10350
	private const int RIGHT = 1;
}
