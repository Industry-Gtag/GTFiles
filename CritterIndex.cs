using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000047 RID: 71
public class CritterIndex : ScriptableObject
{
	// Token: 0x1700001F RID: 31
	public CritterConfiguration this[int index]
	{
		get
		{
			if (index < 0 || index >= this.critterTypes.Count)
			{
				return null;
			}
			return this.critterTypes[index];
		}
	}

	// Token: 0x0600012A RID: 298 RVA: 0x00006EC4 File Offset: 0x000050C4
	private void OnEnable()
	{
		CritterIndex._instance = this;
	}

	// Token: 0x0600012B RID: 299 RVA: 0x00006ECC File Offset: 0x000050CC
	public static Mesh GetMesh(CritterConfiguration.AnimalType animalType)
	{
		if (animalType < CritterConfiguration.AnimalType.Raccoon || animalType >= (CritterConfiguration.AnimalType)CritterIndex._instance.animalMeshes.Count)
		{
			return null;
		}
		return CritterIndex._instance.animalMeshes[(int)animalType].mesh;
	}

	// Token: 0x0600012C RID: 300 RVA: 0x00006EFB File Offset: 0x000050FB
	public int GetRandomCritterType(CrittersRegion region = null)
	{
		return this.critterTypes.IndexOf(this.GetRandomConfiguration(region));
	}

	// Token: 0x0600012D RID: 301 RVA: 0x00006F10 File Offset: 0x00005110
	public CritterConfiguration GetRandomConfiguration(CrittersRegion region = null)
	{
		WeightedList<CritterConfiguration> validCritterTypes = this.GetValidCritterTypes(region);
		if (validCritterTypes.Count == 0)
		{
			return null;
		}
		return validCritterTypes.GetRandomItem();
	}

	// Token: 0x0600012E RID: 302 RVA: 0x00006F35 File Offset: 0x00005135
	public static DateTime GetCritterDateTime()
	{
		if (!GorillaComputer.instance)
		{
			return DateTime.UtcNow;
		}
		return GorillaComputer.instance.GetServerTime();
	}

	// Token: 0x0600012F RID: 303 RVA: 0x00006F58 File Offset: 0x00005158
	private WeightedList<CritterConfiguration> GetValidCritterTypes(CrittersRegion region = null)
	{
		this._currentConfigs.Clear();
		DateTime critterDateTime = CritterIndex.GetCritterDateTime();
		foreach (CritterConfiguration critterConfiguration in this.critterTypes)
		{
			if (critterConfiguration.DateConditionsMet(critterDateTime) && critterConfiguration.CanSpawn(region))
			{
				this._currentConfigs.Add(critterConfiguration, critterConfiguration.spawnWeight);
			}
		}
		return this._currentConfigs;
	}

	// Token: 0x04000130 RID: 304
	public List<CritterIndex.AnimalTypeMeshEntry> animalMeshes = new List<CritterIndex.AnimalTypeMeshEntry>();

	// Token: 0x04000131 RID: 305
	public List<CritterConfiguration> critterTypes;

	// Token: 0x04000132 RID: 306
	private WeightedList<CritterConfiguration> _currentConfigs = new WeightedList<CritterConfiguration>();

	// Token: 0x04000133 RID: 307
	private static CritterIndex _instance;

	// Token: 0x02000048 RID: 72
	[Serializable]
	public class AnimalTypeMeshEntry
	{
		// Token: 0x04000134 RID: 308
		public CritterConfiguration.AnimalType animalType;

		// Token: 0x04000135 RID: 309
		public Mesh mesh;
	}
}
