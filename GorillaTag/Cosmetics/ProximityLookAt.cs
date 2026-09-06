using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012EE RID: 4846
	public class ProximityLookAt : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x0600796F RID: 31087 RVA: 0x00279AF4 File Offset: 0x00277CF4
		private void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			if (this.transferableParent != null)
			{
				this.ownerRig = this.transferableParent.ownerRig;
			}
			if (this.ownerRig == null)
			{
				this.ownerRig = base.GetComponentInParent<VRRig>();
			}
			if (this.ownerRig == null)
			{
				this.ownerRig = GorillaTagger.Instance.offlineVRRig;
			}
			this.CacheSettings();
		}

		// Token: 0x06007970 RID: 31088 RVA: 0x00279B65 File Offset: 0x00277D65
		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			this.lookTarget = null;
			this.lastTargetSwitchTime = float.NegativeInfinity;
		}

		// Token: 0x06007971 RID: 31089 RVA: 0x00279B81 File Offset: 0x00277D81
		private void OnValidate()
		{
			this.CacheSettings();
		}

		// Token: 0x06007972 RID: 31090 RVA: 0x00279B89 File Offset: 0x00277D89
		private void CacheSettings()
		{
			this.normalizedLocalForward = ProximityLookAt.LocalAxisToVector(this.localForward);
			this.cosAngle = Mathf.Cos(this.targetSearchAngleDegrees * 0.017453292f);
			this.sqrRadius = this.lookRadius * this.lookRadius;
		}

		// Token: 0x06007973 RID: 31091 RVA: 0x00279BC8 File Offset: 0x00277DC8
		private static Vector3 LocalAxisToVector(ProximityLookAt.LocalAxis axis)
		{
			switch (axis)
			{
			case ProximityLookAt.LocalAxis.Forward:
				return Vector3.forward;
			case ProximityLookAt.LocalAxis.Back:
				return Vector3.back;
			case ProximityLookAt.LocalAxis.Right:
				return Vector3.right;
			case ProximityLookAt.LocalAxis.Left:
				return Vector3.left;
			case ProximityLookAt.LocalAxis.Up:
				return Vector3.up;
			case ProximityLookAt.LocalAxis.Down:
				return Vector3.down;
			default:
				return Vector3.forward;
			}
		}

		// Token: 0x06007974 RID: 31092 RVA: 0x00279C20 File Offset: 0x00277E20
		public void SliceUpdate()
		{
			Transform transform = this.FindTarget();
			if (transform == this.lookTarget)
			{
				return;
			}
			if (Time.time - this.lastTargetSwitchTime < this.targetSwitchCooldown)
			{
				return;
			}
			this.lookTarget = transform;
			this.lastTargetSwitchTime = Time.time;
		}

		// Token: 0x06007975 RID: 31093 RVA: 0x00279C6C File Offset: 0x00277E6C
		private void LateUpdate()
		{
			if (this.lookTransforms == null)
			{
				return;
			}
			Vector3 vector = base.transform.TransformDirection(this.normalizedLocalForward);
			for (int i = 0; i < this.lookTransforms.Length; i++)
			{
				Transform transform = this.lookTransforms[i];
				if (!(transform == null))
				{
					Vector3 vector2 = ((this.lookTarget != null) ? (this.lookTarget.position - transform.position).normalized : vector);
					vector2 = Vector3.RotateTowards(vector, vector2, this.lookAtAngleDegreeMax * 0.017453292f, 0f);
					if (this.pivotConstraint != null)
					{
						Vector3 vector3 = this.pivotConstraint.InverseTransformDirection(vector2);
						vector3.y = Mathf.Clamp(vector3.y, this.minPivotY, this.maxPivotY);
						vector2 = this.pivotConstraint.TransformDirection(vector3.normalized);
					}
					Vector3 vector4 = Vector3.RotateTowards(transform.rotation * Vector3.forward, vector2, this.rotSpeed * 0.017453292f * Time.deltaTime, 0f);
					transform.rotation = ((this.pivotConstraint != null) ? Quaternion.LookRotation(vector4, this.pivotConstraint.up) : Quaternion.LookRotation(vector4));
				}
			}
		}

		// Token: 0x06007976 RID: 31094 RVA: 0x00279DB8 File Offset: 0x00277FB8
		private Transform FindTarget()
		{
			if (!PhotonNetwork.InRoom)
			{
				return GorillaTagger.Instance.offlineVRRig.tagSound.transform;
			}
			Vector3 vector = base.transform.TransformDirection(this.normalizedLocalForward);
			float num = float.NegativeInfinity;
			Transform transform = null;
			foreach (VRRig vrrig in VRRigCache.ActiveRigs)
			{
				if (this.includeOwner || !(vrrig == this.ownerRig))
				{
					Vector3 vector2 = vrrig.tagSound.transform.position - base.transform.position;
					if (vector2.sqrMagnitude <= this.sqrRadius)
					{
						Vector3 normalized = vector2.normalized;
						float num2 = Vector3.Dot(vector, normalized);
						if (num2 >= this.cosAngle && num2 > num)
						{
							num = num2;
							transform = vrrig.tagSound.transform;
						}
					}
				}
			}
			return transform;
		}

		// Token: 0x04008A9E RID: 35486
		[Header("Settings")]
		[SerializeField]
		private Transform[] lookTransforms;

		// Token: 0x04008A9F RID: 35487
		[Tooltip("The local axis that points 'forward' on this transform.")]
		[SerializeField]
		private ProximityLookAt.LocalAxis localForward = ProximityLookAt.LocalAxis.Down;

		// Token: 0x04008AA0 RID: 35488
		[SerializeField]
		private float lookRadius = 0.5f;

		// Token: 0x04008AA1 RID: 35489
		[Tooltip("The cone angle in degrees used to detect nearby players.Only players within this angle of the forward direction are considered as targets.")]
		[SerializeField]
		private float targetSearchAngleDegrees = 60f;

		// Token: 0x04008AA2 RID: 35490
		[Tooltip("How far in degrees the transform can physically rotate from its rest position.Should be less than or equal to targetSearchAngleDegrees")]
		[SerializeField]
		private float lookAtAngleDegreeMax = 45f;

		// Token: 0x04008AA3 RID: 35491
		[SerializeField]
		private float rotSpeed = 180f;

		// Token: 0x04008AA4 RID: 35492
		[Tooltip("Seconds to hold the current target before switching to a new one")]
		[SerializeField]
		private float targetSwitchCooldown = 0.5f;

		// Token: 0x04008AA5 RID: 35493
		[Tooltip("Whether the cosmetic owner can be considered as a look target.")]
		[SerializeField]
		private bool includeOwner;

		// Token: 0x04008AA6 RID: 35494
		[Header("Pivot Clamping (Optional)")]
		[Tooltip("Assign a pivot transform to constrain rotation relative to it. Leave empty to skip clamping.")]
		[SerializeField]
		private Transform pivotConstraint;

		// Token: 0x04008AA7 RID: 35495
		[SerializeField]
		private float minPivotY = -1f;

		// Token: 0x04008AA8 RID: 35496
		[SerializeField]
		private float maxPivotY = 1f;

		// Token: 0x04008AA9 RID: 35497
		private TransferrableObject transferableParent;

		// Token: 0x04008AAA RID: 35498
		private VRRig ownerRig;

		// Token: 0x04008AAB RID: 35499
		private Transform lookTarget;

		// Token: 0x04008AAC RID: 35500
		private Vector3 normalizedLocalForward;

		// Token: 0x04008AAD RID: 35501
		private float cosAngle;

		// Token: 0x04008AAE RID: 35502
		private float sqrRadius;

		// Token: 0x04008AAF RID: 35503
		private float lastTargetSwitchTime = float.NegativeInfinity;

		// Token: 0x020012EF RID: 4847
		public enum LocalAxis
		{
			// Token: 0x04008AB1 RID: 35505
			Forward,
			// Token: 0x04008AB2 RID: 35506
			Back,
			// Token: 0x04008AB3 RID: 35507
			Right,
			// Token: 0x04008AB4 RID: 35508
			Left,
			// Token: 0x04008AB5 RID: 35509
			Up,
			// Token: 0x04008AB6 RID: 35510
			Down
		}
	}
}
