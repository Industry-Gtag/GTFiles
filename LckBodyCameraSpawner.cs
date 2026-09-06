using System;
using GorillaLocomotion;
using Liv.Lck;
using Liv.Lck.Cosmetics;
using Liv.Lck.GorillaTag;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020003F1 RID: 1009
public class LckBodyCameraSpawner : MonoBehaviourTick
{
	// Token: 0x060017F6 RID: 6134 RVA: 0x000890ED File Offset: 0x000872ED
	public void SetFollowTransform(Transform transform)
	{
		this._followTransform = transform;
	}

	// Token: 0x17000259 RID: 601
	// (get) Token: 0x060017F7 RID: 6135 RVA: 0x000890F6 File Offset: 0x000872F6
	public TabletSpawnInstance tabletSpawnInstance
	{
		get
		{
			return this._tabletSpawnInstance;
		}
	}

	// Token: 0x14000035 RID: 53
	// (add) Token: 0x060017F8 RID: 6136 RVA: 0x00089100 File Offset: 0x00087300
	// (remove) Token: 0x060017F9 RID: 6137 RVA: 0x00089134 File Offset: 0x00087334
	public static event LckBodyCameraSpawner.CameraStateDelegate OnCameraStateChange;

	// Token: 0x1700025A RID: 602
	// (get) Token: 0x060017FA RID: 6138 RVA: 0x00089167 File Offset: 0x00087367
	// (set) Token: 0x060017FB RID: 6139 RVA: 0x00089170 File Offset: 0x00087370
	public LckBodyCameraSpawner.CameraState cameraState
	{
		get
		{
			return this._cameraState;
		}
		set
		{
			switch (value)
			{
			case LckBodyCameraSpawner.CameraState.CameraDisabled:
				this.cameraPosition = LckBodyCameraSpawner.CameraPosition.NotVisible;
				this._tabletSpawnInstance.uiVisible = false;
				this._tabletSpawnInstance.cameraActive = false;
				this.ResetCameraModel();
				this.cameraVisible = false;
				this._shouldMoveCameraToNeck = false;
				break;
			case LckBodyCameraSpawner.CameraState.CameraOnNeck:
				this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
				if (this._tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.GetCurrentTriggerProcessor())
				{
					this._tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.GetCurrentTriggerProcessor().ResetToDefaultAndTriggerButton();
					this._tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.ClearAllTriggers();
				}
				this._tabletSpawnInstance.uiVisible = false;
				this._tabletSpawnInstance.cameraActive = true;
				this.ResetCameraModel();
				if (Application.platform == RuntimePlatform.Android)
				{
					this.SetPreviewActive(false);
				}
				this.cameraVisible = true;
				this._shouldMoveCameraToNeck = false;
				this._dummyTablet.SetDummyTabletBodyState(true);
				break;
			case LckBodyCameraSpawner.CameraState.CameraSpawned:
				this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
				this._tabletSpawnInstance.uiVisible = true;
				this._tabletSpawnInstance.cameraActive = true;
				if (Application.platform == RuntimePlatform.Android)
				{
					this.SetPreviewActive(true);
				}
				this.ResetCameraModel();
				this.cameraVisible = true;
				this._shouldMoveCameraToNeck = false;
				this._dummyTablet.SetDummyTabletBodyState(false);
				break;
			}
			this._cameraState = value;
			LckBodyCameraSpawner.CameraStateDelegate onCameraStateChange = LckBodyCameraSpawner.OnCameraStateChange;
			if (onCameraStateChange == null)
			{
				return;
			}
			onCameraStateChange(this._cameraState);
		}
	}

	// Token: 0x060017FC RID: 6140 RVA: 0x000892D4 File Offset: 0x000874D4
	private void SetPreviewActive(bool isActive)
	{
		LckResult<LckService> service = LckService.GetService();
		if (!service.Success)
		{
			Debug.LogError("LCK Could not get Service" + service.Error.ToString());
			return;
		}
		LckService result = service.Result;
		if (result == null)
		{
			return;
		}
		result.SetPreviewActive(isActive);
	}

