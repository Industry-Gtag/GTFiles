using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x02000888 RID: 2184
public class GorillaIKMgr : MonoBehaviour
{
	// Token: 0x17000512 RID: 1298
	// (get) Token: 0x06003902 RID: 14594 RVA: 0x00136B98 File Offset: 0x00134D98
	public static GorillaIKMgr Instance
	{
		get
		{
			return GorillaIKMgr._instance;
		}
	}

	// Token: 0x06003903 RID: 14595 RVA: 0x00136BA0 File Offset: 0x00134DA0
	private void Awake()
	{
		GorillaIKMgr._instance = this;
		this.firstFrame = true;
		this.tAA = new TransformAccessArray(0, -1);
		this.transformList = new List<Transform>();
		this.job = new GorillaIKMgr.IKJob
		{
			constantInput = new NativeArray<GorillaIKMgr.IKConstantInput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			input = new NativeArray<GorillaIKMgr.IKInput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			output = new NativeArray<GorillaIKMgr.IKOutput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory)
		};
		this.jobXform = new GorillaIKMgr.IKTransformJob
		{
			transformRotations = new NativeArray<Quaternion>(160, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			transformPositions = new NativeArray<Vector3>(160, Allocator.Persistent, NativeArrayOptions.ClearMemory)
		};
	}

	// Token: 0x06003904 RID: 14596 RVA: 0x00136C48 File Offset: 0x00134E48
	private void OnDestroy()
	{
		this.jobHandle.Complete();
		this.jobXformHandle.Complete();
		this.jobXform.transformRotations.Dispose();
		this.jobXform.transformPositions.Dispose();
		this.tAA.Dispose();
		this.job.input.Dispose();
		this.job.constantInput.Dispose();
		this.job.output.Dispose();
	}

	// Token: 0x06003905 RID: 14597 RVA: 0x00136CC8 File Offset: 0x00134EC8
	public void RegisterIK(GorillaIK ik)
	{
		this.ikList.Add(ik);
		this.actualListSz += 2;
		this.updatedSinceLastRun = true;
		if (this.job.constantInput.IsCreated)
		{
			this.SetConstantData(ik, this.actualListSz - 2);
		}
	}

	// Token: 0x06003906 RID: 14598 RVA: 0x00136D18 File Offset: 0x00134F18
	public void DeregisterIK(GorillaIK ik)
	{
		int num = this.ikList.FindIndex((GorillaIK curr) => curr == ik);
		this.updatedSinceLastRun = true;
		this.ikList.RemoveAt(num);
		this.actualListSz -= 2;
		if (this.job.constantInput.IsCreated)
		{
			for (int i = num; i < this.actualListSz; i++)
			{
				this.job.constantInput[i] = this.job.constantInput[i + 2];
			}
		}
	}

	// Token: 0x06003907 RID: 14599 RVA: 0x00136DB4 File Offset: 0x00134FB4
	private void SetConstantData(GorillaIK ik, int index)
	{
		this.job.constantInput[index] = new GorillaIKMgr.IKConstantInput
		{
			initRotLower = ik.initialLowerLeft,
			initRotUpper = ik.initialUpperLeft,
			shoulderPosition = new Vector3(-0.018300775f, -0.04206751f, 0.08612572f),
			bodyPivotPos = new Vector3(0f, 0.011406422f, 1.6582015f),
			shoulderRot = new Quaternion(-0.59150106f, 0.3665933f, 0.20795153f, 0.68738055f)
		};
		this.job.constantInput[index + 1] = new GorillaIKMgr.IKConstantInput
		{
			initRotLower = ik.initialLowerRight,
			initRotUpper = ik.initialUpperRight,
			shoulderPosition = new Vector3(0.018300813f, -0.042066876f, 0.08613044f),
			bodyPivotPos = new Vector3(0f, 0.011406422f, 1.6582015f),
			shoulderRot = new Quaternion(-0.591501f, -0.3665933f, -0.20795153f, 0.6873807f)
		};
	}

