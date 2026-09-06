using System;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x020008FB RID: 2299
public class HandTrackingFingerCurl : MonoBehaviour
{
	// Token: 0x17000575 RID: 1397
	// (get) Token: 0x06003C46 RID: 15430 RVA: 0x0014937C File Offset: 0x0014757C
	// (set) Token: 0x06003C47 RID: 15431 RVA: 0x00149384 File Offset: 0x00147584
	public float ThumbCurl { get; private set; }

	// Token: 0x17000576 RID: 1398
	// (get) Token: 0x06003C48 RID: 15432 RVA: 0x0014938D File Offset: 0x0014758D
	// (set) Token: 0x06003C49 RID: 15433 RVA: 0x00149395 File Offset: 0x00147595
	public float TriggerCurl { get; private set; }

	// Token: 0x17000577 RID: 1399
	// (get) Token: 0x06003C4A RID: 15434 RVA: 0x0014939E File Offset: 0x0014759E
	// (set) Token: 0x06003C4B RID: 15435 RVA: 0x001493A6 File Offset: 0x001475A6
	public float GripCurl { get; private set; }

	// Token: 0x06003C4C RID: 15436 RVA: 0x001493AF File Offset: 0x001475AF
	private void Awake()
	{
		if (this.isLeft)
		{
			HandTrackingFingerCurl.leftCurl = this;
		}
		else
		{
			HandTrackingFingerCurl.rightCurl = this;
		}
		if (this.skeleton == null)
		{
			this.skeleton = base.GetComponent<OVRSkeleton>();
		}
		this.boneXforms = new Transform[84];
	}

	// Token: 0x06003C4D RID: 15437 RVA: 0x001493F0 File Offset: 0x001475F0
	private void LateUpdate()
	{
		if (this.skeleton == null || this.skeleton.Bones == null || this.skeleton.Bones.Count == 0)
		{
			return;
		}
		if (!SubscriptionManager.IsLocalSubscribed() || !SubscriptionManager.GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures.HandTracking))
		{
			return;
		}
		if (this.boneXforms[0] == null)
		{
			foreach (OVRBone ovrbone in this.skeleton.Bones)
			{
				this.boneXforms[(int)ovrbone.Id] = ovrbone.Transform;
			}
		}
		this.ThumbCurl = this.CalcFingerCurl(OVRSkeleton.BoneId.Hand_Thumb3, OVRSkeleton.BoneId.Hand_Thumb2, OVRSkeleton.BoneId.Hand_Thumb1, OVRSkeleton.BoneId.Hand_Thumb0);
		this.TriggerCurl = this.CalcFingerCurl(OVRSkeleton.BoneId.Hand_Middle1, OVRSkeleton.BoneId.Hand_Index3, OVRSkeleton.BoneId.Hand_Index2, OVRSkeleton.BoneId.Hand_Index1);
		this.GripCurl = this.CalcFingerCurl(OVRSkeleton.BoneId.Hand_Ring3, OVRSkeleton.BoneId.Hand_Ring2, OVRSkeleton.BoneId.Hand_Ring1, OVRSkeleton.BoneId.Hand_Middle3);
	}

	// Token: 0x06003C4E RID: 15438 RVA: 0x001494D4 File Offset: 0x001476D4
	private float CalcFingerCurl(OVRSkeleton.BoneId distal, OVRSkeleton.BoneId intermediate, OVRSkeleton.BoneId proximal, OVRSkeleton.BoneId metacarpal)
	{
		Transform transform = this.boneXforms[(int)distal];
		Transform transform2 = this.boneXforms[(int)intermediate];
		Transform transform3 = this.boneXforms[(int)proximal];
		Transform transform4 = this.boneXforms[(int)metacarpal];
		if (transform == null || transform2 == null || transform3 == null || transform4 == null)
		{
			return 0f;
		}
		Vector3 vector = transform.position - transform2.position;
		Vector3 vector2 = transform2.position - transform3.position;
		Vector3 vector3 = transform3.position - transform4.position;
		float num = Vector3.Angle(vector, vector2);
		float num2 = Vector3.Angle(vector2, vector3);
		float num3 = (num + num2) * 0.5f;
		num3 *= this.CurlMultiplier;
		num3 = Mathf.InverseLerp(this.ActivationStart, this.ActivationEnd, num3);
		return Mathf.Clamp01(num3);
	}

	// Token: 0x04004CEC RID: 19692
	[SerializeField]
	private OVRSkeleton skeleton;

	// Token: 0x04004CED RID: 19693
	public float ActivationStart = 5f;

	// Token: 0x04004CEE RID: 19694
	public float ActivationEnd = 95f;

	// Token: 0x04004CEF RID: 19695
	public float CurlMultiplier = 1.2f;

	// Token: 0x04004CF3 RID: 19699
	private Transform[] boneXforms;

	// Token: 0x04004CF4 RID: 19700
	public static HandTrackingFingerCurl leftCurl;

	// Token: 0x04004CF5 RID: 19701
	public static HandTrackingFingerCurl rightCurl;

	// Token: 0x04004CF6 RID: 19702
	[SerializeField]
	private bool isLeft;
}