	// Token: 0x1700025B RID: 603
	// (get) Token: 0x060017FD RID: 6141 RVA: 0x00089325 File Offset: 0x00087525
	// (set) Token: 0x060017FE RID: 6142 RVA: 0x00089330 File Offset: 0x00087530
	public LckBodyCameraSpawner.CameraPosition cameraPosition
	{
		get
		{
			return this._cameraPosition;
		}
		set
		{
			if (this._cameraModelTransform != null && this._cameraPosition != value)
			{
				switch (value)
				{
				case LckBodyCameraSpawner.CameraPosition.CameraDefault:
					this.ChangeCameraModelParent(this._cameraPositionDefault);
					this._cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
					return;
				case LckBodyCameraSpawner.CameraPosition.CameraSlingshot:
					this.ChangeCameraModelParent(this._cameraPositionSlingshot);
					this._cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraSlingshot;
					break;
				case LckBodyCameraSpawner.CameraPosition.NotVisible:
					break;
				default:
					return;
				}
			}
		}
	}

	// Token: 0x1700025C RID: 604
	// (get) Token: 0x060017FF RID: 6143 RVA: 0x0008938E File Offset: 0x0008758E
	// (set) Token: 0x06001800 RID: 6144 RVA: 0x000893A0 File Offset: 0x000875A0
	private bool cameraVisible
	{
		get
		{
			return this._cameraModelTransform.gameObject.activeSelf;
		}
		set
		{
			this._cameraModelTransform.gameObject.SetActive(value);
			this._cameraStrapRenderer.enabled = value;
		}
	}

	// Token: 0x06001801 RID: 6145 RVA: 0x000893BF File Offset: 0x000875BF
	private void Awake()
	{
		this._tabletSpawnInstance = new TabletSpawnInstance(this._cameraSpawnPrefab, this._cameraSpawnParentTransform);
	}