	// Token: 0x06003908 RID: 14600 RVA: 0x00136ED8 File Offset: 0x001350D8
	private void CopyInput()
	{
		int num = 0;
		int i = 0;
		while (i < this.actualListSz)
		{
			GorillaIK gorillaIK = this.ikList[i / 2];
			bool usingUpdatedIK = gorillaIK.usingUpdatedIK;
			if (gorillaIK != GorillaIKMgr.playerIK)
			{
				gorillaIK.lerpLeftElbowDirection = Vector3.Lerp(gorillaIK.lerpLeftElbowDirection, gorillaIK.leftElbowDirection, this.lerpValue);
				gorillaIK.lerpRightElbowDirection = Vector3.Lerp(gorillaIK.lerpRightElbowDirection, gorillaIK.rightElbowDirection, this.lerpValue);
				gorillaIK.lerpBodyRot = (usingUpdatedIK ? Quaternion.Lerp(gorillaIK.lerpBodyRot, gorillaIK.targetBodyRot, this.lerpValue) : gorillaIK.bodyInitialRot);
			}
			else
			{
				gorillaIK.lerpLeftElbowDirection = gorillaIK.leftElbowDirection;
				gorillaIK.lerpRightElbowDirection = gorillaIK.rightElbowDirection;
				gorillaIK.lerpBodyRot = (usingUpdatedIK ? gorillaIK.targetBodyRot : gorillaIK.bodyInitialRot);
			}
			this.job.input[i] = new GorillaIKMgr.IKInput
			{
				targetPos = gorillaIK.GetShoulderLocalTargetPos_Left(usingUpdatedIK),
				elbowDir = gorillaIK.lerpLeftElbowDirection,
				bodyRot = gorillaIK.lerpBodyRot,
				usingNewIK = usingUpdatedIK
			};
			this.job.input[i + 1] = new GorillaIKMgr.IKInput
			{
				targetPos = gorillaIK.GetShoulderLocalTargetPos_Right(usingUpdatedIK),
				elbowDir = gorillaIK.lerpRightElbowDirection,
				bodyRot = gorillaIK.lerpBodyRot,
				usingNewIK = usingUpdatedIK
			};
			gorillaIK.ClearOverrides();
			i += 2;
			num++;
		}
	}

	// Token: 0x06003909 RID: 14601 RVA: 0x00137058 File Offset: 0x00135258
	private void CopyOutput()
	{
		bool flag = false;
		if (this.updatedSinceLastRun || this.tAA.length != this.ikList.Count * 8)
		{
			flag = true;
			this.tAA.Dispose();
			this.transformList.Clear();
		}
		for (int i = 0; i < this.ikList.Count; i++)
		{
			GorillaIK gorillaIK = this.ikList[i];
			if (flag || this.updatedSinceLastRun)
			{
				this.transformList.Add(gorillaIK.leftUpperArm);
				this.transformList.Add(gorillaIK.leftLowerArm);
				this.transformList.Add(gorillaIK.rightUpperArm);
				this.transformList.Add(gorillaIK.rightLowerArm);
				this.transformList.Add(gorillaIK.bodyBone);
				this.transformList.Add(gorillaIK.headBone);
				this.transformList.Add(gorillaIK.leftHand);
				this.transformList.Add(gorillaIK.rightHand);
			}
			this.jobXform.transformRotations[8 * i] = this.job.output[i * 2].upperArmLocalRot;
			this.jobXform.transformRotations[8 * i + 1] = this.job.output[i * 2].lowerArmLocalRot;
			this.jobXform.transformRotations[8 * i + 2] = this.job.output[i * 2 + 1].upperArmLocalRot;
			this.jobXform.transformRotations[8 * i + 3] = this.job.output[i * 2 + 1].lowerArmLocalRot;
			this.jobXform.transformRotations[8 * i + 4] = gorillaIK.lerpBodyRot;
			this.jobXform.transformRotations[8 * i + 5] = gorillaIK.targetHead.rotation;
			this.jobXform.transformRotations[8 * i + 6] = gorillaIK.targetLeft.rotation;
			this.jobXform.transformRotations[8 * i + 7] = gorillaIK.targetRight.rotation;
			this.jobXform.transformPositions[8 * i + 6] = this.job.output[i * 2].handLocalPosition;
			this.jobXform.transformPositions[8 * i + 7] = this.job.output[i * 2 + 1].handLocalPosition;
		}
		if (flag)
		{
			this.tAA = new TransformAccessArray(this.transformList.ToArray(), -1);
		}
		this.updatedSinceLastRun = false;
	}

	// Token: 0x0600390A RID: 14602 RVA: 0x00137310 File Offset: 0x00135510
	public void LateUpdate()
	{
		GorillaIK gorillaIK = GorillaIKMgr.playerIK;
		if (gorillaIK != null)
		{
			gorillaIK.SkeletonUpdate();
		}
		if (!this.firstFrame)
		{
			this.jobXformHandle.Complete();
		}
		this.CopyInput();
		this.jobHandle = this.job.Schedule(this.actualListSz, 20, default(JobHandle));
		this.jobHandle.Complete();
		this.CopyOutput();
		this.jobXformHandle = this.jobXform.Schedule(this.tAA, default(JobHandle));
		this.firstFrame = false;
	}

