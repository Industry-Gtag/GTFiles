using System;
using GorillaTag.Reactions;
using UnityEngine;

namespace Cosmetics
{
	// Token: 0x020011D2 RID: 4562
	public class RaycastLineRenderer : MonoBehaviour, ITickSystemPost
	{
		// Token: 0x17000B3B RID: 2875
		// (get) Token: 0x06007404 RID: 29700 RVA: 0x0025BA73 File Offset: 0x00259C73
		// (set) Token: 0x06007405 RID: 29701 RVA: 0x0025BA7B File Offset: 0x00259C7B
		public bool PostTickRunning { get; set; }

		// Token: 0x06007406 RID: 29702 RVA: 0x0025BA84 File Offset: 0x00259C84
		private void Awake()
		{
			if (this.lineRenderer == null)
			{
				this.lineRenderer = base.GetComponentInChildren<LineRenderer>();
			}
			if (this.lineRenderer != null)
			{
				this.lineRenderer.useWorldSpace = true;
			}
		}

		// Token: 0x06007407 RID: 29703 RVA: 0x0025BABA File Offset: 0x00259CBA
		private void OnDisable()
		{
			this.DisableLine();
		}

		// Token: 0x06007408 RID: 29704 RVA: 0x0025BAC2 File Offset: 0x00259CC2
		public void EnableLine()
		{
			if (this.lineRenderer != null)
			{
				this.lineRenderer.enabled = true;
			}
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x06007409 RID: 29705 RVA: 0x0025BAE4 File Offset: 0x00259CE4
		public void DisableLine()
		{
			TickSystem<object>.RemovePostTickCallback(this);
			if (this.lineRenderer != null)
			{
				this.lineRenderer.enabled = false;
			}
			if (this.impactFx != null)
			{
				this.impactFx.SetActive(false);
			}
		}

		// Token: 0x0600740A RID: 29706 RVA: 0x0025BB20 File Offset: 0x00259D20
		public void PostTick()
		{
			this.UpdateLine();
		}

		// Token: 0x0600740B RID: 29707 RVA: 0x0025BB28 File Offset: 0x00259D28
		private void UpdateLine()
		{
			if (this.origin == null || this.lineRenderer == null)
			{
				return;
			}
			Vector3 position = this.origin.position;
			Vector3 rayDirection = this.GetRayDirection();
			bool flag = Physics.Raycast(position, rayDirection, out this.hit, this.maxDistance, this.hitLayers, QueryTriggerInteraction.Ignore);
			bool flag2 = Physics.Raycast(position, rayDirection, out this.impactHit, this.maxDistance, this.impactLayers, QueryTriggerInteraction.Ignore);
			Vector3 vector = (flag ? this.hit.point : (position + rayDirection * this.maxDistance));
			this.lineRenderer.positionCount = 2;
			this.lineRenderer.SetPosition(0, position);
			this.lineRenderer.SetPosition(1, vector);
			if (flag && this.surfaceEffectSpawner != null)
			{
				this.surfaceEffectSpawner.RequestSpawn(this.hit.point, this.hit.normal);
			}
			if (this.impactFx == null)
			{
				return;
			}
			if (flag2)
			{
				this.impactFx.transform.position = this.impactHit.point;
				if (this.orientImpactToSurface)
				{
					this.impactFx.transform.up = this.impactHit.normal;
				}
				if (!this.impactFx.activeSelf)
				{
					this.impactFx.SetActive(true);
					return;
				}
			}
			else if (this.impactFx.activeSelf)
			{
				this.impactFx.SetActive(false);
			}
		}

		// Token: 0x0600740C RID: 29708 RVA: 0x0025BCA4 File Offset: 0x00259EA4
		private Vector3 GetRayDirection()
		{
			if (this.directionSpace == RaycastLineRenderer.DirectionSpace.World)
			{
				switch (this.directionAxis)
				{
				case RaycastLineRenderer.CastAxis.Forward:
					return Vector3.forward;
				case RaycastLineRenderer.CastAxis.Back:
					return Vector3.back;
				case RaycastLineRenderer.CastAxis.Right:
					return Vector3.right;
				case RaycastLineRenderer.CastAxis.Left:
					return Vector3.left;
				case RaycastLineRenderer.CastAxis.Up:
					return Vector3.up;
				case RaycastLineRenderer.CastAxis.Down:
					return Vector3.down;
				default:
					return Vector3.forward;
				}
			}
			else
			{
				switch (this.directionAxis)
				{
				case RaycastLineRenderer.CastAxis.Forward:
					return this.origin.forward;
				case RaycastLineRenderer.CastAxis.Back:
					return -this.origin.forward;
				case RaycastLineRenderer.CastAxis.Right:
					return this.origin.right;
				case RaycastLineRenderer.CastAxis.Left:
					return -this.origin.right;
				case RaycastLineRenderer.CastAxis.Up:
					return this.origin.up;
				case RaycastLineRenderer.CastAxis.Down:
					return -this.origin.up;
				default:
					return this.origin.forward;
				}
			}
		}

		// Token: 0x040083A7 RID: 33703
		[Tooltip("Origin of the line. The ray is cast from this transform's position.")]
		[SerializeField]
		private Transform origin;

		// Token: 0x040083A8 RID: 33704
		[SerializeField]
		private RaycastLineRenderer.DirectionSpace directionSpace;

		// Token: 0x040083A9 RID: 33705
		[SerializeField]
		private RaycastLineRenderer.CastAxis directionAxis;

		// Token: 0x040083AA RID: 33706
		[Tooltip("Maximum length of the line in meters")]
		[SerializeField]
		private float maxDistance = 10f;

		// Token: 0x040083AB RID: 33707
		[SerializeField]
		private LayerMask hitLayers = 134218241;

		// Token: 0x040083AC RID: 33708
		[Tooltip("Line renderer drawn between the origin and the hit point")]
		[SerializeField]
		private LineRenderer lineRenderer;

		// Token: 0x040083AD RID: 33709
		[Tooltip("Object placed at the point of contact.\nThis must already live in the prefab")]
		[SerializeField]
		private GameObject impactFx;

		// Token: 0x040083AE RID: 33710
		[SerializeField]
		private LayerMask impactLayers = 134218241;

		// Token: 0x040083AF RID: 33711
		[Tooltip("Align the impact FX up (y+) axis to the surface normal at the hit point.")]
		[SerializeField]
		private bool orientImpactToSurface = true;

		// Token: 0x040083B0 RID: 33712
		[Tooltip("Needs to be in the object pool system")]
		[SerializeField]
		private SpawnWorldEffects surfaceEffectSpawner;

		// Token: 0x040083B1 RID: 33713
		private RaycastHit hit;

		// Token: 0x040083B2 RID: 33714
		private RaycastHit impactHit;

		// Token: 0x020011D3 RID: 4563
		private enum DirectionSpace
		{
			// Token: 0x040083B5 RID: 33717
			Local,
			// Token: 0x040083B6 RID: 33718
			World
		}

		// Token: 0x020011D4 RID: 4564
		private enum CastAxis
		{
			// Token: 0x040083B8 RID: 33720
			Forward,
			// Token: 0x040083B9 RID: 33721
			Back,
			// Token: 0x040083BA RID: 33722
			Right,
			// Token: 0x040083BB RID: 33723
			Left,
			// Token: 0x040083BC RID: 33724
			Up,
			// Token: 0x040083BD RID: 33725
			Down
		}
	}
}
