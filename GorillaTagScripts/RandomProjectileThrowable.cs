using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTagScripts
{
	// Token: 0x02000FA9 RID: 4009
	public class RandomProjectileThrowable : MonoBehaviour
	{
		// Token: 0x17000986 RID: 2438
		// (get) Token: 0x060063D0 RID: 25552 RVA: 0x00201BF8 File Offset: 0x001FFDF8
		// (set) Token: 0x060063D1 RID: 25553 RVA: 0x00201C00 File Offset: 0x001FFE00
		public float TimeEnabled { get; private set; }

		// Token: 0x17000987 RID: 2439
		// (get) Token: 0x060063D2 RID: 25554 RVA: 0x00201C09 File Offset: 0x001FFE09
		// (set) Token: 0x060063D3 RID: 25555 RVA: 0x00201C11 File Offset: 0x001FFE11
		public bool ForceDestroy { get; set; }

		// Token: 0x060063D4 RID: 25556 RVA: 0x00201C1A File Offset: 0x001FFE1A
		private void OnEnable()
		{
			this.TimeEnabled = Time.time;
			this.currentProjectile = this.projectilePrefab;
		}

		// Token: 0x060063D5 RID: 25557 RVA: 0x00201C33 File Offset: 0x001FFE33
		private void OnDisable()
		{
			this.ForceDestroy = false;
		}

		// Token: 0x060063D6 RID: 25558 RVA: 0x00201C3C File Offset: 0x001FFE3C
		public void ForceDestroyThrowable()
		{
			this.ForceDestroy = true;
		}

		// Token: 0x060063D7 RID: 25559 RVA: 0x00201C45 File Offset: 0x001FFE45
		public void UpdateProjectilePrefab()
		{
			this.currentProjectile = this.alternativeProjectilePrefab;
		}

		// Token: 0x060063D8 RID: 25560 RVA: 0x00201C53 File Offset: 0x001FFE53
		public GameObject GetProjectilePrefab()
		{
			return this.currentProjectile;
		}

		// Token: 0x060063D9 RID: 25561 RVA: 0x00201C5C File Offset: 0x001FFE5C
		private void OnTriggerEnter(Collider other)
		{
			if (!this.destroyOnTrigger)
			{
				return;
			}
			if (other.gameObject.layer == LayerMask.NameToLayer(this.triggerTag))
			{
				if (this.audioSource && this.triggerClip)
				{
					this.audioSource.GTPlayOneShot(this.triggerClip, 1f);
				}
				if (GorillaTagger.hasInstance && other == GorillaTagger.Instance.headCollider)
				{
					PlayerGameEvents.EatObject(this.interactEventName);
				}
				UnityEvent onDestroyed = this.OnDestroyed;
				if (onDestroyed != null)
				{
					onDestroyed.Invoke();
				}
				this.DestroyProjectile();
			}
		}

		// Token: 0x060063DA RID: 25562 RVA: 0x00201CF5 File Offset: 0x001FFEF5
		public void DestroyProjectile()
		{
			base.StartCoroutine(this.DestroyProjectileCoroutine(0.25f));
		}

		// Token: 0x060063DB RID: 25563 RVA: 0x00201D09 File Offset: 0x001FFF09
		private IEnumerator DestroyProjectileCoroutine(float delay)
		{
			yield return new WaitForSeconds(delay);
			UnityAction<bool> onDestroyRandomProjectile = this.OnDestroyRandomProjectile;
			if (onDestroyRandomProjectile != null)
			{
				onDestroyRandomProjectile(false);
			}
			yield break;
		}

		// Token: 0x0400727F RID: 29311
		public GameObject projectilePrefab;

		// Token: 0x04007280 RID: 29312
		[Tooltip("Use for a different/updated version of the projectile if needed.")]
		public GameObject alternativeProjectilePrefab;

		// Token: 0x04007281 RID: 29313
		[FormerlySerializedAs("weightedChance")]
		[Range(0f, 1f)]
		public float spawnChance = 1f;

		// Token: 0x04007282 RID: 29314
		[Tooltip("(Optional) name broadcast by PlayerGameEvents when the local player eats this projectile")]
		public string interactEventName;

		// Token: 0x04007283 RID: 29315
		[Tooltip("Requires a collider")]
		public bool destroyOnTrigger = true;

		// Token: 0x04007284 RID: 29316
		public string triggerTag = "Gorilla Head";

		// Token: 0x04007285 RID: 29317
		[FormerlySerializedAs("onMoveToHead")]
		public UnityEvent OnDestroyed;

		// Token: 0x04007286 RID: 29318
		public AudioSource audioSource;

		// Token: 0x04007287 RID: 29319
		public AudioClip triggerClip;

		// Token: 0x04007288 RID: 29320
		[Tooltip("Immediately destroys after the release")]
		public bool destroyAfterRelease;

		// Token: 0x04007289 RID: 29321
		[Tooltip("Set a timer to destroy after X seconds is passed and the object is not thrown yet")]
		[FormerlySerializedAs("destroyAfterSeconds")]
		public float autoDestroyAfterSeconds = -1f;

		// Token: 0x0400728A RID: 29322
		[Tooltip("If checked, any amount of passed time will be deducted from the lifetime of the slingshot projectile when thrownShould be less than or equal to lifetime of the slingshot projectile")]
		public bool moveOverPassedLifeTime;

		// Token: 0x0400728D RID: 29325
		public UnityAction<bool> OnDestroyRandomProjectile;

		// Token: 0x0400728E RID: 29326
		private GameObject currentProjectile;
	}
}
