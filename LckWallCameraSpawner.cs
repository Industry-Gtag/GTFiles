using System;
using System.Collections;
using GorillaLocomotion;
using Liv.Lck.Cosmetics;
using Liv.Lck.GorillaTag;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

// Token: 0x02000411 RID: 1041
public class LckWallCameraSpawner : MonoBehaviour
{
	// Token: 0x060018C4 RID: 6340 RVA: 0x0008C378 File Offset: 0x0008A578
	private LckBodyCameraSpawner GetOrCreateBodyCameraSpawner()
	{
		if (LckWallCameraSpawner._bodySpawner != null)
		{
			return LckWallCameraSpawner._bodySpawner;
		}
		GTPlayer instance = GTPlayer.Instance;
		if (instance == null)
		{
			Debug.LogError("Unable to find Player!");
			return null;
		}
		LckWallCameraSpawner.AddGTag(Camera.main.gameObject, GtTagType.HMD);
		LckWallCameraSpawner.AddGTag(instance.gameObject, GtTagType.Player);
		Transform transform = instance.bodyCollider.transform;
		GameObject gameObject = Object.Instantiate<GameObject>(this._lckBodySpawnerPrefab, transform.parent);
		Transform transform2 = gameObject.transform;
		transform2.localPosition = Vector3.zero;
		transform2.localRotation = Quaternion.identity;
		transform2.localScale = Vector3.one;
		LckWallCameraSpawner._bodySpawner = gameObject.GetComponent<LckBodyCameraSpawner>();
		LckWallCameraSpawner._bodySpawner.SetFollowTransform(transform);
		GorillaTagger instance2 = GorillaTagger.Instance;
		if (instance2 != null)
		{
			LckWallCameraSpawner.AddGTag(instance2.leftHandTriggerCollider, GtTagType.LeftHand);
			LckWallCameraSpawner.AddGTag(instance2.rightHandTriggerCollider, GtTagType.RightHand);
		}
		else
		{
			Debug.LogError("Unable to find GorillaTagger!");
		}
		return LckWallCameraSpawner._bodySpawner;
	}

	// Token: 0x060018C5 RID: 6341 RVA: 0x0008C45F File Offset: 0x0008A65F
	private static void AddGTag(GameObject go, GtTagType gtTagType)
	{
		if (go.GetComponent<GtTag>())
		{
			return;
		}
		GtTag gtTag = go.AddComponent<GtTag>();
		gtTag.gtTagType = gtTagType;
		gtTag.enabled = true;
	}

	// Token: 0x17000277 RID: 631
	// (get) Token: 0x060018C6 RID: 6342 RVA: 0x0008C482 File Offset: 0x0008A682
	// (set) Token: 0x060018C7 RID: 6343 RVA: 0x0008C48C File Offset: 0x0008A68C
	public LckWallCameraSpawner.WallSpawnerState wallSpawnerState
	{
		get
		{
			return this._wallSpawnerState;
		}
		set
		{
			switch (value)
			{
			case LckWallCameraSpawner.WallSpawnerState.CameraOnHook:
				this.ResetCameraModel();
				this.UpdateCameraStrap();
				this.cameraVisible = true;
				break;
			case LckWallCameraSpawner.WallSpawnerState.CameraOffHook:
				this.ResetCameraModel();
				this.UpdateCameraStrap();
				this.cameraVisible = true;
				break;
			}
			this._wallSpawnerState = value;
		}
	}

	// Token: 0x060018C8 RID: 6344 RVA: 0x0008C4DC File Offset: 0x0008A6DC
	private void Awake()
	{
		this.InitCameraStrap();
	}

