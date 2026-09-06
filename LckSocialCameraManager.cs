using System;
using Liv.Lck;
using Liv.Lck.GorillaTag;
using UnityEngine;

// Token: 0x0200040F RID: 1039
public class LckSocialCameraManager : MonoBehaviour
{
	// Token: 0x17000273 RID: 627
	// (get) Token: 0x060018A2 RID: 6306 RVA: 0x0008BAF1 File Offset: 0x00089CF1
	public LckDirectGrabbable lckDirectGrabbable
	{
		get
		{
			return this._lckDirectGrabbable;
		}
	}

	// Token: 0x17000274 RID: 628
	// (get) Token: 0x060018A3 RID: 6307 RVA: 0x0008BAF9 File Offset: 0x00089CF9
	public static LckSocialCameraManager Instance
	{
		get
		{
			return LckSocialCameraManager._instance;
		}
	}

	// Token: 0x060018A4 RID: 6308 RVA: 0x0008BB00 File Offset: 0x00089D00
	private void Awake()
	{
		this.SetManagerInstance();
		this._lckCamera = this._gtLckController.GetActiveCamera();
	}

	// Token: 0x060018A5 RID: 6309 RVA: 0x0008BB1C File Offset: 0x00089D1C
	private void OnEnable()
	{
		LckResult<LckService> service = LckService.GetService();
		if (service.Result != null)
		{
			service.Result.OnRecordingStarted += this.OnRecordingStarted;
			service.Result.OnStreamingStarted += this.OnRecordingStarted;
			service.Result.OnRecordingStopped += this.OnRecordingStopped;
			service.Result.OnStreamingStopped += this.OnRecordingStopped;
		}
		LckBodyCameraSpawner.OnCameraStateChange += this.OnBodyCameraStateChanged;
		this._gtLckController.OnCameraModeChanged += this.OnCameraModeChanged;
		this._cameraMode = this._gtLckController.CurrentCameraMode;
	}

	// Token: 0x060018A6 RID: 6310 RVA: 0x0008BBCC File Offset: 0x00089DCC
	private void Update()
	{
		if (this._lckCamera != null)
		{
			Transform transform = this._lckCamera.transform;
			if (this._networkedCococam != null)
			{
				this._networkedCococam.transform.position = transform.position;
				this._networkedCococam.transform.rotation = transform.rotation;
			}
			if (this._networkedTablet != null)
			{
				if (this._networkedTablet.IsOnNeck)
				{
					this._networkedTablet.transform.position = base.transform.position;
				}
				else
				{
					this._networkedTablet.transform.position = base.transform.position + this._tabletPositionOffset * this._networkedTablet.VrRig.scaleFactor;
				}
				this._networkedTablet.transform.rotation = base.transform.rotation;
			}
		}
		if (this._needsUpdate)
		{
			this.UpdateCococamVisibility(this._cameraState, this._cameraMode, this._isForceHidden, this.cameraActive);
			this.UpdateTabletVisibility(this._cameraState, this._isForceHidden, this.cameraActive);
			this.UpdateCococamRecording(this._isRecording);
			this.UpdateTabletRecording(this._isRecording);
			this._needsUpdate = false;
		}
	}

	// Token: 0x060018A7 RID: 6311 RVA: 0x0008BD20 File Offset: 0x00089F20
	private void OnDisable()
	{
		LckResult<LckService> service = LckService.GetService();
		if (service.Result != null)
		{
			service.Result.OnRecordingStarted -= this.OnRecordingStarted;
			service.Result.OnStreamingStarted -= this.OnRecordingStarted;
			service.Result.OnRecordingStopped -= this.OnRecordingStopped;
			service.Result.OnStreamingStopped -= this.OnRecordingStopped;
		}
		LckBodyCameraSpawner.OnCameraStateChange -= this.OnBodyCameraStateChanged;
		this._gtLckController.OnCameraModeChanged -= this.OnCameraModeChanged;
	}

