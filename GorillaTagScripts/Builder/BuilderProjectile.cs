using System;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001040 RID: 4160
	public class BuilderProjectile : MonoBehaviour
	{
		// Token: 0x170009E1 RID: 2529
		// (get) Token: 0x060067B0 RID: 26544 RVA: 0x00215B94 File Offset: 0x00213D94
		// (set) Token: 0x060067B1 RID: 26545 RVA: 0x00215B9C File Offset: 0x00213D9C
		public Vector3 launchPosition { get; private set; }

		// Token: 0x140000AD RID: 173
		// (add) Token: 0x060067B2 RID: 26546 RVA: 0x00215BA8 File Offset: 0x00213DA8
		// (remove) Token: 0x060067B3 RID: 26547 RVA: 0x00215BE0 File Offset: 0x00213DE0
		public event BuilderProjectile.ProjectileImpactEvent OnImpact;

		// Token: 0x060067B4 RID: 26548 RVA: 0x00215C18 File Offset: 0x00213E18
		public void Launch(Vector3 position, Vector3 velocity, BuilderProjectileLauncher sourceObject, int projectileCount, float scale, int timeStamp)
		{
			this.particleLaunched = true;
			this.timeCreated = Time.time;
			this.projectileSource = sourceObject;
			float num = (NetworkSystem.Instance.ServerTimestamp - timeStamp) / 1000f;
			if (num >= this.lifeTime)
			{
				this.Deactivate();
				return;
			}
			this.timeCreated -= num;
			Vector3 vector = Vector3.ProjectOnPlane(velocity, Vector3.up);
			float num2 = 0.017453292f * Vector3.Angle(vector, velocity);
			float num3 = this.projectileRigidbody.mass * this.gravityMultiplier * ((scale < 1f) ? scale : 1f) * 9.8f;
			Vector3 vector2 = num * Mathf.Cos(num2) * vector;
			float num4 = velocity.z * num * Mathf.Sin(num2) - 0.5f * num3 * num * num;
			this.launchPosition = position + vector2 + num4 * Vector3.down;
			Transform transform = base.transform;
			transform.position = position;
			transform.localScale = Vector3.one * scale;
			base.GetComponent<Collider>().contactOffset = 0.01f * scale;
			RigidbodyWaterInteraction component = base.GetComponent<RigidbodyWaterInteraction>();
			if (component != null)
			{
				component.objectRadiusForWaterCollision = 0.02f * scale;
			}
			this.projectileRigidbody.useGravity = false;
			Vector3 vector3 = this.projectileRigidbody.mass * this.gravityMultiplier * ((scale < 1f) ? scale : 1f) * Physics.gravity;
			this.forceComponent.force = vector3;
			this.projectileRigidbody.linearVelocity = velocity + num * vector3;
			this.projectileId = projectileCount;
			this.projectileRigidbody.position = position;
			this.projectileSource.RegisterProjectile(this);
		}

		// Token: 0x060067B5 RID: 26549 RVA: 0x00215DD9 File Offset: 0x00213FD9
		protected void Awake()
		{
			this.projectileRigidbody = base.GetComponent<Rigidbody>();
			this.forceComponent = base.GetComponent<ConstantForce>();
			this.initialScale = base.transform.localScale.x;
		}

		// Token: 0x060067B6 RID: 26550 RVA: 0x00215E0C File Offset: 0x0021400C
		public void Deactivate()
		{
			base.transform.localScale = Vector3.one * this.initialScale;
			this.projectileRigidbody.useGravity = true;
			this.forceComponent.force = Vector3.zero;
			this.OnImpact = null;
			this.aoeKnockbackConfig = null;
			this.impactSoundVolumeOverride = null;
			this.impactSoundPitchOverride = null;
			this.impactEffectScaleMultiplier = 1f;
			this.gravityMultiplier = 1f;
			ObjectPools.instance.Destroy(base.gameObject);
		}

		// Token: 0x060067B7 RID: 26551 RVA: 0x00215EA4 File Offset: 0x002140A4
		private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
		{
			Vector3 vector = position + normal * this.impactEffectOffset;
			GameObject gameObject = ObjectPools.instance.Instantiate(prefab, vector, true);
			Vector3 localScale = base.transform.localScale;
			gameObject.transform.localScale = localScale * this.impactEffectScaleMultiplier;
			gameObject.transform.up = normal;
			SurfaceImpactFX component = gameObject.GetComponent<SurfaceImpactFX>();
			if (component != null)
			{
				component.SetScale(localScale.x * this.impactEffectScaleMultiplier);
			}
			SoundBankPlayer component2 = gameObject.GetComponent<SoundBankPlayer>();
			if (component2 != null && !component2.playOnEnable)
			{
				component2.Play(this.impactSoundVolumeOverride, this.impactSoundPitchOverride);
			}
		}

		// Token: 0x060067B8 RID: 26552 RVA: 0x00215F4C File Offset: 0x0021414C
		public void ApplyHitKnockback(Vector3 hitNormal)
		{
			if (this.aoeKnockbackConfig != null && this.aoeKnockbackConfig.Value.applyAOEKnockback)
			{
				Vector3 vector = Vector3.ProjectOnPlane(hitNormal, Vector3.up);
				vector.Normalize();
				Vector3 vector2 = 0.75f * vector + 0.25f * Vector3.up;
				vector2.Normalize();
				GTPlayer instance = GTPlayer.Instance;
				instance.ApplyKnockback(vector2, this.aoeKnockbackConfig.Value.knockbackVelocity, instance.scale < 0.9f);
			}
		}

		// Token: 0x060067B9 RID: 26553 RVA: 0x00215FDC File Offset: 0x002141DC
		private void OnEnable()
		{
			this.timeCreated = 0f;
			this.particleLaunched = false;
		}

		// Token: 0x060067BA RID: 26554 RVA: 0x00215FF0 File Offset: 0x002141F0
		protected void OnDisable()
		{
			this.particleLaunched = false;
			if (this.projectileSource != null)
			{
				this.projectileSource.UnRegisterProjectile(this);
			}
			this.projectileSource = null;
		}

		// Token: 0x060067BB RID: 26555 RVA: 0x0021601C File Offset: 0x0021421C
		public void UpdateProjectile()
		{
			if (this.particleLaunched)
			{
				if (Time.time > this.timeCreated + this.lifeTime)
				{
					this.Deactivate();
				}
				if (this.faceDirectionOfTravel)
				{
					Transform transform = base.transform;
					Vector3 position = transform.position;
					Vector3 vector = position - this.previousPosition;
					transform.rotation = ((vector.sqrMagnitude > 0f) ? Quaternion.LookRotation(vector) : transform.rotation);
					this.previousPosition = position;
				}
			}
		}

		// Token: 0x060067BC RID: 26556 RVA: 0x00216098 File Offset: 0x00214298
		private void OnCollisionEnter(Collision other)
		{
			if (!this.particleLaunched)
			{
				return;
			}
			BuilderPieceCollider component = other.transform.GetComponent<BuilderPieceCollider>();
			if (component != null && component.piece.gameObject.Equals(this.projectileSource.gameObject))
			{
				return;
			}
			ContactPoint contact = other.GetContact(0);
			if (other.collider.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider))
			{
				this.ApplyHitKnockback(-1f * contact.normal);
			}
			this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
			BuilderProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
			if (onImpact != null)
			{
				onImpact(this, contact.point, null);
			}
			this.Deactivate();
		}

		// Token: 0x060067BD RID: 26557 RVA: 0x00216150 File Offset: 0x00214350
		protected void OnCollisionStay(Collision other)
		{
			if (!this.particleLaunched)
			{
				return;
			}
			BuilderPieceCollider component = other.transform.GetComponent<BuilderPieceCollider>();
			if (component != null && component.piece.gameObject.Equals(this.projectileSource.gameObject))
			{
				return;
			}
			ContactPoint contact = other.GetContact(0);
			if (other.collider.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider))
			{
				this.ApplyHitKnockback(-1f * contact.normal);
			}
			this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
			BuilderProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
			if (onImpact != null)
			{
				onImpact(this, contact.point, null);
			}
			this.Deactivate();
		}

		// Token: 0x060067BE RID: 26558 RVA: 0x00216208 File Offset: 0x00214408
		protected void OnTriggerEnter(Collider other)
		{
			if (!this.particleLaunched)
			{
				return;
			}
			if (!NetworkSystem.Instance.InRoom || GorillaGameManager.instance == null)
			{
				return;
			}
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaTagCollider))
			{
				return;
			}
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			NetPlayer netPlayer = ((componentInParent != null) ? componentInParent.creator : null);
			if (netPlayer == null)
			{
				return;
			}
			if (netPlayer.IsLocal)
			{
				return;
			}
			this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, base.transform.position, Vector3.up);
			this.Deactivate();
		}

		// Token: 0x040076B5 RID: 30389
		public BuilderProjectileLauncher projectileSource;

		// Token: 0x040076B6 RID: 30390
		[Tooltip("Rotates to point along the Y axis after spawn.")]
		public GameObject surfaceImpactEffectPrefab;

		// Token: 0x040076B7 RID: 30391
		[Tooltip("Distance from the surface that the particle should spawn.")]
		private float impactEffectOffset;

		// Token: 0x040076B8 RID: 30392
		public float lifeTime = 20f;

		// Token: 0x040076B9 RID: 30393
		public bool faceDirectionOfTravel = true;

		// Token: 0x040076BA RID: 30394
		private bool particleLaunched;

		// Token: 0x040076BB RID: 30395
		private float timeCreated;

		// Token: 0x040076BD RID: 30397
		private Rigidbody projectileRigidbody;

		// Token: 0x040076BE RID: 30398
		public int projectileId;

		// Token: 0x040076BF RID: 30399
		private float initialScale;

		// Token: 0x040076C0 RID: 30400
		private Vector3 previousPosition;

		// Token: 0x040076C1 RID: 30401
		[HideInInspector]
		public SlingshotProjectile.AOEKnockbackConfig? aoeKnockbackConfig;

		// Token: 0x040076C2 RID: 30402
		[HideInInspector]
		public float? impactSoundVolumeOverride;

		// Token: 0x040076C3 RID: 30403
		[HideInInspector]
		public float? impactSoundPitchOverride;

		// Token: 0x040076C4 RID: 30404
		[HideInInspector]
		public float impactEffectScaleMultiplier = 1f;

		// Token: 0x040076C5 RID: 30405
		[HideInInspector]
		public float gravityMultiplier = 1f;

		// Token: 0x040076C6 RID: 30406
		private ConstantForce forceComponent;

		// Token: 0x02001041 RID: 4161
		// (Invoke) Token: 0x060067C1 RID: 26561
		public delegate void ProjectileImpactEvent(BuilderProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer);
	}
}
