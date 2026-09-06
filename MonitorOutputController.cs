using System;
using Liv.Lck;
using Liv.Lck.GorillaTag;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x02000414 RID: 1044
public class MonitorOutputController : MonoBehaviour
{
	// Token: 0x060018E0 RID: 6368 RVA: 0x0008CBFA File Offset: 0x0008ADFA
	private void Awake()
	{
		this._lckCamera = this._gtLckController.GetActiveCamera();
	}

	// Token: 0x060018E1 RID: 6369 RVA: 0x0008CC0D File Offset: 0x0008AE0D
	private void OnEnable()
	{
		this._gtLckController.OnCameraModeChanged += this.OnCameraModeChanged;
		LckBodyCameraSpawner.OnCameraStateChange += this.CameraStateChanged;
	}

	// Token: 0x060018E2 RID: 6370 RVA: 0x0008CC38 File Offset: 0x0008AE38
	private void Update()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			Object.Destroy(this);
		}
		if (this._shoulderCamera == null)
		{
			this.FindShoulderCamera();
		}
		if (this._lckCamera != null)
		{
			this._shoulderCamera.transform.position = this._lckCamera.transform.position;
			this._shoulderCamera.transform.rotation = this._lckCamera.transform.rotation;
			this._shoulderCamera.fieldOfView = this._lckCamera.fieldOfView;
			return;
		}
		this._lckCamera = this._gtLckController.GetActiveCamera();
	}

	// Token: 0x060018E3 RID: 6371 RVA: 0x0008CCDE File Offset: 0x0008AEDE
	private void CameraStateChanged(LckBodyCameraSpawner.CameraState state)
	{
		switch (state)
		{
		case LckBodyCameraSpawner.CameraState.CameraDisabled:
			this.RestoreShoulderCamera();
			return;
		case LckBodyCameraSpawner.CameraState.CameraOnNeck:
			this.TakeOverShoulderCamera();
			return;
		case LckBodyCameraSpawner.CameraState.CameraSpawned:
			this.TakeOverShoulderCamera();
			return;
		default:
			return;
		}
	}

	// Token: 0x060018E4 RID: 6372 RVA: 0x0008CD07 File Offset: 0x0008AF07
	private void OnDisable()
	{
		this._gtLckController.OnCameraModeChanged -= this.OnCameraModeChanged;
		this._shoulderCamera.gameObject.GetComponentInChildren<CinemachineBrain>().enabled = true;
		LckBodyCameraSpawner.OnCameraStateChange -= this.CameraStateChanged;
	}

	// Token: 0x060018E5 RID: 6373 RVA: 0x0008CD47 File Offset: 0x0008AF47
	private void OnCameraModeChanged(CameraMode mode, ILckCamera lckCamera)
	{
		this._lckCamera = lckCamera.GetCameraComponent();
		this._lckActiveCameraMode = mode;
	}

	// Token: 0x060018E6 RID: 6374 RVA: 0x0008CD5C File Offset: 0x0008AF5C
	private void TakeOverShoulderCamera()
	{
		this.FindShoulderCamera();
		this._shoulderCamera.gameObject.GetComponentInChildren<CinemachineBrain>().enabled = false;
		this._shoulderCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("LCKHide"));
	}

	// Token: 0x060018E7 RID: 6375 RVA: 0x0008CD9C File Offset: 0x0008AF9C
	private void RestoreShoulderCamera()
	{
		this.FindShoulderCamera();
		this._shoulderCamera.gameObject.GetComponentInChildren<CinemachineBrain>().enabled = true;
		this._shoulderCamera.cullingMask |= 1 << LayerMask.NameToLayer("LCKHide");
		this._shoulderCamera.fieldOfView = this._shoulderCameraFov;
	}

	// Token: 0x060018E8 RID: 6376 RVA: 0x0008CDF8 File Offset: 0x0008AFF8
	private void FindShoulderCamera()
	{
		if (this._shoulderCamera != null)
		{
			return;
		}
		if (!GorillaTagger.hasInstance || !base.isActiveAndEnabled)
		{
			return;
		}
		this._shoulderCamera = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>();
		this._shoulderCameraFov = this._shoulderCamera.fieldOfView;
	}

	// Token: 0x040023FD RID: 9213
	[SerializeField]
	private GTLckController _gtLckController;

	// Token: 0x040023FE RID: 9214
	private Camera _lckCamera;

	// Token: 0x040023FF RID: 9215
	private CameraMode _lckActiveCameraMode;

	// Token: 0x04002400 RID: 9216
	private Camera _shoulderCamera;

	// Token: 0x04002401 RID: 9217
	private float _shoulderCameraFov;
}