	// Token: 0x060018A8 RID: 6312 RVA: 0x0008BDBF File Offset: 0x00089FBF
	public void SetForceHidden(bool hidden)
	{
		if (this._isForceHidden == hidden)
		{
			return;
		}
		this._isForceHidden = hidden;
		this._needsUpdate = true;
	}

	// Token: 0x060018A9 RID: 6313 RVA: 0x0008BDD9 File Offset: 0x00089FD9
	public void SetLckSocialCococamCamera(LckSocialCamera socialCamera)
	{
		if (this._networkedCococam == socialCamera)
		{
			return;
		}
		this._networkedCococam = socialCamera;
		this._needsUpdate = true;
	}

	// Token: 0x060018AA RID: 6314 RVA: 0x0008BDF8 File Offset: 0x00089FF8
	public void SetLckSocialTabletCamera(LckSocialCamera socialCameraTablet)
	{
		if (this._networkedTablet == socialCameraTablet)
		{
			return;
		}
		this._networkedTablet = socialCameraTablet;
		this._needsUpdate = true;
	}

	// Token: 0x17000275 RID: 629
	// (get) Token: 0x060018AB RID: 6315 RVA: 0x0008BE17 File Offset: 0x0008A017
	// (set) Token: 0x060018AC RID: 6316 RVA: 0x0008BE24 File Offset: 0x0008A024
	public bool cameraActive
	{
		get
		{
			return this._localCameras.activeSelf;
		}
		set
		{
			if (this._localCameras.activeSelf == value)
			{
				return;
			}
			this._localCameras.SetActive(value);
			this._needsUpdate = true;
		}
	}

	// Token: 0x17000276 RID: 630
	// (get) Token: 0x060018AD RID: 6317 RVA: 0x0008BE48 File Offset: 0x0008A048
	// (set) Token: 0x060018AE RID: 6318 RVA: 0x0008BE55 File Offset: 0x0008A055
	public bool uiVisible
	{
		get
		{
			return this._localUi.activeSelf;
		}
		set
		{
			this._localUi.SetActive(value);
		}
	}

	// Token: 0x060018AF RID: 6319 RVA: 0x0008BE63 File Offset: 0x0008A063
	private void SetManagerInstance()
	{
		LckSocialCameraManager._instance = this;
		Action<LckSocialCameraManager> onManagerSpawned = LckSocialCameraManager.OnManagerSpawned;
		if (onManagerSpawned == null)
		{
			return;
		}
		onManagerSpawned(this);
	}

	// Token: 0x060018B0 RID: 6320 RVA: 0x0008BE7B File Offset: 0x0008A07B
	private void OnBodyCameraStateChanged(LckBodyCameraSpawner.CameraState state)
	{
		if (this._cameraState == state)
		{
			return;
		}
		this._cameraState = state;
		this._needsUpdate = true;
	}

	// Token: 0x060018B1 RID: 6321 RVA: 0x0008BE95 File Offset: 0x0008A095
	private void OnCameraModeChanged(CameraMode mode, ILckCamera lckCamera)
	{
		this._lckCamera = lckCamera.GetCameraComponent();
		if (this._cameraMode == mode)
		{
			return;
		}
		this._cameraMode = mode;
		this._needsUpdate = true;
	}

	// Token: 0x060018B2 RID: 6322 RVA: 0x0008BEBB File Offset: 0x0008A0BB
	private void OnRecordingStarted(LckResult result)
	{
		if (this._isRecording == result.Success)
		{
			return;
		}
		this._isRecording = result.Success;
		this._needsUpdate = true;
	}

	// Token: 0x060018B3 RID: 6323 RVA: 0x0008BEDF File Offset: 0x0008A0DF
	private void OnRecordingStopped(LckResult result)
	{
		if (!this._isRecording)
		{
			return;
		}
		this._isRecording = false;
		this._needsUpdate = true;
	}

