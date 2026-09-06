using System;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x02000A3A RID: 2618
public class GorillaThrowableController : MonoBehaviour
{
	// Token: 0x06004342 RID: 17218 RVA: 0x0016600D File Offset: 0x0016420D
	protected void Awake()
	{
		this.gorillaThrowableLayerMask = LayerMask.GetMask(new string[] { "GorillaThrowable" });
	}

	// Token: 0x06004343 RID: 17219 RVA: 0x00166028 File Offset: 0x00164228
	private void LateUpdate()
	{
		if (this.testCanGrab)
		{
			this.testCanGrab = false;
			this.CanGrabAnObject(this.rightHandController, out this.returnCollider);
			Debug.Log(this.returnCollider.gameObject, this.returnCollider.gameObject);
		}
		if (this.leftHandIsGrabbing)
		{
			if (this.CheckIfHandHasReleased(XRNode.LeftHand))
			{
				if (this.leftHandGrabbedObject != null)
				{
					this.leftHandGrabbedObject.ThrowThisThingo();
					this.leftHandGrabbedObject = null;
				}
				this.leftHandIsGrabbing = false;
			}
		}
		else if (this.CheckIfHandHasGrabbed(XRNode.LeftHand))
		{
			this.leftHandIsGrabbing = true;
			if (this.CanGrabAnObject(this.leftHandController, out this.returnCollider))
			{
				this.leftHandGrabbedObject = this.returnCollider.GetComponent<GorillaThrowable>();
				this.leftHandGrabbedObject.Grabbed(this.leftHandController);
			}
		}
		if (this.rightHandIsGrabbing)
		{
			if (this.CheckIfHandHasReleased(XRNode.RightHand))
			{
				if (this.rightHandGrabbedObject != null)
				{
					this.rightHandGrabbedObject.ThrowThisThingo();
					this.rightHandGrabbedObject = null;
				}
				this.rightHandIsGrabbing = false;
				return;
			}
		}
		else if (this.CheckIfHandHasGrabbed(XRNode.RightHand))
		{
			this.rightHandIsGrabbing = true;
			if (this.CanGrabAnObject(this.rightHandController, out this.returnCollider))
			{
				this.rightHandGrabbedObject = this.returnCollider.GetComponent<GorillaThrowable>();
				this.rightHandGrabbedObject.Grabbed(this.rightHandController);
			}
		}
	}

	// Token: 0x06004344 RID: 17220 RVA: 0x00166174 File Offset: 0x00164374
	private bool CheckIfHandHasReleased(XRNode node)
	{
		this.inputDevice = InputDevices.GetDeviceAtXRNode(node);
		this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand));
		if (this.triggerValue < 0.75f)
		{
			this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(SteamVR_Input_Sources.RightHand));
			if (this.triggerValue < 0.75f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004345 RID: 17221 RVA: 0x001661F0 File Offset: 0x001643F0
	private bool CheckIfHandHasGrabbed(XRNode node)
	{
		this.inputDevice = InputDevices.GetDeviceAtXRNode(node);
		this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand));
		if (this.triggerValue > 0.75f)
		{
			return true;
		}
		this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(SteamVR_Input_Sources.RightHand));
		return this.triggerValue > 0.75f;
	}

	// Token: 0x06004346 RID: 17222 RVA: 0x0016626C File Offset: 0x0016446C
	private bool CanGrabAnObject(Transform handTransform, out Collider returnCollider)
	{
		this.magnitude = 100f;
		returnCollider = null;
		Debug.Log("trying:");
		if (Physics.OverlapSphereNonAlloc(handTransform.position, this.handRadius, this.colliders, this.gorillaThrowableLayerMask) > 0)
		{
			Debug.Log("found something!");
			this.minCollider = this.colliders[0];
			foreach (Collider collider in this.colliders)
			{
				if (collider != null)
				{
					Debug.Log("found this", collider);
					if ((collider.transform.position - handTransform.position).magnitude < this.magnitude)
					{
						this.minCollider = collider;
						this.magnitude = (collider.transform.position - handTransform.position).magnitude;
					}
				}
			}
			returnCollider = this.minCollider;
			return true;
		}
		return false;
	}

	// Token: 0x06004347 RID: 17223 RVA: 0x00166355 File Offset: 0x00164555
	public void GrabbableObjectHover(bool isLeft)
	{
		GorillaTagger.Instance.StartVibration(isLeft, this.hoverVibrationStrength, this.hoverVibrationDuration);
	}

	// Token: 0x04005518 RID: 21784
	public Transform leftHandController;

	// Token: 0x04005519 RID: 21785
	public Transform rightHandController;

	// Token: 0x0400551A RID: 21786
	public bool leftHandIsGrabbing;

	// Token: 0x0400551B RID: 21787
	public bool rightHandIsGrabbing;

	// Token: 0x0400551C RID: 21788
	public GorillaThrowable leftHandGrabbedObject;

	// Token: 0x0400551D RID: 21789
	public GorillaThrowable rightHandGrabbedObject;

	// Token: 0x0400551E RID: 21790
	public float hoverVibrationStrength = 0.25f;

	// Token: 0x0400551F RID: 21791
	public float hoverVibrationDuration = 0.05f;

	// Token: 0x04005520 RID: 21792
	public float handRadius = 0.05f;

	// Token: 0x04005521 RID: 21793
	private InputDevice rightDevice;

	// Token: 0x04005522 RID: 21794
	private InputDevice leftDevice;

	// Token: 0x04005523 RID: 21795
	private InputDevice inputDevice;

	// Token: 0x04005524 RID: 21796
	private float triggerValue;

	// Token: 0x04005525 RID: 21797
	private bool boolVar;

	// Token: 0x04005526 RID: 21798
	private Collider[] colliders = new Collider[10];

	// Token: 0x04005527 RID: 21799
	private Collider minCollider;

	// Token: 0x04005528 RID: 21800
	private Collider returnCollider;

	// Token: 0x04005529 RID: 21801
	private float magnitude;

	// Token: 0x0400552A RID: 21802
	public bool testCanGrab;

	// Token: 0x0400552B RID: 21803
	private int gorillaThrowableLayerMask;
}
