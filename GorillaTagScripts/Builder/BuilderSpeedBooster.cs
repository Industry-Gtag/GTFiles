using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x0200104D RID: 4173
	public class BuilderSpeedBooster : MonoBehaviour
	{
		// Token: 0x06006823 RID: 26659 RVA: 0x002182BC File Offset: 0x002164BC
		private void Awake()
		{
			this.volume = base.GetComponent<Collider>();
			this.windRenderer.enabled = false;
			this.boosting = false;
		}

		// Token: 0x06006824 RID: 26660 RVA: 0x002182E0 File Offset: 0x002164E0
		private void LateUpdate()
		{
			if (this.audioSource && this.audioSource != null && !this.audioSource.isPlaying && this.audioSource.enabled)
			{
				this.audioSource.enabled = false;
			}
		}

		// Token: 0x06006825 RID: 26661 RVA: 0x00218330 File Offset: 0x00216530
		private bool TriggerFilter(Collider other, out Rigidbody rb, out Transform xf)
		{
			rb = null;
			xf = null;
			if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
			{
				rb = GorillaTagger.Instance.GetComponent<Rigidbody>();
				xf = GorillaTagger.Instance.headCollider.GetComponent<Transform>();
			}
			return rb != null && xf != null;
		}

		// Token: 0x06006826 RID: 26662 RVA: 0x00218390 File Offset: 0x00216590
		private void CheckTableZone()
		{
			if (this.hasCheckedZone)
			{
				return;
			}
			BuilderTable builderTable;
			if (BuilderTable.TryGetBuilderTableForZone(GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone, out builderTable))
			{
				this.ignoreMonkeScale = !builderTable.isTableMutable;
			}
			this.hasCheckedZone = true;
		}

		// Token: 0x06006827 RID: 26663 RVA: 0x002183DC File Offset: 0x002165DC
		public void OnTriggerEnter(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			this.CheckTableZone();
			if (!this.ignoreMonkeScale && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor > 0.99)
			{
				return;
			}
			this.positiveForce = Vector3.Dot(base.transform.up, rigidbody.linearVelocity) > 0f;
			if (this.positiveForce)
			{
				this.windRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
			}
			else
			{
				this.windRenderer.transform.localRotation = Quaternion.Euler(0f, 180f, -90f);
			}
			this.windRenderer.enabled = true;
			this.enterPos = transform.position;
			if (!this.boosting)
			{
				this.boosting = true;
				this.enterTime = Time.timeAsDouble;
			}
		}

		// Token: 0x06006828 RID: 26664 RVA: 0x002184CC File Offset: 0x002166CC
		public void OnTriggerExit(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			this.windRenderer.enabled = false;
			this.CheckTableZone();
			if (!this.ignoreMonkeScale && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor > 0.99)
			{
				return;
			}
			if (this.boosting && this.audioSource)
			{
				this.audioSource.enabled = true;
				this.audioSource.Stop();
				this.audioSource.GTPlayOneShot(this.exitClip, 1f);
			}
			this.boosting = false;
		}

		// Token: 0x06006829 RID: 26665 RVA: 0x0021856C File Offset: 0x0021676C
		public void OnTriggerStay(Collider other)
		{
			if (!this.boosting)
			{
				return;
			}
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			if (!this.ignoreMonkeScale && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor > 0.99)
			{
				this.OnTriggerExit(other);
				return;
			}
			if (Time.timeAsDouble > this.enterTime + (double)this.maxBoostDuration)
			{
				this.OnTriggerExit(other);
				return;
			}
			if (this.disableGrip)
			{
				GTPlayer.Instance.SetMaximumSlipThisFrame();
			}
			SizeManager sizeManager = null;
			if (this.scaleWithSize)
			{
				sizeManager = rigidbody.GetComponent<SizeManager>();
			}
			Vector3 vector = rigidbody.linearVelocity;
			if (this.scaleWithSize && sizeManager)
			{
				vector /= sizeManager.currentScale;
			}
			Vector3 vector2 = Vector3.Dot(transform.position - base.transform.position, base.transform.up) * base.transform.up;
			Vector3 vector3 = base.transform.position + vector2 - transform.position;
			float num = vector3.magnitude + 0.0001f;
			Vector3 vector4 = vector3 / num;
			float num2 = Vector3.Dot(vector, vector4);
			float num3 = this.accel;
			if (this.maxDepth > -1f)
			{
				float num4 = Vector3.Dot(transform.position - this.enterPos, vector4);
				float num5 = this.maxDepth - num4;
				float num6 = 0f;
				if (num5 > 0.0001f)
				{
					num6 = num2 * num2 / num5;
				}
				num3 = Mathf.Max(this.accel, num6);
			}
			float deltaTime = Time.deltaTime;
			Vector3 vector5 = base.transform.up * num3 * deltaTime;
			if (!this.positiveForce)
			{
				vector5 *= -1f;
			}
			vector += vector5;
			if ((double)Vector3.Dot(vector5, Vector3.down) <= 0.1)
			{
				vector += Vector3.up * this.addedWorldUpVelocity * deltaTime;
			}
			Vector3 vector6 = Mathf.Min(Vector3.Dot(vector, base.transform.up), this.maxSpeed) * base.transform.up;
			Vector3 vector7 = Vector3.Dot(vector, base.transform.right) * base.transform.right;
			Vector3 vector8 = Vector3.Dot(vector, base.transform.forward) * base.transform.forward;
			float num7 = 1f;
			float num8 = 1f;
			if (this.dampenLateralVelocity)
			{
				num7 = 1f - this.dampenXVelPerc * 0.01f * deltaTime;
				num8 = 1f - this.dampenZVelPerc * 0.01f * deltaTime;
			}
			vector = vector6 + num7 * vector7 + num8 * vector8;
			if (this.applyPullToCenterAcceleration && this.pullToCenterAccel > 0f && this.pullToCenterMaxSpeed > 0f)
			{
				vector -= num2 * vector4;
				if (num > this.pullTOCenterMinDistance)
				{
					num2 += this.pullToCenterAccel * deltaTime;
					float num9 = Mathf.Min(this.pullToCenterMaxSpeed, num / deltaTime);
					num2 = Mathf.Min(num2, num9);
				}
				else
				{
					num2 = 0f;
				}
				vector += num2 * vector4;
				if (vector.magnitude > 0.0001f)
				{
					Vector3 vector9 = Vector3.Cross(base.transform.up, vector4);
					float magnitude = vector9.magnitude;
					if (magnitude > 0.0001f)
					{
						vector9 /= magnitude;
						num2 = Vector3.Dot(vector, vector9);
						vector -= num2 * vector9;
						num2 -= this.pullToCenterAccel * deltaTime;
						num2 = Mathf.Max(0f, num2);
						vector += num2 * vector9;
					}
				}
			}
			if (this.scaleWithSize && sizeManager)
			{
				vector *= sizeManager.currentScale;
			}
			rigidbody.linearVelocity = vector;
		}

		// Token: 0x0600682A RID: 26666 RVA: 0x00218980 File Offset: 0x00216B80
		public void OnDrawGizmosSelected()
		{
			base.GetComponents<Collider>();
			Gizmos.color = Color.magenta;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.pullTOCenterMinDistance / base.transform.lossyScale.x, 1f, this.pullTOCenterMinDistance / base.transform.lossyScale.z));
		}

		// Token: 0x0400774D RID: 30541
		[SerializeField]
		public bool scaleWithSize = true;

		// Token: 0x0400774E RID: 30542
		[SerializeField]
		private float accel;

		// Token: 0x0400774F RID: 30543
		[SerializeField]
		private float maxDepth = -1f;

		// Token: 0x04007750 RID: 30544
		[SerializeField]
		private float maxSpeed;

		// Token: 0x04007751 RID: 30545
		[SerializeField]
		private bool disableGrip;

		// Token: 0x04007752 RID: 30546
		[SerializeField]
		private bool dampenLateralVelocity = true;

		// Token: 0x04007753 RID: 30547
		[SerializeField]
		private float dampenXVelPerc;

		// Token: 0x04007754 RID: 30548
		[SerializeField]
		private float dampenZVelPerc;

		// Token: 0x04007755 RID: 30549
		[SerializeField]
		private bool applyPullToCenterAcceleration = true;

		// Token: 0x04007756 RID: 30550
		[SerializeField]
		private float pullToCenterAccel;

		// Token: 0x04007757 RID: 30551
		[SerializeField]
		private float pullToCenterMaxSpeed;

		// Token: 0x04007758 RID: 30552
		[SerializeField]
		private float pullTOCenterMinDistance = 0.1f;

		// Token: 0x04007759 RID: 30553
		[SerializeField]
		private float addedWorldUpVelocity = 10f;

		// Token: 0x0400775A RID: 30554
		[SerializeField]
		private float maxBoostDuration = 2f;

		// Token: 0x0400775B RID: 30555
		private bool boosting;

		// Token: 0x0400775C RID: 30556
		private double enterTime;

		// Token: 0x0400775D RID: 30557
		private Collider volume;

		// Token: 0x0400775E RID: 30558
		public AudioClip exitClip;

		// Token: 0x0400775F RID: 30559
		public AudioSource audioSource;

		// Token: 0x04007760 RID: 30560
		public MeshRenderer windRenderer;

		// Token: 0x04007761 RID: 30561
		private Vector3 enterPos;

		// Token: 0x04007762 RID: 30562
		private bool positiveForce = true;

		// Token: 0x04007763 RID: 30563
		private bool ignoreMonkeScale;

		// Token: 0x04007764 RID: 30564
		private bool hasCheckedZone;
	}
}
