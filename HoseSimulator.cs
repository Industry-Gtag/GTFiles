using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020002E4 RID: 740
public class HoseSimulator : MonoBehaviour, ISpawnable
{
	// Token: 0x170001DC RID: 476
	// (get) Token: 0x060012D8 RID: 4824 RVA: 0x00064594 File Offset: 0x00062794
	// (set) Token: 0x060012D9 RID: 4825 RVA: 0x0006459C File Offset: 0x0006279C
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170001DD RID: 477
	// (get) Token: 0x060012DA RID: 4826 RVA: 0x000645A5 File Offset: 0x000627A5
	// (set) Token: 0x060012DB RID: 4827 RVA: 0x000645AD File Offset: 0x000627AD
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060012DC RID: 4828 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060012DD RID: 4829 RVA: 0x000645B8 File Offset: 0x000627B8
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.anchors = rig.cosmeticReferences.Get(this.startAnchorRef).GetComponent<HoseSimulatorAnchors>();
		if (this.skinnedMeshRenderer != null)
		{
			Bounds localBounds = this.skinnedMeshRenderer.localBounds;
			localBounds.extents = this.localBoundsOverride;
			this.skinnedMeshRenderer.localBounds = localBounds;
		}
		this.hoseSectionLengths = new float[this.hoseBones.Length - 1];
		this.hoseBonePositions = new Vector3[this.hoseBones.Length];
		this.hoseBoneVelocities = new Vector3[this.hoseBones.Length];
		for (int i = 0; i < this.hoseSectionLengths.Length; i++)
		{
			float num = 1f;
			this.hoseSectionLengths[i] = num;
			this.totalHoseLength += num;
		}
	}

	// Token: 0x060012DE RID: 4830 RVA: 0x00064680 File Offset: 0x00062880
	private void LateUpdate()
	{
		if (this.myHoldable.InLeftHand())
		{
			this.isLeftHanded = true;
		}
		else if (this.myHoldable.InRightHand())
		{
			this.isLeftHanded = false;
		}
		for (int i = 0; i < this.miscBones.Length; i++)
		{
			Transform transform = (this.isLeftHanded ? this.anchors.miscAnchorsLeft[i] : this.anchors.miscAnchorsRight[i]);
			this.miscBones[i].transform.position = transform.position;
			this.miscBones[i].transform.rotation = transform.rotation;
		}
		this.startAnchor = (this.isLeftHanded ? this.anchors.leftAnchorPoint : this.anchors.rightAnchorPoint);
		float x = this.myHoldable.transform.lossyScale.x;
		float num = 0f;
		Vector3 position = this.startAnchor.position;
		Vector3 vector = position + this.startAnchor.forward * this.startStiffness * x;
		Vector3 position2 = this.endAnchor.position;
		Vector3 vector2 = position2 - this.endAnchor.forward * this.endStiffness * x;
		for (int j = 0; j < this.hoseBones.Length; j++)
		{
			float num2 = num / this.totalHoseLength;
			Vector3 vector3 = BezierUtils.BezierSolve(num2, position, vector, vector2, position2);
			Vector3 vector4 = BezierUtils.BezierSolve(num2 + 0.1f, position, vector, vector2, position2);
			if (this.firstUpdate)
			{
				this.hoseBones[j].transform.position = vector3;
				this.hoseBonePositions[j] = vector3;
				this.hoseBoneVelocities[j] = Vector3.zero;
			}
			else
			{
				this.hoseBoneVelocities[j] *= this.damping;
				this.hoseBonePositions[j] += this.hoseBoneVelocities[j] * Time.deltaTime;
				float num3 = this.hoseBoneMaxDisplacement[j] * x;
				if ((vector3 - this.hoseBonePositions[j]).IsLongerThan(num3))
				{
					Vector3 vector5 = vector3 + (this.hoseBonePositions[j] - vector3).normalized * num3;
					this.hoseBoneVelocities[j] += (vector5 - this.hoseBonePositions[j]) / Time.deltaTime;
					this.hoseBonePositions[j] = vector5;
				}
				this.hoseBones[j].transform.position = this.hoseBonePositions[j];
			}
			this.hoseBones[j].transform.rotation = Quaternion.LookRotation(vector4 - vector3, this.endAnchor.transform.up);
			if (j < this.hoseSectionLengths.Length)
			{
				num += this.hoseSectionLengths[j];
			}
		}
		this.firstUpdate = false;
	}

	// Token: 0x060012DF RID: 4831 RVA: 0x000649BE File Offset: 0x00062BBE
	private void OnDrawGizmosSelected()
	{
		if (this.hoseBonePositions != null)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLineStrip(this.hoseBonePositions, false);
		}
	}

	// Token: 0x040016FE RID: 5886
	[SerializeField]
	private SkinnedMeshRenderer skinnedMeshRenderer;

	// Token: 0x040016FF RID: 5887
	[SerializeField]
	private Vector3 localBoundsOverride;

	// Token: 0x04001700 RID: 5888
	[SerializeField]
	private Transform[] miscBones;

	// Token: 0x04001701 RID: 5889
	[SerializeField]
	private Transform[] hoseBones;

	// Token: 0x04001702 RID: 5890
	[SerializeField]
	private float[] hoseBoneMaxDisplacement;

	// Token: 0x04001703 RID: 5891
	[SerializeField]
	private CosmeticRefID startAnchorRef;

	// Token: 0x04001704 RID: 5892
	private Transform startAnchor;

	// Token: 0x04001705 RID: 5893
	[SerializeField]
	private float startStiffness = 0.5f;

	// Token: 0x04001706 RID: 5894
	[SerializeField]
	private Transform endAnchor;

	// Token: 0x04001707 RID: 5895
	[SerializeField]
	private float endStiffness = 0.5f;

	// Token: 0x04001708 RID: 5896
	private Vector3[] hoseBonePositions;

	// Token: 0x04001709 RID: 5897
	private Vector3[] hoseBoneVelocities;

	// Token: 0x0400170A RID: 5898
	[SerializeField]
	private float damping = 0.97f;

	// Token: 0x0400170B RID: 5899
	private float[] hoseSectionLengths;

	// Token: 0x0400170C RID: 5900
	private float totalHoseLength;

	// Token: 0x0400170D RID: 5901
	private bool firstUpdate = true;

	// Token: 0x0400170E RID: 5902
	private HoseSimulatorAnchors anchors;

	// Token: 0x0400170F RID: 5903
	[SerializeField]
	private TransferrableObject myHoldable;

	// Token: 0x04001710 RID: 5904
	private bool isLeftHanded;
}
