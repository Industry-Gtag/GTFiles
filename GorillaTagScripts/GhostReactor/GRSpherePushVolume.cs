using System;
using System.Collections;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor
{
	// Token: 0x0200101F RID: 4127
	[RequireComponent(typeof(SphereCollider))]
	public class GRSpherePushVolume : MonoBehaviour
	{
		// Token: 0x060066AF RID: 26287 RVA: 0x0020FCCA File Offset: 0x0020DECA
		private void Awake()
		{
			this._collider = base.GetComponent<SphereCollider>();
			this._collider.enabled = false;
		}

		// Token: 0x060066B0 RID: 26288 RVA: 0x0020FCE4 File Offset: 0x0020DEE4
		public void Trigger()
		{
			this._collider.enabled = true;
			base.StartCoroutine(this.DisableCoroutine());
		}

		// Token: 0x060066B1 RID: 26289 RVA: 0x0020FD00 File Offset: 0x0020DF00
		private void OnTriggerStay(Collider other)
		{
			if (this._localFlung)
			{
				return;
			}
			if (this._coroutine != null)
			{
				return;
			}
			GRPlayer grplayer;
			if (!other.gameObject.TryGetComponentInParent(out grplayer))
			{
				return;
			}
			if (GRPlayer.GetLocal() != grplayer)
			{
				return;
			}
			this._coroutine = base.StartCoroutine(this.ActionCoroutine(other));
			this._collider.enabled = false;
		}

		// Token: 0x060066B2 RID: 26290 RVA: 0x0020FD5C File Offset: 0x0020DF5C
		private IEnumerator ActionCoroutine(Collider other)
		{
			yield return new WaitForSeconds(this._pushDelay);
			Vector3 vector = this.CalculatePushVector(other);
			GTPlayer.Instance.DoLaunch(vector);
			this._localFlung = true;
			yield return new WaitForSeconds(this._pushCooldown);
			this._localFlung = false;
			this._coroutine = null;
			yield break;
		}

		// Token: 0x060066B3 RID: 26291 RVA: 0x0020FD72 File Offset: 0x0020DF72
		private IEnumerator DisableCoroutine()
		{
			yield return new WaitForSeconds(this._disableAfter);
			this._collider.enabled = false;
			yield break;
		}

		// Token: 0x060066B4 RID: 26292 RVA: 0x0020FD84 File Offset: 0x0020DF84
		private Vector3 CalculatePushVector(Collider other)
		{
			GRSpherePushVolume.PushKind pushKind = this._pushKind;
			Vector3 vector;
			if (pushKind != GRSpherePushVolume.PushKind.Radial)
			{
				if (pushKind != GRSpherePushVolume.PushKind.UpAndOut)
				{
					throw new NotImplementedException();
				}
				vector = this.CalculateUpAndOutPushVector(other);
			}
			else
			{
				vector = this.CalculateRadialPushVector(other);
			}
			return vector;
		}

		// Token: 0x060066B5 RID: 26293 RVA: 0x0020FDBC File Offset: 0x0020DFBC
		private Vector3 CalculateRadialPushVector(Collider other)
		{
			Vector3 vector = other.gameObject.transform.position - base.transform.position;
			float num = vector.magnitude / this._collider.radius;
			return this._pushScaling.Evaluate(num) * this._pushForce * vector.normalized;
		}

		// Token: 0x060066B6 RID: 26294 RVA: 0x0020FE20 File Offset: 0x0020E020
		private Vector3 CalculateUpAndOutPushVector(Collider other)
		{
			Vector3 vector = new Vector3(other.gameObject.transform.position.x - base.transform.position.x, 0f, other.gameObject.transform.position.z - base.transform.position.z);
			float num = vector.magnitude / this._collider.radius;
			vector.Normalize();
			Vector3.RotateTowards(vector, Vector3.up, 0.7853982f, 0f);
			vector *= this._pushForce * this._pushScaling.Evaluate(num);
			return vector;
		}

		// Token: 0x04007598 RID: 30104
		[SerializeField]
		private GRSpherePushVolume.PushKind _pushKind;

		// Token: 0x04007599 RID: 30105
		[SerializeField]
		private float _pushDelay;

		// Token: 0x0400759A RID: 30106
		[SerializeField]
		private float _pushCooldown = 1f;

		// Token: 0x0400759B RID: 30107
		[SerializeField]
		private AnimationCurve _pushScaling = AnimationCurve.Constant(0f, 1f, 1f);

		// Token: 0x0400759C RID: 30108
		[SerializeField]
		private float _pushForce = 1f;

		// Token: 0x0400759D RID: 30109
		[SerializeField]
		private float _disableAfter = 3f;

		// Token: 0x0400759E RID: 30110
		private SphereCollider _collider;

		// Token: 0x0400759F RID: 30111
		private bool _localFlung;

		// Token: 0x040075A0 RID: 30112
		private Coroutine _coroutine;

		// Token: 0x02001020 RID: 4128
		public enum PushKind
		{
			// Token: 0x040075A2 RID: 30114
			Radial,
			// Token: 0x040075A3 RID: 30115
			UpAndOut
		}
	}
}