	// Token: 0x06001802 RID: 6146 RVA: 0x000893D8 File Offset: 0x000875D8
	private new void OnEnable()
	{
		base.OnEnable();
		this.InitCameraStrap();
		this.cameraState = LckBodyCameraSpawner.CameraState.CameraDisabled;
		this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
		ZoneManagement.OnZoneChange += this.OnZoneChanged;
		if (this._swapTablet != null && this._swapEmobi != null && this._dummyTablet != null)
		{
			LckGameObjectSwapCosmetic swapTablet = this._swapTablet;
			swapTablet.OnCosmeticSpawned = (Action<GameObject>)Delegate.Combine(swapTablet.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnTabletCosmeticSpawned));
			LckGameObjectSwapCosmetic swapEmobi = this._swapEmobi;
			swapEmobi.OnCosmeticSpawned = (Action<GameObject>)Delegate.Combine(swapEmobi.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnEmobiCosmeticSpawned));
		}
	}

	// Token: 0x06001803 RID: 6147 RVA: 0x00089492 File Offset: 0x00087692
	private void Update()
	{
		this._tabletSpawnInstance.Update();
	}

	// Token: 0x06001804 RID: 6148 RVA: 0x000894A0 File Offset: 0x000876A0
	private new void OnDisable()
	{
		base.OnDisable();
		ZoneManagement.OnZoneChange -= this.OnZoneChanged;
		if (this._swapTablet != null && this._swapEmobi != null && this._dummyTablet != null)
		{
			LckGameObjectSwapCosmetic swapTablet = this._swapTablet;
			swapTablet.OnCosmeticSpawned = (Action<GameObject>)Delegate.Remove(swapTablet.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnTabletCosmeticSpawned));
			LckGameObjectSwapCosmetic swapEmobi = this._swapEmobi;
			swapEmobi.OnCosmeticSpawned = (Action<GameObject>)Delegate.Remove(swapEmobi.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnEmobiCosmeticSpawned));
		}
	}

	// Token: 0x06001805 RID: 6149 RVA: 0x00089548 File Offset: 0x00087748
	public override void Tick()
	{
		if (this._followTransform != null && base.transform.parent != null)
		{
			Matrix4x4 localToWorldMatrix = base.transform.parent.localToWorldMatrix;
			Vector3 vector = localToWorldMatrix.MultiplyPoint(this._followTransform.localPosition + this._followTransform.localRotation * new Vector3(0f, -0.05f, 0.1f));
			Quaternion quaternion = Quaternion.LookRotation(localToWorldMatrix.MultiplyVector(this._followTransform.localRotation * Vector3.forward), localToWorldMatrix.MultiplyVector(this._followTransform.localRotation * Vector3.up));
			base.transform.SetPositionAndRotation(vector, quaternion);
		}
		LckBodyCameraSpawner.CameraState cameraState = this._cameraState;
		if (cameraState != LckBodyCameraSpawner.CameraState.CameraOnNeck)
		{
			if (cameraState == LckBodyCameraSpawner.CameraState.CameraSpawned)
			{
				this.UpdateCameraStrap();
				if (this._cameraModelGrabbable.isGrabbed)
				{
					GorillaGrabber grabber = this._cameraModelGrabbable.grabber;
					Transform transform = grabber.transform;
					if (this.ShouldSpawnCamera(transform))
					{
						this.SpawnCamera(grabber, transform);
					}
				}
				else
				{
					this.ResetCameraModel();
				}
				if (this._tabletSpawnInstance.isSpawned)
				{
					Transform transform3;
					if (this._tabletSpawnInstance.directGrabbable.isGrabbed)
					{
						GorillaGrabber grabber2 = this._tabletSpawnInstance.directGrabbable.grabber;
						Transform transform2 = grabber2.transform;
						if (!this.ShouldSpawnCamera(transform2))
						{
							this.cameraState = LckBodyCameraSpawner.CameraState.CameraOnNeck;
							this._cameraModelGrabbable.target.SetPositionAndRotation(transform2.position, transform2.rotation * Quaternion.Euler(this._chestSpawnRotationOffset.x, this._chestSpawnRotationOffset.y, this._chestSpawnRotationOffset.z));
							this._tabletSpawnInstance.directGrabbable.ForceRelease();
							this._tabletSpawnInstance.SetParent(this._cameraModelTransform);
							this._tabletSpawnInstance.ResetLocalPose();
							this._cameraModelGrabbable.ForceGrab(grabber2);
							this._cameraModelGrabbable.onReleased += this.OnCameraModelReleased;
							if (this._tabletSpawnInstance.Controller.CurrentCameraMode == CameraMode.Selfie)
							{
								this._returnToCameraMode = new CameraMode?(CameraMode.Selfie);
								this._tabletSpawnInstance.Controller.SetCameraMode(CameraMode.FirstPerson);
							}
						}
					}
					else if (this._shouldMoveCameraToNeck && GtTag.TryGetTransform(GtTagType.HMD, out transform3) && Vector3.SqrMagnitude(base.transform.position - this.tabletSpawnInstance.position) >= this._snapToNeckDistance * this._snapToNeckDistance)
					{
						this.cameraState = LckBodyCameraSpawner.CameraState.CameraOnNeck;
						this._tabletSpawnInstance.SetParent(this._cameraModelTransform);
						this._tabletSpawnInstance.ResetLocalPose();
						this._shouldMoveCameraToNeck = false;
					}
				}
			}
		}
		else
		{
			this.UpdateCameraStrap();
			if (this._cameraModelGrabbable.isGrabbed)
			{
				GorillaGrabber grabber3 = this._cameraModelGrabbable.grabber;
				Transform transform4 = grabber3.transform;
				if (this.ShouldSpawnCamera(transform4))
				{
					this.SpawnCamera(grabber3, transform4);
					if (this._returnToCameraMode != null)
					{
						TabletSpawnInstance tabletSpawnInstance = this._tabletSpawnInstance;
						if (tabletSpawnInstance != null)
						{
							tabletSpawnInstance.Controller.SetCameraMode(this._returnToCameraMode.Value);
						}
						this._returnToCameraMode = null;
					}
				}
			}
			else
			{
				this.ResetCameraModel();
			}
		}
		if (!this.IsSlingshotActiveInHierarchy())
		{
			this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
			return;
		}
		this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraSlingshot;
	}

	// Token: 0x06001806 RID: 6150 RVA: 0x000898A4 File Offset: 0x00087AA4
	private void OnZoneChanged(ZoneData[] zones)
	{
		if (!this._tabletSpawnInstance.isSpawned || this._tabletSpawnInstance.directGrabbable.isGrabbed)
		{
			return;
		}
		this._shouldMoveCameraToNeck = true;
	}

	// Token: 0x06001807 RID: 6151 RVA: 0x000898CD File Offset: 0x00087ACD
	private void OnDestroy()
	{
		this._tabletSpawnInstance.Dispose();
	}

	// Token: 0x06001808 RID: 6152 RVA: 0x000898DC File Offset: 0x00087ADC
	[ContextMenu("Put tablet on neck")]
	public void ManuallySetCameraOnNeck()
	{
		if (this.cameraState == LckBodyCameraSpawner.CameraState.CameraOnNeck || this.cameraState == LckBodyCameraSpawner.CameraState.CameraDisabled || !this._tabletSpawnInstance.isSpawned)
		{
			return;
		}
		this.cameraState = LckBodyCameraSpawner.CameraState.CameraOnNeck;
		this._tabletSpawnInstance.SetParent(this._cameraModelTransform);
		this._tabletSpawnInstance.ResetLocalPose();
		this._shouldMoveCameraToNeck = false;
		if (this._tabletSpawnInstance.Controller.CurrentCameraMode == CameraMode.Selfie)
		{
			this._returnToCameraMode = new CameraMode?(CameraMode.Selfie);
			this._tabletSpawnInstance.Controller.SetCameraMode(CameraMode.FirstPerson);
		}
	}

	// Token: 0x06001809 RID: 6153 RVA: 0x00089963 File Offset: 0x00087B63
	private void OnCameraModelReleased()
	{
		this._cameraModelGrabbable.onReleased -= this.OnCameraModelReleased;
		this.ResetCameraModel();
	}

	// Token: 0x0600180A RID: 6154 RVA: 0x00089984 File Offset: 0x00087B84
	public void SpawnCamera(GorillaGrabber overrideGorillaGrabber, Transform transform)
	{
		if (!this._tabletSpawnInstance.isSpawned)
		{
			this._tabletSpawnInstance.SpawnCamera();
		}
		this.cameraState = LckBodyCameraSpawner.CameraState.CameraSpawned;
		this._cameraModelGrabbable.ForceRelease();
		this._tabletSpawnInstance.ResetParent();
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		vector2 = this._rotationOffsetWindows;
		XRNode xrNode = overrideGorillaGrabber.XrNode;
		if (xrNode != XRNode.LeftHand)
		{
			if (xrNode == XRNode.RightHand)
			{
				vector = this._rightHandSpawnOffsetWindows;
				vector2.z = -12f;
			}
		}
		else
		{
			vector = this._leftHandSpawnOffsetWindows;
			vector2.z = 12f;
		}
		if (!GTPlayer.Instance.IsDefaultScale)
		{
			vector *= 0.06f;
		}
		vector = transform.rotation * vector;
		this._tabletSpawnInstance.SetPositionAndRotation(transform.position + vector, transform.rotation * Quaternion.Euler(vector2));
		this._tabletSpawnInstance.directGrabbable.ForceGrab(overrideGorillaGrabber);
		this._tabletSpawnInstance.SetLocalScale(Vector3.one);
	}

	// Token: 0x0600180B RID: 6155 RVA: 0x00089A84 File Offset: 0x00087C84
	private bool ShouldSpawnCamera(Transform gorillaGrabberTransform)
	{
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		Vector3 vector = worldToLocalMatrix.MultiplyPoint(this._cameraModelOriginTransform.position);
		Vector3 vector2 = worldToLocalMatrix.MultiplyPoint(gorillaGrabberTransform.position);
		return Vector3.SqrMagnitude(vector - vector2) >= this._activateDistance * this._activateDistance;
	}

	// Token: 0x0600180C RID: 6156 RVA: 0x00089ADC File Offset: 0x00087CDC
	private void ChangeCameraModelParent(Transform transform)
	{
		if (this._cameraModelTransform != null)
		{
			this._cameraModelGrabbable.SetOriginalTargetParent(transform);
			if (!this._cameraModelGrabbable.isGrabbed)
			{
				this._cameraModelTransform.transform.parent = transform;
				this._cameraModelTransform.transform.localPosition = Vector3.zero;
			}
		}
	}

	// Token: 0x0600180D RID: 6157 RVA: 0x00089B36 File Offset: 0x00087D36
	private void InitCameraStrap()
	{
		this._cameraStrapRenderer.positionCount = this._cameraStrapPoints.Length;
		this._cameraStrapPositions = new Vector3[this._cameraStrapPoints.Length];
	}

	// Token: 0x0600180E RID: 6158 RVA: 0x00089B60 File Offset: 0x00087D60
	private void UpdateCameraStrap()
	{
		for (int i = 0; i < this._cameraStrapPoints.Length; i++)
		{
			this._cameraStrapPositions[i] = this._cameraStrapPoints[i].position;
		}
		this._cameraStrapRenderer.SetPositions(this._cameraStrapPositions);
		Vector3 lossyScale = base.transform.lossyScale;
		float num = (lossyScale.x + lossyScale.y + lossyScale.z) * 0.3333333f;
		this._cameraStrapRenderer.widthMultiplier = num * 0.02f;
		Color color = ((this.cameraState == LckBodyCameraSpawner.CameraState.CameraSpawned) ? this._ghostColor : this._normalColor);
		this._cameraStrapRenderer.startColor = color;
		this._cameraStrapRenderer.endColor = color;
	}

	// Token: 0x0600180F RID: 6159 RVA: 0x00089C13 File Offset: 0x00087E13
	private void ResetCameraModel()
	{
		this._cameraModelTransform.localPosition = Vector3.zero;
		this._cameraModelTransform.localRotation = Quaternion.identity;
	}

	// Token: 0x06001810 RID: 6160 RVA: 0x00089C35 File Offset: 0x00087E35
	private VRRig GetLocalRig()
	{
		if (this._localRig == null)
		{
			this._localRig = VRRigCache.Instance.localRig.Rig;
		}
		return this._localRig;
	}

	// Token: 0x06001811 RID: 6161 RVA: 0x00089C60 File Offset: 0x00087E60
	private bool IsSlingshotHeldInHand(out bool leftHand, out bool rightHand)
	{
		VRRig localRig = this.GetLocalRig();
		if (localRig == null)
		{
			leftHand = false;
			rightHand = false;
			return false;
		}
		leftHand = localRig.projectileWeapon.InLeftHand();
		rightHand = localRig.projectileWeapon.InRightHand();
		return localRig.projectileWeapon.InHand();
	}

	// Token: 0x06001812 RID: 6162 RVA: 0x00089CAC File Offset: 0x00087EAC
	private bool IsSlingshotActiveInHierarchy()
	{
		VRRig localRig = this.GetLocalRig();
		return !(localRig == null) && !(localRig.projectileWeapon == null) && localRig.projectileWeapon.gameObject.activeInHierarchy;
	}

	// Token: 0x04002329 RID: 9001
	[SerializeField]
	private GameObject _cameraSpawnPrefab;

	// Token: 0x0400232A RID: 9002
	[SerializeField]
	private Transform _cameraSpawnParentTransform;

	// Token: 0x0400232B RID: 9003
	[SerializeField]
	private Transform _cameraModelOriginTransform;

	// Token: 0x0400232C RID: 9004
	[SerializeField]
	private Transform _cameraModelTransform;

	// Token: 0x0400232D RID: 9005
	[SerializeField]
	private LckDirectGrabbable _cameraModelGrabbable;

	// Token: 0x0400232E RID: 9006
	[SerializeField]
	private Transform _cameraPositionDefault;

	// Token: 0x0400232F RID: 9007
	[SerializeField]
	private Transform _cameraPositionSlingshot;

	// Token: 0x04002330 RID: 9008
	private Vector3 _chestSpawnRotationOffset = new Vector3(90f, 0f, 0f);

	// Token: 0x04002331 RID: 9009
	private Vector3 _rightHandSpawnOffsetAndroid = new Vector3(-0.265f, 0.02f, -0.065f);

	// Token: 0x04002332 RID: 9010
	private Vector3 _leftHandSpawnOffsetAndroid = new Vector3(0.245f, 0.022f, -0.12f);

	// Token: 0x04002333 RID: 9011
	private Vector3 _rotationOffsetAndroid = new Vector3(-90f, 60f, 125f);

	// Token: 0x04002334 RID: 9012
	private Vector3 _rotationOffsetWindows = new Vector3(-70f, -180f, 0f);

	// Token: 0x04002335 RID: 9013
	private Vector3 _rightHandSpawnOffsetWindows = new Vector3(-0.23f, -0.035f, -0.225f);

	// Token: 0x04002336 RID: 9014
	private Vector3 _leftHandSpawnOffsetWindows = new Vector3(0.23f, -0.035f, -0.225f);

	// Token: 0x04002337 RID: 9015
	[SerializeField]
	private float _activateDistance = 0.25f;

	// Token: 0x04002338 RID: 9016
	[SerializeField]
	private float _snapToNeckDistance = 6f;

	// Token: 0x04002339 RID: 9017
	[SerializeField]
	private LineRenderer _cameraStrapRenderer;

	// Token: 0x0400233A RID: 9018
	[SerializeField]
	private Transform[] _cameraStrapPoints;

	// Token: 0x0400233B RID: 9019
	[SerializeField]
	private Color _normalColor = Color.red;

	// Token: 0x0400233C RID: 9020
	[SerializeField]
	private Color _ghostColor = Color.gray;

	// Token: 0x0400233D RID: 9021
	[Header("Cosmetics References")]
	[SerializeField]
	private GtDummyTablet _dummyTablet;

	// Token: 0x0400233E RID: 9022
	[SerializeField]
	private LckGameObjectSwapCosmetic _swapTablet;

	// Token: 0x0400233F RID: 9023
	[SerializeField]
	private LckGameObjectSwapCosmetic _swapEmobi;

	// Token: 0x04002340 RID: 9024
	private Transform _followTransform;

	// Token: 0x04002341 RID: 9025
	private Vector3[] _cameraStrapPositions;

	// Token: 0x04002342 RID: 9026
	private TabletSpawnInstance _tabletSpawnInstance;

	// Token: 0x04002343 RID: 9027
	private VRRig _localRig;

	// Token: 0x04002344 RID: 9028
	private bool _shouldMoveCameraToNeck;

	// Token: 0x04002345 RID: 9029
	private CameraMode? _returnToCameraMode;

	// Token: 0x04002347 RID: 9031
	private LckBodyCameraSpawner.CameraState _cameraState;

	// Token: 0x04002348 RID: 9032
	private LckBodyCameraSpawner.CameraPosition _cameraPosition;

	// Token: 0x020003F2 RID: 1010
	public enum CameraState
	{
		// Token: 0x0400234A RID: 9034
		CameraDisabled,
		// Token: 0x0400234B RID: 9035
		CameraOnNeck,
		// Token: 0x0400234C RID: 9036
		CameraSpawned
	}

	// Token: 0x020003F3 RID: 1011
	public enum CameraPosition
	{
		// Token: 0x0400234E RID: 9038
		CameraDefault,
		// Token: 0x0400234F RID: 9039
		CameraSlingshot,
		// Token: 0x04002350 RID: 9040
		NotVisible
	}

	// Token: 0x020003F4 RID: 1012
	// (Invoke) Token: 0x06001815 RID: 6165
	public delegate void CameraStateDelegate(LckBodyCameraSpawner.CameraState state);
}
