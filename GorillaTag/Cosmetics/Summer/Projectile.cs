using System;
using System.Collections.Generic;
using GorillaTag.Reactions;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics.Summer
{
	// Token: 0x02001374 RID: 4980
	public class Projectile : MonoBehaviour, IProjectile
	{
		// Token: 0x06007CB8 RID: 31928 RVA: 0x0028BDDD File Offset: 0x00289FDD
		protected void Awake()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.impactEffectSpawned = false;
			this.forceComponent = base.GetComponent<ConstantForce>();
		}

		// Token: 0x06007CB9 RID: 31929 RVA: 0x00002C2D File Offset: 0x00000E2D
		protected void OnEnable()
		{
		}

		// Token: 0x06007CBA RID: 31930 RVA: 0x0028BE00 File Offset: 0x0028A000
		public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progressStep)
		{
			Transform transform = base.transform;
			transform.SetPositionAndRotation(startPosition, startRotation);
			transform.localScale = Vector3.one * ownerRig.scaleFactor;
			if (this.rigidbody != null)
			{
				this.rigidbody.isKinematic = false;
				this.rigidbody.position = startPosition;
				this.rigidbody.rotation = startRotation;
				this.rigidbody.linearVelocity = velocity;
			}
			if (this.audioSource && this.launchAudio)
			{
				this.audioSource.GTPlayOneShot(this.launchAudio, 1f);
			}
			UnityEvent<float> unityEvent = this.onLaunchShared;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(chargeFrac);
		}

		// Token: 0x06007CBB RID: 31931 RVA: 0x0028BEB1 File Offset: 0x0028A0B1
		private bool IsTagValid(GameObject obj)
		{
			return this.collisionTags.Contains(obj.tag);
		}

		// Token: 0x06007CBC RID: 31932 RVA: 0x0028BEC4 File Offset: 0x0028A0C4
		private void HandleImpact(GameObject hitObject, Vector3 hitPosition, Vector3 hitNormal)
		{
			if (this.impactEffectSpawned)
			{
				return;
			}
			if (this.collisionTags.Count > 0 && !this.IsTagValid(hitObject))
			{
				return;
			}
			if (((1 << hitObject.layer) & this.collisionLayerMasks) == 0)
			{
				return;
			}
			this.SpawnImpactEffect(this.impactEffect, hitPosition, hitNormal);
			if (this.impactEffect != null)
			{
				SoundBankPlayer component = this.impactEffect.GetComponent<SoundBankPlayer>();
				if (component != null && !component.playOnEnable)
				{
					component.Play();
				}
			}
			this.impactEffectSpawned = true;
			if (this.destroyOnCollisionEnter)
			{
				if (this.destroyDelay > 0f)
				{
					base.Invoke("DestroyProjectile", this.destroyDelay);
					return;
				}
				this.DestroyProjectile();
			}
		}

		// Token: 0x06007CBD RID: 31933 RVA: 0x0028BF80 File Offset: 0x0028A180
		private void GetColliderHitInfo(Collider other, out Vector3 position, out Vector3 normal)
		{
			Vector3 vector = Time.fixedDeltaTime * 2f * this.rigidbody.linearVelocity;
			Vector3 vector2 = base.transform.position - vector;
			float magnitude = vector.magnitude;
			Vector3 vector3 = ((magnitude > 0f) ? (vector / magnitude) : Vector3.zero);
			RaycastHit raycastHit;
			if (other.Raycast(new Ray(vector2, vector3), out raycastHit, 2f * magnitude))
			{
				position = raycastHit.point;
				normal = raycastHit.normal;
				return;
			}
			position = base.transform.position;
			normal = Vector3.up;
		}

		// Token: 0x06007CBE RID: 31934 RVA: 0x0028C02C File Offset: 0x0028A22C
		private void OnCollisionEnter(Collision other)
		{
			ContactPoint contact = other.GetContact(0);
			this.HandleImpact(other.gameObject, contact.point, contact.normal);
		}

		// Token: 0x06007CBF RID: 31935 RVA: 0x0028C05C File Offset: 0x0028A25C
		private void OnCollisionStay(Collision other)
		{
			ContactPoint contact = other.GetContact(0);
			this.HandleImpact(other.gameObject, contact.point, contact.normal);
		}

		// Token: 0x06007CC0 RID: 31936 RVA: 0x0028C08C File Offset: 0x0028A28C
		private void OnTriggerEnter(Collider other)
		{
			Vector3 vector;
			Vector3 vector2;
			this.GetColliderHitInfo(other, out vector, out vector2);
			this.HandleImpact(other.gameObject, vector, vector2);
		}

		// Token: 0x06007CC1 RID: 31937 RVA: 0x0028C0B4 File Offset: 0x0028A2B4
		private void OnTriggerStay(Collider other)
		{
			Transform transform = base.transform;
			this.HandleImpact(other.gameObject, transform.position, -transform.forward);
		}

		// Token: 0x06007CC2 RID: 31938 RVA: 0x0028C0E8 File Offset: 0x0028A2E8
		private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
		{
			if (prefab != null)
			{
				Vector3 vector = position + normal * this.impactEffectOffset;
				GameObject gameObject = ObjectPools.instance.Instantiate(prefab, vector, true);
				gameObject.transform.up = normal;
				gameObject.transform.position = vector;
			}
			this.onImpactShared.Invoke();
			if (this.spawnWorldEffects != null)
			{
				this.spawnWorldEffects.RequestSpawn(position, normal);
			}
		}

		// Token: 0x06007CC3 RID: 31939 RVA: 0x0028C15C File Offset: 0x0028A35C
		private void DestroyProjectile()
		{
			this.impactEffectSpawned = false;
			if (this.forceComponent)
			{
				this.forceComponent.enabled = false;
			}
			if (ObjectPools.instance.DoesPoolExist(base.gameObject))
			{
				ObjectPools.instance.Destroy(base.gameObject);
				return;
			}
			Object.Destroy(base.gameObject);
		}

		// Token: 0x04008F6D RID: 36717
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04008F6E RID: 36718
		[SerializeField]
		private GameObject impactEffect;

		// Token: 0x04008F6F RID: 36719
		[SerializeField]
		private AudioClip launchAudio;

		// Token: 0x04008F70 RID: 36720
		[SerializeField]
		private LayerMask collisionLayerMasks;

		// Token: 0x04008F71 RID: 36721
		[SerializeField]
		private List<string> collisionTags = new List<string>();

		// Token: 0x04008F72 RID: 36722
		[SerializeField]
		private bool destroyOnCollisionEnter;

		// Token: 0x04008F73 RID: 36723
		[SerializeField]
		private float destroyDelay = 1f;

		// Token: 0x04008F74 RID: 36724
		[Tooltip("Distance from the surface that the particle should spawn.")]
		[SerializeField]
		private float impactEffectOffset = 0.1f;

		// Token: 0x04008F75 RID: 36725
		[SerializeField]
		private SpawnWorldEffects spawnWorldEffects;

		// Token: 0x04008F76 RID: 36726
		private ConstantForce forceComponent;

		// Token: 0x04008F77 RID: 36727
		public UnityEvent<float> onLaunchShared;

		// Token: 0x04008F78 RID: 36728
		public UnityEvent onImpactShared;

		// Token: 0x04008F79 RID: 36729
		private bool impactEffectSpawned;

		// Token: 0x04008F7A RID: 36730
		private Rigidbody rigidbody;
	}
}
