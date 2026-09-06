using System;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace GorillaLocomotion
{
	// Token: 0x02001191 RID: 4497
	public sealed class Playspace : MonoBehaviour
	{
		// Token: 0x0600716C RID: 29036 RVA: 0x0024CBFD File Offset: 0x0024ADFD
		private void Awake()
		{
			this._sqrSphereRadius = this._sphereRadius * this._sphereRadius;
			this._sqrSnapToThreshold = this._snapToThreshold * this._snapToThreshold;
		}

		// Token: 0x0600716D RID: 29037 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void Start()
		{
		}

		// Token: 0x0600716E RID: 29038 RVA: 0x0024CC28 File Offset: 0x0024AE28
		private void Update()
		{
			Vector3 vector = this._localGorillaHead.transform.position - base.transform.position;
			float sqrMagnitude = vector.sqrMagnitude;
			if (GTPlayer.Instance.enableHoverMode || GTPlayer.Instance.isClimbing || vector.sqrMagnitude > this._sqrSnapToThreshold)
			{
				base.transform.position = this._localGorillaHead.transform.position;
				return;
			}
			Vector3 normalized = vector.normalized;
			vector = this.GetChaseSpeed() * Time.deltaTime * normalized;
			base.transform.position = ((vector.sqrMagnitude > sqrMagnitude) ? this._localGorillaHead.transform.position : (base.transform.position + vector));
			if ((this._localGorillaHead.transform.position - base.transform.position).sqrMagnitude > this._sqrSphereRadius)
			{
				this._localGorillaHead.transform.position = base.transform.position + this._sphereRadius * normalized;
			}
		}

		// Token: 0x0600716F RID: 29039 RVA: 0x0024CD53 File Offset: 0x0024AF53
		private float GetChaseSpeed()
		{
			return this._defaultChaseSpeed;
		}

		// Token: 0x06007170 RID: 29040 RVA: 0x0024CD5B File Offset: 0x0024AF5B
		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(base.transform.position, this._sphereRadius);
		}

		// Token: 0x040081D1 RID: 33233
		[SerializeField]
		private GameObject _localGorillaHead;

		// Token: 0x040081D2 RID: 33234
		[SerializeField]
		private float _sphereRadius;

		// Token: 0x040081D3 RID: 33235
		private float _sqrSphereRadius;

		// Token: 0x040081D4 RID: 33236
		[SerializeField]
		private float _defaultChaseSpeed;

		// Token: 0x040081D5 RID: 33237
		[SerializeField]
		private float _snapToThreshold;

		// Token: 0x040081D6 RID: 33238
		private float _sqrSnapToThreshold;

		// Token: 0x040081D7 RID: 33239
		[SerializeField]
		private GTPlayer m_gtPlayer;

		// Token: 0x040081D8 RID: 33240
		[SerializeField]
		private XROrigin m_xrOrigin;

		// Token: 0x040081D9 RID: 33241
		private Transform m_xrBody;
	}
}
