using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000213 RID: 531
public class WizardStaffHoldable : TransferrableObject
{
	// Token: 0x06000DF6 RID: 3574 RVA: 0x0004C8A5 File Offset: 0x0004AAA5
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.tipTargetLocalPosition = this.tipTransform.localPosition;
		this.hasEffectsGameObject = this.effectsGameObject != null;
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x06000DF7 RID: 3575 RVA: 0x0004C8D8 File Offset: 0x0004AAD8
	internal override void OnEnable()
	{
		base.OnEnable();
		this.InitToDefault();
	}

	// Token: 0x06000DF8 RID: 3576 RVA: 0x0004C8E6 File Offset: 0x0004AAE6
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06000DF9 RID: 3577 RVA: 0x0004C8F4 File Offset: 0x0004AAF4
	private void InitToDefault()
	{
		this.cooldownRemaining = 0f;
		if (this.hasEffectsGameObject && this.effectsHaveBeenPlayed)
		{
			this.effectsGameObject.SetActive(false);
		}
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x06000DFA RID: 3578 RVA: 0x0004C924 File Offset: 0x0004AB24
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!base.InHand() || this.itemState == TransferrableObject.ItemStates.State1 || !GorillaParent.hasInstance || !this.hitLastFrame)
		{
			return;
		}
		if (this.velocityEstimator.linearVelocity.magnitude < this.minSlamVelocity)
		{
			return;
		}
		Vector3 up = this.tipTransform.up;
		Vector3 up2 = Vector3.up;
		if (Vector3.Angle(up, up2) > this.minSlamAngle)
		{
			return;
		}
		this.itemState = TransferrableObject.ItemStates.State1;
		this.cooldownRemaining = this.cooldown;
	}

	// Token: 0x06000DFB RID: 3579 RVA: 0x0004C9A8 File Offset: 0x0004ABA8
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		this.cooldownRemaining -= Time.deltaTime;
		if (this.cooldownRemaining <= 0f)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
			if (this.hasEffectsGameObject)
			{
				this.effectsGameObject.SetActive(false);
			}
			this.effectsHaveBeenPlayed = false;
		}
		if (base.InHand())
		{
			Vector3 position = base.transform.position;
			Vector3 vector = base.transform.TransformPoint(this.tipTargetLocalPosition);
			RaycastHit raycastHit;
			if (Physics.Linecast(position, vector, out raycastHit, this.tipCollisionLayerMask))
			{
				this.tipTransform.position = raycastHit.point;
				this.hitLastFrame = true;
			}
			else
			{
				this.tipTransform.localPosition = this.tipTargetLocalPosition;
				this.hitLastFrame = false;
			}
			if (this.itemState == TransferrableObject.ItemStates.State1 && this.hasEffectsGameObject && !this.effectsHaveBeenPlayed)
			{
				this.effectsGameObject.SetActive(true);
				this.effectsHaveBeenPlayed = true;
			}
		}
	}

	// Token: 0x06000DFC RID: 3580 RVA: 0x0004CA98 File Offset: 0x0004AC98
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.effectsHaveBeenPlayed)
		{
			this.cooldownRemaining = this.cooldown;
		}
	}

	// Token: 0x040010AD RID: 4269
	[Tooltip("This GameObject will activate when the staff hits the ground with enough force.")]
	public GameObject effectsGameObject;

	// Token: 0x040010AE RID: 4270
	[Tooltip("The Transform of the staff's tip which will be used to determine if the staff is being slammed. Up axis (Y) should point along the length of the staff.")]
	public Transform tipTransform;

	// Token: 0x040010AF RID: 4271
	public float tipCollisionRadius = 0.05f;

	// Token: 0x040010B0 RID: 4272
	public LayerMask tipCollisionLayerMask;

	// Token: 0x040010B1 RID: 4273
	[Tooltip("Used to calculate velocity of the staff.")]
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x040010B2 RID: 4274
	public float cooldown = 5f;

	// Token: 0x040010B3 RID: 4275
	[Tooltip("The velocity of the staff's tip must be greater than this value to activate the effect.")]
	public float minSlamVelocity = 0.5f;

	// Token: 0x040010B4 RID: 4276
	[Tooltip("The angle (in degrees) between the staff's tip and the ground must be less than this value to activate the effect.")]
	public float minSlamAngle = 5f;

	// Token: 0x040010B5 RID: 4277
	[DebugReadout]
	private float cooldownRemaining;

	// Token: 0x040010B6 RID: 4278
	[DebugReadout]
	private bool hitLastFrame;

	// Token: 0x040010B7 RID: 4279
	private Vector3 tipTargetLocalPosition;

	// Token: 0x040010B8 RID: 4280
	private bool hasEffectsGameObject;

	// Token: 0x040010B9 RID: 4281
	private bool effectsHaveBeenPlayed;
}