	// Token: 0x060018B4 RID: 6324 RVA: 0x0008BEF8 File Offset: 0x0008A0F8
	private void UpdateCococamRecording(bool recording)
	{
		this.CoconutCamera.SetRecordingState(recording);
		if (this._networkedCococam == null)
		{
			return;
		}
		this._networkedCococam.recording = recording;
	}

	// Token: 0x060018B5 RID: 6325 RVA: 0x0008BF24 File Offset: 0x0008A124
	private void UpdateCococamVisibility(LckBodyCameraSpawner.CameraState cameraState, CameraMode cameraMode, bool forceHidden, bool cameraActive)
	{
		if (cameraMode == CameraMode.ThirdPerson || cameraMode == CameraMode.Drone)
		{
			this.CoconutCamera.SetVisualsActive(cameraActive);
		}
		else
		{
			this.CoconutCamera.SetVisualsActive(false);
		}
		if (this._networkedCococam == null)
		{
			return;
		}
		if (cameraState == LckBodyCameraSpawner.CameraState.CameraDisabled || forceHidden || !cameraActive)
		{
			this._networkedCococam.visible = false;
			return;
		}
		this._networkedCococam.visible = cameraMode == CameraMode.ThirdPerson || cameraMode == CameraMode.Drone;
	}

	// Token: 0x060018B6 RID: 6326 RVA: 0x0008BF92 File Offset: 0x0008A192
	private void UpdateTabletRecording(bool recording)
	{
		if (this._networkedTablet == null)
		{
			return;
		}
		this._networkedTablet.recording = recording;
	}

	// Token: 0x060018B7 RID: 6327 RVA: 0x0008BFB0 File Offset: 0x0008A1B0
	private void UpdateTabletVisibility(LckBodyCameraSpawner.CameraState cameraState, bool forceHidden, bool cameraActive)
	{
		if (this._networkedTablet == null)
		{
			return;
		}
		if (cameraState == LckBodyCameraSpawner.CameraState.CameraDisabled || forceHidden)
		{
			this._networkedTablet.visible = false;
			this._networkedTablet.IsOnNeck = false;
			return;
		}
		this._networkedTablet.visible = cameraActive;
		this._networkedTablet.IsOnNeck = cameraState == LckBodyCameraSpawner.CameraState.CameraOnNeck;
	}

	// Token: 0x040023CA RID: 9162
	[SerializeField]
	private GameObject _localUi;

	// Token: 0x040023CB RID: 9163
	[SerializeField]
	private GameObject _localCameras;

	// Token: 0x040023CC RID: 9164
	[SerializeField]
	private GTLckController _gtLckController;

	// Token: 0x040023CD RID: 9165
	[SerializeField]
	private LckDirectGrabbable _lckDirectGrabbable;

	// Token: 0x040023CE RID: 9166
	[SerializeField]
	public CoconutCamera CoconutCamera;

	// Token: 0x040023CF RID: 9167
	private LckSocialCamera _networkedCococam;

	// Token: 0x040023D0 RID: 9168
	private LckSocialCamera _networkedTablet;

	// Token: 0x040023D1 RID: 9169
	private Camera _lckCamera;

	// Token: 0x040023D2 RID: 9170
	private CameraMode _cameraMode;

	// Token: 0x040023D3 RID: 9171
	private LckBodyCameraSpawner.CameraState _cameraState;

	// Token: 0x040023D4 RID: 9172
	[OnEnterPlay_SetNull]
	private static LckSocialCameraManager _instance;

	// Token: 0x040023D5 RID: 9173
	public static Action<LckSocialCameraManager> OnManagerSpawned;

	// Token: 0x040023D6 RID: 9174
	private bool _isRecording;

	// Token: 0x040023D7 RID: 9175
	private bool _isForceHidden;

	// Token: 0x040023D8 RID: 9176
	private bool _needsUpdate = true;

	// Token: 0x040023D9 RID: 9177
	private Vector3 _tabletPositionOffset = new Vector3(0f, 0.11f, -0.08f);
}
