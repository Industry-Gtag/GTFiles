using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001051 RID: 4177
	public class RecyclerForceVolume : MonoBehaviour
	{
		// Token: 0x0600683B RID: 26683 RVA: 0x00218EC2 File Offset: 0x002170C2
		private void Awake()
		{
			this.volume = base.GetComponent<Collider>();
			this.hasWindFX = this.windEffectRenderer != null;
			if (this.hasWindFX)
			{
				this.windEffectRenderer.enabled = false;
			}
		}

		// Token: 0x0600683C RID: 26684 RVA: 0x00218EF8 File Offset: 0x002170F8
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

		// Token: 0x0600683D RID: 26685 RVA: 0x00218F58 File Offset: 0x00217158
		public void OnTriggerEnter(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			this.enterPos = transform.position;
			ObjectPools.instance.Instantiate(this.windSFX, this.enterPos, true);
			if (this.hasWindFX)
			{
				this.windEffectRenderer.transform.position = base.transform.position + Vector3.Dot(this.enterPos - base.transform.position, base.transform.right) * base.transform.right;
				this.windEffectRenderer.enabled = true;
			}
		}

		// Token: 0x0600683E RID: 26686 RVA: 0x00219008 File Offset: 0x00217208
		public void OnTriggerExit(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			if (this.hasWindFX)
			{
				this.windEffectRenderer.enabled = false;
			}
		}

		// Token: 0x0600683F RID: 26687 RVA: 0x0021903C File Offset: 0x0021723C
		public void OnTriggerStay(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
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
			Vector3 vector2 = Vector3.Dot(base.transform.position - transform.position, base.transform.up) * base.transform.up;
			float num = vector2.magnitude + 0.0001f;
			Vector3 vector3 = vector2 / num;
			float num2 = Vector3.Dot(vector, vector3);
			float num3 = this.accel;
			if (this.maxDepth > -1f)
			{
				float num4 = Vector3.Dot(transform.position - this.enterPos, vector3);
				float num5 = this.maxDepth - num4;
				float num6 = 0f;
				if (num5 > 0.0001f)
				{
					num6 = num2 * num2 / num5;
				}
				num3 = Mathf.Max(this.accel, num6);
			}
			float deltaTime = Time.deltaTime;
			Vector3 vector4 = base.transform.forward * num3 * deltaTime;
			vector += vector4;
			Vector3 vector5 = Vector3.Dot(vector, base.transform.up) * base.transform.up;
			Vector3 vector6 = Vector3.Dot(vector, base.transform.right) * base.transform.right;
			Vector3 vector7 = Mathf.Clamp(Vector3.Dot(vector, base.transform.forward), -1f * this.maxSpeed, this.maxSpeed) * base.transform.forward;
			float num7 = 1f;
			float num8 = 1f;
			if (this.dampenLateralVelocity)
			{
				num7 = 1f - this.dampenXVelPerc * 0.01f * deltaTime;
				num8 = 1f - this.dampenYVelPerc * 0.01f * deltaTime;
			}
			vector = num8 * vector5 + num7 * vector6 + vector7;
			if (this.applyPullToCenterAcceleration && this.pullToCenterAccel > 0f && this.pullToCenterMaxSpeed > 0f)
			{
				vector -= num2 * vector3;
				if (num > this.pullTOCenterMinDistance)
				{
					num2 += this.pullToCenterAccel * deltaTime;
					float num9 = Mathf.Min(this.pullToCenterMaxSpeed, num / deltaTime);
					num2 = Mathf.Clamp(num2, -1f * num9, num9);
				}
				else
				{
					num2 = 0f;
				}
				vector += num2 * vector3;
			}
			if (this.scaleWithSize && sizeManager)
			{
				vector *= sizeManager.currentScale;
			}
			rigidbody.linearVelocity = vector;
		}

		// Token: 0x04007782 RID: 30594
		[SerializeField]
		public bool scaleWithSize = true;

		// Token: 0x04007783 RID: 30595
		[SerializeField]
		private float accel;

		// Token: 0x04007784 RID: 30596
		[SerializeField]
		private float maxDepth = -1f;

		// Token: 0x04007785 RID: 30597
		[SerializeField]
		private float maxSpeed;

		// Token: 0x04007786 RID: 30598
		[SerializeField]
		private bool disableGrip;

		// Token: 0x04007787 RID: 30599
		[SerializeField]
		private bool dampenLateralVelocity = true;

		// Token: 0x04007788 RID: 30600
		[SerializeField]
		private float dampenXVelPerc;

		// Token: 0x04007789 RID: 30601
		[FormerlySerializedAs("dampenZVelPerc")]
		[SerializeField]
		private float dampenYVelPerc;

		// Token: 0x0400778A RID: 30602
		[SerializeField]
		private bool applyPullToCenterAcceleration = true;

		// Token: 0x0400778B RID: 30603
		[SerializeField]
		private float pullToCenterAccel;

		// Token: 0x0400778C RID: 30604
		[SerializeField]
		private float pullToCenterMaxSpeed;

		// Token: 0x0400778D RID: 30605
		[SerializeField]
		private float pullTOCenterMinDistance = 0.1f;

		// Token: 0x0400778E RID: 30606
		private Collider volume;

		// Token: 0x0400778F RID: 30607
		public GameObject windSFX;

		// Token: 0x04007790 RID: 30608
		[SerializeField]
		private MeshRenderer windEffectRenderer;

		// Token: 0x04007791 RID: 30609
		private bool hasWindFX;

		// Token: 0x04007792 RID: 30610
		private Vector3 enterPos;
	}
}
