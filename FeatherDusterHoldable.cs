using System;
using UnityEngine;

// Token: 0x02000215 RID: 533
public class FeatherDusterHoldable : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06000E03 RID: 3587 RVA: 0x0004CD2B File Offset: 0x0004AF2B
	public void Awake()
	{
		this.timeSinceLastSound = this.soundCooldown;
		this.emissionModule = this.particleFx.emission;
		this.initialRateOverTime = this.emissionModule.rateOverTimeMultiplier;
	}

	// Token: 0x06000E04 RID: 3588 RVA: 0x0004CD5B File Offset: 0x0004AF5B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		this.lastWorldPos = base.transform.position;
		this.lastSliceTime = Time.unscaledTime;
		this.emissionModule.rateOverTimeMultiplier = 0f;
	}

	// Token: 0x06000E05 RID: 3589 RVA: 0x00019269 File Offset: 0x00017469
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000E06 RID: 3590 RVA: 0x0004CD90 File Offset: 0x0004AF90
	public void SliceUpdate()
	{
		float unscaledTime = Time.unscaledTime;
		float num = Mathf.Max(unscaledTime - this.lastSliceTime, Mathf.Epsilon);
		this.lastSliceTime = unscaledTime;
		this.timeSinceLastSound += num;
		Transform transform = base.transform;
		Vector3 position = transform.position;
		float num2 = (position - this.lastWorldPos).sqrMagnitude / num;
		this.emissionModule.rateOverTimeMultiplier = 0f;
		if (num2 >= this.collideMinSpeed * this.collideMinSpeed && Physics.OverlapSphereNonAlloc(position, this.overlapSphereRadius * transform.localScale.x, this.colliderResult, this.collisionLayer) > 0)
		{
			this.emissionModule.rateOverTimeMultiplier = this.initialRateOverTime;
			if (this.timeSinceLastSound >= this.soundCooldown)
			{
				this.soundBankPlayer.Play();
				this.timeSinceLastSound = 0f;
			}
		}
		this.lastWorldPos = position;
	}

	// Token: 0x040010C0 RID: 4288
	public LayerMask collisionLayer;

	// Token: 0x040010C1 RID: 4289
	public float overlapSphereRadius = 0.08f;

	// Token: 0x040010C2 RID: 4290
	[Tooltip("Collision is not tested until this speed requirement is met.")]
	private float collideMinSpeed = 1f;

	// Token: 0x040010C3 RID: 4291
	public ParticleSystem particleFx;

	// Token: 0x040010C4 RID: 4292
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x040010C5 RID: 4293
	[SerializeField]
	private float soundCooldown = 0.8f;

	// Token: 0x040010C6 RID: 4294
	private ParticleSystem.EmissionModule emissionModule;

	// Token: 0x040010C7 RID: 4295
	private float initialRateOverTime;

	// Token: 0x040010C8 RID: 4296
	private float timeSinceLastSound;

	// Token: 0x040010C9 RID: 4297
	private Vector3 lastWorldPos;

	// Token: 0x040010CA RID: 4298
	private float lastSliceTime;

	// Token: 0x040010CB RID: 4299
	private Collider[] colliderResult = new Collider[1];
}
