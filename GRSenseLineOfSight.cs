using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020007EE RID: 2030
[Serializable]
public class GRSenseLineOfSight
{
	// Token: 0x060033E2 RID: 13282 RVA: 0x0011CEEE File Offset: 0x0011B0EE
	public bool HasLineOfSight(Vector3 headPos, Vector3 targetPos)
	{
		return GRSenseLineOfSight.HasLineOfSight(headPos, targetPos, this.sightDist, this.visibilityMask.value, this.rayCastMode);
	}

	// Token: 0x060033E3 RID: 13283 RVA: 0x0011CF10 File Offset: 0x0011B110
	public static bool HasLineOfSight(Vector3 headPos, Vector3 targetPos, float sightDist, int layerMask, GRSenseLineOfSight.RaycastMode rayCastMode = GRSenseLineOfSight.RaycastMode.Geometry)
	{
		switch (rayCastMode)
		{
		case GRSenseLineOfSight.RaycastMode.Geometry:
			return GRSenseLineOfSight.HasGeoLineOfSight(headPos, targetPos, sightDist, layerMask);
		case GRSenseLineOfSight.RaycastMode.Navmesh:
			return GRSenseLineOfSight.HasNavmeshLineOfSight(headPos, targetPos, sightDist);
		case GRSenseLineOfSight.RaycastMode.GeometryAndNavMesh:
			return GRSenseLineOfSight.HasGeoLineOfSight(headPos, targetPos, sightDist, layerMask) && GRSenseLineOfSight.HasNavmeshLineOfSight(headPos, targetPos, sightDist);
		case GRSenseLineOfSight.RaycastMode.GeometryOrNavMesh:
			return GRSenseLineOfSight.HasNavmeshLineOfSight(headPos, targetPos, sightDist) || GRSenseLineOfSight.HasGeoLineOfSight(headPos, targetPos, sightDist, layerMask);
		default:
			return false;
		}
	}

	// Token: 0x060033E4 RID: 13284 RVA: 0x0011CF78 File Offset: 0x0011B178
	public static bool HasGeoLineOfSight(Vector3 headPos, Vector3 targetPos, float sightDist, int layerMask)
	{
		float num = Vector3.Distance(targetPos, headPos);
		return num <= sightDist && Physics.RaycastNonAlloc(new Ray(headPos, targetPos - headPos), GRSenseLineOfSight.visibilityHits, Mathf.Min(num, sightDist), layerMask, QueryTriggerInteraction.Ignore) < 1;
	}

	// Token: 0x060033E5 RID: 13285 RVA: 0x0011CFB8 File Offset: 0x0011B1B8
	public static bool HasNavmeshLineOfSight(Vector3 headPos, Vector3 targetPos, float sightDist)
	{
		NavMeshHit navMeshHit;
		NavMeshHit navMeshHit2;
		return (targetPos - headPos).sqrMagnitude <= sightDist * sightDist && NavMesh.SamplePosition(headPos, out navMeshHit, 1f, -1) && !NavMesh.Raycast(navMeshHit.position, targetPos, out navMeshHit2, -1);
	}

	// Token: 0x0400438C RID: 17292
	public float sightDist;

	// Token: 0x0400438D RID: 17293
	public LayerMask visibilityMask;

	// Token: 0x0400438E RID: 17294
	public GRSenseLineOfSight.RaycastMode rayCastMode;

	// Token: 0x0400438F RID: 17295
	public static RaycastHit[] visibilityHits = new RaycastHit[16];

	// Token: 0x020007EF RID: 2031
	public enum RaycastMode
	{
		// Token: 0x04004391 RID: 17297
		Geometry,
		// Token: 0x04004392 RID: 17298
		Navmesh,
		// Token: 0x04004393 RID: 17299
		GeometryAndNavMesh,
		// Token: 0x04004394 RID: 17300
		GeometryOrNavMesh
	}
}
