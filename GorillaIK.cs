using System;
using System.Collections;
using System.Collections.Generic;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Android;

// Token: 0x02000885 RID: 2181
public class GorillaIK : MonoBehaviour
{
	// Token: 0x060038DE RID: 14558 RVA: 0x00136269 File Offset: 0x00134469
	private void Awake()
	{
		this.bodyInitialRot = this.bodyBone.localRotation;
		this.myRig = base.GetComponent<VRRig>();
		this.anchorOverrides = base.GetComponentInChildren<VRRigAnchorOverrides>(true);
		this.ResetIKData();
	}

	// Token: 0x060038DF RID: 14559 RVA: 0x0013629B File Offset: 0x0013449B
	private void OnEnable()
	{
		GorillaIKMgr.Instance.RegisterIK(this);
		if (this.skeleton == null)
		{
			return;
		}
		GorillaIK.playerIK = this;
	}

	// Token: 0x060038E0 RID: 14560 RVA: 0x001362BD File Offset: 0x001344BD
	private void OnDisable()
	{
		GorillaIKMgr.Instance.DeregisterIK(this);
		this.ResetIKData();
	}

	// Token: 0x060038E1 RID: 14561 RVA: 0x001362D0 File Offset: 0x001344D0
	public void ResetIKData()
	{
		this.leftElbowDirection = Vector3.zero;
		this.lerpLeftElbowDirection = Vector3.zero;
		this.rightElbowDirection = Vector3.zero;
		this.lerpRightElbowDirection = Vector3.zero;
		this.targetBodyRot = this.bodyInitialRot;
		this.lerpBodyRot = this.targetBodyRot;
		if (this.projectedBodyRotation != null)
		{
			this.projectedBodyRotation.localRotation = this.targetBodyRot;
		}
		this.renderDisplacement = Vector3.zero;
		this.usingUpdatedIK = false;
	}

	// Token: 0x060038E2 RID: 14562 RVA: 0x00136352 File Offset: 0x00134552
	public bool CanUpdateIK()
	{
		return this.useUpdatedIKCoroutine == null;
	}

	// Token: 0x060038E3 RID: 14563 RVA: 0x0013635D File Offset: 0x0013455D
	public void DelayedUpdateIK(bool usingIK)
	{
		if (!this.CanUpdateIK())
		{
			return;
		}
		this.useUpdatedIKCoroutine = base.StartCoroutine(this.DoDelayedUpdateIK(usingIK));
	}

	// Token: 0x060038E4 RID: 14564 RVA: 0x0013637B File Offset: 0x0013457B
	private IEnumerator DoDelayedUpdateIK(bool usingIK)
	{
		yield return new WaitForSeconds(0.5f);
		this.ResetIKData();
		this.usingUpdatedIK = usingIK;
		yield return new WaitForSeconds(0.5f);
		this.useUpdatedIKCoroutine = null;
		yield break;
	}

	// Token: 0x1700050D RID: 1293
	// (get) Token: 0x060038E5 RID: 14565 RVA: 0x00136391 File Offset: 0x00134591
	// (set) Token: 0x060038E6 RID: 14566 RVA: 0x00136399 File Offset: 0x00134599
	public bool TickRunning { get; set; }

	// Token: 0x060038E7 RID: 14567 RVA: 0x001363A2 File Offset: 0x001345A2
	public void OverrideTargetPos(bool isLeftHand, Vector3 targetWorldPos)
	{
		if (isLeftHand)
		{
			this.hasLeftOverride = true;
			this.leftOverrideWorldPos = targetWorldPos;
			return;
		}
		this.hasRightOverride = true;
		this.rightOverrideWorldPos = targetWorldPos;
	}

	// Token: 0x060038E8 RID: 14568 RVA: 0x001363C4 File Offset: 0x001345C4
	public Vector3 GetShoulderLocalTargetPos_Left(bool updatedIK)
	{
		Vector3 vector = (this.hasLeftOverride ? this.leftOverrideWorldPos : this.targetLeft.position) + this.renderDisplacement;
		if (this.projectedBodyRotation != null && updatedIK)
		{
			return this.projectedLeftShoulderPosition.InverseTransformPoint(vector);
		}
		return this.leftUpperArm.parent.InverseTransformPoint(vector);
	}

