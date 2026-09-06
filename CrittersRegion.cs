using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000077 RID: 119
public class CrittersRegion : MonoBehaviour
{
	// Token: 0x17000037 RID: 55
	// (get) Token: 0x060002E1 RID: 737 RVA: 0x000116E3 File Offset: 0x0000F8E3
	public static List<CrittersRegion> Regions
	{
		get
		{
			return CrittersRegion._regions;
		}
	}

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x060002E2 RID: 738 RVA: 0x000116EA File Offset: 0x0000F8EA
	public int CritterCount
	{
		get
		{
			return this._critters.Count;
		}
	}

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x060002E3 RID: 739 RVA: 0x000116F7 File Offset: 0x0000F8F7
	// (set) Token: 0x060002E4 RID: 740 RVA: 0x000116FF File Offset: 0x0000F8FF
	public int ID { get; private set; }

	// Token: 0x060002E5 RID: 741 RVA: 0x00011708 File Offset: 0x0000F908
	private void OnEnable()
	{
		CrittersRegion.RegisterRegion(this);
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x00011710 File Offset: 0x0000F910
	private void OnDisable()
	{
		CrittersRegion.UnregisterRegion(this);
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x00011718 File Offset: 0x0000F918
	private static void RegisterRegion(CrittersRegion region)
	{
		CrittersRegion._regionLookup[region.ID] = region;
		CrittersRegion._regions.Add(region);
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x00011736 File Offset: 0x0000F936
	private static void UnregisterRegion(CrittersRegion region)
	{
		CrittersRegion._regionLookup.Remove(region.ID);
		CrittersRegion._regions.Remove(region);
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x00011758 File Offset: 0x0000F958
	public static void AddCritterToRegion(CrittersPawn critter, int regionId)
	{
		CrittersRegion crittersRegion;
		if (CrittersRegion._regionLookup.TryGetValue(regionId, out crittersRegion))
		{
			crittersRegion.AddCritter(critter);
			return;
		}
		GTDev.LogError<string>(string.Format("Attempted to add critter to non-existing region {0}.", regionId), null);
	}

	// Token: 0x060002EA RID: 746 RVA: 0x00011794 File Offset: 0x0000F994
	public static void RemoveCritterFromRegion(CrittersPawn critter)
	{
		CrittersRegion crittersRegion;
		if (CrittersRegion._regionLookup.TryGetValue(critter.regionId, out crittersRegion))
		{
			crittersRegion.RemoveCritter(critter);
			return;
		}
		GTDev.LogError<string>(string.Format("Couldn't find region with id {0}", critter.regionId), null);
	}

	// Token: 0x060002EB RID: 747 RVA: 0x000117D8 File Offset: 0x0000F9D8
	public void AddCritter(CrittersPawn pawn)
	{
		this._critters.Add(pawn);
	}

	// Token: 0x060002EC RID: 748 RVA: 0x000117E6 File Offset: 0x0000F9E6
	public void RemoveCritter(CrittersPawn pawn)
	{
		this._critters.Remove(pawn);
	}

	// Token: 0x060002ED RID: 749 RVA: 0x000117F8 File Offset: 0x0000F9F8
	public Vector3 GetSpawnPoint()
	{
		float num = this.scale / 2f;
		float num2 = base.transform.lossyScale.y * this.scale;
		Vector3 vector = base.transform.TransformPoint(new Vector3(Random.Range(-num, num), num, Random.Range(-num, num)));
		RaycastHit raycastHit;
		if (Physics.Raycast(vector, -base.transform.up, out raycastHit, num2, -1, QueryTriggerInteraction.Ignore))
		{
			Debug.DrawLine(vector, raycastHit.point, Color.green, 5f);
			return raycastHit.point;
		}
		Debug.DrawLine(vector, vector - base.transform.up * num2, Color.red, 5f);
		return vector;
	}

	// Token: 0x04000352 RID: 850
	private static List<CrittersRegion> _regions = new List<CrittersRegion>();

	// Token: 0x04000353 RID: 851
	private static Dictionary<int, CrittersRegion> _regionLookup = new Dictionary<int, CrittersRegion>();

	// Token: 0x04000354 RID: 852
	public CrittersBiome Biome = CrittersBiome.Any;

	// Token: 0x04000355 RID: 853
	public int maxCritters = 10;

	// Token: 0x04000356 RID: 854
	public float scale = 10f;

	// Token: 0x04000357 RID: 855
	public List<CrittersPawn> _critters = new List<CrittersPawn>();
}
