using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x0200027B RID: 635
public class PropHuntHandFollower : MonoBehaviour, ICallBack
{
	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x06001126 RID: 4390 RVA: 0x0005BA78 File Offset: 0x00059C78
	// (set) Token: 0x06001127 RID: 4391 RVA: 0x0005BA80 File Offset: 0x00059C80
	public bool hasProp
	{
		get
		{
			return this._hasProp;
		}
		private set
		{
			this._hasProp = value;
		}
	}

	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x06001128 RID: 4392 RVA: 0x0005BA89 File Offset: 0x00059C89
	// (set) Token: 0x06001129 RID: 4393 RVA: 0x0005BA91 File Offset: 0x00059C91
	public bool IsInstantiatingAsync { get; private set; }

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x0600112A RID: 4394 RVA: 0x0005BA9A File Offset: 0x00059C9A
	// (set) Token: 0x0600112B RID: 4395 RVA: 0x0005BAA2 File Offset: 0x00059CA2
	public VRRig attachedToRig { get; private set; }

	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x0600112C RID: 4396 RVA: 0x0005BAAB File Offset: 0x00059CAB
	public bool IsLeftHand
	{
		get
		{
			return this._isLeftHand;
		}
	}

	// Token: 0x0600112D RID: 4397 RVA: 0x0005BAB3 File Offset: 0x00059CB3
	public void Awake()
	{
		this.attachedToRig = base.GetComponent<VRRig>();
		this.attachedToRig.propHuntHandFollower = this;
		this._isLocal = this.attachedToRig.isOfflineVRRig;
		this.raycastHits = new RaycastHit[20];
	}

	// Token: 0x0600112E RID: 4398 RVA: 0x0005BAEB File Offset: 0x00059CEB
	public void Start()
	{
		this.attachedToRig.AddLateUpdateCallback(this);
	}

	// Token: 0x0600112F RID: 4399 RVA: 0x0005BAF9 File Offset: 0x00059CF9
	private void OnEnable()
	{
		GorillaPropHuntGameManager.RegisterPropHandFollower(this);
	}

