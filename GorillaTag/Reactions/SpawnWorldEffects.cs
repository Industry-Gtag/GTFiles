using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag.Reactions
{
	// Token: 0x02001258 RID: 4696
	public class SpawnWorldEffects : MonoBehaviour
	{
		// Token: 0x060076EB RID: 30443 RVA: 0x002691EC File Offset: 0x002673EC
		protected void OnEnable()
		{
			if (GorillaComputer.instance == null)
			{
				Debug.LogError("SpawnWorldEffects: Disabling because GorillaComputer not found! Hierarchy path: " + base.transform.GetPath(), this);
				base.enabled = false;
				return;
			}
			if (this._prefabToSpawn != null && !this._isPrefabInPool)
			{
				if (this._prefabToSpawn.CompareTag("Untagged"))
				{
					Debug.LogError("SpawnWorldEffects: Disabling because Spawn Prefab has no tag! Hierarchy path: " + base.transform.GetPath(), this);
					base.enabled = false;
					return;
				}
				this._isPrefabInPool = ObjectPools.instance.DoesPoolExist(this._prefabToSpawn);
				if (!this._isPrefabInPool)
				{
					Debug.LogError("SpawnWorldEffects: Disabling because Spawn Prefab not in pool! Hierarchy path: " + base.transform.GetPath(), this);
					base.enabled = false;
					return;
				}
				this._pool = ObjectPools.instance.GetPoolByObjectType(this._prefabToSpawn);
			}
			this._hasPrefabToSpawn = this._prefabToSpawn != null && this._isPrefabInPool;
		}

		// Token: 0x060076EC RID: 30444 RVA: 0x002692F0 File Offset: 0x002674F0
		public void RequestSpawn(Vector3 worldPosition)
		{
			this.RequestSpawn(worldPosition, Vector3.up);
		}

		// Token: 0x060076ED RID: 30445 RVA: 0x00269300 File Offset: 0x00267500
		public void RequestSpawn(Vector3 worldPosition, Vector3 normal)
		{
			if (this._maxParticleHitReactionRate < 1E-05f || !FireManager.hasInstance)
			{
				return;
			}
			double num = GTTime.TimeAsDouble();
			if ((float)(num - this._lastCollisionTime) < 1f / this._maxParticleHitReactionRate)
			{
				return;
			}
			if (this._hasPrefabToSpawn && this._isPrefabInPool && this._pool.GetInactiveCount() > 0)
			{
				Vector3 vector = normal;
				if (this._requireSurfaceLayer || this._useNormalOrientation)
				{
					Vector3 vector2;
					bool flag = this.TryGetSurfaceNormal(worldPosition, normal, out vector2);
					if (this._requireSurfaceLayer && !flag)
					{
						return;
					}
					if (this._useNormalOrientation)
					{
						vector = vector2;
					}
				}
				Quaternion? quaternion = null;
				if (this._forwardOrientationSource != null)
				{
					Vector3 vector3 = Vector3.ProjectOnPlane(SpawnWorldEffects.GetAxisVector(this._forwardOrientationSource, this._forwardSourceAxis), vector);
					if (vector3.sqrMagnitude < 0.0001f)
					{
						vector3 = Vector3.ProjectOnPlane((Mathf.Abs(vector.y) < 0.99f) ? Vector3.up : Vector3.right, vector);
					}
					quaternion = new Quaternion?(Quaternion.LookRotation(vector3.normalized, vector));
				}
				FireManager.SpawnFire(this._pool, worldPosition, vector, base.transform.lossyScale.x, quaternion);
			}
			this._lastCollisionTime = num;
		}

		// Token: 0x060076EE RID: 30446 RVA: 0x00269438 File Offset: 0x00267638
		private static Vector3 GetAxisVector(Transform source, SpawnWorldEffects.TransformAxis axis)
		{
			Vector3 vector;
			switch (axis)
			{
			case SpawnWorldEffects.TransformAxis.Forward:
				vector = source.forward;
				break;
			case SpawnWorldEffects.TransformAxis.Back:
				vector = -source.forward;
				break;
			case SpawnWorldEffects.TransformAxis.Right:
				vector = source.right;
				break;
			case SpawnWorldEffects.TransformAxis.Left:
				vector = -source.right;
				break;
			case SpawnWorldEffects.TransformAxis.Up:
				vector = source.up;
				break;
			case SpawnWorldEffects.TransformAxis.Down:
				vector = -source.up;
				break;
			default:
				vector = source.forward;
				break;
			}
			return vector;
		}

		// Token: 0x060076EF RID: 30447 RVA: 0x002694B4 File Offset: 0x002676B4
		private bool TryGetSurfaceNormal(Vector3 worldPosition, Vector3 hitNormal, out Vector3 surfaceNormal)
		{
			Vector3 vector2;
			if (this._raycastDirectionSource != null)
			{
				Vector3 vector = this._raycastDirectionSource.forward;
				if (this._raycastDirectionUseNegativeForward)
				{
					vector = -vector;
				}
				vector2 = ((vector.sqrMagnitude > 1E-06f) ? vector.normalized : Vector3.down);
			}
			else
			{
				vector2 = -((hitNormal.sqrMagnitude > 1E-06f) ? hitNormal.normalized : Vector3.up);
			}
			Vector3 vector3 = worldPosition + -vector2 * 0.05f;
			Vector3 vector4 = vector2;
			RaycastHit raycastHit;
			if (Physics.Raycast(vector3, vector4, out raycastHit, this._normalRaycastDistance + 0.05f, this._normalRaycastLayers, QueryTriggerInteraction.Ignore))
			{
				surfaceNormal = raycastHit.normal;
				return true;
			}
			surfaceNormal = hitNormal;
			return false;
		}

		// Token: 0x04008689 RID: 34441
		[Tooltip("The defaults are numbers for the flamethrower hair dryer.")]
		private readonly float _maxParticleHitReactionRate = 2f;

		// Token: 0x0400868A RID: 34442
		[Tooltip("Must be in the global object pool and have a tag.")]
		[SerializeField]
		private GameObject _prefabToSpawn;

		// Token: 0x0400868B RID: 34443
		[Tooltip("When enabled, a short raycast is fired from the spawn position to find the exact surface normal. The spawned object's Up vector will be aligned to that normal instead of world Up.")]
		[SerializeField]
		private bool _useNormalOrientation;

		// Token: 0x0400868C RID: 34444
		[Tooltip("When enabled, the spawn only happens if the surface raycast hits a collider on Normal Raycast Layers. If the raycast misses (surface not on an allowed layer, or out of range), the effect is not spawned. Independent of Use Normal Orientation.")]
		[SerializeField]
		private bool _requireSurfaceLayer;

		// Token: 0x0400868D RID: 34445
		[SerializeField]
		private float _normalRaycastDistance = 0.3f;

		// Token: 0x0400868E RID: 34446
		[SerializeField]
		private LayerMask _normalRaycastLayers = 134218241;

		// Token: 0x0400868F RID: 34447
		[Header("Raycast Direction Override")]
		[Tooltip("Optional. When assigned, the raycast used for normal-orientation will shoot along this transform's forward axis instead of along the incoming hit normal.")]
		[SerializeField]
		private Transform _raycastDirectionSource;

		// Token: 0x04008690 RID: 34448
		[Tooltip("If true, uses -forward instead of forward from Raycast Direction Source.")]
		[SerializeField]
		private bool _raycastDirectionUseNegativeForward;

		// Token: 0x04008691 RID: 34449
		[Header("Forward Orientation")]
		[Tooltip("Optional. When assigned, the spawned object's forward vector will be aligned to the chosen axis of this transform, projected onto the spawn surface.")]
		[SerializeField]
		private Transform _forwardOrientationSource;

		// Token: 0x04008692 RID: 34450
		[Tooltip("Which local axis of the Forward Orientation Source to use as the spawned object's forward.")]
		[SerializeField]
		private SpawnWorldEffects.TransformAxis _forwardSourceAxis;

		// Token: 0x04008693 RID: 34451
		private bool _hasPrefabToSpawn;

		// Token: 0x04008694 RID: 34452
		private bool _isPrefabInPool;

		// Token: 0x04008695 RID: 34453
		private double _lastCollisionTime;

		// Token: 0x04008696 RID: 34454
		private SinglePool _pool;

		// Token: 0x02001259 RID: 4697
		private enum TransformAxis
		{
			// Token: 0x04008698 RID: 34456
			Forward,
			// Token: 0x04008699 RID: 34457
			Back,
			// Token: 0x0400869A RID: 34458
			Right,
			// Token: 0x0400869B RID: 34459
			Left,
			// Token: 0x0400869C RID: 34460
			Up,
			// Token: 0x0400869D RID: 34461
			Down
		}
	}
}
