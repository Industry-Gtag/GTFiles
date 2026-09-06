using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion.Gameplay;
using GT_CustomMapSupportRuntime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020008F7 RID: 2295
[RequireComponent(typeof(Collider))]
public class HandHold : MonoBehaviour, IGorillaGrabable
{
	// Token: 0x1400006F RID: 111
	// (add) Token: 0x06003C30 RID: 15408 RVA: 0x00148F2C File Offset: 0x0014712C
	// (remove) Token: 0x06003C31 RID: 15409 RVA: 0x00148F60 File Offset: 0x00147160
	public static event HandHold.HandHoldPositionEvent HandPositionRequestOverride;

	// Token: 0x14000070 RID: 112
	// (add) Token: 0x06003C32 RID: 15410 RVA: 0x00148F94 File Offset: 0x00147194
	// (remove) Token: 0x06003C33 RID: 15411 RVA: 0x00148FC8 File Offset: 0x001471C8
	public static event HandHold.HandHoldEvent HandPositionReleaseOverride;

	// Token: 0x06003C34 RID: 15412 RVA: 0x00148FFC File Offset: 0x001471FC
	public void OnDisable()
	{
		for (int i = 0; i < this.currentGrabbers.Count; i++)
		{
			if (this.currentGrabbers[i].IsNotNull())
			{
				this.currentGrabbers[i].Ungrab(this);
			}
		}
	}

	// Token: 0x06003C35 RID: 15413 RVA: 0x00149044 File Offset: 0x00147244
	private void Initialize()
	{
		if (this.initialized)
		{
			return;
		}
		this.myTappable = base.GetComponent<Tappable>();
		this.myCollider = base.GetComponent<Collider>();
		this.initialized = true;
	}

	// Token: 0x06003C36 RID: 15414 RVA: 0x00023F0C File Offset: 0x0002210C
	public virtual bool CanBeGrabbed(GorillaGrabber grabber)
	{
		return true;
	}

