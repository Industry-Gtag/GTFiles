using System;
using Liv.Lck.GorillaTag;
using UnityEngine;

// Token: 0x020003F0 RID: 1008
public class TabletSpawnInstance : IDisposable
{
	// Token: 0x14000033 RID: 51
	// (add) Token: 0x060017DF RID: 6111 RVA: 0x00088D14 File Offset: 0x00086F14
	// (remove) Token: 0x060017E0 RID: 6112 RVA: 0x00088D4C File Offset: 0x00086F4C
	public event Action onGrabbed;

	// Token: 0x14000034 RID: 52
	// (add) Token: 0x060017E1 RID: 6113 RVA: 0x00088D84 File Offset: 0x00086F84
	// (remove) Token: 0x060017E2 RID: 6114 RVA: 0x00088DBC File Offset: 0x00086FBC
	public event Action onReleased;

	// Token: 0x17000253 RID: 595
	// (get) Token: 0x060017E3 RID: 6115 RVA: 0x00088DF1 File Offset: 0x00086FF1
	public LckDirectGrabbable directGrabbable
	{
		get
		{
			return this._lckSocialCameraManager.lckDirectGrabbable;
		}
	}

	// Token: 0x060017E4 RID: 6116 RVA: 0x00088DFE File Offset: 0x00086FFE
	public bool ResetLocalPose()
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return false;
		}
		this._cameraSpawnInstanceTransform.localPosition = Vector3.zero;
		this._cameraSpawnInstanceTransform.localRotation = Quaternion.identity;
		return true;
	}

	// Token: 0x060017E5 RID: 6117 RVA: 0x00088E31 File Offset: 0x00087031
	public bool ResetParent()
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return false;
		}
		this._cameraSpawnInstanceTransform.SetParent(this._cameraSpawnParentTransform);
		return true;
	}

	// Token: 0x060017E6 RID: 6118 RVA: 0x00088E55 File Offset: 0x00087055
	public bool SetParent(Transform transform)
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return false;
		}
		this._cameraSpawnInstanceTransform.SetParent(transform);
		return true;
	}

	// Token: 0x17000254 RID: 596
	// (get) Token: 0x060017E7 RID: 6119 RVA: 0x00088E74 File Offset: 0x00087074
	// (set) Token: 0x060017E8 RID: 6120 RVA: 0x00088E7C File Offset: 0x0008707C
	public bool cameraActive
	{
		get
		{
			return this._cameraActive;
		}
		set
		{
			this._cameraActive = value;
			if (!this._cameraActive && this.Controller != null)
			{
				this.Controller.StopRecording();
			}
			if (this._lckSocialCameraManager != null)
			{
				this._lckSocialCameraManager.cameraActive = this._cameraActive;
			}
		}
	}

	// Token: 0x17000255 RID: 597
	// (get) Token: 0x060017E9 RID: 6121 RVA: 0x00088ED1 File Offset: 0x000870D1
	// (set) Token: 0x060017EA RID: 6122 RVA: 0x00088ED9 File Offset: 0x000870D9
	public bool uiVisible
	{
		get
		{
			return this._uiVisible;
		}
		set
		{
			this._uiVisible = value;
			if (this._lckSocialCameraManager != null)
			{
				this._lckSocialCameraManager.uiVisible = this._uiVisible;
			}
		}
	}

	// Token: 0x17000256 RID: 598
	// (get) Token: 0x060017EB RID: 6123 RVA: 0x00088F01 File Offset: 0x00087101
	public bool isSpawned
	{
		get
		{
			return this._cameraGameObjectInstance != null;
		}
	}

	// Token: 0x060017EC RID: 6124 RVA: 0x00088F0F File Offset: 0x0008710F
	public TabletSpawnInstance(GameObject cameraSpawnPrefab, Transform cameraSpawnParentTransform)
	{
		this._cameraSpawnPrefab = cameraSpawnPrefab;
		this._cameraSpawnParentTransform = cameraSpawnParentTransform;
	}

	// Token: 0x060017ED RID: 6125 RVA: 0x00088F28 File Offset: 0x00087128
	public void Update()
	{
		if (this.Controller == null)
		{
			return;
		}
		Camera activeCamera = this.Controller.GetActiveCamera();
		Camera main = Camera.main;
		if (main != null)
		{
			activeCamera.nearClipPlane = main.nearClipPlane;
			activeCamera.farClipPlane = main.farClipPlane;
		}
	}

	// Token: 0x060017EE RID: 6126 RVA: 0x00088F78 File Offset: 0x00087178
	public void SpawnCamera()
	{
		if (!this.isSpawned)
		{
			this._cameraGameObjectInstance = Object.Instantiate<GameObject>(this._cameraSpawnPrefab, this._cameraSpawnParentTransform);
			this._lckSocialCameraManager = this._cameraGameObjectInstance.GetComponent<LckSocialCameraManager>();
			this._lckSocialCameraManager.lckDirectGrabbable.onGrabbed += delegate
			{
				Action action = this.onGrabbed;
				if (action == null)
				{
					return;
				}
				action();
			};
			this._lckSocialCameraManager.lckDirectGrabbable.onReleased += delegate
			{
				Action action2 = this.onReleased;
				if (action2 == null)
				{
					return;
				}
				action2();
			};
			this._cameraSpawnInstanceTransform = this._cameraGameObjectInstance.transform;
			this.Controller = this._cameraGameObjectInstance.GetComponent<GTLckController>();
		}
		this.uiVisible = this.uiVisible;
		this.cameraActive = this.cameraActive;
	}

	// Token: 0x17000257 RID: 599
	// (get) Token: 0x060017EF RID: 6127 RVA: 0x0008902A File Offset: 0x0008722A
	public Vector3 position
	{
		get
		{
			if (this._cameraSpawnInstanceTransform == null)
			{
				return Vector3.zero;
			}
			return this._cameraSpawnInstanceTransform.position;
		}
	}

	// Token: 0x17000258 RID: 600
	// (get) Token: 0x060017F0 RID: 6128 RVA: 0x0008904B File Offset: 0x0008724B
	public Quaternion rotation
	{
		get
		{
			if (this._cameraSpawnInstanceTransform == null)
			{
				return Quaternion.identity;
			}
			return this._cameraSpawnInstanceTransform.rotation;
		}
	}

	// Token: 0x060017F1 RID: 6129 RVA: 0x0008906C File Offset: 0x0008726C
	public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return;
		}
		this._cameraSpawnInstanceTransform.SetPositionAndRotation(position, rotation);
	}

	// Token: 0x060017F2 RID: 6130 RVA: 0x0008908A File Offset: 0x0008728A
	public void SetLocalScale(Vector3 scale)
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return;
		}
		this._cameraSpawnInstanceTransform.localScale = scale;
	}

	// Token: 0x060017F3 RID: 6131 RVA: 0x000890A7 File Offset: 0x000872A7
	public void Dispose()
	{
		if (this._cameraGameObjectInstance != null)
		{
			Object.Destroy(this._cameraGameObjectInstance);
			this._cameraGameObjectInstance = null;
		}
	}

	// Token: 0x04002320 RID: 8992
	private GameObject _cameraGameObjectInstance;

	// Token: 0x04002321 RID: 8993
	private GameObject _cameraSpawnPrefab;

	// Token: 0x04002322 RID: 8994
	private GameEvents _GtCamera;

	// Token: 0x04002323 RID: 8995
	private Transform _cameraSpawnParentTransform;

	// Token: 0x04002324 RID: 8996
	private Transform _cameraSpawnInstanceTransform;

	// Token: 0x04002325 RID: 8997
	public GTLckController Controller;

	// Token: 0x04002326 RID: 8998
	private LckSocialCameraManager _lckSocialCameraManager;

	// Token: 0x04002327 RID: 8999
	private bool _cameraActive;

	// Token: 0x04002328 RID: 9000
	private bool _uiVisible;
}