	// Token: 0x06001130 RID: 4400 RVA: 0x0005BB01 File Offset: 0x00059D01
	private void OnDisable()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		this.DestroyProp();
		GorillaPropHuntGameManager.UnregisterPropHandFollower(this);
	}

	// Token: 0x06001131 RID: 4401 RVA: 0x0005BB18 File Offset: 0x00059D18
	public void DestroyProp()
	{
		if (!this.hasProp || this._prop == null)
		{
			return;
		}
		PropHuntGrabbableProp propHuntGrabbableProp;
		PropHuntTaggableProp propHuntTaggableProp;
		if (this._prop.TryGetComponent<PropHuntGrabbableProp>(out propHuntGrabbableProp))
		{
			PropHuntPools.ReturnGrabbableProp(propHuntGrabbableProp);
		}
		else if (this._prop.TryGetComponent<PropHuntTaggableProp>(out propHuntTaggableProp))
		{
			PropHuntPools.ReturnTaggableProp(propHuntTaggableProp);
		}
		this._prop = null;
		this.hasProp = false;
	}

	// Token: 0x06001132 RID: 4402 RVA: 0x0005BB78 File Offset: 0x00059D78
	public static void DestroyProp_NoPool(List<MeshCollider> _colliders, ref bool hasProp, ref GameObject _prop)
	{
		foreach (MeshCollider meshCollider in _colliders)
		{
			if (!(meshCollider == null))
			{
				meshCollider.gameObject.transform.parent = null;
				meshCollider.gameObject.SetActive(false);
			}
		}
		if (hasProp)
		{
			Object.Destroy(_prop);
		}
		_prop = null;
		hasProp = false;
	}

	// Token: 0x06001133 RID: 4403 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnRoundStart()
	{
	}

	// Token: 0x06001134 RID: 4404 RVA: 0x0005BBF8 File Offset: 0x00059DF8
	public void CreateProp()
	{
		if (this.hasProp)
		{
			this.DestroyProp();
		}
		this._isLeftHand = false;
		int num = GorillaPropHuntGameManager.instance.GetSeed();
		if (NetworkSystem.Instance.InRoom)
		{
			num += this.attachedToRig.OwningNetPlayer.ActorNumber;
		}
		SRand srand = new SRand(num);
		string cosmeticId = GorillaPropHuntGameManager.instance.GetCosmeticId(srand.NextUInt());
		PropHuntTaggableProp propHuntTaggableProp;
		if (this._isLocal)
		{
			PropHuntGrabbableProp propHuntGrabbableProp;
			if (PropHuntPools.TryGetGrabbableProp(cosmeticId, out propHuntGrabbableProp))
			{
				this._grabbableProp = propHuntGrabbableProp;
				this._taggableProp = null;
				this._prop = propHuntGrabbableProp.gameObject;
				this._propOffset = this._grabbableProp.offset;
				propHuntGrabbableProp.handFollower = this;
				this.hasProp = true;
				for (int i = 0; i < propHuntGrabbableProp.interactionPoints.Count; i++)
				{
					propHuntGrabbableProp.interactionPoints[i].OnSpawn(this.attachedToRig);
				}
				return;
			}
		}
		else if (PropHuntPools.TryGetTaggableProp(cosmeticId, out propHuntTaggableProp))
		{
			this._taggableProp = propHuntTaggableProp;
			this._grabbableProp = null;
			this._prop = propHuntTaggableProp.gameObject;
			this._propOffset = propHuntTaggableProp.offset;
			propHuntTaggableProp.ownerRig = this.attachedToRig;
			this.hasProp = true;
		}
	}

	// Token: 0x06001135 RID: 4405 RVA: 0x0005BD28 File Offset: 0x00059F28
	public void OnPropLoaded(AsyncOperationHandle<GameObject> handle)
	{
		this.IsInstantiatingAsync = false;
		CosmeticSO cosmeticSO = null;
		if (PropHuntHandFollower.TryPrepPropTemplate(handle.Result, this._isLocal, cosmeticSO, this._colliders, this._interactionPoints, out this._grabbableProp, out this._taggableProp))
		{
			this._prop = handle.Result;
			this.hasProp = this._prop != null;
			this._prop.SetActive(true);
			if (this._isLocal)
			{
				this._propOffset = this._grabbableProp.offset;
				this._grabbableProp.handFollower = this;
				for (int i = 0; i < this._interactionPoints.Count; i++)
				{
					this._interactionPoints[i].OnSpawn(this.attachedToRig);
				}
				return;
			}
			this._propOffset = this._taggableProp.offset;
			this._taggableProp.ownerRig = this.attachedToRig;
		}
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x0005BE10 File Offset: 0x0005A010
	public static bool TryPrepPropTemplate(GameObject _prop, bool _isLocal, CosmeticSO debugCosmeticSO, List<MeshCollider> _colliders, List<InteractionPoint> ref_interactionPoints, out PropHuntGrabbableProp grabbableProp, out PropHuntTaggableProp taggableProp)
	{
		if (_isLocal)
		{
			grabbableProp = _prop.AddComponent<PropHuntGrabbableProp>();
			taggableProp = null;
			grabbableProp.interactionPoints = ref_interactionPoints;
		}
		else
		{
			taggableProp = _prop.AddComponent<PropHuntTaggableProp>();
			grabbableProp = null;
		}
		bool flag = false;
		bool flag2 = true;
		Bounds bounds = default(Bounds);
		int num = 0;
		foreach (MeshRenderer meshRenderer in _prop.GetComponentsInChildren<MeshRenderer>())
		{
			MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
			if (!(component == null))
			{
				Mesh sharedMesh = component.sharedMesh;
				if (!(sharedMesh == null) && sharedMesh.isReadable)
				{
					flag = true;
					if (flag2)
					{
						bounds = meshRenderer.bounds;
					}
					else
					{
						bounds.Encapsulate(meshRenderer.bounds);
					}
					MeshCollider meshCollider;
					if (num >= _colliders.Count)
					{
						GameObject gameObject = new GameObject("PropHuntTaggable");
						gameObject.layer = 14;
						meshCollider = gameObject.AddComponent<MeshCollider>();
						meshCollider.convex = true;
						meshCollider.isTrigger = true;
						if (_isLocal)
						{
							ref_interactionPoints.Add(gameObject.AddComponent<InteractionPoint>());
						}
						_colliders.Add(meshCollider);
					}
					else
					{
						meshCollider = _colliders[num];
						meshCollider.gameObject.SetActive(true);
					}
					meshCollider.transform.parent = _prop.transform;
					meshCollider.transform.position = meshRenderer.transform.position;
					meshCollider.transform.rotation = meshRenderer.transform.rotation;
					meshCollider.sharedMesh = sharedMesh;
					num++;
					flag2 = false;
				}
			}
		}
		if (!flag)
		{
			bool flag3 = true;
			PropHuntHandFollower.DestroyProp_NoPool(_colliders, ref flag3, ref _prop);
			return false;
		}
		Vector3 vector = _prop.transform.InverseTransformPoint(bounds.center);
		if (_isLocal)
		{
			grabbableProp.interactionPoints = ref_interactionPoints;
			grabbableProp.offset = vector;
		}
		else
		{
			taggableProp.offset = vector;
		}
		return true;
	}

	// Token: 0x06001137 RID: 4407 RVA: 0x0005BFD4 File Offset: 0x0005A1D4
	void ICallBack.CallBack()
	{
		if (!this.hasProp || this._prop.IsNull())
		{
			return;
		}
		Transform transform = (this._isLeftHand ? this.attachedToRig.leftHand.rigTarget : this.attachedToRig.rightHand.rigTarget);
		Vector3 vector = transform.position;
		if (this.attachedToRig.isLocal)
		{
			vector = (this._isLeftHand ? this.attachedToRig.leftHand.overrideTarget.position : this.attachedToRig.rightHand.overrideTarget.position);
		}
		if ((this._isLeftHand ? Mathf.Max(this.attachedToRig.leftIndex.calcT, this.attachedToRig.leftMiddle.calcT) : Mathf.Max(this.attachedToRig.rightIndex.calcT, this.attachedToRig.rightMiddle.calcT)) > 0.5f)
		{
			this._prop.transform.rotation = transform.TransformRotation(this._lastRelativeAngle);
			this._prop.transform.position = this.GeoCollisionPoint(vector, transform.TransformPoint(this._lastRelativePos) + this._prop.transform.TransformVector(this._propOffset)) - this._prop.transform.TransformVector(this._propOffset);
			this._networkLastRelativePos = transform.InverseTransformPoint(this._prop.transform.position);
			this._networkLastRelativeAngle = transform.InverseTransformRotation(this._prop.transform.rotation);
			return;
		}
		Vector3 vector2 = transform.transform.position - this._prop.transform.TransformPoint(this._propOffset);
		if (vector2.IsLongerThan(GorillaPropHuntGameManager.instance.HandFollowDistance))
		{
			float num = vector2.magnitude - GorillaPropHuntGameManager.instance.HandFollowDistance;
			this._prop.transform.position = this.GeoCollisionPoint(vector, this._prop.transform.position + this._prop.transform.TransformVector(this._propOffset) + vector2.normalized * num) - this._prop.transform.TransformVector(this._propOffset);
		}
		this._lastRelativePos = transform.InverseTransformPoint(this._prop.transform.position);
		this._lastRelativeAngle = transform.InverseTransformRotation(this._prop.transform.rotation);
		this._networkLastRelativePos = this._lastRelativePos;
		this._networkLastRelativeAngle = this._lastRelativeAngle;
	}

	// Token: 0x06001138 RID: 4408 RVA: 0x0005C288 File Offset: 0x0005A488
	public Vector3 GeoCollisionPoint(Vector3 sourcePos, Vector3 targetPos)
	{
		Vector3 vector = targetPos - sourcePos;
		int num = Physics.RaycastNonAlloc(sourcePos, vector.normalized, this.raycastHits, vector.magnitude, this.collisionLayers, QueryTriggerInteraction.Ignore);
		if (num > 0)
		{
			float num2 = vector.sqrMagnitude;
			Vector3 vector2 = targetPos;
			for (int i = 0; i < num; i++)
			{
				Vector3 vector3 = this.raycastHits[i].point - sourcePos;
				if (vector3.sqrMagnitude < num2)
				{
					vector2 = this.raycastHits[i].point;
					num2 = vector3.sqrMagnitude;
				}
			}
			return vector2;
		}
		return targetPos;
	}

	// Token: 0x06001139 RID: 4409 RVA: 0x0005C324 File Offset: 0x0005A524
	public void SwitchHand(bool newIsLeftHand)
	{
		if (this._isLeftHand == newIsLeftHand)
		{
			return;
		}
		this._isLeftHand = newIsLeftHand;
		Transform transform = (this._isLeftHand ? this.attachedToRig.leftHand.rigTarget : this.attachedToRig.rightHand.rigTarget);
		this._lastRelativePos = transform.InverseTransformPoint(this._prop.transform.position);
		this._lastRelativeAngle = transform.InverseTransformRotation(this._prop.transform.rotation);
	}

	// Token: 0x0600113A RID: 4410 RVA: 0x0005C3A5 File Offset: 0x0005A5A5
	public void SetProp(bool isLeftHand, Vector3 propPos, Quaternion propRot)
	{
		this._isLeftHand = isLeftHand;
		this._lastRelativePos = propPos;
		this._lastRelativeAngle = propRot;
	}

	// Token: 0x0600113B RID: 4411 RVA: 0x0005C3BC File Offset: 0x0005A5BC
	public long GetRelativePosRotLong()
	{
		if (this._prop.IsNull())
		{
			return BitPackUtils.PackHandPosRotForNetwork(Vector3.zero, Quaternion.identity);
		}
		return BitPackUtils.PackHandPosRotForNetwork(this._lastRelativePos, this._lastRelativeAngle);
	}

	// Token: 0x0400145D RID: 5213
	private const bool _k__GT_PROP_HUNT__USE_POOLING__ = true;

	// Token: 0x0400145E RID: 5214
	private const bool _k_isBetaOrEditor = false;

	// Token: 0x0400145F RID: 5215
	private const float HandFollowDistance = 0.1f;

	// Token: 0x04001460 RID: 5216
	private bool _hasProp;

	// Token: 0x04001463 RID: 5219
	private bool _isLocal;

	// Token: 0x04001464 RID: 5220
	private GameObject _prop;

	// Token: 0x04001465 RID: 5221
	private bool _isLeftHand;

	// Token: 0x04001466 RID: 5222
	private Vector3 _propOffset;

	// Token: 0x04001467 RID: 5223
	private readonly List<MeshCollider> _colliders = new List<MeshCollider>(4);

	// Token: 0x04001468 RID: 5224
	private readonly List<InteractionPoint> _interactionPoints = new List<InteractionPoint>(4);

	// Token: 0x04001469 RID: 5225
	private Vector3 _lastRelativePos;

	// Token: 0x0400146A RID: 5226
	private Quaternion _lastRelativeAngle;

	// Token: 0x0400146B RID: 5227
	private Vector3 _networkLastRelativePos;

	// Token: 0x0400146C RID: 5228
	private Quaternion _networkLastRelativeAngle;

	// Token: 0x0400146D RID: 5229
	public LayerMask collisionLayers;

	// Token: 0x0400146E RID: 5230
	private Vector3 targetPoint;

	// Token: 0x0400146F RID: 5231
	private RaycastHit[] raycastHits;

	// Token: 0x04001470 RID: 5232
	private PropHuntGrabbableProp _grabbableProp;

	// Token: 0x04001471 RID: 5233
	private PropHuntTaggableProp _taggableProp;
}
