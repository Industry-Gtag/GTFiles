using System;
using GorillaLocomotion.Gameplay;
using Liv.Lck.GorillaTag;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

// Token: 0x020003F7 RID: 1015
public class LckDirectGrabbable : MonoBehaviour, IGorillaGrabable
{
	// Token: 0x14000036 RID: 54
	// (add) Token: 0x0600181E RID: 6174 RVA: 0x00089E6C File Offset: 0x0008806C
	// (remove) Token: 0x0600181F RID: 6175 RVA: 0x00089EA4 File Offset: 0x000880A4
	public event Action onGrabbed;

	// Token: 0x14000037 RID: 55
	// (add) Token: 0x06001820 RID: 6176 RVA: 0x00089EDC File Offset: 0x000880DC
	// (remove) Token: 0x06001821 RID: 6177 RVA: 0x00089F14 File Offset: 0x00088114
	public event Action onReleased;

	// Token: 0x1700025D RID: 605
	// (get) Token: 0x06001822 RID: 6178 RVA: 0x00089F49 File Offset: 0x00088149
	public GorillaGrabber grabber
	{
		get
		{
			return this._grabber;
		}
	}

	// Token: 0x1700025E RID: 606
	// (get) Token: 0x06001823 RID: 6179 RVA: 0x00089F51 File Offset: 0x00088151
	public bool isGrabbed
	{
		get
		{
			return this._grabber != null;
		}
	}

	// Token: 0x06001824 RID: 6180 RVA: 0x00089F5F File Offset: 0x0008815F
	public Vector3 GetLocalGrabbedPosition(GorillaGrabber grabber)
	{
		if (grabber == null)
		{
			return Vector3.zero;
		}
		return base.transform.InverseTransformPoint(grabber.transform.position);
	}

	// Token: 0x06001825 RID: 6181 RVA: 0x00089F86 File Offset: 0x00088186
	public bool CanBeGrabbed(GorillaGrabber grabber)
	{
		return this._grabber == null || grabber == this._grabber;
	}

	// Token: 0x06001826 RID: 6182 RVA: 0x00089FA4 File Offset: 0x000881A4
	public void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
	{
		if (!base.isActiveAndEnabled)
		{
			this._grabber = null;
			grabbedTransform = grabber.transform;
			localGrabbedPosition = Vector3.zero;
			return;
		}
		if (this._grabber != null && this._grabber != grabber)
		{
			this.ForceRelease();
		}
		bool flag;
		bool flag2;
		if (this._precise && this.IsSlingshotHeldInHand(out flag, out flag2) && ((grabber.XrNode == XRNode.LeftHand && flag) || (grabber.XrNode == XRNode.RightHand && flag2)))
		{
			this._grabber = null;
			grabbedTransform = grabber.transform;
			localGrabbedPosition = Vector3.zero;
			return;
		}
		this._grabber = grabber;
		GtColliderTriggerProcessor.CurrentGrabbedHand = grabber.XrNode;
		GtColliderTriggerProcessor.IsGrabbingTablet = true;
		grabbedTransform = base.transform;
		localGrabbedPosition = this.GetLocalGrabbedPosition(this._grabber);
		this.target.SetParent(grabber.transform, true);
		Action action = this.onGrabbed;
		if (action != null)
		{
			action();
		}
		UnityEvent onTabletGrabbed = this.OnTabletGrabbed;
		if (onTabletGrabbed == null)
		{
			return;
		}
		onTabletGrabbed.Invoke();
	}

	// Token: 0x06001827 RID: 6183 RVA: 0x0008A0A4 File Offset: 0x000882A4
	public void OnGrabReleased(GorillaGrabber grabber)
	{
		this.target.transform.SetParent(this._originalTargetParent, true);
		this._grabber = null;
		GtColliderTriggerProcessor.IsGrabbingTablet = false;
		Action action = this.onReleased;
		if (action != null)
		{
			action();
		}
		UnityEvent onTabletReleased = this.OnTabletReleased;
		if (onTabletReleased == null)
		{
			return;
		}
		onTabletReleased.Invoke();
	}

	// Token: 0x06001828 RID: 6184 RVA: 0x0008A0F6 File Offset: 0x000882F6
	public void ForceGrab(GorillaGrabber grabber)
	{
		grabber.Inject(base.transform, this.GetLocalGrabbedPosition(grabber));
	}

	// Token: 0x06001829 RID: 6185 RVA: 0x0008A10B File Offset: 0x0008830B
	public void ForceRelease()
	{
		if (this._grabber == null)
		{
			return;
		}
		this._grabber.Inject(null, Vector3.zero);
	}

	// Token: 0x0600182A RID: 6186 RVA: 0x0008A130 File Offset: 0x00088330
	private bool IsSlingshotHeldInHand(out bool leftHand, out bool rightHand)
	{
		VRRig rig = VRRigCache.Instance.localRig.Rig;
		if (rig == null || rig.projectileWeapon == null)
		{
			leftHand = false;
			rightHand = false;
			return false;
		}
		leftHand = rig.projectileWeapon.InLeftHand();
		rightHand = rig.projectileWeapon.InRightHand();
		return rig.projectileWeapon.InHand();
	}

	// Token: 0x0600182B RID: 6187 RVA: 0x0008A191 File Offset: 0x00088391
	public void SetOriginalTargetParent(Transform parent)
	{
		this._originalTargetParent = parent;
	}

	// Token: 0x0600182C RID: 6188 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool MomentaryGrabOnly()
	{
		return true;
	}

	// Token: 0x0600182E RID: 6190 RVA: 0x00014B5B File Offset: 0x00012D5B
	string IGorillaGrabable.get_name()
	{
		return base.name;
	}

	// Token: 0x04002357 RID: 9047
	public UnityEvent OnTabletGrabbed = new UnityEvent();

	// Token: 0x04002358 RID: 9048
	public UnityEvent OnTabletReleased = new UnityEvent();

	// Token: 0x04002359 RID: 9049
	[SerializeField]
	private Transform _originalTargetParent;

	// Token: 0x0400235A RID: 9050
	public Transform target;

	// Token: 0x0400235B RID: 9051
	[SerializeField]
	private bool _precise;

	// Token: 0x0400235C RID: 9052
	private GorillaGrabber _grabber;
}
