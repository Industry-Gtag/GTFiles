using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020009F4 RID: 2548
public class SpoonClacker : MonoBehaviour
{
	// Token: 0x06004166 RID: 16742 RVA: 0x0015C175 File Offset: 0x0015A375
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x06004167 RID: 16743 RVA: 0x0015C180 File Offset: 0x0015A380
	private void Setup()
	{
		JointLimits limits = this.hingeJoint.limits;
		this.hingeMin = limits.min;
		this.hingeMax = limits.max;
	}

	// Token: 0x06004168 RID: 16744 RVA: 0x0015C1B4 File Offset: 0x0015A3B4
	private void Update()
	{
		if (!this.transferObject)
		{
			return;
		}
		TransferrableObject.PositionState currentState = this.transferObject.currentState;
		if (currentState != TransferrableObject.PositionState.InLeftHand && currentState != TransferrableObject.PositionState.InRightHand)
		{
			return;
		}
		float num = MathUtils.Linear(this.hingeJoint.angle, this.hingeMin, this.hingeMax, 0f, 1f);
		float num2 = (this.invertOut ? (1f - num) : num) * 100f;
		this.skinnedMesh.SetBlendShapeWeight(this.targetBlendShape, num2);
		if (!this._lockMin && num <= this.minThreshold)
		{
			this.OnHitMin.Invoke();
			this._lockMin = true;
		}
		else if (!this._lockMax && num >= 1f - this.maxThreshold)
		{
			this.OnHitMax.Invoke();
			this._lockMax = true;
			if (this._sincelastHit.HasElapsed(this.multiHitCutoff, true))
			{
				this.soundsSingle.Play();
			}
			else
			{
				this.soundsMulti.Play();
			}
		}
		if (this._lockMin && num > this.minThreshold * this.hysterisisFactor)
		{
			this._lockMin = false;
		}
		if (this._lockMax && num < 1f - this.maxThreshold * this.hysterisisFactor)
		{
			this._lockMax = false;
		}
	}

	// Token: 0x04005204 RID: 20996
	public TransferrableObject transferObject;

	// Token: 0x04005205 RID: 20997
	public SkinnedMeshRenderer skinnedMesh;

	// Token: 0x04005206 RID: 20998
	public HingeJoint hingeJoint;

	// Token: 0x04005207 RID: 20999
	public int targetBlendShape;

	// Token: 0x04005208 RID: 21000
	public float hingeMin;

	// Token: 0x04005209 RID: 21001
	public float hingeMax;

	// Token: 0x0400520A RID: 21002
	public bool invertOut;

	// Token: 0x0400520B RID: 21003
	public float minThreshold = 0.01f;

	// Token: 0x0400520C RID: 21004
	public float maxThreshold = 0.01f;

	// Token: 0x0400520D RID: 21005
	public float hysterisisFactor = 4f;

	// Token: 0x0400520E RID: 21006
	public UnityEvent OnHitMin;

	// Token: 0x0400520F RID: 21007
	public UnityEvent OnHitMax;

	// Token: 0x04005210 RID: 21008
	private bool _lockMin;

	// Token: 0x04005211 RID: 21009
	private bool _lockMax;

	// Token: 0x04005212 RID: 21010
	public SoundBankPlayer soundsSingle;

	// Token: 0x04005213 RID: 21011
	public SoundBankPlayer soundsMulti;

	// Token: 0x04005214 RID: 21012
	private TimeSince _sincelastHit;

	// Token: 0x04005215 RID: 21013
	[FormerlySerializedAs("multiHitInterval")]
	public float multiHitCutoff = 0.1f;
}
