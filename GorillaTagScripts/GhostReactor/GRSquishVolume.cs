using System;
using System.Collections;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor
{
	// Token: 0x02001023 RID: 4131
	public class GRSquishVolume : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x060066C4 RID: 26308 RVA: 0x000DF973 File Offset: 0x000DDB73
		private void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this);
		}

		// Token: 0x060066C5 RID: 26309 RVA: 0x000DF97B File Offset: 0x000DDB7B
		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this);
		}

		// Token: 0x060066C6 RID: 26310 RVA: 0x00210058 File Offset: 0x0020E258
		private void Start()
		{
			this.SetCollider(false);
			this.SetTentacleColliders(true);
			this.moonBoss = base.GetComponentInParent<GREnemyBossMoon>();
			if (this.moonBoss != null && !this.moonBoss.squishVolumes.Contains(this))
			{
				this.moonBoss.squishVolumes.Add(this);
			}
		}

		// Token: 0x060066C7 RID: 26311 RVA: 0x002100B4 File Offset: 0x0020E2B4
		public void SliceUpdate()
		{
			this.SetCollider(!this.overrideDisabled && base.transform.position.y < this.squishHeight && Vector3.Angle(-base.transform.forward, Quaternion.Euler(this.rotationOffset) * Vector3.down) < this.facingDownDegrees);
		}

		// Token: 0x060066C8 RID: 26312 RVA: 0x0021011C File Offset: 0x0020E31C
		public void SetCollider(bool colliderEnabled)
		{
			this._collider.enabled = colliderEnabled;
		}

		// Token: 0x060066C9 RID: 26313 RVA: 0x0021012C File Offset: 0x0020E32C
		private void OnTriggerEnter(Collider other)
		{
			GRPlayer grplayer;
			if (!other.gameObject.TryGetComponentInParent(out grplayer))
			{
				return;
			}
			if (GRPlayer.GetLocal() != grplayer)
			{
				return;
			}
			if (this._reenableCoroutine != null)
			{
				return;
			}
			this.SetTentacleColliders(false);
			GTPlayer.Instance.DoLaunch(this.GetLaunchVector());
			this.moonBoss.HitPlayer(GRPlayer.GetLocal(), false);
			this._reenableCoroutine = base.StartCoroutine(this.ReenableCoroutine());
			this.moonBoss.SetSquishVolumeState(false);
		}

		// Token: 0x060066CA RID: 26314 RVA: 0x002101A8 File Offset: 0x0020E3A8
		private void SetTentacleColliders(bool enabled)
		{
			Collider[] collidersToDisable = this._collidersToDisable;
			for (int i = 0; i < collidersToDisable.Length; i++)
			{
				collidersToDisable[i].enabled = enabled;
			}
		}

		// Token: 0x060066CB RID: 26315 RVA: 0x002101D3 File Offset: 0x0020E3D3
		private IEnumerator ReenableCoroutine()
		{
			yield return new WaitForSeconds(this._reenableDelay);
			this.SetTentacleColliders(true);
			this._reenableCoroutine = null;
			this.moonBoss.SetSquishVolumeState(true);
			yield break;
		}

		// Token: 0x060066CC RID: 26316 RVA: 0x002101E4 File Offset: 0x0020E3E4
		private Vector3 GetLaunchVector()
		{
			Vector3 position = GRPlayer.GetLocal().transform.position;
			Vector3 vector = position - base.transform.position;
			Vector3 vector2 = base.transform.position + base.transform.right * Vector3.Dot(vector, base.transform.right);
			Vector3 normalized = (position - vector2).normalized;
			Vector3 normalized2 = new Vector3(normalized.x, 0f, normalized.y).normalized;
			float num = Random.Range(this._launchDeflectionDegrees / 2f, this._launchDeflectionDegrees) * 0.017453292f;
			Vector3 vector3 = Vector3.RotateTowards(normalized2, Vector3.up, num, 0f);
			return this._launchStrength * vector3.normalized;
		}

		// Token: 0x040075AB RID: 30123
		[SerializeField]
		private Collider _collider;

		// Token: 0x040075AC RID: 30124
		[SerializeField]
		private Collider[] _collidersToDisable;

		// Token: 0x040075AD RID: 30125
		[SerializeField]
		private float _reenableDelay = 1f;

		// Token: 0x040075AE RID: 30126
		[SerializeField]
		private float _launchStrength = 8f;

		// Token: 0x040075AF RID: 30127
		[SerializeField]
		private float _launchDeflectionDegrees = 10f;

		// Token: 0x040075B0 RID: 30128
		private Coroutine _reenableCoroutine;

		// Token: 0x040075B1 RID: 30129
		private GREnemyBossMoon moonBoss;

		// Token: 0x040075B2 RID: 30130
		public float squishHeight;

		// Token: 0x040075B3 RID: 30131
		public Vector3 rotationOffset;

		// Token: 0x040075B4 RID: 30132
		public float facingDownDegrees = 20f;

		// Token: 0x040075B5 RID: 30133
		public bool overrideDisabled;
	}
}
