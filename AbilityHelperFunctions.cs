using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200072E RID: 1838
public static class AbilityHelperFunctions
{
	// Token: 0x06002EB2 RID: 11954 RVA: 0x000FF820 File Offset: 0x000FDA20
	public static float EaseOutPower(float t, float power)
	{
		return 1f - Mathf.Pow(1f - t, power);
	}

	// Token: 0x06002EB3 RID: 11955 RVA: 0x000FF838 File Offset: 0x000FDA38
	public static int RandomRangeUnique(int minInclusive, int maxExclusive, int lastValue)
	{
		int num = maxExclusive - minInclusive;
		if (num <= 1)
		{
			return minInclusive;
		}
		int num2 = Random.Range(minInclusive, maxExclusive);
		if (num2 != lastValue)
		{
			return num2;
		}
		return (num2 + 1) % num;
	}

	// Token: 0x06002EB4 RID: 11956 RVA: 0x000FF862 File Offset: 0x000FDA62
	public static int GetNavMeshWalkableArea()
	{
		if (AbilityHelperFunctions.navMeshWalkableArea == -1)
		{
			AbilityHelperFunctions.navMeshWalkableArea = NavMesh.GetAreaFromName("walkable");
		}
		return AbilityHelperFunctions.navMeshWalkableArea;
	}

	// Token: 0x06002EB5 RID: 11957 RVA: 0x000FF880 File Offset: 0x000FDA80
	public static Vector3? GetLocationToInvestigate(Vector3 listenerLocation, float hearingRadius, Vector3? currentInvestigationLocation)
	{
		GameNoiseEvent gameNoiseEvent;
		NavMeshHit navMeshHit;
		if (GRNoiseEventManager.instance.GetMostRecentNoiseEventInRadius(listenerLocation, hearingRadius, out gameNoiseEvent) && NavMesh.SamplePosition(gameNoiseEvent.position, out navMeshHit, 1f, AbilityHelperFunctions.GetNavMeshWalkableArea()))
		{
			return new Vector3?(navMeshHit.position);
		}
		if (currentInvestigationLocation != null)
		{
			return currentInvestigationLocation;
		}
		return null;
	}

	// Token: 0x04003BDD RID: 15325
	private static int navMeshWalkableArea = -1;
}