	// Token: 0x06003C37 RID: 15415 RVA: 0x00149070 File Offset: 0x00147270
	void IGorillaGrabable.OnGrabbed(GorillaGrabber g, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
	{
		this.Initialize();
		grabbedTransform = base.transform;
		Vector3 position = g.transform.position;
		localGrabbedPosition = base.transform.InverseTransformPoint(position);
		Vector3 vector;
		g.Player.AddHandHold(base.transform, localGrabbedPosition, g, g.IsLeftHand, this.rotatePlayerWhenHeld, out vector);
		this.currentGrabbers.AddIfNew(g);
		if (this.handSnapMethod != HandHold.HandSnapMethod.None && HandHold.HandPositionRequestOverride != null)
		{
			HandHold.HandPositionRequestOverride(this, g.IsLeftHand, this.CalculateOffset(position));
		}
		UnityEvent<Vector3> onGrab = this.OnGrab;
		if (onGrab != null)
		{
			onGrab.Invoke(vector);
		}
		UnityEvent<HandHold> onGrabHandHold = this.OnGrabHandHold;
		if (onGrabHandHold != null)
		{
			onGrabHandHold.Invoke(this);
		}
		UnityEvent<bool> onGrabHanded = this.OnGrabHanded;
		if (onGrabHanded != null)
		{
			onGrabHanded.Invoke(g.IsLeftHand);
		}
		if (this.myTappable != null)
		{
			this.myTappable.OnGrab();
		}
	}

	// Token: 0x06003C38 RID: 15416 RVA: 0x00149158 File Offset: 0x00147358
	void IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
	{
		this.Initialize();
		g.Player.RemoveHandHold(g, g.IsLeftHand);
		this.currentGrabbers.Remove(g);
		if (this.handSnapMethod != HandHold.HandSnapMethod.None && HandHold.HandPositionReleaseOverride != null)
		{
			HandHold.HandPositionReleaseOverride(this, g.IsLeftHand);
		}
		UnityEvent onRelease = this.OnRelease;
		if (onRelease != null)
		{
			onRelease.Invoke();
		}
		UnityEvent<HandHold> onReleaseHandHold = this.OnReleaseHandHold;
		if (onReleaseHandHold != null)
		{
			onReleaseHandHold.Invoke(this);
		}
		if (this.myTappable != null)
		{
			this.myTappable.OnRelease();
		}
	}

	// Token: 0x06003C39 RID: 15417 RVA: 0x001491E8 File Offset: 0x001473E8
	private Vector3 CalculateOffset(Vector3 position)
	{
		switch (this.handSnapMethod)
		{
		case HandHold.HandSnapMethod.SnapToNearestEdge:
			if (this.myCollider == null)
			{
				this.myCollider = base.GetComponent<Collider>();
				if (this.myCollider is MeshCollider && !(this.myCollider as MeshCollider).convex)
				{
					this.handSnapMethod = HandHold.HandSnapMethod.None;
					return Vector3.zero;
				}
			}
			return base.transform.position - this.myCollider.ClosestPoint(position);
		case HandHold.HandSnapMethod.SnapToXAxisPoint:
			return base.transform.position - base.transform.TransformPoint(Vector3.right * base.transform.InverseTransformPoint(position).x);
		case HandHold.HandSnapMethod.SnapToYAxisPoint:
			return base.transform.position - base.transform.TransformPoint(Vector3.up * base.transform.InverseTransformPoint(position).y);
		case HandHold.HandSnapMethod.SnapToZAxisPoint:
			return base.transform.position - base.transform.TransformPoint(Vector3.forward * base.transform.InverseTransformPoint(position).z);
		default:
			return Vector3.zero;
		}
	}

	// Token: 0x06003C3A RID: 15418 RVA: 0x00149326 File Offset: 0x00147526
	public bool MomentaryGrabOnly()
	{
		return this.forceMomentary;
	}

	// Token: 0x06003C3B RID: 15419 RVA: 0x0014932E File Offset: 0x0014752E
	public void CopyProperties(HandHoldSettings handHoldSettings)
	{
		this.handSnapMethod = (HandHold.HandSnapMethod)handHoldSettings.handSnapMethod;
		this.rotatePlayerWhenHeld = handHoldSettings.rotatePlayerWhenHeld;
		this.forceMomentary = !handHoldSettings.allowPreGrab;
	}

	// Token: 0x06003C3D RID: 15421 RVA: 0x00014B5B File Offset: 0x00012D5B
	string IGorillaGrabable.get_name()
	{
		return base.name;
	}

	// Token: 0x04004CD8 RID: 19672
	private Dictionary<Transform, Transform> attached = new Dictionary<Transform, Transform>();

	// Token: 0x04004CD9 RID: 19673
	[SerializeField]
	private HandHold.HandSnapMethod handSnapMethod;

	// Token: 0x04004CDA RID: 19674
	[SerializeField]
	private bool rotatePlayerWhenHeld;

	// Token: 0x04004CDB RID: 19675
	[SerializeField]
	private UnityEvent<Vector3> OnGrab;

	// Token: 0x04004CDC RID: 19676
	[SerializeField]
	private UnityEvent<HandHold> OnGrabHandHold;

	// Token: 0x04004CDD RID: 19677
	[SerializeField]
	private UnityEvent<bool> OnGrabHanded;

	// Token: 0x04004CDE RID: 19678
	[SerializeField]
	private UnityEvent OnRelease;

	// Token: 0x04004CDF RID: 19679
	[SerializeField]
	private UnityEvent<HandHold> OnReleaseHandHold;

	// Token: 0x04004CE0 RID: 19680
	private bool initialized;

	// Token: 0x04004CE1 RID: 19681
	private Collider myCollider;

	// Token: 0x04004CE2 RID: 19682
	private Tappable myTappable;

	// Token: 0x04004CE3 RID: 19683
	[Tooltip("Turning this on disables \"pregrabbing\". Use pregrabbing to allow players to catch a handhold even if they have squeezed the trigger too soon. Useful if you're anticipating jumping players needed to grab while airborne")]
	[SerializeField]
	private bool forceMomentary = true;

	// Token: 0x04004CE4 RID: 19684
	private List<GorillaGrabber> currentGrabbers = new List<GorillaGrabber>();

	// Token: 0x020008F8 RID: 2296
	private enum HandSnapMethod
	{
		// Token: 0x04004CE6 RID: 19686
		None,
		// Token: 0x04004CE7 RID: 19687
		SnapToCenterPoint,
		// Token: 0x04004CE8 RID: 19688
		SnapToNearestEdge,
		// Token: 0x04004CE9 RID: 19689
		SnapToXAxisPoint,
		// Token: 0x04004CEA RID: 19690
		SnapToYAxisPoint,
		// Token: 0x04004CEB RID: 19691
		SnapToZAxisPoint
	}

	// Token: 0x020008F9 RID: 2297
	// (Invoke) Token: 0x06003C3F RID: 15423
	public delegate void HandHoldPositionEvent(HandHold hh, bool lh, Vector3 pos);

	// Token: 0x020008FA RID: 2298
	// (Invoke) Token: 0x06003C43 RID: 15427
	public delegate void HandHoldEvent(HandHold hh, bool lh);
}