	// Token: 0x060018C9 RID: 6345 RVA: 0x0008C4E4 File Offset: 0x0008A6E4
	private void OnEnable()
	{
		if (this._swapTablet != null && this._swapEmobi != null && this._dummyTablet != null)
		{
			LckGameObjectSwapCosmetic swapTablet = this._swapTablet;
			swapTablet.OnCosmeticSpawned = (Action<GameObject>)Delegate.Combine(swapTablet.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnTabletCosmeticSpawned));
			LckGameObjectSwapCosmetic swapEmobi = this._swapEmobi;
			swapEmobi.OnCosmeticSpawned = (Action<GameObject>)Delegate.Combine(swapEmobi.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnEmobiCosmeticSpawned));
		}
		this._cameraHandleGrabbable.onGrabbed += this.OnGrabbed;
		this._cameraHandleGrabbable.onReleased += this.OnReleased;
		this.wallSpawnerState = LckWallCameraSpawner.WallSpawnerState.CameraOnHook;
	}

	// Token: 0x060018CA RID: 6346 RVA: 0x0008C5A8 File Offset: 0x0008A7A8
	private void Start()
	{
		this.CreatePrewarmCamera();
	}

	// Token: 0x060018CB RID: 6347 RVA: 0x0008C5B0 File Offset: 0x0008A7B0
	private void Update()
	{
		LckWallCameraSpawner.WallSpawnerState wallSpawnerState = this._wallSpawnerState;
		if (wallSpawnerState != LckWallCameraSpawner.WallSpawnerState.CameraOnHook)
		{
			if (wallSpawnerState != LckWallCameraSpawner.WallSpawnerState.CameraDragging)
			{
				return;
			}
			this.UpdateCameraStrap();
			if (this.ShouldSpawnCamera(this._cameraHandleGrabbable.grabber.transform))
			{
				this.SpawnCamera(this._cameraHandleGrabbable.grabber);
			}
		}
		else
		{
			if (this.GetOrCreateBodyCameraSpawner() == null)
			{
				Debug.LogError("Lck, Unable to find LckBodyCameraSpawner");
				base.gameObject.SetActive(false);
				return;
			}
			if (LckWallCameraSpawner._bodySpawner.cameraState == LckBodyCameraSpawner.CameraState.CameraSpawned && LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.isSpawned && LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.directGrabbable.isGrabbed)
			{
				LckDirectGrabbable directGrabbable = LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.directGrabbable;
				GorillaGrabber grabber = directGrabbable.grabber;
				if (!this.ShouldSpawnCamera(grabber.transform))
				{
					directGrabbable.ForceRelease();
					LckWallCameraSpawner._bodySpawner.cameraState = LckBodyCameraSpawner.CameraState.CameraDisabled;
					this._cameraHandleGrabbable.target.SetPositionAndRotation(grabber.transform.position, grabber.transform.rotation * Quaternion.Euler(this._spawnRotationOffsetWindows, 180f, 0f));
					this._cameraHandleGrabbable.ForceGrab(grabber);
					return;
				}
			}
		}
	}

	// Token: 0x060018CC RID: 6348 RVA: 0x0008C6E8 File Offset: 0x0008A8E8
	private void OnDisable()
	{
		if (this._swapTablet != null && this._swapEmobi != null && this._dummyTablet != null)
		{
			LckGameObjectSwapCosmetic swapTablet = this._swapTablet;
			swapTablet.OnCosmeticSpawned = (Action<GameObject>)Delegate.Remove(swapTablet.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnTabletCosmeticSpawned));
			LckGameObjectSwapCosmetic swapEmobi = this._swapEmobi;
			swapEmobi.OnCosmeticSpawned = (Action<GameObject>)Delegate.Remove(swapEmobi.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnEmobiCosmeticSpawned));
		}
		this._cameraHandleGrabbable.onGrabbed -= this.OnGrabbed;
		this._cameraHandleGrabbable.onReleased -= this.OnReleased;
	}

	// Token: 0x17000278 RID: 632
	// (get) Token: 0x060018CD RID: 6349 RVA: 0x0008C7A5 File Offset: 0x0008A9A5
	// (set) Token: 0x060018CE RID: 6350 RVA: 0x0008C7B7 File Offset: 0x0008A9B7
	private bool cameraVisible
	{
		get
		{
			return this._cameraModelTransform.gameObject.activeSelf;
		}
		set
		{
			this._cameraModelTransform.gameObject.SetActive(value);
			this._cameraStrapRenderer.gameObject.SetActive(value);
		}
	}

	// Token: 0x060018CF RID: 6351 RVA: 0x0008C7DC File Offset: 0x0008A9DC
	private void SpawnCamera(GorillaGrabber lastGorillaGrabber)
	{
		if (LckWallCameraSpawner._bodySpawner == null)
		{
			Debug.LogError("Lck, unable to spawn camera, body spawner is null!");
			return;
		}
		if (LckWallCameraSpawner._bodySpawner.tabletSpawnInstance != null && LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.Controller != null && LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup != null && LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.GetCurrentTriggerProcessor())
		{
			LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.GetCurrentTriggerProcessor().ResetToDefaultAndTriggerButton();
			LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.ClearAllTriggers();
		}
		this.cameraVisible = false;
		this._cameraHandleGrabbable.ForceRelease();
		LckWallCameraSpawner._bodySpawner.SpawnCamera(lastGorillaGrabber, lastGorillaGrabber.transform);
	}

	// Token: 0x060018D0 RID: 6352 RVA: 0x0008C8BD File Offset: 0x0008AABD
	private void InitCameraStrap()
	{
		this._cameraStrapRenderer.positionCount = this._cameraStrapPoints.Length;
		this._cameraStrapPositions = new Vector3[this._cameraStrapPoints.Length];
	}

	// Token: 0x060018D1 RID: 6353 RVA: 0x0008C8E8 File Offset: 0x0008AAE8
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
		this._cameraStrapRenderer.startColor = (this._cameraStrapRenderer.endColor = this._normalColor);
	}

	// Token: 0x060018D2 RID: 6354 RVA: 0x0008C98A File Offset: 0x0008AB8A
	private void ResetCameraModel()
	{
		this._cameraModelTransform.localPosition = Vector3.zero;
		this._cameraModelTransform.localRotation = Quaternion.identity;
	}

	// Token: 0x060018D3 RID: 6355 RVA: 0x0008C9AC File Offset: 0x0008ABAC
	private bool ShouldSpawnCamera(Transform gorillaGrabberTransform)
	{
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		Vector3 vector = worldToLocalMatrix.MultiplyPoint(this._cameraModelOriginTransform.position);
		Vector3 vector2 = worldToLocalMatrix.MultiplyPoint(gorillaGrabberTransform.position);
		return Vector3.SqrMagnitude(vector - vector2) >= this._activateDistance * this._activateDistance;
	}

	// Token: 0x060018D4 RID: 6356 RVA: 0x0008CA02 File Offset: 0x0008AC02
	private void OnGrabbed()
	{
		this.wallSpawnerState = LckWallCameraSpawner.WallSpawnerState.CameraDragging;
	}

	// Token: 0x060018D5 RID: 6357 RVA: 0x0008CA0B File Offset: 0x0008AC0B
	private void OnReleased()
	{
		this.wallSpawnerState = LckWallCameraSpawner.WallSpawnerState.CameraOnHook;
	}

	// Token: 0x060018D6 RID: 6358 RVA: 0x0008CA14 File Offset: 0x0008AC14
	private void CreatePrewarmCamera()
	{
		if (LckWallCameraSpawner._prewarmCamera != null)
		{
			return;
		}
		GameObject gameObject = new GameObject("prewarm camera");
		gameObject.transform.SetParent(base.transform);
		LckWallCameraSpawner._prewarmCamera = gameObject.AddComponent<Camera>();
		Camera main = Camera.main;
		LckWallCameraSpawner._prewarmCamera.clearFlags = main.clearFlags;
		LckWallCameraSpawner._prewarmCamera.fieldOfView = main.fieldOfView;
		LckWallCameraSpawner._prewarmCamera.nearClipPlane = main.nearClipPlane;
		LckWallCameraSpawner._prewarmCamera.farClipPlane = main.farClipPlane;
		LckWallCameraSpawner._prewarmCamera.cullingMask = main.cullingMask;
		LckWallCameraSpawner._prewarmCamera.tag = "Untagged";
		LckWallCameraSpawner._prewarmCamera.stereoTargetEye = StereoTargetEyeMask.None;
		LckWallCameraSpawner._prewarmCamera.targetTexture = new RenderTexture(32, 32, GraphicsFormat.R8G8B8A8_UNorm, GraphicsFormat.D32_SFloat_S8_UInt);
		LckWallCameraSpawner._prewarmCamera.transform.SetPositionAndRotation(main.transform.position, main.transform.rotation);
		base.StartCoroutine(this.DestroyPrewarmCameraDelayed());
	}

	// Token: 0x060018D7 RID: 6359 RVA: 0x0008CB0C File Offset: 0x0008AD0C
	private IEnumerator DestroyPrewarmCameraDelayed()
	{
		yield return new WaitForSeconds(1f);
		this.DestroyPrewarmCamera();
		yield break;
	}

	// Token: 0x060018D8 RID: 6360 RVA: 0x0008CB1B File Offset: 0x0008AD1B
	private void DestroyPrewarmCamera()
	{
		if (LckWallCameraSpawner._prewarmCamera == null)
		{
			return;
		}
		RenderTexture targetTexture = LckWallCameraSpawner._prewarmCamera.targetTexture;
		LckWallCameraSpawner._prewarmCamera.targetTexture = null;
		targetTexture.Release();
		Object.Destroy(LckWallCameraSpawner._prewarmCamera.gameObject);
		LckWallCameraSpawner._prewarmCamera = null;
	}

	// Token: 0x040023E5 RID: 9189
	[SerializeField]
	private GameObject _lckBodySpawnerPrefab;

	// Token: 0x040023E6 RID: 9190
	[SerializeField]
	private LckDirectGrabbable _cameraHandleGrabbable;

	// Token: 0x040023E7 RID: 9191
	[SerializeField]
	private Transform _cameraModelOriginTransform;

	// Token: 0x040023E8 RID: 9192
	[SerializeField]
	private Transform _cameraModelTransform;

	// Token: 0x040023E9 RID: 9193
	[SerializeField]
	private LineRenderer _cameraStrapRenderer;

	// Token: 0x040023EA RID: 9194
	[SerializeField]
	private float _activateDistance = 0.25f;

	// Token: 0x040023EB RID: 9195
	[SerializeField]
	private Transform[] _cameraStrapPoints;

	// Token: 0x040023EC RID: 9196
	private Vector3[] _cameraStrapPositions;

	// Token: 0x040023ED RID: 9197
	private float _spawnRotationOffsetAndroid = -80f;

	// Token: 0x040023EE RID: 9198
	private float _spawnRotationOffsetWindows = -55f;

	// Token: 0x040023EF RID: 9199
	[SerializeField]
	private Color _normalColor = Color.red;

	// Token: 0x040023F0 RID: 9200
	[Header("Cosmetics References")]
	[SerializeField]
	private GtDummyTablet _dummyTablet;

	// Token: 0x040023F1 RID: 9201
	[SerializeField]
	private LckGameObjectSwapCosmetic _swapTablet;

	// Token: 0x040023F2 RID: 9202
	[SerializeField]
	private LckGameObjectSwapCosmetic _swapEmobi;

	// Token: 0x040023F3 RID: 9203
	private static LckBodyCameraSpawner _bodySpawner;

	// Token: 0x040023F4 RID: 9204
	private static Camera _prewarmCamera;

	// Token: 0x040023F5 RID: 9205
	private LckWallCameraSpawner.WallSpawnerState _wallSpawnerState;

	// Token: 0x02000412 RID: 1042
	public enum WallSpawnerState
	{
		// Token: 0x040023F7 RID: 9207
		CameraOnHook,
		// Token: 0x040023F8 RID: 9208
		CameraDragging,
		// Token: 0x040023F9 RID: 9209
		CameraOffHook
	}
}