	// Token: 0x0600390B RID: 14603 RVA: 0x001373A0 File Offset: 0x001355A0
	public static void AddPlayerIK(GorillaIK _playerIK)
	{
		GorillaIKMgr.playerIK = _playerIK;
	}

	// Token: 0x04004911 RID: 18705
	[OnEnterPlay_SetNull]
	private static GorillaIKMgr _instance;

	// Token: 0x04004912 RID: 18706
	private const int MaxSize = 20;

	// Token: 0x04004913 RID: 18707
	private List<GorillaIK> ikList = new List<GorillaIK>(20);

	// Token: 0x04004914 RID: 18708
	private int actualListSz;

	// Token: 0x04004915 RID: 18709
	private JobHandle jobHandle;

	// Token: 0x04004916 RID: 18710
	private JobHandle jobXformHandle;

	// Token: 0x04004917 RID: 18711
	private bool firstFrame = true;

	// Token: 0x04004918 RID: 18712
	private TransformAccessArray tAA;

	// Token: 0x04004919 RID: 18713
	private List<Transform> transformList;

	// Token: 0x0400491A RID: 18714
	private bool updatedSinceLastRun;

	// Token: 0x0400491B RID: 18715
	public const int tFormCount = 8;

	// Token: 0x0400491C RID: 18716
	public static GorillaIK playerIK;

	// Token: 0x0400491D RID: 18717
	private float lerpValue = 0.155f;

	// Token: 0x0400491E RID: 18718
	private GorillaIKMgr.IKJob job;

	// Token: 0x0400491F RID: 18719
	private GorillaIKMgr.IKTransformJob jobXform;

	// Token: 0x02000889 RID: 2185
	private struct IKConstantInput
	{
		// Token: 0x04004920 RID: 18720
		public Quaternion initRotLower;

		// Token: 0x04004921 RID: 18721
		public Quaternion initRotUpper;

		// Token: 0x04004922 RID: 18722
		public Vector3 shoulderPosition;

		// Token: 0x04004923 RID: 18723
		public Vector3 bodyPivotPos;

		// Token: 0x04004924 RID: 18724
		public Quaternion bodyStartRot;

		// Token: 0x04004925 RID: 18725
		public Quaternion shoulderRot;
	}

	// Token: 0x0200088A RID: 2186
	private struct IKInput
	{
		// Token: 0x04004926 RID: 18726
		public bool usingNewIK;

		// Token: 0x04004927 RID: 18727
		public Vector3 targetPos;

		// Token: 0x04004928 RID: 18728
		public Vector3 elbowDir;

		// Token: 0x04004929 RID: 18729
		public Quaternion bodyRot;
	}

	// Token: 0x0200088B RID: 2187
	private struct IKOutput
	{
		// Token: 0x0600390D RID: 14605 RVA: 0x001373CF File Offset: 0x001355CF
		public IKOutput(Quaternion upperArmLocalRot_, Quaternion lowerArmLocalRot_, Vector3 _handLocalPosition)
		{
			this.upperArmLocalRot = upperArmLocalRot_;
			this.lowerArmLocalRot = lowerArmLocalRot_;
			this.handLocalPosition = _handLocalPosition;
		}

		// Token: 0x0400492A RID: 18730
		public Quaternion upperArmLocalRot;

		// Token: 0x0400492B RID: 18731
		public Quaternion lowerArmLocalRot;

		// Token: 0x0400492C RID: 18732
		public Vector3 handLocalPosition;
	}