	// Token: 0x060038E9 RID: 14569 RVA: 0x00136428 File Offset: 0x00134628
	public Vector3 GetShoulderLocalTargetPos_Right(bool updatedIK)
	{
		Vector3 vector = (this.hasRightOverride ? this.rightOverrideWorldPos : this.targetRight.position) + this.renderDisplacement;
		if (this.projectedBodyRotation != null && updatedIK)
		{
			return this.projectedRightShoulderPosition.InverseTransformPoint(vector);
		}
		return this.rightUpperArm.parent.InverseTransformPoint(vector);
	}

	// Token: 0x060038EA RID: 14570 RVA: 0x0013648A File Offset: 0x0013468A
	public void ClearOverrides()
	{
		this.hasLeftOverride = false;
		this.hasRightOverride = false;
	}

	// Token: 0x060038EB RID: 14571 RVA: 0x0013649C File Offset: 0x0013469C
	public void SkeletonUpdate()
	{
		if (!this.canUseUpdatedIK)
		{
			return;
		}
		bool subscriptionSettingBool = SubscriptionManager.GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures.IOBT);
		if (subscriptionSettingBool != this.skeleton.gameObject.activeSelf)
		{
			this.skeleton.gameObject.SetActive(subscriptionSettingBool);
			this.usingUpdatedIK = subscriptionSettingBool;
			if (this.anchorOverrides != null)
			{
				this.anchorOverrides.EnableChestBodyTracking(subscriptionSettingBool);
			}
			if (!subscriptionSettingBool)
			{
				this.ResetIKData();
			}
			return;
		}
		if (!subscriptionSettingBool)
		{
			return;
		}
		if (this.skeleton == null || this.skeleton.Bones == null || this.skeleton.Bones.Count == 0)
		{
			return;
		}
		if (this.boneXforms[0] == null || this.body == null || this.leftArmUpper == null || this.leftArmLower == null || this.rightArmUpper == null || this.rightArmLower == null)
		{
			foreach (OVRBone ovrbone in this.skeleton.Bones)
			{
				this.boneXforms[(int)ovrbone.Id] = ovrbone.Transform;
			}
			this.body = this.boneXforms[5];
			this.leftArmUpper = this.boneXforms[10];
			this.leftArmLower = this.boneXforms[11];
			this.rightArmUpper = this.boneXforms[15];
			this.rightArmLower = this.boneXforms[16];
			return;
		}
		this.usingUpdatedIK = true;
		if (!this.calibrating)
		{
			this.targetBodyRot = Quaternion.Inverse(this.bodyBone.parent.rotation) * this.skeleton.transform.rotation * this.body.localRotation * this.bodyOffsetRotation * this.leanOffsetRotation;
		}
		else
		{
			this.targetBodyRot = Quaternion.Inverse(this.bodyBone.parent.rotation) * this.skeleton.transform.rotation * this.body.localRotation * this.bodyOffsetRotation;
		}
		this.projectedBodyRotation.localRotation = this.targetBodyRot;
		this.leftElbowDirection = this.projectedLeftShoulderPosition.InverseTransformDirection((this.leftArmLower.position - this.leftArmLower.up * this.biasDistance - this.targetLeft.position).normalized).normalized;
		this.rightElbowDirection = this.projectedRightShoulderPosition.InverseTransformDirection((this.rightArmLower.position + this.rightArmLower.up * this.biasDistance - this.targetRight.position).normalized).normalized;
	}

	// Token: 0x060038EC RID: 14572 RVA: 0x001367A0 File Offset: 0x001349A0
	[ContextMenu("Calibrate Lean Offset")]
	public void CalibrateLeanOffset()
	{
		if (this.calibrateCoroutine != null)
		{
			return;
		}
		this.calibrateCoroutine = base.StartCoroutine(this.CalibrateLeanOffsetCoroutine());
	}

	// Token: 0x060038ED RID: 14573 RVA: 0x001367BD File Offset: 0x001349BD
	private IEnumerator CalibrateLeanOffsetCoroutine()
	{
		this.calibrating = true;
		yield return new WaitForSeconds(1f);
		List<Vector3> vecs = new List<Vector3>();
		int maxTries = 5;
		int num;
		for (int tries = 0; tries < maxTries; tries = num + 1)
		{
			yield return new WaitForSeconds(0.2f);
			vecs.Add(this.projectedBodyRotation.InverseTransformDirection(Vector3.up).normalized);
			num = tries;
		}
		this.leanOffsetRotation = Quaternion.FromToRotation(Vector3.up, this.CalculateAverage(vecs));
		this.SaveLeanOffset();
		this.calibrating = false;
		this.calibrateCoroutine = null;
		yield break;
	}

	// Token: 0x060038EE RID: 14574 RVA: 0x001367CC File Offset: 0x001349CC
	[ContextMenu("Reset Lean Offset")]
	public void ResetLeanOffset()
	{
		if (this.calibrateCoroutine != null)
		{
			return;
		}
		this.leanOffsetRotation = Quaternion.identity;
		this.SaveLeanOffset();
	}

	// Token: 0x060038EF RID: 14575 RVA: 0x001367E8 File Offset: 0x001349E8
	private void LoadLeanOffset()
	{
		if (!PlayerPrefs.HasKey("_GorillaIKLeanOffset_X"))
		{
			return;
		}
		float @float = PlayerPrefs.GetFloat("_GorillaIKLeanOffset_X");
		float float2 = PlayerPrefs.GetFloat("_GorillaIKLeanOffset_Y");
		float float3 = PlayerPrefs.GetFloat("_GorillaIKLeanOffset_Z");
		float float4 = PlayerPrefs.GetFloat("_GorillaIKLeanOffset_W");
		this.leanOffsetRotation = new Quaternion(@float, float2, float3, float4);
	}

	// Token: 0x060038F0 RID: 14576 RVA: 0x00136840 File Offset: 0x00134A40
	private void SaveLeanOffset()
	{
		PlayerPrefs.SetFloat("_GorillaIKLeanOffset_X", this.leanOffsetRotation.x);
		PlayerPrefs.SetFloat("_GorillaIKLeanOffset_Y", this.leanOffsetRotation.y);
		PlayerPrefs.SetFloat("_GorillaIKLeanOffset_Z", this.leanOffsetRotation.z);
		PlayerPrefs.SetFloat("_GorillaIKLeanOffset_W", this.leanOffsetRotation.w);
		PlayerPrefs.Save();
	}

	// Token: 0x060038F1 RID: 14577 RVA: 0x001368A8 File Offset: 0x00134AA8
	private Vector3 CalculateAverage(List<Vector3> vecs)
	{
		if (vecs == null || vecs.Count == 0)
		{
			return Vector3.zero;
		}
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < vecs.Count; i++)
		{
			vector += vecs[i];
		}
		return vector / (float)vecs.Count;
	}

	// Token: 0x060038F2 RID: 14578 RVA: 0x001368F8 File Offset: 0x00134AF8
	private Quaternion CalculateAverage(List<Quaternion> quats, int index = 0)
	{
		if (index >= quats.Count - 1)
		{
			return quats[index];
		}
		return Quaternion.Lerp(quats[index], this.CalculateAverage(quats, index + 1), 0.5f);
	}

	// Token: 0x060038F3 RID: 14579 RVA: 0x00136928 File Offset: 0x00134B28
	private void CheckPermissions()
	{
		if (!Permission.HasUserAuthorizedPermission("com.oculus.permission.BODY_TRACKING"))
		{
			PermissionCallbacks permissionCallbacks = new PermissionCallbacks();
			permissionCallbacks.PermissionGranted += this.PermissionGranted;
			Permission.RequestUserPermission("com.oculus.permission.BODY_TRACKING", permissionCallbacks);
			return;
		}
		this.PermissionGranted("");
	}

	// Token: 0x060038F4 RID: 14580 RVA: 0x00136970 File Offset: 0x00134B70
	private void PermissionGranted(string permissionName)
	{
		GorillaIKMgr.AddPlayerIK(this);
		this.boneXforms = new Transform[84];
		this.leftElbowDirection = Vector3.zero;
		this.rightElbowDirection = Vector3.zero;
		this.targetBodyRot = this.bodyInitialRot;
		this.canUseUpdatedIK = true;
	}

	// Token: 0x040048D5 RID: 18645
	private const string LeanOffsetSavePrefsKey = "_GorillaIKLeanOffset";

	// Token: 0x040048D6 RID: 18646
	public static GorillaIK playerIK;

	// Token: 0x040048D7 RID: 18647
	public Transform headBone;

	// Token: 0x040048D8 RID: 18648
	public Transform bodyBone;

	// Token: 0x040048D9 RID: 18649
	public Transform leftUpperArm;

	// Token: 0x040048DA RID: 18650
	public Transform leftLowerArm;

	// Token: 0x040048DB RID: 18651
	public Transform leftHand;

	// Token: 0x040048DC RID: 18652
	public Transform rightUpperArm;

	// Token: 0x040048DD RID: 18653
	public Transform rightLowerArm;

	// Token: 0x040048DE RID: 18654
	public Transform rightHand;

	// Token: 0x040048DF RID: 18655
	public Transform targetLeft;

	// Token: 0x040048E0 RID: 18656
	public Transform targetRight;

	// Token: 0x040048E1 RID: 18657
	public Transform targetHead;

	// Token: 0x040048E2 RID: 18658
	public Quaternion initialUpperLeft;

	// Token: 0x040048E3 RID: 18659
	public Quaternion initialLowerLeft;

	// Token: 0x040048E4 RID: 18660
	public Quaternion initialUpperRight;

	// Token: 0x040048E5 RID: 18661
	public Quaternion initialLowerRight;

	// Token: 0x040048E6 RID: 18662
	[NonSerialized]
	public Quaternion targetBodyRot;

	// Token: 0x040048E7 RID: 18663
	[NonSerialized]
	public Quaternion lerpBodyRot;

	// Token: 0x040048E8 RID: 18664
	[NonSerialized]
	public Vector3 leftElbowDirection;

	// Token: 0x040048E9 RID: 18665
	[NonSerialized]
	public Vector3 lerpLeftElbowDirection;

	// Token: 0x040048EA RID: 18666
	[NonSerialized]
	public Vector3 rightElbowDirection;

	// Token: 0x040048EB RID: 18667
	[NonSerialized]
	public Vector3 lerpRightElbowDirection;

	// Token: 0x040048EC RID: 18668
	public bool usingUpdatedIK;

	// Token: 0x040048ED RID: 18669
	public bool canUseUpdatedIK;

	// Token: 0x040048EE RID: 18670
	private Coroutine useUpdatedIKCoroutine;

	// Token: 0x040048EF RID: 18671
	public Quaternion bodyOffsetRotation;

	// Token: 0x040048F0 RID: 18672
	public Quaternion leanOffsetRotation = Quaternion.identity;

	// Token: 0x040048F1 RID: 18673
	public OVRSkeleton skeleton;

	// Token: 0x040048F2 RID: 18674
	private Transform[] boneXforms;

	// Token: 0x040048F3 RID: 18675
	[NonSerialized]
	public Quaternion bodyInitialRot;

	// Token: 0x040048F4 RID: 18676
	public Transform projectedBodyRotation;

	// Token: 0x040048F5 RID: 18677
	public Transform projectedLeftShoulderPosition;

	// Token: 0x040048F6 RID: 18678
	public Transform projectedRightShoulderPosition;

	// Token: 0x040048F7 RID: 18679
	[NonSerialized]
	public VRRig myRig;

	// Token: 0x040048F8 RID: 18680
	public float biasDistance = 0.2f;

	// Token: 0x040048F9 RID: 18681
	private VRRigAnchorOverrides anchorOverrides;

	// Token: 0x040048FA RID: 18682
	private Coroutine calibrateCoroutine;

	// Token: 0x040048FB RID: 18683
	private bool calibrating;

	// Token: 0x040048FC RID: 18684
	private bool hasLeftOverride;

	// Token: 0x040048FD RID: 18685
	private Vector3 leftOverrideWorldPos;

	// Token: 0x040048FE RID: 18686
	private bool hasRightOverride;

	// Token: 0x040048FF RID: 18687
	private Vector3 rightOverrideWorldPos;

	// Token: 0x04004900 RID: 18688
	[NonSerialized]
	public Vector3 renderDisplacement;

	// Token: 0x04004902 RID: 18690
	private Transform body;

	// Token: 0x04004903 RID: 18691
	private Transform leftArmUpper;

	// Token: 0x04004904 RID: 18692
	private Transform leftArmLower;

	// Token: 0x04004905 RID: 18693
	private Transform rightArmUpper;

	// Token: 0x04004906 RID: 18694
	private Transform rightArmLower;
}
