using System;
using GorillaLocomotion;
using Liv.Lck.GorillaTag;
using UnityEngine;

// Token: 0x02000410 RID: 1040
public class LckTabletSizeManager : MonoBehaviour
{
	// Token: 0x060018B9 RID: 6329 RVA: 0x0008C031 File Offset: 0x0008A231
	private void Start()
	{
		GTLckController controller = this._controller;
		controller.OnFOVUpdated = (Action<CameraMode>)Delegate.Combine(controller.OnFOVUpdated, new Action<CameraMode>(this.UpdateCustomNearClip));
		this._controller.OnHorizontalModeChanged += this.OnHorizontalModeChanged;
	}

	// Token: 0x060018BA RID: 6330 RVA: 0x0008C071 File Offset: 0x0008A271
	private void OnDestroy()
	{
		this._controller.OnHorizontalModeChanged -= this.OnHorizontalModeChanged;
		GTLckController controller = this._controller;
		controller.OnFOVUpdated = (Action<CameraMode>)Delegate.Remove(controller.OnFOVUpdated, new Action<CameraMode>(this.UpdateCustomNearClip));
	}

	// Token: 0x060018BB RID: 6331 RVA: 0x0008C0B1 File Offset: 0x0008A2B1
	private void OnHorizontalModeChanged(bool mode)
	{
		this.UpdateCustomNearClip(CameraMode.Selfie);
		this.UpdateCustomNearClip(CameraMode.FirstPerson);
	}

	// Token: 0x060018BC RID: 6332 RVA: 0x0008C0C4 File Offset: 0x0008A2C4
	private void UpdateCustomNearClip(CameraMode mode)
	{
		if (GTPlayer.Instance.IsDefaultScale)
		{
			return;
		}
		switch (mode)
		{
		case CameraMode.Selfie:
			this.SetCustomNearClip(this._selfieCamera);
			return;
		case CameraMode.FirstPerson:
			this.SetCustomNearClip(this._firstPersonCamera);
			break;
		case CameraMode.ThirdPerson:
		case CameraMode.Headset:
		case CameraMode.Drone:
			break;
		default:
			return;
		}
	}

	// Token: 0x060018BD RID: 6333 RVA: 0x0008C114 File Offset: 0x0008A314
	private void SetCustomNearClip(Camera cam)
	{
		if (GTPlayer.Instance.IsDefaultScale)
		{
			return;
		}
		Matrix4x4 matrix4x;
		if (this._controller.HorizontalMode)
		{
			matrix4x = Matrix4x4.Perspective(cam.fieldOfView, 1.777778f, this._customNearClip, cam.farClipPlane);
		}
		else
		{
			matrix4x = Matrix4x4.Perspective(cam.fieldOfView, 0.5625f, this._customNearClip, cam.farClipPlane);
		}
		cam.projectionMatrix = matrix4x;
	}

	// Token: 0x060018BE RID: 6334 RVA: 0x0008C17E File Offset: 0x0008A37E
	private void ClearCustomNearClip()
	{
		this._selfieCamera.ResetProjectionMatrix();
		this._firstPersonCamera.ResetProjectionMatrix();
	}

	// Token: 0x060018BF RID: 6335 RVA: 0x0008C198 File Offset: 0x0008A398
	private void PlayerBecameSmall()
	{
		this._firstPersonCamera.transform.localPosition = this._firstPersonCamShrinkPosition;
		this._tabletFollower.SetPlayerSizeModifier(false, this._shrinkSize);
		if (!this._lckDirectGrabbable.isGrabbed)
		{
			this.SetCameraOnNeck();
		}
		this.SetCustomNearClip(this._selfieCamera);
		this.SetCustomNearClip(this._firstPersonCamera);
	}

	// Token: 0x060018C0 RID: 6336 RVA: 0x0008C1F8 File Offset: 0x0008A3F8
	private void PlayerBecameDefaultSize()
	{
		this._firstPersonCamera.transform.localPosition = this._firstPersonCamDefaultPosition;
		this._tabletFollower.SetPlayerSizeModifier(true, 1f);
		if (!this._lckDirectGrabbable.isGrabbed)
		{
			this.SetCameraOnNeck();
			base.transform.localScale = Vector3.one;
		}
		this.ClearCustomNearClip();
	}

	// Token: 0x060018C1 RID: 6337 RVA: 0x0008C258 File Offset: 0x0008A458
	private void SetCameraOnNeck()
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance == null)
		{
			Debug.LogError("Unable to find playerInstance!");
			return;
		}
		LckBodyCameraSpawner componentInChildren = instance.GetComponentInChildren<LckBodyCameraSpawner>(true);
		if (componentInChildren == null)
		{
			Debug.LogError("Unable to find bodyCameraSpawner!");
			return;
		}
		componentInChildren.ManuallySetCameraOnNeck();
	}

	// Token: 0x060018C2 RID: 6338 RVA: 0x0008C2A4 File Offset: 0x0008A4A4
	private void Update()
	{
		if (!GTPlayer.Instance.IsDefaultScale && this._isDefaultScale != GTPlayer.Instance.IsDefaultScale)
		{
			this._isDefaultScale = false;
			this.PlayerBecameSmall();
			return;
		}
		if (GTPlayer.Instance.IsDefaultScale && this._isDefaultScale != GTPlayer.Instance.IsDefaultScale)
		{
			this._isDefaultScale = true;
			this.PlayerBecameDefaultSize();
		}
	}

	// Token: 0x040023DA RID: 9178
	[SerializeField]
	private GTLckController _controller;

	// Token: 0x040023DB RID: 9179
	[SerializeField]
	private LckDirectGrabbable _lckDirectGrabbable;

	// Token: 0x040023DC RID: 9180
	[SerializeField]
	private GtTabletFollower _tabletFollower;

	// Token: 0x040023DD RID: 9181
	[SerializeField]
	private Camera _firstPersonCamera;

	// Token: 0x040023DE RID: 9182
	[SerializeField]
	private Camera _selfieCamera;

	// Token: 0x040023DF RID: 9183
	private Vector3 _firstPersonCamShrinkPosition = new Vector3(0f, 0f, -0.78f);

	// Token: 0x040023E0 RID: 9184
	private Vector3 _firstPersonCamDefaultPosition = Vector3.zero;

	// Token: 0x040023E1 RID: 9185
	private float _shrinkSize = 0.06f;

	// Token: 0x040023E2 RID: 9186
	private Vector3 _shrinkVector = new Vector3(0.06f, 0.06f, 0.06f);

	// Token: 0x040023E3 RID: 9187
	private float _customNearClip = 0.0006f;

	// Token: 0x040023E4 RID: 9188
	private bool _isDefaultScale = true;
}