	// Token: 0x0200088C RID: 2188
	[BurstCompile]
	private struct IKJob : IJobParallelFor
	{
		// Token: 0x0600390E RID: 14606 RVA: 0x001373E8 File Offset: 0x001355E8
		public void Execute(int i)
		{
			Quaternion initRotUpper = this.constantInput[i].initRotUpper;
			Vector3 vector = GorillaIKMgr.IKJob.upperArmLocalPos;
			Quaternion quaternion = initRotUpper * this.constantInput[i].initRotLower;
			Vector3 vector2 = vector + initRotUpper * GorillaIKMgr.IKJob.forearmLocalPos;
			Vector3 vector3 = vector2 + quaternion * GorillaIKMgr.IKJob.handLocalPos;
			float num = 0.001f;
			float magnitude = (vector - vector2).magnitude;
			float magnitude2 = (vector2 - vector3).magnitude;
			float num2 = magnitude + magnitude2 - num;
			Vector3 normalized = (vector3 - vector).normalized;
			Vector3 normalized2 = (vector2 - vector).normalized;
			Vector3 normalized3 = (vector3 - vector2).normalized;
			Vector3 normalized4 = (this.input[i].targetPos - vector).normalized;
			float num3 = Mathf.Clamp((this.input[i].targetPos - vector).magnitude, num, num2);
			float num4 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(normalized, normalized2), -1f, 1f));
			float num5 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(-normalized2, normalized3), -1f, 1f));
			float num6 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(normalized, normalized4), -1f, 1f));
			float num7 = Mathf.Acos(Mathf.Clamp((magnitude2 * magnitude2 - magnitude * magnitude - num3 * num3) / (-2f * magnitude * num3), -1f, 1f));
			float num8 = Mathf.Acos(Mathf.Clamp((num3 * num3 - magnitude * magnitude - magnitude2 * magnitude2) / (-2f * magnitude * magnitude2), -1f, 1f));
			Vector3 normalized5 = Vector3.Cross(normalized, normalized2).normalized;
			Vector3 normalized6 = Vector3.Cross(normalized, normalized4).normalized;
			Quaternion quaternion2 = Quaternion.AngleAxis((num7 - num4) * 57.29578f, Quaternion.Inverse(initRotUpper) * normalized5);
			Quaternion quaternion3 = Quaternion.AngleAxis((num8 - num5) * 57.29578f, Quaternion.Inverse(quaternion) * normalized5);
			Quaternion quaternion4 = Quaternion.AngleAxis(num6 * 57.29578f, Quaternion.Inverse(initRotUpper) * normalized6);
			Quaternion quaternion5 = this.constantInput[i].initRotUpper * quaternion4 * quaternion2;
			Quaternion quaternion6 = this.constantInput[i].initRotLower * quaternion3;
			Quaternion quaternion7 = this.input[i].bodyRot * this.constantInput[i].shoulderRot;
			Quaternion quaternion8 = quaternion7 * quaternion5;
			Quaternion quaternion9 = quaternion8 * quaternion6;
			Vector3 vector4 = this.constantInput[i].bodyPivotPos + this.input[i].bodyRot * this.constantInput[i].shoulderPosition + quaternion7 * GorillaIKMgr.IKJob.upperArmLocalPos + quaternion8 * GorillaIKMgr.IKJob.forearmLocalPos + quaternion9 * GorillaIKMgr.IKJob.handLocalPos;
			if (!this.input[i].usingNewIK)
			{
				this.output[i] = new GorillaIKMgr.IKOutput(quaternion5, quaternion6, vector4);
				return;
			}
			Vector3 normalized7 = this.input[i].elbowDir.normalized;
			Vector3 normalized8 = (vector + quaternion5 * GorillaIKMgr.IKJob.forearmLocalPos - vector).normalized;
			Vector3 normalized9 = Vector3.Cross(normalized4, normalized7).normalized;
			quaternion5 = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.Cross(normalized4, normalized8).normalized, normalized9, normalized4), normalized4) * quaternion5;
			this.output[i] = new GorillaIKMgr.IKOutput(quaternion5, quaternion6, vector4);
		}

		// Token: 0x0400492D RID: 18733
		public NativeArray<GorillaIKMgr.IKConstantInput> constantInput;

		// Token: 0x0400492E RID: 18734
		public NativeArray<GorillaIKMgr.IKInput> input;

		// Token: 0x0400492F RID: 18735
		public NativeArray<GorillaIKMgr.IKOutput> output;

		// Token: 0x04004930 RID: 18736
		private static readonly Vector3 upperArmLocalPos = new Vector3(0f, 0.1454885f, -0.02598158f);

		// Token: 0x04004931 RID: 18737
		private static readonly Vector3 forearmLocalPos = new Vector3(0f, 0.4061671f, 0f);

		// Token: 0x04004932 RID: 18738
		private static readonly Vector3 handLocalPos = new Vector3(0f, 0.3816895f, 0f);
	}

	// Token: 0x0200088D RID: 2189
	[BurstCompile]
	private struct IKTransformJob : IJobParallelForTransform
	{
		// Token: 0x06003910 RID: 14608 RVA: 0x0013784C File Offset: 0x00135A4C
		public void Execute(int index, TransformAccess xform)
		{
			if (index % 8 <= 4)
			{
				xform.localRotation = this.transformRotations[index];
			}
			else
			{
				xform.rotation = this.transformRotations[index];
			}
			if (index % 8 >= 6)
			{
				xform.localPosition = this.transformPositions[index];
			}
		}

		// Token: 0x04004933 RID: 18739
		public NativeArray<Quaternion> transformRotations;

		// Token: 0x04004934 RID: 18740
		public NativeArray<Vector3> transformPositions;
	}
}
